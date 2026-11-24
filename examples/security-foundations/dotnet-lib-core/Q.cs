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
                public static CustomerProfileRequest CustomerProfiles()
                {
                    return new CustomerProfileRequest().SelectSelfFields();
                }

                public static CustomerProfileRequest CustomerProfilesWithMinimalFields()
                {
                    return new CustomerProfileRequest();
                }
                public static CustomerOrderRequest CustomerOrders()
                {
                    return new CustomerOrderRequest().SelectSelfFields();
                }

                public static CustomerOrderRequest CustomerOrdersWithMinimalFields()
                {
                    return new CustomerOrderRequest();
                }
    }
}