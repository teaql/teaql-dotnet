using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class OrderSearchPreset
    {
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("OrderSearchPreset", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root, bool hydration = false)
        {
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
        public OrderSearchPreset() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
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
        {
            var oldKey = TeaqlEntityKey();
            Id = value;
            MarkLoaded("Id");
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            _entityRoot.Set(TeaqlEntityKey(), "id", new Value.I64Value(value));
            _teaqlForceCreate = true;
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
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("name", out var nameValue))
                    {
                        entity.MarkLoaded("Name");
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue("filter_json", out var filterJsonValue))
                    {
                        entity.MarkLoaded("FilterJson");
                        if (filterJsonValue.Raw != null)
                            entity.FilterJson = Convert.ToString(filterJsonValue.Raw);
                    }
                    if (record.TryGetValue("request_id", out var requestIdValue))
                    {
                        entity.MarkLoaded("RequestId");
                        if (requestIdValue.Raw != null)
                            entity.RequestId = Convert.ToString(requestIdValue.Raw);
                    }
                    if (record.TryGetValue("owner_user_id", out var ownerUserIdValue))
                    {
                        entity.MarkLoaded("OwnerUserId");
                        if (ownerUserIdValue.Raw != null)
                            entity.OwnerUserId = Convert.ToString(ownerUserIdValue.Raw);
                    }
                    if (record.TryGetValue("CommercePlatform", out var commercePlatformValue)
                        || record.TryGetValue("commerce_platform", out commercePlatformValue))
                    {
                        entity.MarkLoaded("CommercePlatform");
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
                        else if (commercePlatformValue.Raw != null)
                            entity.CommercePlatform = Convert.ToInt64(commercePlatformValue.Raw);
                    }
                    if (record.TryGetValue("create_time", out var createTimeValue))
                    {
                        entity.MarkLoaded("CreateTime");
                        if (createTimeValue.Raw != null)
                            entity.CreateTime = TeaqlDateTime(createTimeValue);
                    }
                    if (record.TryGetValue("update_time", out var updateTimeValue))
                    {
                        entity.MarkLoaded("UpdateTime");
                        if (updateTimeValue.Raw != null)
                            entity.UpdateTime = TeaqlDateTime(updateTimeValue);
                    }
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            return entity;
        }

        internal static OrderSearchPreset FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async Task<OrderSearchPreset> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                if (!Id.HasValue || _teaqlForceCreate)
                {
                }
                TeaqlPreflightGraph(context, graph);
                return await TeaqlSaveWithinGraphAsync(context, graph);
            });
        }

        internal void TeaqlPreflightGraph(UserContext context, GraphMutationSession graph)
        {
            var creating = !Id.HasValue || _teaqlForceCreate;
            if (creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()))
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Name"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("name"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("FilterJson"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("filter_json"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequestId"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("request_id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OwnerUserId"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("owner_user_id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("CommercePlatform"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("commerce_platform"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("CreateTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("create_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("UpdateTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("update_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Version"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("version"), Message = "Mutation requires a fully loaded entity" } });
            }
            var command = _markedForDeletion ? (object)ToDeleteCommand()
                : creating ? (object)ToInsertCommand() : (object)ToUpdateCommand();
            if (!creating && !_markedForDeletion)
            {
                ((UpdateCommand)command).Values = _entityRoot.Change(TeaqlEntityKey());
                var originalVersion = _entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version;
                if (originalVersion.HasValue) ((UpdateCommand)command).Values["version"] = new Value.I64Value(originalVersion.Value);
            }
            graph.Preflight(TeaqlMutationRequest(command, graph.Intent.Comment));
            }
        }

        internal async Task<OrderSearchPreset> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            var creating = !this.Id.HasValue || _teaqlForceCreate;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("OrderSearchPreset",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
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
            if (creating && !Id.HasValue)
            {
                var allocationKey = TeaqlEntityKey();
                Id = checked((long)await graph.AllocateIdAsync("OrderSearchPreset"));
                _entityRoot.Rekey(allocationKey, TeaqlEntityKey());
            }
            var scope = graph.Scope("OrderSearchPreset",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var cmd = _markedForDeletion ? (object)ToDeleteCommand()
                : creating ? (object)ToInsertCommand()
                : (object)ToUpdateCommand();
            if (!creating && !_markedForDeletion) {
                ((UpdateCommand)cmd).Values = _entityRoot.Change(TeaqlEntityKey());
                var originalVersion = _entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version;
                if (originalVersion.HasValue) ((UpdateCommand)cmd).Values["version"] = new Value.I64Value(originalVersion.Value);
            }
            var req = TeaqlMutationRequest(cmd, graph.Intent.Comment);
            var mutationResult = await graph.MutateAsync(req, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for OrderSearchPreset");
            var saved = FromRecord(mutationResult.PersistedRecord);
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
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            await TeaqlSaveChildrenAsync(context, graph, scope);
            return saved;
        }

        private async Task TeaqlSaveChildrenAsync(UserContext context, GraphMutationSession graph, MutationTraceScope scope)
        {
            await Task.CompletedTask;
        }

        private MutationRequest TeaqlMutationRequest(object command, string rootComment) => command switch
        {
            InsertCommand insert => MutationRequest.Create(insert, rootComment, TeaqlEntityKey(), _entityRoot),
            UpdateCommand update => MutationRequest.Create(update, rootComment, TeaqlEntityKey(), _entityRoot),
            DeleteCommand delete => MutationRequest.Create(delete, rootComment, TeaqlEntityKey(), _entityRoot),
            _ => throw new InvalidOperationException("Unsupported mutation command")
        };

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (FilterJson != null) record["filter_json"] = new Value.TextValue(FilterJson);

                    if (RequestId != null) record["request_id"] = new Value.TextValue(RequestId);

                    if (OwnerUserId != null) record["owner_user_id"] = new Value.TextValue(OwnerUserId);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderSearchPreset", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (FilterJson != null) record["filter_json"] = new Value.TextValue(FilterJson);

                    if (RequestId != null) record["request_id"] = new Value.TextValue(RequestId);

                    if (OwnerUserId != null) record["owner_user_id"] = new Value.TextValue(OwnerUserId);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

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
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    _entityRoot.Set(TeaqlEntityKey(), "name", TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateFilterJson(string value)
                {
                    this.FilterJson = value;
                    MarkLoaded("FilterJson");
                    _entityRoot.Set(TeaqlEntityKey(), "filter_json", TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateRequestId(string value)
                {
                    this.RequestId = value;
                    MarkLoaded("RequestId");
                    _entityRoot.Set(TeaqlEntityKey(), "request_id", TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateOwnerUserId(string value)
                {
                    this.OwnerUserId = value;
                    MarkLoaded("OwnerUserId");
                    _entityRoot.Set(TeaqlEntityKey(), "owner_user_id", TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "create_time", TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "update_time", TeaqlValue(value));
                    return this;
                }

                public OrderSearchPreset UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public OrderSearchPreset UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(this.CommercePlatform));
                    return this;
                }


                public OrderSearchPreset UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(value));
                    return this;
                }

    }
}