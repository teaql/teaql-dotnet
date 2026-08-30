using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{
    public class WorkItem
    {
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private EntityKey TeaqlEntityKey() => new EntityKey("WorkItem", Id ?? _ledgerId);
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; } }
        private static Value TeaqlValue(object value) => value switch
        {
            null => new Value.NullValue(),
            string v => new Value.TextValue(v),
            bool v => new Value.BoolValue(v),
            double v => new Value.F64Value(v),
            decimal v => new Value.DecimalValue(v),
            DateTime v => new Value.DateTimeValue(v),
            int v => new Value.I64Value(v),
            long v => new Value.I64Value(v),
            _ => new Value.ObjectValue(value)
        };
        public WorkItem() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long? Platform { get; set; }
        public long? Version { get; set; }
        public Platform PlatformEntity { get; set; }

        private string _comment;
        private bool _markedForDeletion;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
        }

        public WorkItem MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public WorkItem MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public WorkItem AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public WorkItem MarkForDeletion()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static WorkItem Refer(long id)
        {
            return new WorkItem { Id = id }.MarkLoadedOnly("Id");
        }

        public static WorkItem FromRecord(Record record)
        {
            var entity = new WorkItem().MarkLoadedOnly();
            if (record.TryGetValue("id", out var idValue))
            {
                entity.MarkLoaded("Id");
                if (idValue.Raw != null)
                    entity.Id = Convert.ToInt64(idValue.Raw);
            }
            if (record.TryGetValue("title", out var titleValue))
            {
                entity.MarkLoaded("Title");
                if (titleValue.Raw != null)
                    entity.Title = Convert.ToString(titleValue.Raw);
            }
            if (record.TryGetValue("description", out var descriptionValue))
            {
                entity.MarkLoaded("Description");
                if (descriptionValue.Raw != null)
                    entity.Description = Convert.ToString(descriptionValue.Raw);
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

        public async Task<WorkItem> SaveAsync(UserContext context)
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
            if (!creating && !_markedForDeletion)
            {
                ((UpdateCommand)cmd).Values = _entityRoot.Change(TeaqlEntityKey());
                if (Version.HasValue) ((UpdateCommand)cmd).Values["version"] = new Value.I64Value(Version.Value);
            }
            var req = new MutationRequest { Command = cmd, Comment = _comment, LedgerKey = TeaqlEntityKey() };
            var result = await context.DataService.MutateAsync(context, req);
            if (result is not MutationResult mutationResult || mutationResult.PersistedRecord == null)
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for WorkItem");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Title = saved.Title;
            this.Description = saved.Description;
            this.Platform = saved.Platform;
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

            if (Title != null) record["title"] = new Value.TextValue(Title);

            if (Description != null) record["description"] = new Value.TextValue(Description);

            if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

            if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "WorkItem", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
            if (Title != null) record["title"] = new Value.TextValue(Title);

            if (Description != null) record["description"] = new Value.TextValue(Description);

            if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

            if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand
            {
                Entity = "WorkItem",
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null,
                Values = record
            };
        }

        public DeleteCommand ToDeleteCommand()
        {
            if (!Id.HasValue || !Version.HasValue)
                throw new InvalidOperationException("Delete requires a loaded id and version");
            return new DeleteCommand
            {
                Entity = "WorkItem",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("WorkItem");
        }

        public WorkItem UpdateId(long? value)
        {
            this.Id = value;
            MarkLoaded("Id");
            _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
            return this;
        }

        public WorkItem UpdateTitle(string value)
        {
            this.Title = value;
            MarkLoaded("Title");
            _entityRoot.Set(TeaqlEntityKey(), "title", TeaqlValue(value));
            return this;
        }

        public WorkItem UpdateDescription(string value)
        {
            this.Description = value;
            MarkLoaded("Description");
            _entityRoot.Set(TeaqlEntityKey(), "description", TeaqlValue(value));
            return this;
        }

        public WorkItem UpdatePlatform(long? value)
        {
            this.Platform = value;
            MarkLoaded("Platform");
            _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
            return this;
        }

        public WorkItem UpdateVersion(long? value)
        {
            this.Version = value;
            MarkLoaded("Version");
            _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
            return this;
        }
        public WorkItem UpdatePlatform(Platform value)
        {
            this.Platform = value?.Id;
            MarkLoaded("Platform");
            _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(this.Platform));
            return this;
        }


        public WorkItem UpdatePlatformId(long? value)
        {
            this.Platform = value;
            MarkLoaded("Platform");
            _entityRoot.Set(TeaqlEntityKey(), "platform", TeaqlValue(value));
            return this;
        }

    }
}