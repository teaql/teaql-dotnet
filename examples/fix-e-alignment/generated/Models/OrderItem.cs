using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class OrderItem : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("OrderItem", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField("id", global::TeaQL.Runtime.EntityJsonKind.Id, "id"),
            new global::TeaQL.Runtime.EntityJsonField("purchase_order", global::TeaQL.Runtime.EntityJsonKind.Reference, "purchase_order", () => global::Generated.Models.PurchaseOrder._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("tenant", global::TeaQL.Runtime.EntityJsonKind.Reference, "tenant", () => global::Generated.Models.Tenant._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("product_name", global::TeaQL.Runtime.EntityJsonKind.Text, "product_name"),
            new global::TeaQL.Runtime.EntityJsonField("unit_price", global::TeaQL.Runtime.EntityJsonKind.Decimal, "unit_price"),
            new global::TeaQL.Runtime.EntityJsonField("create_time", global::TeaQL.Runtime.EntityJsonKind.DateTime, "create_time"),
            new global::TeaQL.Runtime.EntityJsonField("update_time", global::TeaQL.Runtime.EntityJsonKind.DateTime, "update_time"),
            new global::TeaQL.Runtime.EntityJsonField("version", global::TeaQL.Runtime.EntityJsonKind.Version, "version")}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static OrderItem FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded("Id"))
            {
                yield return new KeyValuePair<string, object?>("id", Id);
            }
            if (IsLoaded("PurchaseOrder"))
            {
                object? jsonPurchaseOrder = IsLoaded("PurchaseOrderEntity") && PurchaseOrderEntity != null && PurchaseOrderEntity.Id == PurchaseOrder
                    ? PurchaseOrderEntity : PurchaseOrder;
                yield return new KeyValuePair<string, object?>("purchase_order", jsonPurchaseOrder);
            }
            if (IsLoaded("Tenant"))
            {
                object? jsonTenant = IsLoaded("TenantEntity") && TenantEntity != null && TenantEntity.Id == Tenant
                    ? TenantEntity : Tenant;
                yield return new KeyValuePair<string, object?>("tenant", jsonTenant);
            }
            if (IsLoaded("ProductName"))
            {
                yield return new KeyValuePair<string, object?>("product_name", ProductName);
            }
            if (IsLoaded("UnitPrice"))
            {
                yield return new KeyValuePair<string, object?>("unit_price", UnitPrice);
            }
            if (IsLoaded("CreateTime"))
            {
                yield return new KeyValuePair<string, object?>("create_time", CreateTime);
            }
            if (IsLoaded("UpdateTime"))
            {
                yield return new KeyValuePair<string, object?>("update_time", UpdateTime);
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
        private EntityKey TeaqlEntityKey() => new EntityKey("OrderItem", Id ?? _ledgerId);
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
            _comment = new MutationIntent(comment).Comment;
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
            var entity = new OrderItem();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static OrderItem FromRecord(Record record)
        {
            var entity = new OrderItem().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
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
                        else if (purchaseOrderValue.Raw == null)
                            entity.MarkLoaded("PurchaseOrderEntity");
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
                        else if (tenantValue.Raw == null)
                            entity.MarkLoaded("TenantEntity");
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
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static OrderItem FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<OrderItem> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("OrderItem");
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
            graph.DiscardRestoredFields("OrderItem", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = !Id.HasValue || _teaqlForceCreate;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderItem", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("PurchaseOrder"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderItem", Location = ObjectLocation.Property("purchase_order"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Tenant"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderItem", Location = ObjectLocation.Property("tenant"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("ProductName"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderItem", Location = ObjectLocation.Property("product_name"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("UnitPrice"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderItem", Location = ObjectLocation.Property("unit_price"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("CreateTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderItem", Location = ObjectLocation.Property("create_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("UpdateTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderItem", Location = ObjectLocation.Property("update_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Version"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderItem", Location = ObjectLocation.Property("version"), Message = "Mutation requires a fully loaded entity" } });
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

        internal async global::System.Threading.Tasks.Task<OrderItem> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            var creating = !this.Id.HasValue || _teaqlForceCreate;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("OrderItem",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("OrderItem",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for OrderItem");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
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
                        if (IsLoaded("PurchaseOrder")) record["purchase_order"] = TeaqlValue(PurchaseOrder);
                        if (IsLoaded("Tenant")) record["tenant"] = TeaqlValue(Tenant);
                        if (IsLoaded("ProductName")) record["product_name"] = TeaqlValue(ProductName);
                        if (IsLoaded("UnitPrice")) record["unit_price"] = TeaqlValue(UnitPrice);
                        if (IsLoaded("CreateTime")) record["create_time"] = TeaqlValue(CreateTime);
                        if (IsLoaded("UpdateTime")) record["update_time"] = TeaqlValue(UpdateTime);
                        if (IsLoaded("Version")) record["version"] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record);
        }

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
                ExpectedVersionValue = _entityRoot.OriginalVersion(TeaqlEntityKey()) ?? this.Version,
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
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
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
                    if (this.PurchaseOrder != value) this.PurchaseOrderEntity = null;
                    this.PurchaseOrder = value;
                    MarkLoaded("PurchaseOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "purchase_order", TeaqlValue(value));
                    return this;
                }

                internal OrderItem UpdateTenant(long? value)
                {
                    if (this.Tenant != value) this.TenantEntity = null;
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
                    UpdatePurchaseOrder(value?.Id);
                    this.PurchaseOrderEntity = value;
                    return this;
                }


                public OrderItem UpdatePurchaseOrderId(long? value)
                {
                    return UpdatePurchaseOrder(value);
                }


                internal OrderItem UpdateTenant(Tenant value)
                {
                    UpdateTenant(value?.Id);
                    this.TenantEntity = value;
                    return this;
                }


                internal OrderItem UpdateTenantId(long? value)
                {
                    return UpdateTenant(value);
                }

    }
}