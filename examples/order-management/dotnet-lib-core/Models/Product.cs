using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeaQL.Core;

namespace Generated.Models
{
    public class Product
    {
                public long? Id { get; set; }
                public string Name { get; set; }
                public string Sku { get; set; }
                public string ImageUrl { get; set; }
                public long? CommercePlatform { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }
                public List<OrderLine> OrderLineList { get; } = new List<OrderLine>();

        private string _comment;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
        }

        public Product MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public Product MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public Product AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public static Product Refer(long id)
        {
            return new Product { Id = id }.MarkLoadedOnly("Id");
        }

        public static Product FromRecord(Record record)
        {
            var entity = new Product().MarkLoadedOnly();
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
                    if (record.TryGetValue("sku", out var skuValue))
                    {
                        entity.MarkLoaded("Sku");
                        if (skuValue.Raw != null)
                            entity.Sku = Convert.ToString(skuValue.Raw);
                    }
                    if (record.TryGetValue("image_url", out var imageUrlValue))
                    {
                        entity.MarkLoaded("ImageUrl");
                        if (imageUrlValue.Raw != null)
                            entity.ImageUrl = Convert.ToString(imageUrlValue.Raw);
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
                        if (record.TryGetValue("OrderLineList", out var orderLineListValue))
                        {
                            entity.MarkLoaded("OrderLineList");
                            var rows = orderLineListValue.Raw as IEnumerable<Record>;
                            if (rows != null)
                                foreach (var row in rows) entity.OrderLineList.Add(OrderLine.FromRecord(row));
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
            foreach (var child in OrderLineList)
            {
                child.UpdateProductId(Id);
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

                    if (Sku != null) record["sku"] = new Value.TextValue(Sku);

                    if (ImageUrl != null) record["image_url"] = new Value.TextValue(ImageUrl);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "Product", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (Sku != null) record["sku"] = new Value.TextValue(Sku);

                    if (ImageUrl != null) record["image_url"] = new Value.TextValue(ImageUrl);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "Product", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("Product");
        }

                public Product UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    return this;
                }

                public Product UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    return this;
                }

                public Product UpdateSku(string value)
                {
                    this.Sku = value;
                    MarkLoaded("Sku");
                    return this;
                }

                public Product UpdateImageUrl(string value)
                {
                    this.ImageUrl = value;
                    MarkLoaded("ImageUrl");
                    return this;
                }

                public Product UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

                public Product UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    return this;
                }

                public Product UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    return this;
                }

                public Product UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    return this;
                }
                public Product UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    return this;
                }


                public Product UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

    }
}