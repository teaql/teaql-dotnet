using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeaQL.Core;

namespace Generated.Models
{
    public class Customer
    {
                public long? Id { get; set; }
                public string Name { get; set; }
                public string Email { get; set; }
                public long? CommercePlatform { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public List<CustomerOrder> CustomerOrderList { get; } = new List<CustomerOrder>();

        private string _comment;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
        }

        public Customer MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public Customer MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public Customer AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public static Customer Refer(long id)
        {
            return new Customer { Id = id }.MarkLoadedOnly("Id");
        }

        public static Customer FromRecord(Record record)
        {
            var entity = new Customer().MarkLoadedOnly();
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("name", out var nameValue))
                    {
                        entity.MarkLoaded("Name");
                        if (nameValue.Raw != null)
                            entity.Name = Convert.ToString(nameValue.Raw);
                    }
                    if (record.TryGetValue("email", out var emailValue))
                    {
                        entity.MarkLoaded("Email");
                        if (emailValue.Raw != null)
                            entity.Email = Convert.ToString(emailValue.Raw);
                    }
                    if (record.TryGetValue("commerce_platform", out var commercePlatformValue))
                    {
                        entity.MarkLoaded("CommercePlatform");
                        if (commercePlatformValue.Raw != null)
                            entity.CommercePlatform = Convert.ToInt64(commercePlatformValue.Raw);
                    }
                    if (record.TryGetValue("create_time", out var createTimeValue))
                    {
                        entity.MarkLoaded("CreateTime");
                        if (createTimeValue.Raw != null)
                            entity.CreateTime = Convert.ToDateTime(createTimeValue.Raw);
                    }
                    if (record.TryGetValue("update_time", out var updateTimeValue))
                    {
                        entity.MarkLoaded("UpdateTime");
                        if (updateTimeValue.Raw != null)
                            entity.UpdateTime = Convert.ToDateTime(updateTimeValue.Raw);
                    }
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
                    }
                        if (record.TryGetValue("CustomerOrderList", out var customerOrderListValue))
                        {
                            entity.MarkLoaded("CustomerOrderList");
                            var rows = customerOrderListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.CustomerOrderList.Add(CustomerOrder.FromRecord(row));
                        }
            return entity;
        }

        public async Task<object> SaveAsync(UserContext context)
        {
            if (string.IsNullOrEmpty(_comment))
            {
                throw new Exception("Security audit failure: AuditAs() must be called before SaveAsync()");
            }

            var creating = !this.Id.HasValue;
            var cmd = creating ? (object)ToInsertCommand() : (object)ToUpdateCommand();
            var req = new MutationRequest { Command = cmd, Comment = _comment };
            var result = await context.DataService.MutateAsync(context, req);
            if (result is MutationResult mutationResult)
            {
                if (creating) Id = mutationResult.Id;
                if (!mutationResult.Deleted) Version = mutationResult.Version;
            }
            foreach (var child in CustomerOrderList)
            {
                child.UpdateCustomerId(Id);
                child.AuditAs(_comment);
                await child.SaveAsync(context);
            }
            return result;
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Email != null) record["email"] = new Value.TextValue(Email);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "Customer", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Email != null) record["email"] = new Value.TextValue(Email);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "Customer", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("Customer");
        }

                public Customer UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    return this;
                }

                public Customer UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    return this;
                }

                public Customer UpdateEmail(string value)
                {
                    this.Email = value;
                    MarkLoaded("Email");
                    return this;
                }

                public Customer UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

                public Customer UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    return this;
                }

                public Customer UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    return this;
                }

                public Customer UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    return this;
                }
                public Customer UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    return this;
                }


                public Customer UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

    }
}