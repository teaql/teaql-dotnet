using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{

    public class Entry : global::TeaQL.Runtime.IEntityJsonDocument
    {
        internal static class __TeaqlFields
        {
            internal const string ID_PROPERTY_MEMBER = "Id";
            internal const string ID_PROPERTY_QUERY = "id";
            internal const string ID_PROPERTY_STORAGE = "id";
            internal const string ID_PROPERTY_MODEL = "id";
            internal const string CATALOG_PROPERTY_MEMBER = "Catalog";
            internal const string CATALOG_PROPERTY_QUERY = "catalog";
            internal const string CATALOG_PROPERTY_STORAGE = "catalog";
            internal const string CATALOG_PROPERTY_MODEL = "catalog";
            internal const string NAME_PROPERTY_MEMBER = "Name";
            internal const string NAME_PROPERTY_QUERY = "name";
            internal const string NAME_PROPERTY_STORAGE = "name";
            internal const string NAME_PROPERTY_MODEL = "name";
            internal const string BORN_AT_PROPERTY_MEMBER = "BornAt";
            internal const string BORN_AT_PROPERTY_QUERY = "born_at";
            internal const string BORN_AT_PROPERTY_STORAGE = "born_at";
            internal const string BORN_AT_PROPERTY_MODEL = "born_at";
            internal const string TOUCHED_AT_PROPERTY_MEMBER = "TouchedAt";
            internal const string TOUCHED_AT_PROPERTY_QUERY = "touched_at";
            internal const string TOUCHED_AT_PROPERTY_STORAGE = "touched_at";
            internal const string TOUCHED_AT_PROPERTY_MODEL = "touched_at";
            internal const string SOURCE_IP_PROPERTY_MEMBER = "SourceIp";
            internal const string SOURCE_IP_PROPERTY_QUERY = "source_ip";
            internal const string SOURCE_IP_PROPERTY_STORAGE = "source_ip";
            internal const string SOURCE_IP_PROPERTY_MODEL = "source_ip";
            internal const string LAST_IP_PROPERTY_MEMBER = "LastIp";
            internal const string LAST_IP_PROPERTY_QUERY = "last_ip";
            internal const string LAST_IP_PROPERTY_STORAGE = "last_ip";
            internal const string LAST_IP_PROPERTY_MODEL = "last_ip";
            internal const string OPENED_BY_PROPERTY_MEMBER = "OpenedBy";
            internal const string OPENED_BY_PROPERTY_QUERY = "opened_by";
            internal const string OPENED_BY_PROPERTY_STORAGE = "opened_by";
            internal const string OPENED_BY_PROPERTY_MODEL = "opened_by";
            internal const string CHANGED_BY_PROPERTY_MEMBER = "ChangedBy";
            internal const string CHANGED_BY_PROPERTY_QUERY = "changed_by";
            internal const string CHANGED_BY_PROPERTY_STORAGE = "changed_by";
            internal const string CHANGED_BY_PROPERTY_MODEL = "changed_by";
            internal const string GUARDED_TOKEN_PROPERTY_MEMBER = "GuardedToken";
            internal const string GUARDED_TOKEN_PROPERTY_QUERY = "guarded_token";
            internal const string GUARDED_TOKEN_PROPERTY_STORAGE = "guarded_token";
            internal const string GUARDED_TOKEN_PROPERTY_MODEL = "guarded_token";
            internal const string VERSION_PROPERTY_MEMBER = "Version";
            internal const string VERSION_PROPERTY_QUERY = "version";
            internal const string VERSION_PROPERTY_STORAGE = "version";
            internal const string VERSION_PROPERTY_MODEL = "version";
        }

        internal static global::TeaQL.Runtime.EntityJsonSchema _teaqlJsonSchema { get; } = new global::TeaQL.Runtime.EntityJsonSchema("Entry", new global::TeaQL.Runtime.EntityJsonField[] {
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.ID_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Id, Entry.__TeaqlFields.ID_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.CATALOG_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Reference, Entry.__TeaqlFields.CATALOG_PROPERTY_STORAGE, () => global::Generated.Models.Catalog._teaqlJsonSchema),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.NAME_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, Entry.__TeaqlFields.NAME_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.BORN_AT_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, Entry.__TeaqlFields.BORN_AT_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.DateTime, Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, Entry.__TeaqlFields.SOURCE_IP_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.LAST_IP_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, Entry.__TeaqlFields.LAST_IP_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.OPENED_BY_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, Entry.__TeaqlFields.OPENED_BY_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, Entry.__TeaqlFields.CHANGED_BY_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Text, Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_STORAGE),
            new global::TeaQL.Runtime.EntityJsonField(Entry.__TeaqlFields.VERSION_PROPERTY_MODEL, global::TeaQL.Runtime.EntityJsonKind.Version, Entry.__TeaqlFields.VERSION_PROPERTY_STORAGE)}, global::TeaQL.Runtime.JsonFieldNamingProfiles.Parse("camelCase"));

        global::TeaQL.Runtime.EntityJsonSchema global::TeaQL.Runtime.IEntityJsonDocument.EntityJsonSchema => _teaqlJsonSchema;
        DynamicPropertySnapshot global::TeaQL.Runtime.IEntityJsonDocument.QueryProperties => DynamicProperties;

        internal bool TeaqlHasPending() => _entityRoot.HasPending(TeaqlEntityKey());

        public string ToJson(global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) => _teaqlJsonSchema.Serialize(this, profile);

        public static Entry FromJson(string payload, global::TeaQL.Runtime.JsonFieldNamingProfile? profile = null) =>
            FromRecord(_teaqlJsonSchema.Parse(payload, profile));

        IEnumerable<KeyValuePair<string, object?>> global::TeaQL.Runtime.IEntityJsonDocument.LoadedJsonFields()
        {
            if (IsLoaded(Entry.__TeaqlFields.ID_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.ID_PROPERTY_STORAGE, Id);
            }
            if (IsLoaded(Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER))
            {
                object? jsonCatalog = IsLoaded("CatalogEntity") && CatalogEntity != null && CatalogEntity.Id == Catalog
                    ? CatalogEntity : Catalog;
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.CATALOG_PROPERTY_STORAGE, jsonCatalog);
            }
            if (IsLoaded(Entry.__TeaqlFields.NAME_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.NAME_PROPERTY_STORAGE, Name);
            }
            if (IsLoaded(Entry.__TeaqlFields.BORN_AT_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.BORN_AT_PROPERTY_STORAGE, BornAt);
            }
            if (IsLoaded(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_STORAGE, TouchedAt);
            }
            if (IsLoaded(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_STORAGE, SourceIp);
            }
            if (IsLoaded(Entry.__TeaqlFields.LAST_IP_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.LAST_IP_PROPERTY_STORAGE, LastIp);
            }
            if (IsLoaded(Entry.__TeaqlFields.OPENED_BY_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.OPENED_BY_PROPERTY_STORAGE, OpenedBy);
            }
            if (IsLoaded(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_STORAGE, ChangedBy);
            }
            if (IsLoaded(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_STORAGE, GuardedToken);
            }
            if (IsLoaded(Entry.__TeaqlFields.VERSION_PROPERTY_MEMBER))
            {
                yield return new KeyValuePair<string, object?>(Entry.__TeaqlFields.VERSION_PROPERTY_STORAGE, Version);
            }
        }

        public DynamicPropertySnapshot DynamicProperties { get; private set; } = DynamicPropertySnapshot.Empty;
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private LoadedScalarSnapshot _loadedSnapshot = new LoadedScalarSnapshot(new Record());
        private EntityCreationState _creationState;
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private bool _teaqlForceCreate;
        private EntityKey TeaqlEntityKey() => new EntityKey("Entry", Id ?? _ledgerId);
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
        private Entry() { _creationState = EntityCreationState.Hydrated; }
        internal static Entry TeaqlNewWithReservedId(long value)
        {
            EntityCreation.RequireReserved(EntityCreationState.Reserved, value);
            var entity = new Entry();
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
                public long? Catalog { get; set; }
                public string? Name { get; set; }
                public DateTime? BornAt { get; set; }
                public DateTime? TouchedAt { get; set; }
                public string? SourceIp { get; set; }
                public string? LastIp { get; set; }
                public string? OpenedBy { get; set; }
                public string? ChangedBy { get; set; }
                public string? GuardedToken { get; set; }
                public long? Version { get; set; }
                public Catalog? CatalogEntity { get; set; }

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
        internal static Entry TeaqlNewWithFixedId(long value)
        {
            return TeaqlNewWithReservedId(value);
        }

        public Entry MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public Entry MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public Entry AuditAs(string comment)
        {
            _comment = new MutationIntent(comment).Comment;
            return this;
        }

        public Entry MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static Entry Refer(long id)
        {
            var entity = new Entry();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
            entity.Id = id;
            return entity.MarkLoadedOnly("Id");
        }

        public static Entry FromRecord(Record record)
        {
            var entity = new Entry().MarkLoadedOnly();
            entity._entityRoot.ClearEntity(entity.TeaqlEntityKey());
                    if (record.TryGetValue(Entry.__TeaqlFields.ID_PROPERTY_STORAGE, out var idValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.ID_PROPERTY_MEMBER);
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER, out var catalogValue)
                        || record.TryGetValue(Entry.__TeaqlFields.CATALOG_PROPERTY_STORAGE, out catalogValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER);
                        if (catalogValue.Raw is Record catalogRow)
                        {
                            entity.CatalogEntity = global::Generated.Models.Catalog.FromRecord(catalogRow);
                            entity.Catalog = entity.CatalogEntity.Id;
                            entity.MarkLoaded("CatalogEntity");
                        }
                        else if (catalogValue.Raw is IEnumerable<Record> catalogRows)
                        {
                            foreach (var row in catalogRows)
                            {
                                entity.CatalogEntity = global::Generated.Models.Catalog.FromRecord(row);
                                entity.Catalog = entity.CatalogEntity.Id;
                                entity.MarkLoaded("CatalogEntity");
                                break;
                            }
                        }
                        else if (catalogValue.Raw == null)
                            entity.MarkLoaded("CatalogEntity");
                        else if (catalogValue.Raw != null)
                            entity.Catalog = Convert.ToInt64(catalogValue.Raw);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.NAME_PROPERTY_MEMBER);
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.BORN_AT_PROPERTY_STORAGE, out var bornAtValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.BORN_AT_PROPERTY_MEMBER);
                        if (bornAtValue.Raw != null)
                            entity.BornAt = TeaqlDateTime(bornAtValue);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_STORAGE, out var touchedAtValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_MEMBER);
                        if (touchedAtValue.Raw != null)
                            entity.TouchedAt = TeaqlDateTime(touchedAtValue);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_STORAGE, out var sourceIpValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_MEMBER);
                        if (sourceIpValue.Raw != null)
                            entity.SourceIp = Convert.ToString(sourceIpValue.Raw);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.LAST_IP_PROPERTY_STORAGE, out var lastIpValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.LAST_IP_PROPERTY_MEMBER);
                        if (lastIpValue.Raw != null)
                            entity.LastIp = Convert.ToString(lastIpValue.Raw);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.OPENED_BY_PROPERTY_STORAGE, out var openedByValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.OPENED_BY_PROPERTY_MEMBER);
                        if (openedByValue.Raw != null)
                            entity.OpenedBy = Convert.ToString(openedByValue.Raw);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_STORAGE, out var changedByValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_MEMBER);
                        if (changedByValue.Raw != null)
                            entity.ChangedBy = Convert.ToString(changedByValue.Raw);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_STORAGE, out var guardedTokenValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_MEMBER);
                        if (guardedTokenValue.Raw != null)
                            entity.GuardedToken = Convert.ToString(guardedTokenValue.Raw);
                    }
                    if (record.TryGetValue(Entry.__TeaqlFields.VERSION_PROPERTY_STORAGE, out var versionValue))
                    {
                        entity.MarkLoaded(Entry.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
            entity.DynamicProperties = DynamicPropertySnapshot.Capture(record, new[] {
                Entry.__TeaqlFields.ID_PROPERTY_QUERY, Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY, Entry.__TeaqlFields.NAME_PROPERTY_QUERY, Entry.__TeaqlFields.BORN_AT_PROPERTY_QUERY, Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_QUERY, Entry.__TeaqlFields.SOURCE_IP_PROPERTY_QUERY, Entry.__TeaqlFields.LAST_IP_PROPERTY_QUERY, Entry.__TeaqlFields.OPENED_BY_PROPERTY_QUERY, Entry.__TeaqlFields.CHANGED_BY_PROPERTY_QUERY, Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_QUERY, Entry.__TeaqlFields.VERSION_PROPERTY_QUERY,
                Entry.__TeaqlFields.ID_PROPERTY_MEMBER, Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER, Entry.__TeaqlFields.NAME_PROPERTY_MEMBER, Entry.__TeaqlFields.BORN_AT_PROPERTY_MEMBER, Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_MEMBER, Entry.__TeaqlFields.SOURCE_IP_PROPERTY_MEMBER, Entry.__TeaqlFields.LAST_IP_PROPERTY_MEMBER, Entry.__TeaqlFields.OPENED_BY_PROPERTY_MEMBER, Entry.__TeaqlFields.CHANGED_BY_PROPERTY_MEMBER, Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_MEMBER, Entry.__TeaqlFields.VERSION_PROPERTY_MEMBER,
            });
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            entity._loadedSnapshot = entity.TeaqlScalarSnapshot();
            return entity;
        }

        internal static Entry FromRecord(Record record, EntityRoot root)
        {
            var entity = FromRecord(record);
            entity.AttachRoot(root, hydration: true);
            return entity;
        }

        public async global::System.Threading.Tasks.Task<Entry> SaveAsync(UserContext context)
        {
            var intent = new MutationIntent(_comment);
            if (TeaqlIsCancelledNew()) throw new InvalidOperationException("Cannot save a cancelled new root");
            TeaqlValidateCreationGraph(new HashSet<object>(ReferenceEqualityComparer.Instance));
            return await context.ExecuteGraphSaveAsync(intent.Comment, async graph =>
            {
                graph.DeclareRootEntity("Entry");
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
            graph.DiscardRestoredFields("Entry", _entityRoot, TeaqlEntityKey(), _loadedSnapshot);
            var creating = TeaqlCreationIntent;
            var changed = creating || _markedForDeletion || _entityRoot.HasPending(TeaqlEntityKey());
            EntityCreation.RequireMutation(_creationState, Id, creating, changed, _loadedSnapshot);
            if (changed)
            {
            if (!creating && !_markedForDeletion)
            {
                if (!IsLoaded(Entry.__TeaqlFields.ID_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.ID_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.CATALOG_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.NAME_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.NAME_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.BORN_AT_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.BORN_AT_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.LAST_IP_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.LAST_IP_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.OPENED_BY_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.OPENED_BY_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
                if (!IsLoaded(Entry.__TeaqlFields.VERSION_PROPERTY_MEMBER))
                    throw new CheckException(new[] { new CheckResult { RuleId = "invalid_type", EntityType = "Entry", Location = ObjectLocation.Property(Entry.__TeaqlFields.VERSION_PROPERTY_MODEL), Message = "Mutation requires a fully loaded entity" } });
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
            var teaqlOriginalCatalog = this.Catalog;
            var teaqlOriginalName = this.Name;
            var teaqlOriginalBornAt = this.BornAt;
            var teaqlOriginalTouchedAt = this.TouchedAt;
            var teaqlOriginalSourceIp = this.SourceIp;
            var teaqlOriginalLastIp = this.LastIp;
            var teaqlOriginalOpenedBy = this.OpenedBy;
            var teaqlOriginalChangedBy = this.ChangedBy;
            var teaqlOriginalGuardedToken = this.GuardedToken;
            var teaqlOriginalVersion = this.Version;
            graph.AfterRollback(() =>
            {
                var currentKey = TeaqlEntityKey();
                this.Id = teaqlOriginalId;
                this.Catalog = teaqlOriginalCatalog;
                this.Name = teaqlOriginalName;
                this.BornAt = teaqlOriginalBornAt;
                this.TouchedAt = teaqlOriginalTouchedAt;
                this.SourceIp = teaqlOriginalSourceIp;
                this.LastIp = teaqlOriginalLastIp;
                this.OpenedBy = teaqlOriginalOpenedBy;
                this.ChangedBy = teaqlOriginalChangedBy;
                this.GuardedToken = teaqlOriginalGuardedToken;
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

        internal async global::System.Threading.Tasks.Task<Entry> TeaqlSaveWithinGraphAsync(UserContext context,
            GraphMutationSession graph, MutationTraceScope? parentScope = null)
        {
            if (!graph.VisitEntity(this, global::TeaQL.Runtime.GraphMutationTraversal.Write)) return this;
            if (TeaqlIsCancelledNew()) return this;
            var creating = TeaqlCreationIntent;
            if (!creating && !_markedForDeletion && !_entityRoot.HasPending(TeaqlEntityKey()))
            {
                var cleanScope = graph.Scope("Entry",
                    Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
                await TeaqlSaveChildrenAsync(context, graph, cleanScope);
                return this;
            }
            var scope = graph.Scope("Entry",
                Id.HasValue ? checked((ulong)Id.Value) : null, _comment, parentScope);
            var mutationResult = await graph.MutatePreparedAsync(this, scope);
            if (mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for Entry");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var committedSnapshot = saved._loadedSnapshot;
            graph.AfterCommit(() => _loadedSnapshot = committedSnapshot);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Catalog = saved.Catalog;
            this.Name = saved.Name;
            this.BornAt = saved.BornAt;
            this.TouchedAt = saved.TouchedAt;
            this.SourceIp = saved.SourceIp;
            this.LastIp = saved.LastIp;
            this.OpenedBy = saved.OpenedBy;
            this.ChangedBy = saved.ChangedBy;
            this.GuardedToken = saved.GuardedToken;
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
                        if (IsLoaded(Entry.__TeaqlFields.ID_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.ID_PROPERTY_STORAGE] = TeaqlValue(Id);
                        if (IsLoaded(Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.CATALOG_PROPERTY_STORAGE] = TeaqlValue(Catalog);
                        if (IsLoaded(Entry.__TeaqlFields.NAME_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.NAME_PROPERTY_STORAGE] = TeaqlValue(Name);
                        if (IsLoaded(Entry.__TeaqlFields.BORN_AT_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.BORN_AT_PROPERTY_STORAGE] = TeaqlValue(BornAt);
                        if (IsLoaded(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_STORAGE] = TeaqlValue(TouchedAt);
                        if (IsLoaded(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.SOURCE_IP_PROPERTY_STORAGE] = TeaqlValue(SourceIp);
                        if (IsLoaded(Entry.__TeaqlFields.LAST_IP_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.LAST_IP_PROPERTY_STORAGE] = TeaqlValue(LastIp);
                        if (IsLoaded(Entry.__TeaqlFields.OPENED_BY_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.OPENED_BY_PROPERTY_STORAGE] = TeaqlValue(OpenedBy);
                        if (IsLoaded(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.CHANGED_BY_PROPERTY_STORAGE] = TeaqlValue(ChangedBy);
                        if (IsLoaded(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_STORAGE] = TeaqlValue(GuardedToken);
                        if (IsLoaded(Entry.__TeaqlFields.VERSION_PROPERTY_MEMBER)) record[Entry.__TeaqlFields.VERSION_PROPERTY_STORAGE] = TeaqlValue(Version);
            return new LoadedScalarSnapshot(TeaqlEntityKey(), record, origin);
        }

        // Repair native scalar values cancelled against an owner/version-qualified baseline.
        // Do not change identity, loading state, or relations through serialization.
        private void TeaqlRestoreFixedValues(Record record)
        {
                    if (record.TryGetValue(Entry.__TeaqlFields.CATALOG_PROPERTY_STORAGE, out var catalogValue))
                        this.Catalog = catalogValue.Raw == null ? null : Convert.ToInt64(catalogValue.Raw);

                    if (record.TryGetValue(Entry.__TeaqlFields.NAME_PROPERTY_STORAGE, out var nameValue))
                        this.Name = nameValue.Raw == null ? null : Convert.ToString(nameValue.Raw);

                    if (record.TryGetValue(Entry.__TeaqlFields.BORN_AT_PROPERTY_STORAGE, out var bornAtValue))
                        this.BornAt = bornAtValue.Raw == null ? null : TeaqlDateTime(bornAtValue);

                    if (record.TryGetValue(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_STORAGE, out var touchedAtValue))
                        this.TouchedAt = touchedAtValue.Raw == null ? null : TeaqlDateTime(touchedAtValue);

                    if (record.TryGetValue(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_STORAGE, out var sourceIpValue))
                        this.SourceIp = sourceIpValue.Raw == null ? null : Convert.ToString(sourceIpValue.Raw);

                    if (record.TryGetValue(Entry.__TeaqlFields.LAST_IP_PROPERTY_STORAGE, out var lastIpValue))
                        this.LastIp = lastIpValue.Raw == null ? null : Convert.ToString(lastIpValue.Raw);

                    if (record.TryGetValue(Entry.__TeaqlFields.OPENED_BY_PROPERTY_STORAGE, out var openedByValue))
                        this.OpenedBy = openedByValue.Raw == null ? null : Convert.ToString(openedByValue.Raw);

                    if (record.TryGetValue(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_STORAGE, out var changedByValue))
                        this.ChangedBy = changedByValue.Raw == null ? null : Convert.ToString(changedByValue.Raw);

                    if (record.TryGetValue(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_STORAGE, out var guardedTokenValue))
                        this.GuardedToken = guardedTokenValue.Raw == null ? null : Convert.ToString(guardedTokenValue.Raw);


        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record[Entry.__TeaqlFields.ID_PROPERTY_STORAGE] = new Value.I64Value(Id.Value);

                    if (Catalog.HasValue) record[Entry.__TeaqlFields.CATALOG_PROPERTY_STORAGE] = new Value.I64Value(Catalog.Value);

                    if (Name != null) record[Entry.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (BornAt.HasValue) record[Entry.__TeaqlFields.BORN_AT_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(BornAt.Value).ToUnixTimeMilliseconds());

                    if (TouchedAt.HasValue) record[Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(TouchedAt.Value).ToUnixTimeMilliseconds());

                    if (SourceIp != null) record[Entry.__TeaqlFields.SOURCE_IP_PROPERTY_STORAGE] = new Value.TextValue(SourceIp);

                    if (LastIp != null) record[Entry.__TeaqlFields.LAST_IP_PROPERTY_STORAGE] = new Value.TextValue(LastIp);

                    if (OpenedBy != null) record[Entry.__TeaqlFields.OPENED_BY_PROPERTY_STORAGE] = new Value.TextValue(OpenedBy);

                    if (ChangedBy != null) record[Entry.__TeaqlFields.CHANGED_BY_PROPERTY_STORAGE] = new Value.TextValue(ChangedBy);

                    if (GuardedToken != null) record[Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_STORAGE] = new Value.TextValue(GuardedToken);

                    if (Version.HasValue) record[Entry.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "Entry", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Catalog.HasValue) record[Entry.__TeaqlFields.CATALOG_PROPERTY_STORAGE] = new Value.I64Value(Catalog.Value);

                    if (Name != null) record[Entry.__TeaqlFields.NAME_PROPERTY_STORAGE] = new Value.TextValue(Name);

                    if (BornAt.HasValue) record[Entry.__TeaqlFields.BORN_AT_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(BornAt.Value).ToUnixTimeMilliseconds());

                    if (TouchedAt.HasValue) record[Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_STORAGE] = new Value.TimestampValue(new DateTimeOffset(TouchedAt.Value).ToUnixTimeMilliseconds());

                    if (SourceIp != null) record[Entry.__TeaqlFields.SOURCE_IP_PROPERTY_STORAGE] = new Value.TextValue(SourceIp);

                    if (LastIp != null) record[Entry.__TeaqlFields.LAST_IP_PROPERTY_STORAGE] = new Value.TextValue(LastIp);

                    if (OpenedBy != null) record[Entry.__TeaqlFields.OPENED_BY_PROPERTY_STORAGE] = new Value.TextValue(OpenedBy);

                    if (ChangedBy != null) record[Entry.__TeaqlFields.CHANGED_BY_PROPERTY_STORAGE] = new Value.TextValue(ChangedBy);

                    if (GuardedToken != null) record[Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_STORAGE] = new Value.TextValue(GuardedToken);

                    if (Version.HasValue) record[Entry.__TeaqlFields.VERSION_PROPERTY_STORAGE] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "Entry", 
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
                Entity = "Entry",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(_entityRoot.OriginalVersion(TeaqlEntityKey()) ?? Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("Entry");
        }

                public Entry UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded(Entry.__TeaqlFields.ID_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.ID_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateCatalog(long? value)
                {
                    if (this.Catalog != value) this.CatalogEntity = null;
                    this.Catalog = value;
                    MarkLoaded(Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.CATALOG_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded(Entry.__TeaqlFields.NAME_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.NAME_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateBornAt(DateTime? value)
                {
                    this.BornAt = value;
                    MarkLoaded(Entry.__TeaqlFields.BORN_AT_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.BORN_AT_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateTouchedAt(DateTime? value)
                {
                    this.TouchedAt = value;
                    MarkLoaded(Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.TOUCHED_AT_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateSourceIp(string value)
                {
                    this.SourceIp = value;
                    MarkLoaded(Entry.__TeaqlFields.SOURCE_IP_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.SOURCE_IP_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateLastIp(string value)
                {
                    this.LastIp = value;
                    MarkLoaded(Entry.__TeaqlFields.LAST_IP_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.LAST_IP_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateOpenedBy(string value)
                {
                    this.OpenedBy = value;
                    MarkLoaded(Entry.__TeaqlFields.OPENED_BY_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.OPENED_BY_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateChangedBy(string value)
                {
                    this.ChangedBy = value;
                    MarkLoaded(Entry.__TeaqlFields.CHANGED_BY_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.CHANGED_BY_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateGuardedToken(string value)
                {
                    this.GuardedToken = value;
                    MarkLoaded(Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.GUARDED_TOKEN_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }

                public Entry UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded(Entry.__TeaqlFields.VERSION_PROPERTY_MEMBER);
                    _entityRoot.Set(TeaqlEntityKey(), Entry.__TeaqlFields.VERSION_PROPERTY_STORAGE, TeaqlValue(value));
                    return this;
                }
                public Entry UpdateCatalog(Catalog value)
                {
                    UpdateCatalog(value?.Id);
                    this.CatalogEntity = value;
                    return this;
                }


                public Entry UpdateCatalogId(long? value)
                {
                    return UpdateCatalog(value);
                }

    }
}