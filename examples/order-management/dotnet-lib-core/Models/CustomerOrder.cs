using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class CustomerOrder : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static class __TeaqlFields
        {
            internal const string ID_PROPERTY_MEMBER = "Id";
            internal const string ID_PROPERTY_QUERY = "id";
            internal const string ID_PROPERTY_STORAGE = "id";
            internal const string ID_PROPERTY_MODEL = "id";
            internal const string ORDER_NUMBER_PROPERTY_MEMBER = "OrderNumber";
            internal const string ORDER_NUMBER_PROPERTY_QUERY = "order_number";
            internal const string ORDER_NUMBER_PROPERTY_STORAGE = "order_number";
            internal const string ORDER_NUMBER_PROPERTY_MODEL = "order_number";
            internal const string ORDER_DATE_PROPERTY_MEMBER = "OrderDate";
            internal const string ORDER_DATE_PROPERTY_QUERY = "order_date";
            internal const string ORDER_DATE_PROPERTY_STORAGE = "order_date";
            internal const string ORDER_DATE_PROPERTY_MODEL = "order_date";
            internal const string TOTAL_AMOUNT_PROPERTY_MEMBER = "TotalAmount";
            internal const string TOTAL_AMOUNT_PROPERTY_QUERY = "total_amount";
            internal const string TOTAL_AMOUNT_PROPERTY_STORAGE = "total_amount";
            internal const string TOTAL_AMOUNT_PROPERTY_MODEL = "total_amount";
            internal const string STATUS_PROPERTY_MEMBER = "Status";
            internal const string STATUS_PROPERTY_QUERY = "status";
            internal const string STATUS_PROPERTY_STORAGE = "status";
            internal const string STATUS_PROPERTY_MODEL = "status";
            internal const string CUSTOMER_PROPERTY_MEMBER = "Customer";
            internal const string CUSTOMER_PROPERTY_QUERY = "customer";
            internal const string CUSTOMER_PROPERTY_STORAGE = "customer";
            internal const string CUSTOMER_PROPERTY_MODEL = "customer";
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

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("CustomerOrder", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, CustomerOrder.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Date, CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Decimal, CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, CustomerOrder.__TeaqlFields.STATUS_PROPERTY_STORAGE, () => global::Generated.Models.OrderStatus._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_STORAGE, () => global::Generated.Models.Customer._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, () => global::Generated.Models.CommercePlatform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(CustomerOrder.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, CustomerOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField("order_line_list", global::TeaQL.Runtime.EntityJsonKind.Many, "OrderLineList", () => global::Generated.Models.OrderLine._teaqlJsonSchema)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static CustomerOrder FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(CustomerOrder.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE, OrderNumber);
            }
            if (IsLoaded(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_STORAGE, OrderDate);
            }
            if (IsLoaded(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_STORAGE, TotalAmount);
            }
            if (IsLoaded(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER))
            {
                object? jsonStatus = IsLoaded("StatusEntity") && StatusEntity != null && StatusEntity.Id == Status
                    ? StatusEntity : Status;
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_STORAGE, jsonStatus);
            }
            if (IsLoaded(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER))
            {
                object? jsonCustomer = IsLoaded("CustomerEntity") && CustomerEntity != null && CustomerEntity.Id == Customer
                    ? CustomerEntity : Customer;
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_STORAGE, jsonCustomer);
            }
            if (IsLoaded(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER))
            {
                object? jsonCommercePlatform = IsLoaded("CommercePlatformEntity") && CommercePlatformEntity != null && CommercePlatformEntity.Id == CommercePlatform
                    ? CommercePlatformEntity : CommercePlatform;
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, jsonCommercePlatform);
            }
            if (IsLoaded(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, CreateTime);
            }
            if (IsLoaded(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, UpdateTime);
            }
            if (IsLoaded(CustomerOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(CustomerOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
            if (OrderLineList.IsLoaded || OrderLineList.Any(child => child.TeaqlHasPending()))
                yield return new KeyValuePair<string, object?>("OrderLineList", OrderLineList);
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("CustomerOrder", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal bool TeaqlCreationIntent => _teaqlForceCreate || _entityRoot.IsNew(TeaqlEntityKey());
        internal bool TeaqlHasPendingGraph(HashSet<object> visited)
        {
            if (!visited.Add(this)) return false;
            if (TeaqlCreationIntent || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey())) return true;
            foreach (var child in OrderLineList)
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
            foreach (var child in OrderLineList)
                if (child.TeaqlHasPendingGraph(visited))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_deletion_state", EntityType = "CustomerOrder", Location = ObjectLocation.Property("id"), Message = "Cancelled new entity has pending descendants; explicit resolution is required" } });
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
            foreach (var child in OrderLineList) child.AttachRoot(root, hydration);
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
        private CustomerOrder() { _creationState = EntityCreationState.Hydrated; }
        internal static CustomerOrder TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new CustomerOrder();
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
                public SmartList<OrderLine> OrderLineList { get; } = SmartList<OrderLine>.Empty();

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
        internal static CustomerOrder TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
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
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE, out var orderNumberValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER);
                        if (orderNumberValue.Raw != null)
                            entity.OrderNumber = Convert.ToString(orderNumberValue.Raw);
                    }
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_STORAGE, out var orderDateValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_MEMBER);
                        if (orderDateValue.Raw != null)
                            entity.OrderDate = TeaqlDateTime(orderDateValue);
                    }
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_STORAGE, out var totalAmountValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_MEMBER);
                        if (totalAmountValue.Raw != null)
                            entity.TotalAmount = Convert.ToDecimal(totalAmountValue.Raw);
                    }
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER, out var statusValue)
                        || record.TryGetValue(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_STORAGE, out statusValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER);
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
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER, out var customerValue)
                        || record.TryGetValue(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_STORAGE, out customerValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER);
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
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, out var commercePlatformValue)
                        || record.TryGetValue(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, out commercePlatformValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER);
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
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, out var createTimeValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER);
                        if (createTimeValue.Raw != null)
                            entity.CreateTime = TeaqlDateTime(createTimeValue);
                    }
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, out var updateTimeValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER);
                        if (updateTimeValue.Raw != null)
                            entity.UpdateTime = TeaqlDateTime(updateTimeValue);
                    }
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(CustomerOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
                        if (record.TryGetValue("OrderLineList", out var orderLineListValue))
                        {
                            entity.MarkLoaded("OrderLineList");
                            entity.OrderLineList.IsLoaded = true;
                            if (orderLineListValue is Value.ListValue rows)
                            {
                                if (rows.Values is SmartList<Value> carrier)
                                    entity.OrderLineList.Facets = carrier.Facets;
                                foreach (var row in rows.Values.OfType<Value.ObjectValue>())
                                    entity.OrderLineList.Add(global::Generated.Models.OrderLine.FromRecord(row.Value));
                            }
                        }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY, CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY, CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY,
                CustomerOrder.__TeaqlFields.ID_PROPERTY_MEMBER, CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER, CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_MEMBER, CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_MEMBER, CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER, CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER, CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER, CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER, CustomerOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER,
                "OrderLineList"
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
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
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

        internal void TeaqlValidateCreationGraph(HashSet<object> visited, bool relationWillChange = false)
        {
            if (!visited.Add(this)) return;
            if (TeaqlIsCancelledNew()) return;
            EntityCreation.RequireMutation(_creationState, Id, TeaqlCreationIntent,
                TeaqlCreationIntent || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()) || relationWillChange,
                _loadedSnapshot);
            foreach (var child in OrderLineList)
                child.TeaqlValidateCreationGraph(visited,
                    (child.TeaqlCreationIntent || child.IsLoaded("CustomerOrder")) && child.CustomerOrder != Id);
        }

        internal void TeaqlPrepareInputGraph(UserContext context, GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Prepare)) return;
            if (TeaqlIsCancelledNew()) return;
            graph.DiscardRestoredFields("CustomerOrder", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(CustomerOrder.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(CustomerOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property(CustomerOrder.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            for (var index = 0; index < OrderLineList.Count; index++)
            {
                var child = OrderLineList[index];
                if (child.TeaqlIsCancelledNew())
                {
                    child.TeaqlPrepareInputGraph(context, graph);
                    continue;
                }
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("CustomerOrder")) && child.CustomerOrder != Id) child.UpdateCustomerOrderId(Id);
                try { child.TeaqlPrepareInputGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_line_list").Index(index);
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
            for (var index = 0; index < OrderLineList.Count; index++)
            {
                try { OrderLineList[index].TeaqlPreflightGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_line_list").Index(index);
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
            var teaqlOriginalOrderNumber = this.OrderNumber;
            var teaqlOriginalOrderDate = this.OrderDate;
            var teaqlOriginalTotalAmount = this.TotalAmount;
            var teaqlOriginalStatus = this.Status;
            var teaqlOriginalCustomer = this.Customer;
            var teaqlOriginalCommercePlatform = this.CommercePlatform;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalUpdateTime = this.UpdateTime;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
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
            foreach (var child in OrderLineList) await child.TeaqlReserveGraphIdentitiesAsync(graph);
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<CustomerOrder> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
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
            _creationState = EntityCreationState.Hydrated;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            await TeaqlSaveChildrenAsync(context, graph, scope);
            return saved;
        }

        private async global::System.Threading.Tasks.Task TeaqlSaveChildrenAsync(UserContext context, GraphMutationSession graph, MutationTraceScope scope)
        {
            for (var index = 0; index < OrderLineList.Count; index++)
            {
                var child = OrderLineList[index];
                if (child.TeaqlIsCancelledNew()) continue;
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("CustomerOrder")) && child.CustomerOrder != Id) child.UpdateCustomerOrderId(Id);
                try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("order_line_list").Index(index);
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
                        if (IsLoaded(CustomerOrder.__TeaqlFields.ID_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE] = TeaqlValue(OrderNumber);
                        if (IsLoaded(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_STORAGE] = TeaqlValue(OrderDate);
                        if (IsLoaded(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_STORAGE] = TeaqlValue(TotalAmount);
                        if (IsLoaded(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.STATUS_PROPERTY_STORAGE] = TeaqlValue(Status);
                        if (IsLoaded(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_STORAGE] = TeaqlValue(Customer);
                        if (IsLoaded(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE] = TeaqlValue(CommercePlatform);
                        if (IsLoaded(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = TeaqlValue(CreateTime);
                        if (IsLoaded(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = TeaqlValue(UpdateTime);
                        if (IsLoaded(CustomerOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[CustomerOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE, out var orderNumberValue))
                        this.OrderNumber = orderNumberValue.Raw == null ? null : Convert.ToString(orderNumberValue.Raw);

                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_STORAGE, out var orderDateValue))
                        this.OrderDate = orderDateValue.Raw == null ? null : TeaqlDateTime(orderDateValue);

                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_STORAGE, out var totalAmountValue))
                        this.TotalAmount = totalAmountValue.Raw == null ? null : Convert.ToDecimal(totalAmountValue.Raw);

                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_STORAGE, out var statusValue))
                        this.Status = statusValue.Raw == null ? null : Convert.ToInt64(statusValue.Raw);

                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_STORAGE, out var customerValue))
                        this.Customer = customerValue.Raw == null ? null : Convert.ToInt64(customerValue.Raw);

                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, out var commercePlatformValue))
                        this.CommercePlatform = commercePlatformValue.Raw == null ? null : Convert.ToInt64(commercePlatformValue.Raw);

                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, out var createTimeValue))
                        this.CreateTime = createTimeValue.Raw == null ? null : TeaqlDateTime(createTimeValue);

                    if (record.TryGetValue(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, out var updateTimeValue))
                        this.UpdateTime = updateTimeValue.Raw == null ? null : TeaqlDateTime(updateTimeValue);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[CustomerOrder.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (OrderNumber != null) record[CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE] = new Value.TextValue(OrderNumber);

                    if (OrderDate.HasValue) record[CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_STORAGE] = new Value.DateValue(OrderDate.Value);

                    if (TotalAmount.HasValue) record[CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_STORAGE] = new Value.DecimalValue(TotalAmount.Value);

                    if (Status.HasValue) record[CustomerOrder.__TeaqlFields.STATUS_PROPERTY_STORAGE] = new Value.I64Value(Status.Value);

                    if (Customer.HasValue) record[CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_STORAGE] = new Value.I64Value(Customer.Value);

                    if (CommercePlatform.HasValue) record[CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record[CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record[CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record[CustomerOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "CustomerOrder", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (OrderNumber != null) record[CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE] = new Value.TextValue(OrderNumber);

                    if (OrderDate.HasValue) record[CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_STORAGE] = new Value.DateValue(OrderDate.Value);

                    if (TotalAmount.HasValue) record[CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_STORAGE] = new Value.DecimalValue(TotalAmount.Value);

                    if (Status.HasValue) record[CustomerOrder.__TeaqlFields.STATUS_PROPERTY_STORAGE] = new Value.I64Value(Status.Value);

                    if (Customer.HasValue) record[CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_STORAGE] = new Value.I64Value(Customer.Value);

                    if (CommercePlatform.HasValue) record[CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record[CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record[CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record[CustomerOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

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
                    MarkLoaded(CustomerOrder.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateOrderNumber(string value)
                {
                    this.OrderNumber = value;
                    MarkLoaded(CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateOrderDate(DateTime? value)
                {
                    this.OrderDate = value;
                    MarkLoaded(CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateTotalAmount(decimal? value)
                {
                    this.TotalAmount = value;
                    MarkLoaded(CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateStatus(long? value)
                {
                    if (this.Status != value) this.StatusEntity = null;
                    this.Status = value;
                    MarkLoaded(CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.STATUS_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateCustomer(long? value)
                {
                    if (this.Customer != value) this.CustomerEntity = null;
                    this.Customer = value;
                    MarkLoaded(CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateCommercePlatform(long? value)
                {
                    if (this.CommercePlatform != value) this.CommercePlatformEntity = null;
                    this.CommercePlatform = value;
                    MarkLoaded(CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded(CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded(CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(CustomerOrder.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), CustomerOrder.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
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
                public CustomerOrder UpdateStatusToPending()
                {
                    return UpdateStatus(1001);
                }

                public CustomerOrder UpdateStatusToConfirmed()
                {
                    return UpdateStatus(1002);
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


                public CustomerOrder UpdateCommercePlatform(CommercePlatform value)
                {
                    UpdateCommercePlatform(value?.Id);
                    this.CommercePlatformEntity = value;
                    return this;
                }


                public CustomerOrder UpdateCommercePlatformId(long? value)
                {
                    return UpdateCommercePlatform(value);
                }

    }
}