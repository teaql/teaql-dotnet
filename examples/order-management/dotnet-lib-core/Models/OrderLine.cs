using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class OrderLine : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("OrderLine", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField("id", global::TeaQL.Runtime.EntityJsonKind.Id, "id"),
            new global::TeaQL.Runtime.EntityJsonField("customer_order", global::TeaQL.Runtime.EntityJsonKind.Reference, "customer_order", () => global::Generated.Models.CustomerOrder._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("product", global::TeaQL.Runtime.EntityJsonKind.Reference, "product", () => global::Generated.Models.Product._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("product_name", global::TeaQL.Runtime.EntityJsonKind.Text, "product_name"),
            new global::TeaQL.Runtime.EntityJsonField("sku", global::TeaQL.Runtime.EntityJsonKind.Text, "sku"),
            new global::TeaQL.Runtime.EntityJsonField("quantity", global::TeaQL.Runtime.EntityJsonKind.Integer, "quantity"),
            new global::TeaQL.Runtime.EntityJsonField("commerce_platform", global::TeaQL.Runtime.EntityJsonKind.Reference, "commerce_platform", () => global::Generated.Models.CommercePlatform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("create_time", global::TeaQL.Runtime.EntityJsonKind.DateTime, "create_time"),
            new global::TeaQL.Runtime.EntityJsonField("version", global::TeaQL.Runtime.EntityJsonKind.Version, "version")}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static OrderLine FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded("Id"))
            {
                yield return new KeyValuePair<string, object?>("id", Id);
            }
            if (IsLoaded("CustomerOrder"))
            {
                object? jsonCustomerOrder = IsLoaded("CustomerOrderEntity") && CustomerOrderEntity != null && CustomerOrderEntity.Id == CustomerOrder
                    ? CustomerOrderEntity : CustomerOrder;
                yield return new KeyValuePair<string, object?>("customer_order", jsonCustomerOrder);
            }
            if (IsLoaded("Product"))
            {
                object? jsonProduct = IsLoaded("ProductEntity") && ProductEntity != null && ProductEntity.Id == Product
                    ? ProductEntity : Product;
                yield return new KeyValuePair<string, object?>("product", jsonProduct);
            }
            if (IsLoaded("ProductName"))
            {
                yield return new KeyValuePair<string, object?>("product_name", ProductName);
            }
            if (IsLoaded("Sku"))
            {
                yield return new KeyValuePair<string, object?>("sku", Sku);
            }
            if (IsLoaded("Quantity"))
            {
                yield return new KeyValuePair<string, object?>("quantity", Quantity);
            }
            if (IsLoaded("CommercePlatform"))
            {
                object? jsonCommercePlatform = IsLoaded("CommercePlatformEntity") && CommercePlatformEntity != null && CommercePlatformEntity.Id == CommercePlatform
                    ? CommercePlatformEntity : CommercePlatform;
                yield return new KeyValuePair<string, object?>("commerce_platform", jsonCommercePlatform);
            }
            if (IsLoaded("CreateTime"))
            {
                yield return new KeyValuePair<string, object?>("create_time", CreateTime);
            }
            if (IsLoaded("Version"))
            {
                yield return new KeyValuePair<string, object?>("version", Version);
            }
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("OrderLine", Id ?? _ledgerId);
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
            _comment = new MutationIntent(comment).Comment;
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
            var entity = new OrderLine();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static OrderLine FromRecord(Record record)
        {
            var entity = new OrderLine().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
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
                        else if (customerOrderValue.Raw == null)
                            entity.MarkLoaded("CustomerOrderEntity");
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
                        else if (productValue.Raw == null)
                            entity.MarkLoaded("ProductEntity");
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
                        else if (commercePlatformValue.Raw == null)
                            entity.MarkLoaded("CommercePlatformEntity");
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
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                "id", "customer_order", "product", "product_name", "sku", "quantity", "commerce_platform", "create_time", "version",
                "Id", "CustomerOrder", "Product", "ProductName", "Sku", "Quantity", "CommercePlatform", "CreateTime", "Version",
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static OrderLine FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<OrderLine> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("OrderLine");
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
            graph.DiscardRestoredFields("OrderLine", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = !Id.HasValue || _teaqlForceCreate;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            if (changed)
            {
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
                var originalVersion = _entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version;
                if (originalVersion.HasValue) ((UpdateCommand)command).Values["version"] = new Value.I64Value(originalVersion.Value);
            }
            graph.PrepareInput(this, TeaqlMutationRequest(command, graph.Intent.Comment));
            }
        }

        internal void TeaqlPreflightGraph(UserContext context, GraphMutationSession graph)
        {
            if (!Id.HasValue || _teaqlForceCreate || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()))
                graph.PreflightPrepared(this);
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
            var teaqlOriginalCustomerOrder = this.CustomerOrder;
            var teaqlOriginalProduct = this.Product;
            var teaqlOriginalProductName = this.ProductName;
            var teaqlOriginalSku = this.Sku;
            var teaqlOriginalQuantity = this.Quantity;
            var teaqlOriginalCommercePlatform = this.CommercePlatform;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
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
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<OrderLine> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            var creating = !this.Id.HasValue || _teaqlForceCreate;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("OrderLine",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("OrderLine",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for OrderLine");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
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

        private LoadedScalarSnapshot TeaqlScalarSnapshot()
        {
            var record = new Record();
                        if (IsLoaded("Id")) record["id"] = TeaqlValue(Id);
                        if (IsLoaded("CustomerOrder")) record["customer_order"] = TeaqlValue(CustomerOrder);
                        if (IsLoaded("Product")) record["product"] = TeaqlValue(Product);
                        if (IsLoaded("ProductName")) record["product_name"] = TeaqlValue(ProductName);
                        if (IsLoaded("Sku")) record["sku"] = TeaqlValue(Sku);
                        if (IsLoaded("Quantity")) record["quantity"] = TeaqlValue(Quantity);
                        if (IsLoaded("CommercePlatform")) record["commerce_platform"] = TeaqlValue(CommercePlatform);
                        if (IsLoaded("CreateTime")) record["create_time"] = TeaqlValue(CreateTime);
                        if (IsLoaded("Version")) record["version"] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record);
        }

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
                ExpectedVersionValue = _entityRoot.OriginalVersion(TeaqlEntityKey()) ?? this.Version,
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
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
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
                    if (this.CustomerOrder != value) this.CustomerOrderEntity = null;
                    this.CustomerOrder = value;
                    MarkLoaded("CustomerOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "customer_order", TeaqlValue(value));
                    return this;
                }

                public OrderLine UpdateProduct(long? value)
                {
                    if (this.Product != value) this.ProductEntity = null;
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
                    if (this.CommercePlatform != value) this.CommercePlatformEntity = null;
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
                    UpdateCustomerOrder(value?.Id);
                    this.CustomerOrderEntity = value;
                    return this;
                }


                public OrderLine UpdateCustomerOrderId(long? value)
                {
                    return UpdateCustomerOrder(value);
                }


                public OrderLine UpdateProduct(Product value)
                {
                    UpdateProduct(value?.Id);
                    this.ProductEntity = value;
                    return this;
                }


                public OrderLine UpdateProductId(long? value)
                {
                    return UpdateProduct(value);
                }


                public OrderLine UpdateCommercePlatform(CommercePlatform value)
                {
                    UpdateCommercePlatform(value?.Id);
                    this.CommercePlatformEntity = value;
                    return this;
                }


                public OrderLine UpdateCommercePlatformId(long? value)
                {
                    return UpdateCommercePlatform(value);
                }

    }
}