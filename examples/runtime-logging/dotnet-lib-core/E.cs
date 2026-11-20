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



        public ShippingOrderListExpression ShippingOrderList()
        {
            var path = _path.Member("shipping_order_list", "ShippingOrderList");
            if (_notLoaded != null) return new ShippingOrderListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return ShippingOrderListExpression.Missing(_root, path);
            if (!_value.IsLoaded("ShippingOrderList"))
                return new ShippingOrderListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "shipping_order_list"));
            return new ShippingOrderListExpression(_value.ShippingOrderList, _root, path);
        }
    }

    public sealed class ShippingOrderExpression
    {
        private readonly Generated.Models.ShippingOrder? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ShippingOrderExpression(
            Generated.Models.ShippingOrder? value,
            string root = "ShippingOrder(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.ShippingOrder(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.ShippingOrder? Eval()
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

        public ValueExpression<string?> Reference()
        {
            var path = _path.Member("reference", "Reference");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Reference"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "reference"));
            return new ValueExpression<string?>(_value.Reference);
        }

        public ValueExpression<string?> CustomerReference()
        {
            var path = _path.Member("customer_reference", "CustomerReference");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("CustomerReference"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "customer_reference"));
            return new ValueExpression<string?>(_value.CustomerReference);
        }

        public ValueExpression<string?> Instructions()
        {
            var path = _path.Member("instructions", "Instructions");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Instructions"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "instructions"));
            return new ValueExpression<string?>(_value.Instructions);
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

        public ShippingItemListExpression ShippingItemList()
        {
            var path = _path.Member("shipping_item_list", "ShippingItemList");
            if (_notLoaded != null) return new ShippingItemListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return ShippingItemListExpression.Missing(_root, path);
            if (!_value.IsLoaded("ShippingItemList"))
                return new ShippingItemListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "shipping_item_list"));
            return new ShippingItemListExpression(_value.ShippingItemList, _root, path);
        }
    }

    public sealed class ShippingItemExpression
    {
        private readonly Generated.Models.ShippingItem? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ShippingItemExpression(
            Generated.Models.ShippingItem? value,
            string root = "ShippingItem(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.ShippingItem(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.ShippingItem? Eval()
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

        public ValueExpression<string?> Sku()
        {
            var path = _path.Member("sku", "Sku");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Sku"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "sku"));
            return new ValueExpression<string?>(_value.Sku);
        }

        public ValueExpression<long?> Quantity()
        {
            var path = _path.Member("quantity", "Quantity");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Quantity"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "quantity"));
            return new ValueExpression<long?>(_value.Quantity);
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

        public ValueExpression<long?> ShippingOrderId()
        {
            var path = _path.Member("shipping_order", "ShippingOrderId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("ShippingOrder"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "shipping_order"));
            return new ValueExpression<long?>(_value.ShippingOrder);
        }

        public ShippingOrderExpression ShippingOrder()
        {
            var path = _path.Member("shipping_order", "ShippingOrder");
            if (_notLoaded != null) return new ShippingOrderExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new ShippingOrderExpression(null, _root, path);
            if (!_value.IsLoaded("ShippingOrder"))
                return new ShippingOrderExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "shipping_order"));
            if (_value.ShippingOrder is null) return new ShippingOrderExpression(null, _root, path);
            if (!_value.IsLoaded("ShippingOrderEntity") || _value.ShippingOrderEntity is null
                || _value.ShippingOrderEntity.Id != _value.ShippingOrder)
                return new ShippingOrderExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "shipping_order"));
            return new ShippingOrderExpression(_value.ShippingOrderEntity, _root, path);
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

    public sealed class ShippingOrderListExpression
    {
        private readonly IReadOnlyList<Generated.Models.ShippingOrder> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ShippingOrderListExpression(
            IReadOnlyList<Generated.Models.ShippingOrder>? items,
            string? root = "ShippingOrder(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.ShippingOrder>();
            _root = root ?? "ShippingOrder(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static ShippingOrderListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.ShippingOrder>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public ShippingOrderExpression First() => Item(0, true);

        public ShippingOrderExpression Get(int index)
        {
            return Item(index, false);
        }

        private ShippingOrderExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new ShippingOrderExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new ShippingOrderExpression(null, _root, itemPath)
                : new ShippingOrderExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class ShippingItemListExpression
    {
        private readonly IReadOnlyList<Generated.Models.ShippingItem> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ShippingItemListExpression(
            IReadOnlyList<Generated.Models.ShippingItem>? items,
            string? root = "ShippingItem(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.ShippingItem>();
            _root = root ?? "ShippingItem(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static ShippingItemListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.ShippingItem>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public ShippingItemExpression First() => Item(0, true);

        public ShippingItemExpression Get(int index)
        {
            return Item(index, false);
        }

        private ShippingItemExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new ShippingItemExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new ShippingItemExpression(null, _root, itemPath)
                : new ShippingItemExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static WarehouseExpression Warehouse(Generated.Models.Warehouse? value)
        {
            return new WarehouseExpression(value, "Warehouse");
        }

        public static ShippingOrderExpression ShippingOrder(Generated.Models.ShippingOrder? value)
        {
            return new ShippingOrderExpression(value, "ShippingOrder");
        }

        public static ShippingItemExpression ShippingItem(Generated.Models.ShippingItem? value)
        {
            return new ShippingItemExpression(value, "ShippingItem");
        }
    }
}