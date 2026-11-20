using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class FreightQuote : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("FreightQuote", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField("id", global::TeaQL.Runtime.EntityJsonKind.Id, "id"),
            new global::TeaQL.Runtime.EntityJsonField("platform", global::TeaQL.Runtime.EntityJsonKind.Reference, "platform", () => global::Generated.Models.Platform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("reference", global::TeaQL.Runtime.EntityJsonKind.Text, "reference"),
            new global::TeaQL.Runtime.EntityJsonField("amount", global::TeaQL.Runtime.EntityJsonKind.Decimal, "amount"),
            new global::TeaQL.Runtime.EntityJsonField("shipment_number", global::TeaQL.Runtime.EntityJsonKind.Long, "shipment_number"),
            new global::TeaQL.Runtime.EntityJsonField("package_count", global::TeaQL.Runtime.EntityJsonKind.Integer, "package_count"),
            new global::TeaQL.Runtime.EntityJsonField("delivery_date", global::TeaQL.Runtime.EntityJsonKind.Date, "delivery_date"),
            new global::TeaQL.Runtime.EntityJsonField("pickup_time", global::TeaQL.Runtime.EntityJsonKind.Time, "pickup_time"),
            new global::TeaQL.Runtime.EntityJsonField("quoted_at", global::TeaQL.Runtime.EntityJsonKind.DateTime, "quoted_at"),
            new global::TeaQL.Runtime.EntityJsonField("instructions", global::TeaQL.Runtime.EntityJsonKind.Text, "instructions"),
            new global::TeaQL.Runtime.EntityJsonField("version", global::TeaQL.Runtime.EntityJsonKind.Version, "version")}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static FreightQuote FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded("Id"))
            {
                yield return new KeyValuePair<string, object?>("id", Id);
            }
            if (IsLoaded("Platform"))
            {
                object? jsonPlatform = IsLoaded("PlatformEntity") && PlatformEntity != null && PlatformEntity.Id == Platform
                    ? PlatformEntity : Platform;
                yield return new KeyValuePair<string, object?>("platform", jsonPlatform);
            }
            if (IsLoaded("Reference"))
            {
                yield return new KeyValuePair<string, object?>("reference", Reference);
            }
            if (IsLoaded("Amount"))
            {
                yield return new KeyValuePair<string, object?>("amount", Amount);
            }
            if (IsLoaded("ShipmentNumber"))
            {
                yield return new KeyValuePair<string, object?>("shipment_number", ShipmentNumber);
            }
            if (IsLoaded("PackageCount"))
            {
                yield return new KeyValuePair<string, object?>("package_count", PackageCount);
            }
            if (IsLoaded("DeliveryDate"))
            {
                yield return new KeyValuePair<string, object?>("delivery_date", DeliveryDate);
            }
            if (IsLoaded("PickupTime"))
            {
                yield return new KeyValuePair<string, object?>("pickup_time", PickupTime);
            }
            if (IsLoaded("QuotedAt"))
            {
                yield return new KeyValuePair<string, object?>("quoted_at", QuotedAt);
            }
            if (IsLoaded("Instructions"))
            {
                yield return new KeyValuePair<string, object?>("instructions", Instructions);
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
        private EntityKey TeaqlEntityKey() => new EntityKey("FreightQuote", Id ?? _ledgerId);
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
        private FreightQuote() { _creationState = EntityCreationState.Hydrated; }
        internal static FreightQuote TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new FreightQuote();
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
                public long? Platform { get; set; }
                public string? Reference { get; set; }
                public decimal? Amount { get; set; }
                public long? ShipmentNumber { get; set; }
                public long? PackageCount { get; set; }
                public DateTime? DeliveryDate { get; set; }
                public TimeSpan? PickupTime { get; set; }
                public DateTime? QuotedAt { get; set; }
                public string? Instructions { get; set; }
                public long? Version { get; set; }
                public Platform? PlatformEntity { get; set; }

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
        internal static FreightQuote TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public FreightQuote MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public FreightQuote MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public FreightQuote AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public FreightQuote MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static FreightQuote Refer(long id)
        {
            var entity = new FreightQuote();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static FreightQuote FromRecord(Record record)
        {
            var entity = new FreightQuote().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("Platform", out var platformValue)
                        || record.TryGetValue("platform", out platformValue))
                    {
                        entity.MarkLoaded("Platform");
                        if (platformValue.Raw is Record platformRow)
                        {
                            entity.PlatformEntity = global::Generated.Models.Platform.FromRecord(platformRow);
                            entity.Platform = entity.PlatformEntity.Id;
                            entity.MarkLoaded("PlatformEntity");
                        }
                        else if (platformValue.Raw is IEnumerable<Record> platformRows)
                        {
                            foreach (var row in platformRows)
                            {
                                entity.PlatformEntity = global::Generated.Models.Platform.FromRecord(row);
                                entity.Platform = entity.PlatformEntity.Id;
                                entity.MarkLoaded("PlatformEntity");
                                break;
                            }
                        }
                        else if (platformValue.Raw == null)
                            entity.MarkLoaded("PlatformEntity");
                        else if (platformValue.Raw != null)
                            entity.Platform = Convert.ToInt64(platformValue.Raw);
                    }
                    if (record.TryGetValue("reference", out var referenceValue))
                    {
                        entity.MarkLoaded("Reference");
                        if (referenceValue.Raw != null)
                            entity.Reference = Convert.ToString(referenceValue.Raw);
                    }
                    if (record.TryGetValue("amount", out var amountValue))
                    {
                        entity.MarkLoaded("Amount");
                        if (amountValue.Raw != null)
                            entity.Amount = Convert.ToDecimal(amountValue.Raw);
                    }
                    if (record.TryGetValue("shipment_number", out var shipmentNumberValue))
                    {
                        entity.MarkLoaded("ShipmentNumber");
                        if (shipmentNumberValue.Raw != null)
                            entity.ShipmentNumber = Convert.ToInt64(shipmentNumberValue.Raw);
                    }
                    if (record.TryGetValue("package_count", out var packageCountValue))
                    {
                        entity.MarkLoaded("PackageCount");
                        if (packageCountValue.Raw != null)
                            entity.PackageCount = Convert.ToInt64(packageCountValue.Raw);
                    }
                    if (record.TryGetValue("delivery_date", out var deliveryDateValue))
                    {
                        entity.MarkLoaded("DeliveryDate");
                        if (deliveryDateValue.Raw != null)
                            entity.DeliveryDate = TeaqlDateTime(deliveryDateValue);
                    }
                    if (record.TryGetValue("pickup_time", out var pickupTimeValue))
                    {
                        entity.MarkLoaded("PickupTime");
                        if (pickupTimeValue.Raw != null)
                            entity.PickupTime = pickupTimeValue.Raw is TimeSpan time ? time : TimeSpan.Parse(Convert.ToString(pickupTimeValue.Raw) ?? throw new InvalidOperationException("Time value requires a non-null string"), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (record.TryGetValue("quoted_at", out var quotedAtValue))
                    {
                        entity.MarkLoaded("QuotedAt");
                        if (quotedAtValue.Raw != null)
                            entity.QuotedAt = TeaqlDateTime(quotedAtValue);
                    }
                    if (record.TryGetValue("instructions", out var instructionsValue))
                    {
                        entity.MarkLoaded("Instructions");
                        if (instructionsValue.Raw != null)
                            entity.Instructions = Convert.ToString(instructionsValue.Raw);
                    }
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                "id", "platform", "reference", "amount", "shipment_number", "package_count", "delivery_date", "pickup_time", "quoted_at", "instructions", "version",
                "Id", "Platform", "Reference", "Amount", "ShipmentNumber", "PackageCount", "DeliveryDate", "PickupTime", "QuotedAt", "Instructions", "Version",
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static FreightQuote FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<FreightQuote> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("FreightQuote");
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
            graph.DiscardRestoredFields("FreightQuote", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Platform"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("platform"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Reference"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("reference"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Amount"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("amount"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("ShipmentNumber"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("shipment_number"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("PackageCount"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("package_count"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("DeliveryDate"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("delivery_date"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("PickupTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("pickup_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("QuotedAt"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("quoted_at"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Instructions"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("instructions"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Version"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property("version"), Message = "Mutation requires a fully loaded entity" } });
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
            var teaqlOriginalPlatform = this.Platform;
            var teaqlOriginalReference = this.Reference;
            var teaqlOriginalAmount = this.Amount;
            var teaqlOriginalShipmentNumber = this.ShipmentNumber;
            var teaqlOriginalPackageCount = this.PackageCount;
            var teaqlOriginalDeliveryDate = this.DeliveryDate;
            var teaqlOriginalPickupTime = this.PickupTime;
            var teaqlOriginalQuotedAt = this.QuotedAt;
            var teaqlOriginalInstructions = this.Instructions;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Platform = teaqlOriginalPlatform;
                this.Reference = teaqlOriginalReference;
                this.Amount = teaqlOriginalAmount;
                this.ShipmentNumber = teaqlOriginalShipmentNumber;
                this.PackageCount = teaqlOriginalPackageCount;
                this.DeliveryDate = teaqlOriginalDeliveryDate;
                this.PickupTime = teaqlOriginalPickupTime;
                this.QuotedAt = teaqlOriginalQuotedAt;
                this.Instructions = teaqlOriginalInstructions;
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

        internal async global::System.Threading.Tasks.Task<FreightQuote> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("FreightQuote",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("FreightQuote",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for FreightQuote");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Platform = saved.Platform;
            this.Reference = saved.Reference;
            this.Amount = saved.Amount;
            this.ShipmentNumber = saved.ShipmentNumber;
            this.PackageCount = saved.PackageCount;
            this.DeliveryDate = saved.DeliveryDate;
            this.PickupTime = saved.PickupTime;
            this.QuotedAt = saved.QuotedAt;
            this.Instructions = saved.Instructions;
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
                        if (IsLoaded("Platform")) record["platform"] = TeaqlValue(Platform);
                        if (IsLoaded("Reference")) record["reference"] = TeaqlValue(Reference);
                        if (IsLoaded("Amount")) record["amount"] = TeaqlValue(Amount);
                        if (IsLoaded("ShipmentNumber")) record["shipment_number"] = TeaqlValue(ShipmentNumber);
                        if (IsLoaded("PackageCount")) record["package_count"] = TeaqlValue(PackageCount);
                        if (IsLoaded("DeliveryDate")) record["delivery_date"] = TeaqlValue(DeliveryDate);
                        if (IsLoaded("PickupTime")) record["pickup_time"] = TeaqlValue(PickupTime);
                        if (IsLoaded("QuotedAt")) record["quoted_at"] = TeaqlValue(QuotedAt);
                        if (IsLoaded("Instructions")) record["instructions"] = TeaqlValue(Instructions);
                        if (IsLoaded("Version")) record["version"] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue("platform", out var platformValue))
                        this.Platform = platformValue.Raw == null ? null : Convert.ToInt64(platformValue.Raw);

                    if (record.TryGetValue("reference", out var referenceValue))
                        this.Reference = referenceValue.Raw == null ? null : Convert.ToString(referenceValue.Raw);

                    if (record.TryGetValue("amount", out var amountValue))
                        this.Amount = amountValue.Raw == null ? null : Convert.ToDecimal(amountValue.Raw);

                    if (record.TryGetValue("shipment_number", out var shipmentNumberValue))
                        this.ShipmentNumber = shipmentNumberValue.Raw == null ? null : Convert.ToInt64(shipmentNumberValue.Raw);

                    if (record.TryGetValue("package_count", out var packageCountValue))
                        this.PackageCount = packageCountValue.Raw == null ? null : Convert.ToInt64(packageCountValue.Raw);

                    if (record.TryGetValue("delivery_date", out var deliveryDateValue))
                        this.DeliveryDate = deliveryDateValue.Raw == null ? null : TeaqlDateTime(deliveryDateValue);

                    if (record.TryGetValue("pickup_time", out var pickupTimeValue))
                        this.PickupTime = pickupTimeValue.Raw == null ? null : pickupTimeValue.Raw is TimeSpan time ? time : TimeSpan.Parse(Convert.ToString(pickupTimeValue.Raw) ?? throw new InvalidOperationException("Time value requires a non-null string"), System.Globalization.CultureInfo.InvariantCulture);

                    if (record.TryGetValue("quoted_at", out var quotedAtValue))
                        this.QuotedAt = quotedAtValue.Raw == null ? null : TeaqlDateTime(quotedAtValue);

                    if (record.TryGetValue("instructions", out var instructionsValue))
                        this.Instructions = instructionsValue.Raw == null ? null : Convert.ToString(instructionsValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (Reference != null) record["reference"] = new Value.TextValue(Reference);

                    if (Amount.HasValue) record["amount"] = new Value.DecimalValue(Amount.Value);

                    if (ShipmentNumber.HasValue) record["shipment_number"] = new Value.I64Value(ShipmentNumber.Value);

                    if (PackageCount.HasValue) record["package_count"] = new Value.I64Value(PackageCount.Value);

                    if (DeliveryDate.HasValue) record["delivery_date"] = new Value.DateValue(DeliveryDate.Value);

                    if (PickupTime.HasValue) record["pickup_time"] = new Value.TimeValue(PickupTime.Value);

                    if (QuotedAt.HasValue) record["quoted_at"] = new Value.TimestampValue(new DateTimeOffset(QuotedAt.Value).ToUnixTimeMilliseconds());

                    if (Instructions != null) record["instructions"] = new Value.TextValue(Instructions);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "FreightQuote", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (Reference != null) record["reference"] = new Value.TextValue(Reference);

                    if (Amount.HasValue) record["amount"] = new Value.DecimalValue(Amount.Value);

                    if (ShipmentNumber.HasValue) record["shipment_number"] = new Value.I64Value(ShipmentNumber.Value);

                    if (PackageCount.HasValue) record["package_count"] = new Value.I64Value(PackageCount.Value);

                    if (DeliveryDate.HasValue) record["delivery_date"] = new Value.DateValue(DeliveryDate.Value);

                    if (PickupTime.HasValue) record["pickup_time"] = new Value.TimeValue(PickupTime.Value);

                    if (QuotedAt.HasValue) record["quoted_at"] = new Value.TimestampValue(new DateTimeOffset(QuotedAt.Value).ToUnixTimeMilliseconds());

                    if (Instructions != null) record["instructions"] = new Value.TextValue(Instructions);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "FreightQuote", 
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
                Entity = "FreightQuote",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("FreightQuote");
        }

                public FreightQuote UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdatePlatform(long? value)
                {
                    if (this.Platform != value) this.PlatformEntity = null;
                    this.Platform = value;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateReference(string value)
                {
                    this.Reference = value;
                    MarkLoaded("Reference");
                    _entityRoot.Set(TeaqlEntityKey(), "reference", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateAmount(decimal? value)
                {
                    this.Amount = value;
                    MarkLoaded("Amount");
                    _entityRoot.Set(TeaqlEntityKey(), "amount", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateShipmentNumber(long? value)
                {
                    this.ShipmentNumber = value;
                    MarkLoaded("ShipmentNumber");
                    _entityRoot.Set(TeaqlEntityKey(), "shipment_number", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdatePackageCount(long? value)
                {
                    this.PackageCount = value;
                    MarkLoaded("PackageCount");
                    _entityRoot.Set(TeaqlEntityKey(), "package_count", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateDeliveryDate(DateTime? value)
                {
                    this.DeliveryDate = value;
                    MarkLoaded("DeliveryDate");
                    _entityRoot.Set(TeaqlEntityKey(), "delivery_date", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdatePickupTime(TimeSpan? value)
                {
                    this.PickupTime = value;
                    MarkLoaded("PickupTime");
                    _entityRoot.Set(TeaqlEntityKey(), "pickup_time", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateQuotedAt(DateTime? value)
                {
                    this.QuotedAt = value;
                    MarkLoaded("QuotedAt");
                    _entityRoot.Set(TeaqlEntityKey(), "quoted_at", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateInstructions(string value)
                {
                    this.Instructions = value;
                    MarkLoaded("Instructions");
                    _entityRoot.Set(TeaqlEntityKey(), "instructions", TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public FreightQuote UpdatePlatform(Platform value)
                {
                    UpdatePlatform(value?.Id);
                    this.PlatformEntity = value;
                    return this;
                }


                public FreightQuote UpdatePlatformId(long? value)
                {
                    return UpdatePlatform(value);
                }

    }
}