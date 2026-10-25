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

        public ValueExpression<string?> BaseUrl()
        {
            var path = _path.Member("base_url", "BaseUrl");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("BaseUrl"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "base_url"));
            return new ValueExpression<string?>(_value.BaseUrl);
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



        public SchoolTypeListExpression SchoolTypeList()
        {
            var path = _path.Member("school_type_list", "SchoolTypeList");
            if (_notLoaded != null) return new SchoolTypeListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return SchoolTypeListExpression.Missing(_root, path);
            if (!_value.IsLoaded("SchoolTypeList"))
                return new SchoolTypeListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "school_type_list"));
            return new SchoolTypeListExpression(_value.SchoolTypeList, _root, path);
        }

        public SchoolListExpression SchoolList()
        {
            var path = _path.Member("school_list", "SchoolList");
            if (_notLoaded != null) return new SchoolListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return SchoolListExpression.Missing(_root, path);
            if (!_value.IsLoaded("SchoolList"))
                return new SchoolListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "school_list"));
            return new SchoolListExpression(_value.SchoolList, _root, path);
        }
    }

    public sealed class SchoolTypeExpression
    {
        private readonly Generated.Models.SchoolType? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public SchoolTypeExpression(
            Generated.Models.SchoolType? value,
            string root = "SchoolType(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.SchoolType(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.SchoolType? Eval()
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

        public ValueExpression<decimal?> DisplayOrder()
        {
            var path = _path.Member("display_order", "DisplayOrder");
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<decimal?>.Missing();
            if (!_value.IsLoaded("DisplayOrder"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "display_order"));
            return new ValueExpression<decimal?>(_value.DisplayOrder);
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
            if (!_value.IsLoaded("PlatformEntity"))
                return new PlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "platform"));
            return new PlatformExpression(_value.PlatformEntity, _root, path);
        }

        public SchoolListExpression SchoolList()
        {
            var path = _path.Member("school_list", "SchoolList");
            if (_notLoaded != null) return new SchoolListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return SchoolListExpression.Missing(_root, path);
            if (!_value.IsLoaded("SchoolList"))
                return new SchoolListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "school_list"));
            return new SchoolListExpression(_value.SchoolList, _root, path);
        }
    }

    public sealed class SchoolExpression
    {
        private readonly Generated.Models.School? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public SchoolExpression(
            Generated.Models.School? value,
            string root = "School(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.School(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.School? Eval()
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

        public ValueExpression<string?> Address()
        {
            var path = _path.Member("address", "Address");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Address"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "address"));
            return new ValueExpression<string?>(_value.Address);
        }

        public ValueExpression<System.DateTime?> EstablishedDate()
        {
            var path = _path.Member("established_date", "EstablishedDate");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("EstablishedDate"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "established_date"));
            return new ValueExpression<System.DateTime?>(_value.EstablishedDate);
        }

        public ValueExpression<long?> StudentCapacity()
        {
            var path = _path.Member("student_capacity", "StudentCapacity");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("StudentCapacity"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "student_capacity"));
            return new ValueExpression<long?>(_value.StudentCapacity);
        }

        public ValueExpression<bool?> Active()
        {
            var path = _path.Member("active", "Active");
            if (_notLoaded != null) return ValueExpression<bool?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<bool?>.Missing();
            if (!_value.IsLoaded("Active"))
                return ValueExpression<bool?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "active"));
            return new ValueExpression<bool?>(_value.Active);
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

        public ValueExpression<long?> PlatformId()
        {
            var path = _path.Member("platform", "PlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Platform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "platform"));
            return new ValueExpression<long?>(_value.Platform);
        }

        public ValueExpression<long?> SchoolTypeId()
        {
            var path = _path.Member("school_type", "SchoolTypeId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("SchoolType"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "school_type"));
            return new ValueExpression<long?>(_value.SchoolType);
        }

        public PlatformExpression Platform()
        {
            var path = _path.Member("platform", "Platform");
            if (_notLoaded != null) return new PlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new PlatformExpression(null, _root, path);
            if (!_value.IsLoaded("PlatformEntity"))
                return new PlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "platform"));
            return new PlatformExpression(_value.PlatformEntity, _root, path);
        }

        public SchoolTypeExpression SchoolType()
        {
            var path = _path.Member("school_type", "SchoolType");
            if (_notLoaded != null) return new SchoolTypeExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new SchoolTypeExpression(null, _root, path);
            if (!_value.IsLoaded("SchoolTypeEntity"))
                return new SchoolTypeExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "school_type"));
            return new SchoolTypeExpression(_value.SchoolTypeEntity, _root, path);
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

    public sealed class SchoolTypeListExpression
    {
        private readonly IReadOnlyList<Generated.Models.SchoolType> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public SchoolTypeListExpression(
            IReadOnlyList<Generated.Models.SchoolType>? items,
            string? root = "SchoolType(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.SchoolType>();
            _root = root ?? "SchoolType(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static SchoolTypeListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.SchoolType>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public SchoolTypeExpression First() => Item(0, true);

        public SchoolTypeExpression Get(int index)
        {
            return Item(index, false);
        }

        private SchoolTypeExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new SchoolTypeExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new SchoolTypeExpression(null, _root, itemPath)
                : new SchoolTypeExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class SchoolListExpression
    {
        private readonly IReadOnlyList<Generated.Models.School> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public SchoolListExpression(
            IReadOnlyList<Generated.Models.School>? items,
            string? root = "School(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.School>();
            _root = root ?? "School(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static SchoolListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.School>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public SchoolExpression First() => Item(0, true);

        public SchoolExpression Get(int index)
        {
            return Item(index, false);
        }

        private SchoolExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new SchoolExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new SchoolExpression(null, _root, itemPath)
                : new SchoolExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static PlatformExpression Platform(Generated.Models.Platform? value)
        {
            return new PlatformExpression(value, "Platform");
        }

        public static SchoolTypeExpression SchoolType(Generated.Models.SchoolType? value)
        {
            return new SchoolTypeExpression(value, "SchoolType");
        }

        public static SchoolExpression School(Generated.Models.School? value)
        {
            return new SchoolExpression(value, "School");
        }
    }
}