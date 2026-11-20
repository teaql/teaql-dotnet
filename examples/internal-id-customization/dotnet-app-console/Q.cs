using Generated.Models;
using Generated.Requests;

namespace Generated
{
    public static class Q
    {
                public static WarehouseRequest Warehouses()
                {
                    return new WarehouseRequest().SelectSelfFields();
                }

                public static WarehouseRequest WarehousesWithMinimalFields()
                {
                    return new WarehouseRequest();
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