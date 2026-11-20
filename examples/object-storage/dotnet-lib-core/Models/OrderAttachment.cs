using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class OrderAttachment : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("OrderAttachment", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField("id", global::TeaQL.Runtime.EntityJsonKind.Id, "id"),
            new global::TeaQL.Runtime.EntityJsonField("customer_order", global::TeaQL.Runtime.EntityJsonKind.Reference, "customer_order", () => global::Generated.Models.CustomerOrder._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("file_name", global::TeaQL.Runtime.EntityJsonKind.Text, "file_name"),
            new global::TeaQL.Runtime.EntityJsonField("content_type", global::TeaQL.Runtime.EntityJsonKind.Text, "content_type"),
            new global::TeaQL.Runtime.EntityJsonField("content_length", global::TeaQL.Runtime.EntityJsonKind.Integer, "content_length"),
            new global::TeaQL.Runtime.EntityJsonField("content_sha256", global::TeaQL.Runtime.EntityJsonKind.Text, "content_sha256"),
            new global::TeaQL.Runtime.EntityJsonField("object_key", global::TeaQL.Runtime.EntityJsonKind.Text, "object_key"),
            new global::TeaQL.Runtime.EntityJsonField("version", global::TeaQL.Runtime.EntityJsonKind.Version, "version")}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static OrderAttachment FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
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
            if (IsLoaded("FileName"))
            {
                yield return new KeyValuePair<string, object?>("file_name", FileName);
            }
            if (IsLoaded("ContentType"))
            {
                yield return new KeyValuePair<string, object?>("content_type", ContentType);
            }
            if (IsLoaded("ContentLength"))
            {
                yield return new KeyValuePair<string, object?>("content_length", ContentLength);
            }
            if (IsLoaded("ContentSha256"))
            {
                yield return new KeyValuePair<string, object?>("content_sha256", ContentSha256);
            }
            if (IsLoaded("ObjectKey"))
            {
                yield return new KeyValuePair<string, object?>("object_key", ObjectKey);
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
        private EntityKey TeaqlEntityKey() => new EntityKey("OrderAttachment", Id ?? _ledgerId);
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
        private OrderAttachment() { _creationState = EntityCreationState.Hydrated; }
        internal static OrderAttachment TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new OrderAttachment();
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
                public string? FileName { get; set; }
                public string? ContentType { get; set; }
                public long? ContentLength { get; set; }
                public string? ContentSha256 { get; set; }
                public string? ObjectKey { get; set; }
                public long? Version { get; set; }
                public CustomerOrder? CustomerOrderEntity { get; set; }

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
        internal static OrderAttachment TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public OrderAttachment MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public OrderAttachment MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public OrderAttachment AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public OrderAttachment MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static OrderAttachment Refer(long id)
        {
            var entity = new OrderAttachment();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static OrderAttachment FromRecord(Record record)
        {
            var entity = new OrderAttachment().MarkLoadedOnly();
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
                    if (record.TryGetValue("file_name", out var fileNameValue))
                    {
                        entity.MarkLoaded("FileName");
                        if (fileNameValue.Raw != null)
                            entity.FileName = Convert.ToString(fileNameValue.Raw);
                    }
                    if (record.TryGetValue("content_type", out var contentTypeValue))
                    {
                        entity.MarkLoaded("ContentType");
                        if (contentTypeValue.Raw != null)
                            entity.ContentType = Convert.ToString(contentTypeValue.Raw);
                    }
                    if (record.TryGetValue("content_length", out var contentLengthValue))
                    {
                        entity.MarkLoaded("ContentLength");
                        if (contentLengthValue.Raw != null)
                            entity.ContentLength = Convert.ToInt64(contentLengthValue.Raw);
                    }
                    if (record.TryGetValue("content_sha256", out var contentSha256Value))
                    {
                        entity.MarkLoaded("ContentSha256");
                        if (contentSha256Value.Raw != null)
                            entity.ContentSha256 = Convert.ToString(contentSha256Value.Raw);
                    }
                    if (record.TryGetValue("object_key", out var objectKeyValue))
                    {
                        entity.MarkLoaded("ObjectKey");
                        if (objectKeyValue.Raw != null)
                            entity.ObjectKey = Convert.ToString(objectKeyValue.Raw);
                    }
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                "id", "customer_order", "file_name", "content_type", "content_length", "content_sha256", "object_key", "version",
                "Id", "CustomerOrder", "FileName", "ContentType", "ContentLength", "ContentSha256", "ObjectKey", "Version",
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static OrderAttachment FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<OrderAttachment> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("OrderAttachment");
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
            graph.DiscardRestoredFields("OrderAttachment", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderAttachment", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("CustomerOrder"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderAttachment", Location = ObjectLocation.Property("customer_order"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("FileName"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderAttachment", Location = ObjectLocation.Property("file_name"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("ContentType"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderAttachment", Location = ObjectLocation.Property("content_type"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("ContentLength"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderAttachment", Location = ObjectLocation.Property("content_length"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("ContentSha256"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderAttachment", Location = ObjectLocation.Property("content_sha256"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("ObjectKey"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderAttachment", Location = ObjectLocation.Property("object_key"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Version"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "OrderAttachment", Location = ObjectLocation.Property("version"), Message = "Mutation requires a fully loaded entity" } });
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
            var teaqlOriginalCustomerOrder = this.CustomerOrder;
            var teaqlOriginalFileName = this.FileName;
            var teaqlOriginalContentType = this.ContentType;
            var teaqlOriginalContentLength = this.ContentLength;
            var teaqlOriginalContentSha256 = this.ContentSha256;
            var teaqlOriginalObjectKey = this.ObjectKey;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.CustomerOrder = teaqlOriginalCustomerOrder;
                this.FileName = teaqlOriginalFileName;
                this.ContentType = teaqlOriginalContentType;
                this.ContentLength = teaqlOriginalContentLength;
                this.ContentSha256 = teaqlOriginalContentSha256;
                this.ObjectKey = teaqlOriginalObjectKey;
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

        internal async global::System.Threading.Tasks.Task<OrderAttachment> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("OrderAttachment",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("OrderAttachment",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for OrderAttachment");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.CustomerOrder = saved.CustomerOrder;
            this.FileName = saved.FileName;
            this.ContentType = saved.ContentType;
            this.ContentLength = saved.ContentLength;
            this.ContentSha256 = saved.ContentSha256;
            this.ObjectKey = saved.ObjectKey;
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
                        if (IsLoaded("CustomerOrder")) record["customer_order"] = TeaqlValue(CustomerOrder);
                        if (IsLoaded("FileName")) record["file_name"] = TeaqlValue(FileName);
                        if (IsLoaded("ContentType")) record["content_type"] = TeaqlValue(ContentType);
                        if (IsLoaded("ContentLength")) record["content_length"] = TeaqlValue(ContentLength);
                        if (IsLoaded("ContentSha256")) record["content_sha256"] = TeaqlValue(ContentSha256);
                        if (IsLoaded("ObjectKey")) record["object_key"] = TeaqlValue(ObjectKey);
                        if (IsLoaded("Version")) record["version"] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue("customer_order", out var customerOrderValue))
                        this.CustomerOrder = customerOrderValue.Raw == null ? null : Convert.ToInt64(customerOrderValue.Raw);

                    if (record.TryGetValue("file_name", out var fileNameValue))
                        this.FileName = fileNameValue.Raw == null ? null : Convert.ToString(fileNameValue.Raw);

                    if (record.TryGetValue("content_type", out var contentTypeValue))
                        this.ContentType = contentTypeValue.Raw == null ? null : Convert.ToString(contentTypeValue.Raw);

                    if (record.TryGetValue("content_length", out var contentLengthValue))
                        this.ContentLength = contentLengthValue.Raw == null ? null : Convert.ToInt64(contentLengthValue.Raw);

                    if (record.TryGetValue("content_sha256", out var contentSha256Value))
                        this.ContentSha256 = contentSha256Value.Raw == null ? null : Convert.ToString(contentSha256Value.Raw);

                    if (record.TryGetValue("object_key", out var objectKeyValue))
                        this.ObjectKey = objectKeyValue.Raw == null ? null : Convert.ToString(objectKeyValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (CustomerOrder.HasValue) record["customer_order"] = new Value.I64Value(CustomerOrder.Value);

                    if (FileName != null) record["file_name"] = new Value.TextValue(FileName);

                    if (ContentType != null) record["content_type"] = new Value.TextValue(ContentType);

                    if (ContentLength.HasValue) record["content_length"] = new Value.I64Value(ContentLength.Value);

                    if (ContentSha256 != null) record["content_sha256"] = new Value.TextValue(ContentSha256);

                    if (ObjectKey != null) record["object_key"] = new Value.TextValue(ObjectKey);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderAttachment", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (CustomerOrder.HasValue) record["customer_order"] = new Value.I64Value(CustomerOrder.Value);

                    if (FileName != null) record["file_name"] = new Value.TextValue(FileName);

                    if (ContentType != null) record["content_type"] = new Value.TextValue(ContentType);

                    if (ContentLength.HasValue) record["content_length"] = new Value.I64Value(ContentLength.Value);

                    if (ContentSha256 != null) record["content_sha256"] = new Value.TextValue(ContentSha256);

                    if (ObjectKey != null) record["object_key"] = new Value.TextValue(ObjectKey);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "OrderAttachment", 
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
                Entity = "OrderAttachment",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("OrderAttachment");
        }

                public OrderAttachment UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public OrderAttachment UpdateCustomerOrder(long? value)
                {
                    if (this.CustomerOrder != value) this.CustomerOrderEntity = null;
                    this.CustomerOrder = value;
                    MarkLoaded("CustomerOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "customer_order", TeaqlValue(value));
                    return this;
                }

                public OrderAttachment UpdateFileName(string value)
                {
                    this.FileName = value;
                    MarkLoaded("FileName");
                    _entityRoot.Set(TeaqlEntityKey(), "file_name", TeaqlValue(value));
                    return this;
                }

                public OrderAttachment UpdateContentType(string value)
                {
                    this.ContentType = value;
                    MarkLoaded("ContentType");
                    _entityRoot.Set(TeaqlEntityKey(), "content_type", TeaqlValue(value));
                    return this;
                }

                public OrderAttachment UpdateContentLength(long? value)
                {
                    this.ContentLength = value;
                    MarkLoaded("ContentLength");
                    _entityRoot.Set(TeaqlEntityKey(), "content_length", TeaqlValue(value));
                    return this;
                }

                public OrderAttachment UpdateContentSha256(string value)
                {
                    this.ContentSha256 = value;
                    MarkLoaded("ContentSha256");
                    _entityRoot.Set(TeaqlEntityKey(), "content_sha256", TeaqlValue(value));
                    return this;
                }

                public OrderAttachment UpdateObjectKey(string value)
                {
                    this.ObjectKey = value;
                    MarkLoaded("ObjectKey");
                    _entityRoot.Set(TeaqlEntityKey(), "object_key", TeaqlValue(value));
                    return this;
                }

                public OrderAttachment UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public OrderAttachment UpdateCustomerOrder(CustomerOrder value)
                {
                    UpdateCustomerOrder(value?.Id);
                    this.CustomerOrderEntity = value;
                    return this;
                }


                public OrderAttachment UpdateCustomerOrderId(long? value)
                {
                    return UpdateCustomerOrder(value);
                }

    }
}