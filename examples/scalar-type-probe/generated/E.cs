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
        private readonly TeaQLNotLoadedException? _notLoaded;

        public bool HasValue
        {
            get
            {
                if (_notLoaded != null) throw _notLoaded;
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

    public sealed class TypePlatformExpression
    {
        private readonly Generated.Models.TypePlatform? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TypePlatformExpression(
            Generated.Models.TypePlatform? value,
            string root = "TypePlatform(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.TypePlatform? Eval()
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

        public ValueExpression<string?> Name()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "Name");
            if (!_value.IsLoaded("Name"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Name"));
            return new ValueExpression<string?>(_value.Name);
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



        public ScalarTypeProbeListExpression ScalarTypeProbeList()
        {
            var path = ExpressionPath.Append(_path, "ScalarTypeProbeList");
            if (_notLoaded != null) return new ScalarTypeProbeListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return ScalarTypeProbeListExpression.Missing(_root, path);
            if (!_value.IsLoaded("ScalarTypeProbeList"))
                return new ScalarTypeProbeListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "ScalarTypeProbeList"));
            return new ScalarTypeProbeListExpression(_value.ScalarTypeProbeList, _root, path);
        }
    }

    public sealed class ScalarTypeProbeExpression
    {
        private readonly Generated.Models.ScalarTypeProbe? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ScalarTypeProbeExpression(
            Generated.Models.ScalarTypeProbe? value,
            string root = "ScalarTypeProbe(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.ScalarTypeProbe? Eval()
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

        public ValueExpression<string?> RequiredString()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredString");
            if (!_value.IsLoaded("RequiredString"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredString"));
            return new ValueExpression<string?>(_value.RequiredString);
        }

        public ValueExpression<string?> OptionalString()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalString");
            if (!_value.IsLoaded("OptionalString"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalString"));
            return new ValueExpression<string?>(_value.OptionalString);
        }

        public ValueExpression<string?> LargeText()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "LargeText");
            if (!_value.IsLoaded("LargeText"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "LargeText"));
            return new ValueExpression<string?>(_value.LargeText);
        }

        public ValueExpression<bool?> RequiredBoolean()
        {
            if (_notLoaded != null) return ValueExpression<bool?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<bool?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredBoolean");
            if (!_value.IsLoaded("RequiredBoolean"))
                return ValueExpression<bool?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredBoolean"));
            return new ValueExpression<bool?>(_value.RequiredBoolean);
        }

        public ValueExpression<bool?> OptionalBoolean()
        {
            if (_notLoaded != null) return ValueExpression<bool?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<bool?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalBoolean");
            if (!_value.IsLoaded("OptionalBoolean"))
                return ValueExpression<bool?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalBoolean"));
            return new ValueExpression<bool?>(_value.OptionalBoolean);
        }

        public ValueExpression<long?> RequiredInteger()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredInteger");
            if (!_value.IsLoaded("RequiredInteger"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredInteger"));
            return new ValueExpression<long?>(_value.RequiredInteger);
        }

        public ValueExpression<long?> OptionalInteger()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalInteger");
            if (!_value.IsLoaded("OptionalInteger"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalInteger"));
            return new ValueExpression<long?>(_value.OptionalInteger);
        }

        public ValueExpression<long?> RequiredLong()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredLong");
            if (!_value.IsLoaded("RequiredLong"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredLong"));
            return new ValueExpression<long?>(_value.RequiredLong);
        }

        public ValueExpression<long?> OptionalLong()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalLong");
            if (!_value.IsLoaded("OptionalLong"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalLong"));
            return new ValueExpression<long?>(_value.OptionalLong);
        }

        public ValueExpression<decimal?> RequiredDecimal()
        {
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<decimal?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredDecimal");
            if (!_value.IsLoaded("RequiredDecimal"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredDecimal"));
            return new ValueExpression<decimal?>(_value.RequiredDecimal);
        }

        public ValueExpression<decimal?> OptionalDecimal()
        {
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<decimal?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalDecimal");
            if (!_value.IsLoaded("OptionalDecimal"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalDecimal"));
            return new ValueExpression<decimal?>(_value.OptionalDecimal);
        }

        public ValueExpression<double?> RequiredFloat()
        {
            if (_notLoaded != null) return ValueExpression<double?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<double?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredFloat");
            if (!_value.IsLoaded("RequiredFloat"))
                return ValueExpression<double?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredFloat"));
            return new ValueExpression<double?>(_value.RequiredFloat);
        }

        public ValueExpression<double?> OptionalFloat()
        {
            if (_notLoaded != null) return ValueExpression<double?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<double?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalFloat");
            if (!_value.IsLoaded("OptionalFloat"))
                return ValueExpression<double?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalFloat"));
            return new ValueExpression<double?>(_value.OptionalFloat);
        }

        public ValueExpression<double?> RequiredDouble()
        {
            if (_notLoaded != null) return ValueExpression<double?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<double?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredDouble");
            if (!_value.IsLoaded("RequiredDouble"))
                return ValueExpression<double?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredDouble"));
            return new ValueExpression<double?>(_value.RequiredDouble);
        }

        public ValueExpression<double?> OptionalDouble()
        {
            if (_notLoaded != null) return ValueExpression<double?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<double?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalDouble");
            if (!_value.IsLoaded("OptionalDouble"))
                return ValueExpression<double?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalDouble"));
            return new ValueExpression<double?>(_value.OptionalDouble);
        }

        public ValueExpression<System.DateTime?> RequiredDate()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredDate");
            if (!_value.IsLoaded("RequiredDate"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredDate"));
            return new ValueExpression<System.DateTime?>(_value.RequiredDate);
        }

        public ValueExpression<System.DateTime?> OptionalDate()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalDate");
            if (!_value.IsLoaded("OptionalDate"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalDate"));
            return new ValueExpression<System.DateTime?>(_value.OptionalDate);
        }

        public ValueExpression<System.TimeSpan?> RequiredTime()
        {
            if (_notLoaded != null) return ValueExpression<System.TimeSpan?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.TimeSpan?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredTime");
            if (!_value.IsLoaded("RequiredTime"))
                return ValueExpression<System.TimeSpan?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredTime"));
            return new ValueExpression<System.TimeSpan?>(_value.RequiredTime);
        }

        public ValueExpression<System.TimeSpan?> OptionalTime()
        {
            if (_notLoaded != null) return ValueExpression<System.TimeSpan?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.TimeSpan?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalTime");
            if (!_value.IsLoaded("OptionalTime"))
                return ValueExpression<System.TimeSpan?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalTime"));
            return new ValueExpression<System.TimeSpan?>(_value.OptionalTime);
        }

        public ValueExpression<System.DateTime?> RequiredDatetime()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "RequiredDatetime");
            if (!_value.IsLoaded("RequiredDatetime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequiredDatetime"));
            return new ValueExpression<System.DateTime?>(_value.RequiredDatetime);
        }

        public ValueExpression<System.DateTime?> OptionalDatetime()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "OptionalDatetime");
            if (!_value.IsLoaded("OptionalDatetime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OptionalDatetime"));
            return new ValueExpression<System.DateTime?>(_value.OptionalDatetime);
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

        public ValueExpression<long?> TypePlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "TypePlatform");
            if (!_value.IsLoaded("TypePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "TypePlatform"));
            return new ValueExpression<long?>(_value.TypePlatform);
        }

        public TypePlatformExpression TypePlatform()
        {
            var path = ExpressionPath.Append(_path, "TypePlatform");
            if (_notLoaded != null) return new TypePlatformExpression(null, _root, path, _notLoaded);
            if (_value is null) return new TypePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("TypePlatformEntity"))
                return new TypePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(_root, path, "TypePlatform"));
            return new TypePlatformExpression(_value.TypePlatformEntity, _root, path);
        }

    }

    public sealed class TypePlatformListExpression
    {
        private readonly IReadOnlyList<Generated.Models.TypePlatform> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TypePlatformListExpression(
            IReadOnlyList<Generated.Models.TypePlatform>? items,
            string? root = "TypePlatform(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.TypePlatform>();
            _root = root ?? "TypePlatform(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static TypePlatformListExpression Missing(string? root = null, string path = "") =>
            new(new List<Generated.Models.TypePlatform>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public TypePlatformExpression First() => Get(0);

        public TypePlatformExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new TypePlatformExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new TypePlatformExpression(null, _root, itemPath)
                : new TypePlatformExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class ScalarTypeProbeListExpression
    {
        private readonly IReadOnlyList<Generated.Models.ScalarTypeProbe> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ScalarTypeProbeListExpression(
            IReadOnlyList<Generated.Models.ScalarTypeProbe>? items,
            string? root = "ScalarTypeProbe(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.ScalarTypeProbe>();
            _root = root ?? "ScalarTypeProbe(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static ScalarTypeProbeListExpression Missing(string? root = null, string path = "") =>
            new(new List<Generated.Models.ScalarTypeProbe>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public ScalarTypeProbeExpression First() => Get(0);

        public ScalarTypeProbeExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new ScalarTypeProbeExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new ScalarTypeProbeExpression(null, _root, itemPath)
                : new ScalarTypeProbeExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static TypePlatformExpression TypePlatform(Generated.Models.TypePlatform? value)
        {
            return new TypePlatformExpression(value, $"TypePlatform(id={value?.Id})");
        }

        public static ScalarTypeProbeExpression ScalarTypeProbe(Generated.Models.ScalarTypeProbe? value)
        {
            return new ScalarTypeProbeExpression(value, $"ScalarTypeProbe(id={value?.Id})");
        }
    }
}