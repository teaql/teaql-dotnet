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

        public T OrElse(T fallback)
        {
            var value = Eval();
            return _hasValue && value is not null ? value : fallback;
        }

        public static ValueExpression<T> Missing() => new(default!, false);
        public static ValueExpression<T> NotLoaded(TeaQLNotLoadedException error) => new(default!, false, error);
    }

    public sealed class CommercePlatformExpression
    {
        private readonly Generated.Models.CommercePlatform _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public CommercePlatformExpression(
            Generated.Models.CommercePlatform value,
            string root = "CommercePlatform(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.CommercePlatform Eval()
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


        public CustomerListExpression CustomerList()
        {
            var path = ExpressionPath.Append(_path, "CustomerList");
            if (_notLoaded != null) return new CustomerListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return CustomerListExpression.Missing(_root, path);
            if (!_value.IsLoaded("CustomerList"))
                return new CustomerListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "CustomerList"));
            return new CustomerListExpression(_value.CustomerList, _root, path);
        }

        public OrderStatusListExpression OrderStatusList()
        {
            var path = ExpressionPath.Append(_path, "OrderStatusList");
            if (_notLoaded != null) return new OrderStatusListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return OrderStatusListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderStatusList"))
                return new OrderStatusListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "OrderStatusList"));
            return new OrderStatusListExpression(_value.OrderStatusList, _root, path);
        }

        public CustomerOrderListExpression CustomerOrderList()
        {
            var path = ExpressionPath.Append(_path, "CustomerOrderList");
            if (_notLoaded != null) return new CustomerOrderListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return CustomerOrderListExpression.Missing(_root, path);
            if (!_value.IsLoaded("CustomerOrderList"))
                return new CustomerOrderListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "CustomerOrderList"));
            return new CustomerOrderListExpression(_value.CustomerOrderList, _root, path);
        }

        public ProductListExpression ProductList()
        {
            var path = ExpressionPath.Append(_path, "ProductList");
            if (_notLoaded != null) return new ProductListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return ProductListExpression.Missing(_root, path);
            if (!_value.IsLoaded("ProductList"))
                return new ProductListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "ProductList"));
            return new ProductListExpression(_value.ProductList, _root, path);
        }

        public OrderLineListExpression OrderLineList()
        {
            var path = ExpressionPath.Append(_path, "OrderLineList");
            if (_notLoaded != null) return new OrderLineListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return OrderLineListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderLineList"))
                return new OrderLineListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "OrderLineList"));
            return new OrderLineListExpression(_value.OrderLineList, _root, path);
        }

        public OrderSearchPresetListExpression OrderSearchPresetList()
        {
            var path = ExpressionPath.Append(_path, "OrderSearchPresetList");
            if (_notLoaded != null) return new OrderSearchPresetListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return OrderSearchPresetListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderSearchPresetList"))
                return new OrderSearchPresetListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "OrderSearchPresetList"));
            return new OrderSearchPresetListExpression(_value.OrderSearchPresetList, _root, path);
        }
    }

    public sealed class CustomerExpression
    {
        private readonly Generated.Models.Customer _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public CustomerExpression(
            Generated.Models.Customer value,
            string root = "Customer(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.Customer Eval()
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

        public ValueExpression<string> Email()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "Email");
            if (!_value.IsLoaded("Email"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Email"));
            return new ValueExpression<string>(_value.Email);
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

        public ValueExpression<long?> CommercePlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "CommercePlatform");
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "CommercePlatform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public CustomerOrderListExpression CustomerOrderList()
        {
            var path = ExpressionPath.Append(_path, "CustomerOrderList");
            if (_notLoaded != null) return new CustomerOrderListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return CustomerOrderListExpression.Missing(_root, path);
            if (!_value.IsLoaded("CustomerOrderList"))
                return new CustomerOrderListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "CustomerOrderList"));
            return new CustomerOrderListExpression(_value.CustomerOrderList, _root, path);
        }
    }

    public sealed class OrderStatusExpression
    {
        private readonly Generated.Models.OrderStatus _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public OrderStatusExpression(
            Generated.Models.OrderStatus value,
            string root = "OrderStatus(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderStatus Eval()
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

        public ValueExpression<string> Color()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "Color");
            if (!_value.IsLoaded("Color"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Color"));
            return new ValueExpression<string>(_value.Color);
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

        public ValueExpression<long?> CommercePlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "CommercePlatform");
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "CommercePlatform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public CustomerOrderListExpression CustomerOrderList()
        {
            var path = ExpressionPath.Append(_path, "CustomerOrderList");
            if (_notLoaded != null) return new CustomerOrderListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return CustomerOrderListExpression.Missing(_root, path);
            if (!_value.IsLoaded("CustomerOrderList"))
                return new CustomerOrderListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "CustomerOrderList"));
            return new CustomerOrderListExpression(_value.CustomerOrderList, _root, path);
        }
    }

    public sealed class CustomerOrderExpression
    {
        private readonly Generated.Models.CustomerOrder _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public CustomerOrderExpression(
            Generated.Models.CustomerOrder value,
            string root = "CustomerOrder(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.CustomerOrder Eval()
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

        public ValueExpression<string> OrderNumber()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "OrderNumber");
            if (!_value.IsLoaded("OrderNumber"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OrderNumber"));
            return new ValueExpression<string>(_value.OrderNumber);
        }

        public ValueExpression<System.DateTime?> OrderDate()
        {
            if (_notLoaded != null) return ValueExpression<System.DateTime?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<System.DateTime?>.Missing();
            var path = ExpressionPath.Append(_path, "OrderDate");
            if (!_value.IsLoaded("OrderDate"))
                return ValueExpression<System.DateTime?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OrderDate"));
            return new ValueExpression<System.DateTime?>(_value.OrderDate);
        }

        public ValueExpression<decimal?> TotalAmount()
        {
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<decimal?>.Missing();
            var path = ExpressionPath.Append(_path, "TotalAmount");
            if (!_value.IsLoaded("TotalAmount"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "TotalAmount"));
            return new ValueExpression<decimal?>(_value.TotalAmount);
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

        public ValueExpression<long?> StatusId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Status");
            if (!_value.IsLoaded("Status"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Status"));
            return new ValueExpression<long?>(_value.Status);
        }

        public ValueExpression<long?> CustomerId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Customer");
            if (!_value.IsLoaded("Customer"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Customer"));
            return new ValueExpression<long?>(_value.Customer);
        }

        public ValueExpression<long?> CommercePlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "CommercePlatform");
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "CommercePlatform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public OrderLineListExpression OrderLineList()
        {
            var path = ExpressionPath.Append(_path, "OrderLineList");
            if (_notLoaded != null) return new OrderLineListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return OrderLineListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderLineList"))
                return new OrderLineListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "OrderLineList"));
            return new OrderLineListExpression(_value.OrderLineList, _root, path);
        }
    }

    public sealed class ProductExpression
    {
        private readonly Generated.Models.Product _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public ProductExpression(
            Generated.Models.Product value,
            string root = "Product(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.Product Eval()
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

        public ValueExpression<string> Sku()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "Sku");
            if (!_value.IsLoaded("Sku"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Sku"));
            return new ValueExpression<string>(_value.Sku);
        }

        public ValueExpression<string> ImageUrl()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "ImageUrl");
            if (!_value.IsLoaded("ImageUrl"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "ImageUrl"));
            return new ValueExpression<string>(_value.ImageUrl);
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

        public ValueExpression<long?> CommercePlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "CommercePlatform");
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "CommercePlatform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

        public OrderLineListExpression OrderLineList()
        {
            var path = ExpressionPath.Append(_path, "OrderLineList");
            if (_notLoaded != null) return new OrderLineListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return OrderLineListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderLineList"))
                return new OrderLineListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "OrderLineList"));
            return new OrderLineListExpression(_value.OrderLineList, _root, path);
        }
    }

    public sealed class OrderLineExpression
    {
        private readonly Generated.Models.OrderLine _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public OrderLineExpression(
            Generated.Models.OrderLine value,
            string root = "OrderLine(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderLine Eval()
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

        public ValueExpression<string> ProductName()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "ProductName");
            if (!_value.IsLoaded("ProductName"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "ProductName"));
            return new ValueExpression<string>(_value.ProductName);
        }

        public ValueExpression<string> Sku()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "Sku");
            if (!_value.IsLoaded("Sku"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Sku"));
            return new ValueExpression<string>(_value.Sku);
        }

        public ValueExpression<long?> Quantity()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Quantity");
            if (!_value.IsLoaded("Quantity"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Quantity"));
            return new ValueExpression<long?>(_value.Quantity);
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

        public ValueExpression<long?> Version()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Version");
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Version"));
            return new ValueExpression<long?>(_value.Version);
        }

        public ValueExpression<long?> CustomerOrderId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "CustomerOrder");
            if (!_value.IsLoaded("CustomerOrder"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "CustomerOrder"));
            return new ValueExpression<long?>(_value.CustomerOrder);
        }

        public ValueExpression<long?> ProductId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Product");
            if (!_value.IsLoaded("Product"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Product"));
            return new ValueExpression<long?>(_value.Product);
        }

        public ValueExpression<long?> CommercePlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "CommercePlatform");
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "CommercePlatform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

    }

    public sealed class OrderSearchPresetExpression
    {
        private readonly Generated.Models.OrderSearchPreset _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException _notLoaded;

        public OrderSearchPresetExpression(
            Generated.Models.OrderSearchPreset value,
            string root = "OrderSearchPreset(null)",
            string path = "",
            TeaQLNotLoadedException notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderSearchPreset Eval()
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

        public ValueExpression<string> FilterJson()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "FilterJson");
            if (!_value.IsLoaded("FilterJson"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "FilterJson"));
            return new ValueExpression<string>(_value.FilterJson);
        }

        public ValueExpression<string> RequestId()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "RequestId");
            if (!_value.IsLoaded("RequestId"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "RequestId"));
            return new ValueExpression<string>(_value.RequestId);
        }

        public ValueExpression<string> OwnerUserId()
        {
            if (_notLoaded != null) return ValueExpression<string>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string>.Missing();
            var path = ExpressionPath.Append(_path, "OwnerUserId");
            if (!_value.IsLoaded("OwnerUserId"))
                return ValueExpression<string>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OwnerUserId"));
            return new ValueExpression<string>(_value.OwnerUserId);
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

        public ValueExpression<long?> CommercePlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "CommercePlatform");
            if (!_value.IsLoaded("CommercePlatform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "CommercePlatform"));
            return new ValueExpression<long?>(_value.CommercePlatform);
        }

    }

    public sealed class CommercePlatformListExpression
    {
        private readonly IReadOnlyList<Generated.Models.CommercePlatform> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public CommercePlatformListExpression(
            IReadOnlyList<Generated.Models.CommercePlatform> items,
            string root = "CommercePlatform(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.CommercePlatform>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CommercePlatformListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.CommercePlatform>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public CommercePlatformExpression First() => Get(0);

        public CommercePlatformExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new CommercePlatformExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new CommercePlatformExpression(null, _root, itemPath)
                : new CommercePlatformExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class CustomerListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Customer> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public CustomerListExpression(
            IReadOnlyList<Generated.Models.Customer> items,
            string root = "Customer(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Customer>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CustomerListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.Customer>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public CustomerExpression First() => Get(0);

        public CustomerExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new CustomerExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new CustomerExpression(null, _root, itemPath)
                : new CustomerExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class OrderStatusListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderStatus> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public OrderStatusListExpression(
            IReadOnlyList<Generated.Models.OrderStatus> items,
            string root = "OrderStatus(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderStatus>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderStatusListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.OrderStatus>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public OrderStatusExpression First() => Get(0);

        public OrderStatusExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new OrderStatusExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new OrderStatusExpression(null, _root, itemPath)
                : new OrderStatusExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class CustomerOrderListExpression
    {
        private readonly IReadOnlyList<Generated.Models.CustomerOrder> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public CustomerOrderListExpression(
            IReadOnlyList<Generated.Models.CustomerOrder> items,
            string root = "CustomerOrder(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.CustomerOrder>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CustomerOrderListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.CustomerOrder>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public CustomerOrderExpression First() => Get(0);

        public CustomerOrderExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new CustomerOrderExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new CustomerOrderExpression(null, _root, itemPath)
                : new CustomerOrderExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class ProductListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Product> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public ProductListExpression(
            IReadOnlyList<Generated.Models.Product> items,
            string root = "Product(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Product>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static ProductListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.Product>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public ProductExpression First() => Get(0);

        public ProductExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new ProductExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new ProductExpression(null, _root, itemPath)
                : new ProductExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class OrderLineListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderLine> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public OrderLineListExpression(
            IReadOnlyList<Generated.Models.OrderLine> items,
            string root = "OrderLine(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderLine>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderLineListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.OrderLine>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public OrderLineExpression First() => Get(0);

        public OrderLineExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new OrderLineExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new OrderLineExpression(null, _root, itemPath)
                : new OrderLineExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class OrderSearchPresetListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderSearchPreset> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException _notLoaded;

        public OrderSearchPresetListExpression(
            IReadOnlyList<Generated.Models.OrderSearchPreset> items,
            string root = "OrderSearchPreset(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderSearchPreset>();
            _root = root;
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderSearchPresetListExpression Missing(string root = null, string path = "") =>
            new(new List<Generated.Models.OrderSearchPreset>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public OrderSearchPresetExpression First() => Get(0);

        public OrderSearchPresetExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new OrderSearchPresetExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new OrderSearchPresetExpression(null, _root, itemPath)
                : new OrderSearchPresetExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static CommercePlatformExpression CommercePlatform(Generated.Models.CommercePlatform value)
        {
            return new CommercePlatformExpression(value, $"CommercePlatform(id={value?.Id})");
        }

        public static CustomerExpression Customer(Generated.Models.Customer value)
        {
            return new CustomerExpression(value, $"Customer(id={value?.Id})");
        }

        public static OrderStatusExpression OrderStatus(Generated.Models.OrderStatus value)
        {
            return new OrderStatusExpression(value, $"OrderStatus(id={value?.Id})");
        }

        public static CustomerOrderExpression CustomerOrder(Generated.Models.CustomerOrder value)
        {
            return new CustomerOrderExpression(value, $"CustomerOrder(id={value?.Id})");
        }

        public static ProductExpression Product(Generated.Models.Product value)
        {
            return new ProductExpression(value, $"Product(id={value?.Id})");
        }

        public static OrderLineExpression OrderLine(Generated.Models.OrderLine value)
        {
            return new OrderLineExpression(value, $"OrderLine(id={value?.Id})");
        }

        public static OrderSearchPresetExpression OrderSearchPreset(Generated.Models.OrderSearchPreset value)
        {
            return new OrderSearchPresetExpression(value, $"OrderSearchPreset(id={value?.Id})");
        }
    }
}