using Generated.Requests;

namespace Generated
{
    public static class Q
    {
                public static CommercePlatformRequest CommercePlatforms()
                {
                    return new CommercePlatformRequest().SelectSelfFields();
                }

                public static CommercePlatformRequest CommercePlatformsWithMinimalFields()
                {
                    return new CommercePlatformRequest();
                }
                public static CustomerRequest Customers()
                {
                    return new CustomerRequest().SelectSelfFields();
                }

                public static CustomerRequest CustomersWithMinimalFields()
                {
                    return new CustomerRequest();
                }
                public static OrderStatusRequest OrderStatuses()
                {
                    return new OrderStatusRequest().SelectSelfFields();
                }

                public static OrderStatusRequest OrderStatusesWithMinimalFields()
                {
                    return new OrderStatusRequest();
                }
                public static CustomerOrderRequest CustomerOrders()
                {
                    return new CustomerOrderRequest().SelectSelfFields();
                }

                public static CustomerOrderRequest CustomerOrdersWithMinimalFields()
                {
                    return new CustomerOrderRequest();
                }
                public static ProductRequest Products()
                {
                    return new ProductRequest().SelectSelfFields();
                }

                public static ProductRequest ProductsWithMinimalFields()
                {
                    return new ProductRequest();
                }
                public static OrderLineRequest OrderLines()
                {
                    return new OrderLineRequest().SelectSelfFields();
                }

                public static OrderLineRequest OrderLinesWithMinimalFields()
                {
                    return new OrderLineRequest();
                }
                public static OrderSearchPresetRequest OrderSearchPresets()
                {
                    return new OrderSearchPresetRequest().SelectSelfFields();
                }

                public static OrderSearchPresetRequest OrderSearchPresetsWithMinimalFields()
                {
                    return new OrderSearchPresetRequest();
                }
    }
}