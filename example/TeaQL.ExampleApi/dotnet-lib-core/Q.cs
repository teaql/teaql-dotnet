using Generated.Models;
using Generated.Requests;

namespace Generated
{
    public static class Q
    {
                public static MerchantRequest Merchants()
                {
                    return new MerchantRequest().SelectSelfFields();
                }

                public static MerchantRequest MerchantsWithMinimalFields()
                {
                    return new MerchantRequest();
                }
                public static FederatedOrderRequest FederatedOrders()
                {
                    return new FederatedOrderRequest().SelectSelfFields();
                }

                public static FederatedOrderRequest FederatedOrdersWithMinimalFields()
                {
                    return new FederatedOrderRequest();
                }
    }
}