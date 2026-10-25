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

    public sealed class CommercePlatformExpression
    {
        private readonly Generated.Models.CommercePlatform? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CommercePlatformExpression(
            Generated.Models.CommercePlatform? value,
            string root = "CommercePlatform(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.CommercePlatform(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.CommercePlatform? Eval()
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

        public ProductListExpression ProductList()
        {
            var path = _path.Member("product_list", "ProductList");
            if (_notLoaded != null) return new ProductListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return ProductListExpression.Missing(_root, path);
            if (!_value.IsLoaded("ProductList"))
                return new ProductListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "product_list"));
            return new ProductListExpression(_value.ProductList, _root, path);
        }

        public OrderLineListExpression OrderLineList()
        {
            var path = _path.Member("order_line_list", "OrderLineList");
            if (_notLoaded != null) return new OrderLineListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return OrderLineListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderLineList"))
                return new OrderLineListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "order_line_list"));
            return new OrderLineListExpression(_value.OrderLineList, _root, path);
        }

        public OrderSearchPresetListExpression OrderSearchPresetList()
        {
            var path = _path.Member("order_search_preset_list", "OrderSearchPresetList");
            if (_notLoaded != null) return new OrderSearchPresetListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return OrderSearchPresetListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderSearchPresetList"))
                return new OrderSearchPresetListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "order_search_preset_list"));
            return new OrderSearchPresetListExpression(_value.OrderSearchPresetList, _root, path);
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

        public ValueExpression<string?> Email()
        {
            var path = _path.Member("email", "Email");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Email"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "email"));
            return new ValueExpression<string?>(_value.Email);
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

        public ValueExpression<long?> CommercePlatformId()
        {
            var path = _path.Member("commerce_platform", "CommercePlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public CommercePlatformExpression CommercePlatform()
        {
            var path = _path.Member("commerce_platform", "CommercePlatform");
            if (_notLoaded != null) return new CommercePlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CommercePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("CommercePlatformEntity"))
                return new CommercePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new CommercePlatformExpression(_value.CommercePlatformEntity, _root, path);
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

        public ValueExpression<long?> CommercePlatformId()
        {
            var path = _path.Member("commerce_platform", "CommercePlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public CommercePlatformExpression CommercePlatform()
        {
            var path = _path.Member("commerce_platform", "CommercePlatform");
            if (_notLoaded != null) return new CommercePlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CommercePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("CommercePlatformEntity"))
                return new CommercePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new CommercePlatformExpression(_value.CommercePlatformEntity, _root, path);
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

        public ValueExpression<long?> StatusId()
        {
            var path = _path.Member("status", "StatusId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Status"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "status"));
            return new ValueExpression<long?>(_value.Status);
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

        public ValueExpression<long?> CommercePlatformId()
        {
            var path = _path.Member("commerce_platform", "CommercePlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public OrderStatusExpression Status()
        {
            var path = _path.Member("status", "Status");
            if (_notLoaded != null) return new OrderStatusExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new OrderStatusExpression(null, _root, path);
            if (!_value.IsLoaded("StatusEntity"))
                return new OrderStatusExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "status"));
            return new OrderStatusExpression(_value.StatusEntity, _root, path);
        }

        public CustomerExpression Customer()
        {
            var path = _path.Member("customer", "Customer");
            if (_notLoaded != null) return new CustomerExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CustomerExpression(null, _root, path);
            if (!_value.IsLoaded("CustomerEntity"))
                return new CustomerExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer"));
            return new CustomerExpression(_value.CustomerEntity, _root, path);
        }

        public CommercePlatformExpression CommercePlatform()
        {
            var path = _path.Member("commerce_platform", "CommercePlatform");
            if (_notLoaded != null) return new CommercePlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CommercePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("CommercePlatformEntity"))
                return new CommercePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new CommercePlatformExpression(_value.CommercePlatformEntity, _root, path);
        }

        public OrderLineListExpression OrderLineList()
        {
            var path = _path.Member("order_line_list", "OrderLineList");
            if (_notLoaded != null) return new OrderLineListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return OrderLineListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderLineList"))
                return new OrderLineListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "order_line_list"));
            return new OrderLineListExpression(_value.OrderLineList, _root, path);
        }
    }

    public sealed class ProductExpression
    {
        private readonly Generated.Models.Product? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ProductExpression(
            Generated.Models.Product? value,
            string root = "Product(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Product(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Product? Eval()
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

        public ValueExpression<string?> Sku()
        {
            var path = _path.Member("sku", "Sku");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Sku"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "sku"));
            return new ValueExpression<string?>(_value.Sku);
        }

        public ValueExpression<string?> ImageUrl()
        {
            var path = _path.Member("image_url", "ImageUrl");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ImageUrl"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "image_url"));
            return new ValueExpression<string?>(_value.ImageUrl);
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

        public ValueExpression<long?> CommercePlatformId()
        {
            var path = _path.Member("commerce_platform", "CommercePlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public CommercePlatformExpression CommercePlatform()
        {
            var path = _path.Member("commerce_platform", "CommercePlatform");
            if (_notLoaded != null) return new CommercePlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CommercePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("CommercePlatformEntity"))
                return new CommercePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new CommercePlatformExpression(_value.CommercePlatformEntity, _root, path);
        }

        public OrderLineListExpression OrderLineList()
        {
            var path = _path.Member("order_line_list", "OrderLineList");
            if (_notLoaded != null) return new OrderLineListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return OrderLineListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderLineList"))
                return new OrderLineListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "order_line_list"));
            return new OrderLineListExpression(_value.OrderLineList, _root, path);
        }
    }

    public sealed class OrderLineExpression
    {
        private readonly Generated.Models.OrderLine? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderLineExpression(
            Generated.Models.OrderLine? value,
            string root = "OrderLine(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.OrderLine(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderLine? Eval()
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

        public ValueExpression<System.DateTime?> CreateTime()
        {
            var path = _path.Member("create_time", "CreateTime");
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            if (!_value.IsLoaded("CreateTime"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "create_time"));
            return new ValueExpression<System.DateTime?>(_value.CreateTime);
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

        public ValueExpression<long?> ProductId()
        {
            var path = _path.Member("product", "ProductId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Product"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "product"));
            return new ValueExpression<long?>(_value.Product);
        }

        public ValueExpression<long?> CommercePlatformId()
        {
            var path = _path.Member("commerce_platform", "CommercePlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public CustomerOrderExpression CustomerOrder()
        {
            var path = _path.Member("customer_order", "CustomerOrder");
            if (_notLoaded != null) return new CustomerOrderExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CustomerOrderExpression(null, _root, path);
            if (!_value.IsLoaded("CustomerOrderEntity"))
                return new CustomerOrderExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_order"));
            return new CustomerOrderExpression(_value.CustomerOrderEntity, _root, path);
        }

        public ProductExpression Product()
        {
            var path = _path.Member("product", "Product");
            if (_notLoaded != null) return new ProductExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new ProductExpression(null, _root, path);
            if (!_value.IsLoaded("ProductEntity"))
                return new ProductExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "product"));
            return new ProductExpression(_value.ProductEntity, _root, path);
        }

        public CommercePlatformExpression CommercePlatform()
        {
            var path = _path.Member("commerce_platform", "CommercePlatform");
            if (_notLoaded != null) return new CommercePlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CommercePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("CommercePlatformEntity"))
                return new CommercePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new CommercePlatformExpression(_value.CommercePlatformEntity, _root, path);
        }

    }

    public sealed class OrderSearchPresetExpression
    {
        private readonly Generated.Models.OrderSearchPreset? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderSearchPresetExpression(
            Generated.Models.OrderSearchPreset? value,
            string root = "OrderSearchPreset(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.OrderSearchPreset(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderSearchPreset? Eval()
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

        public ValueExpression<string?> FilterJson()
        {
            var path = _path.Member("filter_json", "FilterJson");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("FilterJson"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "filter_json"));
            return new ValueExpression<string?>(_value.FilterJson);
        }

        public ValueExpression<string?> RequestId()
        {
            var path = _path.Member("request_id", "RequestId");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("RequestId"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "request_id"));
            return new ValueExpression<string?>(_value.RequestId);
        }

        public ValueExpression<string?> OwnerUserId()
        {
            var path = _path.Member("owner_user_id", "OwnerUserId");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("OwnerUserId"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "owner_user_id"));
            return new ValueExpression<string?>(_value.OwnerUserId);
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

        public ValueExpression<long?> CommercePlatformId()
        {
            var path = _path.Member("commerce_platform", "CommercePlatformId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public CommercePlatformExpression CommercePlatform()
        {
            var path = _path.Member("commerce_platform", "CommercePlatform");
            if (_notLoaded != null) return new CommercePlatformExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CommercePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("CommercePlatformEntity"))
                return new CommercePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new CommercePlatformExpression(_value.CommercePlatformEntity, _root, path);
        }

    }

    public sealed class CommercePlatformListExpression
    {
        private readonly IReadOnlyList<Generated.Models.CommercePlatform> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CommercePlatformListExpression(
            IReadOnlyList<Generated.Models.CommercePlatform>? items,
            string? root = "CommercePlatform(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.CommercePlatform>();
            _root = root ?? "CommercePlatform(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CommercePlatformListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.CommercePlatform>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public CommercePlatformExpression First() => Item(0, true);

        public CommercePlatformExpression Get(int index)
        {
            return Item(index, false);
        }

        private CommercePlatformExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new CommercePlatformExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new CommercePlatformExpression(null, _root, itemPath)
                : new CommercePlatformExpression(_items[index], _root, itemPath);
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

    public sealed class ProductListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Product> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ProductListExpression(
            IReadOnlyList<Generated.Models.Product>? items,
            string? root = "Product(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Product>();
            _root = root ?? "Product(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static ProductListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Product>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public ProductExpression First() => Item(0, true);

        public ProductExpression Get(int index)
        {
            return Item(index, false);
        }

        private ProductExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new ProductExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new ProductExpression(null, _root, itemPath)
                : new ProductExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class OrderLineListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderLine> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderLineListExpression(
            IReadOnlyList<Generated.Models.OrderLine>? items,
            string? root = "OrderLine(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderLine>();
            _root = root ?? "OrderLine(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderLineListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.OrderLine>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public OrderLineExpression First() => Item(0, true);

        public OrderLineExpression Get(int index)
        {
            return Item(index, false);
        }

        private OrderLineExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new OrderLineExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new OrderLineExpression(null, _root, itemPath)
                : new OrderLineExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class OrderSearchPresetListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderSearchPreset> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderSearchPresetListExpression(
            IReadOnlyList<Generated.Models.OrderSearchPreset>? items,
            string? root = "OrderSearchPreset(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderSearchPreset>();
            _root = root ?? "OrderSearchPreset(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderSearchPresetListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.OrderSearchPreset>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public OrderSearchPresetExpression First() => Item(0, true);

        public OrderSearchPresetExpression Get(int index)
        {
            return Item(index, false);
        }

        private OrderSearchPresetExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new OrderSearchPresetExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new OrderSearchPresetExpression(null, _root, itemPath)
                : new OrderSearchPresetExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static CommercePlatformExpression CommercePlatform(Generated.Models.CommercePlatform? value)
        {
            return new CommercePlatformExpression(value, "CommercePlatform");
        }

        public static CustomerExpression Customer(Generated.Models.Customer? value)
        {
            return new CustomerExpression(value, "Customer");
        }

        public static OrderStatusExpression OrderStatus(Generated.Models.OrderStatus? value)
        {
            return new OrderStatusExpression(value, "OrderStatus");
        }

        public static CustomerOrderExpression CustomerOrder(Generated.Models.CustomerOrder? value)
        {
            return new CustomerOrderExpression(value, "CustomerOrder");
        }

        public static ProductExpression Product(Generated.Models.Product? value)
        {
            return new ProductExpression(value, "Product");
        }

        public static OrderLineExpression OrderLine(Generated.Models.OrderLine? value)
        {
            return new OrderLineExpression(value, "OrderLine");
        }

        public static OrderSearchPresetExpression OrderSearchPreset(Generated.Models.OrderSearchPreset? value)
        {
            return new OrderSearchPresetExpression(value, "OrderSearchPreset");
        }
    }
}