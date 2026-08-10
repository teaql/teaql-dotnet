using System;
using System.Collections.Generic;
using TeaQL.Core;

namespace Generated.Models
{
    public class Task
    {
                public long? Id { get; set; }
                public string Name { get; set; }
                public long? Status { get; set; }
                public long? Platform { get; set; }
                public long? Version { get; set; }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Status.HasValue) record["status"] = new Value.I64Value(Status.Value);

                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "Task", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Status.HasValue) record["status"] = new Value.I64Value(Status.Value);

                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "Task", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("Task");
        }
    }
}