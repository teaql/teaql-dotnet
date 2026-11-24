using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class TaskStatus : global::TeaQL.Runtime.IEntityJsonDocument
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
            internal const string PROGRESS_PROPERTY_MEMBER = "Progress";
            internal const string PROGRESS_PROPERTY_QUERY = "progress";
            internal const string PROGRESS_PROPERTY_STORAGE = "progress";
            internal const string PROGRESS_PROPERTY_MODEL = "progress";
            internal const string PLATFORM_PROPERTY_MEMBER = "Platform";
            internal const string PLATFORM_PROPERTY_QUERY = "platform";
            internal const string PLATFORM_PROPERTY_STORAGE = "platform";
            internal const string PLATFORM_PROPERTY_MODEL = "platform";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("TaskStatus", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(TaskStatus.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, TaskStatus.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(TaskStatus.__TeaqlFields.NAME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, TaskStatus.__TeaqlFields.NAME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(TaskStatus.__TeaqlFields.CODE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, TaskStatus.__TeaqlFields.CODE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(TaskStatus.__TeaqlFields.COLOR_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, TaskStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Decimal, TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Decimal, TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, () => global::Generated.Models.Platform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(TaskStatus.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, TaskStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField("task_list", global::TeaQL.Runtime.EntityJsonKind.Many, "TaskList", () => global::Generated.Models.Task._teaqlJsonSchema)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static TaskStatus FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(TaskStatus.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(TaskStatus.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(TaskStatus.__TeaqlFields.NAME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(TaskStatus.__TeaqlFields.NAME_PROPERTY_STORAGE, Name);
            }
            if (IsLoaded(TaskStatus.__TeaqlFields.CODE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(TaskStatus.__TeaqlFields.CODE_PROPERTY_STORAGE, Code);
            }
            if (IsLoaded(TaskStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(TaskStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE, Color);
            }
            if (IsLoaded(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE, DisplayOrder);
            }
            if (IsLoaded(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_STORAGE, Progress);
            }
            if (IsLoaded(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER))
            {
                object? jsonPlatform = IsLoaded("PlatformEntity") && PlatformEntity != null && PlatformEntity.Id == Platform
                    ? PlatformEntity : Platform;
                yield return new KeyValuePair<string, object?>(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, jsonPlatform);
            }
            if (IsLoaded(TaskStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(TaskStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
            if (TaskList.IsLoaded || TaskList.Any(child => child.TeaqlHasPending()))
                yield return new KeyValuePair<string, object?>("TaskList", TaskList);
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("TaskStatus", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal bool TeaqlCreationIntent => _teaqlForceCreate || _entityRoot.IsNew(TeaqlEntityKey());
        internal bool TeaqlHasPendingGraph(HashSet<object> visited)
        {
            if (!visited.Add(this)) return false;
            if (TeaqlCreationIntent || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey())) return true;
            foreach (var child in TaskList)
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
            foreach (var child in TaskList)
                if (child.TeaqlHasPendingGraph(visited))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_deletion_state", EntityType = "TaskStatus", Location = ObjectLocation.Property("id"), Message = "Cancelled new entity has pending descendants; explicit resolution is required" } });
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
            foreach (var child in TaskList) child.AttachRoot(root, hydration);
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
        private TaskStatus() { _creationState = EntityCreationState.Hydrated; }
        internal static TaskStatus TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new TaskStatus();
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
                public decimal? Progress { get; set; }
                public long? Platform { get; set; }
                public long? Version { get; set; }
                public Platform? PlatformEntity { get; set; }
                public SmartList<Task> TaskList { get; } = SmartList<Task>.Empty();

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
        internal static TaskStatus TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public TaskStatus MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public TaskStatus MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public TaskStatus AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public TaskStatus MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static TaskStatus Refer(long id)
        {
            var entity = new TaskStatus();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static TaskStatus FromRecord(Record record)
        {
            var entity = new TaskStatus().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue(TaskStatus.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(TaskStatus.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(TaskStatus.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                    {
                        entity.MarkLoaded(TaskStatus.__TeaqlFields.NAME_PROPERTY_MEMBER);
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue(TaskStatus.__TeaqlFields.CODE_PROPERTY_STORAGE, out var codeValue))
                    {
                        entity.MarkLoaded(TaskStatus.__TeaqlFields.CODE_PROPERTY_MEMBER);
                        if (codeValue.Raw != null)
                            entity.Code = Convert.ToString(codeValue.Raw);
                    }
                    if (record.TryGetValue(TaskStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE, out var colorValue))
                    {
                        entity.MarkLoaded(TaskStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER);
                        if (colorValue.Raw != null)
                            entity.Color = Convert.ToString(colorValue.Raw);
                    }
                    if (record.TryGetValue(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE, out var displayOrderValue))
                    {
                        entity.MarkLoaded(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER);
                        if (displayOrderValue.Raw != null)
                            entity.DisplayOrder = Convert.ToDecimal(displayOrderValue.Raw);
                    }
                    if (record.TryGetValue(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_STORAGE, out var progressValue))
                    {
                        entity.MarkLoaded(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_MEMBER);
                        if (progressValue.Raw != null)
                            entity.Progress = Convert.ToDecimal(progressValue.Raw);
                    }
                    if (record.TryGetValue(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, out var platformValue)
                        || record.TryGetValue(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, out platformValue))
                    {
                        entity.MarkLoaded(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER);
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
                    if (record.TryGetValue(TaskStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(TaskStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
                        if (record.TryGetValue("TaskList", out var taskListValue))
                        {
                            entity.MarkLoaded("TaskList");
                            entity.TaskList.IsLoaded = true;
                            if (taskListValue is Value.ListValue rows)
                            {
                                if (rows.Values is SmartList<Value> carrier)
                                    entity.TaskList.Facets = carrier.Facets;
                                foreach (var row in rows.Values.OfType<Value.ObjectValue>())
                                    entity.TaskList.Add(global::Generated.Models.Task.FromRecord(row.Value));
                            }
                        }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY, TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY,
                TaskStatus.__TeaqlFields.ID_PROPERTY_MEMBER, TaskStatus.__TeaqlFields.NAME_PROPERTY_MEMBER, TaskStatus.__TeaqlFields.CODE_PROPERTY_MEMBER, TaskStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER, TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER, TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_MEMBER, TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, TaskStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER,
                "TaskList"
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static TaskStatus FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<TaskStatus> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("TaskStatus");
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
            foreach (var child in TaskList)
                child.TeaqlValidateCreationGraph(visited,
                    (child.TeaqlCreationIntent || child.IsLoaded("Status")) && child.Status != Id);
        }

        internal void TeaqlPrepareInputGraph(UserContext context, GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Prepare)) return;
            if (TeaqlIsCancelledNew()) return;
            graph.DiscardRestoredFields("TaskStatus", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(TaskStatus.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "TaskStatus", Location = ObjectLocation.Property(TaskStatus.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(TaskStatus.__TeaqlFields.NAME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "TaskStatus", Location = ObjectLocation.Property(TaskStatus.__TeaqlFields.NAME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(TaskStatus.__TeaqlFields.CODE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "TaskStatus", Location = ObjectLocation.Property(TaskStatus.__TeaqlFields.CODE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(TaskStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "TaskStatus", Location = ObjectLocation.Property(TaskStatus.__TeaqlFields.COLOR_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "TaskStatus", Location = ObjectLocation.Property(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "TaskStatus", Location = ObjectLocation.Property(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "TaskStatus", Location = ObjectLocation.Property(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(TaskStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "TaskStatus", Location = ObjectLocation.Property(TaskStatus.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            for (var index = 0; index < TaskList.Count; index++)
            {
                var child = TaskList[index];
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
                    var prefix = ObjectLocation.Property("task_list").Index(index);
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
            for (var index = 0; index < TaskList.Count; index++)
            {
                try { TaskList[index].TeaqlPreflightGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("task_list").Index(index);
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
            var teaqlOriginalProgress = this.Progress;
            var teaqlOriginalPlatform = this.Platform;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Name = teaqlOriginalName;
                this.Code = teaqlOriginalCode;
                this.Color = teaqlOriginalColor;
                this.DisplayOrder = teaqlOriginalDisplayOrder;
                this.Progress = teaqlOriginalProgress;
                this.Platform = teaqlOriginalPlatform;
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
            foreach (var child in TaskList) await child.TeaqlReserveGraphIdentitiesAsync(graph);
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<TaskStatus> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("TaskStatus",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("TaskStatus",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for TaskStatus");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Name = saved.Name;
            this.Code = saved.Code;
            this.Color = saved.Color;
            this.DisplayOrder = saved.DisplayOrder;
            this.Progress = saved.Progress;
            this.Platform = saved.Platform;
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
            for (var index = 0; index < TaskList.Count; index++)
            {
                var child = TaskList[index];
                if (child.TeaqlIsCancelledNew()) continue;
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("Status")) && child.Status != Id) child.UpdateStatusId(Id);
                try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("task_list").Index(index);
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
                        if (IsLoaded(TaskStatus.__TeaqlFields.ID_PROPERTY_MEMBER)) record[TaskStatus.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(TaskStatus.__TeaqlFields.NAME_PROPERTY_MEMBER)) record[TaskStatus.__TeaqlFields.NAME_PROPERTY_STORAGE] = TeaqlValue(Name);
                        if (IsLoaded(TaskStatus.__TeaqlFields.CODE_PROPERTY_MEMBER)) record[TaskStatus.__TeaqlFields.CODE_PROPERTY_STORAGE] = TeaqlValue(Code);
                        if (IsLoaded(TaskStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER)) record[TaskStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE] = TeaqlValue(Color);
                        if (IsLoaded(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER)) record[TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE] = TeaqlValue(DisplayOrder);
                        if (IsLoaded(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_MEMBER)) record[TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_STORAGE] = TeaqlValue(Progress);
                        if (IsLoaded(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)) record[TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = TeaqlValue(Platform);
                        if (IsLoaded(TaskStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[TaskStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(TaskStatus.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                        this.Name = nameValue.Raw == null ? null : Convert.ToString(nameValue.Raw);

                    if (record.TryGetValue(TaskStatus.__TeaqlFields.CODE_PROPERTY_STORAGE, out var codeValue))
                        this.Code = codeValue.Raw == null ? null : Convert.ToString(codeValue.Raw);

                    if (record.TryGetValue(TaskStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE, out var colorValue))
                        this.Color = colorValue.Raw == null ? null : Convert.ToString(colorValue.Raw);

                    if (record.TryGetValue(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE, out var displayOrderValue))
                        this.DisplayOrder = displayOrderValue.Raw == null ? null : Convert.ToDecimal(displayOrderValue.Raw);

                    if (record.TryGetValue(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_STORAGE, out var progressValue))
                        this.Progress = progressValue.Raw == null ? null : Convert.ToDecimal(progressValue.Raw);

                    if (record.TryGetValue(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, out var platformValue))
                        this.Platform = platformValue.Raw == null ? null : Convert.ToInt64(platformValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[TaskStatus.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (Name != null) record[TaskStatus.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (Code != null) record[TaskStatus.__TeaqlFields.CODE_PROPERTY_STORAGE] = new Value.TextValue(Code);

                    if (Color != null) record[TaskStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record[TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE] = new Value.DecimalValue(DisplayOrder.Value);

                    if (Progress.HasValue) record[TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_STORAGE] = new Value.DecimalValue(Progress.Value);

                    if (Platform.HasValue) record[TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(Platform.Value);

                    if (Version.HasValue) record[TaskStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "TaskStatus", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record[TaskStatus.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (Code != null) record[TaskStatus.__TeaqlFields.CODE_PROPERTY_STORAGE] = new Value.TextValue(Code);

                    if (Color != null) record[TaskStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record[TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE] = new Value.DecimalValue(DisplayOrder.Value);

                    if (Progress.HasValue) record[TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_STORAGE] = new Value.DecimalValue(Progress.Value);

                    if (Platform.HasValue) record[TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(Platform.Value);

                    if (Version.HasValue) record[TaskStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "TaskStatus", 
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
                Entity = "TaskStatus",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("TaskStatus");
        }

                public TaskStatus UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded(TaskStatus.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), TaskStatus.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public TaskStatus UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded(TaskStatus.__TeaqlFields.NAME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), TaskStatus.__TeaqlFields.NAME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public TaskStatus UpdateCode(string value)
                {
                    this.Code = value;
                    MarkLoaded(TaskStatus.__TeaqlFields.CODE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), TaskStatus.__TeaqlFields.CODE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public TaskStatus UpdateColor(string value)
                {
                    this.Color = value;
                    MarkLoaded(TaskStatus.__TeaqlFields.COLOR_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), TaskStatus.__TeaqlFields.COLOR_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public TaskStatus UpdateDisplayOrder(decimal? value)
                {
                    this.DisplayOrder = value;
                    MarkLoaded(TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public TaskStatus UpdateProgress(decimal? value)
                {
                    this.Progress = value;
                    MarkLoaded(TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public TaskStatus UpdatePlatform(long? value)
                {
                    if (this.Platform != value) this.PlatformEntity = null;
                    this.Platform = value;
                    MarkLoaded(TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public TaskStatus UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(TaskStatus.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), TaskStatus.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
                public TaskStatus UpdatePlatform(Platform value)
                {
                    UpdatePlatform(value?.Id);
                    this.PlatformEntity = value;
                    return this;
                }


                public TaskStatus UpdatePlatformId(long? value)
                {
                    return UpdatePlatform(value);
                }

    }
}