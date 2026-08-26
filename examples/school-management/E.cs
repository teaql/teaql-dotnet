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

        public ValueExpression<string> BaseUrl()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "BaseUrl");
            if (!_value.IsLoaded("BaseUrl"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "BaseUrl"));
            return new ValueExpression<string>(_value.BaseUrl);
        }

        public ValueExpression<System.DateTime?> CreateTime()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "CreateTime");
            if (!_value.IsLoaded("CreateTime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "CreateTime"));
            return new ValueExpression<System.DateTime?>(_value.CreateTime);
        }

        public ValueExpression<System.DateTime?> UpdateTime()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "UpdateTime");
            if (!_value.IsLoaded("UpdateTime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "UpdateTime"));
            return new ValueExpression<System.DateTime?>(_value.UpdateTime);
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



        public SchoolTypeListExpression SchoolTypeList()
        {
            var path = ExpressionPath.Append(_path, "SchoolTypeList");
            if (_notLoaded != null) return new SchoolTypeListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return SchoolTypeListExpression.Missing(_root, path);
            if (!_value.IsLoaded("SchoolTypeList"))
                return new SchoolTypeListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "SchoolTypeList"));
            return new SchoolTypeListExpression(_value.SchoolTypeList, _root, path);
        }

        public SchoolListExpression SchoolList()
        {
            var path = ExpressionPath.Append(_path, "SchoolList");
            if (_notLoaded != null) return new SchoolListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return SchoolListExpression.Missing(_root, path);
            if (!_value.IsLoaded("SchoolList"))
                return new SchoolListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "SchoolList"));
            return new SchoolListExpression(_value.SchoolList, _root, path);
        }
    }

    public sealed class SchoolTypeExpression
    {
        private readonly Generated.Models.SchoolType _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public SchoolTypeExpression(
            Generated.Models.SchoolType value,
            string root = "SchoolType(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.SchoolType Eval()
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

        public ValueExpression<string> Code()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "Code");
            if (!_value.IsLoaded("Code"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Code"));
            return new ValueExpression<string>(_value.Code);
        }

        public ValueExpression<decimal?> DisplayOrder()
        {
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<decimal?>.Missing();
            var path = ExpressionPath.Append(_path, "DisplayOrder");
            if (!_value.IsLoaded("DisplayOrder"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "DisplayOrder"));
            return new ValueExpression<decimal?>(_value.DisplayOrder);
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

        public SchoolListExpression SchoolList()
        {
            var path = ExpressionPath.Append(_path, "SchoolList");
            if (_notLoaded != null) return new SchoolListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return SchoolListExpression.Missing(_root, path);
            if (!_value.IsLoaded("SchoolList"))
                return new SchoolListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "SchoolList"));
            return new SchoolListExpression(_value.SchoolList, _root, path);
        }
    }

    public sealed class SchoolExpression
    {
        private readonly Generated.Models.School _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public SchoolExpression(
            Generated.Models.School value,
            string root = "School(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.School Eval()
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

        public ValueExpression<string> Address()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "Address");
            if (!_value.IsLoaded("Address"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Address"));
            return new ValueExpression<string>(_value.Address);
        }

        public ValueExpression<System.DateTime?> EstablishedDate()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "EstablishedDate");
            if (!_value.IsLoaded("EstablishedDate"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "EstablishedDate"));
            return new ValueExpression<System.DateTime?>(_value.EstablishedDate);
        }

        public ValueExpression<long?> StudentCapacity()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "StudentCapacity");
            if (!_value.IsLoaded("StudentCapacity"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "StudentCapacity"));
            return new ValueExpression<long?>(_value.StudentCapacity);
        }

        public ValueExpression<long?> Active()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Active");
            if (!_value.IsLoaded("Active"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Active"));
            return new ValueExpression<long?>(_value.Active);
        }

        public ValueExpression<System.DateTime?> CreateTime()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "CreateTime");
            if (!_value.IsLoaded("CreateTime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "CreateTime"));
            return new ValueExpression<System.DateTime?>(_value.CreateTime);
        }

        public ValueExpression<System.DateTime?> UpdateTime()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "UpdateTime");
            if (!_value.IsLoaded("UpdateTime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "UpdateTime"));
            return new ValueExpression<System.DateTime?>(_value.UpdateTime);
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

        public ValueExpression<long?> SchoolTypeId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "SchoolType");
            if (!_value.IsLoaded("SchoolType"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "SchoolType"));
            return new ValueExpression<long?>(_value.SchoolType);
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

        public SchoolTypeExpression SchoolType()
        {
            var path = ExpressionPath.Append(_path, "SchoolType");
            if (_notLoaded != null) return new SchoolTypeExpression(null, _root, path, _notLoaded);
            if (_value is null) return new SchoolTypeExpression(null, _root, path);
            if (!_value.IsLoaded("SchoolTypeEntity"))
                return new SchoolTypeExpression(null, _root, path,
                    new TeaQLNotLoadedException(_root, path, "SchoolType"));
            return new SchoolTypeExpression(_value.SchoolTypeEntity, _root, path);
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

    public sealed class SchoolTypeListExpression
    {
        private readonly IReadOnlyList<Generated.Models.SchoolType> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public SchoolTypeListExpression(
            IReadOnlyList<Generated.Models.SchoolType> items,
            string root = "SchoolType(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.SchoolType>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static SchoolTypeListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.SchoolType>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public SchoolTypeExpression First() => Get(0);

        public SchoolTypeExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new SchoolTypeExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new SchoolTypeExpression(null, _root, itemPath)
                : new SchoolTypeExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class SchoolListExpression
    {
        private readonly IReadOnlyList<Generated.Models.School> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public SchoolListExpression(
            IReadOnlyList<Generated.Models.School> items,
            string root = "School(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.School>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static SchoolListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.School>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public SchoolExpression First() => Get(0);

        public SchoolExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new SchoolExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new SchoolExpression(null, _root, itemPath)
                : new SchoolExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static PlatformExpression Platform(Generated.Models.Platform value)
        {
            return new PlatformExpression(value, $"Platform(id={value?.Id})");
        }

        public static SchoolTypeExpression SchoolType(Generated.Models.SchoolType value)
        {
            return new SchoolTypeExpression(value, $"SchoolType(id={value?.Id})");
        }

        public static SchoolExpression School(Generated.Models.School value)
        {
            return new SchoolExpression(value, $"School(id={value?.Id})");
        }
    }
}