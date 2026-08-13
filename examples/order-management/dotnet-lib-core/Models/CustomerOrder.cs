using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeaQL.Core;

namespace Generated.Models
{
    public class CustomerOrder
    {
                public long? Id { get; set; }
                public string OrderNumber { get; set; }
                public DateTime? OrderDate { get; set; }
                public decimal? TotalAmount { get; set; }
                public long? Status { get; set; }
                public long? Customer { get; set; }
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

        public CustomerOrder MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public CustomerOrder MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public CustomerOrder AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public static CustomerOrder Refer(long id)
        {
            return new CustomerOrder { Id = id }.MarkLoadedOnly("Id");
        }

        public static CustomerOrder FromRecord(Record record)
        {
            var entity = new CustomerOrder().MarkLoadedOnly();
                    if (record.TryGetValue("id", out var idValue))
                    {
                        entity.MarkLoaded("Id");
                        if (idValue.Raw != null)
                            entity.Id = Convert.ToInt64(idValue.Raw);
                    }
                    if (record.TryGetValue("order_number", out var orderNumberValue))
                    {
                        entity.MarkLoaded("OrderNumber");
                        if (orderNumberValue.Raw != null)
                            entity.OrderNumber = Convert.ToString(orderNumberValue.Raw);
                    }
                    if (record.TryGetValue("order_date", out var orderDateValue))
                    {
                        entity.MarkLoaded("OrderDate");
                        if (orderDateValue.Raw != null)
                            entity.OrderDate = Convert.ToDateTime(orderDateValue.Raw);
                    }
                    if (record.TryGetValue("total_amount", out var totalAmountValue))
                    {
                        entity.MarkLoaded("TotalAmount");
                        if (totalAmountValue.Raw != null)
                            entity.TotalAmount = Convert.ToDecimal(totalAmountValue.Raw);
                    }
                    if (record.TryGetValue("status", out var statusValue))
                    {
                        entity.MarkLoaded("Status");
                        if (statusValue.Raw != null)
                            entity.Status = Convert.ToInt64(statusValue.Raw);
                    }
                    if (record.TryGetValue("customer", out var customerValue))
                    {
                        entity.MarkLoaded("Customer");
                        if (customerValue.Raw != null)
                            entity.Customer = Convert.ToInt64(customerValue.Raw);
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
                child.UpdateCustomerOrderId(Id);
                child.AuditAs(_comment);
                await child.SaveAsync(ctx);
            }
            return result;
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (OrderNumber != null) record["order_number"] = new Value.TextValue(OrderNumber);

                    if (OrderDate.HasValue) record["order_date"] = new Value.DateValue(OrderDate.Value);

                    if (TotalAmount.HasValue) record["total_amount"] = new Value.DecimalValue(TotalAmount.Value);

                    if (Status.HasValue) record["status"] = new Value.I64Value(Status.Value);

                    if (Customer.HasValue) record["customer"] = new Value.I64Value(Customer.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "CustomerOrder", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (OrderNumber != null) record["order_number"] = new Value.TextValue(OrderNumber);

                    if (OrderDate.HasValue) record["order_date"] = new Value.DateValue(OrderDate.Value);

                    if (TotalAmount.HasValue) record["total_amount"] = new Value.DecimalValue(TotalAmount.Value);

                    if (Status.HasValue) record["status"] = new Value.I64Value(Status.Value);

                    if (Customer.HasValue) record["customer"] = new Value.I64Value(Customer.Value);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "CustomerOrder", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("CustomerOrder");
        }

                public CustomerOrder UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    return this;
                }

                public CustomerOrder UpdateOrderNumber(string value)
                {
                    this.OrderNumber = value;
                    MarkLoaded("OrderNumber");
                    return this;
                }

                public CustomerOrder UpdateOrderDate(DateTime? value)
                {
                    this.OrderDate = value;
                    MarkLoaded("OrderDate");
                    return this;
                }

                public CustomerOrder UpdateTotalAmount(decimal? value)
                {
                    this.TotalAmount = value;
                    MarkLoaded("TotalAmount");
                    return this;
                }

                public CustomerOrder UpdateStatus(long? value)
                {
                    this.Status = value;
                    MarkLoaded("Status");
                    return this;
                }

                public CustomerOrder UpdateCustomer(long? value)
                {
                    this.Customer = value;
                    MarkLoaded("Customer");
                    return this;
                }

                public CustomerOrder UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

                public CustomerOrder UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    return this;
                }

                public CustomerOrder UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    return this;
                }

                public CustomerOrder UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    return this;
                }
                public CustomerOrder UpdateStatus(OrderStatus value)
                {
                    this.Status = value?.Id;
                    MarkLoaded("Status");
                    return this;
                }


                public CustomerOrder UpdateStatusId(long? value)
                {
                    this.Status = value;
                    MarkLoaded("Status");
                    return this;
                }
                public CustomerOrder UpdateStatusToPending()
                {
                    this.Status = 1001;
                    MarkLoaded("Status");
                    return this;
                }

                public CustomerOrder UpdateStatusToProcessing()
                {
                    this.Status = 1002;
                    MarkLoaded("Status");
                    return this;
                }

                public CustomerOrder UpdateStatusToShipped()
                {
                    this.Status = 1003;
                    MarkLoaded("Status");
                    return this;
                }

                public CustomerOrder UpdateStatusToCompleted()
                {
                    this.Status = 1004;
                    MarkLoaded("Status");
                    return this;
                }



                public CustomerOrder UpdateCustomer(Customer value)
                {
                    this.Customer = value?.Id;
                    MarkLoaded("Customer");
                    return this;
                }


                public CustomerOrder UpdateCustomerId(long? value)
                {
                    this.Customer = value;
                    MarkLoaded("Customer");
                    return this;
                }


                public CustomerOrder UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    return this;
                }


                public CustomerOrder UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

    }
}