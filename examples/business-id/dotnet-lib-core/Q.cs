using Generated.Models;
using Generated.Requests;

namespace Generated
{
    public static class Q
    {
                public static PlatformRequest Platforms()
                {
                    return new PlatformRequest().SelectSelfFields();
                }

                public static PlatformRequest PlatformsWithMinimalFields()
                {
                    return new PlatformRequest();
                }
                public static PurchaseOrderRequest PurchaseOrders()
                {
                    return new PurchaseOrderRequest().SelectSelfFields();
                }

                public static PurchaseOrderRequest PurchaseOrdersWithMinimalFields()
                {
                    return new PurchaseOrderRequest();
                }
                public static PurchaseOrderRequest PurchaseOrderByOrderNumber(OrderNumber value)
                {
                    return PurchaseOrders().WithOrderNumberIs(value).Limit(2);
                }
    }
}