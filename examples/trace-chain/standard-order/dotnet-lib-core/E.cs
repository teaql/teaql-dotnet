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

        public ValueExpression<long?> Version()
        {
            var path = _path.Member("version", "Version");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "version"));
            return new ValueExpression<long?>(_value.Version);
        }



        public OrderStatusListExpression OrderStatusList()
        {
            var path = _path.Member("order_status_list", "OrderStatusList");
            if (_notLoaded != null) return new OrderStatusListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return OrderStatusListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderStatusList"))
                return new OrderStatusListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "order_status_list"));
            return new OrderStatusListExpression(_value.OrderStatusList, _root, path);
        }

        public CustomerListExpression CustomerList()
        {
            var path = _path.Member("customer_list", "CustomerList");
            if (_notLoaded != null) return new CustomerListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return CustomerListExpression.Missing(_root, path);
            if (!_value.IsLoaded("CustomerList"))
                return new CustomerListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_list"));
            return new CustomerListExpression(_value.CustomerList, _root, path);
        }
    }

    public sealed class OrderStatusExpression
    {
        private readonly Generated.Models.OrderStatus? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderStatusExpression(
            Generated.Models.OrderStatus? value,
            string root = "OrderStatus(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.OrderStatus(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderStatus? Eval()
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

        public CustomerOrderListExpression CustomerOrderList()
        {
            var path = _path.Member("customer_order_list", "CustomerOrderList");
            if (_notLoaded != null) return new CustomerOrderListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return CustomerOrderListExpression.Missing(_root, path);
            if (!_value.IsLoaded("CustomerOrderList"))
                return new CustomerOrderListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_order_list"));
            return new CustomerOrderListExpression(_value.CustomerOrderList, _root, path);
        }
    }

    public sealed class CustomerExpression
    {
        private readonly Generated.Models.Customer? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerExpression(
            Generated.Models.Customer? value,
            string root = "Customer(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Customer(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Customer? Eval()
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

        public CustomerOrderListExpression CustomerOrderList()
        {
            var path = _path.Member("customer_order_list", "CustomerOrderList");
            if (_notLoaded != null) return new CustomerOrderListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return CustomerOrderListExpression.Missing(_root, path);
            if (!_value.IsLoaded("CustomerOrderList"))
                return new CustomerOrderListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_order_list"));
            return new CustomerOrderListExpression(_value.CustomerOrderList, _root, path);
        }
    }

    public sealed class CustomerOrderExpression
    {
        private readonly Generated.Models.CustomerOrder? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerOrderExpression(
            Generated.Models.CustomerOrder? value,
            string root = "CustomerOrder(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.CustomerOrder(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.CustomerOrder? Eval()
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

        public ValueExpression<string?> OrderNumber()
        {
            var path = _path.Member("order_number", "OrderNumber");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("OrderNumber"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "order_number"));
            return new ValueExpression<string?>(_value.OrderNumber);
        }

        public ValueExpression<System.DateTime?> OrderDate()
        {
            var path = _path.Member("order_date", "OrderDate");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("OrderDate"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "order_date"));
            return new ValueExpression<System.DateTime?>(_value.OrderDate);
        }

        public ValueExpression<decimal?> TotalAmount()
        {
            var path = _path.Member("total_amount", "TotalAmount");
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<decimal?>.Missing();
            if (!_value.IsLoaded("TotalAmount"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "total_amount"));
            return new ValueExpression<decimal?>(_value.TotalAmount);
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

        public ValueExpression<long?> CustomerId()
        {
            var path = _path.Member("customer", "CustomerId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Customer"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "customer"));
            return new ValueExpression<long?>(_value.Customer);
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

        public CustomerExpression Customer()
        {
            var path = _path.Member("customer", "Customer");
            if (_notLoaded != null) return new CustomerExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CustomerExpression(null, _root, path);
            if (!_value.IsLoaded("Customer"))
                return new CustomerExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer"));
            if (_value.Customer is null) return new CustomerExpression(null, _root, path);
            if (!_value.IsLoaded("CustomerEntity") || _value.CustomerEntity is null
                || _value.CustomerEntity.Id != _value.Customer)
                return new CustomerExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer"));
            return new CustomerExpression(_value.CustomerEntity, _root, path);
        }

        public OrderStatusExpression Status()
        {
            var path = _path.Member("status", "Status");
            if (_notLoaded != null) return new OrderStatusExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new OrderStatusExpression(null, _root, path);
            if (!_value.IsLoaded("Status"))
                return new OrderStatusExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "status"));
            if (_value.Status is null) return new OrderStatusExpression(null, _root, path);
            if (!_value.IsLoaded("StatusEntity") || _value.StatusEntity is null
                || _value.StatusEntity.Id != _value.Status)
                return new OrderStatusExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "status"));
            return new OrderStatusExpression(_value.StatusEntity, _root, path);
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

        public ValueExpression<long?> Quantity()
        {
            var path = _path.Member("quantity", "Quantity");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Quantity"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "quantity"));
            return new ValueExpression<long?>(_value.Quantity);
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

        public ValueExpression<long?> Version()
        {
            var path = _path.Member("version", "Version");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "version"));
            return new ValueExpression<long?>(_value.Version);
        }

        public ValueExpression<long?> CustomerOrderId()
        {
            var path = _path.Member("customer_order", "CustomerOrderId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("CustomerOrder"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "customer_order"));
            return new ValueExpression<long?>(_value.CustomerOrder);
        }

        public CustomerOrderExpression CustomerOrder()
        {
            var path = _path.Member("customer_order", "CustomerOrder");
            if (_notLoaded != null) return new CustomerOrderExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CustomerOrderExpression(null, _root, path);
            if (!_value.IsLoaded("CustomerOrder"))
                return new CustomerOrderExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_order"));
            if (_value.CustomerOrder is null) return new CustomerOrderExpression(null, _root, path);
            if (!_value.IsLoaded("CustomerOrderEntity") || _value.CustomerOrderEntity is null
                || _value.CustomerOrderEntity.Id != _value.CustomerOrder)
                return new CustomerOrderExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_order"));
            return new CustomerOrderExpression(_value.CustomerOrderEntity, _root, path);
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

    public sealed class OrderStatusListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderStatus> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderStatusListExpression(
            IReadOnlyList<Generated.Models.OrderStatus>? items,
            string? root = "OrderStatus(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderStatus>();
            _root = root ?? "OrderStatus(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderStatusListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.OrderStatus>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public OrderStatusExpression First() => Item(0, true);

        public OrderStatusExpression Get(int index)
        {
            return Item(index, false);
        }

        private OrderStatusExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new OrderStatusExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new OrderStatusExpression(null, _root, itemPath)
                : new OrderStatusExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class CustomerListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Customer> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerListExpression(
            IReadOnlyList<Generated.Models.Customer>? items,
            string? root = "Customer(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Customer>();
            _root = root ?? "Customer(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CustomerListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Customer>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public CustomerExpression First() => Item(0, true);

        public CustomerExpression Get(int index)
        {
            return Item(index, false);
        }

        private CustomerExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new CustomerExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new CustomerExpression(null, _root, itemPath)
                : new CustomerExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class CustomerOrderListExpression
    {
        private readonly IReadOnlyList<Generated.Models.CustomerOrder> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerOrderListExpression(
            IReadOnlyList<Generated.Models.CustomerOrder>? items,
            string? root = "CustomerOrder(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.CustomerOrder>();
            _root = root ?? "CustomerOrder(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CustomerOrderListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.CustomerOrder>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public CustomerOrderExpression First() => Item(0, true);

        public CustomerOrderExpression Get(int index)
        {
            return Item(index, false);
        }

        private CustomerOrderExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new CustomerOrderExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new CustomerOrderExpression(null, _root, itemPath)
                : new CustomerOrderExpression(_items[index], _root, itemPath);
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
        public static PlatformExpression Platform(Generated.Models.Platform? value)
        {
            return new PlatformExpression(value, "Platform");
        }

        public static OrderStatusExpression OrderStatus(Generated.Models.OrderStatus? value)
        {
            return new OrderStatusExpression(value, "OrderStatus");
        }

        public static CustomerExpression Customer(Generated.Models.Customer? value)
        {
            return new CustomerExpression(value, "Customer");
        }

        public static CustomerOrderExpression CustomerOrder(Generated.Models.CustomerOrder? value)
        {
            return new CustomerOrderExpression(value, "CustomerOrder");
        }

        public static OrderItemExpression OrderItem(Generated.Models.OrderItem? value)
        {
            return new OrderItemExpression(value, "OrderItem");
        }
    }
}