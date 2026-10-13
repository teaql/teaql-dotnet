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

        public ValueExpression<string?> Description()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "Description");
            if (!_value.IsLoaded("Description"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Description"));
            return new ValueExpression<string?>(_value.Description);
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

        public PaymentListExpression PaymentList()
        {
            var path = ExpressionPath.Append(_path, "PaymentList");
            if (_notLoaded != null) return new PaymentListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return PaymentListExpression.Missing(_root, path);
            if (!_value.IsLoaded("PaymentList"))
                return new PaymentListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "PaymentList"));
            return new PaymentListExpression(_value.PaymentList, _root, path);
        }

        public ShipmentListExpression ShipmentList()
        {
            var path = ExpressionPath.Append(_path, "ShipmentList");
            if (_notLoaded != null) return new ShipmentListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return ShipmentListExpression.Missing(_root, path);
            if (!_value.IsLoaded("ShipmentList"))
                return new ShipmentListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "ShipmentList"));
            return new ShipmentListExpression(_value.ShipmentList, _root, path);
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

    public sealed class PaymentExpression
    {
        private readonly Generated.Models.Payment? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PaymentExpression(
            Generated.Models.Payment? value,
            string root = "Payment(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.Payment? Eval()
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

        public ValueExpression<string?> ReferenceCode()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "ReferenceCode");
            if (!_value.IsLoaded("ReferenceCode"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "ReferenceCode"));
            return new ValueExpression<string?>(_value.ReferenceCode);
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

        public PaymentAttemptListExpression PaymentAttemptList()
        {
            var path = ExpressionPath.Append(_path, "PaymentAttemptList");
            if (_notLoaded != null) return new PaymentAttemptListExpression(null, _root, path, false, _notLoaded);
            if (_value is null) return PaymentAttemptListExpression.Missing(_root, path);
            if (!_value.IsLoaded("PaymentAttemptList"))
                return new PaymentAttemptListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(_root, path, "PaymentAttemptList"));
            return new PaymentAttemptListExpression(_value.PaymentAttemptList, _root, path);
        }
    }

    public sealed class PaymentAttemptExpression
    {
        private readonly Generated.Models.PaymentAttempt? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PaymentAttemptExpression(
            Generated.Models.PaymentAttempt? value,
            string root = "PaymentAttempt(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.PaymentAttempt? Eval()
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

        public ValueExpression<string?> ReferenceCode()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "ReferenceCode");
            if (!_value.IsLoaded("ReferenceCode"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "ReferenceCode"));
            return new ValueExpression<string?>(_value.ReferenceCode);
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

        public ValueExpression<long?> PaymentId()
        {
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<long?>.Missing();
            var path = ExpressionPath.Append(_path, "Payment");
            if (!_value.IsLoaded("Payment"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "Payment"));
            return new ValueExpression<long?>(_value.Payment);
        }

        public PaymentExpression Payment()
        {
            var path = ExpressionPath.Append(_path, "Payment");
            if (_notLoaded != null) return new PaymentExpression(null, _root, path, _notLoaded);
            if (_value is null) return new PaymentExpression(null, _root, path);
            if (!_value.IsLoaded("PaymentEntity"))
                return new PaymentExpression(null, _root, path,
                    new TeaQLNotLoadedException(_root, path, "Payment"));
            return new PaymentExpression(_value.PaymentEntity, _root, path);
        }

    }

    public sealed class ShipmentExpression
    {
        private readonly Generated.Models.Shipment? _value;
        private readonly string _root;
        private readonly string _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ShipmentExpression(
            Generated.Models.Shipment? value,
            string root = "Shipment(null)",
            string path = "",
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path;
            _notLoaded = notLoaded;
        }

        public Generated.Models.Shipment? Eval()
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

        public ValueExpression<string?> ReferenceCode()
        {
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded);
            if (_value is null) return ValueExpression<string?>.Missing();
            var path = ExpressionPath.Append(_path, "ReferenceCode");
            if (!_value.IsLoaded("ReferenceCode"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(_root, path, "ReferenceCode"));
            return new ValueExpression<string?>(_value.ReferenceCode);
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

    public sealed class PaymentListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Payment> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PaymentListExpression(
            IReadOnlyList<Generated.Models.Payment>? items,
            string? root = "Payment(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Payment>();
            _root = root ?? "Payment(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static PaymentListExpression Missing(string? root = null, string path = "") =>
            new(new List<Generated.Models.Payment>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public PaymentExpression First() => Get(0);

        public PaymentExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new PaymentExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new PaymentExpression(null, _root, itemPath)
                : new PaymentExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class PaymentAttemptListExpression
    {
        private readonly IReadOnlyList<Generated.Models.PaymentAttempt> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PaymentAttemptListExpression(
            IReadOnlyList<Generated.Models.PaymentAttempt>? items,
            string? root = "PaymentAttempt(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.PaymentAttempt>();
            _root = root ?? "PaymentAttempt(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static PaymentAttemptListExpression Missing(string? root = null, string path = "") =>
            new(new List<Generated.Models.PaymentAttempt>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public PaymentAttemptExpression First() => Get(0);

        public PaymentAttemptExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new PaymentAttemptExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new PaymentAttemptExpression(null, _root, itemPath)
                : new PaymentAttemptExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class ShipmentListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Shipment> _items;
        private readonly string _root;
        private readonly string _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ShipmentListExpression(
            IReadOnlyList<Generated.Models.Shipment>? items,
            string? root = "Shipment(null)",
            string path = "",
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Shipment>();
            _root = root ?? "Shipment(null)";
            _path = path;
            _present = present;
            _notLoaded = notLoaded;
        }

        public static ShipmentListExpression Missing(string? root = null, string path = "") =>
            new(new List<Generated.Models.Shipment>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded);
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public ShipmentExpression First() => Get(0);

        public ShipmentExpression Get(int index)
        {
            var itemPath = ExpressionPath.Append(_path, $"Get({index})");
            if (_notLoaded != null) return new ShipmentExpression(null, _root, itemPath, _notLoaded);
            return !_present || index < 0 || index >= _items.Count
                ? new ShipmentExpression(null, _root, itemPath)
                : new ShipmentExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static PlatformExpression Platform(Generated.Models.Platform? value)
        {
            return new PlatformExpression(value, $"Platform(id={value?.Id})");
        }

        public static CustomerOrderExpression CustomerOrder(Generated.Models.CustomerOrder? value)
        {
            return new CustomerOrderExpression(value, $"CustomerOrder(id={value?.Id})");
        }

        public static OrderItemExpression OrderItem(Generated.Models.OrderItem? value)
        {
            return new OrderItemExpression(value, $"OrderItem(id={value?.Id})");
        }

        public static PaymentExpression Payment(Generated.Models.Payment? value)
        {
            return new PaymentExpression(value, $"Payment(id={value?.Id})");
        }

        public static PaymentAttemptExpression PaymentAttempt(Generated.Models.PaymentAttempt? value)
        {
            return new PaymentAttemptExpression(value, $"PaymentAttempt(id={value?.Id})");
        }

        public static ShipmentExpression Shipment(Generated.Models.Shipment? value)
        {
            return new ShipmentExpression(value, $"Shipment(id={value?.Id})");
        }
    }
}