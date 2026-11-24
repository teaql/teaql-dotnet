using Generated.Models;
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
                public static CustomerOrderRequest CustomerOrders()
                {
                    return new CustomerOrderRequest().SelectSelfFields();
                }

                public static CustomerOrderRequest CustomerOrdersWithMinimalFields()
                {
                    return new CustomerOrderRequest();
                }
                public static OrderAttachmentRequest OrderAttachments()
                {
                    return new OrderAttachmentRequest().SelectSelfFields();
                }

                public static OrderAttachmentRequest OrderAttachmentsWithMinimalFields()
                {
                    return new OrderAttachmentRequest();
                }
    }
}