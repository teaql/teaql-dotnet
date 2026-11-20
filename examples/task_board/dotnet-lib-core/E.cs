using System;
using System.Collections.Generic;
using TeaQL.Runtime;

namespace Generated
{
    public sealed class ValueExpression<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public bool HasValue
        {
            get
            {
                if (_notLoaded != null) throw _notLoaded.Terminal("HasValue");
                return _hasValue;
            }
        }

        public ValueExpression(T value, bool hasValue = true, TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _hasValue = hasValue;
            _notLoaded = notLoaded;
        }

        public T Eval()
        {
            if (_notLoaded != null) throw _notLoaded.Terminal("Eval()");
            return _value;
        }

        public T OrIfNull(T fallback)
        {
            if (_notLoaded != null) throw _notLoaded.Terminal("OrIfNull(<fallback>)");
            var value = _value;
            return _hasValue && value is not null ? value : fallback;
        }

        public static ValueExpression<T> Missing() => new(default!, false);
        public static ValueExpression<T> NotLoaded(TeaQLNotLoadedException error) => new(default!, false, error);
    }

    public sealed class PlatformExpression
    {
        private readonly Generated.Models.Platform? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PlatformExpression(
            Generated.Models.Platform? value,
            string root = "Platform(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Platform(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Platform? Eval()
        {
            if (_notLoaded != null) throw _notLoaded.Terminal("Eval()");
            return _value;
        }

        public ValueExpression<long?> Id()
        {
            var path = _path.Member("id", "Id");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Id"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "id"));
            return new ValueExpression<long?>(_value.Id);
        }

        public ValueExpression<string?> Name()
        {
            var path = _path.Member("name", "Name");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Name"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "name"));
            return new ValueExpression<string?>(_value.Name);
        }

        public ValueExpression<System.DateTime?> Founded()
        {
            var path = _path.Member("founded", "Founded");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("Founded"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "founded"));
            return new ValueExpression<System.DateTime?>(_value.Founded);
        }

        public ValueExpression<string?> UserEmail()
        {
            var path = _path.Member("user_email", "UserEmail");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("UserEmail"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "user_email"));
            return new ValueExpression<string?>(_value.UserEmail);
        }

        public ValueExpression<long?> Version()
        {
            var path = _path.Member("version", "Version");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "version"));
            return new ValueExpression<long?>(_value.Version);
        }



        public TaskStatusListExpression TaskStatusList()
        {
            var path = _path.Member("task_status_list", "TaskStatusList");
            if (_notLoaded != null) return new TaskStatusListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return TaskStatusListExpression.Missing(_root, path);
            if (!_value.IsLoaded("TaskStatusList"))
                return new TaskStatusListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "task_status_list"));
            return new TaskStatusListExpression(_value.TaskStatusList, _root, path);
        }

        public TaskListExpression TaskList()
        {
            var path = _path.Member("task_list", "TaskList");
            if (_notLoaded != null) return new TaskListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return TaskListExpression.Missing(_root, path);
            if (!_value.IsLoaded("TaskList"))
                return new TaskListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "task_list"));
            return new TaskListExpression(_value.TaskList, _root, path);
        }
    }

    public sealed class TaskStatusExpression
    {
        private readonly Generated.Models.TaskStatus? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TaskStatusExpression(
            Generated.Models.TaskStatus? value,
            string root = "TaskStatus(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.TaskStatus(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.TaskStatus? Eval()
        {
            if (_notLoaded != null) throw _notLoaded.Terminal("Eval()");
            return _value;
        }

        public ValueExpression<long?> Id()
        {
            var path = _path.Member("id", "Id");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Id"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "id"));
            return new ValueExpression<long?>(_value.Id);
        }

        public ValueExpression<string?> Name()
        {
            var path = _path.Member("name", "Name");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Name"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "name"));
            return new ValueExpression<string?>(_value.Name);
        }

        public ValueExpression<string?> Code()
        {
            var path = _path.Member("code", "Code");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Code"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "code"));
            return new ValueExpression<string?>(_value.Code);
        }

        public ValueExpression<string?> Color()
        {
            var path = _path.Member("color", "Color");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Color"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "color"));
            return new ValueExpression<string?>(_value.Color);
        }

        public ValueExpression<decimal?> DisplayOrder()
        {
            var path = _path.Member("display_order", "DisplayOrder");
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<decimal?>.Missing();
            if (!_value.IsLoaded("DisplayOrder"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "display_order"));
            return new ValueExpression<decimal?>(_value.DisplayOrder);
        }

        public ValueExpression<decimal?> Progress()
        {
            var path = _path.Member("progress", "Progress");
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<decimal?>.Missing();
            if (!_value.IsLoaded("Progress"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "progress"));
            return new ValueExpression<decimal?>(_value.Progress);
        }

        public ValueExpression<long?> Version()
        {
            var path = _path.Member("version", "Version");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "version"));
            return new ValueExpression<long?>(_value.Version);
        }

        public ValueExpression<long?> PlatformId()
        {
            var path = _path.Member("platform", "PlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Platform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "platform"));
            return new ValueExpression<long?>(_value.Platform);
        }

        public PlatformExpression Platform()
        {
            var path = _path.Member("platform", "Platform");
            if (_notLoaded != null) return new PlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new PlatformExpression(null, _root, path);
            if (!_value.IsLoaded("Platform"))
                return new PlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "platform"));
            if (_value.Platform is null) return new PlatformExpression(null, _root, path);
            if (!_value.IsLoaded("PlatformEntity") || _value.PlatformEntity is null
                || _value.PlatformEntity.Id != _value.Platform)
                return new PlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "platform"));
            return new PlatformExpression(_value.PlatformEntity, _root, path);
        }

        public TaskListExpression TaskList()
        {
            var path = _path.Member("task_list", "TaskList");
            if (_notLoaded != null) return new TaskListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return TaskListExpression.Missing(_root, path);
            if (!_value.IsLoaded("TaskList"))
                return new TaskListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "task_list"));
            return new TaskListExpression(_value.TaskList, _root, path);
        }
    }

    public sealed class TaskExpression
    {
        private readonly Generated.Models.Task? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TaskExpression(
            Generated.Models.Task? value,
            string root = "Task(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Task(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Task? Eval()
        {
            if (_notLoaded != null) throw _notLoaded.Terminal("Eval()");
            return _value;
        }

        public ValueExpression<long?> Id()
        {
            var path = _path.Member("id", "Id");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Id"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "id"));
            return new ValueExpression<long?>(_value.Id);
        }

        public ValueExpression<string?> Name()
        {
            var path = _path.Member("name", "Name");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Name"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "name"));
            return new ValueExpression<string?>(_value.Name);
        }

        public ValueExpression<long?> Version()
        {
            var path = _path.Member("version", "Version");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "version"));
            return new ValueExpression<long?>(_value.Version);
        }

        public ValueExpression<long?> StatusId()
        {
            var path = _path.Member("status", "StatusId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Status"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "status"));
            return new ValueExpression<long?>(_value.Status);
        }

        public ValueExpression<long?> PlatformId()
        {
            var path = _path.Member("platform", "PlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Platform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "platform"));
            return new ValueExpression<long?>(_value.Platform);
        }

        public TaskStatusExpression Status()
        {
            var path = _path.Member("status", "Status");
            if (_notLoaded != null) return new TaskStatusExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new TaskStatusExpression(null, _root, path);
            if (!_value.IsLoaded("Status"))
                return new TaskStatusExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "status"));
            if (_value.Status is null) return new TaskStatusExpression(null, _root, path);
            if (!_value.IsLoaded("StatusEntity") || _value.StatusEntity is null
                || _value.StatusEntity.Id != _value.Status)
                return new TaskStatusExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "status"));
            return new TaskStatusExpression(_value.StatusEntity, _root, path);
        }

        public PlatformExpression Platform()
        {
            var path = _path.Member("platform", "Platform");
            if (_notLoaded != null) return new PlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new PlatformExpression(null, _root, path);
            if (!_value.IsLoaded("Platform"))
                return new PlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "platform"));
            if (_value.Platform is null) return new PlatformExpression(null, _root, path);
            if (!_value.IsLoaded("PlatformEntity") || _value.PlatformEntity is null
                || _value.PlatformEntity.Id != _value.Platform)
                return new PlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "platform"));
            return new PlatformExpression(_value.PlatformEntity, _root, path);
        }

        public TaskExecutionLogListExpression TaskExecutionLogList()
        {
            var path = _path.Member("task_execution_log_list", "TaskExecutionLogList");
            if (_notLoaded != null) return new TaskExecutionLogListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return TaskExecutionLogListExpression.Missing(_root, path);
            if (!_value.IsLoaded("TaskExecutionLogList"))
                return new TaskExecutionLogListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "task_execution_log_list"));
            return new TaskExecutionLogListExpression(_value.TaskExecutionLogList, _root, path);
        }
    }

    public sealed class TaskExecutionLogExpression
    {
        private readonly Generated.Models.TaskExecutionLog? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TaskExecutionLogExpression(
            Generated.Models.TaskExecutionLog? value,
            string root = "TaskExecutionLog(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.TaskExecutionLog(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.TaskExecutionLog? Eval()
        {
            if (_notLoaded != null) throw _notLoaded.Terminal("Eval()");
            return _value;
        }

        public ValueExpression<long?> Id()
        {
            var path = _path.Member("id", "Id");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Id"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "id"));
            return new ValueExpression<long?>(_value.Id);
        }

        public ValueExpression<string?> Action()
        {
            var path = _path.Member("action", "Action");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Action"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "action"));
            return new ValueExpression<string?>(_value.Action);
        }

        public ValueExpression<string?> Detail()
        {
            var path = _path.Member("detail", "Detail");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Detail"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "detail"));
            return new ValueExpression<string?>(_value.Detail);
        }

        public ValueExpression<long?> Version()
        {
            var path = _path.Member("version", "Version");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "version"));
            return new ValueExpression<long?>(_value.Version);
        }

        public ValueExpression<long?> TaskId()
        {
            var path = _path.Member("task", "TaskId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Task"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "task"));
            return new ValueExpression<long?>(_value.Task);
        }

        public TaskExpression Task()
        {
            var path = _path.Member("task", "Task");
            if (_notLoaded != null) return new TaskExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new TaskExpression(null, _root, path);
            if (!_value.IsLoaded("Task"))
                return new TaskExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "task"));
            if (_value.Task is null) return new TaskExpression(null, _root, path);
            if (!_value.IsLoaded("TaskEntity") || _value.TaskEntity is null
                || _value.TaskEntity.Id != _value.Task)
                return new TaskExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "task"));
            return new TaskExpression(_value.TaskEntity, _root, path);
        }

    }

    public sealed class PlatformListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Platform> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PlatformListExpression(
            IReadOnlyList<Generated.Models.Platform>? items,
            string? root = "Platform(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Platform>();
            _root = root ?? "Platform(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static PlatformListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Platform>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public PlatformExpression First() => Item(0, true);

        public PlatformExpression Get(int index)
        {
            return Item(index, false);
        }

        private PlatformExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new PlatformExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new PlatformExpression(null, _root, itemPath)
                : new PlatformExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class TaskStatusListExpression
    {
        private readonly IReadOnlyList<Generated.Models.TaskStatus> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TaskStatusListExpression(
            IReadOnlyList<Generated.Models.TaskStatus>? items,
            string? root = "TaskStatus(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.TaskStatus>();
            _root = root ?? "TaskStatus(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static TaskStatusListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.TaskStatus>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public TaskStatusExpression First() => Item(0, true);

        public TaskStatusExpression Get(int index)
        {
            return Item(index, false);
        }

        private TaskStatusExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new TaskStatusExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new TaskStatusExpression(null, _root, itemPath)
                : new TaskStatusExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class TaskListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Task> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TaskListExpression(
            IReadOnlyList<Generated.Models.Task>? items,
            string? root = "Task(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Task>();
            _root = root ?? "Task(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static TaskListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Task>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public TaskExpression First() => Item(0, true);

        public TaskExpression Get(int index)
        {
            return Item(index, false);
        }

        private TaskExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new TaskExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new TaskExpression(null, _root, itemPath)
                : new TaskExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class TaskExecutionLogListExpression
    {
        private readonly IReadOnlyList<Generated.Models.TaskExecutionLog> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TaskExecutionLogListExpression(
            IReadOnlyList<Generated.Models.TaskExecutionLog>? items,
            string? root = "TaskExecutionLog(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.TaskExecutionLog>();
            _root = root ?? "TaskExecutionLog(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static TaskExecutionLogListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.TaskExecutionLog>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public TaskExecutionLogExpression First() => Item(0, true);

        public TaskExecutionLogExpression Get(int index)
        {
            return Item(index, false);
        }

        private TaskExecutionLogExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new TaskExecutionLogExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new TaskExecutionLogExpression(null, _root, itemPath)
                : new TaskExecutionLogExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static PlatformExpression Platform(Generated.Models.Platform? value)
        {
            return new PlatformExpression(value, "Platform");
        }

        public static TaskStatusExpression TaskStatus(Generated.Models.TaskStatus? value)
        {
            return new TaskStatusExpression(value, "TaskStatus");
        }

        public static TaskExpression Task(Generated.Models.Task? value)
        {
            return new TaskExpression(value, "Task");
        }

        public static TaskExecutionLogExpression TaskExecutionLog(Generated.Models.TaskExecutionLog? value)
        {
            return new TaskExecutionLogExpression(value, "TaskExecutionLog");
        }
    }
}