using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeaQL.Core;

namespace Generated.Models
{
    public class OrderLine
    {
                public long? Id { get; set; }
                public long? CustomerOrder { get; set; }
                public long? Product { get; set; }
                public string ProductName { get; set; }
                public string Sku { get; set; }
                public long? Quantity { get; set; }
                public long? CommercePlatform { get; set; }
                public DateTime? CreateTime { get; set; }
                public long? Version { get; set; }

        private string _comment;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
        }

        public OrderLine MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public OrderLine MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public OrderLine AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public static OrderLine Refer(long id)
        {
            return new OrderLine { Id = id }.MarkLoadedOnly("Id");
        }

        public static OrderLine FromRecord(Record record)
        {
            var entity = new OrderLine().MarkLoadedOnly();
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("customer_order", out var customerOrderValue))
                    {
                        entity.MarkLoaded("CustomerOrder");
                        if (customerOrderValue.Raw != null)
                            entity.CustomerOrder = Convert.ToInt64(customerOrderValue.Raw);
                    }
                    if (record.TryGetValue("product", out var productValue))
                    {
                        entity.MarkLoaded("Product");
                        if (productValue.Raw != null)
                            entity.Product = Convert.ToInt64(productValue.Raw);
                    }
                    if (record.TryGetValue("product_name", out var productNameValue))
                    {
                        entity.MarkLoaded("ProductName");
                        if (productNameValue.Raw != null)
                            entity.ProductName = Convert.ToString(productNameValue.Raw);
                    }
                    if (record.TryGetValue("sku", out var skuValue))
                    {
                        entity.MarkLoaded("Sku");
                        if (skuValue.Raw != null)
                            entity.Sku = Convert.ToString(skuValue.Raw);
                    }
                    if (record.TryGetValue("quantity", out var quantityValue))
                    {
                        entity.MarkLoaded("Quantity");
                        if (quantityValue.Raw != null)
                            entity.Quantity = Convert.ToInt64(quantityValue.Raw);
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
                    if (record.TryGetValue("version", out var versionValue))
                    {
                        entity.MarkLoaded("Version");
                        if (versionValue.Raw != null)
                            entity.Version = Convert.ToInt64(versionValue.Raw);
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
            return result;
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (CustomerOrder.HasValue) record["customer_order"] = new Value.I64Value(CustomerOrder.Value);

                    if (Product.HasValue) record["product"] = new Value.I64Value(Product.Value);

                    if (ProductName != null) record["product_name"] = new Value.TextValue(ProductName);

                    if (Sku != null) record["sku"] = new Value.TextValue(Sku);

                    if (Quantity.HasValue) record["quantity"] = new Value.I64Value(Quantity.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderLine", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (CustomerOrder.HasValue) record["customer_order"] = new Value.I64Value(CustomerOrder.Value);

                    if (Product.HasValue) record["product"] = new Value.I64Value(Product.Value);

                    if (ProductName != null) record["product_name"] = new Value.TextValue(ProductName);

                    if (Sku != null) record["sku"] = new Value.TextValue(Sku);

                    if (Quantity.HasValue) record["quantity"] = new Value.I64Value(Quantity.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "OrderLine", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("OrderLine");
        }

                public OrderLine UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    return this;
                }

                public OrderLine UpdateCustomerOrder(long? value)
                {
                    this.CustomerOrder = value;
                    MarkLoaded("CustomerOrder");
                    return this;
                }

                public OrderLine UpdateProduct(long? value)
                {
                    this.Product = value;
                    MarkLoaded("Product");
                    return this;
                }

                public OrderLine UpdateProductName(string value)
                {
                    this.ProductName = value;
                    MarkLoaded("ProductName");
                    return this;
                }

                public OrderLine UpdateSku(string value)
                {
                    this.Sku = value;
                    MarkLoaded("Sku");
                    return this;
                }

                public OrderLine UpdateQuantity(long? value)
                {
                    this.Quantity = value;
                    MarkLoaded("Quantity");
                    return this;
                }

                public OrderLine UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

                public OrderLine UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    return this;
                }

                public OrderLine UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    return this;
                }
                public OrderLine UpdateCustomerOrder(CustomerOrder value)
                {
                    this.CustomerOrder = value?.Id;
                    MarkLoaded("CustomerOrder");
                    return this;
                }


                public OrderLine UpdateCustomerOrderId(long? value)
                {
                    this.CustomerOrder = value;
                    MarkLoaded("CustomerOrder");
                    return this;
                }


                public OrderLine UpdateProduct(Product value)
                {
                    this.Product = value?.Id;
                    MarkLoaded("Product");
                    return this;
                }


                public OrderLine UpdateProductId(long? value)
                {
                    this.Product = value;
                    MarkLoaded("Product");
                    return this;
                }


                public OrderLine UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    return this;
                }


                public OrderLine UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

    }
}