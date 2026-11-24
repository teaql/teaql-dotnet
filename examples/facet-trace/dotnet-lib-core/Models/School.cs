using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class School : global::TeaQL.Runtime.IEntityJsonDocument
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
            internal const string SCHOOL_TYPE_PROPERTY_MEMBER = "SchoolType";
            internal const string SCHOOL_TYPE_PROPERTY_QUERY = "school_type";
            internal const string SCHOOL_TYPE_PROPERTY_STORAGE = "school_type";
            internal const string SCHOOL_TYPE_PROPERTY_MODEL = "school_type";
            internal const string NAME_PROPERTY_MEMBER = "Name";
            internal const string NAME_PROPERTY_QUERY = "name";
            internal const string NAME_PROPERTY_STORAGE = "name";
            internal const string NAME_PROPERTY_MODEL = "name";
            internal const string ADDRESS_PROPERTY_MEMBER = "Address";
            internal const string ADDRESS_PROPERTY_QUERY = "address";
            internal const string ADDRESS_PROPERTY_STORAGE = "address";
            internal const string ADDRESS_PROPERTY_MODEL = "address";
            internal const string ESTABLISHED_DATE_PROPERTY_MEMBER = "EstablishedDate";
            internal const string ESTABLISHED_DATE_PROPERTY_QUERY = "established_date";
            internal const string ESTABLISHED_DATE_PROPERTY_STORAGE = "established_date";
            internal const string ESTABLISHED_DATE_PROPERTY_MODEL = "established_date";
            internal const string STUDENT_CAPACITY_PROPERTY_MEMBER = "StudentCapacity";
            internal const string STUDENT_CAPACITY_PROPERTY_QUERY = "student_capacity";
            internal const string STUDENT_CAPACITY_PROPERTY_STORAGE = "student_capacity";
            internal const string STUDENT_CAPACITY_PROPERTY_MODEL = "student_capacity";
            internal const string ACTIVE_PROPERTY_MEMBER = "Active";
            internal const string ACTIVE_PROPERTY_QUERY = "active";
            internal const string ACTIVE_PROPERTY_STORAGE = "active";
            internal const string ACTIVE_PROPERTY_MODEL = "active";
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

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("School", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, School.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.PLATFORM_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, School.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, () => global::Generated.Models.Platform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_STORAGE, () => global::Generated.Models.SchoolType._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.NAME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, School.__TeaqlFields.NAME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.ADDRESS_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, School.__TeaqlFields.ADDRESS_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Date, School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Integer, School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.ACTIVE_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Boolean, School.__TeaqlFields.ACTIVE_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.CREATE_TIME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, School.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.UPDATE_TIME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, School.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(School.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, School.__TeaqlFields.VERSION_PROPERTY_STORAGE)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static School FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(School.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER))
            {
                object? jsonPlatform = IsLoaded("PlatformEntity") && PlatformEntity != null && PlatformEntity.Id == Platform
                    ? PlatformEntity : Platform;
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, jsonPlatform);
            }
            if (IsLoaded(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER))
            {
                object? jsonSchoolType = IsLoaded("SchoolTypeEntity") && SchoolTypeEntity != null && SchoolTypeEntity.Id == SchoolType
                    ? SchoolTypeEntity : SchoolType;
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_STORAGE, jsonSchoolType);
            }
            if (IsLoaded(School.__TeaqlFields.NAME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.NAME_PROPERTY_STORAGE, Name);
            }
            if (IsLoaded(School.__TeaqlFields.ADDRESS_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.ADDRESS_PROPERTY_STORAGE, Address);
            }
            if (IsLoaded(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_STORAGE, EstablishedDate);
            }
            if (IsLoaded(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_STORAGE, StudentCapacity);
            }
            if (IsLoaded(School.__TeaqlFields.ACTIVE_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.ACTIVE_PROPERTY_STORAGE, Active);
            }
            if (IsLoaded(School.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, CreateTime);
            }
            if (IsLoaded(School.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, UpdateTime);
            }
            if (IsLoaded(School.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(School.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("School", Id ?? _ledgerId);
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
        private School() { _creationState = EntityCreationState.Hydrated; }
        internal static School TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new School();
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
                public long? SchoolType { get; set; }
                public string? Name { get; set; }
                public string? Address { get; set; }
                public DateTime? EstablishedDate { get; set; }
                public long? StudentCapacity { get; set; }
                public bool? Active { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public Platform? PlatformEntity { get; set; }
                public SchoolType? SchoolTypeEntity { get; set; }

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
        internal static School TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public School MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public School MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public School AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public School MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static School Refer(long id)
        {
            var entity = new School();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static School FromRecord(Record record)
        {
            var entity = new School().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue(School.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, out var platformValue)
                        || record.TryGetValue(School.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, out platformValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER);
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
                    if (record.TryGetValue(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER, out var schoolTypeValue)
                        || record.TryGetValue(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_STORAGE, out schoolTypeValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER);
                        if (schoolTypeValue.Raw is Record schoolTypeRow)
                        {
                            entity.SchoolTypeEntity = global::Generated.Models.SchoolType.FromRecord(schoolTypeRow);
                            entity.SchoolType = entity.SchoolTypeEntity.Id;
                            entity.MarkLoaded("SchoolTypeEntity");
                        }
                        else if (schoolTypeValue.Raw is IEnumerable<Record> schoolTypeRows)
                        {
                            foreach (var row in schoolTypeRows)
                            {
                                entity.SchoolTypeEntity = global::Generated.Models.SchoolType.FromRecord(row);
                                entity.SchoolType = entity.SchoolTypeEntity.Id;
                                entity.MarkLoaded("SchoolTypeEntity");
                                break;
                            }
                        }
                        else if (schoolTypeValue.Raw == null)
                            entity.MarkLoaded("SchoolTypeEntity");
                        else if (schoolTypeValue.Raw != null)
                            entity.SchoolType = Convert.ToInt64(schoolTypeValue.Raw);
                    }
                    if (record.TryGetValue(School.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.NAME_PROPERTY_MEMBER);
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue(School.__TeaqlFields.ADDRESS_PROPERTY_STORAGE, out var addressValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.ADDRESS_PROPERTY_MEMBER);
                        if (addressValue.Raw != null)
                            entity.Address = Convert.ToString(addressValue.Raw);
                    }
                    if (record.TryGetValue(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_STORAGE, out var establishedDateValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_MEMBER);
                        if (establishedDateValue.Raw != null)
                            entity.EstablishedDate = TeaqlDateTime(establishedDateValue);
                    }
                    if (record.TryGetValue(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_STORAGE, out var studentCapacityValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_MEMBER);
                        if (studentCapacityValue.Raw != null)
                            entity.StudentCapacity = Convert.ToInt64(studentCapacityValue.Raw);
                    }
                    if (record.TryGetValue(School.__TeaqlFields.ACTIVE_PROPERTY_STORAGE, out var activeValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.ACTIVE_PROPERTY_MEMBER);
                        if (activeValue.Raw != null)
                            entity.Active = Convert.ToBoolean(activeValue.Raw);
                    }
                    if (record.TryGetValue(School.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, out var createTimeValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER);
                        if (createTimeValue.Raw != null)
                            entity.CreateTime = TeaqlDateTime(createTimeValue);
                    }
                    if (record.TryGetValue(School.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, out var updateTimeValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER);
                        if (updateTimeValue.Raw != null)
                            entity.UpdateTime = TeaqlDateTime(updateTimeValue);
                    }
                    if (record.TryGetValue(School.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(School.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                School.__TeaqlFields.ID_PROPERTY_QUERY, School.__TeaqlFields.PLATFORM_PROPERTY_QUERY, School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY, School.__TeaqlFields.NAME_PROPERTY_QUERY, School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, School.__TeaqlFields.VERSION_PROPERTY_QUERY,
                School.__TeaqlFields.ID_PROPERTY_MEMBER, School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER, School.__TeaqlFields.NAME_PROPERTY_MEMBER, School.__TeaqlFields.ADDRESS_PROPERTY_MEMBER, School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_MEMBER, School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_MEMBER, School.__TeaqlFields.ACTIVE_PROPERTY_MEMBER, School.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER, School.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER, School.__TeaqlFields.VERSION_PROPERTY_MEMBER,
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static School FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<School> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("School");
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
            graph.DiscardRestoredFields("School", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(School.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.PLATFORM_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.NAME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.NAME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.ADDRESS_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.ADDRESS_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.ACTIVE_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.ACTIVE_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.CREATE_TIME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.UPDATE_TIME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(School.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property(School.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            var teaqlOriginalSchoolType = this.SchoolType;
            var teaqlOriginalName = this.Name;
            var teaqlOriginalAddress = this.Address;
            var teaqlOriginalEstablishedDate = this.EstablishedDate;
            var teaqlOriginalStudentCapacity = this.StudentCapacity;
            var teaqlOriginalActive = this.Active;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalUpdateTime = this.UpdateTime;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Platform = teaqlOriginalPlatform;
                this.SchoolType = teaqlOriginalSchoolType;
                this.Name = teaqlOriginalName;
                this.Address = teaqlOriginalAddress;
                this.EstablishedDate = teaqlOriginalEstablishedDate;
                this.StudentCapacity = teaqlOriginalStudentCapacity;
                this.Active = teaqlOriginalActive;
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
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<School> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("School",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("School",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for School");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Platform = saved.Platform;
            this.SchoolType = saved.SchoolType;
            this.Name = saved.Name;
            this.Address = saved.Address;
            this.EstablishedDate = saved.EstablishedDate;
            this.StudentCapacity = saved.StudentCapacity;
            this.Active = saved.Active;
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
                        if (IsLoaded(School.__TeaqlFields.ID_PROPERTY_MEMBER)) record[School.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)) record[School.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = TeaqlValue(Platform);
                        if (IsLoaded(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER)) record[School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_STORAGE] = TeaqlValue(SchoolType);
                        if (IsLoaded(School.__TeaqlFields.NAME_PROPERTY_MEMBER)) record[School.__TeaqlFields.NAME_PROPERTY_STORAGE] = TeaqlValue(Name);
                        if (IsLoaded(School.__TeaqlFields.ADDRESS_PROPERTY_MEMBER)) record[School.__TeaqlFields.ADDRESS_PROPERTY_STORAGE] = TeaqlValue(Address);
                        if (IsLoaded(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_MEMBER)) record[School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_STORAGE] = TeaqlValue(EstablishedDate);
                        if (IsLoaded(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_MEMBER)) record[School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_STORAGE] = TeaqlValue(StudentCapacity);
                        if (IsLoaded(School.__TeaqlFields.ACTIVE_PROPERTY_MEMBER)) record[School.__TeaqlFields.ACTIVE_PROPERTY_STORAGE] = TeaqlValue(Active);
                        if (IsLoaded(School.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER)) record[School.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = TeaqlValue(CreateTime);
                        if (IsLoaded(School.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER)) record[School.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = TeaqlValue(UpdateTime);
                        if (IsLoaded(School.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[School.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(School.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, out var platformValue))
                        this.Platform = platformValue.Raw == null ? null : Convert.ToInt64(platformValue.Raw);

                    if (record.TryGetValue(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_STORAGE, out var schoolTypeValue))
                        this.SchoolType = schoolTypeValue.Raw == null ? null : Convert.ToInt64(schoolTypeValue.Raw);

                    if (record.TryGetValue(School.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                        this.Name = nameValue.Raw == null ? null : Convert.ToString(nameValue.Raw);

                    if (record.TryGetValue(School.__TeaqlFields.ADDRESS_PROPERTY_STORAGE, out var addressValue))
                        this.Address = addressValue.Raw == null ? null : Convert.ToString(addressValue.Raw);

                    if (record.TryGetValue(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_STORAGE, out var establishedDateValue))
                        this.EstablishedDate = establishedDateValue.Raw == null ? null : TeaqlDateTime(establishedDateValue);

                    if (record.TryGetValue(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_STORAGE, out var studentCapacityValue))
                        this.StudentCapacity = studentCapacityValue.Raw == null ? null : Convert.ToInt64(studentCapacityValue.Raw);

                    if (record.TryGetValue(School.__TeaqlFields.ACTIVE_PROPERTY_STORAGE, out var activeValue))
                        this.Active = activeValue.Raw == null ? null : Convert.ToBoolean(activeValue.Raw);

                    if (record.TryGetValue(School.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, out var createTimeValue))
                        this.CreateTime = createTimeValue.Raw == null ? null : TeaqlDateTime(createTimeValue);

                    if (record.TryGetValue(School.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, out var updateTimeValue))
                        this.UpdateTime = updateTimeValue.Raw == null ? null : TeaqlDateTime(updateTimeValue);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[School.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (Platform.HasValue) record[School.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(Platform.Value);

                    if (SchoolType.HasValue) record[School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_STORAGE] = new Value.I64Value(SchoolType.Value);

                    if (Name != null) record[School.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (Address != null) record[School.__TeaqlFields.ADDRESS_PROPERTY_STORAGE] = new Value.TextValue(Address);

                    if (EstablishedDate.HasValue) record[School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_STORAGE] = new Value.DateValue(EstablishedDate.Value);

                    if (StudentCapacity.HasValue) record[School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_STORAGE] = new Value.I64Value(StudentCapacity.Value);

                    if (Active.HasValue) record[School.__TeaqlFields.ACTIVE_PROPERTY_STORAGE] = new Value.BoolValue(Active.Value);

                    if (CreateTime.HasValue) record[School.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record[School.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record[School.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "School", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Platform.HasValue) record[School.__TeaqlFields.PLATFORM_PROPERTY_STORAGE] = new Value.I64Value(Platform.Value);

                    if (SchoolType.HasValue) record[School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_STORAGE] = new Value.I64Value(SchoolType.Value);

                    if (Name != null) record[School.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (Address != null) record[School.__TeaqlFields.ADDRESS_PROPERTY_STORAGE] = new Value.TextValue(Address);

                    if (EstablishedDate.HasValue) record[School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_STORAGE] = new Value.DateValue(EstablishedDate.Value);

                    if (StudentCapacity.HasValue) record[School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_STORAGE] = new Value.I64Value(StudentCapacity.Value);

                    if (Active.HasValue) record[School.__TeaqlFields.ACTIVE_PROPERTY_STORAGE] = new Value.BoolValue(Active.Value);

                    if (CreateTime.HasValue) record[School.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record[School.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record[School.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "School", 
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
                Entity = "School",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("School");
        }

                public School UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded(School.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdatePlatform(long? value)
                {
                    if (this.Platform != value) this.PlatformEntity = null;
                    this.Platform = value;
                    MarkLoaded(School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.PLATFORM_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdateSchoolType(long? value)
                {
                    if (this.SchoolType != value) this.SchoolTypeEntity = null;
                    this.SchoolType = value;
                    MarkLoaded(School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded(School.__TeaqlFields.NAME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.NAME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdateAddress(string value)
                {
                    this.Address = value;
                    MarkLoaded(School.__TeaqlFields.ADDRESS_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.ADDRESS_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdateEstablishedDate(DateTime? value)
                {
                    this.EstablishedDate = value;
                    MarkLoaded(School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdateStudentCapacity(long? value)
                {
                    this.StudentCapacity = value;
                    MarkLoaded(School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdateActive(bool? value)
                {
                    this.Active = value;
                    MarkLoaded(School.__TeaqlFields.ACTIVE_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.ACTIVE_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded(School.__TeaqlFields.CREATE_TIME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.CREATE_TIME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded(School.__TeaqlFields.UPDATE_TIME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.UPDATE_TIME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public School UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(School.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), School.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
                public School UpdatePlatform(Platform value)
                {
                    UpdatePlatform(value?.Id);
                    this.PlatformEntity = value;
                    return this;
                }


                public School UpdatePlatformId(long? value)
                {
                    return UpdatePlatform(value);
                }


                public School UpdateSchoolType(SchoolType value)
                {
                    UpdateSchoolType(value?.Id);
                    this.SchoolTypeEntity = value;
                    return this;
                }


                public School UpdateSchoolTypeId(long? value)
                {
                    return UpdateSchoolType(value);
                }
                public School UpdateSchoolTypeToPrimary()
                {
                    return UpdateSchoolType(1001);
                }

                public School UpdateSchoolTypeToSecondary()
                {
                    return UpdateSchoolType(1002);
                }


    }
}