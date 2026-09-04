using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{
    public class SchoolType
    {
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("SchoolType", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; } foreach (var child in SchoolList) child.AttachRoot(root); }
        private static Value TeaqlValue(object value) => value switch {
            null => new Value.NullValue(), string v => new Value.TextValue(v), bool v => new Value.BoolValue(v),
            double v => new Value.F64Value(v), decimal v => new Value.DecimalValue(v), DateTime v => new Value.DateTimeValue(v), TimeSpan v => new Value.TimeValue(v),
            int v => new Value.I64Value(v), long v => new Value.I64Value(v), _ => new Value.ObjectValue(value)
        };
        public SchoolType() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Platform { get; set; }
                public long? Id { get; set; }
                public string Name { get; set; }
                public string Code { get; set; }
                public decimal? DisplayOrder { get; set; }
                public long? Version { get; set; }
                public Platform PlatformEntity { get; set; }
                public List<School> SchoolList { get; } = new List<School>();

        private string _comment;
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

        public SchoolType MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public SchoolType MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public SchoolType AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public SchoolType MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static SchoolType Refer(long id)
        {
            return new SchoolType { Id = id }.MarkLoadedOnly("Id");
        }

        public static SchoolType FromRecord(Record record)
        {
            var entity = new SchoolType().MarkLoadedOnly();
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
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("name", out var nameValue))
                    {
                        entity.MarkLoaded("Name");
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue("code", out var codeValue))
                    {
                        entity.MarkLoaded("Code");
                        if (codeValue.Raw != null)
                            entity.Code = Convert.ToString(codeValue.Raw);
                    }
                    if (record.TryGetValue("display_order", out var displayOrderValue))
                    {
                        entity.MarkLoaded("DisplayOrder");
                        if (displayOrderValue.Raw != null)
                            entity.DisplayOrder = Convert.ToDecimal(displayOrderValue.Raw);
                    }
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
                        if (record.TryGetValue("SchoolList", out var schoolListValue))
                        {
                            entity.MarkLoaded("SchoolList");
                            var rows = schoolListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.SchoolList.Add(global::Generated.Models.School.FromRecord(row));
                        }
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            return entity;
        }

        internal static SchoolType FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root);
            return entity;
        }

        public async Task<SchoolType> SaveAsync(UserContext context)
        {
            return await context.ExecuteGraphSaveAsync(async () =>
            {
                TeaqlPreflightGraph(context);
                return await TeaqlSaveWithinGraphAsync(context);
            });
        }

        internal void TeaqlPreflightGraph(UserContext context)
        {
            if (string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: AuditAs() must be called before SaveAsync()");
            var creating = !Id.HasValue || _teaqlForceCreate;
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Platform"))
                    throw new CheckException(new[] { new CheckResult("invalid_type", ObjectLocation.Property("platform"), Message: "Mutation requires a fully loaded entity") });
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult("invalid_type", ObjectLocation.Property("id"), Message: "Mutation requires a fully loaded entity") });
                if (!IsLoaded("Name"))
                    throw new CheckException(new[] { new CheckResult("invalid_type", ObjectLocation.Property("name"), Message: "Mutation requires a fully loaded entity") });
                if (!IsLoaded("Code"))
                    throw new CheckException(new[] { new CheckResult("invalid_type", ObjectLocation.Property("code"), Message: "Mutation requires a fully loaded entity") });
                if (!IsLoaded("DisplayOrder"))
                    throw new CheckException(new[] { new CheckResult("invalid_type", ObjectLocation.Property("display_order"), Message: "Mutation requires a fully loaded entity") });
                if (!IsLoaded("Version"))
                    throw new CheckException(new[] { new CheckResult("invalid_type", ObjectLocation.Property("version"), Message: "Mutation requires a fully loaded entity") });
            }
            var command = _markedForDeletion ? (object)ToDeleteCommand()
                : creating ? (object)ToInsertCommand() : (object)ToUpdateCommand();
            if (!creating && !_markedForDeletion)
            {
                ((UpdateCommand)command).Values = _entityRoot.Change(TeaqlEntityKey());
                if (Version.HasValue) ((UpdateCommand)command).Values["version"] = new Value.I64Value(Version.Value);
            }
            context.CheckAndFix(new MutationRequest { Command = command, Comment = _comment, LedgerKey = TeaqlEntityKey(), LedgerRoot = _entityRoot });
            for (var index = 0; index < SchoolList.Count; index++)
            {
                var child = SchoolList[index];
                child.AttachRoot(_entityRoot);
                child.UpdateSchoolTypeId(Id ?? _ledgerId);
                child.AuditAs(_comment);
                try { child.TeaqlPreflightGraph(context); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("school_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        violation with { Location = violation.Location.PrefixedBy(prefix) }).ToArray());
                }
            }
        }

        internal async Task<SchoolType> TeaqlSaveWithinGraphAsync(UserContext context)
        {
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
            var teaqlOriginalFullyLoaded = _fullyLoaded;
            var teaqlOriginalLoadedFields = new HashSet<string>(_loadedFields, StringComparer.OrdinalIgnoreCase);
            var teaqlOriginalPlatform = this.Platform;
            var teaqlOriginalId = this.Id;
            var teaqlOriginalName = this.Name;
            var teaqlOriginalCode = this.Code;
            var teaqlOriginalDisplayOrder = this.DisplayOrder;
            var teaqlOriginalVersion = this.Version;
            context.AfterGraphRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Platform = teaqlOriginalPlatform;
                this.Id = teaqlOriginalId;
                this.Name = teaqlOriginalName;
                this.Code = teaqlOriginalCode;
                this.DisplayOrder = teaqlOriginalDisplayOrder;
                this.Version = teaqlOriginalVersion;
                _ledgerId = teaqlOriginalLedgerId;
                _markedForDeletion = teaqlOriginalMarkedForDeletion;
                _teaqlForceCreate = teaqlOriginalForceCreate;
                _fullyLoaded = teaqlOriginalFullyLoaded;
                _loadedFields = teaqlOriginalLoadedFields;
                _entityRoot.Rekey(currentKey, teaqlOriginalKey);
            });
            context.AfterGraphCommit(() =>
            {
                _entityRoot.ClearEntity(TeaqlEntityKey());
                if (Version.HasValue) _entityRoot.SetOriginalVersion(TeaqlEntityKey(), Version.Value);
            });
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new Exception("Security audit failure: AuditAs() must be called before SaveAsync()");
            }
            var creating = !this.Id.HasValue || _teaqlForceCreate;
            if (_markedForDeletion && creating)
                throw new InvalidOperationException("Cannot delete an entity without an id");
            var cmd = _markedForDeletion ? (object)ToDeleteCommand()
                : creating ? (object)ToInsertCommand()
                : (object)ToUpdateCommand();
            if (!creating && !_markedForDeletion) {
                ((UpdateCommand)cmd).Values = _entityRoot.Change(TeaqlEntityKey());
                if (Version.HasValue) ((UpdateCommand)cmd).Values["version"] = new Value.I64Value(Version.Value);
            }
            var req = new MutationRequest { Command = cmd, Comment = _comment, LedgerKey = TeaqlEntityKey(), LedgerRoot = _entityRoot };
            var result = await context.DataService.MutateAsync(context, req);
            if (result is not MutationResult mutationResult || mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for SchoolType");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var oldKey = TeaqlEntityKey();
            this.Platform = saved.Platform;
            this.Id = saved.Id;
            this.Name = saved.Name;
            this.Code = saved.Code;
            this.DisplayOrder = saved.DisplayOrder;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _teaqlForceCreate = false;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            for (var index = 0; index < SchoolList.Count; index++)
            {
                var child = SchoolList[index];
                child.AttachRoot(_entityRoot);
                child.UpdateSchoolTypeId(Id);
                child.AuditAs(_comment);
                try { await child.TeaqlSaveWithinGraphAsync(context); }
                catch (CheckException error)
                {
                    var prefix = ObjectLocation.Property("school_list").Index(index);
                    throw new CheckException(error.Violations.Select(violation =>
                        violation with { Location = violation.Location.PrefixedBy(prefix) }).ToArray());
                }
            }
            return saved;
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Code != null) record["code"] = new Value.TextValue(Code);

                    if (DisplayOrder.HasValue) record["display_order"] = new Value.DecimalValue(DisplayOrder.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "SchoolType", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);


                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Code != null) record["code"] = new Value.TextValue(Code);

                    if (DisplayOrder.HasValue) record["display_order"] = new Value.DecimalValue(DisplayOrder.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "SchoolType", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public DeleteCommand ToDeleteCommand()
        {
            if (!Id.HasValue || !Version.HasValue)
                throw new InvalidOperationException("Delete requires a loaded id and version");
            return new DeleteCommand {
                Entity = "SchoolType",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("SchoolType");
        }

                public SchoolType UpdatePlatform(long? value)
                {
                    this.Platform = value;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
                    return this;
                }

                public SchoolType UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public SchoolType UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    _entityRoot.Set(TeaqlEntityKey(), "name", TeaqlValue(value));
                    return this;
                }

                public SchoolType UpdateCode(string value)
                {
                    this.Code = value;
                    MarkLoaded("Code");
                    _entityRoot.Set(TeaqlEntityKey(), "code", TeaqlValue(value));
                    return this;
                }

                public SchoolType UpdateDisplayOrder(decimal? value)
                {
                    this.DisplayOrder = value;
                    MarkLoaded("DisplayOrder");
                    _entityRoot.Set(TeaqlEntityKey(), "display_order", TeaqlValue(value));
                    return this;
                }

                public SchoolType UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public SchoolType UpdatePlatform(Platform value)
                {
                    this.Platform = value?.Id;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(this.Platform));
                    return this;
                }


                public SchoolType UpdatePlatformId(long? value)
                {
                    this.Platform = value;
                    MarkLoaded("Platform");
                    _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
                    return this;
                }

    }
}