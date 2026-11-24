using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class OrderStatus : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static class __TeaqlFields
        {
            internal const string ID_PROPERTY_MEMBER = "Id";
            internal const string ID_PROPERTY_QUERY = "id";
            internal const string ID_PROPERTY_STORAGE = "id";
            internal const string ID_PROPERTY_MODEL = "id";
            internal const string NAME_PROPERTY_MEMBER = "Name";
            internal const string NAME_PROPERTY_QUERY = "name";
            internal const string NAME_PROPERTY_STORAGE = "name";
            internal const string NAME_PROPERTY_MODEL = "name";
            internal const string CODE_PROPERTY_MEMBER = "Code";
            internal const string CODE_PROPERTY_QUERY = "code";
            internal const string CODE_PROPERTY_STORAGE = "code";
            internal const string CODE_PROPERTY_MODEL = "code";
            internal const string COLOR_PROPERTY_MEMBER = "Color";
            internal const string COLOR_PROPERTY_QUERY = "color";
            internal const string COLOR_PROPERTY_STORAGE = "color";
            internal const string COLOR_PROPERTY_MODEL = "color";
            internal const string DISPLAY_ORDER_PROPERTY_MEMBER = "DisplayOrder";
            internal const string DISPLAY_ORDER_PROPERTY_QUERY = "display_order";
            internal const string DISPLAY_ORDER_PROPERTY_STORAGE = "display_order";
            internal const string DISPLAY_ORDER_PROPERTY_MODEL = "display_order";
            internal const string COMMERCE_PLATFORM_PROPERTY_MEMBER = "CommercePlatform";
            internal const string COMMERCE_PLATFORM_PROPERTY_QUERY = "commerce_platform";
            internal const string COMMERCE_PLATFORM_PROPERTY_STORAGE = "commerce_platform";
            internal const string COMMERCE_PLATFORM_PROPERTY_MODEL = "commerce_platform";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("OrderStatus", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(OrderStatus.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, OrderStatus.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderStatus.__TeaqlFields.NAME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, OrderStatus.__TeaqlFields.NAME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderStatus.__TeaqlFields.CODE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, OrderStatus.__TeaqlFields.CODE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderStatus.__TeaqlFields.COLOR_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, OrderStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Decimal, OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, () => global::Generated.Models.CommercePlatform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(OrderStatus.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, OrderStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField("customer_order_list", global::TeaQL.Runtime.EntityJsonKind.Many, "CustomerOrderList", () => global::Generated.Models.CustomerOrder._teaqlJsonSchema)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static OrderStatus FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(OrderStatus.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderStatus.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(OrderStatus.__TeaqlFields.NAME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderStatus.__TeaqlFields.NAME_PROPERTY_STORAGE, Name);
            }
            if (IsLoaded(OrderStatus.__TeaqlFields.CODE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderStatus.__TeaqlFields.CODE_PROPERTY_STORAGE, Code);
            }
            if (IsLoaded(OrderStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE, Color);
            }
            if (IsLoaded(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE, DisplayOrder);
            }
            if (IsLoaded(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER))
            {
                object? jsonCommercePlatform = IsLoaded("CommercePlatformEntity") && CommercePlatformEntity != null && CommercePlatformEntity.Id == CommercePlatform
                    ? CommercePlatformEntity : CommercePlatform;
                yield return new KeyValuePair<string, object?>(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, jsonCommercePlatform);
            }
            if (IsLoaded(OrderStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(OrderStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
            if (CustomerOrderList.IsLoaded || CustomerOrderList.Any(child => child.TeaqlHasPending()))
                yield return new KeyValuePair<string, object?>("CustomerOrderList", CustomerOrderList);
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("OrderStatus", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal bool TeaqlCreationIntent => _teaqlForceCreate || _entityRoot.IsNew(TeaqlEntityKey());
        internal bool TeaqlHasPendingGraph(HashSet<object> visited)
        {
            if (!visited.Add(this)) return false;
            if (TeaqlCreationIntent || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey())) return true;
            foreach (var child in CustomerOrderList)
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
            foreach (var child in CustomerOrderList)
                if (child.TeaqlHasPendingGraph(visited))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_deletion_state", EntityType = "OrderStatus", Location = ObjectLocation.Property("id"), Message = "Cancelled new entity has pending descendants; explicit resolution is required" } });
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
            foreach (var child in CustomerOrderList) child.AttachRoot(root, hydration);
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
        private OrderStatus() { _creationState = EntityCreationState.Hydrated; }
        internal static OrderStatus TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new OrderStatus();
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
                public string? Name { get; set; }
                public string? Code { get; set; }
                public string? Color { get; set; }
                public decimal? DisplayOrder { get; set; }
                public long? CommercePlatform { get; set; }
                public long? Version { get; set; }
                public CommercePlatform? CommercePlatformEntity { get; set; }
                public SmartList<CustomerOrder> CustomerOrderList { get; } = SmartList<CustomerOrder>.Empty();

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
        internal static OrderStatus TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public OrderStatus MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public OrderStatus MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public OrderStatus AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public OrderStatus MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static OrderStatus Refer(long id)
        {
            var entity = new OrderStatus();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static OrderStatus FromRecord(Record record)
        {
            var entity = new OrderStatus().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue(OrderStatus.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(OrderStatus.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(OrderStatus.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                    {
                        entity.MarkLoaded(OrderStatus.__TeaqlFields.NAME_PROPERTY_MEMBER);
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue(OrderStatus.__TeaqlFields.CODE_PROPERTY_STORAGE, out var codeValue))
                    {
                        entity.MarkLoaded(OrderStatus.__TeaqlFields.CODE_PROPERTY_MEMBER);
                        if (codeValue.Raw != null)
                            entity.Code = Convert.ToString(codeValue.Raw);
                    }
                    if (record.TryGetValue(OrderStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE, out var colorValue))
                    {
                        entity.MarkLoaded(OrderStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER);
                        if (colorValue.Raw != null)
                            entity.Color = Convert.ToString(colorValue.Raw);
                    }
                    if (record.TryGetValue(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE, out var displayOrderValue))
                    {
                        entity.MarkLoaded(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER);
                        if (displayOrderValue.Raw != null)
                            entity.DisplayOrder = Convert.ToDecimal(displayOrderValue.Raw);
                    }
                    if (record.TryGetValue(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, out var commercePlatformValue)
                        || record.TryGetValue(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, out commercePlatformValue))
                    {
                        entity.MarkLoaded(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER);
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
                    if (record.TryGetValue(OrderStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(OrderStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
                        if (record.TryGetValue("CustomerOrderList", out var customerOrderListValue))
                        {
                            entity.MarkLoaded("CustomerOrderList");
                            entity.CustomerOrderList.IsLoaded = true;
                            if (customerOrderListValue is Value.ListValue rows)
                            {
                                if (rows.Values is SmartList<Value> carrier)
                                    entity.CustomerOrderList.Facets = carrier.Facets;
                                foreach (var row in rows.Values.OfType<Value.ObjectValue>())
                                    entity.CustomerOrderList.Add(global::Generated.Models.CustomerOrder.FromRecord(row.Value));
                            }
                        }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                OrderStatus.__TeaqlFields.ID_PROPERTY_QUERY, OrderStatus.__TeaqlFields.NAME_PROPERTY_QUERY, OrderStatus.__TeaqlFields.CODE_PROPERTY_QUERY, OrderStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, OrderStatus.__TeaqlFields.VERSION_PROPERTY_QUERY,
                OrderStatus.__TeaqlFields.ID_PROPERTY_MEMBER, OrderStatus.__TeaqlFields.NAME_PROPERTY_MEMBER, OrderStatus.__TeaqlFields.CODE_PROPERTY_MEMBER, OrderStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER, OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER, OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, OrderStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER,
                "CustomerOrderList"
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static OrderStatus FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<OrderStatus> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("OrderStatus");
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
            foreach (var child in CustomerOrderList)
                child.TeaqlValidateCreationGraph(visited,
                    (child.TeaqlCreationIntent || child.IsLoaded("Status")) && child.Status != Id);
        }

        internal void TeaqlPrepareInputGraph(UserContext context, GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Prepare)) return;
            if (TeaqlIsCancelledNew()) return;
            graph.DiscardRestoredFields("OrderStatus", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(OrderStatus.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderStatus", Location = ObjectLocation.Property(OrderStatus.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderStatus.__TeaqlFields.NAME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderStatus", Location = ObjectLocation.Property(OrderStatus.__TeaqlFields.NAME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderStatus.__TeaqlFields.CODE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderStatus", Location = ObjectLocation.Property(OrderStatus.__TeaqlFields.CODE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderStatus", Location = ObjectLocation.Property(OrderStatus.__TeaqlFields.COLOR_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderStatus", Location = ObjectLocation.Property(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderStatus", Location = ObjectLocation.Property(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(OrderStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderStatus", Location = ObjectLocation.Property(OrderStatus.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            for (var index = 0; index < CustomerOrderList.Count; index++)
            {
                var child = CustomerOrderList[index];
                if (child.TeaqlIsCancelledNew())
                {
                    child.TeaqlPrepareInputGraph(context, graph);
                    continue;
                }
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("Status")) && child.Status != Id) child.UpdateStatusId(Id);
                try { child.TeaqlPrepareInputGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("customer_order_list").Index(index);
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
            for (var index = 0; index < CustomerOrderList.Count; index++)
            {
                try { CustomerOrderList[index].TeaqlPreflightGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("customer_order_list").Index(index);
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
            var teaqlOriginalName = this.Name;
            var teaqlOriginalCode = this.Code;
            var teaqlOriginalColor = this.Color;
            var teaqlOriginalDisplayOrder = this.DisplayOrder;
            var teaqlOriginalCommercePlatform = this.CommercePlatform;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Name = teaqlOriginalName;
                this.Code = teaqlOriginalCode;
                this.Color = teaqlOriginalColor;
                this.DisplayOrder = teaqlOriginalDisplayOrder;
                this.CommercePlatform = teaqlOriginalCommercePlatform;
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
            foreach (var child in CustomerOrderList) await child.TeaqlReserveGraphIdentitiesAsync(graph);
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<OrderStatus> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("OrderStatus",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("OrderStatus",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for OrderStatus");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Name = saved.Name;
            this.Code = saved.Code;
            this.Color = saved.Color;
            this.DisplayOrder = saved.DisplayOrder;
            this.CommercePlatform = saved.CommercePlatform;
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
            for (var index = 0; index < CustomerOrderList.Count; index++)
            {
                var child = CustomerOrderList[index];
                if (child.TeaqlIsCancelledNew()) continue;
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("Status")) && child.Status != Id) child.UpdateStatusId(Id);
                try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("customer_order_list").Index(index);
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
                        if (IsLoaded(OrderStatus.__TeaqlFields.ID_PROPERTY_MEMBER)) record[OrderStatus.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(OrderStatus.__TeaqlFields.NAME_PROPERTY_MEMBER)) record[OrderStatus.__TeaqlFields.NAME_PROPERTY_STORAGE] = TeaqlValue(Name);
                        if (IsLoaded(OrderStatus.__TeaqlFields.CODE_PROPERTY_MEMBER)) record[OrderStatus.__TeaqlFields.CODE_PROPERTY_STORAGE] = TeaqlValue(Code);
                        if (IsLoaded(OrderStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER)) record[OrderStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE] = TeaqlValue(Color);
                        if (IsLoaded(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER)) record[OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE] = TeaqlValue(DisplayOrder);
                        if (IsLoaded(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER)) record[OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE] = TeaqlValue(CommercePlatform);
                        if (IsLoaded(OrderStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[OrderStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(OrderStatus.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                        this.Name = nameValue.Raw == null ? null : Convert.ToString(nameValue.Raw);

                    if (record.TryGetValue(OrderStatus.__TeaqlFields.CODE_PROPERTY_STORAGE, out var codeValue))
                        this.Code = codeValue.Raw == null ? null : Convert.ToString(codeValue.Raw);

                    if (record.TryGetValue(OrderStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE, out var colorValue))
                        this.Color = colorValue.Raw == null ? null : Convert.ToString(colorValue.Raw);

                    if (record.TryGetValue(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE, out var displayOrderValue))
                        this.DisplayOrder = displayOrderValue.Raw == null ? null : Convert.ToDecimal(displayOrderValue.Raw);

                    if (record.TryGetValue(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, out var commercePlatformValue))
                        this.CommercePlatform = commercePlatformValue.Raw == null ? null : Convert.ToInt64(commercePlatformValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[OrderStatus.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (Name != null) record[OrderStatus.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (Code != null) record[OrderStatus.__TeaqlFields.CODE_PROPERTY_STORAGE] = new Value.TextValue(Code);

                    if (Color != null) record[OrderStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record[OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE] = new Value.DecimalValue(DisplayOrder.Value);

                    if (CommercePlatform.HasValue) record[OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(CommercePlatform.Value);

                    if (Version.HasValue) record[OrderStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderStatus", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record[OrderStatus.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (Code != null) record[OrderStatus.__TeaqlFields.CODE_PROPERTY_STORAGE] = new Value.TextValue(Code);

                    if (Color != null) record[OrderStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record[OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE] = new Value.DecimalValue(DisplayOrder.Value);

                    if (CommercePlatform.HasValue) record[OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(CommercePlatform.Value);

                    if (Version.HasValue) record[OrderStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "OrderStatus", 
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
                Entity = "OrderStatus",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("OrderStatus");
        }

                public OrderStatus UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded(OrderStatus.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderStatus.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded(OrderStatus.__TeaqlFields.NAME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderStatus.__TeaqlFields.NAME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateCode(string value)
                {
                    this.Code = value;
                    MarkLoaded(OrderStatus.__TeaqlFields.CODE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderStatus.__TeaqlFields.CODE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateColor(string value)
                {
                    this.Color = value;
                    MarkLoaded(OrderStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateDisplayOrder(decimal? value)
                {
                    this.DisplayOrder = value;
                    MarkLoaded(OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateCommercePlatform(long? value)
                {
                    if (this.CommercePlatform != value) this.CommercePlatformEntity = null;
                    this.CommercePlatform = value;
                    MarkLoaded(OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderStatus.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public OrderStatus UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(OrderStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), OrderStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
                public OrderStatus UpdateCommercePlatform(CommercePlatform value)
                {
                    UpdateCommercePlatform(value?.Id);
                    this.CommercePlatformEntity = value;
                    return this;
                }


                public OrderStatus UpdateCommercePlatformId(long? value)
                {
                    return UpdateCommercePlatform(value);
                }

    }
}