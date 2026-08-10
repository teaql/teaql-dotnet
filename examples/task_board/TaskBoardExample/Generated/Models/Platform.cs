using System;
using System.Collections.Generic;
using TeaQL.Core;

namespace Generated.Models
{
    public class Platform
    {
                public long? Id { get; set; }
                public string Name { get; set; }
                public long? Founded { get; set; }
                public string UserEmail { get; set; }
                public long? Version { get; set; }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Founded.HasValue) record["founded"] = new Value.I64Value(Founded.Value);

                    if (UserEmail != null) record["user_email"] = new Value.TextValue(UserEmail);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "Platform", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Founded.HasValue) record["founded"] = new Value.I64Value(Founded.Value);

                    if (UserEmail != null) record["user_email"] = new Value.TextValue(UserEmail);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "Platform", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("Platform");
        }
    }
}