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
            if (!_value.IsLoaded("CommercePlatform"))
                return new CommercePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            if (_value.CommercePlatform is null) return new CommercePlatformExpression(null, _root, path);
            if (!_value.IsLoaded("CommercePlatformEntity") || _value.CommercePlatformEntity is null
                || _value.CommercePlatformEntity.Id != _value.CommercePlatform)
                return new CommercePlatformExpression(null, _root, path,
                    new TeaQLNotLoadedException(path.MarkMissing(), "commerce_platform"));
            return new CommercePlatformExpression(_value.CommercePlatformEntity, _root, path);
        }

        public OrderAttachmentListExpression OrderAttachmentList()
        {
            var path = _path.Member("order_attachment_list", "OrderAttachmentList");
            if (_notLoaded != null) return new OrderAttachmentListExpression(null, _root, path, false, _notLoaded.WithPath(path));
            if (_value is null) return OrderAttachmentListExpression.Missing(_root, path);
            if (!_value.IsLoaded("OrderAttachmentList"))
                return new OrderAttachmentListExpression(null, _root, path, false,
                    new TeaQLNotLoadedException(path.MarkMissing(), "order_attachment_list"));
            return new OrderAttachmentListExpression(_value.OrderAttachmentList, _root, path);
        }
    }

    public sealed class OrderAttachmentExpression
    {
        private readonly Generated.Models.OrderAttachment? _value;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderAttachmentExpression(
            Generated.Models.OrderAttachment? value,
            string root = "OrderAttachment(null)",
            ExpressionPath? path = null,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _value = value;
            _root = root;
            _path = path ?? ExpressionPath.Root("E.OrderAttachment(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _notLoaded = notLoaded;
        }

        public Generated.Models.OrderAttachment? Eval()
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

        public ValueExpression<string?> FileName()
        {
            var path = _path.Member("file_name", "FileName");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("FileName"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "file_name"));
            return new ValueExpression<string?>(_value.FileName);
        }

        public ValueExpression<string?> ContentType()
        {
            var path = _path.Member("content_type", "ContentType");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ContentType"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "content_type"));
            return new ValueExpression<string?>(_value.ContentType);
        }

        public ValueExpression<long?> ContentLength()
        {
            var path = _path.Member("content_length", "ContentLength");
            if (_notLoaded != null) return ValueExpression<long?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<long?>.Missing();
            if (!_value.IsLoaded("ContentLength"))
                return ValueExpression<long?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "content_length"));
            return new ValueExpression<long?>(_value.ContentLength);
        }

        public ValueExpression<string?> ContentSha256()
        {
            var path = _path.Member("content_sha256", "ContentSha256");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ContentSha256"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "content_sha256"));
            return new ValueExpression<string?>(_value.ContentSha256);
        }

        public ValueExpression<string?> ObjectKey()
        {
            var path = _path.Member("object_key", "ObjectKey");
            if (_notLoaded != null) return ValueExpression<string?>.NotLoaded(_notLoaded.WithPath(path));
            if (_value is null) return ValueExpression<string?>.Missing();
            if (!_value.IsLoaded("ObjectKey"))
                return ValueExpression<string?>.NotLoaded(new TeaQLNotLoadedException(path.MarkMissing(), "object_key"));
            return new ValueExpression<string?>(_value.ObjectKey);
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

    public sealed class OrderAttachmentListExpression
    {
        private readonly IReadOnlyList<Generated.Models.OrderAttachment> _items;
        private readonly string _root;
        private readonly ExpressionPath _path;
        private readonly bool _present;
        private readonly TeaQLNotLoadedException? _notLoaded;

        public OrderAttachmentListExpression(
            IReadOnlyList<Generated.Models.OrderAttachment>? items,
            string? root = "OrderAttachment(null)",
            ExpressionPath? path = null,
            bool present = true,
            TeaQLNotLoadedException? notLoaded = null)
        {
            _items = items ?? new List<Generated.Models.OrderAttachment>();
            _root = root ?? "OrderAttachment(null)";
            _path = path ?? ExpressionPath.Root("E.entity(<root>)", JsonFieldNamingProfiles.Parse("camelCase"));
            _present = present;
            _notLoaded = notLoaded;
        }

        public static OrderAttachmentListExpression Missing(string? root = null, ExpressionPath? path = null) =>
            new(new List<Generated.Models.OrderAttachment>(), root, path, false);

        public ValueExpression<int> Size()
        {
            if (_notLoaded != null) return ValueExpression<int>.NotLoaded(_notLoaded.WithPath(_path.Operation("Size()")));
            return _present ? new ValueExpression<int>(_items.Count) : ValueExpression<int>.Missing();
        }

        public OrderAttachmentExpression First() => Item(0, true);

        public OrderAttachmentExpression Get(int index)
        {
            return Item(index, false);
        }

        private OrderAttachmentExpression Item(int index, bool first)
        {
            var itemPath = _path.At(index, first);
            if (_notLoaded != null) return new OrderAttachmentExpression(null, _root, itemPath, _notLoaded.WithPath(itemPath));
            return !_present || index < 0 || index >= _items.Count
                ? new OrderAttachmentExpression(null, _root, itemPath)
                : new OrderAttachmentExpression(_items[index], _root, itemPath);
        }
    }

    public static class E
    {
        public static CommercePlatformExpression CommercePlatform(Generated.Models.CommercePlatform? value)
        {
            return new CommercePlatformExpression(value, "CommercePlatform");
        }

        public static CustomerOrderExpression CustomerOrder(Generated.Models.CustomerOrder? value)
        {
            return new CustomerOrderExpression(value, "CustomerOrder");
        }

        public static OrderAttachmentExpression OrderAttachment(Generated.Models.OrderAttachment? value)
        {
            return new OrderAttachmentExpression(value, "OrderAttachment");
        }
    }
}