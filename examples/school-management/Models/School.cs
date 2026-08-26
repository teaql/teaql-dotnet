using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{
    public class School
    {
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private EntityKey TeaqlEntityKey() => new EntityKey("School", Id ?? _ledgerId);
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; }  }
        private static Value TeaqlValue(object value) => value switch {
            null => new Value.NullValue(), string v => new Value.TextValue(v), bool v => new Value.BoolValue(v),
            double v => new Value.F64Value(v), decimal v => new Value.DecimalValue(v), DateTime v => new Value.DateTimeValue(v),
            int v => new Value.I64Value(v), long v => new Value.I64Value(v), _ => new Value.ObjectValue(value)
        };
        public School() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Id { get; set; }
                public long? Platform { get; set; }
                public long? SchoolType { get; set; }
                public string Name { get; set; }
                public string Address { get; set; }
                public DateTime? EstablishedDate { get; set; }
                public long? StudentCapacity { get; set; }
                public long? Active { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public Platform PlatformEntity { get; set; }
                public SchoolType SchoolTypeEntity { get; set; }

        private string _comment;
        private bool _markedForDeletion;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
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
            _comment = comment;
            return this;
        }

        public School MarkAsDeleted()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static School Refer(long id)
        {
            return new School { Id = id }.MarkLoadedOnly("Id");
        }

        public static School FromRecord(Record record)
        {
            var entity = new School().MarkLoadedOnly();
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
                        if (platformValue.Raw is IEnumerable<Record> platformRows)
                        {
                            foreach (var row in platformRows)
                            {
                                entity.PlatformEntity = global::Generated.Models.Platform.FromRecord(row);
                                entity.Platform = entity.PlatformEntity.Id;
                                entity.MarkLoaded("PlatformEntity");
                                break;
                            }
                        }
                        else if (platformValue.Raw != null)
                            entity.Platform = Convert.ToInt64(platformValue.Raw);
                    }
                    if (record.TryGetValue("SchoolType", out var schoolTypeValue)
                        || record.TryGetValue("school_type", out schoolTypeValue))
                    {
                        entity.MarkLoaded("SchoolType");
                        if (schoolTypeValue.Raw is IEnumerable<Record> schoolTypeRows)
                        {
                            foreach (var row in schoolTypeRows)
                            {
                                entity.SchoolTypeEntity = global::Generated.Models.SchoolType.FromRecord(row);
                                entity.SchoolType = entity.SchoolTypeEntity.Id;
                                entity.MarkLoaded("SchoolTypeEntity");
                                break;
                            }
                        }
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
                            entity.EstablishedDate = Convert.ToDateTime(establishedDateValue.Raw);
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
                            entity.Active = Convert.ToInt64(activeValue.Raw);
                    }
                    if (record.TryGetValue("create_time", out var createTimeValue))
                    {
                        entity.MarkLoaded("CreateTime");
                        if (createTimeValue.Raw != null)
                            entity.CreateTime = Convert.ToDateTime(createTimeValue.Raw);
                    }
                    if (record.TryGetValue("update_time", out var updateTimeValue))
                    {
                        entity.MarkLoaded("UpdateTime");
                        if (updateTimeValue.Raw != null)
                            entity.UpdateTime = Convert.ToDateTime(updateTimeValue.Raw);
                    }
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            return entity;
        }

        public async Task<School> SaveAsync(UserContext context)
        {
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new Exception("Security audit failure: AuditAs() must be called before SaveAsync()");
            }
            AttachRoot(context.EntityRoot);

            var creating = !this.Id.HasValue;
            if (_markedForDeletion && creating)
                throw new InvalidOperationException("Cannot delete an entity without an id");
            var cmd = _markedForDeletion ? (object)ToDeleteCommand()
                : creating ? (object)ToInsertCommand()
                : (object)ToUpdateCommand();
            if (!creating && !_markedForDeletion) {
                ((UpdateCommand)cmd).Values = _entityRoot.Change(TeaqlEntityKey());
                if (Version.HasValue) ((UpdateCommand)cmd).Values["version"] = new Value.I64Value(Version.Value);
            }
            var req = new MutationRequest { Command = cmd, Comment = _comment, LedgerKey = TeaqlEntityKey() };
            var result = await context.DataService.MutateAsync(context, req);
            if (result is not MutationResult mutationResult || mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for School");
            var saved = FromRecord(mutationResult.PersistedRecord);
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
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            _entityRoot.ClearEntity(TeaqlEntityKey());
            if (Version.HasValue) _entityRoot.SetOriginalVersion(TeaqlEntityKey(), Version.Value);
            return saved;
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

                    if (Active.HasValue) record["active"] = new Value.I64Value(Active.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

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

                    if (Active.HasValue) record["active"] = new Value.I64Value(Active.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "School", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
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
                Version = new Value.I64Value(Version.Value)
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
                    this.Platform = value;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
                    return this;
                }

                public School UpdateSchoolType(long? value)
                {
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

                public School UpdateActive(long? value)
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
                    this.Platform = value?.Id;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(this.Platform));
                    return this;
                }


                public School UpdatePlatformId(long? value)
                {
                    this.Platform = value;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
                    return this;
                }


                public School UpdateSchoolType(SchoolType value)
                {
                    this.SchoolType = value?.Id;
                    MarkLoaded("SchoolType");
                    _entityRoot.Set(TeaqlEntityKey(), "school_type", TeaqlValue(this.SchoolType));
                    return this;
                }


                public School UpdateSchoolTypeId(long? value)
                {
                    this.SchoolType = value;
                    MarkLoaded("SchoolType");
                    _entityRoot.Set(TeaqlEntityKey(), "school_type", TeaqlValue(value));
                    return this;
                }
                public School UpdateSchoolTypeToPrimary()
                {
                    this.SchoolType = 1001;
                    MarkLoaded("SchoolType");
                    return this;
                }


    }
}