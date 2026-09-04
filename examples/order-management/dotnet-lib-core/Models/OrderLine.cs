using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{
    public class OrderLine
    {
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("OrderLine", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; }  }
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
        public OrderLine() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Id { get; set; }
                public long? CustomerOrder { get; set; }
                public long? Product { get; set; }
                public string? ProductName { get; set; }
                public string? Sku { get; set; }
                public long? Quantity { get; set; }
                public long? CommercePlatform { get; set; }
                public DateTime? CreateTime { get; set; }
                public long? Version { get; set; }
                public CustomerOrder? CustomerOrderEntity { get; set; }
                public Product? ProductEntity { get; set; }
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

        public OrderLine MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public OrderLine MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public OrderLine AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public OrderLine MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static OrderLine Refer(long id)
        {
            return new OrderLine { Id = id }.MarkLoadedOnly("Id");
        }

        public static OrderLine FromRecord(Record record)
        {
            var entity = new OrderLine().MarkLoadedOnly();
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("CustomerOrder", out var customerOrderValue)
                        || record.TryGetValue("customer_order", out customerOrderValue))
                    {
                        entity.MarkLoaded("CustomerOrder");
                        if (customerOrderValue.Raw is Record customerOrderRow)
                        {
                            entity.CustomerOrderEntity = global::Generated.Models.CustomerOrder.FromRecord(customerOrderRow);
                            entity.CustomerOrder = entity.CustomerOrderEntity.Id;
                            entity.MarkLoaded("CustomerOrderEntity");
                        }
                        else if (customerOrderValue.Raw is IEnumerable<Record> customerOrderRows)
                        {
                            foreach (var row in customerOrderRows)
                            {
                                entity.CustomerOrderEntity = global::Generated.Models.CustomerOrder.FromRecord(row);
                                entity.CustomerOrder = entity.CustomerOrderEntity.Id;
                                entity.MarkLoaded("CustomerOrderEntity");
                                break;
                            }
                        }
                        else if (customerOrderValue.Raw != null)
                            entity.CustomerOrder = Convert.ToInt64(customerOrderValue.Raw);
                    }
                    if (record.TryGetValue("Product", out var productValue)
                        || record.TryGetValue("product", out productValue))
                    {
                        entity.MarkLoaded("Product");
                        if (productValue.Raw is Record productRow)
                        {
                            entity.ProductEntity = global::Generated.Models.Product.FromRecord(productRow);
                            entity.Product = entity.ProductEntity.Id;
                            entity.MarkLoaded("ProductEntity");
                        }
                        else if (productValue.Raw is IEnumerable<Record> productRows)
                        {
                            foreach (var row in productRows)
                            {
                                entity.ProductEntity = global::Generated.Models.Product.FromRecord(row);
                                entity.Product = entity.ProductEntity.Id;
                                entity.MarkLoaded("ProductEntity");
                                break;
                            }
                        }
                        else if (productValue.Raw != null)
                            entity.Product = Convert.ToInt64(productValue.Raw);
                    }
                    if (record.TryGetValue("product_name", out var productNameValue))
                    {
                        entity.MarkLoaded("ProductName");
                        if (productNameValue.Raw != null)
                            entity.ProductName = Convert.ToString(productNameValue.Raw);
                    }
                    if (record.TryGetValue("sku", out var skuValue))
                    {
                        entity.MarkLoaded("Sku");
                        if (skuValue.Raw != null)
                            entity.Sku = Convert.ToString(skuValue.Raw);
                    }
                    if (record.TryGetValue("quantity", out var quantityValue))
                    {
                        entity.MarkLoaded("Quantity");
                        if (quantityValue.Raw != null)
                            entity.Quantity = Convert.ToInt64(quantityValue.Raw);
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

        internal static OrderLine FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root);
            return entity;
        }

        public async Task<OrderLine> SaveAsync(UserContext context)
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
                if (!IsLoaded("CustomerOrder"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("customer_order"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Product"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("product"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("ProductName"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("product_name"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Sku"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("sku"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Quantity"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("quantity"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("CommercePlatform"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("commerce_platform"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("CreateTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("create_time"), Message = "Mutation requires a fully loaded entity" } });
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
        }

        internal async Task<OrderLine> TeaqlSaveWithinGraphAsync(UserContext context)
        {
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
            var teaqlOriginalFullyLoaded = _fullyLoaded;
            var teaqlOriginalLoadedFields = new HashSet<string>(_loadedFields, StringComparer.OrdinalIgnoreCase);
            var teaqlOriginalId = this.Id;
            var teaqlOriginalCustomerOrder = this.CustomerOrder;
            var teaqlOriginalProduct = this.Product;
            var teaqlOriginalProductName = this.ProductName;
            var teaqlOriginalSku = this.Sku;
            var teaqlOriginalQuantity = this.Quantity;
            var teaqlOriginalCommercePlatform = this.CommercePlatform;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalVersion = this.Version;
            context.AfterGraphRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.CustomerOrder = teaqlOriginalCustomerOrder;
                this.Product = teaqlOriginalProduct;
                this.ProductName = teaqlOriginalProductName;
                this.Sku = teaqlOriginalSku;
                this.Quantity = teaqlOriginalQuantity;
                this.CommercePlatform = teaqlOriginalCommercePlatform;
                this.CreateTime = teaqlOriginalCreateTime;
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
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for OrderLine");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.CustomerOrder = saved.CustomerOrder;
            this.Product = saved.Product;
            this.ProductName = saved.ProductName;
            this.Sku = saved.Sku;
            this.Quantity = saved.Quantity;
            this.CommercePlatform = saved.CommercePlatform;
            this.CreateTime = saved.CreateTime;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _teaqlForceCreate = false;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
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

                    if (CustomerOrder.HasValue) record["customer_order"] = new Value.I64Value(CustomerOrder.Value);

                    if (Product.HasValue) record["product"] = new Value.I64Value(Product.Value);

                    if (ProductName != null) record["product_name"] = new Value.TextValue(ProductName);

                    if (Sku != null) record["sku"] = new Value.TextValue(Sku);

                    if (Quantity.HasValue) record["quantity"] = new Value.I64Value(Quantity.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderLine", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (CustomerOrder.HasValue) record["customer_order"] = new Value.I64Value(CustomerOrder.Value);

                    if (Product.HasValue) record["product"] = new Value.I64Value(Product.Value);

                    if (ProductName != null) record["product_name"] = new Value.TextValue(ProductName);

                    if (Sku != null) record["sku"] = new Value.TextValue(Sku);

                    if (Quantity.HasValue) record["quantity"] = new Value.I64Value(Quantity.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "OrderLine", 
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
                Entity = "OrderLine",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("OrderLine");
        }

                public OrderLine UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public OrderLine UpdateCustomerOrder(long? value)
                {
                    this.CustomerOrder = value;
                    MarkLoaded("CustomerOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "customer_order", TeaqlValue(value));
                    return this;
                }

                public OrderLine UpdateProduct(long? value)
                {
                    this.Product = value;
                    MarkLoaded("Product");
                    _entityRoot.Set(TeaqlEntityKey(), "product", TeaqlValue(value));
                    return this;
                }

                public OrderLine UpdateProductName(string value)
                {
                    this.ProductName = value;
                    MarkLoaded("ProductName");
                    _entityRoot.Set(TeaqlEntityKey(), "product_name", TeaqlValue(value));
                    return this;
                }

                public OrderLine UpdateSku(string value)
                {
                    this.Sku = value;
                    MarkLoaded("Sku");
                    _entityRoot.Set(TeaqlEntityKey(), "sku", TeaqlValue(value));
                    return this;
                }

                public OrderLine UpdateQuantity(long? value)
                {
                    this.Quantity = value;
                    MarkLoaded("Quantity");
                    _entityRoot.Set(TeaqlEntityKey(), "quantity", TeaqlValue(value));
                    return this;
                }

                public OrderLine UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(value));
                    return this;
                }

                public OrderLine UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "create_time", TeaqlValue(value));
                    return this;
                }

                public OrderLine UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public OrderLine UpdateCustomerOrder(CustomerOrder value)
                {
                    this.CustomerOrder = value?.Id;
                    MarkLoaded("CustomerOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "customer_order", TeaqlValue(this.CustomerOrder));
                    return this;
                }


                public OrderLine UpdateCustomerOrderId(long? value)
                {
                    this.CustomerOrder = value;
                    MarkLoaded("CustomerOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "customer_order", TeaqlValue(value));
                    return this;
                }


                public OrderLine UpdateProduct(Product value)
                {
                    this.Product = value?.Id;
                    MarkLoaded("Product");
                    _entityRoot.Set(TeaqlEntityKey(), "product", TeaqlValue(this.Product));
                    return this;
                }


                public OrderLine UpdateProductId(long? value)
                {
                    this.Product = value;
                    MarkLoaded("Product");
                    _entityRoot.Set(TeaqlEntityKey(), "product", TeaqlValue(value));
                    return this;
                }


                public OrderLine UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(this.CommercePlatform));
                    return this;
                }


                public OrderLine UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "commerce_platform", TeaqlValue(value));
                    return this;
                }

    }
}