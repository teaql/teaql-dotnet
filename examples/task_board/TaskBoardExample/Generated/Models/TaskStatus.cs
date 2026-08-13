using System;
using System.Collections.Generic;
using TeaQL.Core;

namespace Generated.Models
{
    public class TaskStatus
    {
                public long? Id { get; set; }
                public string Name { get; set; }
                public string Code { get; set; }
                public string Color { get; set; }
                public long? DisplayOrder { get; set; }
                public long? Progress { get; set; }
                public long? Platform { get; set; }
                public long? Version { get; set; }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Code != null) record["code"] = new Value.TextValue(Code);

                    if (Color != null) record["color"] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record["display_order"] = new Value.I64Value(DisplayOrder.Value);

                    if (Progress.HasValue) record["progress"] = new Value.I64Value(Progress.Value);

                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "TaskStatus", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Code != null) record["code"] = new Value.TextValue(Code);

                    if (Color != null) record["color"] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record["display_order"] = new Value.I64Value(DisplayOrder.Value);

                    if (Progress.HasValue) record["progress"] = new Value.I64Value(Progress.Value);

                    if (Platform.HasValue) record["platform"] = new Value.I64Value(Platform.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "TaskStatus", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("TaskStatus");
        }
    }
}