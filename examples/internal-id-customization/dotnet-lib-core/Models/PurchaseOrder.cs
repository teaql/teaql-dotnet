using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class PurchaseOrder : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static class __TeaqlFields
        {
            internal const string ID_PROPERTY_MEMBER = "Id";
            internal const string ID_PROPERTY_QUERY = "id";
            internal const string ID_PROPERTY_STORAGE = "id";
            internal const string ID_PROPERTY_MODEL = "id";
            internal const string WAREHOUSE_PROPERTY_MEMBER = "Warehouse";
            internal const string WAREHOUSE_PROPERTY_QUERY = "warehouse";
            internal const string WAREHOUSE_PROPERTY_STORAGE = "warehouse";
            internal const string WAREHOUSE_PROPERTY_MODEL = "warehouse";
            internal const string REFERENCE_PROPERTY_MEMBER = "Reference";
            internal const string REFERENCE_PROPERTY_QUERY = "reference";
            internal const string REFERENCE_PROPERTY_STORAGE = "reference";
            internal const string REFERENCE_PROPERTY_MODEL = "reference";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("PurchaseOrder", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(PurchaseOrder.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, PurchaseOrder.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_STORAGE, () => global::Generated.Models.Warehouse._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField("order_item_list", global::TeaQL.Runtime.EntityJsonKind.Many, "OrderItemList", () => global::Generated.Models.OrderItem._teaqlJsonSchema)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

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
            if (IsLoaded(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER))
            {
                object? jsonWarehouse = IsLoaded("WarehouseEntity") && WarehouseEntity != null && WarehouseEntity.Id == Warehouse
                    ? WarehouseEntity : Warehouse;
                yield return new KeyValuePair<string, object?>(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_STORAGE, jsonWarehouse);
            }
            if (IsLoaded(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_STORAGE, Reference);
            }
            if (IsLoaded(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
            if (OrderItemList.IsLoaded || OrderItemList.Any(child => child.TeaqlHasPending()))
                yield return new KeyValuePair<string, object?>("OrderItemList", OrderItemList);
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
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
            foreach (var child in OrderItemList)
                if (child.TeaqlHasPendingGraph(visited)) return true;
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
            foreach (var child in OrderItemList)
                if (child.TeaqlHasPendingGraph(visited))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_deletion_state", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("id"), Message = "Cancelled new entity has pending descendants; explicit resolution is required" } });
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
            foreach (var child in OrderItemList) child.AttachRoot(root, hydration);
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
                public long? Warehouse { get; set; }
                public string? Reference { get; set; }
                public long? Version { get; set; }
                public Warehouse? WarehouseEntity { get; set; }
                public SmartList<OrderItem> OrderItemList { get; } = SmartList<OrderItem>.Empty();

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
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER, out var warehouseValue)
                        || record.TryGetValue(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_STORAGE, out warehouseValue))
                    {
                        entity.MarkLoaded(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER);
                        if (warehouseValue.Raw is Record warehouseRow)
                        {
                            entity.WarehouseEntity = global::Generated.Models.Warehouse.FromRecord(warehouseRow);
                            entity.Warehouse = entity.WarehouseEntity.Id;
                            entity.MarkLoaded("WarehouseEntity");
                        }
                        else if (warehouseValue.Raw is IEnumerable<Record> warehouseRows)
                        {
                            foreach (var row in warehouseRows)
                            {
                                entity.WarehouseEntity = global::Generated.Models.Warehouse.FromRecord(row);
                                entity.Warehouse = entity.WarehouseEntity.Id;
                                entity.MarkLoaded("WarehouseEntity");
                                break;
                            }
                        }
                        else if (warehouseValue.Raw == null)
                            entity.MarkLoaded("WarehouseEntity");
                        else if (warehouseValue.Raw != null)
                            entity.Warehouse = Convert.ToInt64(warehouseValue.Raw);
                    }
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_STORAGE, out var referenceValue))
                    {
                        entity.MarkLoaded(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_MEMBER);
                        if (referenceValue.Raw != null)
                            entity.Reference = Convert.ToString(referenceValue.Raw);
                    }
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
                        if (record.TryGetValue("OrderItemList", out var orderItemListValue))
                        {
                            entity.MarkLoaded("OrderItemList");
                            entity.OrderItemList.IsLoaded = true;
                            if (orderItemListValue is Value.ListValue rows)
                            {
                                if (rows.Values is SmartList<Value> carrier)
                                    entity.OrderItemList.Facets = carrier.Facets;
                                foreach (var row in rows.Values.OfType<Value.ObjectValue>())
                                    entity.OrderItemList.Add(global::Generated.Models.OrderItem.FromRecord(row.Value));
                            }
                        }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY, PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY,
                PurchaseOrder.__TeaqlFields.ID_PROPERTY_MEMBER, PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER, PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_MEMBER, PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER,
                "OrderItemList"
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
            foreach (var child in OrderItemList)
                child.TeaqlValidateCreationGraph(visited,
                    (child.TeaqlCreationIntent || child.IsLoaded("PurchaseOrder")) && child.PurchaseOrder != Id);
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
                if (!IsLoaded(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "PurchaseOrder", Location = ObjectLocation.Property(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "PurchaseOrder", Location = ObjectLocation.Property(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            for (var index = 0; index < OrderItemList.Count; index++)
            {
                var child = OrderItemList[index];
                if (child.TeaqlIsCancelledNew())
                {
                    child.TeaqlPrepareInputGraph(context, graph);
                    continue;
                }
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("PurchaseOrder")) && child.PurchaseOrder != Id) child.UpdatePurchaseOrderId(Id);
                try { child.TeaqlPrepareInputGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_item_list").Index(index);
                    throw new CheckException(context.TranslateCheckResults(error.Violations.Select(violation =>
                        violation.PrefixedBy(prefix)).ToArray()));
                }
            }
        }

        internal void TeaqlPreflightGraph(UserContext context, GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Preflight)) return;
            if (TeaqlIsCancelledNew()) return;
            if (!Id.HasValue || _teaqlForceCreate || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()))
                graph.PreflightPrepared(this);
            for (var index = 0; index < OrderItemList.Count; index++)
            {
                try { OrderItemList[index].TeaqlPreflightGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_item_list").Index(index);
                    throw new CheckException(context.TranslateCheckResults(error.Violations.Select(violation =>
                        violation.PrefixedBy(prefix)).ToArray()));
                }
            }
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
            var teaqlOriginalWarehouse = this.Warehouse;
            var teaqlOriginalReference = this.Reference;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Warehouse = teaqlOriginalWarehouse;
                this.Reference = teaqlOriginalReference;
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
            foreach (var child in OrderItemList) await child.TeaqlReserveGraphIdentitiesAsync(graph);
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
            this.Warehouse = saved.Warehouse;
            this.Reference = saved.Reference;
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
            for (var index = 0; index < OrderItemList.Count; index++)
            {
                var child = OrderItemList[index];
                if (child.TeaqlIsCancelledNew()) continue;
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("PurchaseOrder")) && child.PurchaseOrder != Id) child.UpdatePurchaseOrderId(Id);
                try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_item_list").Index(index);
                    throw new CheckException(context.TranslateCheckResults(error.Violations.Select(violation =>
                        violation.PrefixedBy(prefix)).ToArray()));
                }
            }
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
                        if (IsLoaded(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER)) record[PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_STORAGE] = TeaqlValue(Warehouse);
                        if (IsLoaded(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_MEMBER)) record[PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_STORAGE] = TeaqlValue(Reference);
                        if (IsLoaded(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_STORAGE, out var warehouseValue))
                        this.Warehouse = warehouseValue.Raw == null ? null : Convert.ToInt64(warehouseValue.Raw);

                    if (record.TryGetValue(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_STORAGE, out var referenceValue))
                        this.Reference = referenceValue.Raw == null ? null : Convert.ToString(referenceValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[PurchaseOrder.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (Warehouse.HasValue) record[PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_STORAGE] = new Value.I64Value(Warehouse.Value);

                    if (Reference != null) record[PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_STORAGE] = new Value.TextValue(Reference);

                    if (Version.HasValue) record[PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "PurchaseOrder", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Warehouse.HasValue) record[PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_STORAGE] = new Value.I64Value(Warehouse.Value);

                    if (Reference != null) record[PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_STORAGE] = new Value.TextValue(Reference);

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

                public PurchaseOrder UpdateWarehouse(long? value)
                {
                    if (this.Warehouse != value) this.WarehouseEntity = null;
                    this.Warehouse = value;
                    MarkLoaded(PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateReference(string value)
                {
                    this.Reference = value;
                    MarkLoaded(PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
                public PurchaseOrder UpdateWarehouse(Warehouse value)
                {
                    UpdateWarehouse(value?.Id);
                    this.WarehouseEntity = value;
                    return this;
                }


                public PurchaseOrder UpdateWarehouseId(long? value)
                {
                    return UpdateWarehouse(value);
                }

    }
}