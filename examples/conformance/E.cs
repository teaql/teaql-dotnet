using System;
using System.Collections.Generic;

namespace Generated
{
    public sealed class TeaQLNotLoadedException : InvalidOperationException
    {
        public string Root { get; }
        public string AccessPath { get; }
        public string BreakPoint { get; }
        public string SuggestedFix { get; }

        public TeaQLNotLoadedException(string root, string accessPath, string breakPoint)
            : base($"TeaQLNotLoadedError: root={root} access_path={accessPath} break_point={breakPoint} " +
                   $"suggested_fix=Select{breakPoint}(...) human_message=访问 {root}.{accessPath} 时缺少预加载")
        {
            Root = root;
            AccessPath = accessPath;
            BreakPoint = breakPoint;
            SuggestedFix = $"Select{breakPoint}(...)";
        }
    }

    internal static class ExpressionPath
    {
        internal static string Append(string prefix, string field) =>
            string.IsNullOrEmpty(prefix) ? field : $"{prefix}.{field}";
    }

    public sealed class ValueExpression<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;
        private readonly TeaQLNotLoadedException _notLoaded;

        public bool HasValue
        {
            get
            {
                if (_notLoaded != null) throw _notLoaded;
                return _hasValue;
            }
        }

        public ValueExpression(T value, bool hasValue = true, TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _hasValue = hasValue;
            _notLoaded = notLoaded;
        }

        public T Eval()
        {
            if (_notLoaded != null) throw _notLoaded;
            return _value;
        }

        public T OrIfNull(T fallback)
        {
            var value = Eval();
            return _hasValue && value is not null ? value : fallback;
        }

        public static ValueExpression<T> Missing() => new(default!, false);
        public static ValueExpression<T> NotLoaded(TeaQLNotLoadedException error) => new(default!, false, error);
    }

    public sealed class PlatformExpression
    {
        private readonly Generated.Models.Platform _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public PlatformExpression(
            Generated.Models.Platform value,
            string root = "Platform(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.Platform Eval()
        {
            if (_notLoaded != null) throw _notLoaded;
            return _value;
        }

        public ValueExpression<long?> Id()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Id");
            if (!_value.IsLoaded("Id"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Id"));
            return new ValueExpression<long?>(_value.Id);
        }

        public ValueExpression<string> Name()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "Name");
            if (!_value.IsLoaded("Name"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Name"));
            return new ValueExpression<string>(_value.Name);
        }

        public ValueExpression<long?> Version()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Version");
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Version"));
            return new ValueExpression<long?>(_value.Version);
        }



        public WorkItemListExpression WorkItemList()
        {
            var path = ExpressionPath.Append(_path, "WorkItemList");
            if (_notLoaded != null) return new WorkItemListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return WorkItemListExpression.Missing(_root, path);
            if (!_value.IsLoaded("WorkItemList"))
                return new WorkItemListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "WorkItemList"));
            return new WorkItemListExpression(_value.WorkItemList, _root, path);
        }
    }

    public sealed class WorkItemExpression
    {
        private readonly Generated.Models.WorkItem _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public WorkItemExpression(
            Generated.Models.WorkItem value,
            string root = "WorkItem(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.WorkItem Eval()
        {
            if (_notLoaded != null) throw _notLoaded;
            return _value;
        }

        public ValueExpression<long?> Id()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Id");
            if (!_value.IsLoaded("Id"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Id"));
            return new ValueExpression<long?>(_value.Id);
        }

        public ValueExpression<string> Title()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "Title");
            if (!_value.IsLoaded("Title"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Title"));
            return new ValueExpression<string>(_value.Title);
        }

        public ValueExpression<string> Description()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "Description");
            if (!_value.IsLoaded("Description"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Description"));
            return new ValueExpression<string>(_value.Description);
        }

        public ValueExpression<long?> Version()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Version");
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Version"));
            return new ValueExpression<long?>(_value.Version);
        }

        public ValueExpression<long?> PlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Platform");
            if (!_value.IsLoaded("Platform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Platform"));
            return new ValueExpression<long?>(_value.Platform);
        }

        public PlatformExpression Platform()
        {
            var path = ExpressionPath.Append(_path, "Platform");
            if (_notLoaded != null) return new PlatformExpression(null, _root, path, _notLoaded);
            if (_value is null) return new PlatformExpression(null, _root, path);
            if (!_value.IsLoaded("PlatformEntity"))
                return new PlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(_root, path, "Platform"));
            return new PlatformExpression(_value.PlatformEntity, _root, path);
        }

    }

    public sealed class PlatformListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Platform> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public PlatformListExpression(
            IReadOnlyList<Generated.Models.Platform> items,
            string root = "Platform(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Platform>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static PlatformListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.Platform>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public PlatformExpression First() => Get(0);

        public PlatformExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new PlatformExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new PlatformExpression(null, _root, itemPath)
                : new PlatformExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class WorkItemListExpression
    {
        private readonly IReadOnlyList<Generated.Models.WorkItem> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public WorkItemListExpression(
            IReadOnlyList<Generated.Models.WorkItem> items,
            string root = "WorkItem(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.WorkItem>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static WorkItemListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.WorkItem>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public WorkItemExpression First() => Get(0);

        public WorkItemExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new WorkItemExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new WorkItemExpression(null, _root, itemPath)
                : new WorkItemExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static PlatformExpression Platform(Generated.Models.Platform value)
        {
            return new PlatformExpression(value, $"Platform(id={value?.Id})");
        }

        public static WorkItemExpression WorkItem(Generated.Models.WorkItem value)
        {
            return new WorkItemExpression(value, $"WorkItem(id={value?.Id})");
        }
    }
}