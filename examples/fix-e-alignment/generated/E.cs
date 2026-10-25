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

    public sealed class TenantExpression
    {
        private readonly Generated.Models.Tenant? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TenantExpression(
            Generated.Models.Tenant? value,
            string root = "Tenant(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Tenant(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Tenant? Eval()
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



        public PurchaseOrderListExpression PurchaseOrderList()
        {
            var path = _path.Member("purchase_order_list", "PurchaseOrderList");
            if (_notLoaded != null) return new PurchaseOrderListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return PurchaseOrderListExpression.Missing(_root, path);
            if (!_value.IsLoaded("PurchaseOrderList"))
                return new PurchaseOrderListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "purchase_order_list"));
            return new PurchaseOrderListExpression(_value.PurchaseOrderList, _root, path);
        }

        public OrderItemListExpression OrderItemList()
        {
            var path = _path.Member("order_item_list", "OrderItemList");
            if (_notLoaded != null) return new OrderItemListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return OrderItemListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderItemList"))
                return new OrderItemListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "order_item_list"));
            return new OrderItemListExpression(_value.OrderItemList, _root, path);
        }
    }

    public sealed class PurchaseOrderExpression
    {
        private readonly Generated.Models.PurchaseOrder? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PurchaseOrderExpression(
            Generated.Models.PurchaseOrder? value,
            string root = "PurchaseOrder(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.PurchaseOrder(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.PurchaseOrder? Eval()
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

        public ValueExpression<string?> ContactPhone()
        {
            var path = _path.Member("contact_phone", "ContactPhone");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ContactPhone"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "contact_phone"));
            return new ValueExpression<string?>(_value.ContactPhone);
        }

        public ValueExpression<string?> InternalNote()
        {
            var path = _path.Member("internal_note", "InternalNote");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("InternalNote"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "internal_note"));
            return new ValueExpression<string?>(_value.InternalNote);
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

        public ValueExpression<long?> TenantId()
        {
            var path = _path.Member("tenant", "TenantId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Tenant"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "tenant"));
            return new ValueExpression<long?>(_value.Tenant);
        }

        public TenantExpression Tenant()
        {
            var path = _path.Member("tenant", "Tenant");
            if (_notLoaded != null) return new TenantExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new TenantExpression(null, _root, path);
            if (!_value.IsLoaded("TenantEntity"))
                return new TenantExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "tenant"));
            return new TenantExpression(_value.TenantEntity, _root, path);
        }

        public OrderItemListExpression OrderItemList()
        {
            var path = _path.Member("order_item_list", "OrderItemList");
            if (_notLoaded != null) return new OrderItemListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return OrderItemListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderItemList"))
                return new OrderItemListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "order_item_list"));
            return new OrderItemListExpression(_value.OrderItemList, _root, path);
        }
    }

    public sealed class OrderItemExpression
    {
        private readonly Generated.Models.OrderItem? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderItemExpression(
            Generated.Models.OrderItem? value,
            string root = "OrderItem(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.OrderItem(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderItem? Eval()
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

        public ValueExpression<string?> ProductName()
        {
            var path = _path.Member("product_name", "ProductName");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ProductName"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "product_name"));
            return new ValueExpression<string?>(_value.ProductName);
        }

        public ValueExpression<decimal?> UnitPrice()
        {
            var path = _path.Member("unit_price", "UnitPrice");
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<decimal?>.Missing();
            if (!_value.IsLoaded("UnitPrice"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "unit_price"));
            return new ValueExpression<decimal?>(_value.UnitPrice);
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

        public ValueExpression<long?> PurchaseOrderId()
        {
            var path = _path.Member("purchase_order", "PurchaseOrderId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("PurchaseOrder"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "purchase_order"));
            return new ValueExpression<long?>(_value.PurchaseOrder);
        }

        public ValueExpression<long?> TenantId()
        {
            var path = _path.Member("tenant", "TenantId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Tenant"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "tenant"));
            return new ValueExpression<long?>(_value.Tenant);
        }

        public PurchaseOrderExpression PurchaseOrder()
        {
            var path = _path.Member("purchase_order", "PurchaseOrder");
            if (_notLoaded != null) return new PurchaseOrderExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new PurchaseOrderExpression(null, _root, path);
            if (!_value.IsLoaded("PurchaseOrderEntity"))
                return new PurchaseOrderExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "purchase_order"));
            return new PurchaseOrderExpression(_value.PurchaseOrderEntity, _root, path);
        }

        public TenantExpression Tenant()
        {
            var path = _path.Member("tenant", "Tenant");
            if (_notLoaded != null) return new TenantExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new TenantExpression(null, _root, path);
            if (!_value.IsLoaded("TenantEntity"))
                return new TenantExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "tenant"));
            return new TenantExpression(_value.TenantEntity, _root, path);
        }

    }

    public sealed class TenantListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Tenant> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public TenantListExpression(
            IReadOnlyList<Generated.Models.Tenant>? items,
            string? root = "Tenant(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Tenant>();
            _root = root ?? "Tenant(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static TenantListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Tenant>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public TenantExpression First() => Item(0, true);

        public TenantExpression Get(int index)
        {
            return Item(index, false);
        }

        private TenantExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new TenantExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new TenantExpression(null, _root, itemPath)
                : new TenantExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class PurchaseOrderListExpression
    {
        private readonly IReadOnlyList<Generated.Models.PurchaseOrder> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PurchaseOrderListExpression(
            IReadOnlyList<Generated.Models.PurchaseOrder>? items,
            string? root = "PurchaseOrder(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.PurchaseOrder>();
            _root = root ?? "PurchaseOrder(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static PurchaseOrderListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.PurchaseOrder>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public PurchaseOrderExpression First() => Item(0, true);

        public PurchaseOrderExpression Get(int index)
        {
            return Item(index, false);
        }

        private PurchaseOrderExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new PurchaseOrderExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new PurchaseOrderExpression(null, _root, itemPath)
                : new PurchaseOrderExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class OrderItemListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderItem> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderItemListExpression(
            IReadOnlyList<Generated.Models.OrderItem>? items,
            string? root = "OrderItem(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderItem>();
            _root = root ?? "OrderItem(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderItemListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.OrderItem>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public OrderItemExpression First() => Item(0, true);

        public OrderItemExpression Get(int index)
        {
            return Item(index, false);
        }

        private OrderItemExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new OrderItemExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new OrderItemExpression(null, _root, itemPath)
                : new OrderItemExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static TenantExpression Tenant(Generated.Models.Tenant? value)
        {
            return new TenantExpression(value, "Tenant");
        }

        public static PurchaseOrderExpression PurchaseOrder(Generated.Models.PurchaseOrder? value)
        {
            return new PurchaseOrderExpression(value, "PurchaseOrder");
        }

        public static OrderItemExpression OrderItem(Generated.Models.OrderItem? value)
        {
            return new OrderItemExpression(value, "OrderItem");
        }
    }
}