using System.Security.Cryptography;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public sealed record ContextFixInput(string Entity, string Field, string Function,
    IReadOnlyList<string> Arguments, string Operation, DateTimeOffset Now);

/// <summary>Trusted synchronous SPI. Prepare external/async inputs before save.</summary>
public interface IContextFixResolver
{
    Value Resolve(UserContext context, ContextFixInput input);
}

/// <summary>Context assignments supplement a frozen, per-entity business intent.</summary>
public static class ContextFix
{
    private static readonly AsyncLocal<Frame?> Active = new();

    internal static string Entity(MutationRequest request) => request switch {
        InsertMutationRequest insert => insert.Command.Entity,
        UpdateMutationRequest update => update.Command.Entity, _ => ""
    };
    internal static Record? Values(MutationRequest request) => request switch {
        InsertMutationRequest insert => insert.Command.Values,
        UpdateMutationRequest update => update.Command.Values, _ => null
    };
    internal static PropertyDescriptor[] Fields(UserContext context, MutationRequest request) =>
        context.Metadata?.GetEntity(Entity(request))?.Properties.Where(property =>
            property.CreateFunction != null || property.UpdateFunction != null).ToArray() ?? [];

    internal static Frame Open(UserContext context, MutationRequest request) => new(context, request);

    internal sealed class Frame : IDisposable
    {
        internal readonly UserContext Context;
        internal readonly MutationRequest Request;
        internal readonly PropertyDescriptor[] Fields;
        internal readonly bool Allowed;
        private readonly Frame? _parent;
        private readonly Record? _baseline;
        private readonly Record _before;
        internal bool Closed;
        private bool _completed;

        internal Frame(UserContext context, MutationRequest request)
        {
            Context = context; Request = request; Fields = ContextFix.Fields(context, request);
            var values = Values(request); _parent = Active.Value;
            _before = values == null ? new() : new LoadedScalarSnapshot(new Record(values
                .Where(item => Fields.Any(field => field.Name == item.Key)).ToDictionary(item => item.Key, item => item.Value))).CopyValues();
            var old = (Request as UpdateMutationRequest)?.Command.OldValues;
            if (old == null && Request.LoadedSnapshot is { Origin: LoadedScalarSnapshotOrigin.Hydrated } snapshot
                && Request.LedgerKey is { } key && snapshot.Owner == key
                && snapshot.TryOriginal("id", out var id) && LoadedScalarSnapshot.SameValue(id, key.Id)
                && snapshot.TryOriginal("version", out var version) && version is Value.I64Value originalVersion
                && Request.LedgerRoot?.OriginalVersion(key) == originalVersion.Value)
                old = snapshot.CopyValues();
            _baseline = old == null ? null : new LoadedScalarSnapshot(old).CopyValues();
            Allowed = HasBusinessChange();
            Active.Value = this;
        }
        private bool HasBusinessChange()
        {
            return Request is InsertMutationRequest || Request is UpdateMutationRequest && Values(Request) is { } values
                && values.Any(item => item.Key is not "id" and not "version" && !item.Key.StartsWith('_')
                    && !Fields.Any(field => field.Name == item.Key)
                    && (_baseline == null || !_baseline.TryGetValue(item.Key, out var original)
                        || !LoadedScalarSnapshot.SameValue(item.Value, original)));
        }
        internal void NormalizeFinalIntent()
        {
            if (Request is UpdateMutationRequest && !HasBusinessChange()) Restore();
        }
        private void Restore()
        {
            if (Values(Request) is not { } values) return;
            foreach (var field in Fields) {
                if (_before.TryGetValue(field.Name, out var value)) values[field.Name] = LoadedScalarSnapshot.CopyValue(value);
                else values.Remove(field.Name);
            }
        }
        internal void Complete() => _completed = true;
        public void Dispose()
        {
            if (Closed) return;
            try {
                if (!_completed) Restore();
            }
            finally { Closed = true; Active.Value = _parent; }
        }
    }

    public static bool Allowed(UserContext context, MutationRequest request) =>
        Current(context, request) is { Allowed: true };

    private static Frame? Current(UserContext context, MutationRequest request) =>
        Active.Value is { Closed: false } frame && ReferenceEquals(frame.Context, context)
            && ReferenceEquals(frame.Request, request) ? frame : null;

    public static void Apply(UserContext context, MutationRequest request, DateTimeOffset now,
        string field, string function, IReadOnlyList<string>? arguments = null, string? locationName = null)
    {
        var frame = Current(context, request);
        if (frame == null) throw Invalid();
        if (!frame.Allowed) return;
        var descriptor = frame.Fields.FirstOrDefault(property => property.Name == field);
        if (descriptor == null || (request is InsertMutationRequest ? descriptor.CreateFunction : descriptor.UpdateFunction) != function)
            throw Invalid();
        Value? result = null;
        var failed = false;
        try {
            var args = Array.AsReadOnly((arguments ?? []).ToArray());
            result = context.GetResource<IContextFixResolver>() is { } resolver
                ? resolver.Resolve(context, new(Entity(request), field, function, args,
                    request is InsertMutationRequest ? "insert" : "update", now))
                : function switch {
                    "now" => new Value.TimestampValue(now.ToUnixTimeMilliseconds()),
                    "uniStr" => new Value.TextValue((args.Count > 0 ? args[0] : "") + Convert.ToHexString(RandomNumberGenerator.GetBytes(16))),
                    _ => throw new InvalidOperationException("Context function requires a resolver")
                };
            if (!Valid(descriptor, result)) failed = true;
        }
        catch (Exception) { failed = true; }
        // Never chain the provider exception or serialize raw provider values.
        if (failed) throw Invalid();
        Values(request)![field] = LoadedScalarSnapshot.CopyValue(result!);
        context.RecordFixEvidence(new(Entity(request), locationName ?? field,
            function == "now" ? FixEvidenceSource.Clock : FixEvidenceSource.Context, "contextFix:" + function));

        CheckException Invalid() => new([new CheckResult { RuleId = "invalid_type", EntityType = Entity(request),
            Location = ObjectLocation.Property(locationName ?? field), Message = "Context Fix failed for this field" }]);
    }

    private static bool Valid(PropertyDescriptor field, Value? value) => value switch {
        null => false,
        Value.NullValue => field.Nullable,
        Value.TypedNullValue item => field.Nullable && item.Type == field.DataType,
        Value.BoolValue => field.DataType == DataType.Bool,
        Value.I64Value item => field.DataType == DataType.I64 && (!field.IsInt32 || item.Value is >= int.MinValue and <= int.MaxValue),
        Value.U64Value => field.DataType == DataType.U64,
        Value.F64Value item => field.DataType == DataType.F64 && double.IsFinite(item.Value),
        Value.DecimalValue => field.DataType == DataType.Decimal,
        Value.TextValue => field.DataType is DataType.Text or DataType.LargeText,
        Value.TimestampValue item => field.DataType == DataType.Timestamp
            && item.Milliseconds >= DateTimeOffset.MinValue.ToUnixTimeMilliseconds()
            && item.Milliseconds <= DateTimeOffset.MaxValue.ToUnixTimeMilliseconds(),
        Value.DateTimeValue => field.DataType == DataType.Timestamp,
        Value.DateValue => field.DataType == DataType.Date,
        Value.TimeValue => field.DataType == DataType.Time,
        Value.JsonValue item => field.DataType == DataType.Json && JsonValid(item),
        _ => false
    };

    private static bool JsonValid(Value.JsonValue item) { _ = item.Value?.ToJsonString(); return true; }
}
