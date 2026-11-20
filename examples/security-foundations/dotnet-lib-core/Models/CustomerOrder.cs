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
            new global::TeaQL.Runtime.EntityJsonField("merchant", global::TeaQL.Runtime.EntityJsonKind.Reference, "merchant", () => global::Generated.Models.Merchant._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("customer_profile", global::TeaQL.Runtime.EntityJsonKind.Reference, "customer_profile", () => global::Generated.Models.CustomerProfile._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("order_number", global::TeaQL.Runtime.EntityJsonKind.Text, "order_number"),
            new global::TeaQL.Runtime.EntityJsonField("fulfillment_status", global::TeaQL.Runtime.EntityJsonKind.Text, "fulfillment_status"),
            new global::TeaQL.Runtime.EntityJsonField("version", global::TeaQL.Runtime.EntityJsonKind.Version, "version")}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static CustomerOrder FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded("Id"))
            {
                yield return new KeyValuePair<string, object?>("id", Id);
            }
            if (IsLoaded("Merchant"))
            {
                object? jsonMerchant = IsLoaded("MerchantEntity") && MerchantEntity != null && MerchantEntity.Id == Merchant
                    ? MerchantEntity : Merchant;
                yield return new KeyValuePair<string, object?>("merchant", jsonMerchant);
            }
            if (IsLoaded("CustomerProfile"))
            {
                object? jsonCustomerProfile = IsLoaded("CustomerProfileEntity") && CustomerProfileEntity != null && CustomerProfileEntity.Id == CustomerProfile
                    ? CustomerProfileEntity : CustomerProfile;
                yield return new KeyValuePair<string, object?>("customer_profile", jsonCustomerProfile);
            }
            if (IsLoaded("OrderNumber"))
            {
                yield return new KeyValuePair<string, object?>("order_number", OrderNumber);
            }
            if (IsLoaded("FulfillmentStatus"))
            {
                yield return new KeyValuePair<string, object?>("fulfillment_status", FulfillmentStatus);
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
                public long? Merchant { get; set; }
                public long? CustomerProfile { get; set; }
                public string? OrderNumber { get; set; }
                public string? FulfillmentStatus { get; set; }
                public long? Version { get; set; }
                public Merchant? MerchantEntity { get; set; }
                public CustomerProfile? CustomerProfileEntity { get; set; }

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
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("Merchant", out var merchantValue)
                        || record.TryGetValue("merchant", out merchantValue))
                    {
                        entity.MarkLoaded("Merchant");
                        if (merchantValue.Raw is Record merchantRow)
                        {
                            entity.MerchantEntity = global::Generated.Models.Merchant.FromRecord(merchantRow);
                            entity.Merchant = entity.MerchantEntity.Id;
                            entity.MarkLoaded("MerchantEntity");
                        }
                        else if (merchantValue.Raw is IEnumerable<Record> merchantRows)
                        {
                            foreach (var row in merchantRows)
                            {
                                entity.MerchantEntity = global::Generated.Models.Merchant.FromRecord(row);
                                entity.Merchant = entity.MerchantEntity.Id;
                                entity.MarkLoaded("MerchantEntity");
                                break;
                            }
                        }
                        else if (merchantValue.Raw == null)
                            entity.MarkLoaded("MerchantEntity");
                        else if (merchantValue.Raw != null)
                            entity.Merchant = Convert.ToInt64(merchantValue.Raw);
                    }
                    if (record.TryGetValue("CustomerProfile", out var customerProfileValue)
                        || record.TryGetValue("customer_profile", out customerProfileValue))
                    {
                        entity.MarkLoaded("CustomerProfile");
                        if (customerProfileValue.Raw is Record customerProfileRow)
                        {
                            entity.CustomerProfileEntity = global::Generated.Models.CustomerProfile.FromRecord(customerProfileRow);
                            entity.CustomerProfile = entity.CustomerProfileEntity.Id;
                            entity.MarkLoaded("CustomerProfileEntity");
                        }
                        else if (customerProfileValue.Raw is IEnumerable<Record> customerProfileRows)
                        {
                            foreach (var row in customerProfileRows)
                            {
                                entity.CustomerProfileEntity = global::Generated.Models.CustomerProfile.FromRecord(row);
                                entity.CustomerProfile = entity.CustomerProfileEntity.Id;
                                entity.MarkLoaded("CustomerProfileEntity");
                                break;
                            }
                        }
                        else if (customerProfileValue.Raw == null)
                            entity.MarkLoaded("CustomerProfileEntity");
                        else if (customerProfileValue.Raw != null)
                            entity.CustomerProfile = Convert.ToInt64(customerProfileValue.Raw);
                    }
                    if (record.TryGetValue("order_number", out var orderNumberValue))
                    {
                        entity.MarkLoaded("OrderNumber");
                        if (orderNumberValue.Raw != null)
                            entity.OrderNumber = Convert.ToString(orderNumberValue.Raw);
                    }
                    if (record.TryGetValue("fulfillment_status", out var fulfillmentStatusValue))
                    {
                        entity.MarkLoaded("FulfillmentStatus");
                        if (fulfillmentStatusValue.Raw != null)
                            entity.FulfillmentStatus = Convert.ToString(fulfillmentStatusValue.Raw);
                    }
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                "id", "merchant", "customer_profile", "order_number", "fulfillment_status", "version",
                "Id", "Merchant", "CustomerProfile", "OrderNumber", "FulfillmentStatus", "Version",
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
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Merchant"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property("merchant"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("CustomerProfile"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property("customer_profile"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OrderNumber"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property("order_number"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("FulfillmentStatus"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property("fulfillment_status"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Version"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "CustomerOrder", Location = ObjectLocation.Property("version"), Message = "Mutation requires a fully loaded entity" } });
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
        }

        internal void TeaqlPreflightGraph(UserContext context, GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Preflight)) return;
            if (TeaqlIsCancelledNew()) return;
            if (!Id.HasValue || _teaqlForceCreate || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()))
                graph.PreflightPrepared(this);
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
            var teaqlOriginalMerchant = this.Merchant;
            var teaqlOriginalCustomerProfile = this.CustomerProfile;
            var teaqlOriginalOrderNumber = this.OrderNumber;
            var teaqlOriginalFulfillmentStatus = this.FulfillmentStatus;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Merchant = teaqlOriginalMerchant;
                this.CustomerProfile = teaqlOriginalCustomerProfile;
                this.OrderNumber = teaqlOriginalOrderNumber;
                this.FulfillmentStatus = teaqlOriginalFulfillmentStatus;
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
            this.Merchant = saved.Merchant;
            this.CustomerProfile = saved.CustomerProfile;
            this.OrderNumber = saved.OrderNumber;
            this.FulfillmentStatus = saved.FulfillmentStatus;
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
                        if (IsLoaded("Id")) record["id"] = TeaqlValue(Id);
                        if (IsLoaded("Merchant")) record["merchant"] = TeaqlValue(Merchant);
                        if (IsLoaded("CustomerProfile")) record["customer_profile"] = TeaqlValue(CustomerProfile);
                        if (IsLoaded("OrderNumber")) record["order_number"] = TeaqlValue(OrderNumber);
                        if (IsLoaded("FulfillmentStatus")) record["fulfillment_status"] = TeaqlValue(FulfillmentStatus);
                        if (IsLoaded("Version")) record["version"] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue("merchant", out var merchantValue))
                        this.Merchant = merchantValue.Raw == null ? null : Convert.ToInt64(merchantValue.Raw);

                    if (record.TryGetValue("customer_profile", out var customerProfileValue))
                        this.CustomerProfile = customerProfileValue.Raw == null ? null : Convert.ToInt64(customerProfileValue.Raw);

                    if (record.TryGetValue("order_number", out var orderNumberValue))
                        this.OrderNumber = orderNumberValue.Raw == null ? null : Convert.ToString(orderNumberValue.Raw);

                    if (record.TryGetValue("fulfillment_status", out var fulfillmentStatusValue))
                        this.FulfillmentStatus = fulfillmentStatusValue.Raw == null ? null : Convert.ToString(fulfillmentStatusValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Merchant.HasValue) record["merchant"] = new Value.I64Value(Merchant.Value);

                    if (CustomerProfile.HasValue) record["customer_profile"] = new Value.I64Value(CustomerProfile.Value);

                    if (OrderNumber != null) record["order_number"] = new Value.TextValue(OrderNumber);

                    if (FulfillmentStatus != null) record["fulfillment_status"] = new Value.TextValue(FulfillmentStatus);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "CustomerOrder", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Merchant.HasValue) record["merchant"] = new Value.I64Value(Merchant.Value);

                    if (CustomerProfile.HasValue) record["customer_profile"] = new Value.I64Value(CustomerProfile.Value);

                    if (OrderNumber != null) record["order_number"] = new Value.TextValue(OrderNumber);

                    if (FulfillmentStatus != null) record["fulfillment_status"] = new Value.TextValue(FulfillmentStatus);

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

                public CustomerOrder UpdateMerchant(long? value)
                {
                    if (this.Merchant != value) this.MerchantEntity = null;
                    this.Merchant = value;
                    MarkLoaded("Merchant");
                    _entityRoot.Set(TeaqlEntityKey(), "merchant", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateCustomerProfile(long? value)
                {
                    if (this.CustomerProfile != value) this.CustomerProfileEntity = null;
                    this.CustomerProfile = value;
                    MarkLoaded("CustomerProfile");
                    _entityRoot.Set(TeaqlEntityKey(), "customer_profile", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateOrderNumber(string value)
                {
                    this.OrderNumber = value;
                    MarkLoaded("OrderNumber");
                    _entityRoot.Set(TeaqlEntityKey(), "order_number", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateFulfillmentStatus(string value)
                {
                    this.FulfillmentStatus = value;
                    MarkLoaded("FulfillmentStatus");
                    _entityRoot.Set(TeaqlEntityKey(), "fulfillment_status", TeaqlValue(value));
                    return this;
                }

                public CustomerOrder UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public CustomerOrder UpdateMerchant(Merchant value)
                {
                    UpdateMerchant(value?.Id);
                    this.MerchantEntity = value;
                    return this;
                }


                public CustomerOrder UpdateMerchantId(long? value)
                {
                    return UpdateMerchant(value);
                }


                public CustomerOrder UpdateCustomerProfile(CustomerProfile value)
                {
                    UpdateCustomerProfile(value?.Id);
                    this.CustomerProfileEntity = value;
                    return this;
                }


                public CustomerOrder UpdateCustomerProfileId(long? value)
                {
                    return UpdateCustomerProfile(value);
                }

    }
}