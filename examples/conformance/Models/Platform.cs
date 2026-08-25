using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace Generated.Models
{
    public class Platform
    {
        private static long _teaqlTemporaryId;
        private EntityRoot _entityRoot = new EntityRoot();
        private long _ledgerId = -Interlocked.Increment(ref _teaqlTemporaryId);
        private EntityKey TeaqlEntityKey() => new EntityKey("Platform", Id ?? _ledgerId);
        internal void AttachRoot(EntityRoot root) { if (!ReferenceEquals(root, _entityRoot)) { root.MergeFrom(_entityRoot); _entityRoot = root; } foreach (var child in WorkItemList) child.AttachRoot(root); }
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
        public Platform() { _entityRoot.MarkAsNew(TeaqlEntityKey()); }
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? Version { get; set; }
        public List<WorkItem> WorkItemList { get; } = new List<WorkItem>();

        private string _comment;
        private bool _markedForDeletion;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
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
            _comment = comment;
            return this;
        }

        public Platform MarkAsDeleted()
        {
            _markedForDeletion = true;
            _entityRoot.MarkAsDeleted(TeaqlEntityKey());
            return this;
        }

        public static Platform Refer(long id)
        {
            return new Platform { Id = id }.MarkLoadedOnly("Id");
        }

        public static Platform FromRecord(Record record)
        {
            var entity = new Platform().MarkLoadedOnly();
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
            if (record.TryGetValue("version", out var versionValue))
            {
                entity.MarkLoaded("Version");
                if (versionValue.Raw != null)
                    entity.Version = Convert.ToInt64(versionValue.Raw);
            }
            if (record.TryGetValue("WorkItemList", out var workItemListValue))
            {
                entity.MarkLoaded("WorkItemList");
                var rows = workItemListValue.Raw as IEnumerable<Record>;
                if (rows != null)
                    foreach (var row in rows) entity.WorkItemList.Add(global::Generated.Models.WorkItem.FromRecord(row));
            }
            entity._ledgerId = entity.Id ?? entity._ledgerId;
            entity._entityRoot.MarkAsPersisted(entity.TeaqlEntityKey());
            if (entity.Version.HasValue) entity._entityRoot.SetOriginalVersion(entity.TeaqlEntityKey(), entity.Version.Value);
            return entity;
        }

        public async Task<Platform> SaveAsync(UserContext context)
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
                throw new InvalidOperationException("Mutation provider did not return authoritative persisted state for Platform");
            var saved = FromRecord(mutationResult.PersistedRecord);
            var oldKey = TeaqlEntityKey();
            this.Id = saved.Id;
            this.Name = saved.Name;
            this.Version = saved.Version;
            _ledgerId = Id ?? _ledgerId;
            _entityRoot.Rekey(oldKey, TeaqlEntityKey());
            foreach (var child in WorkItemList)
            {
                child.AttachRoot(_entityRoot);
                child.UpdatePlatformId(Id);
                child.AuditAs(_comment);
                await child.SaveAsync(context);
            }
            _entityRoot.ClearEntity(TeaqlEntityKey());
            if (Version.HasValue) _entityRoot.SetOriginalVersion(TeaqlEntityKey(), Version.Value);
            return saved;
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
            if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

            if (Name != null) record["name"] = new Value.TextValue(Name);

            if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "Platform", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
            if (Name != null) record["name"] = new Value.TextValue(Name);

            if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand
            {
                Entity = "Platform",
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
                Entity = "Platform",
                Id = new Value.I64Value(Id.Value),
                Version = new Value.I64Value(Version.Value)
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("Platform");
        }

        public Platform UpdateId(long? value)
        {
            this.Id = value;
            MarkLoaded("Id");
            _entityRoot.Set(TeaqlEntityKey(), "id", TeaqlValue(value));
            return this;
        }

        public Platform UpdateName(string value)
        {
            this.Name = value;
            MarkLoaded("Name");
            _entityRoot.Set(TeaqlEntityKey(), "name", TeaqlValue(value));
            return this;
        }

        public Platform UpdateVersion(long? value)
        {
            this.Version = value;
            MarkLoaded("Version");
            _entityRoot.Set(TeaqlEntityKey(), "version", TeaqlValue(value));
            return this;
        }
    }
}