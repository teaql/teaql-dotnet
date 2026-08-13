using System;
using System.Collections.Generic;
using TeaQL.Core;

namespace Generated.Models
{
    public class TaskExecutionLog
    {
                public long? Id { get; set; }
                public long? Task { get; set; }
                public string Action { get; set; }
                public string Detail { get; set; }
                public long? Version { get; set; }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Task.HasValue) record["task"] = new Value.I64Value(Task.Value);

                    if (Action != null) record["action"] = new Value.TextValue(Action);

                    if (Detail != null) record["detail"] = new Value.TextValue(Detail);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "TaskExecutionLog", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Task.HasValue) record["task"] = new Value.I64Value(Task.Value);

                    if (Action != null) record["action"] = new Value.TextValue(Action);

                    if (Detail != null) record["detail"] = new Value.TextValue(Detail);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "TaskExecutionLog", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("TaskExecutionLog");
        }
    }
}