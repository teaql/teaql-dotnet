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
                public static PaymentRequest Payments()
                {
                    return new PaymentRequest().SelectSelfFields();
                }

                public static PaymentRequest PaymentsWithMinimalFields()
                {
                    return new PaymentRequest();
                }
                public static PaymentAttemptRequest PaymentAttempts()
                {
                    return new PaymentAttemptRequest().SelectSelfFields();
                }

                public static PaymentAttemptRequest PaymentAttemptsWithMinimalFields()
                {
                    return new PaymentAttemptRequest();
                }
                public static ShipmentRequest Shipments()
                {
                    return new ShipmentRequest().SelectSelfFields();
                }

                public static ShipmentRequest ShipmentsWithMinimalFields()
                {
                    return new ShipmentRequest();
                }
    }
}