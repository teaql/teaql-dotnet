using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class Payment : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static class __TeaqlFields
        {
            internal const string ID_PROPERTY_MEMBER = "Id";
            internal const string ID_PROPERTY_QUERY = "id";
            internal const string ID_PROPERTY_STORAGE = "id";
            internal const string ID_PROPERTY_MODEL = "id";
            internal const string CUSTOMER_ORDER_PROPERTY_MEMBER = "CustomerOrder";
            internal const string CUSTOMER_ORDER_PROPERTY_QUERY = "customer_order";
            internal const string CUSTOMER_ORDER_PROPERTY_STORAGE = "customer_order";
            internal const string CUSTOMER_ORDER_PROPERTY_MODEL = "customer_order";
            internal const string REFERENCE_CODE_PROPERTY_MEMBER = "ReferenceCode";
            internal const string REFERENCE_CODE_PROPERTY_QUERY = "reference_code";
            internal const string REFERENCE_CODE_PROPERTY_STORAGE = "reference_code";
            internal const string REFERENCE_CODE_PROPERTY_MODEL = "reference_code";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("Payment", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(Payment.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, Payment.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_STORAGE, () => global::Generated.Models.CustomerOrder._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Payment.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, Payment.__TeaqlFields.VERSION_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField("payment_attempt_list", global::TeaQL.Runtime.EntityJsonKind.Many, "PaymentAttemptList", () => global::Generated.Models.PaymentAttempt._teaqlJsonSchema)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static Payment FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(Payment.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Payment.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER))
            {
                object? jsonCustomerOrder = IsLoaded("CustomerOrderEntity") && CustomerOrderEntity != null && CustomerOrderEntity.Id == CustomerOrder
                    ? CustomerOrderEntity : CustomerOrder;
                yield return new KeyValuePair<string, object?>(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_STORAGE, jsonCustomerOrder);
            }
            if (IsLoaded(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_STORAGE, ReferenceCode);
            }
            if (IsLoaded(Payment.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Payment.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
            if (PaymentAttemptList.IsLoaded || PaymentAttemptList.Any(child => child.TeaqlHasPending()))
                yield return new KeyValuePair<string, object?>("PaymentAttemptList", PaymentAttemptList);
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("Payment", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal bool TeaqlCreationIntent => _teaqlForceCreate || _entityRoot.IsNew(TeaqlEntityKey());
        internal bool TeaqlHasPendingGraph(HashSet<object> visited)
        {
            if (!visited.Add(this)) return false;
            if (TeaqlCreationIntent || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey())) return true;
            foreach (var child in PaymentAttemptList)
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
            foreach (var child in PaymentAttemptList)
                if (child.TeaqlHasPendingGraph(visited))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_deletion_state", EntityType = "Payment", Location = ObjectLocation.Property("id"), Message = "Cancelled new entity has pending descendants; explicit resolution is required" } });
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
            foreach (var child in PaymentAttemptList) child.AttachRoot(root, hydration);
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
        private Payment() { _creationState = EntityCreationState.Hydrated; }
        internal static Payment TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new Payment();
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
                public long? CustomerOrder { get; set; }
                public string? ReferenceCode { get; set; }
                public long? Version { get; set; }
                public CustomerOrder? CustomerOrderEntity { get; set; }
                public SmartList<PaymentAttempt> PaymentAttemptList { get; } = SmartList<PaymentAttempt>.Empty();

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
        internal static Payment TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public Payment MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public Payment MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public Payment AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public Payment MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static Payment Refer(long id)
        {
            var entity = new Payment();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static Payment FromRecord(Record record)
        {
            var entity = new Payment().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue(Payment.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(Payment.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, out var customerOrderValue)
                        || record.TryGetValue(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_STORAGE, out customerOrderValue))
                    {
                        entity.MarkLoaded(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER);
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
                    if (record.TryGetValue(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_STORAGE, out var referenceCodeValue))
                    {
                        entity.MarkLoaded(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_MEMBER);
                        if (referenceCodeValue.Raw != null)
                            entity.ReferenceCode = Convert.ToString(referenceCodeValue.Raw);
                    }
                    if (record.TryGetValue(Payment.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(Payment.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
                        if (record.TryGetValue("PaymentAttemptList", out var paymentAttemptListValue))
                        {
                            entity.MarkLoaded("PaymentAttemptList");
                            entity.PaymentAttemptList.IsLoaded = true;
                            if (paymentAttemptListValue is Value.ListValue rows)
                            {
                                if (rows.Values is SmartList<Value> carrier)
                                    entity.PaymentAttemptList.Facets = carrier.Facets;
                                foreach (var row in rows.Values.OfType<Value.ObjectValue>())
                                    entity.PaymentAttemptList.Add(global::Generated.Models.PaymentAttempt.FromRecord(row.Value));
                            }
                        }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                Payment.__TeaqlFields.ID_PROPERTY_QUERY, Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, Payment.__TeaqlFields.VERSION_PROPERTY_QUERY,
                Payment.__TeaqlFields.ID_PROPERTY_MEMBER, Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_MEMBER, Payment.__TeaqlFields.VERSION_PROPERTY_MEMBER,
                "PaymentAttemptList"
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static Payment FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<Payment> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("Payment");
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
            foreach (var child in PaymentAttemptList)
                child.TeaqlValidateCreationGraph(visited,
                    (child.TeaqlCreationIntent || child.IsLoaded("Payment")) && child.Payment != Id);
        }

        internal void TeaqlPrepareInputGraph(UserContext context, GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Prepare)) return;
            if (TeaqlIsCancelledNew()) return;
            graph.DiscardRestoredFields("Payment", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(Payment.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Payment", Location = ObjectLocation.Property(Payment.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Payment", Location = ObjectLocation.Property(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Payment", Location = ObjectLocation.Property(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Payment.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Payment", Location = ObjectLocation.Property(Payment.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            for (var index = 0; index < PaymentAttemptList.Count; index++)
            {
                var child = PaymentAttemptList[index];
                if (child.TeaqlIsCancelledNew())
                {
                    child.TeaqlPrepareInputGraph(context, graph);
                    continue;
                }
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("Payment")) && child.Payment != Id) child.UpdatePaymentId(Id);
                try { child.TeaqlPrepareInputGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("payment_attempt_list").Index(index);
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
            for (var index = 0; index < PaymentAttemptList.Count; index++)
            {
                try { PaymentAttemptList[index].TeaqlPreflightGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("payment_attempt_list").Index(index);
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
            var teaqlOriginalCustomerOrder = this.CustomerOrder;
            var teaqlOriginalReferenceCode = this.ReferenceCode;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.CustomerOrder = teaqlOriginalCustomerOrder;
                this.ReferenceCode = teaqlOriginalReferenceCode;
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
            foreach (var child in PaymentAttemptList) await child.TeaqlReserveGraphIdentitiesAsync(graph);
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<Payment> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("Payment",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("Payment",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for Payment");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.CustomerOrder = saved.CustomerOrder;
            this.ReferenceCode = saved.ReferenceCode;
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
            for (var index = 0; index < PaymentAttemptList.Count; index++)
            {
                var child = PaymentAttemptList[index];
                if (child.TeaqlIsCancelledNew()) continue;
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("Payment")) && child.Payment != Id) child.UpdatePaymentId(Id);
                try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("payment_attempt_list").Index(index);
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
                        if (IsLoaded(Payment.__TeaqlFields.ID_PROPERTY_MEMBER)) record[Payment.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER)) record[Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_STORAGE] = TeaqlValue(CustomerOrder);
                        if (IsLoaded(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_MEMBER)) record[Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_STORAGE] = TeaqlValue(ReferenceCode);
                        if (IsLoaded(Payment.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[Payment.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_STORAGE, out var customerOrderValue))
                        this.CustomerOrder = customerOrderValue.Raw == null ? null : Convert.ToInt64(customerOrderValue.Raw);

                    if (record.TryGetValue(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_STORAGE, out var referenceCodeValue))
                        this.ReferenceCode = referenceCodeValue.Raw == null ? null : Convert.ToString(referenceCodeValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[Payment.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (CustomerOrder.HasValue) record[Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_STORAGE] = new Value.I64Value(CustomerOrder.Value);

                    if (ReferenceCode != null) record[Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_STORAGE] = new Value.TextValue(ReferenceCode);

                    if (Version.HasValue) record[Payment.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "Payment", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (CustomerOrder.HasValue) record[Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_STORAGE] = new Value.I64Value(CustomerOrder.Value);

                    if (ReferenceCode != null) record[Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_STORAGE] = new Value.TextValue(ReferenceCode);

                    if (Version.HasValue) record[Payment.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "Payment", 
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
                Entity = "Payment",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("Payment");
        }

                public Payment UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded(Payment.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Payment.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Payment UpdateCustomerOrder(long? value)
                {
                    if (this.CustomerOrder != value) this.CustomerOrderEntity = null;
                    this.CustomerOrder = value;
                    MarkLoaded(Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Payment UpdateReferenceCode(string value)
                {
                    this.ReferenceCode = value;
                    MarkLoaded(Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Payment UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(Payment.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Payment.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
                public Payment UpdateCustomerOrder(CustomerOrder value)
                {
                    UpdateCustomerOrder(value?.Id);
                    this.CustomerOrderEntity = value;
                    return this;
                }


                public Payment UpdateCustomerOrderId(long? value)
                {
                    return UpdateCustomerOrder(value);
                }

    }
}