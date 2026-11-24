using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class FreightQuote : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static class __TeaqlFields
        {
            internal const string ID_PROPERTY_MEMBER = "Id";
            internal const string ID_PROPERTY_QUERY = "id";
            internal const string ID_PROPERTY_STORAGE = "id";
            internal const string ID_PROPERTY_MODEL = "id";
            internal const string PLATFORM_PROPERTY_MEMBER = "Platform";
            internal const string PLATFORM_PROPERTY_QUERY = "platform";
            internal const string PLATFORM_PROPERTY_STORAGE = "platform";
            internal const string PLATFORM_PROPERTY_MODEL = "platform";
            internal const string REFERENCE_PROPERTY_MEMBER = "Reference";
            internal const string REFERENCE_PROPERTY_QUERY = "reference";
            internal const string REFERENCE_PROPERTY_STORAGE = "reference";
            internal const string REFERENCE_PROPERTY_MODEL = "reference";
            internal const string AMOUNT_PROPERTY_MEMBER = "Amount";
            internal const string AMOUNT_PROPERTY_QUERY = "amount";
            internal const string AMOUNT_PROPERTY_STORAGE = "amount";
            internal const string AMOUNT_PROPERTY_MODEL = "amount";
            internal const string SHIPMENT_NUMBER_PROPERTY_MEMBER = "ShipmentNumber";
            internal const string SHIPMENT_NUMBER_PROPERTY_QUERY = "shipment_number";
            internal const string SHIPMENT_NUMBER_PROPERTY_STORAGE = "shipment_number";
            internal const string SHIPMENT_NUMBER_PROPERTY_MODEL = "shipment_number";
            internal const string PACKAGE_COUNT_PROPERTY_MEMBER = "PackageCount";
            internal const string PACKAGE_COUNT_PROPERTY_QUERY = "package_count";
            internal const string PACKAGE_COUNT_PROPERTY_STORAGE = "package_count";
            internal const string PACKAGE_COUNT_PROPERTY_MODEL = "package_count";
            internal const string DELIVERY_DATE_PROPERTY_MEMBER = "DeliveryDate";
            internal const string DELIVERY_DATE_PROPERTY_QUERY = "delivery_date";
            internal const string DELIVERY_DATE_PROPERTY_STORAGE = "delivery_date";
            internal const string DELIVERY_DATE_PROPERTY_MODEL = "delivery_date";
            internal const string PICKUP_TIME_PROPERTY_MEMBER = "PickupTime";
            internal const string PICKUP_TIME_PROPERTY_QUERY = "pickup_time";
            internal const string PICKUP_TIME_PROPERTY_STORAGE = "pickup_time";
            internal const string PICKUP_TIME_PROPERTY_MODEL = "pickup_time";
            internal const string QUOTED_AT_PROPERTY_MEMBER = "QuotedAt";
            internal const string QUOTED_AT_PROPERTY_QUERY = "quoted_at";
            internal const string QUOTED_AT_PROPERTY_STORAGE = "quoted_at";
            internal const string QUOTED_AT_PROPERTY_MODEL = "quoted_at";
            internal const string INSTRUCTIONS_PROPERTY_MEMBER = "Instructions";
            internal const string INSTRUCTIONS_PROPERTY_QUERY = "instructions";
            internal const string INSTRUCTIONS_PROPERTY_STORAGE = "instructions";
            internal const string INSTRUCTIONS_PROPERTY_MODEL = "instructions";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("FreightQuote", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, FreightQuote.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, () => global::Generated.Models.Platform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Decimal, FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Long, FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Integer, FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Date, FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Time, FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(FreightQuote.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, FreightQuote.__TeaqlFields.VERSION_PROPERTY_STORAGE)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static FreightQuote FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(FreightQuote.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_MEMBER))
            {
                object? jsonPlatform = IsLoaded("PlatformEntity") && PlatformEntity != null && PlatformEntity.Id == Platform
                    ? PlatformEntity : Platform;
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, jsonPlatform);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_STORAGE, Reference);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_STORAGE, Amount);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_STORAGE, ShipmentNumber);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_STORAGE, PackageCount);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_STORAGE, DeliveryDate);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_STORAGE, PickupTime);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_STORAGE, QuotedAt);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_STORAGE, Instructions);
            }
            if (IsLoaded(FreightQuote.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(FreightQuote.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
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
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, out var platformValue)
                        || record.TryGetValue(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, out platformValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_MEMBER);
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
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_STORAGE, out var referenceValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_MEMBER);
                        if (referenceValue.Raw != null)
                            entity.Reference = Convert.ToString(referenceValue.Raw);
                    }
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_STORAGE, out var amountValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_MEMBER);
                        if (amountValue.Raw != null)
                            entity.Amount = Convert.ToDecimal(amountValue.Raw);
                    }
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_STORAGE, out var shipmentNumberValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_MEMBER);
                        if (shipmentNumberValue.Raw != null)
                            entity.ShipmentNumber = Convert.ToInt64(shipmentNumberValue.Raw);
                    }
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_STORAGE, out var packageCountValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_MEMBER);
                        if (packageCountValue.Raw != null)
                            entity.PackageCount = Convert.ToInt64(packageCountValue.Raw);
                    }
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_STORAGE, out var deliveryDateValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_MEMBER);
                        if (deliveryDateValue.Raw != null)
                            entity.DeliveryDate = TeaqlDateTime(deliveryDateValue);
                    }
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_STORAGE, out var pickupTimeValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_MEMBER);
                        if (pickupTimeValue.Raw != null)
                            entity.PickupTime = pickupTimeValue.Raw is TimeSpan time ? time : TimeSpan.Parse(Convert.ToString(pickupTimeValue.Raw) ?? throw new InvalidOperationException("Time value requires a non-null string"), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_STORAGE, out var quotedAtValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_MEMBER);
                        if (quotedAtValue.Raw != null)
                            entity.QuotedAt = TeaqlDateTime(quotedAtValue);
                    }
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_STORAGE, out var instructionsValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_MEMBER);
                        if (instructionsValue.Raw != null)
                            entity.Instructions = Convert.ToString(instructionsValue.Raw);
                    }
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(FreightQuote.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                FreightQuote.__TeaqlFields.ID_PROPERTY_QUERY, FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_QUERY, FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_QUERY, FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_QUERY, FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_QUERY, FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_QUERY, FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_QUERY, FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_QUERY, FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_QUERY, FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_QUERY, FreightQuote.__TeaqlFields.VERSION_PROPERTY_QUERY,
                FreightQuote.__TeaqlFields.ID_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_MEMBER, FreightQuote.__TeaqlFields.VERSION_PROPERTY_MEMBER,
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
                if (!IsLoaded(FreightQuote.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(FreightQuote.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "FreightQuote", Location = ObjectLocation.Property(FreightQuote.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
                        if (IsLoaded(FreightQuote.__TeaqlFields.ID_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = TeaqlValue(Platform);
                        if (IsLoaded(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_STORAGE] = TeaqlValue(Reference);
                        if (IsLoaded(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_STORAGE] = TeaqlValue(Amount);
                        if (IsLoaded(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_STORAGE] = TeaqlValue(ShipmentNumber);
                        if (IsLoaded(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_STORAGE] = TeaqlValue(PackageCount);
                        if (IsLoaded(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_STORAGE] = TeaqlValue(DeliveryDate);
                        if (IsLoaded(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_STORAGE] = TeaqlValue(PickupTime);
                        if (IsLoaded(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_STORAGE] = TeaqlValue(QuotedAt);
                        if (IsLoaded(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_STORAGE] = TeaqlValue(Instructions);
                        if (IsLoaded(FreightQuote.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[FreightQuote.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, out var platformValue))
                        this.Platform = platformValue.Raw == null ? null : Convert.ToInt64(platformValue.Raw);

                    if (record.TryGetValue(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_STORAGE, out var referenceValue))
                        this.Reference = referenceValue.Raw == null ? null : Convert.ToString(referenceValue.Raw);

                    if (record.TryGetValue(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_STORAGE, out var amountValue))
                        this.Amount = amountValue.Raw == null ? null : Convert.ToDecimal(amountValue.Raw);

                    if (record.TryGetValue(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_STORAGE, out var shipmentNumberValue))
                        this.ShipmentNumber = shipmentNumberValue.Raw == null ? null : Convert.ToInt64(shipmentNumberValue.Raw);

                    if (record.TryGetValue(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_STORAGE, out var packageCountValue))
                        this.PackageCount = packageCountValue.Raw == null ? null : Convert.ToInt64(packageCountValue.Raw);

                    if (record.TryGetValue(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_STORAGE, out var deliveryDateValue))
                        this.DeliveryDate = deliveryDateValue.Raw == null ? null : TeaqlDateTime(deliveryDateValue);

                    if (record.TryGetValue(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_STORAGE, out var pickupTimeValue))
                        this.PickupTime = pickupTimeValue.Raw == null ? null : pickupTimeValue.Raw is TimeSpan time ? time : TimeSpan.Parse(Convert.ToString(pickupTimeValue.Raw) ?? throw new InvalidOperationException("Time value requires a non-null string"), System.Globalization.CultureInfo.InvariantCulture);

                    if (record.TryGetValue(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_STORAGE, out var quotedAtValue))
                        this.QuotedAt = quotedAtValue.Raw == null ? null : TeaqlDateTime(quotedAtValue);

                    if (record.TryGetValue(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_STORAGE, out var instructionsValue))
                        this.Instructions = instructionsValue.Raw == null ? null : Convert.ToString(instructionsValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[FreightQuote.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (Platform.HasValue) record[FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(Platform.Value);

                    if (Reference != null) record[FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_STORAGE] = new Value.TextValue(Reference);

                    if (Amount.HasValue) record[FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_STORAGE] = new Value.DecimalValue(Amount.Value);

                    if (ShipmentNumber.HasValue) record[FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_STORAGE] = new Value.I64Value(ShipmentNumber.Value);

                    if (PackageCount.HasValue) record[FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_STORAGE] = new Value.I64Value(PackageCount.Value);

                    if (DeliveryDate.HasValue) record[FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_STORAGE] = new Value.DateValue(DeliveryDate.Value);

                    if (PickupTime.HasValue) record[FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_STORAGE] = new Value.TimeValue(PickupTime.Value);

                    if (QuotedAt.HasValue) record[FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(QuotedAt.Value).ToUnixTimeMilliseconds());

                    if (Instructions != null) record[FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_STORAGE] = new Value.TextValue(Instructions);

                    if (Version.HasValue) record[FreightQuote.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "FreightQuote", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Platform.HasValue) record[FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(Platform.Value);

                    if (Reference != null) record[FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_STORAGE] = new Value.TextValue(Reference);

                    if (Amount.HasValue) record[FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_STORAGE] = new Value.DecimalValue(Amount.Value);

                    if (ShipmentNumber.HasValue) record[FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_STORAGE] = new Value.I64Value(ShipmentNumber.Value);

                    if (PackageCount.HasValue) record[FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_STORAGE] = new Value.I64Value(PackageCount.Value);

                    if (DeliveryDate.HasValue) record[FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_STORAGE] = new Value.DateValue(DeliveryDate.Value);

                    if (PickupTime.HasValue) record[FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_STORAGE] = new Value.TimeValue(PickupTime.Value);

                    if (QuotedAt.HasValue) record[FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(QuotedAt.Value).ToUnixTimeMilliseconds());

                    if (Instructions != null) record[FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_STORAGE] = new Value.TextValue(Instructions);

                    if (Version.HasValue) record[FreightQuote.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

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
                    MarkLoaded(FreightQuote.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdatePlatform(long? value)
                {
                    if (this.Platform != value) this.PlatformEntity = null;
                    this.Platform = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateReference(string value)
                {
                    this.Reference = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.REFERENCE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateAmount(decimal? value)
                {
                    this.Amount = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.AMOUNT_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateShipmentNumber(long? value)
                {
                    this.ShipmentNumber = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.SHIPMENT_NUMBER_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdatePackageCount(long? value)
                {
                    this.PackageCount = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.PACKAGE_COUNT_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateDeliveryDate(DateTime? value)
                {
                    this.DeliveryDate = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.DELIVERY_DATE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdatePickupTime(TimeSpan? value)
                {
                    this.PickupTime = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.PICKUP_TIME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateQuotedAt(DateTime? value)
                {
                    this.QuotedAt = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.QUOTED_AT_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateInstructions(string value)
                {
                    this.Instructions = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.INSTRUCTIONS_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public FreightQuote UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(FreightQuote.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), FreightQuote.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
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