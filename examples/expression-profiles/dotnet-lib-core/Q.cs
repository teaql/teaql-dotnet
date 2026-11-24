using Generated.Models;
using Generated.Requests;

namespace Generated
{
    public static class Q
    {
                public static DistributionCenterRequest DistributionCenters()
                {
                    return new DistributionCenterRequest().SelectSelfFields();
                }

                public static DistributionCenterRequest DistributionCentersWithMinimalFields()
                {
                    return new DistributionCenterRequest();
                }
                public static DispatchOrderRequest DispatchOrders()
                {
                    return new DispatchOrderRequest().SelectSelfFields();
                }

                public static DispatchOrderRequest DispatchOrdersWithMinimalFields()
                {
                    return new DispatchOrderRequest();
                }
    }
}