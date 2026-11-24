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

    public sealed class MerchantExpression
    {
        private readonly Generated.Models.Merchant? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public MerchantExpression(
            Generated.Models.Merchant? value,
            string root = "Merchant(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.Merchant(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.Merchant? Eval()
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



        public CustomerProfileListExpression CustomerProfileList()
        {
            var path = _path.Member("customer_profile_list", "CustomerProfileList");
            if (_notLoaded != null) return new CustomerProfileListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return CustomerProfileListExpression.Missing(_root, path);
            if (!_value.IsLoaded("CustomerProfileList"))
                return new CustomerProfileListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_profile_list"));
            return new CustomerProfileListExpression(_value.CustomerProfileList, _root, path);
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

    public sealed class CustomerProfileExpression
    {
        private readonly Generated.Models.CustomerProfile? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerProfileExpression(
            Generated.Models.CustomerProfile? value,
            string root = "CustomerProfile(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.CustomerProfile(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.CustomerProfile? Eval()
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

        public ValueExpression<string?> Email()
        {
            var path = _path.Member("email", "Email");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("Email"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "email"));
            return new ValueExpression<string?>(_value.Email);
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

        public ValueExpression<long?> MerchantId()
        {
            var path = _path.Member("merchant", "MerchantId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Merchant"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "merchant"));
            return new ValueExpression<long?>(_value.Merchant);
        }

        public MerchantExpression Merchant()
        {
            var path = _path.Member("merchant", "Merchant");
            if (_notLoaded != null) return new MerchantExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new MerchantExpression(null, _root, path);
            if (!_value.IsLoaded("Merchant"))
                return new MerchantExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "merchant"));
            if (_value.Merchant is null) return new MerchantExpression(null, _root, path);
            if (!_value.IsLoaded("MerchantEntity") || _value.MerchantEntity is null
                || _value.MerchantEntity.Id != _value.Merchant)
                return new MerchantExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "merchant"));
            return new MerchantExpression(_value.MerchantEntity, _root, path);
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

        public ValueExpression<string?> FulfillmentStatus()
        {
            var path = _path.Member("fulfillment_status", "FulfillmentStatus");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("FulfillmentStatus"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "fulfillment_status"));
            return new ValueExpression<string?>(_value.FulfillmentStatus);
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

        public ValueExpression<long?> MerchantId()
        {
            var path = _path.Member("merchant", "MerchantId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("Merchant"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "merchant"));
            return new ValueExpression<long?>(_value.Merchant);
        }

        public ValueExpression<long?> CustomerProfileId()
        {
            var path = _path.Member("customer_profile", "CustomerProfileId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("CustomerProfile"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "customer_profile"));
            return new ValueExpression<long?>(_value.CustomerProfile);
        }

        public MerchantExpression Merchant()
        {
            var path = _path.Member("merchant", "Merchant");
            if (_notLoaded != null) return new MerchantExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new MerchantExpression(null, _root, path);
            if (!_value.IsLoaded("Merchant"))
                return new MerchantExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "merchant"));
            if (_value.Merchant is null) return new MerchantExpression(null, _root, path);
            if (!_value.IsLoaded("MerchantEntity") || _value.MerchantEntity is null
                || _value.MerchantEntity.Id != _value.Merchant)
                return new MerchantExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "merchant"));
            return new MerchantExpression(_value.MerchantEntity, _root, path);
        }

        public CustomerProfileExpression CustomerProfile()
        {
            var path = _path.Member("customer_profile", "CustomerProfile");
            if (_notLoaded != null) return new CustomerProfileExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new CustomerProfileExpression(null, _root, path);
            if (!_value.IsLoaded("CustomerProfile"))
                return new CustomerProfileExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_profile"));
            if (_value.CustomerProfile is null) return new CustomerProfileExpression(null, _root, path);
            if (!_value.IsLoaded("CustomerProfileEntity") || _value.CustomerProfileEntity is null
                || _value.CustomerProfileEntity.Id != _value.CustomerProfile)
                return new CustomerProfileExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "customer_profile"));
            return new CustomerProfileExpression(_value.CustomerProfileEntity, _root, path);
        }

    }

    public sealed class MerchantListExpression
    {
        private readonly IReadOnlyList<Generated.Models.Merchant> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public MerchantListExpression(
            IReadOnlyList<Generated.Models.Merchant>? items,
            string? root = "Merchant(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.Merchant>();
            _root = root ?? "Merchant(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static MerchantListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.Merchant>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public MerchantExpression First() => Item(0, true);

        public MerchantExpression Get(int index)
        {
            return Item(index, false);
        }

        private MerchantExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new MerchantExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new MerchantExpression(null, _root, itemPath)
                : new MerchantExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class CustomerProfileListExpression
    {
        private readonly IReadOnlyList<Generated.Models.CustomerProfile> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public CustomerProfileListExpression(
            IReadOnlyList<Generated.Models.CustomerProfile>? items,
            string? root = "CustomerProfile(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.CustomerProfile>();
            _root = root ?? "CustomerProfile(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static CustomerProfileListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.CustomerProfile>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public CustomerProfileExpression First() => Item(0, true);

        public CustomerProfileExpression Get(int index)
        {
            return Item(index, false);
        }

        private CustomerProfileExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new CustomerProfileExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new CustomerProfileExpression(null, _root, itemPath)
                : new CustomerProfileExpression(_items[index], _root, itemPath);
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

    public static class E
    {
        public static MerchantExpression Merchant(Generated.Models.Merchant? value)
        {
            return new MerchantExpression(value, "Merchant");
        }

        public static CustomerProfileExpression CustomerProfile(Generated.Models.CustomerProfile? value)
        {
            return new CustomerProfileExpression(value, "CustomerProfile");
        }

        public static CustomerOrderExpression CustomerOrder(Generated.Models.CustomerOrder? value)
        {
            return new CustomerOrderExpression(value, "CustomerOrder");
        }
    }
}