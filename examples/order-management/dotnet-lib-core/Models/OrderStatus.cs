using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeaQL.Core;

namespace Generated.Models
{
    public class OrderStatus
    {
                public long? Id { get; set; }
                public string Name { get; set; }
                public string Code { get; set; }
                public string Color { get; set; }
                public decimal? DisplayOrder { get; set; }
                public long? CommercePlatform { get; set; }
                public long? Version { get; set; }
                public List<CustomerOrder> CustomerOrderList { get; } = new List<CustomerOrder>();

        private string _comment;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
        }

        public OrderStatus MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public OrderStatus MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public OrderStatus AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public static OrderStatus Refer(long id)
        {
            return new OrderStatus { Id = id }.MarkLoadedOnly("Id");
        }

        public static OrderStatus FromRecord(Record record)
        {
            var entity = new OrderStatus().MarkLoadedOnly();
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
                    if (record.TryGetValue("code", out var codeValue))
                    {
                        entity.MarkLoaded("Code");
                        if (codeValue.Raw != null)
                            entity.Code = Convert.ToString(codeValue.Raw);
                    }
                    if (record.TryGetValue("color", out var colorValue))
                    {
                        entity.MarkLoaded("Color");
                        if (colorValue.Raw != null)
                            entity.Color = Convert.ToString(colorValue.Raw);
                    }
                    if (record.TryGetValue("display_order", out var displayOrderValue))
                    {
                        entity.MarkLoaded("DisplayOrder");
                        if (displayOrderValue.Raw != null)
                            entity.DisplayOrder = Convert.ToDecimal(displayOrderValue.Raw);
                    }
                    if (record.TryGetValue("commerce_platform", out var commercePlatformValue))
                    {
                        entity.MarkLoaded("CommercePlatform");
                        if (commercePlatformValue.Raw != null)
                            entity.CommercePlatform = Convert.ToInt64(commercePlatformValue.Raw);
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
                child.UpdateStatusId(Id);
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

                    if (Code != null) record["code"] = new Value.TextValue(Code);

                    if (Color != null) record["color"] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record["display_order"] = new Value.DecimalValue(DisplayOrder.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderStatus", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Code != null) record["code"] = new Value.TextValue(Code);

                    if (Color != null) record["color"] = new Value.TextValue(Color);

                    if (DisplayOrder.HasValue) record["display_order"] = new Value.DecimalValue(DisplayOrder.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "OrderStatus", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("OrderStatus");
        }

                public OrderStatus UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    return this;
                }

                public OrderStatus UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    return this;
                }

                public OrderStatus UpdateCode(string value)
                {
                    this.Code = value;
                    MarkLoaded("Code");
                    return this;
                }

                public OrderStatus UpdateColor(string value)
                {
                    this.Color = value;
                    MarkLoaded("Color");
                    return this;
                }

                public OrderStatus UpdateDisplayOrder(decimal? value)
                {
                    this.DisplayOrder = value;
                    MarkLoaded("DisplayOrder");
                    return this;
                }

                public OrderStatus UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

                public OrderStatus UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    return this;
                }
                public OrderStatus UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    return this;
                }


                public OrderStatus UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

    }
}