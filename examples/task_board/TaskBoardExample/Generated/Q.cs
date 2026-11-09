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
                public static TaskStatusRequest TaskStatuses()
                {
                    return new TaskStatusRequest().SelectSelfFields();
                }

                public static TaskStatusRequest TaskStatusesWithMinimalFields()
                {
                    return new TaskStatusRequest();
                }
                public static TaskRequest Tasks()
                {
                    return new TaskRequest().SelectSelfFields();
                }

                public static TaskRequest TasksWithMinimalFields()
                {
                    return new TaskRequest();
                }
                public static TaskExecutionLogRequest TaskExecutionLogs()
                {
                    return new TaskExecutionLogRequest().SelectSelfFields();
                }

                public static TaskExecutionLogRequest TaskExecutionLogsWithMinimalFields()
                {
                    return new TaskExecutionLogRequest();
                }
    }
}