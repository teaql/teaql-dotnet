using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{
    public readonly record struct OrderNumber
    {
        public string Value { get; }

        public OrderNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("order_number Business ID must not be blank", nameof(value));
            Value = value;
        }

        public override string ToString() => Value;

        public static BusinessIdDefinition Definition() => new(
            PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MODEL, "daily-permuted-v1", "ORD",
            "yyyyMMdd", "daily", 6,
            "-", "order_number", 1);
    }

    public class PurchaseOrder : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static class __TeaqlFields
        {
            internal const string ID_PROPERTY_MEMBER = "Id";
            internal const string ID_PROPERTY_QUERY = "id";
            internal const string ID_PROPERTY_STORAGE = "id";
            internal const string ID_PROPERTY_MODEL = "id";
            internal const string PLATFORM_PROPERTY_MEMBER = "Platform";
            internal const string PLATFORM_PROPERTY_QUERY = "platform";
            internal const string PLATFORM_PROPERTY_STORAGE = "platform";
            internal const string PLATFORM_PROPERTY_MODEL = "platform";
            internal const string SUPPLIER_PROPERTY_MEMBER = "Supplier";
            internal const string SUPPLIER_PROPERTY_QUERY = "supplier";
            internal const string SUPPLIER_PROPERTY_STORAGE = "supplier";
            internal const string SUPPLIER_PROPERTY_MODEL = "supplier";
            internal const string ORDER_NUMBER_PROPERTY_MEMBER = "OrderNumber";
            internal const string ORDER_NUMBER_PROPERTY_QUERY = "order_number";
            internal const string ORDER_NUMBER_PROPERTY_STORAGE = "order_number";
            internal const string ORDER_NUMBER_PROPERTY_MODEL = "order_number";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("PurchaseOrder", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(PurchaseOrder.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, PurchaseOrder.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, () => global::Generated.Models.Platform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static PurchaseOrder FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(PurchaseOrder.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(PurchaseOrder.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER))
            {
                object? jsonPlatform = IsLoaded("PlatformEntity") && PlatformEntity != null && PlatformEntity.Id == Platform
                    ? PlatformEntity : Platform;
                yield return new KeyValuePair<string, object?>(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, jsonPlatform);
            }
            if (IsLoaded(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_STORAGE, Supplier);
            }
            if (IsLoaded(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE, OrderNumber?.Value);
            }
            if (IsLoaded(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private sealed class OrderNumberSlot(PurchaseOrder entity) : IBusinessIdSlot
        {
            public string? CurrentValue => entity.OrderNumber?.Value;
            public bool IsNewAggregate => !entity.Id.HasValue || entity._teaqlForceCreate;
            public void AssignCanonicalValue(string value) =>
                entity.UpdateOrderNumber(new OrderNumber(value));
        }
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("PurchaseOrder", Id ?? _ledgerId);
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
           OrderNumber v => new Value.TextValue(v.Value),
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
        private PurchaseOrder() { _creationState = EntityCreationState.Hydrated; }
        internal static PurchaseOrder TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new PurchaseOrder();
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
                public long? Platform { get; set; }
                public string? Supplier { get; set; }
                public OrderNumber? OrderNumber { get; set; }
                public long? Version { get; set; }
                public Platform? PlatformEntity { get; set; }

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
        internal static PurchaseOrder TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public PurchaseOrder MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public PurchaseOrder MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public PurchaseOrder AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public PurchaseOrder MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static PurchaseOrder Refer(long id)
        {
            var entity = new PurchaseOrder();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static PurchaseOrder FromRecord(Record record)
        {
            var entity = new PurchaseOrder().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(PurchaseOrder.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, out var platformValue)
                        || record.TryGetValue(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, out platformValue))
                    {
                        entity.MarkLoaded(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER);
                        if (platformValue.Raw is Record platformRow)
                        {
                            entity.PlatformEntity = global::Generated.Models.Platform.FromRecord(platformRow);
                            entity.Platform = entity.PlatformEntity.Id;
                            entity.MarkLoaded("PlatformEntity");
                        }
                        else if (platformValue.Raw is IEnumerable<Record> platformRows)
                        {
                            foreach (var row in platformRows)
                            {
                                entity.PlatformEntity = global::Generated.Models.Platform.FromRecord(row);
                                entity.Platform = entity.PlatformEntity.Id;
                                entity.MarkLoaded("PlatformEntity");
                                break;
                            }
                        }
                        else if (platformValue.Raw == null)
                            entity.MarkLoaded("PlatformEntity");
                        else if (platformValue.Raw != null)
                            entity.Platform = Convert.ToInt64(platformValue.Raw);
                    }
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_STORAGE, out var supplierValue))
                    {
                        entity.MarkLoaded(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_MEMBER);
                        if (supplierValue.Raw != null)
                            entity.Supplier = Convert.ToString(supplierValue.Raw);
                    }
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE, out var orderNumberValue))
                    {
                        entity.MarkLoaded(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER);
                        if (orderNumberValue.Raw != null)
                            entity.OrderNumber = new OrderNumber(Convert.ToString(orderNumberValue.Raw)!);
                    }
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY, PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY,
                PurchaseOrder.__TeaqlFields.ID_PROPERTY_MEMBER, PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_MEMBER, PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER, PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER,
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static PurchaseOrder FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<PurchaseOrder> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("PurchaseOrder");
                if (!Id.HasValue || _teaqlForceCreate)
                {
                    var rootOrderNumber = context.RequireActiveRoot("Platform");
                    await context.EnsureBusinessIdAsync(
                        global::Generated.Models.OrderNumber.Definition(), rootOrderNumber.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        "purchase_order", new OrderNumberSlot(this));
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
            graph.DiscardRestoredFields("PurchaseOrder", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(PurchaseOrder.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "PurchaseOrder", Location = ObjectLocation.Property(PurchaseOrder.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "PurchaseOrder", Location = ObjectLocation.Property(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "PurchaseOrder", Location = ObjectLocation.Property(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "PurchaseOrder", Location = ObjectLocation.Property(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "PurchaseOrder", Location = ObjectLocation.Property(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            var teaqlOriginalPlatform = this.Platform;
            var teaqlOriginalSupplier = this.Supplier;
            var teaqlOriginalOrderNumber = this.OrderNumber;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Platform = teaqlOriginalPlatform;
                this.Supplier = teaqlOriginalSupplier;
                this.OrderNumber = teaqlOriginalOrderNumber;
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

        internal async global::System.Threading.Tasks.Task<PurchaseOrder> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("PurchaseOrder",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("PurchaseOrder",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for PurchaseOrder");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Platform = saved.Platform;
            this.Supplier = saved.Supplier;
            this.OrderNumber = saved.OrderNumber;
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
                        if (IsLoaded(PurchaseOrder.__TeaqlFields.ID_PROPERTY_MEMBER)) record[PurchaseOrder.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)) record[PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = TeaqlValue(Platform);
                        if (IsLoaded(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_MEMBER)) record[PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_STORAGE] = TeaqlValue(Supplier);
                        if (IsLoaded(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER)) record[PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE] = TeaqlValue(OrderNumber);
                        if (IsLoaded(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, out var platformValue))
                        this.Platform = platformValue.Raw == null ? null : Convert.ToInt64(platformValue.Raw);

                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_STORAGE, out var supplierValue))
                        this.Supplier = supplierValue.Raw == null ? null : Convert.ToString(supplierValue.Raw);

                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE, out var orderNumberValue))
                        this.OrderNumber = orderNumberValue.Raw == null ? null : new OrderNumber(Convert.ToString(orderNumberValue.Raw)!);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[PurchaseOrder.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (Platform.HasValue) record[PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(Platform.Value);

                    if (Supplier != null) record[PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_STORAGE] = new Value.TextValue(Supplier);

                    if (OrderNumber.HasValue) record[PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE] = new Value.TextValue(OrderNumber.Value.Value);

                    if (Version.HasValue) record[PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "PurchaseOrder", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Platform.HasValue) record[PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(Platform.Value);

                    if (Supplier != null) record[PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_STORAGE] = new Value.TextValue(Supplier);

                    if (OrderNumber.HasValue) record[PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE] = new Value.TextValue(OrderNumber.Value.Value);

                    if (Version.HasValue) record[PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "PurchaseOrder", 
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
                Entity = "PurchaseOrder",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("PurchaseOrder");
        }

                public PurchaseOrder UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded(PurchaseOrder.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), PurchaseOrder.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdatePlatform(long? value)
                {
                    if (this.Platform != value) this.PlatformEntity = null;
                    this.Platform = value;
                    MarkLoaded(PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateSupplier(string value)
                {
                    this.Supplier = value;
                    MarkLoaded(PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateOrderNumber(OrderNumber value)
                {
                    this.OrderNumber = value;
                    MarkLoaded(PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
                public PurchaseOrder UpdatePlatform(Platform value)
                {
                    UpdatePlatform(value?.Id);
                    this.PlatformEntity = value;
                    return this;
                }


                public PurchaseOrder UpdatePlatformId(long? value)
                {
                    return UpdatePlatform(value);
                }

    }
}