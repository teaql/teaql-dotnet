using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime;

/// <summary>
/// Pure enforcement of model-declared context-managed references. Applications
/// define their root's meaning; this layer contains no tenant policy or I/O.
/// </summary>
internal sealed class ContextRootAdmission
{
    private readonly UserContext _context;
    private readonly MutationRequest _request;
    private readonly string _entity;
    private readonly IReadOnlyDictionary<string, string> _fields;
    private readonly Record _original = new();
    private readonly bool _creating;

    private ContextRootAdmission(UserContext context, MutationRequest request, string entity,
        IReadOnlyDictionary<string, string> fields)
    {
        _context = context; _request = request; _entity = entity; _fields = fields;
        _creating = request is InsertMutationRequest;
        foreach (var field in fields.Keys)
            if (request.LoadedSnapshot?.TryOriginal(field, out var value) == true) _original[field] = value;
            else if (request is UpdateMutationRequest update && update.Command.OldValues?.TryGetValue(field, out value) == true)
                _original[field] = value;
    }

    internal static ContextRootAdmission? Capture(UserContext context, MutationRequest request)
    {
        var entity = request switch {
            InsertMutationRequest insert => insert.Command.Entity,
            UpdateMutationRequest update => update.Command.Entity,
            DeleteMutationRequest delete => delete.Command.Entity,
            RecoverMutationRequest recover => recover.Command.Entity,
            _ => null
        };
        return entity != null && context.ContextRootFields.TryGetValue(entity, out var fields) && fields.Count != 0
            ? new ContextRootAdmission(context, request, entity, fields) : null;
    }

    internal void Validate(bool final = false)
    {
        foreach (var (field, targetType) in _fields)
        {
            ContextEntityRef root;
            try { root = _context.RequireActiveRoot(targetType); }
            catch (ContextRootException) { Reject(field, "context_root_missing"); throw; }
            if (root.Id <= 0) Reject(field, "context_root_missing");
            var seen = false;
            void Inspect(Value value)
            {
                if (_creating && value is (Value.NullValue or Value.TypedNullValue)) return;
                seen = true;
                var valid = value switch {
                    Value.I64Value number => number.Value > 0 && number.Value == root.Id,
                    Value.U64Value number => number.Value > 0 && number.Value <= long.MaxValue && number.Value == (ulong)root.Id,
                    _ => false
                };
                if (!valid) Reject(field, "context_root_mismatch");
            }
            if (_original.TryGetValue(field, out var original)) Inspect(original);
            if (_request.LoadedSnapshot?.TryOriginal(field, out var currentOriginal) == true) Inspect(currentOriginal);
            else if (_original.ContainsKey(field) && _request.LoadedSnapshot != null) Reject(field, "context_root_missing");
            var pending = _request switch {
                InsertMutationRequest insert => insert.Command.Values,
                UpdateMutationRequest update => update.Command.Values,
                _ => null
            };
            if (pending?.TryGetValue(field, out var value) == true) Inspect(value);
            // Only the pre-Fix create phase may omit a contextual default.
            if (!seen && (!_creating || final)) Reject(field, "context_root_missing");
        }
    }

    internal void ApplyCreateDefaults()
    {
        if (_request is not InsertMutationRequest insert) return;
        foreach (var (field, targetType) in _fields)
            if (!insert.Command.Values.TryGetValue(field, out var value)
                || value is Value.NullValue or Value.TypedNullValue)
            {
                insert.Command.Values[field] = new Value.I64Value(_context.RequireActiveRoot(targetType).Id);
                _context.RecordFixEvidence(new FixEvidence(_entity, field,
                    FixEvidenceSource.Context, "activeRoot:" + targetType));
            }
    }

    private void Reject(string field, string rule)
    {
        // Root IDs do not need to cross the diagnostic boundary.
        var results = new[] { new CheckResult { RuleId = rule, EntityType = _entity, Location = ObjectLocation.Property(field) } };
        throw new CheckException(_context.TranslateCheckResults(results));
    }
}
