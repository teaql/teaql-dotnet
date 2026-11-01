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

    public sealed class TypePlatformExpression
    {
        private readonly Generated.Models.TypePlatform? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TypePlatformExpression(
            Generated.Models.TypePlatform? value,
            string root = "TypePlatform(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.TypePlatform(<root>)", JsonFieldNamingProfiles.Parse("snake_case"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.TypePlatform? Eval()
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



        public ScalarTypeProbeListExpression ScalarTypeProbeList()
        {
            var path = _path.Member("scalar_type_probe_list", "ScalarTypeProbeList");
            if (_notLoaded != null) return new ScalarTypeProbeListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return ScalarTypeProbeListExpression.Missing(_root, path);
            if (!_value.IsLoaded("ScalarTypeProbeList"))
                return new ScalarTypeProbeListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "scalar_type_probe_list"));
            return new ScalarTypeProbeListExpression(_value.ScalarTypeProbeList, _root, path);
        }
    }

    public sealed class ScalarTypeProbeExpression
    {
        private readonly Generated.Models.ScalarTypeProbe? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ScalarTypeProbeExpression(
            Generated.Models.ScalarTypeProbe? value,
            string root = "ScalarTypeProbe(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.ScalarTypeProbe(<root>)", JsonFieldNamingProfiles.Parse("snake_case"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.ScalarTypeProbe? Eval()
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

        public ValueExpression<string?> RequiredString()
        {
            var path = _path.Member("required_string", "RequiredString");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("RequiredString"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_string"));
            return new ValueExpression<string?>(_value.RequiredString);
        }

        public ValueExpression<string?> OptionalString()
        {
            var path = _path.Member("optional_string", "OptionalString");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("OptionalString"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_string"));
            return new ValueExpression<string?>(_value.OptionalString);
        }

        public ValueExpression<string?> LargeText()
        {
            var path = _path.Member("large_text", "LargeText");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("LargeText"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "large_text"));
            return new ValueExpression<string?>(_value.LargeText);
        }

        public ValueExpression<bool?> RequiredBoolean()
        {
            var path = _path.Member("required_boolean", "RequiredBoolean");
            if (_notLoaded != null) return ValueExpression<bool?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<bool?>.Missing();
            if (!_value.IsLoaded("RequiredBoolean"))
                return ValueExpression<bool?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_boolean"));
            return new ValueExpression<bool?>(_value.RequiredBoolean);
        }

        public ValueExpression<bool?> OptionalBoolean()
        {
            var path = _path.Member("optional_boolean", "OptionalBoolean");
            if (_notLoaded != null) return ValueExpression<bool?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<bool?>.Missing();
            if (!_value.IsLoaded("OptionalBoolean"))
                return ValueExpression<bool?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_boolean"));
            return new ValueExpression<bool?>(_value.OptionalBoolean);
        }

        public ValueExpression<long?> RequiredInteger()
        {
            var path = _path.Member("required_integer", "RequiredInteger");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("RequiredInteger"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_integer"));
            return new ValueExpression<long?>(_value.RequiredInteger);
        }

        public ValueExpression<long?> OptionalInteger()
        {
            var path = _path.Member("optional_integer", "OptionalInteger");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("OptionalInteger"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_integer"));
            return new ValueExpression<long?>(_value.OptionalInteger);
        }

        public ValueExpression<long?> RequiredLong()
        {
            var path = _path.Member("required_long", "RequiredLong");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("RequiredLong"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_long"));
            return new ValueExpression<long?>(_value.RequiredLong);
        }

        public ValueExpression<long?> OptionalLong()
        {
            var path = _path.Member("optional_long", "OptionalLong");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("OptionalLong"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_long"));
            return new ValueExpression<long?>(_value.OptionalLong);
        }

        public ValueExpression<decimal?> RequiredDecimal()
        {
            var path = _path.Member("required_decimal", "RequiredDecimal");
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<decimal?>.Missing();
            if (!_value.IsLoaded("RequiredDecimal"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_decimal"));
            return new ValueExpression<decimal?>(_value.RequiredDecimal);
        }

        public ValueExpression<decimal?> OptionalDecimal()
        {
            var path = _path.Member("optional_decimal", "OptionalDecimal");
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<decimal?>.Missing();
            if (!_value.IsLoaded("OptionalDecimal"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_decimal"));
            return new ValueExpression<decimal?>(_value.OptionalDecimal);
        }

        public ValueExpression<double?> RequiredFloat()
        {
            var path = _path.Member("required_float", "RequiredFloat");
            if (_notLoaded != null) return ValueExpression<double?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<double?>.Missing();
            if (!_value.IsLoaded("RequiredFloat"))
                return ValueExpression<double?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_float"));
            return new ValueExpression<double?>(_value.RequiredFloat);
        }

        public ValueExpression<double?> OptionalFloat()
        {
            var path = _path.Member("optional_float", "OptionalFloat");
            if (_notLoaded != null) return ValueExpression<double?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<double?>.Missing();
            if (!_value.IsLoaded("OptionalFloat"))
                return ValueExpression<double?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_float"));
            return new ValueExpression<double?>(_value.OptionalFloat);
        }

        public ValueExpression<double?> RequiredDouble()
        {
            var path = _path.Member("required_double", "RequiredDouble");
            if (_notLoaded != null) return ValueExpression<double?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<double?>.Missing();
            if (!_value.IsLoaded("RequiredDouble"))
                return ValueExpression<double?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_double"));
            return new ValueExpression<double?>(_value.RequiredDouble);
        }

        public ValueExpression<double?> OptionalDouble()
        {
            var path = _path.Member("optional_double", "OptionalDouble");
            if (_notLoaded != null) return ValueExpression<double?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<double?>.Missing();
            if (!_value.IsLoaded("OptionalDouble"))
                return ValueExpression<double?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_double"));
            return new ValueExpression<double?>(_value.OptionalDouble);
        }

        public ValueExpression<System.DateTime?> RequiredDate()
        {
            var path = _path.Member("required_date", "RequiredDate");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("RequiredDate"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_date"));
            return new ValueExpression<System.DateTime?>(_value.RequiredDate);
        }

        public ValueExpression<System.DateTime?> OptionalDate()
        {
            var path = _path.Member("optional_date", "OptionalDate");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("OptionalDate"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_date"));
            return new ValueExpression<System.DateTime?>(_value.OptionalDate);
        }

        public ValueExpression<System.TimeSpan?> RequiredTime()
        {
            var path = _path.Member("required_time", "RequiredTime");
            if (_notLoaded != null) return ValueExpression<System.TimeSpan?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.TimeSpan?>.Missing();
            if (!_value.IsLoaded("RequiredTime"))
                return ValueExpression<System.TimeSpan?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_time"));
            return new ValueExpression<System.TimeSpan?>(_value.RequiredTime);
        }

        public ValueExpression<System.TimeSpan?> OptionalTime()
        {
            var path = _path.Member("optional_time", "OptionalTime");
            if (_notLoaded != null) return ValueExpression<System.TimeSpan?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.TimeSpan?>.Missing();
            if (!_value.IsLoaded("OptionalTime"))
                return ValueExpression<System.TimeSpan?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_time"));
            return new ValueExpression<System.TimeSpan?>(_value.OptionalTime);
        }

        public ValueExpression<System.DateTime?> RequiredDatetime()
        {
            var path = _path.Member("required_datetime", "RequiredDatetime");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("RequiredDatetime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "required_datetime"));
            return new ValueExpression<System.DateTime?>(_value.RequiredDatetime);
        }

        public ValueExpression<System.DateTime?> OptionalDatetime()
        {
            var path = _path.Member("optional_datetime", "OptionalDatetime");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("OptionalDatetime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "optional_datetime"));
            return new ValueExpression<System.DateTime?>(_value.OptionalDatetime);
        }

        public ValueExpression<System.DateTime?> CreateTime()
        {
            var path = _path.Member("create_time", "CreateTime");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("CreateTime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "create_time"));
            return new ValueExpression<System.DateTime?>(_value.CreateTime);
        }

        public ValueExpression<System.DateTime?> UpdateTime()
        {
            var path = _path.Member("update_time", "UpdateTime");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("UpdateTime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "update_time"));
            return new ValueExpression<System.DateTime?>(_value.UpdateTime);
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

        public ValueExpression<long?> TypePlatformId()
        {
            var path = _path.Member("type_platform", "TypePlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("TypePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "type_platform"));
            return new ValueExpression<long?>(_value.TypePlatform);
        }

        public TypePlatformExpression TypePlatform()
        {
            var path = _path.Member("type_platform", "TypePlatform");
            if (_notLoaded != null) return new TypePlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new TypePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("TypePlatform"))
                return new TypePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "type_platform"));
            if (_value.TypePlatform is null) return new TypePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("TypePlatformEntity") || _value.TypePlatformEntity is null
                || _value.TypePlatformEntity.Id != _value.TypePlatform)
                return new TypePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "type_platform"));
            return new TypePlatformExpression(_value.TypePlatformEntity, _root, path);
        }

    }

    public sealed class TypePlatformListExpression
    {
        private readonly IReadOnlyList<Generated.Models.TypePlatform> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TypePlatformListExpression(
            IReadOnlyList<Generated.Models.TypePlatform>? items,
            string? root = "TypePlatform(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.TypePlatform>();
            _root = root ?? "TypePlatform(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("snake_case"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static TypePlatformListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.TypePlatform>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public TypePlatformExpression First() => Item(0, true);

        public TypePlatformExpression Get(int index)
        {
            return Item(index, false);
        }

        private TypePlatformExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new TypePlatformExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new TypePlatformExpression(null, _root, itemPath)
                : new TypePlatformExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class ScalarTypeProbeListExpression
    {
        private readonly IReadOnlyList<Generated.Models.ScalarTypeProbe> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ScalarTypeProbeListExpression(
            IReadOnlyList<Generated.Models.ScalarTypeProbe>? items,
            string? root = "ScalarTypeProbe(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.ScalarTypeProbe>();
            _root = root ?? "ScalarTypeProbe(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("snake_case"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static ScalarTypeProbeListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.ScalarTypeProbe>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public ScalarTypeProbeExpression First() => Item(0, true);

        public ScalarTypeProbeExpression Get(int index)
        {
            return Item(index, false);
        }

        private ScalarTypeProbeExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new ScalarTypeProbeExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new ScalarTypeProbeExpression(null, _root, itemPath)
                : new ScalarTypeProbeExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static TypePlatformExpression TypePlatform(Generated.Models.TypePlatform? value)
        {
            return new TypePlatformExpression(value, "TypePlatform");
        }

        public static ScalarTypeProbeExpression ScalarTypeProbe(Generated.Models.ScalarTypeProbe? value)
        {
            return new ScalarTypeProbeExpression(value, "ScalarTypeProbe");
        }
    }
}