using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class OrderStatus
    {
        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("OrderStatus", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; } foreach (var child in CustomerOrderList) child.AttachRoot(root); }
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
        public OrderStatus() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Platform { get; set; }
                public long? Id { get; set; }
                public string? Name { get; set; }
                public string? Code { get; set; }
                public string? Color { get; set; }
                public long? DisplayOrder { get; set; }
                public long? Version { get; set; }
                public Platform? PlatformEntity { get; set; }
                public List<CustomerOrder> CustomerOrderList { get; } = new List<CustomerOrder>();

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

        public OrderStatus MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public OrderStatus MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public OrderStatus AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public OrderStatus MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static OrderStatus Refer(long id)
        {
            return new OrderStatus { Id = id }.MarkLoadedOnly("Id");
        }

        public static OrderStatus FromRecord(Record record)
        {
            var entity = new OrderStatus().MarkLoadedOnly();
                    if (record.TryGetValue("Platform", out var platformValue)
                        || record.TryGetValue("platform", out platformValue))
                    {
                        entity.MarkLoaded("Platform");
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
                        else if (platformValue.Raw != null)
                            entity.Platform = Convert.ToInt64(platformValue.Raw);
                    }
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
                    if (record.TryGetValue("code", out var codeValue))
                    {
                        entity.MarkLoaded("Code");
                        if (codeValue.Raw != null)
                            entity.Code = Convert.ToString(codeValue.Raw);
                    }
                    if (record.TryGetValue("color", out var colorValue))
                    {
                        entity.MarkLoaded("Color");
                        if (colorValue.Raw != null)
                            entity.Color = Convert.ToString(colorValue.Raw);
                    }
                    if (record.TryGetValue("display_order", out var displayOrderValue))
                    {
                        entity.MarkLoaded("DisplayOrder");
                        if (displayOrderValue.Raw != null)
                            entity.DisplayOrder = Convert.ToInt64(displayOrderValue.Raw);
                    }
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
                        if (record.TryGetValue("CustomerOrderList", out var customerOrderListValue))
                        {
                            entity.MarkLoaded("CustomerOrderList");
                            if (customerOrderListValue is Value.ListValue rows)
                                foreach (var row in rows.Values.OfType<Value.ObjectValue>())
                                    entity.CustomerOrderList.Add(global::Generated.Models.CustomerOrder.FromRecord(row.Value));
                        }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                "platform", "id", "name", "code", "color", "display_order", "version",
                "Platform", "Id", "Name", "Code", "Color", "DisplayOrder", "Version",
                "CustomerOrderList"
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            return entity;
        }

        internal static OrderStatus FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root);
            return entity;
        }

        public async Task<OrderStatus> SaveAsync(UserContext context)
        {
            return await context.ExecuteGraphSaveAsync(_comment!, async graph =>
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
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Platform"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("platform"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Name"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("name"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Code"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("code"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Color"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("color"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("DisplayOrder"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("display_order"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Version"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("version"), Message = "Mutation requires a fully loaded entity" } });
            }
            var command = _markedForDeletion ? (object)ToDeleteCommand()
                : creating ? (object)ToInsertCommand() : (object)ToUpdateCommand();
            if (!creating && !_markedForDeletion)
            {
                ((UpdateCommand)command).Values = _entityRoot.Change(TeaqlEntityKey());
                if (Version.HasValue) ((UpdateCommand)command).Values["version"] = new Value.I64Value(Version.Value);
            }
            graph.Preflight(TeaqlMutationRequest(command, string.IsNullOrWhiteSpace(_comment) ? graph.Intent.Comment : _comment!));
            }
            for (var index = 0; index < CustomerOrderList.Count; index++)
            {
                var child = CustomerOrderList[index];
                child.AttachRoot(_entityRoot);
                if (child.Status != (Id ?? _ledgerId)) child.UpdateStatusId(Id ?? _ledgerId);
                try { child.TeaqlPreflightGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("customer_order_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        new CheckResult { RuleId = violation.RuleId, Location = violation.Location.PrefixedBy(prefix), EntityType = violation.EntityType, SourceInstancePath = violation.SourceInstancePath, InputValue = violation.InputValue, SystemValue = violation.SystemValue, Message = violation.Message }).ToArray());
                }
            }
        }

        internal async Task<OrderStatus> TeaqlSaveWithinGraphAsync(UserContext context, GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            var scope = graph.Scope("OrderStatus", Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            if (Id.HasValue && !_teaqlForceCreate && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                for (var index = 0; index < CustomerOrderList.Count; index++)
                {
                    var child = CustomerOrderList[index];
                    child.AttachRoot(_entityRoot);
                    if (child.Status != Id) child.UpdateStatusId(Id);
                    try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                    catch (CheckException error)
                    {
                        var prefix = ObjectLocation.Property("customer_order_list").Index(index);
                        throw new CheckException(error.Violations.Select(violation =>
                            new CheckResult { RuleId = violation.RuleId, Location = violation.Location.PrefixedBy(prefix), EntityType = violation.EntityType, SourceInstancePath = violation.SourceInstancePath, InputValue = violation.InputValue, SystemValue = violation.SystemValue, Message = violation.Message }).ToArray());
                    }
                }
                return this;
            }
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
            var teaqlOriginalFullyLoaded = _fullyLoaded;
            var teaqlOriginalLoadedFields = new HashSet<string>(_loadedFields, StringComparer.OrdinalIgnoreCase);
            var teaqlOriginalPlatform = this.Platform;
            var teaqlOriginalId = this.Id;
            var teaqlOriginalName = this.Name;
            var teaqlOriginalCode = this.Code;
            var teaqlOriginalColor = this.Color;
            var teaqlOriginalDisplayOrder = this.DisplayOrder;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Platform = teaqlOriginalPlatform;
                this.Id = teaqlOriginalId;
                this.Name = teaqlOriginalName;
                this.Code = teaqlOriginalCode;
                this.Color = teaqlOriginalColor;
                this.DisplayOrder = teaqlOriginalDisplayOrder;
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
            var creating = !this.Id.HasValue || _teaqlForceCreate;
            if (_markedForDeletion && creating)
                throw new InvalidOperationException("Cannot delete an entity without an id");
            var cmd = _markedForDeletion ? (object)ToDeleteCommand()
                : creating ? (object)ToInsertCommand()
                : (object)ToUpdateCommand();
            if (!creating && !_markedForDeletion) {
                ((UpdateCommand)cmd).Values = _entityRoot.Change(TeaqlEntityKey());
                if (Version.HasValue) ((UpdateCommand)cmd).Values["version"] = new Value.I64Value(Version.Value);
            }
            var req = TeaqlMutationRequest(cmd, scope.Node.Detail!);
            var mutationResult = await graph.MutateAsync(req, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for OrderStatus");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var oldKey = TeaqlEntityKey();
            this.Platform = saved.Platform;
            this.Id = saved.Id;
            this.Name = saved.Name;
            this.Code = saved.Code;
            this.Color = saved.Color;
            this.DisplayOrder = saved.DisplayOrder;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _teaqlForceCreate = false;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            for (var index = 0; index < CustomerOrderList.Count; index++)
            {
                var child = CustomerOrderList[index];
                child.AttachRoot(_entityRoot);
                if (child.Status != Id) child.UpdateStatusId(Id);
                try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("customer_order_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        new CheckResult { RuleId = violation.RuleId, Location = violation.Location.PrefixedBy(prefix), EntityType = violation.EntityType, SourceInstancePath = violation.SourceInstancePath, InputValue = violation.InputValue, SystemValue = violation.SystemValue, Message = violation.Message }).ToArray());
                }
            }
            return saved;
        }

        private MutationRequest TeaqlMutationRequest(object command, string comment) => command switch
        {
            InsertCommand insert => MutationRequest.Create(insert, comment, TeaqlEntityKey(), _entityRoot),
            UpdateCommand update => MutationRequest.Create(update, comment, TeaqlEntityKey(), _entityRoot),
            DeleteCommand delete => MutationRequest.Create(delete, comment, TeaqlEntityKey(), _entityRoot),
            _ => throw new InvalidOperationException("Unsupported mutation command")
        };

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Code != null) record["code"] = new Value.TextValue(Code);

                    if (Color != null) record["color"] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record["display_order"] = new Value.I64Value(DisplayOrder.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderStatus", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);


                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Code != null) record["code"] = new Value.TextValue(Code);

                    if (Color != null) record["color"] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record["display_order"] = new Value.I64Value(DisplayOrder.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "OrderStatus", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : throw new InvalidOperationException("Update requires a loaded id"),
                ExpectedVersionValue = this.Version,
                Values = record 
            };
        }

        public DeleteCommand ToDeleteCommand()
        {
            if (!Id.HasValue || !Version.HasValue)
                throw new InvalidOperationException("Delete requires a loaded id and version");
            return new DeleteCommand {
                Entity = "OrderStatus",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("OrderStatus");
        }

                public OrderStatus UpdatePlatform(long? value)
                {
                    this.Platform = value;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    _entityRoot.Set(TeaqlEntityKey(), "name", TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateCode(string value)
                {
                    this.Code = value;
                    MarkLoaded("Code");
                    _entityRoot.Set(TeaqlEntityKey(), "code", TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateColor(string value)
                {
                    this.Color = value;
                    MarkLoaded("Color");
                    _entityRoot.Set(TeaqlEntityKey(), "color", TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateDisplayOrder(long? value)
                {
                    this.DisplayOrder = value;
                    MarkLoaded("DisplayOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "display_order", TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public OrderStatus UpdatePlatform(Platform value)
                {
                    this.Platform = value?.Id;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(this.Platform));
                    return this;
                }


                public OrderStatus UpdatePlatformId(long? value)
                {
                    this.Platform = value;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
                    return this;
                }

    }
}