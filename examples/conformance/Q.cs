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
                public static WorkItemRequest WorkItems()
                {
                    return new WorkItemRequest().SelectSelfFields();
                }

                public static WorkItemRequest WorkItemsWithMinimalFields()
                {
                    return new WorkItemRequest();
                }
    }
}