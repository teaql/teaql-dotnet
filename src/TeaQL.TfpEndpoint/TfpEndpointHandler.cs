using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new TfpEndpointException("TFP_UNAUTHORIZED", "Trusted federation context is required");

        public async Task<Dictionary<string, object>> HandleQueryAsync(
            string payloadJson, IReadOnlyDictionary<string, string> carrier)
            => throw new TfpEndpointException("TFP_UNAUTHORIZED", "Trusted federation context is required");

        public Task<Dictionary<string, object>> HandleQueryAsync(
            TrustedFederalContext trusted, string payloadJson) =>
            HandleQueryAsync(trusted, payloadJson, new Dictionary<string, string>());

        public async Task<Dictionary<string, object>> HandleQueryAsync(
            TrustedFederalContext trusted, string payloadJson, IReadOnlyDictionary<string, string> carrier)
        {
            using var propagation = _telemetry.ActivateSafely(carrier);
            return await _telemetry.ObserveAsync(
                RuntimeOperation.Create("tfp", "server.query",
                    new Dictionary<string, object> { ["teaql.tfp.role"] = "server" }),
                () => HandleQueryCoreAsync(trusted, payloadJson),
                result => new Dictionary<string, object>
                {
                    ["teaql.result.cardinality"] = ((List<Dictionary<string, object?>>)result["data"]).Count
                }).ConfigureAwait(false);
        }

        private async Task<Dictionary<string, object>> HandleQueryCoreAsync(
            TrustedFederalContext trusted, string payloadJson)
        {
            RejectPrivilegedInput(payloadJson, new HashSet<string> {
                "entity", "filterCondition", "limitValue", "offsetValue", "orderItems",
                "selectItems", "groupByItems", "aggregateItems", "commentText", "purposeText"
            });
            var tfpQuery = JsonSerializer.Deserialize<TfpSelectQuery>(payloadJson);
            if (tfpQuery == null)
                throw new ArgumentException("Failed to parse JSON payload");

            RequireEntity(trusted, tfpQuery.Entity);
            if (!trusted.ReadableFields.TryGetValue(tfpQuery.Entity, out var fields))
                throw new TfpEndpointException("TFP_POLICY_VIOLATION", "No readable field policy");
            if (string.IsNullOrWhiteSpace(tfpQuery.CommentText))
                throw new TfpEndpointException("TFP_INVALID_REQUEST", "commentText is required");
            if (string.IsNullOrWhiteSpace(tfpQuery.PurposeText))
                throw new TfpEndpointException("TFP_POLICY_VIOLATION", "purposeText is required");

            var q = new SelectQuery(tfpQuery.Entity);
            
            if (tfpQuery.LimitValue.HasValue)
            {
                if (tfpQuery.LimitValue.Value < 1 || tfpQuery.LimitValue.Value > (ulong)trusted.MaxPageSize)
                    throw new TfpEndpointException("TFP_POLICY_VIOLATION", "Invalid federation page size");
                q.Limit(tfpQuery.LimitValue.Value);
            }
                
            if (tfpQuery.OffsetValue.HasValue)
                q.Offset(tfpQuery.OffsetValue.Value);

            if (tfpQuery.OrderItems != null)
            {
                foreach (var o in tfpQuery.OrderItems)
                {
                    var dir = o.Direction == "Desc" ? SortDirection.Desc : SortDirection.Asc;
                    if (!string.Equals(o.Direction, "Asc", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(o.Direction, "Desc", StringComparison.OrdinalIgnoreCase))
                        throw new TfpEndpointException("TFP_INVALID_REQUEST", "Unsupported order direction");
                    q.OrderBy(Core.OrderBy.New(MapField(fields, o.Field), dir));
                }
            }

            if (tfpQuery.FilterCondition != null)
                q.Filter(ParseFilter(JsonSerializer.SerializeToElement(tfpQuery.FilterCondition), fields));
            q.AndFilter(Expr.Eq(trusted.TenantField, trusted.TenantId));
            q.AndFilter(Expr.Gt("version", new Value.I64Value(0)));

            if (tfpQuery.SelectItems != null)
                q.Projects(tfpQuery.SelectItems.Select(field => MapField(fields, field)).ToArray());

            if (tfpQuery.GroupByItems != null)
            {
                foreach (var g in tfpQuery.GroupByItems)
                {
                    q.GroupBy(MapField(fields, g));
                }
            }
            if (tfpQuery.AggregateItems is { Count: > 0 })
                throw new TfpEndpointException("TFP_INVALID_REQUEST",
                    "Aggregation is not supported by this endpoint");

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
            throw new TfpEndpointException("TFP_UNAUTHORIZED", "Trusted federation context is required");

        public async Task<Dictionary<string, object>> HandleMutationAsync(
            string payloadJson, IReadOnlyDictionary<string, string> carrier)
            => throw new TfpEndpointException("TFP_UNAUTHORIZED", "Trusted federation context is required");

        public Task<Dictionary<string, object>> HandleMutationAsync(
            TrustedFederalContext trusted, string payloadJson) =>
            HandleMutationAsync(trusted, payloadJson, new Dictionary<string, string>());

        public async Task<Dictionary<string, object>> HandleMutationAsync(
            TrustedFederalContext trusted, string payloadJson, IReadOnlyDictionary<string, string> carrier)
        {
            using var propagation = _telemetry.ActivateSafely(carrier);
            return await _telemetry.ObserveAsync(
                RuntimeOperation.Create("tfp", "server.mutation",
                    new Dictionary<string, object> { ["teaql.tfp.role"] = "server" }),
                () => HandleMutationCoreAsync(trusted, payloadJson)).ConfigureAwait(false);
        }

        private async Task<Dictionary<string, object>> HandleMutationCoreAsync(
            TrustedFederalContext trusted, string payloadJson)
        {
            RejectPrivilegedInput(payloadJson, new HashSet<string> {
                "entity", "action", "payload", "id", "expectedVersion", "comment"
            });
            var tfpMut = JsonSerializer.Deserialize<TfpMutationQuery>(payloadJson);
            if (tfpMut == null)
                throw new ArgumentException("Failed to parse JSON payload");
            RequireEntity(trusted, tfpMut.Entity);
            if (string.IsNullOrWhiteSpace(tfpMut.Comment))
                throw new TfpEndpointException("TFP_AUDIT_REASON_REQUIRED", "Mutation audit reason is required");
            if (!trusted.AllowedActions.TryGetValue(tfpMut.Entity, out var actions)
                || !actions.Contains(tfpMut.Action))
                throw new TfpEndpointException("TFP_POLICY_VIOLATION", "Mutation action is not allowed");
            if (!trusted.WritableFields.TryGetValue(tfpMut.Entity, out var writable))
                throw new TfpEndpointException("TFP_POLICY_VIOLATION", "No writable field policy");

            var trace = new List<TraceNode> { new TraceNode(tfpMut.Entity, null, tfpMut.Comment ?? "") };

            var record = new Record();
            if (tfpMut.Payload != null)
            {
                foreach (var kv in tfpMut.Payload)
                {
                    if (!writable.TryGetValue(kv.Key, out var mapped))
                        throw new TfpEndpointException("TFP_FORBIDDEN_FIELD", $"Mutation field is not allowed: {kv.Key}");
                    record[mapped] = MapToValue(kv.Value);
                }
            }
            record[trusted.TenantField] = trusted.TenantId;

            Value idVal = MapToValue(tfpMut.Id);

            long? expectedVersion = tfpMut.ExpectedVersion;

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

        private static void RequireEntity(TrustedFederalContext trusted, string entity)
        {
            if (string.IsNullOrWhiteSpace(entity) || !trusted.AllowedEntities.Contains(entity))
                throw new TfpEndpointException("TFP_FORBIDDEN_ENTITY", "Entity is not allowed");
        }

        private static string MapField(IReadOnlyDictionary<string, string> fields, string field)
        {
            if (!fields.TryGetValue(field, out var mapped))
                throw new TfpEndpointException("TFP_FORBIDDEN_FIELD", $"Field is not allowed: {field}");
            return mapped;
        }

        private Value JsonValue(JsonElement element) => MapJsonElementToValue(element);

        private Expr ParseFilter(JsonElement node, IReadOnlyDictionary<string, string> fields)
        {
            if (node.ValueKind != JsonValueKind.Object || node.EnumerateObject().Count() != 1)
                throw new TfpEndpointException("TFP_INVALID_REQUEST", "Filter must contain one expression");
            var item = node.EnumerateObject().Single();
            if (item.Name is "$and" or "$or")
            {
                if (item.Value.ValueKind != JsonValueKind.Array)
                    throw new TfpEndpointException("TFP_INVALID_REQUEST", "Logical filter requires an array");
                var parts = item.Value.EnumerateArray().Select(child => ParseFilter(child, fields)).ToList();
                if (parts.Count == 0) throw new TfpEndpointException("TFP_INVALID_REQUEST", "Logical filter requires operands");
                return item.Name == "$and" ? Expr.And(parts.ToArray()) : Expr.Or(parts.ToArray());
            }
            var field = MapField(fields, item.Name);
            if (item.Value.ValueKind != JsonValueKind.Object || item.Value.EnumerateObject().Count() != 1)
                throw new TfpEndpointException("TFP_INVALID_REQUEST", "Invalid field predicate");
            var operation = item.Value.EnumerateObject().Single();
            return operation.Name switch
            {
                "$eq" => Expr.Eq(field, JsonValue(operation.Value)),
                "$gte" => Expr.Gte(field, JsonValue(operation.Value)),
                "$lte" => Expr.Lte(field, JsonValue(operation.Value)),
                "$in" when operation.Value.ValueKind == JsonValueKind.Array =>
                    Expr.InList(field, operation.Value.EnumerateArray().Select(JsonValue)),
                _ => throw new TfpEndpointException("TFP_INVALID_REQUEST", "Unsupported predicate operator")
            };
        }

        private static void RejectPrivilegedInput(string payload, ISet<string> allowedTopLevel)
        {
            using var document = JsonDocument.Parse(payload);
            foreach (var property in document.RootElement.EnumerateObject())
                if (!allowedTopLevel.Contains(property.Name))
                    throw new TfpEndpointException("TFP_INVALID_REQUEST", $"Unknown TFP field: {property.Name}");
            var forbidden = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
                "tenant", "tenantId", "merchant", "merchantId", "user", "userId",
                "permissions", "requestPolicy", "purposePolicy", "trustedContext",
                "hardLimit", "hard_limit", "hardLimitValue", "hard_limit_value"
            };
            void Visit(JsonElement value)
            {
                if (value.ValueKind == JsonValueKind.Object)
                    foreach (var property in value.EnumerateObject()) {
                        if (forbidden.Contains(property.Name)) throw new TfpEndpointException(
                            "TFP_POLICY_VIOLATION", "Client cannot provide server-owned field");
                        Visit(property.Value);
                    }
                else if (value.ValueKind == JsonValueKind.Array)
                    foreach (var child in value.EnumerateArray()) Visit(child);
            }
            Visit(document.RootElement);
        }
    }
}
