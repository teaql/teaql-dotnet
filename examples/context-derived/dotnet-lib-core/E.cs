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

    public sealed class CatalogExpression
    {
        private readonly Generated.Models.Catalog? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CatalogExpression(
            Generated.Models.Catalog? value,
            string root = "Catalog(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Catalog(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Catalog? Eval()
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



        public EntryListExpression EntryList()
        {
            var path = _path.Member("entry_list", "EntryList");
            if (_notLoaded != null) return new EntryListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return EntryListExpression.Missing(_root, path);
            if (!_value.IsLoaded("EntryList"))
                return new EntryListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "entry_list"));
            return new EntryListExpression(_value.EntryList, _root, path);
        }
    }

    public sealed class EntryExpression
    {
        private readonly Generated.Models.Entry? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public EntryExpression(
            Generated.Models.Entry? value,
            string root = "Entry(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Entry(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Entry? Eval()
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

        public ValueExpression<System.DateTime?> BornAt()
        {
            var path = _path.Member("born_at", "BornAt");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("BornAt"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "born_at"));
            return new ValueExpression<System.DateTime?>(_value.BornAt);
        }

        public ValueExpression<System.DateTime?> TouchedAt()
        {
            var path = _path.Member("touched_at", "TouchedAt");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("TouchedAt"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "touched_at"));
            return new ValueExpression<System.DateTime?>(_value.TouchedAt);
        }

        public ValueExpression<string?> SourceIp()
        {
            var path = _path.Member("source_ip", "SourceIp");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("SourceIp"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "source_ip"));
            return new ValueExpression<string?>(_value.SourceIp);
        }

        public ValueExpression<string?> LastIp()
        {
            var path = _path.Member("last_ip", "LastIp");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("LastIp"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "last_ip"));
            return new ValueExpression<string?>(_value.LastIp);
        }

        public ValueExpression<string?> OpenedBy()
        {
            var path = _path.Member("opened_by", "OpenedBy");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("OpenedBy"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "opened_by"));
            return new ValueExpression<string?>(_value.OpenedBy);
        }

        public ValueExpression<string?> ChangedBy()
        {
            var path = _path.Member("changed_by", "ChangedBy");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ChangedBy"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "changed_by"));
            return new ValueExpression<string?>(_value.ChangedBy);
        }

        public ValueExpression<string?> GuardedToken()
        {
            var path = _path.Member("guarded_token", "GuardedToken");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("GuardedToken"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "guarded_token"));
            return new ValueExpression<string?>(_value.GuardedToken);
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

        public ValueExpression<long?> CatalogId()
        {
            var path = _path.Member("catalog", "CatalogId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Catalog"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "catalog"));
            return new ValueExpression<long?>(_value.Catalog);
        }

        public CatalogExpression Catalog()
        {
            var path = _path.Member("catalog", "Catalog");
            if (_notLoaded != null) return new CatalogExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CatalogExpression(null, _root, path);
            if (!_value.IsLoaded("Catalog"))
                return new CatalogExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "catalog"));
            if (_value.Catalog is null) return new CatalogExpression(null, _root, path);
            if (!_value.IsLoaded("CatalogEntity") || _value.CatalogEntity is null
                || _value.CatalogEntity.Id != _value.Catalog)
                return new CatalogExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "catalog"));
            return new CatalogExpression(_value.CatalogEntity, _root, path);
        }

    }

    public sealed class CatalogListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Catalog> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CatalogListExpression(
            IReadOnlyList<Generated.Models.Catalog>? items,
            string? root = "Catalog(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Catalog>();
            _root = root ?? "Catalog(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CatalogListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Catalog>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public CatalogExpression First() => Item(0, true);

        public CatalogExpression Get(int index)
        {
            return Item(index, false);
        }

        private CatalogExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new CatalogExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new CatalogExpression(null, _root, itemPath)
                : new CatalogExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class EntryListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Entry> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public EntryListExpression(
            IReadOnlyList<Generated.Models.Entry>? items,
            string? root = "Entry(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Entry>();
            _root = root ?? "Entry(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static EntryListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Entry>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public EntryExpression First() => Item(0, true);

        public EntryExpression Get(int index)
        {
            return Item(index, false);
        }

        private EntryExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new EntryExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new EntryExpression(null, _root, itemPath)
                : new EntryExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static CatalogExpression Catalog(Generated.Models.Catalog? value)
        {
            return new CatalogExpression(value, "Catalog");
        }

        public static EntryExpression Entry(Generated.Models.Entry? value)
        {
            return new EntryExpression(value, "Entry");
        }
    }
}