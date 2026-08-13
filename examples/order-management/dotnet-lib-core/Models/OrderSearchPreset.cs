using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeaQL.Core;

namespace Generated.Models
{
    public class OrderSearchPreset
    {
                public long? Id { get; set; }
                public string Name { get; set; }
                public string FilterJson { get; set; }
                public string RequestId { get; set; }
                public string OwnerUserId { get; set; }
                public long? CommercePlatform { get; set; }
                public DateTime? CreateTime { get; set; }
                public DateTime? UpdateTime { get; set; }
                public long? Version { get; set; }

        private string _comment;
        private bool _fullyLoaded = true;
        private HashSet<string> _loadedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool IsLoaded(string field)
        {
            return _fullyLoaded || _loadedFields.Contains(field);
        }

        public OrderSearchPreset MarkLoaded(params string[] fields)
        {
            foreach (var field in fields) _loadedFields.Add(field);
            return this;
        }

        public OrderSearchPreset MarkLoadedOnly(params string[] fields)
        {
            _fullyLoaded = false;
            _loadedFields = new HashSet<string>(fields, StringComparer.OrdinalIgnoreCase);
            return this;
        }

        public OrderSearchPreset AuditAs(string comment)
        {
            _comment = comment;
            return this;
        }

        public static OrderSearchPreset Refer(long id)
        {
            return new OrderSearchPreset { Id = id }.MarkLoadedOnly("Id");
        }

        public static OrderSearchPreset FromRecord(Record record)
        {
            var entity = new OrderSearchPreset().MarkLoadedOnly();
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
                    if (record.TryGetValue("filter_json", out var filterJsonValue))
                    {
                        entity.MarkLoaded("FilterJson");
                        if (filterJsonValue.Raw != null)
                            entity.FilterJson = Convert.ToString(filterJsonValue.Raw);
                    }
                    if (record.TryGetValue("request_id", out var requestIdValue))
                    {
                        entity.MarkLoaded("RequestId");
                        if (requestIdValue.Raw != null)
                            entity.RequestId = Convert.ToString(requestIdValue.Raw);
                    }
                    if (record.TryGetValue("owner_user_id", out var ownerUserIdValue))
                    {
                        entity.MarkLoaded("OwnerUserId");
                        if (ownerUserIdValue.Raw != null)
                            entity.OwnerUserId = Convert.ToString(ownerUserIdValue.Raw);
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
            return result;
        }

        public InsertCommand ToInsertCommand()
        {
            var record = new Record();
                    if (Id.HasValue) record["id"] = new Value.I64Value(Id.Value);

                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (FilterJson != null) record["filter_json"] = new Value.TextValue(FilterJson);

                    if (RequestId != null) record["request_id"] = new Value.TextValue(RequestId);

                    if (OwnerUserId != null) record["owner_user_id"] = new Value.TextValue(OwnerUserId);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new InsertCommand { Entity = "OrderSearchPreset", Values = record };
        }

        public UpdateCommand ToUpdateCommand()
        {
            var record = new Record();
                    if (Name != null) record["name"] = new Value.TextValue(Name);

                    if (FilterJson != null) record["filter_json"] = new Value.TextValue(FilterJson);

                    if (RequestId != null) record["request_id"] = new Value.TextValue(RequestId);

                    if (OwnerUserId != null) record["owner_user_id"] = new Value.TextValue(OwnerUserId);

                    if (CommercePlatform.HasValue) record["commerce_platform"] = new Value.I64Value(CommercePlatform.Value);

                    if (CreateTime.HasValue) record["create_time"] = new Value.DateValue(CreateTime.Value);

                    if (UpdateTime.HasValue) record["update_time"] = new Value.DateValue(UpdateTime.Value);

                    if (Version.HasValue) record["version"] = new Value.I64Value(Version.Value);

            return new UpdateCommand { 
                Entity = "OrderSearchPreset", 
                Id = this.Id.HasValue ? new Value.I64Value(this.Id.Value) : null, 
                Values = record 
            };
        }

        public SelectQuery ToSelectQuery()
        {
            return new SelectQuery("OrderSearchPreset");
        }

                public OrderSearchPreset UpdateId(long? value)
                {
                    this.Id = value;
                    MarkLoaded("Id");
                    return this;
                }

                public OrderSearchPreset UpdateName(string value)
                {
                    this.Name = value;
                    MarkLoaded("Name");
                    return this;
                }

                public OrderSearchPreset UpdateFilterJson(string value)
                {
                    this.FilterJson = value;
                    MarkLoaded("FilterJson");
                    return this;
                }

                public OrderSearchPreset UpdateRequestId(string value)
                {
                    this.RequestId = value;
                    MarkLoaded("RequestId");
                    return this;
                }

                public OrderSearchPreset UpdateOwnerUserId(string value)
                {
                    this.OwnerUserId = value;
                    MarkLoaded("OwnerUserId");
                    return this;
                }

                public OrderSearchPreset UpdateCommercePlatform(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

                public OrderSearchPreset UpdateCreateTime(DateTime? value)
                {
                    this.CreateTime = value;
                    MarkLoaded("CreateTime");
                    return this;
                }

                public OrderSearchPreset UpdateUpdateTime(DateTime? value)
                {
                    this.UpdateTime = value;
                    MarkLoaded("UpdateTime");
                    return this;
                }

                public OrderSearchPreset UpdateVersion(long? value)
                {
                    this.Version = value;
                    MarkLoaded("Version");
                    return this;
                }
                public OrderSearchPreset UpdateCommercePlatform(CommercePlatform value)
                {
                    this.CommercePlatform = value?.Id;
                    MarkLoaded("CommercePlatform");
                    return this;
                }


                public OrderSearchPreset UpdateCommercePlatformId(long? value)
                {
                    this.CommercePlatform = value;
                    MarkLoaded("CommercePlatform");
                    return this;
                }

    }
}