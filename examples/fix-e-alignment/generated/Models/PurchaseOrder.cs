using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class PurchaseOrder : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("PurchaseOrder", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField("id", global::TeaQL.Runtime.EntityJsonKind.Id, "id"),
            new global::TeaQL.Runtime.EntityJsonField("tenant", global::TeaQL.Runtime.EntityJsonKind.Reference, "tenant", () => global::Generated.Models.Tenant._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("contact_phone", global::TeaQL.Runtime.EntityJsonKind.Text, "contact_phone"),
            new global::TeaQL.Runtime.EntityJsonField("internal_note", global::TeaQL.Runtime.EntityJsonKind.Text, "internal_note"),
            new global::TeaQL.Runtime.EntityJsonField("create_time", global::TeaQL.Runtime.EntityJsonKind.DateTime, "create_time"),
            new global::TeaQL.Runtime.EntityJsonField("update_time", global::TeaQL.Runtime.EntityJsonKind.DateTime, "update_time"),
            new global::TeaQL.Runtime.EntityJsonField("version", global::TeaQL.Runtime.EntityJsonKind.Version, "version"),
            new global::TeaQL.Runtime.EntityJsonField("order_item_list", global::TeaQL.Runtime.EntityJsonKind.Many, "OrderItemList", () => global::Generated.Models.OrderItem._teaqlJsonSchema)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static PurchaseOrder FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded("Id"))
            {
                yield return new KeyValuePair<string, object?>("id", Id);
            }
            if (IsLoaded("Tenant"))
            {
                object? jsonTenant = IsLoaded("TenantEntity") && TenantEntity != null && TenantEntity.Id == Tenant
                    ? TenantEntity : Tenant;
                yield return new KeyValuePair<string, object?>("tenant", jsonTenant);
            }
            if (IsLoaded("ContactPhone"))
            {
                yield return new KeyValuePair<string, object?>("contact_phone", ContactPhone);
            }
            if (IsLoaded("InternalNote"))
            {
                yield return new KeyValuePair<string, object?>("internal_note", InternalNote);
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
            if (IsLoaded("OrderItemList"))
                yield return new KeyValuePair<string, object?>("OrderItemList", OrderItemList);
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("PurchaseOrder", Id ?? _ledgerId);
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
        public PurchaseOrder() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Id { get; set; }
                public long? Tenant { get; set; }
                public string? ContactPhone { get; set; }
                public string? InternalNote { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public Tenant? TenantEntity { get; set; }
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

        public PurchaseOrder MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public PurchaseOrder MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public PurchaseOrder AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public PurchaseOrder MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static PurchaseOrder Refer(long id)
        {
            var entity = new PurchaseOrder();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static PurchaseOrder FromRecord(Record record)
        {
            var entity = new PurchaseOrder().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
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
                    if (record.TryGetValue("contact_phone", out var contactPhoneValue))
                    {
                        entity.MarkLoaded("ContactPhone");
                        if (contactPhoneValue.Raw != null)
                            entity.ContactPhone = Convert.ToString(contactPhoneValue.Raw);
                    }
                    if (record.TryGetValue("internal_note", out var internalNoteValue))
                    {
                        entity.MarkLoaded("InternalNote");
                        if (internalNoteValue.Raw != null)
                            entity.InternalNote = Convert.ToString(internalNoteValue.Raw);
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
                "id", "tenant", "contact_phone", "internal_note", "create_time", "update_time", "version",
                "Id", "Tenant", "ContactPhone", "InternalNote", "CreateTime", "UpdateTime", "Version",
                "OrderItemList"
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static PurchaseOrder FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<PurchaseOrder> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("PurchaseOrder");
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
            graph.DiscardRestoredFields("PurchaseOrder", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = !Id.HasValue || _teaqlForceCreate;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Tenant"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("tenant"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("ContactPhone"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("contact_phone"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("InternalNote"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("internal_note"), Message = "Mutation requires a fully loaded entity" } });
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
                var originalVersion = _entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version;
                if (originalVersion.HasValue) ((UpdateCommand)command).Values["version"] = new Value.I64Value(originalVersion.Value);
            }
            graph.PrepareInput(this, TeaqlMutationRequest(command, graph.Intent.Comment));
            }
            for (var index = 0; index < OrderItemList.Count; index++)
            {
                var child = OrderItemList[index];
                child.AttachRoot(_entityRoot);
                if (child.PurchaseOrder != (Id ?? _ledgerId)) child.UpdatePurchaseOrderId(Id ?? _ledgerId);
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
            var teaqlOriginalTenant = this.Tenant;
            var teaqlOriginalContactPhone = this.ContactPhone;
            var teaqlOriginalInternalNote = this.InternalNote;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalUpdateTime = this.UpdateTime;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Tenant = teaqlOriginalTenant;
                this.ContactPhone = teaqlOriginalContactPhone;
                this.InternalNote = teaqlOriginalInternalNote;
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
            foreach (var child in OrderItemList) await child.TeaqlReserveGraphIdentitiesAsync(graph);
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<PurchaseOrder> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            var creating = !this.Id.HasValue || _teaqlForceCreate;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("PurchaseOrder",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("PurchaseOrder",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for PurchaseOrder");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Tenant = saved.Tenant;
            this.ContactPhone = saved.ContactPhone;
            this.InternalNote = saved.InternalNote;
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
            for (var index = 0; index < OrderItemList.Count; index++)
            {
                var child = OrderItemList[index];
                child.AttachRoot(_entityRoot);
                if (child.PurchaseOrder != Id) child.UpdatePurchaseOrderId(Id);
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
                        if (IsLoaded("Tenant")) record["tenant"] = TeaqlValue(Tenant);
                        if (IsLoaded("ContactPhone")) record["contact_phone"] = TeaqlValue(ContactPhone);
                        if (IsLoaded("InternalNote")) record["internal_note"] = TeaqlValue(InternalNote);
                        if (IsLoaded("CreateTime")) record["create_time"] = TeaqlValue(CreateTime);
                        if (IsLoaded("UpdateTime")) record["update_time"] = TeaqlValue(UpdateTime);
                        if (IsLoaded("Version")) record["version"] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record);
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Tenant.HasValue) record["tenant"] = new Value.I64Value(Tenant.Value);

                    if (ContactPhone != null) record["contact_phone"] = new Value.TextValue(ContactPhone);

                    if (InternalNote != null) record["internal_note"] = new Value.TextValue(InternalNote);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "PurchaseOrder", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Tenant.HasValue) record["tenant"] = new Value.I64Value(Tenant.Value);

                    if (ContactPhone != null) record["contact_phone"] = new Value.TextValue(ContactPhone);

                    if (InternalNote != null) record["internal_note"] = new Value.TextValue(InternalNote);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "PurchaseOrder", 
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
                Entity = "PurchaseOrder",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("PurchaseOrder");
        }

                public PurchaseOrder UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                internal PurchaseOrder UpdateTenant(long? value)
                {
                    if (this.Tenant != value) this.TenantEntity = null;
                    this.Tenant = value;
                    MarkLoaded("Tenant");
                    _entityRoot.Set(TeaqlEntityKey(), "tenant", TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateContactPhone(string value)
                {
                    this.ContactPhone = value;
                    MarkLoaded("ContactPhone");
                    _entityRoot.Set(TeaqlEntityKey(), "contact_phone", TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateInternalNote(string value)
                {
                    this.InternalNote = value;
                    MarkLoaded("InternalNote");
                    _entityRoot.Set(TeaqlEntityKey(), "internal_note", TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "create_time", TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "update_time", TeaqlValue(value));
                    return this;
                }

                public PurchaseOrder UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                internal PurchaseOrder UpdateTenant(Tenant value)
                {
                    UpdateTenant(value?.Id);
                    this.TenantEntity = value;
                    return this;
                }


                internal PurchaseOrder UpdateTenantId(long? value)
                {
                    return UpdateTenant(value);
                }

    }
}