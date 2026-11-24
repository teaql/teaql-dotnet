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

    public sealed class WarehouseExpression
    {
        private readonly Generated.Models.Warehouse? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public WarehouseExpression(
            Generated.Models.Warehouse? value,
            string root = "Warehouse(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Warehouse(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Warehouse? Eval()
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



        public SupplierListExpression SupplierList()
        {
            var path = _path.Member("supplier_list", "SupplierList");
            if (_notLoaded != null) return new SupplierListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return SupplierListExpression.Missing(_root, path);
            if (!_value.IsLoaded("SupplierList"))
                return new SupplierListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "supplier_list"));
            return new SupplierListExpression(_value.SupplierList, _root, path);
        }
    }

    public sealed class SupplierExpression
    {
        private readonly Generated.Models.Supplier? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public SupplierExpression(
            Generated.Models.Supplier? value,
            string root = "Supplier(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Supplier(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Supplier? Eval()
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

        public ValueExpression<long?> MinimumOrderQuantity()
        {
            var path = _path.Member("minimum_order_quantity", "MinimumOrderQuantity");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("MinimumOrderQuantity"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "minimum_order_quantity"));
            return new ValueExpression<long?>(_value.MinimumOrderQuantity);
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

        public ValueExpression<long?> WarehouseId()
        {
            var path = _path.Member("warehouse", "WarehouseId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Warehouse"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "warehouse"));
            return new ValueExpression<long?>(_value.Warehouse);
        }

        public WarehouseExpression Warehouse()
        {
            var path = _path.Member("warehouse", "Warehouse");
            if (_notLoaded != null) return new WarehouseExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new WarehouseExpression(null, _root, path);
            if (!_value.IsLoaded("Warehouse"))
                return new WarehouseExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "warehouse"));
            if (_value.Warehouse is null) return new WarehouseExpression(null, _root, path);
            if (!_value.IsLoaded("WarehouseEntity") || _value.WarehouseEntity is null
                || _value.WarehouseEntity.Id != _value.Warehouse)
                return new WarehouseExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "warehouse"));
            return new WarehouseExpression(_value.WarehouseEntity, _root, path);
        }

    }

    public sealed class WarehouseListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Warehouse> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public WarehouseListExpression(
            IReadOnlyList<Generated.Models.Warehouse>? items,
            string? root = "Warehouse(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Warehouse>();
            _root = root ?? "Warehouse(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static WarehouseListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Warehouse>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public WarehouseExpression First() => Item(0, true);

        public WarehouseExpression Get(int index)
        {
            return Item(index, false);
        }

        private WarehouseExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new WarehouseExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new WarehouseExpression(null, _root, itemPath)
                : new WarehouseExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class SupplierListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Supplier> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public SupplierListExpression(
            IReadOnlyList<Generated.Models.Supplier>? items,
            string? root = "Supplier(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Supplier>();
            _root = root ?? "Supplier(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static SupplierListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Supplier>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public SupplierExpression First() => Item(0, true);

        public SupplierExpression Get(int index)
        {
            return Item(index, false);
        }

        private SupplierExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new SupplierExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new SupplierExpression(null, _root, itemPath)
                : new SupplierExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static WarehouseExpression Warehouse(Generated.Models.Warehouse? value)
        {
            return new WarehouseExpression(value, "Warehouse");
        }

        public static SupplierExpression Supplier(Generated.Models.Supplier? value)
        {
            return new SupplierExpression(value, "Supplier");
        }
    }
}