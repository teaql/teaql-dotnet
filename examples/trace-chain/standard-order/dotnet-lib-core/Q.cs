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
                public static OrderStatusRequest OrderStatuses()
                {
                    return new OrderStatusRequest().SelectSelfFields();
                }

                public static OrderStatusRequest OrderStatusesWithMinimalFields()
                {
                    return new OrderStatusRequest();
                }
                public static CustomerRequest Customers()
                {
                    return new CustomerRequest().SelectSelfFields();
                }

                public static CustomerRequest CustomersWithMinimalFields()
                {
                    return new CustomerRequest();
                }
                public static CustomerOrderRequest CustomerOrders()
                {
                    return new CustomerOrderRequest().SelectSelfFields();
                }

                public static CustomerOrderRequest CustomerOrdersWithMinimalFields()
                {
                    return new CustomerOrderRequest();
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