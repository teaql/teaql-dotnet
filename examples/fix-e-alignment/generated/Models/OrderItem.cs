using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class OrderItem
    {
        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("OrderItem", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; }  }
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
        public OrderItem() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Id { get; set; }
                public long? PurchaseOrder { get; set; }
                public long? Tenant { get; set; }
                public string? ProductName { get; set; }
                public decimal? UnitPrice { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public PurchaseOrder? PurchaseOrderEntity { get; set; }
                public Tenant? TenantEntity { get; set; }

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

        public OrderItem MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public OrderItem MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public OrderItem AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public OrderItem MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static OrderItem Refer(long id)
        {
            return new OrderItem { Id = id }.MarkLoadedOnly("Id");
        }

        public static OrderItem FromRecord(Record record)
        {
            var entity = new OrderItem().MarkLoadedOnly();
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("PurchaseOrder", out var purchaseOrderValue)
                        || record.TryGetValue("purchase_order", out purchaseOrderValue))
                    {
                        entity.MarkLoaded("PurchaseOrder");
                        if (purchaseOrderValue.Raw is Record purchaseOrderRow)
                        {
                            entity.PurchaseOrderEntity = global::Generated.Models.PurchaseOrder.FromRecord(purchaseOrderRow);
                            entity.PurchaseOrder = entity.PurchaseOrderEntity.Id;
                            entity.MarkLoaded("PurchaseOrderEntity");
                        }
                        else if (purchaseOrderValue.Raw is IEnumerable<Record> purchaseOrderRows)
                        {
                            foreach (var row in purchaseOrderRows)
                            {
                                entity.PurchaseOrderEntity = global::Generated.Models.PurchaseOrder.FromRecord(row);
                                entity.PurchaseOrder = entity.PurchaseOrderEntity.Id;
                                entity.MarkLoaded("PurchaseOrderEntity");
                                break;
                            }
                        }
                        else if (purchaseOrderValue.Raw != null)
                            entity.PurchaseOrder = Convert.ToInt64(purchaseOrderValue.Raw);
                    }
                    if (record.TryGetValue("Tenant", out var tenantValue)
                        || record.TryGetValue("tenant", out tenantValue))
                    {
                        entity.MarkLoaded("Tenant");
                        if (tenantValue.Raw is Record tenantRow)
                        {
                            entity.TenantEntity = global::Generated.Models.Tenant.FromRecord(tenantRow);
                            entity.Tenant = entity.TenantEntity.Id;
                            entity.MarkLoaded("TenantEntity");
                        }
                        else if (tenantValue.Raw is IEnumerable<Record> tenantRows)
                        {
                            foreach (var row in tenantRows)
                            {
                                entity.TenantEntity = global::Generated.Models.Tenant.FromRecord(row);
                                entity.Tenant = entity.TenantEntity.Id;
                                entity.MarkLoaded("TenantEntity");
                                break;
                            }
                        }
                        else if (tenantValue.Raw != null)
                            entity.Tenant = Convert.ToInt64(tenantValue.Raw);
                    }
                    if (record.TryGetValue("product_name", out var productNameValue))
                    {
                        entity.MarkLoaded("ProductName");
                        if (productNameValue.Raw != null)
                            entity.ProductName = Convert.ToString(productNameValue.Raw);
                    }
                    if (record.TryGetValue("unit_price", out var unitPriceValue))
                    {
                        entity.MarkLoaded("UnitPrice");
                        if (unitPriceValue.Raw != null)
                            entity.UnitPrice = Convert.ToDecimal(unitPriceValue.Raw);
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
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                "id", "purchase_order", "tenant", "product_name", "unit_price", "create_time", "update_time", "version",
                "Id", "PurchaseOrder", "Tenant", "ProductName", "UnitPrice", "CreateTime", "UpdateTime", "Version",
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            return entity;
        }

        internal static OrderItem FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root);
            return entity;
        }

        public async Task<OrderItem> SaveAsync(UserContext context)
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
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("PurchaseOrder"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("purchase_order"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Tenant"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("tenant"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("ProductName"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("product_name"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("UnitPrice"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("unit_price"), Message = "Mutation requires a fully loaded entity" } });
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
            graph.Preflight(TeaqlMutationRequest(command, string.IsNullOrWhiteSpace(_comment) ? graph.Intent.Comment : _comment!));
            }
        }

        internal async Task<OrderItem> TeaqlSaveWithinGraphAsync(UserContext context, GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            var scope = graph.Scope("OrderItem", Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            if (Id.HasValue && !_teaqlForceCreate && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                return this;
            }
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
            var teaqlOriginalFullyLoaded = _fullyLoaded;
            var teaqlOriginalLoadedFields = new HashSet<string>(_loadedFields, StringComparer.OrdinalIgnoreCase);
            var teaqlOriginalId = this.Id;
            var teaqlOriginalPurchaseOrder = this.PurchaseOrder;
            var teaqlOriginalTenant = this.Tenant;
            var teaqlOriginalProductName = this.ProductName;
            var teaqlOriginalUnitPrice = this.UnitPrice;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalUpdateTime = this.UpdateTime;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.PurchaseOrder = teaqlOriginalPurchaseOrder;
                this.Tenant = teaqlOriginalTenant;
                this.ProductName = teaqlOriginalProductName;
                this.UnitPrice = teaqlOriginalUnitPrice;
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
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for OrderItem");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.PurchaseOrder = saved.PurchaseOrder;
            this.Tenant = saved.Tenant;
            this.ProductName = saved.ProductName;
            this.UnitPrice = saved.UnitPrice;
            this.CreateTime = saved.CreateTime;
            this.UpdateTime = saved.UpdateTime;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _teaqlForceCreate = false;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
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
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (PurchaseOrder.HasValue) record["purchase_order"] = new Value.I64Value(PurchaseOrder.Value);

                    if (Tenant.HasValue) record["tenant"] = new Value.I64Value(Tenant.Value);

                    if (ProductName != null) record["product_name"] = new Value.TextValue(ProductName);

                    if (UnitPrice.HasValue) record["unit_price"] = new Value.DecimalValue(UnitPrice.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderItem", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (PurchaseOrder.HasValue) record["purchase_order"] = new Value.I64Value(PurchaseOrder.Value);

                    if (Tenant.HasValue) record["tenant"] = new Value.I64Value(Tenant.Value);

                    if (ProductName != null) record["product_name"] = new Value.TextValue(ProductName);

                    if (UnitPrice.HasValue) record["unit_price"] = new Value.DecimalValue(UnitPrice.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "OrderItem", 
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
                Entity = "OrderItem",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("OrderItem");
        }

                public OrderItem UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public OrderItem UpdatePurchaseOrder(long? value)
                {
                    this.PurchaseOrder = value;
                    MarkLoaded("PurchaseOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "purchase_order", TeaqlValue(value));
                    return this;
                }

                internal OrderItem UpdateTenant(long? value)
                {
                    this.Tenant = value;
                    MarkLoaded("Tenant");
                    _entityRoot.Set(TeaqlEntityKey(), "tenant", TeaqlValue(value));
                    return this;
                }

                public OrderItem UpdateProductName(string value)
                {
                    this.ProductName = value;
                    MarkLoaded("ProductName");
                    _entityRoot.Set(TeaqlEntityKey(), "product_name", TeaqlValue(value));
                    return this;
                }

                public OrderItem UpdateUnitPrice(decimal? value)
                {
                    this.UnitPrice = value;
                    MarkLoaded("UnitPrice");
                    _entityRoot.Set(TeaqlEntityKey(), "unit_price", TeaqlValue(value));
                    return this;
                }

                public OrderItem UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "create_time", TeaqlValue(value));
                    return this;
                }

                public OrderItem UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "update_time", TeaqlValue(value));
                    return this;
                }

                public OrderItem UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public OrderItem UpdatePurchaseOrder(PurchaseOrder value)
                {
                    this.PurchaseOrder = value?.Id;
                    MarkLoaded("PurchaseOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "purchase_order", TeaqlValue(this.PurchaseOrder));
                    return this;
                }


                public OrderItem UpdatePurchaseOrderId(long? value)
                {
                    this.PurchaseOrder = value;
                    MarkLoaded("PurchaseOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "purchase_order", TeaqlValue(value));
                    return this;
                }


                internal OrderItem UpdateTenant(Tenant value)
                {
                    this.Tenant = value?.Id;
                    MarkLoaded("Tenant");
                    _entityRoot.Set(TeaqlEntityKey(), "tenant", TeaqlValue(this.Tenant));
                    return this;
                }


                internal OrderItem UpdateTenantId(long? value)
                {
                    this.Tenant = value;
                    MarkLoaded("Tenant");
                    _entityRoot.Set(TeaqlEntityKey(), "tenant", TeaqlValue(value));
                    return this;
                }

    }
}