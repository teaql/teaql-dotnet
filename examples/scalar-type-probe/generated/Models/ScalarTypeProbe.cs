using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class ScalarTypeProbe
    {
        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("ScalarTypeProbe", Id ?? _ledgerId);
        internal EntityRoot TeaqlMutationLedger => _entityRoot;
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; }  }
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
        public ScalarTypeProbe() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
                public long? Id { get; set; }
                public long? TypePlatform { get; set; }
                public string? RequiredString { get; set; }
                public string? OptionalString { get; set; }
                public string? LargeText { get; set; }
                public bool? RequiredBoolean { get; set; }
                public bool? OptionalBoolean { get; set; }
                public long? RequiredInteger { get; set; }
                public long? OptionalInteger { get; set; }
                public long? RequiredLong { get; set; }
                public long? OptionalLong { get; set; }
                public decimal? RequiredDecimal { get; set; }
                public decimal? OptionalDecimal { get; set; }
                public double? RequiredFloat { get; set; }
                public double? OptionalFloat { get; set; }
                public double? RequiredDouble { get; set; }
                public double? OptionalDouble { get; set; }
                public DateTime? RequiredDate { get; set; }
                public DateTime? OptionalDate { get; set; }
                public TimeSpan? RequiredTime { get; set; }
                public TimeSpan? OptionalTime { get; set; }
                public DateTime? RequiredDatetime { get; set; }
                public DateTime? OptionalDatetime { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public TypePlatform? TypePlatformEntity { get; set; }

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

        public ScalarTypeProbe MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public ScalarTypeProbe MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public ScalarTypeProbe AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public ScalarTypeProbe MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static ScalarTypeProbe Refer(long id)
        {
            return new ScalarTypeProbe { Id = id }.MarkLoadedOnly("Id");
        }

        public static ScalarTypeProbe FromRecord(Record record)
        {
            var entity = new ScalarTypeProbe().MarkLoadedOnly();
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("TypePlatform", out var typePlatformValue)
                        || record.TryGetValue("type_platform", out typePlatformValue))
                    {
                        entity.MarkLoaded("TypePlatform");
                        if (typePlatformValue.Raw is Record typePlatformRow)
                        {
                            entity.TypePlatformEntity = global::Generated.Models.TypePlatform.FromRecord(typePlatformRow);
                            entity.TypePlatform = entity.TypePlatformEntity.Id;
                            entity.MarkLoaded("TypePlatformEntity");
                        }
                        else if (typePlatformValue.Raw is IEnumerable<Record> typePlatformRows)
                        {
                            foreach (var row in typePlatformRows)
                            {
                                entity.TypePlatformEntity = global::Generated.Models.TypePlatform.FromRecord(row);
                                entity.TypePlatform = entity.TypePlatformEntity.Id;
                                entity.MarkLoaded("TypePlatformEntity");
                                break;
                            }
                        }
                        else if (typePlatformValue.Raw != null)
                            entity.TypePlatform = Convert.ToInt64(typePlatformValue.Raw);
                    }
                    if (record.TryGetValue("required_string", out var requiredStringValue))
                    {
                        entity.MarkLoaded("RequiredString");
                        if (requiredStringValue.Raw != null)
                            entity.RequiredString = Convert.ToString(requiredStringValue.Raw);
                    }
                    if (record.TryGetValue("optional_string", out var optionalStringValue))
                    {
                        entity.MarkLoaded("OptionalString");
                        if (optionalStringValue.Raw != null)
                            entity.OptionalString = Convert.ToString(optionalStringValue.Raw);
                    }
                    if (record.TryGetValue("large_text", out var largeTextValue))
                    {
                        entity.MarkLoaded("LargeText");
                        if (largeTextValue.Raw != null)
                            entity.LargeText = Convert.ToString(largeTextValue.Raw);
                    }
                    if (record.TryGetValue("required_boolean", out var requiredBooleanValue))
                    {
                        entity.MarkLoaded("RequiredBoolean");
                        if (requiredBooleanValue.Raw != null)
                            entity.RequiredBoolean = Convert.ToBoolean(requiredBooleanValue.Raw);
                    }
                    if (record.TryGetValue("optional_boolean", out var optionalBooleanValue))
                    {
                        entity.MarkLoaded("OptionalBoolean");
                        if (optionalBooleanValue.Raw != null)
                            entity.OptionalBoolean = Convert.ToBoolean(optionalBooleanValue.Raw);
                    }
                    if (record.TryGetValue("required_integer", out var requiredIntegerValue))
                    {
                        entity.MarkLoaded("RequiredInteger");
                        if (requiredIntegerValue.Raw != null)
                            entity.RequiredInteger = Convert.ToInt64(requiredIntegerValue.Raw);
                    }
                    if (record.TryGetValue("optional_integer", out var optionalIntegerValue))
                    {
                        entity.MarkLoaded("OptionalInteger");
                        if (optionalIntegerValue.Raw != null)
                            entity.OptionalInteger = Convert.ToInt64(optionalIntegerValue.Raw);
                    }
                    if (record.TryGetValue("required_long", out var requiredLongValue))
                    {
                        entity.MarkLoaded("RequiredLong");
                        if (requiredLongValue.Raw != null)
                            entity.RequiredLong = Convert.ToInt64(requiredLongValue.Raw);
                    }
                    if (record.TryGetValue("optional_long", out var optionalLongValue))
                    {
                        entity.MarkLoaded("OptionalLong");
                        if (optionalLongValue.Raw != null)
                            entity.OptionalLong = Convert.ToInt64(optionalLongValue.Raw);
                    }
                    if (record.TryGetValue("required_decimal", out var requiredDecimalValue))
                    {
                        entity.MarkLoaded("RequiredDecimal");
                        if (requiredDecimalValue.Raw != null)
                            entity.RequiredDecimal = Convert.ToDecimal(requiredDecimalValue.Raw);
                    }
                    if (record.TryGetValue("optional_decimal", out var optionalDecimalValue))
                    {
                        entity.MarkLoaded("OptionalDecimal");
                        if (optionalDecimalValue.Raw != null)
                            entity.OptionalDecimal = Convert.ToDecimal(optionalDecimalValue.Raw);
                    }
                    if (record.TryGetValue("required_float", out var requiredFloatValue))
                    {
                        entity.MarkLoaded("RequiredFloat");
                        if (requiredFloatValue.Raw != null)
                            entity.RequiredFloat = Convert.ToDouble(requiredFloatValue.Raw);
                    }
                    if (record.TryGetValue("optional_float", out var optionalFloatValue))
                    {
                        entity.MarkLoaded("OptionalFloat");
                        if (optionalFloatValue.Raw != null)
                            entity.OptionalFloat = Convert.ToDouble(optionalFloatValue.Raw);
                    }
                    if (record.TryGetValue("required_double", out var requiredDoubleValue))
                    {
                        entity.MarkLoaded("RequiredDouble");
                        if (requiredDoubleValue.Raw != null)
                            entity.RequiredDouble = Convert.ToDouble(requiredDoubleValue.Raw);
                    }
                    if (record.TryGetValue("optional_double", out var optionalDoubleValue))
                    {
                        entity.MarkLoaded("OptionalDouble");
                        if (optionalDoubleValue.Raw != null)
                            entity.OptionalDouble = Convert.ToDouble(optionalDoubleValue.Raw);
                    }
                    if (record.TryGetValue("required_date", out var requiredDateValue))
                    {
                        entity.MarkLoaded("RequiredDate");
                        if (requiredDateValue.Raw != null)
                            entity.RequiredDate = TeaqlDateTime(requiredDateValue);
                    }
                    if (record.TryGetValue("optional_date", out var optionalDateValue))
                    {
                        entity.MarkLoaded("OptionalDate");
                        if (optionalDateValue.Raw != null)
                            entity.OptionalDate = TeaqlDateTime(optionalDateValue);
                    }
                    if (record.TryGetValue("required_time", out var requiredTimeValue))
                    {
                        entity.MarkLoaded("RequiredTime");
                        if (requiredTimeValue.Raw != null)
                            entity.RequiredTime = requiredTimeValue.Raw is TimeSpan time ? time : TimeSpan.Parse(Convert.ToString(requiredTimeValue.Raw), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (record.TryGetValue("optional_time", out var optionalTimeValue))
                    {
                        entity.MarkLoaded("OptionalTime");
                        if (optionalTimeValue.Raw != null)
                            entity.OptionalTime = optionalTimeValue.Raw is TimeSpan time ? time : TimeSpan.Parse(Convert.ToString(optionalTimeValue.Raw), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (record.TryGetValue("required_datetime", out var requiredDatetimeValue))
                    {
                        entity.MarkLoaded("RequiredDatetime");
                        if (requiredDatetimeValue.Raw != null)
                            entity.RequiredDatetime = TeaqlDateTime(requiredDatetimeValue);
                    }
                    if (record.TryGetValue("optional_datetime", out var optionalDatetimeValue))
                    {
                        entity.MarkLoaded("OptionalDatetime");
                        if (optionalDatetimeValue.Raw != null)
                            entity.OptionalDatetime = TeaqlDateTime(optionalDatetimeValue);
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
                "id", "type_platform", "required_string", "optional_string", "large_text", "required_boolean", "optional_boolean", "required_integer", "optional_integer", "required_long", "optional_long", "required_decimal", "optional_decimal", "required_float", "optional_float", "required_double", "optional_double", "required_date", "optional_date", "required_time", "optional_time", "required_datetime", "optional_datetime", "create_time", "update_time", "version",
                "Id", "TypePlatform", "RequiredString", "OptionalString", "LargeText", "RequiredBoolean", "OptionalBoolean", "RequiredInteger", "OptionalInteger", "RequiredLong", "OptionalLong", "RequiredDecimal", "OptionalDecimal", "RequiredFloat", "OptionalFloat", "RequiredDouble", "OptionalDouble", "RequiredDate", "OptionalDate", "RequiredTime", "OptionalTime", "RequiredDatetime", "OptionalDatetime", "CreateTime", "UpdateTime", "Version",
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            return entity;
        }

        internal static ScalarTypeProbe FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root);
            return entity;
        }

        public async Task<ScalarTypeProbe> SaveAsync(UserContext context)
        {
            return await context.ExecuteGraphSaveAsync(_comment!, async graph =>
            {
                if (!Id.HasValue || _teaqlForceCreate)
                {
                }
                TeaqlPreflightGraph(context, graph);
                return await TeaqlSaveWithinGraphAsync(context, graph);
            });
        }

        internal void TeaqlPreflightGraph(UserContext context, GraphMutationSession graph)
        {
            var creating = !Id.HasValue || _teaqlForceCreate;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded("Id"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("id"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("TypePlatform"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("type_platform"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredString"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_string"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalString"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_string"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("LargeText"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("large_text"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredBoolean"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_boolean"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalBoolean"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_boolean"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredInteger"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_integer"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalInteger"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_integer"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredLong"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_long"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalLong"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_long"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredDecimal"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_decimal"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalDecimal"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_decimal"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredFloat"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_float"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalFloat"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_float"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredDouble"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_double"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalDouble"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_double"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredDate"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_date"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalDate"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_date"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("RequiredDatetime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("required_datetime"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("OptionalDatetime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("optional_datetime"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("CreateTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("create_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("UpdateTime"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("update_time"), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded("Version"))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("version"), Message = "Mutation requires a fully loaded entity" } });
            }
            var command = _markedForDeletion ? (object)ToDeleteCommand()
                : creating ? (object)ToInsertCommand() : (object)ToUpdateCommand();
            if (!creating && !_markedForDeletion)
            {
                ((UpdateCommand)command).Values = _entityRoot.Change(TeaqlEntityKey());
                if (Version.HasValue) ((UpdateCommand)command).Values["version"] = new Value.I64Value(Version.Value);
            }
            graph.Preflight(TeaqlMutationRequest(command, string.IsNullOrWhiteSpace(_comment) ? graph.Intent.Comment : _comment!));
            }
        }

        internal async Task<ScalarTypeProbe> TeaqlSaveWithinGraphAsync(UserContext context, GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            var scope = graph.Scope("ScalarTypeProbe", Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            if (Id.HasValue && !_teaqlForceCreate && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                return this;
            }
            var teaqlOriginalKey = TeaqlEntityKey();
            var teaqlOriginalLedgerId = _ledgerId;
            var teaqlOriginalMarkedForDeletion = _markedForDeletion;
            var teaqlOriginalForceCreate = _teaqlForceCreate;
            var teaqlOriginalFullyLoaded = _fullyLoaded;
            var teaqlOriginalLoadedFields = new HashSet<string>(_loadedFields, StringComparer.OrdinalIgnoreCase);
            var teaqlOriginalId = this.Id;
            var teaqlOriginalTypePlatform = this.TypePlatform;
            var teaqlOriginalRequiredString = this.RequiredString;
            var teaqlOriginalOptionalString = this.OptionalString;
            var teaqlOriginalLargeText = this.LargeText;
            var teaqlOriginalRequiredBoolean = this.RequiredBoolean;
            var teaqlOriginalOptionalBoolean = this.OptionalBoolean;
            var teaqlOriginalRequiredInteger = this.RequiredInteger;
            var teaqlOriginalOptionalInteger = this.OptionalInteger;
            var teaqlOriginalRequiredLong = this.RequiredLong;
            var teaqlOriginalOptionalLong = this.OptionalLong;
            var teaqlOriginalRequiredDecimal = this.RequiredDecimal;
            var teaqlOriginalOptionalDecimal = this.OptionalDecimal;
            var teaqlOriginalRequiredFloat = this.RequiredFloat;
            var teaqlOriginalOptionalFloat = this.OptionalFloat;
            var teaqlOriginalRequiredDouble = this.RequiredDouble;
            var teaqlOriginalOptionalDouble = this.OptionalDouble;
            var teaqlOriginalRequiredDate = this.RequiredDate;
            var teaqlOriginalOptionalDate = this.OptionalDate;
            var teaqlOriginalRequiredTime = this.RequiredTime;
            var teaqlOriginalOptionalTime = this.OptionalTime;
            var teaqlOriginalRequiredDatetime = this.RequiredDatetime;
            var teaqlOriginalOptionalDatetime = this.OptionalDatetime;
            var teaqlOriginalCreateTime = this.CreateTime;
            var teaqlOriginalUpdateTime = this.UpdateTime;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.TypePlatform = teaqlOriginalTypePlatform;
                this.RequiredString = teaqlOriginalRequiredString;
                this.OptionalString = teaqlOriginalOptionalString;
                this.LargeText = teaqlOriginalLargeText;
                this.RequiredBoolean = teaqlOriginalRequiredBoolean;
                this.OptionalBoolean = teaqlOriginalOptionalBoolean;
                this.RequiredInteger = teaqlOriginalRequiredInteger;
                this.OptionalInteger = teaqlOriginalOptionalInteger;
                this.RequiredLong = teaqlOriginalRequiredLong;
                this.OptionalLong = teaqlOriginalOptionalLong;
                this.RequiredDecimal = teaqlOriginalRequiredDecimal;
                this.OptionalDecimal = teaqlOriginalOptionalDecimal;
                this.RequiredFloat = teaqlOriginalRequiredFloat;
                this.OptionalFloat = teaqlOriginalOptionalFloat;
                this.RequiredDouble = teaqlOriginalRequiredDouble;
                this.OptionalDouble = teaqlOriginalOptionalDouble;
                this.RequiredDate = teaqlOriginalRequiredDate;
                this.OptionalDate = teaqlOriginalOptionalDate;
                this.RequiredTime = teaqlOriginalRequiredTime;
                this.OptionalTime = teaqlOriginalOptionalTime;
                this.RequiredDatetime = teaqlOriginalRequiredDatetime;
                this.OptionalDatetime = teaqlOriginalOptionalDatetime;
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
            var req = TeaqlMutationRequest(cmd, scope.Node.Detail!);
            var mutationResult = await graph.MutateAsync(req, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for ScalarTypeProbe");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.TypePlatform = saved.TypePlatform;
            this.RequiredString = saved.RequiredString;
            this.OptionalString = saved.OptionalString;
            this.LargeText = saved.LargeText;
            this.RequiredBoolean = saved.RequiredBoolean;
            this.OptionalBoolean = saved.OptionalBoolean;
            this.RequiredInteger = saved.RequiredInteger;
            this.OptionalInteger = saved.OptionalInteger;
            this.RequiredLong = saved.RequiredLong;
            this.OptionalLong = saved.OptionalLong;
            this.RequiredDecimal = saved.RequiredDecimal;
            this.OptionalDecimal = saved.OptionalDecimal;
            this.RequiredFloat = saved.RequiredFloat;
            this.OptionalFloat = saved.OptionalFloat;
            this.RequiredDouble = saved.RequiredDouble;
            this.OptionalDouble = saved.OptionalDouble;
            this.RequiredDate = saved.RequiredDate;
            this.OptionalDate = saved.OptionalDate;
            this.RequiredTime = saved.RequiredTime;
            this.OptionalTime = saved.OptionalTime;
            this.RequiredDatetime = saved.RequiredDatetime;
            this.OptionalDatetime = saved.OptionalDatetime;
            this.CreateTime = saved.CreateTime;
            this.UpdateTime = saved.UpdateTime;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _teaqlForceCreate = false;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            return saved;
        }

        private MutationRequest TeaqlMutationRequest(object command, string comment) => command switch
        {
            InsertCommand insert => MutationRequest.Create(insert, comment, TeaqlEntityKey(), _entityRoot),
            UpdateCommand update => MutationRequest.Create(update, comment, TeaqlEntityKey(), _entityRoot),
            DeleteCommand delete => MutationRequest.Create(delete, comment, TeaqlEntityKey(), _entityRoot),
            _ => throw new InvalidOperationException("Unsupported mutation command")
        };

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (TypePlatform.HasValue) record["type_platform"] = new Value.I64Value(TypePlatform.Value);

                    if (RequiredString != null) record["required_string"] = new Value.TextValue(RequiredString);

                    if (OptionalString != null) record["optional_string"] = new Value.TextValue(OptionalString);

                    if (LargeText != null) record["large_text"] = new Value.TextValue(LargeText);

                    if (RequiredBoolean.HasValue) record["required_boolean"] = new Value.BoolValue(RequiredBoolean.Value);

                    if (OptionalBoolean.HasValue) record["optional_boolean"] = new Value.BoolValue(OptionalBoolean.Value);

                    if (RequiredInteger.HasValue) record["required_integer"] = new Value.I64Value(RequiredInteger.Value);

                    if (OptionalInteger.HasValue) record["optional_integer"] = new Value.I64Value(OptionalInteger.Value);

                    if (RequiredLong.HasValue) record["required_long"] = new Value.I64Value(RequiredLong.Value);

                    if (OptionalLong.HasValue) record["optional_long"] = new Value.I64Value(OptionalLong.Value);

                    if (RequiredDecimal.HasValue) record["required_decimal"] = new Value.DecimalValue(RequiredDecimal.Value);

                    if (OptionalDecimal.HasValue) record["optional_decimal"] = new Value.DecimalValue(OptionalDecimal.Value);

                    if (RequiredFloat.HasValue) record["required_float"] = new Value.F64Value(RequiredFloat.Value);

                    if (OptionalFloat.HasValue) record["optional_float"] = new Value.F64Value(OptionalFloat.Value);

                    if (RequiredDouble.HasValue) record["required_double"] = new Value.F64Value(RequiredDouble.Value);

                    if (OptionalDouble.HasValue) record["optional_double"] = new Value.F64Value(OptionalDouble.Value);

                    if (RequiredDate.HasValue) record["required_date"] = new Value.DateValue(RequiredDate.Value);

                    if (OptionalDate.HasValue) record["optional_date"] = new Value.DateValue(OptionalDate.Value);

                    if (RequiredTime.HasValue) record["required_time"] = new Value.TimeValue(RequiredTime.Value);

                    if (OptionalTime.HasValue) record["optional_time"] = new Value.TimeValue(OptionalTime.Value);

                    if (RequiredDatetime.HasValue) record["required_datetime"] = new Value.TimestampValue(new DateTimeOffset(RequiredDatetime.Value).ToUnixTimeMilliseconds());

                    if (OptionalDatetime.HasValue) record["optional_datetime"] = new Value.TimestampValue(new DateTimeOffset(OptionalDatetime.Value).ToUnixTimeMilliseconds());

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "ScalarTypeProbe", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (TypePlatform.HasValue) record["type_platform"] = new Value.I64Value(TypePlatform.Value);

                    if (RequiredString != null) record["required_string"] = new Value.TextValue(RequiredString);

                    if (OptionalString != null) record["optional_string"] = new Value.TextValue(OptionalString);

                    if (LargeText != null) record["large_text"] = new Value.TextValue(LargeText);

                    if (RequiredBoolean.HasValue) record["required_boolean"] = new Value.BoolValue(RequiredBoolean.Value);

                    if (OptionalBoolean.HasValue) record["optional_boolean"] = new Value.BoolValue(OptionalBoolean.Value);

                    if (RequiredInteger.HasValue) record["required_integer"] = new Value.I64Value(RequiredInteger.Value);

                    if (OptionalInteger.HasValue) record["optional_integer"] = new Value.I64Value(OptionalInteger.Value);

                    if (RequiredLong.HasValue) record["required_long"] = new Value.I64Value(RequiredLong.Value);

                    if (OptionalLong.HasValue) record["optional_long"] = new Value.I64Value(OptionalLong.Value);

                    if (RequiredDecimal.HasValue) record["required_decimal"] = new Value.DecimalValue(RequiredDecimal.Value);

                    if (OptionalDecimal.HasValue) record["optional_decimal"] = new Value.DecimalValue(OptionalDecimal.Value);

                    if (RequiredFloat.HasValue) record["required_float"] = new Value.F64Value(RequiredFloat.Value);

                    if (OptionalFloat.HasValue) record["optional_float"] = new Value.F64Value(OptionalFloat.Value);

                    if (RequiredDouble.HasValue) record["required_double"] = new Value.F64Value(RequiredDouble.Value);

                    if (OptionalDouble.HasValue) record["optional_double"] = new Value.F64Value(OptionalDouble.Value);

                    if (RequiredDate.HasValue) record["required_date"] = new Value.DateValue(RequiredDate.Value);

                    if (OptionalDate.HasValue) record["optional_date"] = new Value.DateValue(OptionalDate.Value);

                    if (RequiredTime.HasValue) record["required_time"] = new Value.TimeValue(RequiredTime.Value);

                    if (OptionalTime.HasValue) record["optional_time"] = new Value.TimeValue(OptionalTime.Value);

                    if (RequiredDatetime.HasValue) record["required_datetime"] = new Value.TimestampValue(new DateTimeOffset(RequiredDatetime.Value).ToUnixTimeMilliseconds());

                    if (OptionalDatetime.HasValue) record["optional_datetime"] = new Value.TimestampValue(new DateTimeOffset(OptionalDatetime.Value).ToUnixTimeMilliseconds());

                    if (CreateTime.HasValue) record["create_time"] = new Value.TimestampValue(new DateTimeOffset(CreateTime.Value).ToUnixTimeMilliseconds());

                    if (UpdateTime.HasValue) record["update_time"] = new Value.TimestampValue(new DateTimeOffset(UpdateTime.Value).ToUnixTimeMilliseconds());

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "ScalarTypeProbe", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : throw new InvalidOperationException("Update requires a loaded id"),
                ExpectedVersionValue = this.Version,
                Values = record 
            };
        }

        public DeleteCommand ToDeleteCommand()
        {
            if (!Id.HasValue || !Version.HasValue)
                throw new InvalidOperationException("Delete requires a loaded id and version");
            return new DeleteCommand {
                Entity = "ScalarTypeProbe",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("ScalarTypeProbe");
        }

                public ScalarTypeProbe UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateTypePlatform(long? value)
                {
                    this.TypePlatform = value;
                    MarkLoaded("TypePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "type_platform", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredString(string value)
                {
                    this.RequiredString = value;
                    MarkLoaded("RequiredString");
                    _entityRoot.Set(TeaqlEntityKey(), "required_string", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalString(string value)
                {
                    this.OptionalString = value;
                    MarkLoaded("OptionalString");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_string", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateLargeText(string value)
                {
                    this.LargeText = value;
                    MarkLoaded("LargeText");
                    _entityRoot.Set(TeaqlEntityKey(), "large_text", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredBoolean(bool? value)
                {
                    this.RequiredBoolean = value;
                    MarkLoaded("RequiredBoolean");
                    _entityRoot.Set(TeaqlEntityKey(), "required_boolean", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalBoolean(bool? value)
                {
                    this.OptionalBoolean = value;
                    MarkLoaded("OptionalBoolean");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_boolean", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredInteger(long? value)
                {
                    this.RequiredInteger = value;
                    MarkLoaded("RequiredInteger");
                    _entityRoot.Set(TeaqlEntityKey(), "required_integer", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalInteger(long? value)
                {
                    this.OptionalInteger = value;
                    MarkLoaded("OptionalInteger");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_integer", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredLong(long? value)
                {
                    this.RequiredLong = value;
                    MarkLoaded("RequiredLong");
                    _entityRoot.Set(TeaqlEntityKey(), "required_long", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalLong(long? value)
                {
                    this.OptionalLong = value;
                    MarkLoaded("OptionalLong");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_long", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredDecimal(decimal? value)
                {
                    this.RequiredDecimal = value;
                    MarkLoaded("RequiredDecimal");
                    _entityRoot.Set(TeaqlEntityKey(), "required_decimal", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalDecimal(decimal? value)
                {
                    this.OptionalDecimal = value;
                    MarkLoaded("OptionalDecimal");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_decimal", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredFloat(double? value)
                {
                    this.RequiredFloat = value;
                    MarkLoaded("RequiredFloat");
                    _entityRoot.Set(TeaqlEntityKey(), "required_float", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalFloat(double? value)
                {
                    this.OptionalFloat = value;
                    MarkLoaded("OptionalFloat");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_float", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredDouble(double? value)
                {
                    this.RequiredDouble = value;
                    MarkLoaded("RequiredDouble");
                    _entityRoot.Set(TeaqlEntityKey(), "required_double", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalDouble(double? value)
                {
                    this.OptionalDouble = value;
                    MarkLoaded("OptionalDouble");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_double", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredDate(DateTime? value)
                {
                    this.RequiredDate = value;
                    MarkLoaded("RequiredDate");
                    _entityRoot.Set(TeaqlEntityKey(), "required_date", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalDate(DateTime? value)
                {
                    this.OptionalDate = value;
                    MarkLoaded("OptionalDate");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_date", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredTime(TimeSpan? value)
                {
                    this.RequiredTime = value;
                    MarkLoaded("RequiredTime");
                    _entityRoot.Set(TeaqlEntityKey(), "required_time", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalTime(TimeSpan? value)
                {
                    this.OptionalTime = value;
                    MarkLoaded("OptionalTime");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_time", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateRequiredDatetime(DateTime? value)
                {
                    this.RequiredDatetime = value;
                    MarkLoaded("RequiredDatetime");
                    _entityRoot.Set(TeaqlEntityKey(), "required_datetime", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateOptionalDatetime(DateTime? value)
                {
                    this.OptionalDatetime = value;
                    MarkLoaded("OptionalDatetime");
                    _entityRoot.Set(TeaqlEntityKey(), "optional_datetime", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "create_time", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    _entityRoot.Set(TeaqlEntityKey(), "update_time", TeaqlValue(value));
                    return this;
                }

                public ScalarTypeProbe UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
                    return this;
                }
                public ScalarTypeProbe UpdateTypePlatform(TypePlatform value)
                {
                    this.TypePlatform = value?.Id;
                    MarkLoaded("TypePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "type_platform", TeaqlValue(this.TypePlatform));
                    return this;
                }


                public ScalarTypeProbe UpdateTypePlatformId(long? value)
                {
                    this.TypePlatform = value;
                    MarkLoaded("TypePlatform");
                    _entityRoot.Set(TeaqlEntityKey(), "type_platform", TeaqlValue(value));
                    return this;
                }

    }
}