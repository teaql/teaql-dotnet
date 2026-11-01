using Generated.Models;
using Generated.Requests;

namespace Generated
{
    public static class Q
    {
                public static TypePlatformRequest TypePlatforms()
                {
                    return new TypePlatformRequest().SelectSelfFields();
                }

                public static TypePlatformRequest TypePlatformsWithMinimalFields()
                {
                    return new TypePlatformRequest();
                }
                public static ScalarTypeProbeRequest ScalarTypeProbes()
                {
                    return new ScalarTypeProbeRequest().SelectSelfFields();
                }

                public static ScalarTypeProbeRequest ScalarTypeProbesWithMinimalFields()
                {
                    return new ScalarTypeProbeRequest();
                }
    }
}