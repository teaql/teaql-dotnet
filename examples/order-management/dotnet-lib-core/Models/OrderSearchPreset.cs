using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class OrderSearchPreset : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static class __TeaqlFields
        {
            internal const string ID_PROPERTY_MEMBER = "Id";
            internal const string ID_PROPERTY_QUERY = "id";
            internal const string ID_PROPERTY_STORAGE = "id";
            internal const string ID_PROPERTY_MODEL = "id";
            internal const string NAME_PROPERTY_MEMBER = "Name";
            internal const string NAME_PROPERTY_QUERY = "name";
            internal const string NAME_PROPERTY_STORAGE = "name";
            internal const string NAME_PROPERTY_MODEL = "name";
            internal const string FILTER_JSON_PROPERTY_MEMBER = "FilterJson";
            internal const string FILTER_JSON_PROPERTY_QUERY = "filter_json";
            internal const string FILTER_JSON_PROPERTY_STORAGE = "filter_json";
            internal const string FILTER_JSON_PROPERTY_MODEL = "filter_json";
            internal const string REQUEST_ID_PROPERTY_MEMBER = "RequestId";
            internal const string REQUEST_ID_PROPERTY_QUERY = "request_id";
            internal const string REQUEST_ID_PROPERTY_STORAGE = "request_id";
            internal const string REQUEST_ID_PROPERTY_MODEL = "request_id";
            internal const string OWNER_USER_ID_PROPERTY_MEMBER = "OwnerUserId";
            internal const string OWNER_USER_ID_PROPERTY_QUERY = "owner_user_id";
            internal const string OWNER_USER_ID_PROPERTY_STORAGE = "owner_user_id";
            internal const string OWNER_USER_ID_PROPERTY_MODEL = "owner_user_id";
            internal const string COMMERCE_PLATFORM_PROPERTY_MEMBER = "CommercePlatform";
            internal const string COMMERCE_PLATFORM_PROPERTY_QUERY = "commerce_platform";
            internal const string COMMERCE_PLATFORM_PROPERTY_STORAGE = "commerce_platform";
            internal const string COMMERCE_PLATFORM_PROPERTY_MODEL = "commerce_platform";
            internal const string CREATE_TIME_PROPERTY_MEMBER = "CreateTime";
            internal const string CREATE_TIME_PROPERTY_QUERY = "create_time";
            internal const string CREATE_TIME_PROPERTY_STORAGE = "create_time";
            internal const string CREATE_TIME_PROPERTY_MODEL = "create_time";
            internal const string UPDATE_TIME_PROPERTY_MEMBER = "UpdateTime";
            internal const string UPDATE_TIME_PROPERTY_QUERY = "update_time";
            internal const string UPDATE_TIME_PROPERTY_STORAGE = "update_time";
            internal const string UPDATE_TIME_PROPERTY_MODEL = "update_time";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("OrderSearchPreset", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(OrderSearchPreset.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, OrderSearchPreset.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, () => global::Generated.Models.CommercePlatform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_STORAGE)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static OrderSearchPreset FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(OrderSearchPreset.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderSearchPreset.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_STORAGE, Name);
            }
            if (IsLoaded(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_STORAGE, FilterJson);
            }
            if (IsLoaded(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_STORAGE, RequestId);
            }
            if (IsLoaded(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_STORAGE, OwnerUserId);
            }
            if (IsLoaded(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER))
            {
                object? jsonCommercePlatform = IsLoaded("CommercePlatformEntity") && CommercePlatformEntity != null && CommercePlatformEntity.Id == CommercePlatform
                    ? CommercePlatformEntity : CommercePlatform;
                yield return new KeyValuePair<string, object?>(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, jsonCommercePlatform);
            }
            if (IsLoaded(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, CreateTime);
            }
            if (IsLoaded(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, UpdateTime);
            }
            if (IsLoaded(OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("OrderSearchPreset", Id ?? _ledgerId);
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
        private OrderSearchPreset() { _creationState = EntityCreationState.Hydrated; }
        internal static OrderSearchPreset TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new OrderSearchPreset();
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
                public string? Name { get; set; }
                public string? FilterJson { get; set; }
                public string? RequestId { get; set; }
                public string? OwnerUserId { get; set; }
                public long? CommercePlatform { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public CommercePlatform? CommercePlatformEntity { get; set; }

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
        internal static OrderSearchPreset TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public OrderSearchPreset MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public OrderSearchPreset MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public OrderSearchPreset AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public OrderSearchPreset MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static OrderSearchPreset Refer(long id)
        {
            var entity = new OrderSearchPreset();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static OrderSearchPreset FromRecord(Record record)
        {
            var entity = new OrderSearchPreset().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(OrderSearchPreset.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                    {
                        entity.MarkLoaded(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_MEMBER);
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_STORAGE, out var filterJsonValue))
                    {
                        entity.MarkLoaded(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_MEMBER);
                        if (filterJsonValue.Raw != null)
                            entity.FilterJson = Convert.ToString(filterJsonValue.Raw);
                    }
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_STORAGE, out var requestIdValue))
                    {
                        entity.MarkLoaded(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_MEMBER);
                        if (requestIdValue.Raw != null)
                            entity.RequestId = Convert.ToString(requestIdValue.Raw);
                    }
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_STORAGE, out var ownerUserIdValue))
                    {
                        entity.MarkLoaded(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_MEMBER);
                        if (ownerUserIdValue.Raw != null)
                            entity.OwnerUserId = Convert.ToString(ownerUserIdValue.Raw);
                    }
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, out var commercePlatformValue)
                        || record.TryGetValue(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, out commercePlatformValue))
                    {
                        entity.MarkLoaded(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER);
                        if (commercePlatformValue.Raw is Record commercePlatformRow)
                        {
                            entity.CommercePlatformEntity = global::Generated.Models.CommercePlatform.FromRecord(commercePlatformRow);
                            entity.CommercePlatform = entity.CommercePlatformEntity.Id;
                            entity.MarkLoaded("CommercePlatformEntity");
                        }
                        else if (commercePlatformValue.Raw is IEnumerable<Record> commercePlatformRows)
                        {
                            foreach (var row in commercePlatformRows)
                            {
                                entity.CommercePlatformEntity = global::Generated.Models.CommercePlatform.FromRecord(row);
                                entity.CommercePlatform = entity.CommercePlatformEntity.Id;
                                entity.MarkLoaded("CommercePlatformEntity");
                                break;
                            }
                        }
                        else if (commercePlatformValue.Raw == null)
                            entity.MarkLoaded("CommercePlatformEntity");
                        else if (commercePlatformValue.Raw != null)
                            entity.CommercePlatform = Convert.ToInt64(commercePlatformValue.Raw);
                    }
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, out var createTimeValue))
                    {
                        entity.MarkLoaded(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER);
                        if (createTimeValue.Raw != null)
                            entity.CreateTime = TeaqlDateTime(createTimeValue);
                    }
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, out var updateTimeValue))
                    {
                        entity.MarkLoaded(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER);
                        if (updateTimeValue.Raw != null)
                            entity.UpdateTime = TeaqlDateTime(updateTimeValue);
                    }
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY,
                OrderSearchPreset.__TeaqlFields.ID_PROPERTY_MEMBER, OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_MEMBER, OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_MEMBER, OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_MEMBER, OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_MEMBER, OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER, OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER, OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_MEMBER,
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static OrderSearchPreset FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<OrderSearchPreset> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("OrderSearchPreset");
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
            graph.DiscardRestoredFields("OrderSearchPreset", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(OrderSearchPreset.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderSearchPreset", Location = ObjectLocation.Property(OrderSearchPreset.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderSearchPreset", Location = ObjectLocation.Property(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderSearchPreset", Location = ObjectLocation.Property(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderSearchPreset", Location = ObjectLocation.Property(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderSearchPreset", Location = ObjectLocation.Property(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderSearchPreset", Location = ObjectLocation.Property(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderSearchPreset", Location = ObjectLocation.Property(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderSearchPreset", Location = ObjectLocation.Property(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderSearchPreset", Location = ObjectLocation.Property(OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            var teaqlOriginalName = this.Name;
            var teaqlOriginalFilterJson = this.FilterJson;
            var teaqlOriginalRequestId = this.RequestId;
            var teaqlOriginalOwnerUserId = this.OwnerUserId;
            var teaqlOriginalCommercePlatform = this.CommercePlatform;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalUpdateTime = this.UpdateTime;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Name = teaqlOriginalName;
                this.FilterJson = teaqlOriginalFilterJson;
                this.RequestId = teaqlOriginalRequestId;
                this.OwnerUserId = teaqlOriginalOwnerUserId;
                this.CommercePlatform = teaqlOriginalCommercePlatform;
                this.CreateTime = teaqlOriginalCreateTime;
                this.UpdateTime = teaqlOriginalUpdateTime;
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

        internal async global::System.Threading.Tasks.Task<OrderSearchPreset> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("OrderSearchPreset",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("OrderSearchPreset",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for OrderSearchPreset");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Name = saved.Name;
            this.FilterJson = saved.FilterJson;
            this.RequestId = saved.RequestId;
            this.OwnerUserId = saved.OwnerUserId;
            this.CommercePlatform = saved.CommercePlatform;
            this.CreateTime = saved.CreateTime;
            this.UpdateTime = saved.UpdateTime;
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
                        if (IsLoaded(OrderSearchPreset.__TeaqlFields.ID_PROPERTY_MEMBER)) record[OrderSearchPreset.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_MEMBER)) record[OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_STORAGE] = TeaqlValue(Name);
                        if (IsLoaded(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_MEMBER)) record[OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_STORAGE] = TeaqlValue(FilterJson);
                        if (IsLoaded(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_MEMBER)) record[OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_STORAGE] = TeaqlValue(RequestId);
                        if (IsLoaded(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_MEMBER)) record[OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_STORAGE] = TeaqlValue(OwnerUserId);
                        if (IsLoaded(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER)) record[OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE] = TeaqlValue(CommercePlatform);
                        if (IsLoaded(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER)) record[OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = TeaqlValue(CreateTime);
                        if (IsLoaded(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER)) record[OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = TeaqlValue(UpdateTime);
                        if (IsLoaded(OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                        this.Name = nameValue.Raw == null ? null : Convert.ToString(nameValue.Raw);

                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_STORAGE, out var filterJsonValue))
                        this.FilterJson = filterJsonValue.Raw == null ? null : Convert.ToString(filterJsonValue.Raw);

                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_STORAGE, out var requestIdValue))
                        this.RequestId = requestIdValue.Raw == null ? null : Convert.ToString(requestIdValue.Raw);

                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_STORAGE, out var ownerUserIdValue))
                        this.OwnerUserId = ownerUserIdValue.Raw == null ? null : Convert.ToString(ownerUserIdValue.Raw);

                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, out var commercePlatformValue))
                        this.CommercePlatform = commercePlatformValue.Raw == null ? null : Convert.ToInt64(commercePlatformValue.Raw);

                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, out var createTimeValue))
                        this.CreateTime = createTimeValue.Raw == null ? null : TeaqlDateTime(createTimeValue);

                    if (record.TryGetValue(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, out var updateTimeValue))
                        this.UpdateTime = updateTimeValue.Raw == null ? null : TeaqlDateTime(updateTimeValue);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[OrderSearchPreset.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (Name != null) record[OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (FilterJson != null) record[OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_STORAGE] = new Value.TextValue(FilterJson);

                    if (RequestId != null) record[OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_STORAGE] = new Value.TextValue(RequestId);

                    if (OwnerUserId != null) record[OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_STORAGE] = new Value.TextValue(OwnerUserId);

                    if (CommercePlatform.HasValue) record[OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record[OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record[OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record[OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderSearchPreset", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record[OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (FilterJson != null) record[OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_STORAGE] = new Value.TextValue(FilterJson);

                    if (RequestId != null) record[OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_STORAGE] = new Value.TextValue(RequestId);

                    if (OwnerUserId != null) record[OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_STORAGE] = new Value.TextValue(OwnerUserId);

                    if (CommercePlatform.HasValue) record[OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record[OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record[OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record[OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "OrderSearchPreset", 
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
                Entity = "OrderSearchPreset",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("OrderSearchPreset");
        }

                public OrderSearchPreset UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded(OrderSearchPreset.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderSearchPreset.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded(OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateFilterJson(string value)
                {
                    this.FilterJson = value;
                    MarkLoaded(OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateRequestId(string value)
                {
                    this.RequestId = value;
                    MarkLoaded(OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateOwnerUserId(string value)
                {
                    this.OwnerUserId = value;
                    MarkLoaded(OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateCommercePlatform(long? value)
                {
                    if (this.CommercePlatform != value) this.CommercePlatformEntity = null;
                    this.CommercePlatform = value;
                    MarkLoaded(OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded(OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded(OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
                public OrderSearchPreset UpdateCommercePlatform(CommercePlatform value)
                {
                    UpdateCommercePlatform(value?.Id);
                    this.CommercePlatformEntity = value;
                    return this;
                }


                public OrderSearchPreset UpdateCommercePlatformId(long? value)
                {
                    return UpdateCommercePlatform(value);
                }

    }
}