using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public static class MutationPolicyWarnings
{
    public const string MissingPolicy = "MUTATION-POLICY-001";
    public const string MissingApproval = "MUTATION-POLICY-002";
}

public sealed record MutationPolicyIdentity
{
    public string Id { get; }
    public string Version { get; }
    public string Fingerprint { get; }

    public MutationPolicyIdentity(string id, string version, string fingerprint)
    {
        Id = Required(id, nameof(id));
        Version = Required(version, nameof(version));
        Fingerprint = Required(fingerprint, nameof(fingerprint));
    }

    private static string Required(string value, string parameter) =>
        !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : throw new ArgumentException("Mutation policy identity values must not be blank", parameter);
}

public enum MutationOperationKind
{
    Create,
    Update,
    Delete,
    Recover
}

public sealed record MutationOperation(
    MutationOperationKind Kind,
    string Entity,
    Value? Id,
    long? OriginalVersion,
    IReadOnlyDictionary<string, Value> ChangedValues);

public sealed record MutationPlan(
    string ExecutionId,
    string RequestKey,
    string RootEntityType,
    string? AuditReason,
    IReadOnlyList<MutationOperation> Operations);

public enum MutationVerdict
{
    Allow,
    Deny
}

public sealed record MutationDecision(
    MutationVerdict Verdict,
    string? Code = null,
    string? Message = null,
    IReadOnlyList<string>? FieldPaths = null)
{
    public static MutationDecision Allowed() => new(MutationVerdict.Allow);

    public static MutationDecision Denied(string code, string message, params string[] fieldPaths) =>
        new(MutationVerdict.Deny,
            !string.IsNullOrWhiteSpace(code) ? code : throw new ArgumentException("A denial code is required", nameof(code)),
            message,
            Array.AsReadOnly(fieldPaths ?? Array.Empty<string>()));
}

public interface IMutationPolicy
{
    MutationPolicyIdentity Identity { get; }
    MutationDecision Review(UserContext context, MutationPlan plan);
}

public interface IMutationPolicyRegistry
{
    IMutationPolicy? Resolve(string requestKey);
}

public sealed class DelegatingMutationPolicyRegistry(Func<string, IMutationPolicy?> resolve)
    : IMutationPolicyRegistry
{
    public IMutationPolicy? Resolve(string requestKey) => resolve(requestKey);
}

public sealed record MutationPolicyApproval(
    MutationPolicyIdentity Policy,
    string ApprovedBy,
    DateTimeOffset ApprovedAt)
{
    public bool IsValidFor(MutationPolicyIdentity identity) =>
        Policy == identity && !string.IsNullOrWhiteSpace(ApprovedBy) && ApprovedAt != default;
}

public interface IMutationPolicyApprovalProvider
{
    MutationPolicyApproval? FindApproval(MutationPolicyIdentity identity);
}

public sealed class DelegatingMutationPolicyApprovalProvider(
    Func<MutationPolicyIdentity, MutationPolicyApproval?> find) : IMutationPolicyApprovalProvider
{
    public MutationPolicyApproval? FindApproval(MutationPolicyIdentity identity) => find(identity);
}

public enum MutationPolicySource
{
    GeneratedDefault,
    Customer
}

public enum MutationPolicyApprovalStatus
{
    NotApplicable,
    Missing,
    Approved
}

public sealed record MutationOperationSummary(
    MutationOperationKind Kind,
    string Entity,
    Value? Id,
    IReadOnlyList<string> ChangedFields);

public sealed record MutationGovernanceSnapshot(
    string ExecutionId,
    string RequestKey,
    MutationPolicySource Source,
    MutationPolicyIdentity? Policy,
    MutationPolicyApprovalStatus ApprovalStatus,
    IReadOnlyList<string> WarningCodes,
    IReadOnlyList<MutationOperationSummary> Operations);

public sealed record MutationGovernanceEvent(
    MutationGovernanceSnapshot Snapshot,
    string WarningCode,
    bool FirstOccurrence);

public interface IMutationGovernanceSink
{
    void OnWarning(UserContext context, MutationGovernanceEvent warning);
}

public sealed class DelegatingMutationGovernanceSink(
    Action<UserContext, MutationGovernanceEvent> warning) : IMutationGovernanceSink
{
    public void OnWarning(UserContext context, MutationGovernanceEvent item) => warning(context, item);
}

internal sealed class TextMutationGovernanceSink : IMutationGovernanceSink
{
    public void OnWarning(UserContext context, MutationGovernanceEvent warning)
    {
        if (!warning.FirstOccurrence) return;
        Console.Error.WriteLine(
            "[TeaQL Mutation Policy][WARN] code={0} requestKey={1} source={2} approval={3}",
            warning.WarningCode,
            warning.Snapshot.RequestKey,
            warning.Snapshot.Source,
            warning.Snapshot.ApprovalStatus);
    }
}

public sealed class MutationPolicyException(string message) : Exception(message);

public sealed class MutationPolicyRuntimeState
{
    private static long _executionSequence;
    private readonly ConcurrentDictionary<string, byte> _emittedWarnings = new(StringComparer.Ordinal);
    private readonly AsyncLocal<MutationGovernanceSnapshot?> _active = new();
    private readonly List<MutationOperation> _preflight = new();
    private string? _preflightRootEntity;
    private string? _preflightAuditReason;
    private bool _graphActive;
    private bool _graphReviewed;

    internal IMutationPolicyRegistry? Registry { get; set; }
    internal IMutationPolicyApprovalProvider? ApprovalProvider { get; set; }
    internal IMutationGovernanceSink WarningSink { get; set; } = new TextMutationGovernanceSink();
    internal MutationGovernanceSnapshot? Current => _active.Value;

    internal void BeginGraph(string comment)
    {
        _preflight.Clear();
        _preflightRootEntity = null;
        _preflightAuditReason = new MutationIntent(comment).Comment;
        _graphActive = true;
        _graphReviewed = false;
        _active.Value = null;
    }

    internal void EndGraph()
    {
        _preflight.Clear();
        _preflightRootEntity = null;
        _preflightAuditReason = null;
        _graphActive = false;
        _graphReviewed = false;
        _active.Value = null;
    }

    internal void RecordPreflight(MutationRequest request)
    {
        if (!_graphActive) return;
        if (_graphReviewed && Registry != null)
            throw new MutationPolicyException("Mutation preflight cannot add operations after customer policy review");
        _preflightRootEntity ??= RootEntityName(request);
        _preflightAuditReason ??= FirstComment(request);
        // Snapshot after CheckAndFix. A later entity/request mutation must not
        // change the plan already presented to the customer policy.
        _preflight.AddRange(ToOperations(request));
    }

    internal IDisposable EnterMutation(UserContext context, MutationRequest request)
    {
        if (_graphActive)
        {
            if (!_graphReviewed)
            {
                if (Registry != null && _preflight.Count == 0)
                    throw new MutationPolicyException(
                        "Customer mutation policy requires complete graph preflight before provider mutation");
                var plan = _preflight.Count == 0
                    ? BuildPlan(context, ToOperations(request), RootEntityName(request), FirstComment(request))
                    : BuildPlan(context, _preflight.ToArray(), _preflightRootEntity!, _preflightAuditReason);
                _active.Value = Review(context, plan);
                _graphReviewed = true;
            }
            return EmptyDisposable.Instance;
        }

        _active.Value = Review(context, BuildPlan(
            context,
            ToOperations(request),
            RootEntityName(request),
            FirstComment(request)));
        return new DelegateDisposable(() => _active.Value = null);
    }

    internal MutationGovernanceSnapshot Review(UserContext context, MutationPlan plan)
    {
        ValidatePlan(plan);
        var policy = Registry?.Resolve(plan.RequestKey);
        MutationPolicySource source;
        MutationPolicyIdentity? identity;
        MutationPolicyApprovalStatus approval;
        IReadOnlyList<string> warnings;
        if (policy == null)
        {
            source = MutationPolicySource.GeneratedDefault;
            identity = null;
            approval = MutationPolicyApprovalStatus.NotApplicable;
            warnings = Array.AsReadOnly(new[] { MutationPolicyWarnings.MissingPolicy });
        }
        else
        {
            identity = policy.Identity;
            var decision = policy.Review(context, ClonePlan(plan));
            if (decision.Verdict == MutationVerdict.Deny)
                throw new MutationPolicyException(
                    $"[MUTATION POLICY DENIED] {decision.Code ?? "MUTATION-POLICY-DENIED"}: {decision.Message ?? "mutation rejected"}");
            source = MutationPolicySource.Customer;
            var found = ApprovalProvider?.FindApproval(identity);
            approval = found?.IsValidFor(identity) == true
                ? MutationPolicyApprovalStatus.Approved
                : MutationPolicyApprovalStatus.Missing;
            warnings = approval == MutationPolicyApprovalStatus.Approved
                ? Array.Empty<string>()
                : Array.AsReadOnly(new[] { MutationPolicyWarnings.MissingApproval });
        }

        var operations = plan.Operations.Select(operation => new MutationOperationSummary(
            operation.Kind,
            operation.Entity,
            operation.Id,
            Array.AsReadOnly(operation.ChangedValues.Keys.OrderBy(field => field, StringComparer.Ordinal).ToArray())))
            .ToArray();
        var snapshot = new MutationGovernanceSnapshot(
            plan.ExecutionId,
            plan.RequestKey,
            source,
            identity,
            approval,
            warnings,
            Array.AsReadOnly(operations));
        foreach (var code in warnings) EmitWarning(context, snapshot, code);
        return snapshot;
    }

    private void EmitWarning(UserContext context, MutationGovernanceSnapshot snapshot, string code)
    {
        var identity = snapshot.Policy == null
            ? "none"
            : $"{snapshot.Policy.Id}:{snapshot.Policy.Version}:{snapshot.Policy.Fingerprint}";
        var first = _emittedWarnings.TryAdd($"{snapshot.RequestKey}|{identity}|{code}", 0);
        try { WarningSink.OnWarning(context, new MutationGovernanceEvent(snapshot, code, first)); }
        catch { /* warning delivery is fail-open for the business mutation */ }
    }

    private static MutationPlan BuildPlan(
        UserContext context,
        IReadOnlyList<MutationOperation> operations,
        string rootEntity,
        string? auditReason)
    {
        if (operations.Count == 0) throw new MutationPolicyException("Mutation plan must contain an operation");
        return new MutationPlan(
            $"{context.TraceId}-mutation-{Interlocked.Increment(ref _executionSequence)}",
            $"{rootEntity}.saveGraph",
            rootEntity,
            auditReason,
            Array.AsReadOnly(operations.Select(CloneOperation).ToArray()));
    }

    private static void ValidatePlan(MutationPlan plan)
    {
        if (string.IsNullOrWhiteSpace(plan.ExecutionId))
            throw new MutationPolicyException("Mutation plan execution id is required");
        if (string.IsNullOrWhiteSpace(plan.RequestKey))
            throw new MutationPolicyException("Mutation plan request key is required");
        if (string.IsNullOrWhiteSpace(plan.RootEntityType))
            throw new MutationPolicyException("Mutation plan root entity type is required");
        if (plan.Operations == null || plan.Operations.Count == 0)
            throw new MutationPolicyException("Mutation plan must contain an operation");
        if (plan.Operations.Any(operation => string.IsNullOrWhiteSpace(operation.Entity)))
            throw new MutationPolicyException("Mutation operation entity type is required");
    }

    private static MutationOperation ToOperation(MutationRequest request) => request switch
    {
        InsertMutationRequest insert => new MutationOperation(
            MutationOperationKind.Create,
            insert.Command.Entity,
            insert.Command.Values.TryGetValue("id", out var id) ? CloneValue(id) : null,
            null,
            ReadOnlyValues(insert.Command.Values)),
        UpdateMutationRequest update => new MutationOperation(
            MutationOperationKind.Update,
            update.Command.Entity,
            CloneValue(update.Command.Id),
            update.Command.ExpectedVersionValue,
            ReadOnlyValues(update.Command.Values)),
        DeleteMutationRequest delete => new MutationOperation(
            MutationOperationKind.Delete,
            delete.Command.Entity,
            CloneValue(delete.Command.Id),
            delete.Command.ExpectedVersionValue,
            ReadOnlyValues(new Record())),
        RecoverMutationRequest recover => new MutationOperation(
            MutationOperationKind.Recover,
            recover.Command.Entity,
            CloneValue(recover.Command.Id),
            recover.Command.ExpectedVersionValue,
            ReadOnlyValues(new Record())),
        _ => throw new MutationPolicyException($"Unsupported mutation request {request.GetType().Name}")
    };

    private static MutationOperation[] ToOperations(MutationRequest request) => request switch
    {
        BatchMutationRequest batch when batch.Requests.Count == 0 =>
            throw new MutationPolicyException("Mutation batch must contain an operation"),
        BatchMutationRequest batch => batch.Requests.SelectMany(ToOperations).ToArray(),
        _ => new[] { ToOperation(request) }
    };

    private static string RootEntityName(MutationRequest request) => request switch
    {
        InsertMutationRequest insert => insert.Command.Entity,
        UpdateMutationRequest update => update.Command.Entity,
        DeleteMutationRequest delete => delete.Command.Entity,
        RecoverMutationRequest recover => recover.Command.Entity,
        BatchMutationRequest batch when batch.Requests.Count != 0 => RootEntityName(batch.Requests[0]),
        BatchMutationRequest => throw new MutationPolicyException("Mutation batch must contain an operation"),
        _ => throw new MutationPolicyException($"Unsupported mutation request {request.GetType().Name}")
    };

    private static string FirstComment(MutationRequest request) => request.Comment;

    private static IReadOnlyDictionary<string, Value> ReadOnlyValues(Record values) =>
        new ReadOnlyDictionary<string, Value>(values.ToDictionary(
            pair => pair.Key,
            pair => CloneValue(pair.Value),
            StringComparer.Ordinal));

    private static MutationPlan ClonePlan(MutationPlan plan) => plan with
    {
        Operations = Array.AsReadOnly(plan.Operations.Select(CloneOperation).ToArray())
    };

    private static MutationOperation CloneOperation(MutationOperation operation) => operation with
    {
        Id = operation.Id == null ? null : CloneValue(operation.Id),
        ChangedValues = new ReadOnlyDictionary<string, Value>(operation.ChangedValues.ToDictionary(
            pair => pair.Key,
            pair => CloneValue(pair.Value),
            StringComparer.Ordinal))
    };

    private static Value CloneValue(Value value) => value switch
    {
        Value.JsonValue(var node) => new Value.JsonValue(node?.DeepClone()),
        Value.ObjectValue(var record) => new Value.ObjectValue(new Record(record.ToDictionary(
            pair => pair.Key,
            pair => CloneValue(pair.Value),
            StringComparer.Ordinal))),
        Value.ListValue(var values) => new Value.ListValue(values.Select(CloneValue).ToList()),
        _ => value
    };

    private sealed class DelegateDisposable(Action dispose) : IDisposable
    {
        private Action? _dispose = dispose;
        public void Dispose() => Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }

    private sealed class EmptyDisposable : IDisposable
    {
        public static readonly EmptyDisposable Instance = new();
        public void Dispose() { }
    }
}
