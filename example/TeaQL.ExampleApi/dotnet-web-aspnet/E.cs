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



        public FederatedOrderListExpression FederatedOrderList()
        {
            var path = _path.Member("federated_order_list", "FederatedOrderList");
            if (_notLoaded != null) return new FederatedOrderListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return FederatedOrderListExpression.Missing(_root, path);
            if (!_value.IsLoaded("FederatedOrderList"))
                return new FederatedOrderListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "federated_order_list"));
            return new FederatedOrderListExpression(_value.FederatedOrderList, _root, path);
        }
    }

    public sealed class FederatedOrderExpression
    {
        private readonly Generated.Models.FederatedOrder? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public FederatedOrderExpression(
            Generated.Models.FederatedOrder? value,
            string root = "FederatedOrder(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.FederatedOrder(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.FederatedOrder? Eval()
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

    public sealed class FederatedOrderListExpression
    {
        private readonly IReadOnlyList<Generated.Models.FederatedOrder> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public FederatedOrderListExpression(
            IReadOnlyList<Generated.Models.FederatedOrder>? items,
            string? root = "FederatedOrder(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.FederatedOrder>();
            _root = root ?? "FederatedOrder(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static FederatedOrderListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.FederatedOrder>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public FederatedOrderExpression First() => Item(0, true);

        public FederatedOrderExpression Get(int index)
        {
            return Item(index, false);
        }

        private FederatedOrderExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new FederatedOrderExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new FederatedOrderExpression(null, _root, itemPath)
                : new FederatedOrderExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static MerchantExpression Merchant(Generated.Models.Merchant? value)
        {
            return new MerchantExpression(value, "Merchant");
        }

        public static FederatedOrderExpression FederatedOrder(Generated.Models.FederatedOrder? value)
        {
            return new FederatedOrderExpression(value, "FederatedOrder");
        }
    }
}