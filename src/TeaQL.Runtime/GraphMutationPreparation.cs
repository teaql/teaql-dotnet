using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

// Invocation-owned generated adapter state, never a Context or entity field.
public sealed partial class GraphMutationSession
{
    internal sealed class PreparedMutation(MutationRequest input)
    {
        internal MutationRequest Input { get; } = input;
        internal EntityKey Key { get; set; } = input.LedgerKey!;
        internal bool IdReserved { get; set; }
        internal MutationRequest? Final { get; set; }
        internal bool Executed { get; set; }
        internal Record LedgerValues { get; } = CopyValues(input.LedgerRoot!.Change(input.LedgerKey!));
        internal long? LedgerVersion { get; } = input.LedgerRoot!.OriginalVersion(input.LedgerKey!);
        internal bool LedgerNew { get; } = input.LedgerRoot!.IsNew(input.LedgerKey!);
        internal bool LedgerDeleted { get; } = input.LedgerRoot!.IsDeleted(input.LedgerKey!);
    }
    private readonly Dictionary<object, PreparedMutation> _prepared = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<EntityKey, Value> _assignedIds = new();
    private readonly Dictionary<MutationRequest, PreparedMutation> _preparedDispatch = new(ReferenceEqualityComparer.Instance);
    private bool _preparedWriteAdmitted;
    private bool _preparedGraphSealed;
    internal bool IsSealedEmpty => _preparedGraphSealed && _prepared.Count == 0;

    /// <summary>Called only after the generated traversal has visited every reached entity.</summary>
    public void SealPreparedGraph()
    {
        RequireActive();
        if (_transaction != null || _preparedGraphSealed)
            throw new InvalidOperationException("GRAPH_INPUT_PREPARATION_CLOSED");
        foreach (var entity in _prepared.Keys) ValidatePreparedInputUnchanged(entity);
        _preparedGraphSealed = true;
    }

    /// <summary>The requested graph root is still the policy entry point when only its children change.</summary>
    public void DeclareRootEntity(string entityType)
    {
        RequireActive();
        if (_transaction != null || _prepared.Count != 0)
            throw new InvalidOperationException("GRAPH_ROOT_MUST_PRECEDE_PREPARATION");
        _context.DeclareGraphMutationRoot(entityType);
    }

    /// <summary>Validate/fix input before opening a transaction; retain a private snapshot.</summary>
    public void PrepareInput(object entity, MutationRequest request)
    {
        RequireActive(); ArgumentNullException.ThrowIfNull(entity);
        if (_transaction != null || _preparedGraphSealed) throw new InvalidOperationException("GRAPH_INPUT_PREPARATION_CLOSED");
        if (_prepared.ContainsKey(entity)) throw new InvalidOperationException("GRAPH_ENTITY_ALREADY_PREPARED");
        if (request.LedgerKey == null || request.LedgerRoot == null)
            throw new InvalidOperationException("GRAPH_PREPARATION_REQUIRES_LEDGER");
        var entityType = request switch {
            InsertMutationRequest insert => insert.Command.Entity, UpdateMutationRequest update => update.Command.Entity,
            DeleteMutationRequest delete => delete.Command.Entity, RecoverMutationRequest recover => recover.Command.Entity,
            _ => throw new NotSupportedException("Graph preparation requires one typed entity mutation")
        };
        if (entityType != request.LedgerKey.EntityType)
            throw new InvalidOperationException("GRAPH_LEDGER_ENTITY_TYPE_MISMATCH");
        var captured = Request(CopyRequest(request));
        _context.ValidateGraphMutation(captured, this);
        _intentValues.AddRange(RuntimeDataService.PrivateMutationValues(captured, _context).Select(LoadedScalarSnapshot.CopyValue));
        _intentValues.AddRange(RuntimeDataService.LoadedPrivateValues(captured, _context).Select(LoadedScalarSnapshot.CopyValue));
        _context.CheckAndFix(captured);
        _intentValues.AddRange(RuntimeDataService.PrivateMutationValues(captured, _context).Select(LoadedScalarSnapshot.CopyValue));
        _prepared.Add(entity, new PreparedMutation(CopyRequest(captured)));
    }

    /// <summary>Reserve all graph identities after admission, before final policy preflight.</summary>
    public async Task<ulong> ReservePreparedIdAsync(object entity)
    {
        var prepared = Prepared(entity);
        if (_transaction == null) throw new InvalidOperationException("GRAPH_PREPARATION_CANNOT_ALLOCATE_ID");
        if (prepared.Input is not InsertMutationRequest insert)
            throw new InvalidOperationException("GRAPH_ID_RESERVATION_REQUIRES_CREATE");
        if (prepared.IdReserved) return prepared.Key.Id.TryU64()!.Value;
        var id = insert.Command.Values.TryGetValue("id", out var explicitId)
            ? explicitId.TryU64() ?? throw new InvalidOperationException("GRAPH_ID_MUST_BE_POSITIVE")
            : await AllocateIdAsync(insert.Command.Entity).ConfigureAwait(false);
        if (id == 0 || id > long.MaxValue) throw new InvalidOperationException("GRAPH_ID_MUST_BE_POSITIVE");
        prepared.Key = new EntityKey(insert.Command.Entity, checked((long)id));
        _assignedIds[prepared.Input.LedgerKey!] = prepared.Key.Id;
        var originalKey = prepared.Input.LedgerKey!;
        var assignedKey = prepared.Key;
        if (originalKey != assignedKey)
        {
            // Ledger identity is runtime state. Generated adapters may repeat
            // this idempotent rekey, but correctness must not depend on it.
            prepared.Input.LedgerRoot!.Rekey(originalKey, assignedKey);
            AfterRollback(() => prepared.Input.LedgerRoot!.Rekey(assignedKey, originalKey));
        }
        prepared.IdReserved = true;
        return id;
    }

    /// <summary>Bind typed graph references, check final values, then freeze the reviewed command.</summary>
    public void PreflightPrepared(object entity)
    {
        var prepared = Prepared(entity);
        if (_transaction == null) throw new InvalidOperationException("GRAPH_FINAL_PREFLIGHT_REQUIRES_TRANSACTION");
        if (prepared.Final != null) throw new InvalidOperationException("GRAPH_ENTITY_ALREADY_PREFLIGHTED");
        var captured = Request(CopyRequest(prepared.Input, prepared.Key));
        Record? values = captured switch
        {
            InsertMutationRequest insert => insert.Command.Values,
            UpdateMutationRequest update => update.Command.Values,
            _ => null
        };
        var entityType = prepared.Key.EntityType;
        if (captured is InsertMutationRequest)
        {
            if (!prepared.IdReserved) throw new InvalidOperationException("GRAPH_ID_NOT_RESERVED");
            values!["id"] = prepared.Key.Id;
        }
        if (values != null && _context.Metadata?.GetEntity(entityType) is { } metadata)
            foreach (var relation in metadata.Relations.Where(item => !item.IsMany && item.ForeignKeyValue == "id"))
                if (values.TryGetValue(relation.LocalKeyValue, out var oldId)
                    && _assignedIds.TryGetValue(new EntityKey(relation.TargetEntity, oldId), out var assigned))
                    values[relation.LocalKeyValue] = assigned;
        _context.ValidateGraphMutation(captured, this);
        _context.PreflightPreparedMutation(captured);
        if (captured is InsertMutationRequest &&
            (!values!.TryGetValue("id", out var checkedId) || checkedId.TryI64() != prepared.Key.Id.TryI64()))
            throw new CheckException(new[] {new CheckResult {RuleId = "invalid_type", EntityType = entityType,
                Location = ObjectLocation.Property("id"), Message = "Fix cannot change a reserved graph identity"}});
        _intentValues.AddRange(RuntimeDataService.PrivateMutationValues(captured, _context).Select(LoadedScalarSnapshot.CopyValue));
        _intentValues.AddRange(RuntimeDataService.LoadedPrivateValues(captured, _context).Select(LoadedScalarSnapshot.CopyValue));
        prepared.Final = CopyRequest(captured); // A checker may retain and later mutate its argument.
    }

    /// <summary>Execute exactly one final checked snapshot, with no additional Fix pass.</summary>
    public Task<MutationResult> MutatePreparedAsync(object entity, MutationTraceScope? scope = null)
    {
        var prepared = Prepared(entity);
        if (prepared.Final == null) throw new InvalidOperationException("GRAPH_ENTITY_NOT_PREFLIGHTED");
        if (_prepared.Values.Any(item => item.Final == null))
            throw new InvalidOperationException("GRAPH_PREFLIGHT_INCOMPLETE");
        if (prepared.Executed) throw new InvalidOperationException("GRAPH_ENTITY_ALREADY_EXECUTED");
        var service = _service ?? throw new InvalidOperationException("GRAPH_PREPARATION_CANNOT_MUTATE");
        var request = Request(CopyRequest(prepared.Final), scope);
        prepared.Executed = true;
        _preparedDispatch.Add(request, prepared);
        return service.MutateAsync(request);
    }

    internal PreparedMutation? ConsumePreparedMutation(MutationRequest request) =>
        _preparedDispatch.Remove(request, out var prepared) ? prepared : null;

    internal void ValidatePreparedMutationForWrite(PreparedMutation prepared)
    {
        RequireActive();
        if (!_preparedWriteAdmitted)
        {
            // The first policy callback can edit any reached node. Check the
            // whole plan once before its first write, not once per operation.
            ValidatePreparedGraphBeforeCommit();
            _preparedWriteAdmitted = true;
        }
        else ValidatePreparedLedgerUnchanged(prepared, identitiesBound: true);
    }

    internal void ValidatePreparedGraphBeforeCommit()
    {
        RequireActive();
        foreach (var prepared in _prepared.Values)
            ValidatePreparedLedgerUnchanged(prepared, identitiesBound: true);
    }

    private PreparedMutation Prepared(object entity)
    {
        RequireActive();
        return _prepared.TryGetValue(entity, out var prepared) ? prepared
            : throw new InvalidOperationException("GRAPH_ENTITY_NOT_PREPARED");
    }

    // Reaching one instance twice is legal only while its checked mutation intent
    // is unchanged. In particular, a child cannot silently move between parents
    // after its first Fix. This check is preparation-only: subsequent runtime ID
    // reservation/rekeying is intentional and must not be confused with an edit.
    private void ValidatePreparedInputUnchanged(object entity)
    {
        if (!_prepared.TryGetValue(entity, out var prepared)) return;
        ValidatePreparedLedgerUnchanged(prepared, identitiesBound: false);
    }

    private void ValidatePreparedLedgerUnchanged(PreparedMutation prepared, bool identitiesBound)
    {
        var ledger = prepared.Input.LedgerRoot!;
        var key = identitiesBound ? prepared.Key : prepared.Input.LedgerKey!;
        var current = ledger.Change(key);
        var fields = current.Keys.Concat(prepared.LedgerValues.Keys).Distinct(StringComparer.Ordinal);
        var changedField = fields.FirstOrDefault(field =>
            !current.TryGetValue(field, out var value)
            || !prepared.LedgerValues.TryGetValue(field, out var original)
            || !LoadedScalarSnapshot.SameValue(BoundField(field, value), BoundField(field, original)));
        if (changedField == null && ledger.OriginalVersion(key) == prepared.LedgerVersion
            && ledger.IsNew(key) == prepared.LedgerNew && ledger.IsDeleted(key) == prepared.LedgerDeleted) return;
        throw new CheckException(new[] {new CheckResult {
            RuleId = "mutation_changed_after_preparation", EntityType = key.EntityType,
            Location = changedField == null ? ObjectLocation.Root() : ObjectLocation.Property(changedField),
            Message = "Mutation intent changed after this entity was checked; prepare a new graph save"
        }});

        Value BoundField(string field, Value value)
        {
            if (identitiesBound && _context.Metadata?.GetEntity(key.EntityType) is { } metadata)
                foreach (var relation in metadata.Relations)
                    if (!relation.IsMany && relation.ForeignKeyValue == "id" && relation.LocalKeyValue == field
                        && _assignedIds.TryGetValue(new EntityKey(relation.TargetEntity, value), out var assigned))
                        return assigned;
            return value;
        }
    }

    private void ClearPreparedMutations()
    {
        _prepared.Clear(); _assignedIds.Clear(); _preparedDispatch.Clear(); _traversal.Clear(); _preparedWriteAdmitted = false;
    }

    private static Record CopyValues(Record values) => new LoadedScalarSnapshot(values).CopyValues();
    private static List<TraceNode> CopyTrace(IEnumerable<TraceNode> trace) => trace.Select(node => node with { }).ToList();
    private static MutationRequest CopyRequest(MutationRequest request, EntityKey? key = null)
    {
        var ledgerKey = key ?? request.LedgerKey;
        MutationRequest copy = request switch
        {
            InsertMutationRequest insert => new InsertMutationRequest(insert.Command with {
                Values = CopyValues(insert.Command.Values), TraceChain = CopyTrace(insert.Command.TraceChain)
            }, request.Comment) {LedgerKey = ledgerKey, LedgerRoot = request.LedgerRoot},
            UpdateMutationRequest update => new UpdateMutationRequest(update.Command with {
                Id = LoadedScalarSnapshot.CopyValue(update.Command.Id), Values = CopyValues(update.Command.Values),
                OldValues = update.Command.OldValues == null ? null : CopyValues(update.Command.OldValues),
                Guards = CopyValues(update.Command.Guards), TraceChain = CopyTrace(update.Command.TraceChain)
            }, request.Comment) {LedgerKey = ledgerKey, LedgerRoot = request.LedgerRoot},
            DeleteMutationRequest delete => new DeleteMutationRequest(delete.Command with {
                Id = LoadedScalarSnapshot.CopyValue(delete.Command.Id), Guards = CopyValues(delete.Command.Guards),
                TraceChain = CopyTrace(delete.Command.TraceChain)
            }, request.Comment) {LedgerKey = ledgerKey, LedgerRoot = request.LedgerRoot},
            RecoverMutationRequest recover => new RecoverMutationRequest(recover.Command with {
                Id = LoadedScalarSnapshot.CopyValue(recover.Command.Id), Guards = CopyValues(recover.Command.Guards),
                TraceChain = CopyTrace(recover.Command.TraceChain)
            }, request.Comment) {LedgerKey = ledgerKey, LedgerRoot = request.LedgerRoot},
            _ => throw new NotSupportedException("Graph preparation requires one typed entity mutation")
        };
        if (request.LoadedSnapshot != null) copy.WithLoadedSnapshot(request.LoadedSnapshot);
        return copy;
    }
}
