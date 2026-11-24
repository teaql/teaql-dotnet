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
                public static CustomerRequest Customers()
                {
                    return new CustomerRequest().SelectSelfFields();
                }

                public static CustomerRequest CustomersWithMinimalFields()
                {
                    return new CustomerRequest();
                }
                public static CustomerContactRequest CustomerContacts()
                {
                    return new CustomerContactRequest().SelectSelfFields();
                }

                public static CustomerContactRequest CustomerContactsWithMinimalFields()
                {
                    return new CustomerContactRequest();
                }
    }
}