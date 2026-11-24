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
                public static FreightQuoteRequest FreightQuotes()
                {
                    return new FreightQuoteRequest().SelectSelfFields();
                }

                public static FreightQuoteRequest FreightQuotesWithMinimalFields()
                {
                    return new FreightQuoteRequest();
                }
    }
}