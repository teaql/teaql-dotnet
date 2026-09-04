using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{
    public class Customer
    {
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("Customer", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; } foreach (var child in CustomerOrderList) child.AttachRoot(root); }
        private static Value TeaqlValue(object? value) => value switch {
            null => new Value.NullValue(), string v => new Value.TextValue(v), bool v => new Value.BoolValue(v),
            double v => new Value.F64Value(v), decimal v => new Value.DecimalValue(v), DateTime v => new Value.TimestampValue(new DateTimeOffset(v).ToUnixTimeMilliseconds()), TimeSpan v => new Value.TimeValue(v),
            int v => new Value.I64Value(v), long v => new Value.I64Value(v), _ => throw new ArgumentException($"Unsupported TeaQL value type: {value.GetType().FullName}")
        };
        private static DateTime TeaqlDateTime(Value value) => value switch {
            Value.TimestampValue v => DateTimeOffset.FromUnixTimeMilliseconds(v.Milliseconds).UtcDateTime,
            Value.DateTimeValue v => v.Value,
            Value.DateValue v => v.Value,
            _ => Convert.ToDateTime(value.Raw)
        };
        public Customer() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Id { get; set; }
                public string? Name { get; set; }
                public string? Email { get; set; }
                public long? CommercePlatform { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public CommercePlatform? CommercePlatformEntity { get; set; }
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

        public Customer MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public Customer MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public Customer AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public Customer MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static Customer Refer(long id)
        {
            return new Customer { Id = id }.MarkLoadedOnly("Id");
        }

        public static Customer FromRecord(Record record)
        {
            var entity = new Customer().MarkLoadedOnly();
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
                    if (record.TryGetValue("email", out var emailValue))
                    {
                        entity.MarkLoaded("Email");
                        if (emailValue.Raw != null)
                            entity.Email = Convert.ToString(emailValue.Raw);
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
                        if (record.TryGetValue("CustomerOrderList", out var customerOrderListValue))
                        {
                            entity.MarkLoaded("CustomerOrderList");
                            var rows = customerOrderListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.CustomerOrderList.Add(global::Generated.Models.CustomerOrder.FromRecord(row));
                        }
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            return entity;
        }

        internal static Customer FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root);
            return entity;
        }

        public async Task<Customer> SaveAsync(UserContext context)
        {
            return await context.ExecuteGraphSaveAsync(async () =>
            {
                TeaqlPreflightGraph(context);
                return await TeaqlSaveWithinGraphAsync(context);
            });
        }

        internal void TeaqlPreflightGraph(UserContext context)
        {
            if (string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: AuditAs() must be called before SaveAsync()");
            var creating = !Id.HasValue || _teaqlForceCreate;
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Name"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("name"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Email"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("email"), Message = "Mutation requires a fully loaded entity" } });
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
                if (Version.HasValue) ((UpdateCommand)command).Values["version"] = new Value.I64Value(Version.Value);
            }
            context.PreflightMutation(TeaqlMutationRequest(command));
            for (var index = 0; index < CustomerOrderList.Count; index++)
            {
                var child = CustomerOrderList[index];
                child.AttachRoot(_entityRoot);
                child.UpdateCustomerId(Id ?? _ledgerId);
                child.AuditAs(_comment!);
                try { child.TeaqlPreflightGraph(context); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("customer_order_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        new CheckResult { RuleId = violation.RuleId, Location = violation.Location.PrefixedBy(prefix), EntityType = violation.EntityType, SourceInstancePath = violation.SourceInstancePath, InputValue = violation.InputValue, SystemValue = violation.SystemValue, Message = violation.Message }).ToArray());
                }
            }
        }

        internal async Task<Customer> TeaqlSaveWithinGraphAsync(UserContext context)
        {
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
            var teaqlOriginalFullyLoaded = _fullyLoaded;
            var teaqlOriginalLoadedFields = new HashSet<string>(_loadedFields, StringComparer.OrdinalIgnoreCase);
            var teaqlOriginalId = this.Id;
            var teaqlOriginalName = this.Name;
            var teaqlOriginalEmail = this.Email;
            var teaqlOriginalCommercePlatform = this.CommercePlatform;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalUpdateTime = this.UpdateTime;
            var teaqlOriginalVersion = this.Version;
            context.AfterGraphRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Name = teaqlOriginalName;
                this.Email = teaqlOriginalEmail;
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
            context.AfterGraphCommit(() =>
            {
                _entityRoot.ClearEntity(TeaqlEntityKey());
                if (Version.HasValue) _entityRoot.SetOriginalVersion(TeaqlEntityKey(), Version.Value);
            });
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new Exception("Security audit failure: AuditAs() must be called before SaveAsync()");
            }
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
            var req = TeaqlMutationRequest(cmd);
            var mutationResult = await context.RequireResource<IDataService>().MutateAsync(req);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for Customer");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Name = saved.Name;
            this.Email = saved.Email;
            this.CommercePlatform = saved.CommercePlatform;
            this.CreateTime = saved.CreateTime;
            this.UpdateTime = saved.UpdateTime;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _teaqlForceCreate = false;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            for (var index = 0; index < CustomerOrderList.Count; index++)
            {
                var child = CustomerOrderList[index];
                child.AttachRoot(_entityRoot);
                child.UpdateCustomerId(Id);
                child.AuditAs(_comment!);
                try { await child.TeaqlSaveWithinGraphAsync(context); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("customer_order_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        new CheckResult { RuleId = violation.RuleId, Location = violation.Location.PrefixedBy(prefix), EntityType = violation.EntityType, SourceInstancePath = violation.SourceInstancePath, InputValue = violation.InputValue, SystemValue = violation.SystemValue, Message = violation.Message }).ToArray());
                }
            }
            return saved;
        }

        private MutationRequest TeaqlMutationRequest(object command) => command switch
        {
            InsertCommand insert => MutationRequest.Create(insert, _comment!, TeaqlEntityKey(), _entityRoot),
            UpdateCommand update => MutationRequest.Create(update, _comment!, TeaqlEntityKey(), _entityRoot),
            DeleteCommand delete => MutationRequest.Create(delete, _comment!, TeaqlEntityKey(), _entityRoot),
            _ => throw new InvalidOperationException("Unsupported mutation command")
        };

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Email != null) record["email"] = new Value.TextValue(Email);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "Customer", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Email != null) record["email"] = new Value.TextValue(Email);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "Customer", 
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
                Entity = "Customer",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("Customer");
        }

                public Customer UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public Customer UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    _entityRoot.Set(TeaqlEntityKey(), "name", TeaqlValue(value));
                    return this;
                }

                public Customer UpdateEmail(string value)
                {
                    this.Email = value;
                    MarkLoaded("Email");
                    _entityRoot.Set(TeaqlEntityKey(), "email", TeaqlValue(value));
                    return this;
                }

                public Customer UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(value));
                    return this;
                }

                public Customer UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "create_time", TeaqlValue(value));
                    return this;
                }

                public Customer UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "update_time", TeaqlValue(value));
                    return this;
                }

                public Customer UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public Customer UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(this.CommercePlatform));
                    return this;
                }


                public Customer UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(value));
                    return this;
                }

    }
}