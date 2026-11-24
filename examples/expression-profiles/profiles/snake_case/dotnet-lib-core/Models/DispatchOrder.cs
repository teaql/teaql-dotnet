using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class DispatchOrder : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static class __TeaqlFields
        {
            internal const string ID_PROPERTY_MEMBER = "Id";
            internal const string ID_PROPERTY_QUERY = "id";
            internal const string ID_PROPERTY_STORAGE = "id";
            internal const string ID_PROPERTY_MODEL = "id";
            internal const string DISTRIBUTION_CENTER_PROPERTY_MEMBER = "DistributionCenter";
            internal const string DISTRIBUTION_CENTER_PROPERTY_QUERY = "distribution_center";
            internal const string DISTRIBUTION_CENTER_PROPERTY_STORAGE = "distribution_center";
            internal const string DISTRIBUTION_CENTER_PROPERTY_MODEL = "distribution_center";
            internal const string ORDER_REFERENCE_PROPERTY_MEMBER = "OrderReference";
            internal const string ORDER_REFERENCE_PROPERTY_QUERY = "order_reference";
            internal const string ORDER_REFERENCE_PROPERTY_STORAGE = "order_reference";
            internal const string ORDER_REFERENCE_PROPERTY_MODEL = "order_reference";
            internal const string HANDLING_NOTE_PROPERTY_MEMBER = "HandlingNote";
            internal const string HANDLING_NOTE_PROPERTY_QUERY = "handling_note";
            internal const string HANDLING_NOTE_PROPERTY_STORAGE = "handling_note";
            internal const string HANDLING_NOTE_PROPERTY_MODEL = "handling_note";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("DispatchOrder", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(DispatchOrder.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, DispatchOrder.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_STORAGE, () => global::Generated.Models.DistributionCenter._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(DispatchOrder.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, DispatchOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("snake_case"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static DispatchOrder FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(DispatchOrder.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(DispatchOrder.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER))
            {
                object? jsonDistributionCenter = IsLoaded("DistributionCenterEntity") && DistributionCenterEntity != null && DistributionCenterEntity.Id == DistributionCenter
                    ? DistributionCenterEntity : DistributionCenter;
                yield return new KeyValuePair<string, object?>(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_STORAGE, jsonDistributionCenter);
            }
            if (IsLoaded(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_STORAGE, OrderReference);
            }
            if (IsLoaded(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_STORAGE, HandlingNote);
            }
            if (IsLoaded(DispatchOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(DispatchOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("DispatchOrder", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal bool TeaqlCreationIntent => _teaqlForceCreate || _entityRoot.IsNew(TeaqlEntityKey());
        internal bool TeaqlHasPendingGraph(HashSet<object> visited)
        {
            if (!visited.Add(this)) return false;
            if (TeaqlCreationIntent || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey())) return true;
            return false;
        }
        internal bool TeaqlIsCancelledNew()
        {
            if (!_markedForDeletion || !TeaqlCreationIntent) return false;
            var key = TeaqlEntityKey();
            EntityCreation.RequireMutation(_creationState, Id, true, true, _loadedSnapshot);
            if (!_entityRoot.IsNew(key) || !_entityRoot.IsDeleted(key)
                || !_loadedSnapshot.CopyValues().TryGetValue("id", out var reservedId) || reservedId.TryI64() != Id)
                throw new EntityCreationException();
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance) { this };
            return true;
        }
        internal void AttachRoot(EntityRoot root, bool hydration = false)
        {
            if (!hydration && TeaqlIsCancelledNew()) return;
            var key = TeaqlEntityKey();
            if (!ReferenceEquals(root, _entityRoot) && (hydration || _entityRoot.HasPending(key)))
            {
                root.MergeEntityFrom(_entityRoot, key);
                _entityRoot = root;
            }
        }
        private static Value TeaqlValue(object? value) => value switch {
            null => new Value.NullValue(), string v => new Value.TextValue(v), bool v => new Value.BoolValue(v),
            double v => new Value.F64Value(v), decimal v => new Value.DecimalValue(v), DateTime v => new Value.TimestampValue(new DateTimeOffset(v).ToUnixTimeMilliseconds()), TimeSpan v => new Value.TimeValue(v),
            int v => new Value.I64Value(v), long v => new Value.I64Value(v), _ => throw new ArgumentException($"Unsupported TeaQL value type: {value.GetType().FullName}")
        };
        private static DateTime TeaqlDateTime(Value value) => value switch {
            Value.TimestampValue v => DateTimeOffset.FromUnixTimeMilliseconds(v.Milliseconds).UtcDateTime,
            Value.I64Value v => DateTimeOffset.FromUnixTimeMilliseconds(v.Value).UtcDateTime,
            Value.U64Value v => DateTimeOffset.FromUnixTimeMilliseconds(checked((long)v.Value)).UtcDateTime,
            Value.DateTimeValue v => v.Value,
            Value.DateValue v => v.Value,
            _ => Convert.ToDateTime(value.Raw)
        };
        private DispatchOrder() { _creationState = EntityCreationState.Hydrated; }
        internal static DispatchOrder TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new DispatchOrder();
            entity._creationState = EntityCreationState.Reserved;
            entity.TeaqlInitializeNewId(value);
            return entity;
        }
        internal void TeaqlValidateReservedIdentity(long expected)
        {
            EntityCreation.RequireReserved(_creationState, Id);
            if (Id != expected || !_entityRoot.IsNew(TeaqlEntityKey())) throw new EntityCreationException();
        }
                public long? Id { get; set; }
                public long? DistributionCenter { get; set; }
                public string? OrderReference { get; set; }
                public string? HandlingNote { get; set; }
                public long? Version { get; set; }
                public DistributionCenter? DistributionCenterEntity { get; set; }

        private string? _comment;
        private bool _markedForDeletion;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
        }

        internal void TeaqlInitializeGeneratedBootstrapId(long value)
            => TeaqlInitializeNewId(value);

        internal void TeaqlInitializeNewId(long value)
        {
            EntityCreation.RequireReserved(_creationState, value);
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "A positive identity is required");
            var oldKey = TeaqlEntityKey();
            Id = value;
            MarkLoaded("Id");
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            _entityRoot.Set(TeaqlEntityKey(), "id", new Value.I64Value(value));
            _entityRoot.MarkAsNew(TeaqlEntityKey());
            _teaqlForceCreate = true;
            _loadedSnapshot = TeaqlScalarSnapshot(LoadedScalarSnapshotOrigin.CreationBaseline);
        }

        // Fixed model seeds do not consume an unrelated allocator value.
        internal static DispatchOrder TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public DispatchOrder MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public DispatchOrder MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public DispatchOrder AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public DispatchOrder MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static DispatchOrder Refer(long id)
        {
            var entity = new DispatchOrder();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static DispatchOrder FromRecord(Record record)
        {
            var entity = new DispatchOrder().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue(DispatchOrder.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(DispatchOrder.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER, out var distributionCenterValue)
                        || record.TryGetValue(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_STORAGE, out distributionCenterValue))
                    {
                        entity.MarkLoaded(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER);
                        if (distributionCenterValue.Raw is Record distributionCenterRow)
                        {
                            entity.DistributionCenterEntity = global::Generated.Models.DistributionCenter.FromRecord(distributionCenterRow);
                            entity.DistributionCenter = entity.DistributionCenterEntity.Id;
                            entity.MarkLoaded("DistributionCenterEntity");
                        }
                        else if (distributionCenterValue.Raw is IEnumerable<Record> distributionCenterRows)
                        {
                            foreach (var row in distributionCenterRows)
                            {
                                entity.DistributionCenterEntity = global::Generated.Models.DistributionCenter.FromRecord(row);
                                entity.DistributionCenter = entity.DistributionCenterEntity.Id;
                                entity.MarkLoaded("DistributionCenterEntity");
                                break;
                            }
                        }
                        else if (distributionCenterValue.Raw == null)
                            entity.MarkLoaded("DistributionCenterEntity");
                        else if (distributionCenterValue.Raw != null)
                            entity.DistributionCenter = Convert.ToInt64(distributionCenterValue.Raw);
                    }
                    if (record.TryGetValue(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_STORAGE, out var orderReferenceValue))
                    {
                        entity.MarkLoaded(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_MEMBER);
                        if (orderReferenceValue.Raw != null)
                            entity.OrderReference = Convert.ToString(orderReferenceValue.Raw);
                    }
                    if (record.TryGetValue(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_STORAGE, out var handlingNoteValue))
                    {
                        entity.MarkLoaded(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_MEMBER);
                        if (handlingNoteValue.Raw != null)
                            entity.HandlingNote = Convert.ToString(handlingNoteValue.Raw);
                    }
                    if (record.TryGetValue(DispatchOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(DispatchOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY, DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY,
                DispatchOrder.__TeaqlFields.ID_PROPERTY_MEMBER, DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER, DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_MEMBER, DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_MEMBER, DispatchOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER,
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static DispatchOrder FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<DispatchOrder> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("DispatchOrder");
                if (!Id.HasValue || _teaqlForceCreate)
                {
                }
                TeaqlPrepareInputGraph(context, graph);
                graph.SealPreparedGraph();
                await global::System.Threading.Tasks.Task.CompletedTask;
            }, async graph =>
            {
                await TeaqlReserveGraphIdentitiesAsync(graph);
                TeaqlPreflightGraph(context, graph);
                return await TeaqlSaveWithinGraphAsync(context, graph);
            });
        }

        internal void TeaqlValidateCreationGraph(HashSet<object> visited, bool relationWillChange = false)
        {
            if (!visited.Add(this)) return;
            if (TeaqlIsCancelledNew()) return;
            EntityCreation.RequireMutation(_creationState, Id, TeaqlCreationIntent,
                TeaqlCreationIntent || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()) || relationWillChange,
                _loadedSnapshot);
        }

        internal void TeaqlPrepareInputGraph(UserContext context, GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Prepare)) return;
            if (TeaqlIsCancelledNew()) return;
            graph.DiscardRestoredFields("DispatchOrder", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(DispatchOrder.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "DispatchOrder", Location = ObjectLocation.Property(DispatchOrder.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "DispatchOrder", Location = ObjectLocation.Property(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "DispatchOrder", Location = ObjectLocation.Property(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "DispatchOrder", Location = ObjectLocation.Property(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(DispatchOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "DispatchOrder", Location = ObjectLocation.Property(DispatchOrder.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
            }
            var command = _markedForDeletion ? (object)ToDeleteCommand()
                : creating ? (object)ToInsertCommand() : (object)ToUpdateCommand();
            if (!creating && !_markedForDeletion)
            {
                ((UpdateCommand)command).Values = _entityRoot.Change(TeaqlEntityKey());
                var originalVersion = _entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version;
                if (originalVersion.HasValue) ((UpdateCommand)command).Values["version"] = new Value.I64Value(originalVersion.Value);
            }
            graph.PrepareInput(this, TeaqlMutationRequest(command, graph.Intent.Comment), TeaqlRestoreFixedValues);
            }
        }

        internal void TeaqlPreflightGraph(UserContext context, GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Preflight)) return;
            if (TeaqlIsCancelledNew()) return;
            if (!Id.HasValue || _teaqlForceCreate || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()))
                graph.PreflightPrepared(this);
        }

        internal async global::System.Threading.Tasks.Task TeaqlReserveGraphIdentitiesAsync(GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Reserve)) return;
            if (TeaqlIsCancelledNew()) return;
            var creating = TeaqlCreationIntent;
            if (creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()))
            {
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
            var teaqlOriginalCreationState = _creationState;
            var teaqlOriginalFullyLoaded = _fullyLoaded;
            var teaqlOriginalLoadedFields = new HashSet<string>(_loadedFields, StringComparer.OrdinalIgnoreCase);
            var teaqlOriginalId = this.Id;
            var teaqlOriginalDistributionCenter = this.DistributionCenter;
            var teaqlOriginalOrderReference = this.OrderReference;
            var teaqlOriginalHandlingNote = this.HandlingNote;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.DistributionCenter = teaqlOriginalDistributionCenter;
                this.OrderReference = teaqlOriginalOrderReference;
                this.HandlingNote = teaqlOriginalHandlingNote;
                this.Version = teaqlOriginalVersion;
                _ledgerId = teaqlOriginalLedgerId;
                _markedForDeletion = teaqlOriginalMarkedForDeletion;
                _teaqlForceCreate = teaqlOriginalForceCreate;
                _creationState = teaqlOriginalCreationState;
                _fullyLoaded = teaqlOriginalFullyLoaded;
                _loadedFields = teaqlOriginalLoadedFields;
                _entityRoot.Rekey(currentKey, teaqlOriginalKey);
            });
            graph.AfterCommit(() =>
            {
                _entityRoot.ClearEntity(TeaqlEntityKey());
                if (Version.HasValue) _entityRoot.AcceptCommittedVersion(TeaqlEntityKey(), Version.Value);
            });
            if (_markedForDeletion && creating)
                throw new InvalidOperationException("Cannot delete an entity without an id");
            if (creating)
            {
                EntityCreation.RequireReserved(_creationState, Id);
                graph.BindPreparedId(this, Id!.Value);
            }
            }
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<DispatchOrder> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("DispatchOrder",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("DispatchOrder",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for DispatchOrder");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.DistributionCenter = saved.DistributionCenter;
            this.OrderReference = saved.OrderReference;
            this.HandlingNote = saved.HandlingNote;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _teaqlForceCreate = false;
            _creationState = EntityCreationState.Hydrated;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            await TeaqlSaveChildrenAsync(context, graph, scope);
            return saved;
        }

        private async global::System.Threading.Tasks.Task TeaqlSaveChildrenAsync(UserContext context, GraphMutationSession graph, MutationTraceScope scope)
        {
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        private MutationRequest TeaqlMutationRequest(object command, string rootComment) => (command switch
        {
            InsertCommand insert => MutationRequest.Create(insert, rootComment, TeaqlEntityKey(), _entityRoot),
            UpdateCommand update => MutationRequest.Create(update, rootComment, TeaqlEntityKey(), _entityRoot),
            DeleteCommand delete => MutationRequest.Create(delete, rootComment, TeaqlEntityKey(), _entityRoot),
            _ => throw new InvalidOperationException("Unsupported mutation command")
        }).WithLoadedSnapshot(_loadedSnapshot);

        private LoadedScalarSnapshot TeaqlScalarSnapshot(LoadedScalarSnapshotOrigin origin = LoadedScalarSnapshotOrigin.Hydrated)
        {
            var record = new Record();
                        if (IsLoaded(DispatchOrder.__TeaqlFields.ID_PROPERTY_MEMBER)) record[DispatchOrder.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER)) record[DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_STORAGE] = TeaqlValue(DistributionCenter);
                        if (IsLoaded(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_MEMBER)) record[DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_STORAGE] = TeaqlValue(OrderReference);
                        if (IsLoaded(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_MEMBER)) record[DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_STORAGE] = TeaqlValue(HandlingNote);
                        if (IsLoaded(DispatchOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[DispatchOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_STORAGE, out var distributionCenterValue))
                        this.DistributionCenter = distributionCenterValue.Raw == null ? null : Convert.ToInt64(distributionCenterValue.Raw);

                    if (record.TryGetValue(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_STORAGE, out var orderReferenceValue))
                        this.OrderReference = orderReferenceValue.Raw == null ? null : Convert.ToString(orderReferenceValue.Raw);

                    if (record.TryGetValue(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_STORAGE, out var handlingNoteValue))
                        this.HandlingNote = handlingNoteValue.Raw == null ? null : Convert.ToString(handlingNoteValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[DispatchOrder.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (DistributionCenter.HasValue) record[DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_STORAGE] = new Value.I64Value(DistributionCenter.Value);

                    if (OrderReference != null) record[DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_STORAGE] = new Value.TextValue(OrderReference);

                    if (HandlingNote != null) record[DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_STORAGE] = new Value.TextValue(HandlingNote);

                    if (Version.HasValue) record[DispatchOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "DispatchOrder", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (DistributionCenter.HasValue) record[DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_STORAGE] = new Value.I64Value(DistributionCenter.Value);

                    if (OrderReference != null) record[DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_STORAGE] = new Value.TextValue(OrderReference);

                    if (HandlingNote != null) record[DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_STORAGE] = new Value.TextValue(HandlingNote);

                    if (Version.HasValue) record[DispatchOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "DispatchOrder", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : throw new InvalidOperationException("Update requires a loaded id"),
                ExpectedVersionValue = _entityRoot.OriginalVersion(TeaqlEntityKey()) ?? this.Version,
                Values = record 
            };
        }

        public DeleteCommand ToDeleteCommand()
        {
            if (!Id.HasValue || !Version.HasValue)
                throw new InvalidOperationException("Delete requires a loaded id and version");
            return new DeleteCommand {
                Entity = "DispatchOrder",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("DispatchOrder");
        }

                public DispatchOrder UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded(DispatchOrder.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), DispatchOrder.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public DispatchOrder UpdateDistributionCenter(long? value)
                {
                    if (this.DistributionCenter != value) this.DistributionCenterEntity = null;
                    this.DistributionCenter = value;
                    MarkLoaded(DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public DispatchOrder UpdateOrderReference(string value)
                {
                    this.OrderReference = value;
                    MarkLoaded(DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public DispatchOrder UpdateHandlingNote(string value)
                {
                    this.HandlingNote = value;
                    MarkLoaded(DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public DispatchOrder UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(DispatchOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), DispatchOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
                public DispatchOrder UpdateDistributionCenter(DistributionCenter value)
                {
                    UpdateDistributionCenter(value?.Id);
                    this.DistributionCenterEntity = value;
                    return this;
                }


                public DispatchOrder UpdateDistributionCenterId(long? value)
                {
                    return UpdateDistributionCenter(value);
                }

    }
}