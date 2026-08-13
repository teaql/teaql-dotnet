using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeaQL.Core;

namespace Generated.Models
{
    public class CommercePlatform
    {
                public long? Id { get; set; }
                public string Name { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public List<Customer> CustomerList { get; } = new List<Customer>();
                public List<OrderStatus> OrderStatusList { get; } = new List<OrderStatus>();
                public List<CustomerOrder> CustomerOrderList { get; } = new List<CustomerOrder>();
                public List<Product> ProductList { get; } = new List<Product>();
                public List<OrderLine> OrderLineList { get; } = new List<OrderLine>();
                public List<OrderSearchPreset> OrderSearchPresetList { get; } = new List<OrderSearchPreset>();

        private string _comment;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
        }

        public CommercePlatform MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public CommercePlatform MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public CommercePlatform AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public static CommercePlatform Refer(long id)
        {
            return new CommercePlatform { Id = id }.MarkLoadedOnly("Id");
        }

        public static CommercePlatform FromRecord(Record record)
        {
            var entity = new CommercePlatform().MarkLoadedOnly();
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
                        if (record.TryGetValue("CustomerList", out var customerListValue))
                        {
                            entity.MarkLoaded("CustomerList");
                            var rows = customerListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.CustomerList.Add(Customer.FromRecord(row));
                        }
                        if (record.TryGetValue("OrderStatusList", out var orderStatusListValue))
                        {
                            entity.MarkLoaded("OrderStatusList");
                            var rows = orderStatusListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.OrderStatusList.Add(OrderStatus.FromRecord(row));
                        }
                        if (record.TryGetValue("CustomerOrderList", out var customerOrderListValue))
                        {
                            entity.MarkLoaded("CustomerOrderList");
                            var rows = customerOrderListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.CustomerOrderList.Add(CustomerOrder.FromRecord(row));
                        }
                        if (record.TryGetValue("ProductList", out var productListValue))
                        {
                            entity.MarkLoaded("ProductList");
                            var rows = productListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.ProductList.Add(Product.FromRecord(row));
                        }
                        if (record.TryGetValue("OrderLineList", out var orderLineListValue))
                        {
                            entity.MarkLoaded("OrderLineList");
                            var rows = orderLineListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.OrderLineList.Add(OrderLine.FromRecord(row));
                        }
                        if (record.TryGetValue("OrderSearchPresetList", out var orderSearchPresetListValue))
                        {
                            entity.MarkLoaded("OrderSearchPresetList");
                            var rows = orderSearchPresetListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.OrderSearchPresetList.Add(OrderSearchPreset.FromRecord(row));
                        }
            return entity;
        }

        public async Task<object> SaveAsync(UserContext ctx)
        {
            if (string.IsNullOrEmpty(_comment))
            {
                throw new Exception("Security audit failure: AuditAs() must be called before SaveAsync()");
            }

            var creating = !this.Id.HasValue;
            var cmd = creating ? (object)ToInsertCommand() : (object)ToUpdateCommand();
            var req = new MutationRequest { Command = cmd, Comment = _comment };
            var result = await ctx.DataService.MutateAsync(ctx, req);
            if (result is MutationResult mutationResult)
            {
                if (creating) Id = mutationResult.Id;
                if (!mutationResult.Deleted) Version = mutationResult.Version;
            }
            foreach (var child in CustomerList)
            {
                child.UpdateCommercePlatformId(Id);
                child.AuditAs(_comment);
                await child.SaveAsync(ctx);
            }
            foreach (var child in OrderStatusList)
            {
                child.UpdateCommercePlatformId(Id);
                child.AuditAs(_comment);
                await child.SaveAsync(ctx);
            }
            foreach (var child in CustomerOrderList)
            {
                child.UpdateCommercePlatformId(Id);
                child.AuditAs(_comment);
                await child.SaveAsync(ctx);
            }
            foreach (var child in ProductList)
            {
                child.UpdateCommercePlatformId(Id);
                child.AuditAs(_comment);
                await child.SaveAsync(ctx);
            }
            foreach (var child in OrderLineList)
            {
                child.UpdateCommercePlatformId(Id);
                child.AuditAs(_comment);
                await child.SaveAsync(ctx);
            }
            foreach (var child in OrderSearchPresetList)
            {
                child.UpdateCommercePlatformId(Id);
                child.AuditAs(_comment);
                await child.SaveAsync(ctx);
            }
            return result;
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "CommercePlatform", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "CommercePlatform", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("CommercePlatform");
        }

                public CommercePlatform UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    return this;
                }

                public CommercePlatform UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    return this;
                }

                public CommercePlatform UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    return this;
                }

                public CommercePlatform UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    return this;
                }

                public CommercePlatform UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    return this;
                }
    }
}