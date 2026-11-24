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
                public static ShippingOrderRequest ShippingOrders()
                {
                    return new ShippingOrderRequest().SelectSelfFields();
                }

                public static ShippingOrderRequest ShippingOrdersWithMinimalFields()
                {
                    return new ShippingOrderRequest();
                }
                public static ShippingItemRequest ShippingItems()
                {
                    return new ShippingItemRequest().SelectSelfFields();
                }

                public static ShippingItemRequest ShippingItemsWithMinimalFields()
                {
                    return new ShippingItemRequest();
                }
    }
}