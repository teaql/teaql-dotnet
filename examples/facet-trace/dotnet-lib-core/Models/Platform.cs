using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class Platform : global::TeaQL.Runtime.IEntityJsonDocument
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
            internal const string BASE_URL_PROPERTY_MEMBER = "BaseUrl";
            internal const string BASE_URL_PROPERTY_QUERY = "base_url";
            internal const string BASE_URL_PROPERTY_STORAGE = "base_url";
            internal const string BASE_URL_PROPERTY_MODEL = "base_url";
            internal const string CREATE_TIME_PROPERTY_MEMBER = "CreateTime";
            internal const string CREATE_TIME_PROPERTY_QUERY = "create_time";
            internal const string CREATE_TIME_PROPERTY_STORAGE = "create_time";
            internal const string CREATE_TIME_PROPERTY_MODEL = "create_time";
            internal const string UPDATE_TIME_PROPERTY_MEMBER = "UpdateTime";
            internal const string UPDATE_TIME_PROPERTY_QUERY = "update_time";
            internal const string UPDATE_TIME_PROPERTY_STORAGE = "update_time";
            internal const string UPDATE_TIME_PROPERTY_MODEL = "update_time";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("Platform", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(Platform.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, Platform.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Platform.__TeaqlFields.NAME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, Platform.__TeaqlFields.NAME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Platform.__TeaqlFields.BASE_URL_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, Platform.__TeaqlFields.BASE_URL_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, Platform.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Platform.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, Platform.__TeaqlFields.VERSION_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField("school_type_list", global::TeaQL.Runtime.EntityJsonKind.Many, "SchoolTypeList", () => global::Generated.Models.SchoolType._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("school_list", global::TeaQL.Runtime.EntityJsonKind.Many, "SchoolList", () => global::Generated.Models.School._teaqlJsonSchema)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static Platform FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(Platform.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Platform.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(Platform.__TeaqlFields.NAME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Platform.__TeaqlFields.NAME_PROPERTY_STORAGE, Name);
            }
            if (IsLoaded(Platform.__TeaqlFields.BASE_URL_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Platform.__TeaqlFields.BASE_URL_PROPERTY_STORAGE, BaseUrl);
            }
            if (IsLoaded(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, CreateTime);
            }
            if (IsLoaded(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, UpdateTime);
            }
            if (IsLoaded(Platform.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Platform.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
            if (SchoolTypeList.IsLoaded || SchoolTypeList.Any(child => child.TeaqlHasPending()))
                yield return new KeyValuePair<string, object?>("SchoolTypeList", SchoolTypeList);
            if (SchoolList.IsLoaded || SchoolList.Any(child => child.TeaqlHasPending()))
                yield return new KeyValuePair<string, object?>("SchoolList", SchoolList);
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("Platform", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal bool TeaqlCreationIntent => _teaqlForceCreate || _entityRoot.IsNew(TeaqlEntityKey());
        internal bool TeaqlHasPendingGraph(HashSet<object> visited)
        {
            if (!visited.Add(this)) return false;
            if (TeaqlCreationIntent || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey())) return true;
            foreach (var child in SchoolTypeList)
                if (child.TeaqlHasPendingGraph(visited)) return true;
            foreach (var child in SchoolList)
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
            foreach (var child in SchoolTypeList)
                if (child.TeaqlHasPendingGraph(visited))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_deletion_state", EntityType = "Platform", Location = ObjectLocation.Property("id"), Message = "Cancelled new entity has pending descendants; explicit resolution is required" } });
            foreach (var child in SchoolList)
                if (child.TeaqlHasPendingGraph(visited))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_deletion_state", EntityType = "Platform", Location = ObjectLocation.Property("id"), Message = "Cancelled new entity has pending descendants; explicit resolution is required" } });
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
            foreach (var child in SchoolTypeList) child.AttachRoot(root, hydration);
            foreach (var child in SchoolList) child.AttachRoot(root, hydration);
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
        private Platform() { _creationState = EntityCreationState.Hydrated; }
        internal static Platform TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new Platform();
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
                public string? BaseUrl { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public SmartList<SchoolType> SchoolTypeList { get; } = SmartList<SchoolType>.Empty();
                public SmartList<School> SchoolList { get; } = SmartList<School>.Empty();

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
        internal static Platform TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public Platform MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public Platform MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public Platform AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public Platform MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static Platform Refer(long id)
        {
            var entity = new Platform();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static Platform FromRecord(Record record)
        {
            var entity = new Platform().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue(Platform.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(Platform.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(Platform.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                    {
                        entity.MarkLoaded(Platform.__TeaqlFields.NAME_PROPERTY_MEMBER);
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue(Platform.__TeaqlFields.BASE_URL_PROPERTY_STORAGE, out var baseUrlValue))
                    {
                        entity.MarkLoaded(Platform.__TeaqlFields.BASE_URL_PROPERTY_MEMBER);
                        if (baseUrlValue.Raw != null)
                            entity.BaseUrl = Convert.ToString(baseUrlValue.Raw);
                    }
                    if (record.TryGetValue(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, out var createTimeValue))
                    {
                        entity.MarkLoaded(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER);
                        if (createTimeValue.Raw != null)
                            entity.CreateTime = TeaqlDateTime(createTimeValue);
                    }
                    if (record.TryGetValue(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, out var updateTimeValue))
                    {
                        entity.MarkLoaded(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER);
                        if (updateTimeValue.Raw != null)
                            entity.UpdateTime = TeaqlDateTime(updateTimeValue);
                    }
                    if (record.TryGetValue(Platform.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(Platform.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
                        if (record.TryGetValue("SchoolTypeList", out var schoolTypeListValue))
                        {
                            entity.MarkLoaded("SchoolTypeList");
                            entity.SchoolTypeList.IsLoaded = true;
                            if (schoolTypeListValue is Value.ListValue rows)
                            {
                                if (rows.Values is SmartList<Value> carrier)
                                    entity.SchoolTypeList.Facets = carrier.Facets;
                                foreach (var row in rows.Values.OfType<Value.ObjectValue>())
                                    entity.SchoolTypeList.Add(global::Generated.Models.SchoolType.FromRecord(row.Value));
                            }
                        }
                        if (record.TryGetValue("SchoolList", out var schoolListValue))
                        {
                            entity.MarkLoaded("SchoolList");
                            entity.SchoolList.IsLoaded = true;
                            if (schoolListValue is Value.ListValue rows)
                            {
                                if (rows.Values is SmartList<Value> carrier)
                                    entity.SchoolList.Facets = carrier.Facets;
                                foreach (var row in rows.Values.OfType<Value.ObjectValue>())
                                    entity.SchoolList.Add(global::Generated.Models.School.FromRecord(row.Value));
                            }
                        }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                Platform.__TeaqlFields.ID_PROPERTY_QUERY, Platform.__TeaqlFields.NAME_PROPERTY_QUERY, Platform.__TeaqlFields.BASE_URL_PROPERTY_QUERY, Platform.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, Platform.__TeaqlFields.VERSION_PROPERTY_QUERY,
                Platform.__TeaqlFields.ID_PROPERTY_MEMBER, Platform.__TeaqlFields.NAME_PROPERTY_MEMBER, Platform.__TeaqlFields.BASE_URL_PROPERTY_MEMBER, Platform.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER, Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER, Platform.__TeaqlFields.VERSION_PROPERTY_MEMBER,
                "SchoolTypeList", "SchoolList"
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static Platform FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<Platform> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("Platform");
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
            foreach (var child in SchoolTypeList)
                child.TeaqlValidateCreationGraph(visited,
                    (child.TeaqlCreationIntent || child.IsLoaded("Platform")) && child.Platform != Id);
            foreach (var child in SchoolList)
                child.TeaqlValidateCreationGraph(visited,
                    (child.TeaqlCreationIntent || child.IsLoaded("Platform")) && child.Platform != Id);
        }

        internal void TeaqlPrepareInputGraph(UserContext context, GraphMutationSession graph)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Prepare)) return;
            if (TeaqlIsCancelledNew()) return;
            graph.DiscardRestoredFields("Platform", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(Platform.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Platform", Location = ObjectLocation.Property(Platform.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Platform.__TeaqlFields.NAME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Platform", Location = ObjectLocation.Property(Platform.__TeaqlFields.NAME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Platform.__TeaqlFields.BASE_URL_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Platform", Location = ObjectLocation.Property(Platform.__TeaqlFields.BASE_URL_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Platform", Location = ObjectLocation.Property(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Platform", Location = ObjectLocation.Property(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Platform.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Platform", Location = ObjectLocation.Property(Platform.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            for (var index = 0; index < SchoolTypeList.Count; index++)
            {
                var child = SchoolTypeList[index];
                if (child.TeaqlIsCancelledNew())
                {
                    child.TeaqlPrepareInputGraph(context, graph);
                    continue;
                }
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("Platform")) && child.Platform != Id) child.UpdatePlatformId(Id);
                try { child.TeaqlPrepareInputGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("school_type_list").Index(index);
                    throw new CheckException(context.TranslateCheckResults(error.Violations.Select(violation =>
                        violation.PrefixedBy(prefix)).ToArray()));
                }
            }
            for (var index = 0; index < SchoolList.Count; index++)
            {
                var child = SchoolList[index];
                if (child.TeaqlIsCancelledNew())
                {
                    child.TeaqlPrepareInputGraph(context, graph);
                    continue;
                }
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("Platform")) && child.Platform != Id) child.UpdatePlatformId(Id);
                try { child.TeaqlPrepareInputGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("school_list").Index(index);
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
            for (var index = 0; index < SchoolTypeList.Count; index++)
            {
                try { SchoolTypeList[index].TeaqlPreflightGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("school_type_list").Index(index);
                    throw new CheckException(context.TranslateCheckResults(error.Violations.Select(violation =>
                        violation.PrefixedBy(prefix)).ToArray()));
                }
            }
            for (var index = 0; index < SchoolList.Count; index++)
            {
                try { SchoolList[index].TeaqlPreflightGraph(context, graph); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("school_list").Index(index);
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
            var teaqlOriginalBaseUrl = this.BaseUrl;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalUpdateTime = this.UpdateTime;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Name = teaqlOriginalName;
                this.BaseUrl = teaqlOriginalBaseUrl;
                this.CreateTime = teaqlOriginalCreateTime;
                this.UpdateTime = teaqlOriginalUpdateTime;
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
            foreach (var child in SchoolTypeList) await child.TeaqlReserveGraphIdentitiesAsync(graph);
            foreach (var child in SchoolList) await child.TeaqlReserveGraphIdentitiesAsync(graph);
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<Platform> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("Platform",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("Platform",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for Platform");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Name = saved.Name;
            this.BaseUrl = saved.BaseUrl;
            this.CreateTime = saved.CreateTime;
            this.UpdateTime = saved.UpdateTime;
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
            for (var index = 0; index < SchoolTypeList.Count; index++)
            {
                var child = SchoolTypeList[index];
                if (child.TeaqlIsCancelledNew()) continue;
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("Platform")) && child.Platform != Id) child.UpdatePlatformId(Id);
                try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("school_type_list").Index(index);
                    throw new CheckException(context.TranslateCheckResults(error.Violations.Select(violation =>
                        violation.PrefixedBy(prefix)).ToArray()));
                }
            }
            for (var index = 0; index < SchoolList.Count; index++)
            {
                var child = SchoolList[index];
                if (child.TeaqlIsCancelledNew()) continue;
                child.AttachRoot(_entityRoot);
                if ((child.TeaqlCreationIntent || child.IsLoaded("Platform")) && child.Platform != Id) child.UpdatePlatformId(Id);
                try { await child.TeaqlSaveWithinGraphAsync(context, graph, scope); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("school_list").Index(index);
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
                        if (IsLoaded(Platform.__TeaqlFields.ID_PROPERTY_MEMBER)) record[Platform.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(Platform.__TeaqlFields.NAME_PROPERTY_MEMBER)) record[Platform.__TeaqlFields.NAME_PROPERTY_STORAGE] = TeaqlValue(Name);
                        if (IsLoaded(Platform.__TeaqlFields.BASE_URL_PROPERTY_MEMBER)) record[Platform.__TeaqlFields.BASE_URL_PROPERTY_STORAGE] = TeaqlValue(BaseUrl);
                        if (IsLoaded(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER)) record[Platform.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = TeaqlValue(CreateTime);
                        if (IsLoaded(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER)) record[Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = TeaqlValue(UpdateTime);
                        if (IsLoaded(Platform.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[Platform.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(Platform.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                        this.Name = nameValue.Raw == null ? null : Convert.ToString(nameValue.Raw);

                    if (record.TryGetValue(Platform.__TeaqlFields.BASE_URL_PROPERTY_STORAGE, out var baseUrlValue))
                        this.BaseUrl = baseUrlValue.Raw == null ? null : Convert.ToString(baseUrlValue.Raw);

                    if (record.TryGetValue(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, out var createTimeValue))
                        this.CreateTime = createTimeValue.Raw == null ? null : TeaqlDateTime(createTimeValue);

                    if (record.TryGetValue(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, out var updateTimeValue))
                        this.UpdateTime = updateTimeValue.Raw == null ? null : TeaqlDateTime(updateTimeValue);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[Platform.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (Name != null) record[Platform.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (BaseUrl != null) record[Platform.__TeaqlFields.BASE_URL_PROPERTY_STORAGE] = new Value.TextValue(BaseUrl);

                    if (CreateTime.HasValue) record[Platform.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record[Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record[Platform.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "Platform", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record[Platform.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (BaseUrl != null) record[Platform.__TeaqlFields.BASE_URL_PROPERTY_STORAGE] = new Value.TextValue(BaseUrl);

                    if (CreateTime.HasValue) record[Platform.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record[Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record[Platform.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "Platform", 
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
                Entity = "Platform",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("Platform");
        }

                public Platform UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded(Platform.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Platform.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Platform UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded(Platform.__TeaqlFields.NAME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Platform.__TeaqlFields.NAME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Platform UpdateBaseUrl(string value)
                {
                    this.BaseUrl = value;
                    MarkLoaded(Platform.__TeaqlFields.BASE_URL_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Platform.__TeaqlFields.BASE_URL_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Platform UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded(Platform.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Platform.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Platform UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded(Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Platform.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Platform UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(Platform.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Platform.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
    }
}