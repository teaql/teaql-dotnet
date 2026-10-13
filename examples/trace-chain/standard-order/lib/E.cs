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

    public sealed class PlatformExpression
    {
        private readonly Generated.Models.Platform? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PlatformExpression(
            Generated.Models.Platform? value,
            string root = "Platform(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.Platform? Eval()
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
    }

    public sealed class OrderStatusExpression
    {
        private readonly Generated.Models.OrderStatus? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderStatusExpression(
            Generated.Models.OrderStatus? value,
            string root = "OrderStatus(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderStatus? Eval()
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

        public ValueExpression<string?> Code()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "Code");
            if (!_value.IsLoaded("Code"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Code"));
            return new ValueExpression<string?>(_value.Code);
        }

        public ValueExpression<string?> Color()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "Color");
            if (!_value.IsLoaded("Color"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Color"));
            return new ValueExpression<string?>(_value.Color);
        }

        public ValueExpression<long?> DisplayOrder()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "DisplayOrder");
            if (!_value.IsLoaded("DisplayOrder"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "DisplayOrder"));
            return new ValueExpression<long?>(_value.DisplayOrder);
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

        public ValueExpression<long?> PlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Platform");
            if (!_value.IsLoaded("Platform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Platform"));
            return new ValueExpression<long?>(_value.Platform);
        }

        public PlatformExpression Platform()
        {
            var path = ExpressionPath.Append(_path, "Platform");
            if (_notLoaded != null) return new PlatformExpression(null, _root, path, _notLoaded);
            if (_value is null) return new PlatformExpression(null, _root, path);
            if (!_value.IsLoaded("PlatformEntity"))
                return new PlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(_root, path, "Platform"));
            return new PlatformExpression(_value.PlatformEntity, _root, path);
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

    public sealed class CustomerExpression
    {
        private readonly Generated.Models.Customer? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerExpression(
            Generated.Models.Customer? value,
            string root = "Customer(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.Customer? Eval()
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

        public ValueExpression<long?> PlatformId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Platform");
            if (!_value.IsLoaded("Platform"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Platform"));
            return new ValueExpression<long?>(_value.Platform);
        }

        public PlatformExpression Platform()
        {
            var path = ExpressionPath.Append(_path, "Platform");
            if (_notLoaded != null) return new PlatformExpression(null, _root, path, _notLoaded);
            if (_value is null) return new PlatformExpression(null, _root, path);
            if (!_value.IsLoaded("PlatformEntity"))
                return new PlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(_root, path, "Platform"));
            return new PlatformExpression(_value.PlatformEntity, _root, path);
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
        private readonly Generated.Models.CustomerOrder? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerOrderExpression(
            Generated.Models.CustomerOrder? value,
            string root = "CustomerOrder(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.CustomerOrder? Eval()
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

        public ValueExpression<string?> OrderNumber()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "OrderNumber");
            if (!_value.IsLoaded("OrderNumber"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "OrderNumber"));
            return new ValueExpression<string?>(_value.OrderNumber);
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

        public ValueExpression<long?> Version()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Version");
            if (!_value.IsLoaded("Version"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Version"));
            return new ValueExpression<long?>(_value.Version);
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

        public ValueExpression<long?> StatusId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Status");
            if (!_value.IsLoaded("Status"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Status"));
            return new ValueExpression<long?>(_value.Status);
        }

        public CustomerExpression Customer()
        {
            var path = ExpressionPath.Append(_path, "Customer");
            if (_notLoaded != null) return new CustomerExpression(null, _root, path, _notLoaded);
            if (_value is null) return new CustomerExpression(null, _root, path);
            if (!_value.IsLoaded("CustomerEntity"))
                return new CustomerExpression(null, _root, path,
                    new TeaQLNotLoadedException(_root, path, "Customer"));
            return new CustomerExpression(_value.CustomerEntity, _root, path);
        }

        public OrderStatusExpression Status()
        {
            var path = ExpressionPath.Append(_path, "Status");
            if (_notLoaded != null) return new OrderStatusExpression(null, _root, path, _notLoaded);
            if (_value is null) return new OrderStatusExpression(null, _root, path);
            if (!_value.IsLoaded("StatusEntity"))
                return new OrderStatusExpression(null, _root, path,
                    new TeaQLNotLoadedException(_root, path, "Status"));
            return new OrderStatusExpression(_value.StatusEntity, _root, path);
        }

        public OrderItemListExpression OrderItemList()
        {
            var path = ExpressionPath.Append(_path, "OrderItemList");
            if (_notLoaded != null) return new OrderItemListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return OrderItemListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderItemList"))
                return new OrderItemListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "OrderItemList"));
            return new OrderItemListExpression(_value.OrderItemList, _root, path);
        }
    }

    public sealed class OrderItemExpression
    {
        private readonly Generated.Models.OrderItem? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderItemExpression(
            Generated.Models.OrderItem? value,
            string root = "OrderItem(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderItem? Eval()
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

        public ValueExpression<string?> ProductName()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "ProductName");
            if (!_value.IsLoaded("ProductName"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "ProductName"));
            return new ValueExpression<string?>(_value.ProductName);
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

        public ValueExpression<decimal?> UnitPrice()
        {
            if (_notLoaded != null) return ValueExpression<decimal?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<decimal?>.Missing();
            var path = ExpressionPath.Append(_path, "UnitPrice");
            if (!_value.IsLoaded("UnitPrice"))
                return ValueExpression<decimal?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "UnitPrice"));
            return new ValueExpression<decimal?>(_value.UnitPrice);
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

        public CustomerOrderExpression CustomerOrder()
        {
            var path = ExpressionPath.Append(_path, "CustomerOrder");
            if (_notLoaded != null) return new CustomerOrderExpression(null, _root, path, _notLoaded);
            if (_value is null) return new CustomerOrderExpression(null, _root, path);
            if (!_value.IsLoaded("CustomerOrderEntity"))
                return new CustomerOrderExpression(null, _root, path,
                    new TeaQLNotLoadedException(_root, path, "CustomerOrder"));
            return new CustomerOrderExpression(_value.CustomerOrderEntity, _root, path);
        }

    }

    public sealed class PlatformListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Platform> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PlatformListExpression(
            IReadOnlyList<Generated.Models.Platform>? items,
            string? root = "Platform(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Platform>();
            _root = root ?? "Platform(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static PlatformListExpression Missing(string? root = null, string path = "") =>
            new(new List<Generated.Models.Platform>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public PlatformExpression First() => Get(0);

        public PlatformExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new PlatformExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new PlatformExpression(null, _root, itemPath)
                : new PlatformExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class OrderStatusListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderStatus> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderStatusListExpression(
            IReadOnlyList<Generated.Models.OrderStatus>? items,
            string? root = "OrderStatus(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderStatus>();
            _root = root ?? "OrderStatus(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderStatusListExpression Missing(string? root = null, string path = "") =>
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

    public sealed class CustomerListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Customer> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerListExpression(
            IReadOnlyList<Generated.Models.Customer>? items,
            string? root = "Customer(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Customer>();
            _root = root ?? "Customer(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CustomerListExpression Missing(string? root = null, string path = "") =>
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

    public sealed class CustomerOrderListExpression
    {
        private readonly IReadOnlyList<Generated.Models.CustomerOrder> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerOrderListExpression(
            IReadOnlyList<Generated.Models.CustomerOrder>? items,
            string? root = "CustomerOrder(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.CustomerOrder>();
            _root = root ?? "CustomerOrder(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CustomerOrderListExpression Missing(string? root = null, string path = "") =>
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

    public sealed class OrderItemListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderItem> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderItemListExpression(
            IReadOnlyList<Generated.Models.OrderItem>? items,
            string? root = "OrderItem(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderItem>();
            _root = root ?? "OrderItem(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderItemListExpression Missing(string? root = null, string path = "") =>
            new(new List<Generated.Models.OrderItem>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public OrderItemExpression First() => Get(0);

        public OrderItemExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new OrderItemExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new OrderItemExpression(null, _root, itemPath)
                : new OrderItemExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static PlatformExpression Platform(Generated.Models.Platform? value)
        {
            return new PlatformExpression(value, $"Platform(id={value?.Id})");
        }

        public static OrderStatusExpression OrderStatus(Generated.Models.OrderStatus? value)
        {
            return new OrderStatusExpression(value, $"OrderStatus(id={value?.Id})");
        }

        public static CustomerExpression Customer(Generated.Models.Customer? value)
        {
            return new CustomerExpression(value, $"Customer(id={value?.Id})");
        }

        public static CustomerOrderExpression CustomerOrder(Generated.Models.CustomerOrder? value)
        {
            return new CustomerOrderExpression(value, $"CustomerOrder(id={value?.Id})");
        }

        public static OrderItemExpression OrderItem(Generated.Models.OrderItem? value)
        {
            return new OrderItemExpression(value, $"OrderItem(id={value?.Id})");
        }
    }
}