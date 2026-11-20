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

    public sealed class DistributionCenterExpression
    {
        private readonly Generated.Models.DistributionCenter? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public DistributionCenterExpression(
            Generated.Models.DistributionCenter? value,
            string root = "DistributionCenter(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.DistributionCenter(<root>)", JsonFieldNamingProfiles.Parse("snake_case"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.DistributionCenter? Eval()
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

        public ValueExpression<string?> DockName()
        {
            var path = _path.Member("dock_name", "DockName");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("DockName"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "dock_name"));
            return new ValueExpression<string?>(_value.DockName);
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



        public DispatchOrderListExpression DispatchOrderList()
        {
            var path = _path.Member("dispatch_order_list", "DispatchOrderList");
            if (_notLoaded != null) return new DispatchOrderListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return DispatchOrderListExpression.Missing(_root, path);
            if (!_value.IsLoaded("DispatchOrderList"))
                return new DispatchOrderListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "dispatch_order_list"));
            return new DispatchOrderListExpression(_value.DispatchOrderList, _root, path);
        }
    }

    public sealed class DispatchOrderExpression
    {
        private readonly Generated.Models.DispatchOrder? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public DispatchOrderExpression(
            Generated.Models.DispatchOrder? value,
            string root = "DispatchOrder(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.DispatchOrder(<root>)", JsonFieldNamingProfiles.Parse("snake_case"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.DispatchOrder? Eval()
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

        public ValueExpression<string?> OrderReference()
        {
            var path = _path.Member("order_reference", "OrderReference");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("OrderReference"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "order_reference"));
            return new ValueExpression<string?>(_value.OrderReference);
        }

        public ValueExpression<string?> HandlingNote()
        {
            var path = _path.Member("handling_note", "HandlingNote");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("HandlingNote"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "handling_note"));
            return new ValueExpression<string?>(_value.HandlingNote);
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

        public ValueExpression<long?> DistributionCenterId()
        {
            var path = _path.Member("distribution_center", "DistributionCenterId");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("DistributionCenter"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "distribution_center"));
            return new ValueExpression<long?>(_value.DistributionCenter);
        }

        public DistributionCenterExpression DistributionCenter()
        {
            var path = _path.Member("distribution_center", "DistributionCenter");
            if (_notLoaded != null) return new DistributionCenterExpression(null, _root, path, _notLoaded.WithPath(path));
            if (_value is null) return new DistributionCenterExpression(null, _root, path);
            if (!_value.IsLoaded("DistributionCenter"))
                return new DistributionCenterExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "distribution_center"));
            if (_value.DistributionCenter is null) return new DistributionCenterExpression(null, _root, path);
            if (!_value.IsLoaded("DistributionCenterEntity") || _value.DistributionCenterEntity is null
                || _value.DistributionCenterEntity.Id != _value.DistributionCenter)
                return new DistributionCenterExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "distribution_center"));
            return new DistributionCenterExpression(_value.DistributionCenterEntity, _root, path);
        }

    }

    public sealed class DistributionCenterListExpression
    {
        private readonly IReadOnlyList<Generated.Models.DistributionCenter> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public DistributionCenterListExpression(
            IReadOnlyList<Generated.Models.DistributionCenter>? items,
            string? root = "DistributionCenter(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.DistributionCenter>();
            _root = root ?? "DistributionCenter(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("snake_case"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static DistributionCenterListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.DistributionCenter>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public DistributionCenterExpression First() => Item(0, true);

        public DistributionCenterExpression Get(int index)
        {
            return Item(index, false);
        }

        private DistributionCenterExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new DistributionCenterExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new DistributionCenterExpression(null, _root, itemPath)
                : new DistributionCenterExpression(_items[index], _root, itemPath);
        }
    }

    public sealed class DispatchOrderListExpression
    {
        private readonly IReadOnlyList<Generated.Models.DispatchOrder> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public DispatchOrderListExpression(
            IReadOnlyList<Generated.Models.DispatchOrder>? items,
            string? root = "DispatchOrder(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.DispatchOrder>();
            _root = root ?? "DispatchOrder(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("snake_case"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static DispatchOrderListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.DispatchOrder>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public DispatchOrderExpression First() => Item(0, true);

        public DispatchOrderExpression Get(int index)
        {
            return Item(index, false);
        }

        private DispatchOrderExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new DispatchOrderExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new DispatchOrderExpression(null, _root, itemPath)
                : new DispatchOrderExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static DistributionCenterExpression DistributionCenter(Generated.Models.DistributionCenter? value)
        {
            return new DistributionCenterExpression(value, "DistributionCenter");
        }

        public static DispatchOrderExpression DispatchOrder(Generated.Models.DispatchOrder? value)
        {
            return new DispatchOrderExpression(value, "DispatchOrder");
        }
    }
}