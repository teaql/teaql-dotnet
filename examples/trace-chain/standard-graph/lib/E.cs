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

        public ValueExpression<string?> Description()
        {
            var path = _path.Member("description", "Description");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Description"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "description"));
            return new ValueExpression<string?>(_value.Description);
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

        public PaymentListExpression PaymentList()
        {
            var path = _path.Member("payment_list", "PaymentList");
            if (_notLoaded != null) return new PaymentListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return PaymentListExpression.Missing(_root, path);
            if (!_value.IsLoaded("PaymentList"))
                return new PaymentListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "payment_list"));
            return new PaymentListExpression(_value.PaymentList, _root, path);
        }

        public ShipmentListExpression ShipmentList()
        {
            var path = _path.Member("shipment_list", "ShipmentList");
            if (_notLoaded != null) return new ShipmentListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return ShipmentListExpression.Missing(_root, path);
            if (!_value.IsLoaded("ShipmentList"))
                return new ShipmentListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "shipment_list"));
            return new ShipmentListExpression(_value.ShipmentList, _root, path);
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
            if (!_value.IsLoaded("CustomerOrderEntity"))
                return new CustomerOrderExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_order"));
            return new CustomerOrderExpression(_value.CustomerOrderEntity, _root, path);
        }

    }

    public sealed class PaymentExpression
    {
        private readonly Generated.Models.Payment? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PaymentExpression(
            Generated.Models.Payment? value,
            string root = "Payment(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Payment(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Payment? Eval()
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

        public ValueExpression<string?> ReferenceCode()
        {
            var path = _path.Member("reference_code", "ReferenceCode");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ReferenceCode"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "reference_code"));
            return new ValueExpression<string?>(_value.ReferenceCode);
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
            if (!_value.IsLoaded("CustomerOrderEntity"))
                return new CustomerOrderExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_order"));
            return new CustomerOrderExpression(_value.CustomerOrderEntity, _root, path);
        }

        public PaymentAttemptListExpression PaymentAttemptList()
        {
            var path = _path.Member("payment_attempt_list", "PaymentAttemptList");
            if (_notLoaded != null) return new PaymentAttemptListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return PaymentAttemptListExpression.Missing(_root, path);
            if (!_value.IsLoaded("PaymentAttemptList"))
                return new PaymentAttemptListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "payment_attempt_list"));
            return new PaymentAttemptListExpression(_value.PaymentAttemptList, _root, path);
        }
    }

    public sealed class PaymentAttemptExpression
    {
        private readonly Generated.Models.PaymentAttempt? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PaymentAttemptExpression(
            Generated.Models.PaymentAttempt? value,
            string root = "PaymentAttempt(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.PaymentAttempt(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.PaymentAttempt? Eval()
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

        public ValueExpression<string?> ReferenceCode()
        {
            var path = _path.Member("reference_code", "ReferenceCode");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ReferenceCode"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "reference_code"));
            return new ValueExpression<string?>(_value.ReferenceCode);
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

        public ValueExpression<long?> PaymentId()
        {
            var path = _path.Member("payment", "PaymentId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Payment"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "payment"));
            return new ValueExpression<long?>(_value.Payment);
        }

        public PaymentExpression Payment()
        {
            var path = _path.Member("payment", "Payment");
            if (_notLoaded != null) return new PaymentExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new PaymentExpression(null, _root, path);
            if (!_value.IsLoaded("PaymentEntity"))
                return new PaymentExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "payment"));
            return new PaymentExpression(_value.PaymentEntity, _root, path);
        }

    }

    public sealed class ShipmentExpression
    {
        private readonly Generated.Models.Shipment? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ShipmentExpression(
            Generated.Models.Shipment? value,
            string root = "Shipment(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Shipment(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Shipment? Eval()
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

        public ValueExpression<string?> ReferenceCode()
        {
            var path = _path.Member("reference_code", "ReferenceCode");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ReferenceCode"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "reference_code"));
            return new ValueExpression<string?>(_value.ReferenceCode);
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
            if (!_value.IsLoaded("CustomerOrderEntity"))
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

    public sealed class PaymentListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Payment> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PaymentListExpression(
            IReadOnlyList<Generated.Models.Payment>? items,
            string? root = "Payment(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Payment>();
            _root = root ?? "Payment(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static PaymentListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Payment>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public PaymentExpression First() => Item(0, true);

        public PaymentExpression Get(int index)
        {
            return Item(index, false);
        }

        private PaymentExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new PaymentExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new PaymentExpression(null, _root, itemPath)
                : new PaymentExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class PaymentAttemptListExpression
    {
        private readonly IReadOnlyList<Generated.Models.PaymentAttempt> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public PaymentAttemptListExpression(
            IReadOnlyList<Generated.Models.PaymentAttempt>? items,
            string? root = "PaymentAttempt(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.PaymentAttempt>();
            _root = root ?? "PaymentAttempt(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static PaymentAttemptListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.PaymentAttempt>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public PaymentAttemptExpression First() => Item(0, true);

        public PaymentAttemptExpression Get(int index)
        {
            return Item(index, false);
        }

        private PaymentAttemptExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new PaymentAttemptExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new PaymentAttemptExpression(null, _root, itemPath)
                : new PaymentAttemptExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class ShipmentListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Shipment> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public ShipmentListExpression(
            IReadOnlyList<Generated.Models.Shipment>? items,
            string? root = "Shipment(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Shipment>();
            _root = root ?? "Shipment(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static ShipmentListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Shipment>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public ShipmentExpression First() => Item(0, true);

        public ShipmentExpression Get(int index)
        {
            return Item(index, false);
        }

        private ShipmentExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new ShipmentExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new ShipmentExpression(null, _root, itemPath)
                : new ShipmentExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static PlatformExpression Platform(Generated.Models.Platform? value)
        {
            return new PlatformExpression(value, "Platform");
        }

        public static CustomerOrderExpression CustomerOrder(Generated.Models.CustomerOrder? value)
        {
            return new CustomerOrderExpression(value, "CustomerOrder");
        }

        public static OrderItemExpression OrderItem(Generated.Models.OrderItem? value)
        {
            return new OrderItemExpression(value, "OrderItem");
        }

        public static PaymentExpression Payment(Generated.Models.Payment? value)
        {
            return new PaymentExpression(value, "Payment");
        }

        public static PaymentAttemptExpression PaymentAttempt(Generated.Models.PaymentAttempt? value)
        {
            return new PaymentAttemptExpression(value, "PaymentAttempt");
        }

        public static ShipmentExpression Shipment(Generated.Models.Shipment? value)
        {
            return new ShipmentExpression(value, "Shipment");
        }
    }
}