using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class CustomerOrder : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("CustomerOrder", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField("id", global::TeaQL.Runtime.EntityJsonKind.Id, "id"),
            new global::TeaQL.Runtime.EntityJsonField("customer", global::TeaQL.Runtime.EntityJsonKind.Reference, "customer", () => global::Generated.Models.Customer._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("status", global::TeaQL.Runtime.EntityJsonKind.Reference, "status", () => global::Generated.Models.OrderStatus._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("order_number", global::TeaQL.Runtime.EntityJsonKind.Text, "order_number"),
            new global::TeaQL.Runtime.EntityJsonField("order_date", global::TeaQL.Runtime.EntityJsonKind.Date, "order_date"),
            new global::TeaQL.Runtime.EntityJsonField("total_amount", global::TeaQL.Runtime.EntityJsonKind.Decimal, "total_amount"),
            new global::TeaQL.Runtime.EntityJsonField("version", global::TeaQL.Runtime.EntityJsonKind.Version, "version"),
            new global::TeaQL.Runtime.EntityJsonField("order_item_list", global::TeaQL.Runtime.EntityJsonKind.Many, "OrderItemList", () => global::Generated.Models.OrderItem._teaqlJsonSchema)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static CustomerOrder FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded("Id"))
            {
                yield return new KeyValuePair<string, object?>("id", Id);
            }
            if (IsLoaded("Customer"))
            {
                object? jsonCustomer = IsLoaded("CustomerEntity") && CustomerEntity != null && CustomerEntity.Id == Customer
                    ? CustomerEntity : Customer;
                yield return new KeyValuePair<string, object?>("customer", jsonCustomer);
            }
            if (IsLoaded("Status"))
            {
                object? jsonStatus = IsLoaded("StatusEntity") && StatusEntity != null && StatusEntity.Id == Status
                    ? StatusEntity : Status;
                yield return new KeyValuePair<string, object?>("status", jsonStatus);
            }
            if (IsLoaded("OrderNumber"))
            {
                yield return new KeyValuePair<string, object?>("order_number", OrderNumber);
            }
            if (IsLoaded("OrderDate"))
            {
                yield return new KeyValuePair<string, object?>("order_date", OrderDate);
            }
            if (IsLoaded("TotalAmount"))
            {
                yield return new KeyValuePair<string, object?>("total_amount", TotalAmount);
            }
            if (IsLoaded("Version"))
            {
                yield return new KeyValuePair<string, object?>("version", Version);
            }
            if (IsLoaded("OrderItemList"))
                yield return new KeyValuePair<string, object?>("OrderItemList", OrderItemList);
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("CustomerOrder", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root, bool hydration = false)
        {
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
        public CustomerOrder() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Id { get; set; }
                public long? Customer { get; set; }
                public long? Status { get; set; }
                public string? OrderNumber { get; set; }
                public DateTime? OrderDate { get; set; }
                public decimal? TotalAmount { get; set; }
                public long? Version { get; set; }
                public Customer? CustomerEntity { get; set; }
                public OrderStatus? StatusEntity { get; set; }
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
            _comment = new MutationIntent(comment).Comment;
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
            var entity = new CustomerOrder();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static CustomerOrder FromRecord(Record record)
        {
            var entity = new CustomerOrder().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
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
                        else if (customerValue.Raw == null)
                            entity.MarkLoaded("CustomerEntity");
                        else if (customerValue.Raw != null)
                            entity.Customer = Convert.ToInt64(customerValue.Raw);
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
                        else if (statusValue.Raw == null)
                            entity.MarkLoaded("StatusEntity");
                        else if (statusValue.Raw != null)
                            entity.Status = Convert.ToInt64(statusValue.Raw);
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
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
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
                "id", "customer", "status", "order_number", "order_date", "total_amount", "version",
                "Id", "Customer", "Status", "OrderNumber", "OrderDate", "TotalAmount", "Version",
                "OrderItemList"
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static CustomerOrder FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<CustomerOrder> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("CustomerOrder");
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

        internal void TeaqlPrepareInputGraph(UserContext context, GraphMutationSession graph)
        {
            graph.DiscardRestoredFields("CustomerOrder", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = !Id.HasValue || _teaqlForceCreate;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Customer"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("customer"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Status"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("status"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OrderNumber"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("order_number"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OrderDate"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("order_date"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("TotalAmount"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("total_amount"), Message = "Mutation requires a fully loaded entity" } });
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
            graph.PrepareInput(this, TeaqlMutationRequest(command, graph.Intent.Comment));
            }
            for (var index = 0; index < OrderItemList.Count; index++)
            {
                var child = OrderItemList[index];
                child.AttachRoot(_entityRoot);
                if (child.CustomerOrder != (Id ?? _ledgerId)) child.UpdateCustomerOrderId(Id ?? _ledgerId);
                try { child.TeaqlPrepareInputGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_item_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        new CheckResult { RuleId = violation.RuleId, Location = violation.Location.PrefixedBy(prefix), EntityType = violation.EntityType, SourceInstancePath = violation.SourceInstancePath, InputValue = violation.InputValue, SystemValue = violation.SystemValue, Message = violation.Message }).ToArray());
                }
            }
        }

        internal void TeaqlPreflightGraph(UserContext context, GraphMutationSession graph)
        {
            if (!Id.HasValue || _teaqlForceCreate || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()))
                graph.PreflightPrepared(this);
            for (var index = 0; index < OrderItemList.Count; index++)
            {
                try { OrderItemList[index].TeaqlPreflightGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_item_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        new CheckResult { RuleId = violation.RuleId, Location = violation.Location.PrefixedBy(prefix), EntityType = violation.EntityType, SourceInstancePath = violation.SourceInstancePath, InputValue = violation.InputValue, SystemValue = violation.SystemValue, Message = violation.Message }).ToArray());
                }
            }
        }

        internal async global::System.Threading.Tasks.Task TeaqlReserveGraphIdentitiesAsync(GraphMutationSession graph)
        {
            var creating = !Id.HasValue || _teaqlForceCreate;
            if (creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()))
            {
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
            var teaqlOriginalFullyLoaded = _fullyLoaded;
            var teaqlOriginalLoadedFields = new HashSet<string>(_loadedFields, StringComparer.OrdinalIgnoreCase);
            var teaqlOriginalId = this.Id;
            var teaqlOriginalCustomer = this.Customer;
            var teaqlOriginalStatus = this.Status;
            var teaqlOriginalOrderNumber = this.OrderNumber;
            var teaqlOriginalOrderDate = this.OrderDate;
            var teaqlOriginalTotalAmount = this.TotalAmount;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Customer = teaqlOriginalCustomer;
                this.Status = teaqlOriginalStatus;
                this.OrderNumber = teaqlOriginalOrderNumber;
                this.OrderDate = teaqlOriginalOrderDate;
                this.TotalAmount = teaqlOriginalTotalAmount;
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
            if (creating)
            {
                var allocationKey = TeaqlEntityKey();
                Id = checked((long)await graph.ReservePreparedIdAsync(this));
                _teaqlForceCreate = true;
                _entityRoot.Rekey(allocationKey, TeaqlEntityKey());
            }
            }
            foreach (var child in OrderItemList) await child.TeaqlReserveGraphIdentitiesAsync(graph);
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<CustomerOrder> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            var creating = !this.Id.HasValue || _teaqlForceCreate;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("CustomerOrder",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("CustomerOrder",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for CustomerOrder");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Customer = saved.Customer;
            this.Status = saved.Status;
            this.OrderNumber = saved.OrderNumber;
            this.OrderDate = saved.OrderDate;
            this.TotalAmount = saved.TotalAmount;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _teaqlForceCreate = false;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            await TeaqlSaveChildrenAsync(context, graph, scope);
            return saved;
        }

        private async global::System.Threading.Tasks.Task TeaqlSaveChildrenAsync(UserContext context, GraphMutationSession graph, MutationTraceScope scope)
        {
            for (var index = 0; index < OrderItemList.Count; index++)
            {
                var child = OrderItemList[index];
                child.AttachRoot(_entityRoot);
                if (child.CustomerOrder != Id) child.UpdateCustomerOrderId(Id);
                try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_item_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        new CheckResult { RuleId = violation.RuleId, Location = violation.Location.PrefixedBy(prefix), EntityType = violation.EntityType, SourceInstancePath = violation.SourceInstancePath, InputValue = violation.InputValue, SystemValue = violation.SystemValue, Message = violation.Message }).ToArray());
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

        private LoadedScalarSnapshot TeaqlScalarSnapshot()
        {
            var record = new Record();
                        if (IsLoaded("Id")) record["id"] = TeaqlValue(Id);
                        if (IsLoaded("Customer")) record["customer"] = TeaqlValue(Customer);
                        if (IsLoaded("Status")) record["status"] = TeaqlValue(Status);
                        if (IsLoaded("OrderNumber")) record["order_number"] = TeaqlValue(OrderNumber);
                        if (IsLoaded("OrderDate")) record["order_date"] = TeaqlValue(OrderDate);
                        if (IsLoaded("TotalAmount")) record["total_amount"] = TeaqlValue(TotalAmount);
                        if (IsLoaded("Version")) record["version"] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record);
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Customer.HasValue) record["customer"] = new Value.I64Value(Customer.Value);

                    if (Status.HasValue) record["status"] = new Value.I64Value(Status.Value);

                    if (OrderNumber != null) record["order_number"] = new Value.TextValue(OrderNumber);

                    if (OrderDate.HasValue) record["order_date"] = new Value.DateValue(OrderDate.Value);

                    if (TotalAmount.HasValue) record["total_amount"] = new Value.DecimalValue(TotalAmount.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "CustomerOrder", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Customer.HasValue) record["customer"] = new Value.I64Value(Customer.Value);

                    if (Status.HasValue) record["status"] = new Value.I64Value(Status.Value);

                    if (OrderNumber != null) record["order_number"] = new Value.TextValue(OrderNumber);

                    if (OrderDate.HasValue) record["order_date"] = new Value.DateValue(OrderDate.Value);

                    if (TotalAmount.HasValue) record["total_amount"] = new Value.DecimalValue(TotalAmount.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "CustomerOrder", 
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
                Entity = "CustomerOrder",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
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

                public CustomerOrder UpdateCustomer(long? value)
                {
                    if (this.Customer != value) this.CustomerEntity = null;
                    this.Customer = value;
                    MarkLoaded("Customer");
                    _entityRoot.Set(TeaqlEntityKey(), "customer", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateStatus(long? value)
                {
                    if (this.Status != value) this.StatusEntity = null;
                    this.Status = value;
                    MarkLoaded("Status");
                    _entityRoot.Set(TeaqlEntityKey(), "status", TeaqlValue(value));
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

                public CustomerOrder UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public CustomerOrder UpdateCustomer(Customer value)
                {
                    UpdateCustomer(value?.Id);
                    this.CustomerEntity = value;
                    return this;
                }


                public CustomerOrder UpdateCustomerId(long? value)
                {
                    return UpdateCustomer(value);
                }


                public CustomerOrder UpdateStatus(OrderStatus value)
                {
                    UpdateStatus(value?.Id);
                    this.StatusEntity = value;
                    return this;
                }


                public CustomerOrder UpdateStatusId(long? value)
                {
                    return UpdateStatus(value);
                }
                public CustomerOrder UpdateStatusToNew()
                {
                    return UpdateStatus(1001);
                }

                public CustomerOrder UpdateStatusToPaid()
                {
                    return UpdateStatus(1002);
                }

                public CustomerOrder UpdateStatusToCancelled()
                {
                    return UpdateStatus(1003);
                }


    }
}