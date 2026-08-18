using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;

namespace TeaQL.TfpEndpoint
{
    public class TfpEndpointHandler
    {
        private readonly IDataService _dataService;
        private readonly IRuntimeTelemetry _telemetry;

        public TfpEndpointHandler(IDataService dataService, IRuntimeTelemetry? telemetry = null)
        {
            _dataService = dataService;
            _telemetry = telemetry ?? NoopRuntimeTelemetry.Instance;
        }

        private Value MapToValue(object? obj)
        {
            if (obj == null) return new Value.NullValue();
            return obj switch
            {
                bool b => new Value.BoolValue(b),
                int i => new Value.I64Value(i),
                long l => new Value.I64Value(l),
                uint ui => new Value.U64Value(ui),
                ulong ul => new Value.U64Value(ul),
                double d => new Value.F64Value(d),
                decimal dec => new Value.DecimalValue(dec),
                string s => new Value.TextValue(s),
                JsonElement e => MapJsonElementToValue(e),
                _ => new Value.TextValue(obj.ToString() ?? "")
            };
        }

        private Value MapJsonElementToValue(JsonElement e)
        {
            return e.ValueKind switch
            {
                JsonValueKind.Null => new Value.NullValue(),
                JsonValueKind.True => new Value.BoolValue(true),
                JsonValueKind.False => new Value.BoolValue(false),
                JsonValueKind.Number => e.TryGetInt64(out var l) ? new Value.I64Value(l) : new Value.F64Value(e.GetDouble()),
                JsonValueKind.String => new Value.TextValue(e.GetString() ?? ""),
                _ => new Value.TextValue(e.GetRawText())
            };
        }

        public Task<Dictionary<string, object>> HandleQueryAsync(string payloadJson) =>
            _telemetry.ObserveAsync(
                RuntimeOperation.Create("tfp", "server.query",
                    new Dictionary<string, object> { ["teaql.tfp.role"] = "server" }),
                () => HandleQueryCoreAsync(payloadJson),
                result => new Dictionary<string, object>
                {
                    ["teaql.result.cardinality"] = ((List<Dictionary<string, object?>>)result["data"]).Count
                });

        private async Task<Dictionary<string, object>> HandleQueryCoreAsync(string payloadJson)
        {
            var tfpQuery = JsonSerializer.Deserialize<TfpSelectQuery>(payloadJson);
            if (tfpQuery == null)
                throw new ArgumentException("Failed to parse JSON payload");

            var q = new SelectQuery(tfpQuery.Entity);
            
            if (tfpQuery.LimitValue.HasValue)
                q.Limit(tfpQuery.LimitValue.Value);
                
            if (tfpQuery.OffsetValue.HasValue)
                q.Offset(tfpQuery.OffsetValue.Value);

            if (tfpQuery.OrderItems != null)
            {
                foreach (var o in tfpQuery.OrderItems)
                {
                    var dir = o.Direction == "Desc" ? SortDirection.Desc : SortDirection.Asc;
                    q.OrderBy(Core.OrderBy.New(o.Field, dir));
                }
            }

            // Implicit soft delete filter: version > 0
            q.Filter(Expr.Gt("version", new Value.I64Value(0)));

            if (tfpQuery.SelectItems != null)
                q.Projects(tfpQuery.SelectItems.ToArray());

            if (tfpQuery.GroupByItems != null)
            {
                foreach (var g in tfpQuery.GroupByItems)
                {
                    q.GroupBy(g);
                }
            }

            if (tfpQuery.CommentText != null)
                q.Comment(tfpQuery.CommentText);

            var req = new QueryRequest
            {
                Query = q,
                Comment = tfpQuery.CommentText
            };

            var res = await _dataService.QueryAsync(req);

            var rows = new List<Dictionary<string, object?>>();
            foreach (var r in res.Rows)
            {
                var dict = new Dictionary<string, object?>();
                foreach (var kvp in r)
                {
                    dict[kvp.Key] = kvp.Value.ToJsonValue();
                }
                rows.Add(dict);
            }

            return new Dictionary<string, object>
            {
                ["data"] = rows,
                ["resultCode"] = 0,
                ["status"] = "YES"
            };
        }

        public Task<Dictionary<string, object>> HandleMutationAsync(string payloadJson) =>
            _telemetry.ObserveAsync(
                RuntimeOperation.Create("tfp", "server.mutation",
                    new Dictionary<string, object> { ["teaql.tfp.role"] = "server" }),
                () => HandleMutationCoreAsync(payloadJson));

        private async Task<Dictionary<string, object>> HandleMutationCoreAsync(string payloadJson)
        {
            var tfpMut = JsonSerializer.Deserialize<TfpMutationQuery>(payloadJson);
            if (tfpMut == null)
                throw new ArgumentException("Failed to parse JSON payload");

            var trace = new List<TraceNode> { new TraceNode(tfpMut.Entity, null, tfpMut.Comment ?? "") };

            var record = new Record();
            if (tfpMut.Payload != null)
            {
                foreach (var kv in tfpMut.Payload)
                {
                    record[kv.Key] = MapToValue(kv.Value);
                }
            }

            Value idVal = MapToValue(tfpMut.Id);

            long? expectedVersion = null;
            if (tfpMut.Payload != null && tfpMut.Payload.TryGetValue("version", out var versionObj))
            {
                if (versionObj is System.Text.Json.JsonElement je && je.ValueKind == System.Text.Json.JsonValueKind.Number)
                {
                    expectedVersion = je.GetInt64();
                }
            }

            MutationRequest mutReq = tfpMut.Action switch
            {
                "Create" => new InsertMutationRequest(new InsertCommand { Entity = tfpMut.Entity, Values = record, TraceChain = trace }),
                "Update" => new UpdateMutationRequest(new UpdateCommand { Entity = tfpMut.Entity, Id = idVal, ExpectedVersionValue = expectedVersion, Values = record, TraceChain = trace }),
                "Delete" => new DeleteMutationRequest(new DeleteCommand { Entity = tfpMut.Entity, Id = idVal, ExpectedVersionValue = expectedVersion, SoftDelete = true, TraceChain = trace }),
                "Recover" => new RecoverMutationRequest(new RecoverCommand { Entity = tfpMut.Entity, Id = idVal, TraceChain = trace }),
                _ => throw new ArgumentException($"Unknown mutation action: {tfpMut.Action}")
            };

            var res = await _dataService.MutateAsync(mutReq);

            var dataArr = new List<Dictionary<string, object?>>();
            if (res.GeneratedValues != null && res.GeneratedValues.Count > 0)
            {
                var m = new Dictionary<string, object?>();
                foreach (var kv in res.GeneratedValues)
                {
                    m[kv.Key] = kv.Value.ToJsonValue();
                }
                dataArr.Add(m);
            }

            return new Dictionary<string, object>
            {
                ["affectedRows"] = res.AffectedRows,
                ["resultCode"] = 0,
                ["status"] = "YES",
                ["data"] = dataArr
            };
        }
    }
}
