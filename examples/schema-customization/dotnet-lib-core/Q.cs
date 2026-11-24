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
                public static SchoolRequest Schools()
                {
                    return new SchoolRequest().SelectSelfFields();
                }

                public static SchoolRequest SchoolsWithMinimalFields()
                {
                    return new SchoolRequest();
                }
    }
}