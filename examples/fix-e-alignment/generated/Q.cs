using Generated.Models;
using Generated.Requests;

namespace Generated
{
    public static class Q
    {
                public static TenantRequest Tenants()
                {
                    return new TenantRequest().SelectSelfFields();
                }

                public static TenantRequest TenantsWithMinimalFields()
                {
                    return new TenantRequest();
                }
                public static PurchaseOrderRequest PurchaseOrders()
                {
                    return new PurchaseOrderRequest().SelectSelfFields();
                }

                public static PurchaseOrderRequest PurchaseOrdersWithMinimalFields()
                {
                    return new PurchaseOrderRequest();
                }
                public static OrderItemRequest OrderItems()
                {
                    return new OrderItemRequest().SelectSelfFields();
                }

                public static OrderItemRequest OrderItemsWithMinimalFields()
                {
                    return new OrderItemRequest();
                }
    }
}