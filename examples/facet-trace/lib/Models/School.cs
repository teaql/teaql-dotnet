using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class School : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("School", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField("id", global::TeaQL.Runtime.EntityJsonKind.Id, "id"),
            new global::TeaQL.Runtime.EntityJsonField("platform", global::TeaQL.Runtime.EntityJsonKind.Reference, "platform", () => global::Generated.Models.Platform._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("school_type", global::TeaQL.Runtime.EntityJsonKind.Reference, "school_type", () => global::Generated.Models.SchoolType._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField("name", global::TeaQL.Runtime.EntityJsonKind.Text, "name"),
            new global::TeaQL.Runtime.EntityJsonField("address", global::TeaQL.Runtime.EntityJsonKind.Text, "address"),
            new global::TeaQL.Runtime.EntityJsonField("established_date", global::TeaQL.Runtime.EntityJsonKind.Date, "established_date"),
            new global::TeaQL.Runtime.EntityJsonField("student_capacity", global::TeaQL.Runtime.EntityJsonKind.Integer, "student_capacity"),
            new global::TeaQL.Runtime.EntityJsonField("active", global::TeaQL.Runtime.EntityJsonKind.Boolean, "active"),
            new global::TeaQL.Runtime.EntityJsonField("create_time", global::TeaQL.Runtime.EntityJsonKind.DateTime, "create_time"),
            new global::TeaQL.Runtime.EntityJsonField("update_time", global::TeaQL.Runtime.EntityJsonKind.DateTime, "update_time"),
            new global::TeaQL.Runtime.EntityJsonField("version", global::TeaQL.Runtime.EntityJsonKind.Version, "version")}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static School FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
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
            if (IsLoaded("SchoolType"))
            {
                object? jsonSchoolType = IsLoaded("SchoolTypeEntity") && SchoolTypeEntity != null && SchoolTypeEntity.Id == SchoolType
                    ? SchoolTypeEntity : SchoolType;
                yield return new KeyValuePair<string, object?>("school_type", jsonSchoolType);
            }
            if (IsLoaded("Name"))
            {
                yield return new KeyValuePair<string, object?>("name", Name);
            }
            if (IsLoaded("Address"))
            {
                yield return new KeyValuePair<string, object?>("address", Address);
            }
            if (IsLoaded("EstablishedDate"))
            {
                yield return new KeyValuePair<string, object?>("established_date", EstablishedDate);
            }
            if (IsLoaded("StudentCapacity"))
            {
                yield return new KeyValuePair<string, object?>("student_capacity", StudentCapacity);
            }
            if (IsLoaded("Active"))
            {
                yield return new KeyValuePair<string, object?>("active", Active);
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
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("School", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root, bool hydration = false)
        {
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
        public School() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
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
        {
            var oldKey = TeaqlEntityKey();
            Id = value;
            MarkLoaded("Id");
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            _entityRoot.Set(TeaqlEntityKey(), "id", new Value.I64Value(value));
            _teaqlForceCreate = true;
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
                    if (record.TryGetValue("SchoolType", out var schoolTypeValue)
                        || record.TryGetValue("school_type", out schoolTypeValue))
                    {
                        entity.MarkLoaded("SchoolType");
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
                    if (record.TryGetValue("name", out var nameValue))
                    {
                        entity.MarkLoaded("Name");
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue("address", out var addressValue))
                    {
                        entity.MarkLoaded("Address");
                        if (addressValue.Raw != null)
                            entity.Address = Convert.ToString(addressValue.Raw);
                    }
                    if (record.TryGetValue("established_date", out var establishedDateValue))
                    {
                        entity.MarkLoaded("EstablishedDate");
                        if (establishedDateValue.Raw != null)
                            entity.EstablishedDate = TeaqlDateTime(establishedDateValue);
                    }
                    if (record.TryGetValue("student_capacity", out var studentCapacityValue))
                    {
                        entity.MarkLoaded("StudentCapacity");
                        if (studentCapacityValue.Raw != null)
                            entity.StudentCapacity = Convert.ToInt64(studentCapacityValue.Raw);
                    }
                    if (record.TryGetValue("active", out var activeValue))
                    {
                        entity.MarkLoaded("Active");
                        if (activeValue.Raw != null)
                            entity.Active = Convert.ToBoolean(activeValue.Raw);
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
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                "id", "platform", "school_type", "name", "address", "established_date", "student_capacity", "active", "create_time", "update_time", "version",
                "Id", "Platform", "SchoolType", "Name", "Address", "EstablishedDate", "StudentCapacity", "Active", "CreateTime", "UpdateTime", "Version",
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

        internal void TeaqlPrepareInputGraph(UserContext context, GraphMutationSession graph)
        {
            graph.DiscardRestoredFields("School", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = !Id.HasValue || _teaqlForceCreate;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Platform"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("platform"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("SchoolType"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("school_type"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Name"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("name"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Address"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("address"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("EstablishedDate"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("established_date"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("StudentCapacity"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("student_capacity"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Active"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("active"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("CreateTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("create_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("UpdateTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("update_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Version"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "School", Location = ObjectLocation.Property("version"), Message = "Mutation requires a fully loaded entity" } });
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
        }

        internal void TeaqlPreflightGraph(UserContext context, GraphMutationSession graph)
        {
            if (!Id.HasValue || _teaqlForceCreate || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey()))
                graph.PreflightPrepared(this);
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
            await global::System.Threading.Tasks.Task.CompletedTask;
        }

        internal async global::System.Threading.Tasks.Task<School> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            var creating = !this.Id.HasValue || _teaqlForceCreate;
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

        private LoadedScalarSnapshot TeaqlScalarSnapshot()
        {
            var record = new Record();
                        if (IsLoaded("Id")) record["id"] = TeaqlValue(Id);
                        if (IsLoaded("Platform")) record["platform"] = TeaqlValue(Platform);
                        if (IsLoaded("SchoolType")) record["school_type"] = TeaqlValue(SchoolType);
                        if (IsLoaded("Name")) record["name"] = TeaqlValue(Name);
                        if (IsLoaded("Address")) record["address"] = TeaqlValue(Address);
                        if (IsLoaded("EstablishedDate")) record["established_date"] = TeaqlValue(EstablishedDate);
                        if (IsLoaded("StudentCapacity")) record["student_capacity"] = TeaqlValue(StudentCapacity);
                        if (IsLoaded("Active")) record["active"] = TeaqlValue(Active);
                        if (IsLoaded("CreateTime")) record["create_time"] = TeaqlValue(CreateTime);
                        if (IsLoaded("UpdateTime")) record["update_time"] = TeaqlValue(UpdateTime);
                        if (IsLoaded("Version")) record["version"] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record);
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (SchoolType.HasValue) record["school_type"] = new Value.I64Value(SchoolType.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Address != null) record["address"] = new Value.TextValue(Address);

                    if (EstablishedDate.HasValue) record["established_date"] = new Value.DateValue(EstablishedDate.Value);

                    if (StudentCapacity.HasValue) record["student_capacity"] = new Value.I64Value(StudentCapacity.Value);

                    if (Active.HasValue) record["active"] = new Value.BoolValue(Active.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "School", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (SchoolType.HasValue) record["school_type"] = new Value.I64Value(SchoolType.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Address != null) record["address"] = new Value.TextValue(Address);

                    if (EstablishedDate.HasValue) record["established_date"] = new Value.DateValue(EstablishedDate.Value);

                    if (StudentCapacity.HasValue) record["student_capacity"] = new Value.I64Value(StudentCapacity.Value);

                    if (Active.HasValue) record["active"] = new Value.BoolValue(Active.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

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
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public School UpdatePlatform(long? value)
                {
                    if (this.Platform != value) this.PlatformEntity = null;
                    this.Platform = value;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
                    return this;
                }

                public School UpdateSchoolType(long? value)
                {
                    if (this.SchoolType != value) this.SchoolTypeEntity = null;
                    this.SchoolType = value;
                    MarkLoaded("SchoolType");
                    _entityRoot.Set(TeaqlEntityKey(), "school_type", TeaqlValue(value));
                    return this;
                }

                public School UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    _entityRoot.Set(TeaqlEntityKey(), "name", TeaqlValue(value));
                    return this;
                }

                public School UpdateAddress(string value)
                {
                    this.Address = value;
                    MarkLoaded("Address");
                    _entityRoot.Set(TeaqlEntityKey(), "address", TeaqlValue(value));
                    return this;
                }

                public School UpdateEstablishedDate(DateTime? value)
                {
                    this.EstablishedDate = value;
                    MarkLoaded("EstablishedDate");
                    _entityRoot.Set(TeaqlEntityKey(), "established_date", TeaqlValue(value));
                    return this;
                }

                public School UpdateStudentCapacity(long? value)
                {
                    this.StudentCapacity = value;
                    MarkLoaded("StudentCapacity");
                    _entityRoot.Set(TeaqlEntityKey(), "student_capacity", TeaqlValue(value));
                    return this;
                }

                public School UpdateActive(bool? value)
                {
                    this.Active = value;
                    MarkLoaded("Active");
                    _entityRoot.Set(TeaqlEntityKey(), "active", TeaqlValue(value));
                    return this;
                }

                public School UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "create_time", TeaqlValue(value));
                    return this;
                }

                public School UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "update_time", TeaqlValue(value));
                    return this;
                }

                public School UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
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