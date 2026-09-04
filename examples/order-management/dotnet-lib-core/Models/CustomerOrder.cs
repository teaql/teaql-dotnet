using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{
    public class CustomerOrder
    {
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("CustomerOrder", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; } foreach (var child in OrderLineList) child.AttachRoot(root); }
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
        public CustomerOrder() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Id { get; set; }
                public string? OrderNumber { get; set; }
                public DateTime? OrderDate { get; set; }
                public decimal? TotalAmount { get; set; }
                public long? Status { get; set; }
                public long? Customer { get; set; }
                public long? CommercePlatform { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public OrderStatus? StatusEntity { get; set; }
                public Customer? CustomerEntity { get; set; }
                public CommercePlatform? CommercePlatformEntity { get; set; }
                public List<OrderLine> OrderLineList { get; } = new List<OrderLine>();

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

        public CustomerOrder MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public CustomerOrder MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public CustomerOrder AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public CustomerOrder MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static CustomerOrder Refer(long id)
        {
            return new CustomerOrder { Id = id }.MarkLoadedOnly("Id");
        }

        public static CustomerOrder FromRecord(Record record)
        {
            var entity = new CustomerOrder().MarkLoadedOnly();
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("order_number", out var orderNumberValue))
                    {
                        entity.MarkLoaded("OrderNumber");
                        if (orderNumberValue.Raw != null)
                            entity.OrderNumber = Convert.ToString(orderNumberValue.Raw);
                    }
                    if (record.TryGetValue("order_date", out var orderDateValue))
                    {
                        entity.MarkLoaded("OrderDate");
                        if (orderDateValue.Raw != null)
                            entity.OrderDate = TeaqlDateTime(orderDateValue);
                    }
                    if (record.TryGetValue("total_amount", out var totalAmountValue))
                    {
                        entity.MarkLoaded("TotalAmount");
                        if (totalAmountValue.Raw != null)
                            entity.TotalAmount = Convert.ToDecimal(totalAmountValue.Raw);
                    }
                    if (record.TryGetValue("Status", out var statusValue)
                        || record.TryGetValue("status", out statusValue))
                    {
                        entity.MarkLoaded("Status");
                        if (statusValue.Raw is Record statusRow)
                        {
                            entity.StatusEntity = global::Generated.Models.OrderStatus.FromRecord(statusRow);
                            entity.Status = entity.StatusEntity.Id;
                            entity.MarkLoaded("StatusEntity");
                        }
                        else if (statusValue.Raw is IEnumerable<Record> statusRows)
                        {
                            foreach (var row in statusRows)
                            {
                                entity.StatusEntity = global::Generated.Models.OrderStatus.FromRecord(row);
                                entity.Status = entity.StatusEntity.Id;
                                entity.MarkLoaded("StatusEntity");
                                break;
                            }
                        }
                        else if (statusValue.Raw != null)
                            entity.Status = Convert.ToInt64(statusValue.Raw);
                    }
                    if (record.TryGetValue("Customer", out var customerValue)
                        || record.TryGetValue("customer", out customerValue))
                    {
                        entity.MarkLoaded("Customer");
                        if (customerValue.Raw is Record customerRow)
                        {
                            entity.CustomerEntity = global::Generated.Models.Customer.FromRecord(customerRow);
                            entity.Customer = entity.CustomerEntity.Id;
                            entity.MarkLoaded("CustomerEntity");
                        }
                        else if (customerValue.Raw is IEnumerable<Record> customerRows)
                        {
                            foreach (var row in customerRows)
                            {
                                entity.CustomerEntity = global::Generated.Models.Customer.FromRecord(row);
                                entity.Customer = entity.CustomerEntity.Id;
                                entity.MarkLoaded("CustomerEntity");
                                break;
                            }
                        }
                        else if (customerValue.Raw != null)
                            entity.Customer = Convert.ToInt64(customerValue.Raw);
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
                        if (record.TryGetValue("OrderLineList", out var orderLineListValue))
                        {
                            entity.MarkLoaded("OrderLineList");
                            var rows = orderLineListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.OrderLineList.Add(global::Generated.Models.OrderLine.FromRecord(row));
                        }
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            return entity;
        }

        internal static CustomerOrder FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root);
            return entity;
        }

        public async Task<CustomerOrder> SaveAsync(UserContext context)
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
                if (!IsLoaded("OrderNumber"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("order_number"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OrderDate"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("order_date"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("TotalAmount"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("total_amount"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Status"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("status"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Customer"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("customer"), Message = "Mutation requires a fully loaded entity" } });
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
            for (var index = 0; index < OrderLineList.Count; index++)
            {
                var child = OrderLineList[index];
                child.AttachRoot(_entityRoot);
                child.UpdateCustomerOrderId(Id ?? _ledgerId);
                child.AuditAs(_comment!);
                try { child.TeaqlPreflightGraph(context); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_line_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        new CheckResult { RuleId = violation.RuleId, Location = violation.Location.PrefixedBy(prefix), EntityType = violation.EntityType, SourceInstancePath = violation.SourceInstancePath, InputValue = violation.InputValue, SystemValue = violation.SystemValue, Message = violation.Message }).ToArray());
                }
            }
        }

        internal async Task<CustomerOrder> TeaqlSaveWithinGraphAsync(UserContext context)
        {
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
            var teaqlOriginalFullyLoaded = _fullyLoaded;
            var teaqlOriginalLoadedFields = new HashSet<string>(_loadedFields, StringComparer.OrdinalIgnoreCase);
            var teaqlOriginalId = this.Id;
            var teaqlOriginalOrderNumber = this.OrderNumber;
            var teaqlOriginalOrderDate = this.OrderDate;
            var teaqlOriginalTotalAmount = this.TotalAmount;
            var teaqlOriginalStatus = this.Status;
            var teaqlOriginalCustomer = this.Customer;
            var teaqlOriginalCommercePlatform = this.CommercePlatform;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalUpdateTime = this.UpdateTime;
            var teaqlOriginalVersion = this.Version;
            context.AfterGraphRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.OrderNumber = teaqlOriginalOrderNumber;
                this.OrderDate = teaqlOriginalOrderDate;
                this.TotalAmount = teaqlOriginalTotalAmount;
                this.Status = teaqlOriginalStatus;
                this.Customer = teaqlOriginalCustomer;
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
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for CustomerOrder");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.OrderNumber = saved.OrderNumber;
            this.OrderDate = saved.OrderDate;
            this.TotalAmount = saved.TotalAmount;
            this.Status = saved.Status;
            this.Customer = saved.Customer;
            this.CommercePlatform = saved.CommercePlatform;
            this.CreateTime = saved.CreateTime;
            this.UpdateTime = saved.UpdateTime;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _teaqlForceCreate = false;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            for (var index = 0; index < OrderLineList.Count; index++)
            {
                var child = OrderLineList[index];
                child.AttachRoot(_entityRoot);
                child.UpdateCustomerOrderId(Id);
                child.AuditAs(_comment!);
                try { await child.TeaqlSaveWithinGraphAsync(context); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_line_list").Index(index);
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

                    if (OrderNumber != null) record["order_number"] = new Value.TextValue(OrderNumber);

                    if (OrderDate.HasValue) record["order_date"] = new Value.DateValue(OrderDate.Value);

                    if (TotalAmount.HasValue) record["total_amount"] = new Value.DecimalValue(TotalAmount.Value);

                    if (Status.HasValue) record["status"] = new Value.I64Value(Status.Value);

                    if (Customer.HasValue) record["customer"] = new Value.I64Value(Customer.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "CustomerOrder", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (OrderNumber != null) record["order_number"] = new Value.TextValue(OrderNumber);

                    if (OrderDate.HasValue) record["order_date"] = new Value.DateValue(OrderDate.Value);

                    if (TotalAmount.HasValue) record["total_amount"] = new Value.DecimalValue(TotalAmount.Value);

                    if (Status.HasValue) record["status"] = new Value.I64Value(Status.Value);

                    if (Customer.HasValue) record["customer"] = new Value.I64Value(Customer.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "CustomerOrder", 
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
                Entity = "CustomerOrder",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("CustomerOrder");
        }

                public CustomerOrder UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateOrderNumber(string value)
                {
                    this.OrderNumber = value;
                    MarkLoaded("OrderNumber");
                    _entityRoot.Set(TeaqlEntityKey(), "order_number", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateOrderDate(DateTime? value)
                {
                    this.OrderDate = value;
                    MarkLoaded("OrderDate");
                    _entityRoot.Set(TeaqlEntityKey(), "order_date", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateTotalAmount(decimal? value)
                {
                    this.TotalAmount = value;
                    MarkLoaded("TotalAmount");
                    _entityRoot.Set(TeaqlEntityKey(), "total_amount", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateStatus(long? value)
                {
                    this.Status = value;
                    MarkLoaded("Status");
                    _entityRoot.Set(TeaqlEntityKey(), "status", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateCustomer(long? value)
                {
                    this.Customer = value;
                    MarkLoaded("Customer");
                    _entityRoot.Set(TeaqlEntityKey(), "customer", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "create_time", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "update_time", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public CustomerOrder UpdateStatus(OrderStatus value)
                {
                    this.Status = value?.Id;
                    MarkLoaded("Status");
                    _entityRoot.Set(TeaqlEntityKey(), "status", TeaqlValue(this.Status));
                    return this;
                }


                public CustomerOrder UpdateStatusId(long? value)
                {
                    this.Status = value;
                    MarkLoaded("Status");
                    _entityRoot.Set(TeaqlEntityKey(), "status", TeaqlValue(value));
                    return this;
                }
                public CustomerOrder UpdateStatusToPending()
                {
                    this.Status = 1001;
                    MarkLoaded("Status");
                    return this;
                }

                public CustomerOrder UpdateStatusToConfirmed()
                {
                    this.Status = 1002;
                    MarkLoaded("Status");
                    return this;
                }



                public CustomerOrder UpdateCustomer(Customer value)
                {
                    this.Customer = value?.Id;
                    MarkLoaded("Customer");
                    _entityRoot.Set(TeaqlEntityKey(), "customer", TeaqlValue(this.Customer));
                    return this;
                }


                public CustomerOrder UpdateCustomerId(long? value)
                {
                    this.Customer = value;
                    MarkLoaded("Customer");
                    _entityRoot.Set(TeaqlEntityKey(), "customer", TeaqlValue(value));
                    return this;
                }


                public CustomerOrder UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(this.CommercePlatform));
                    return this;
                }


                public CustomerOrder UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(value));
                    return this;
                }

    }
}