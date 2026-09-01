using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace TeaQL.Core
{
    public sealed class RuntimeModule
    {
        public IReadOnlyList<string> Entities { get; }
        public IReadOnlyDictionary<string, IEntityChecker> Checkers { get; }
        public IReadOnlyDictionary<string, Record> SchemaSamples { get; }
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, bool>> SchemaRequired { get; }
        public IReadOnlyList<BootstrapEntity> RootEntities { get; }
        public IReadOnlyList<BootstrapEntity> ConstantEntities { get; }
        public RuntimeModule(IEnumerable<string> entities, IReadOnlyDictionary<string, IEntityChecker> checkers = null, IReadOnlyDictionary<string, Record> schemaSamples = null, IReadOnlyDictionary<string, IReadOnlyDictionary<string, bool>> schemaRequired = null, IEnumerable<BootstrapEntity> rootEntities = null, IEnumerable<BootstrapEntity> constantEntities = null) {
            Entities = Array.AsReadOnly(entities.Distinct(StringComparer.Ordinal).ToArray());
            Checkers = checkers ?? new Dictionary<string, IEntityChecker>();
            SchemaSamples = schemaSamples ?? new Dictionary<string, Record>();
            SchemaRequired = schemaRequired ?? new Dictionary<string, IReadOnlyDictionary<string, bool>>();
            RootEntities = Array.AsReadOnly((rootEntities ?? Array.Empty<BootstrapEntity>()).ToArray());
            ConstantEntities = Array.AsReadOnly((constantEntities ?? Array.Empty<BootstrapEntity>()).ToArray());
        }
        public RuntimeModule And(RuntimeModule other) =>
            new RuntimeModule(Entities.Concat(other.Entities), Checkers.Concat(other.Checkers).ToDictionary(x => x.Key, x => x.Value), SchemaSamples.Concat(other.SchemaSamples).ToDictionary(x => x.Key, x => x.Value), SchemaRequired.Concat(other.SchemaRequired).ToDictionary(x => x.Key, x => x.Value), RootEntities.Concat(other.RootEntities), ConstantEntities.Concat(other.ConstantEntities));
    }
    public sealed record BootstrapEntity(string Entity, long Id, Record Values);
    public abstract record ObjectLocationSegment
    {
        private ObjectLocationSegment() { }
        public sealed record PropertySegment(string Name) : ObjectLocationSegment;
        public sealed record IndexSegment(int Value) : ObjectLocationSegment;
    }
    public sealed class ObjectLocation
    {
        private readonly IReadOnlyList<ObjectLocationSegment> _segments;
        private ObjectLocation(IReadOnlyList<ObjectLocationSegment> segments) { _segments = segments; }
        public static ObjectLocation Root() => new ObjectLocation(Array.Empty<ObjectLocationSegment>());
        public static ObjectLocation Property(string name) => Root().PropertyAt(name);
        public ObjectLocation PropertyAt(string name) => new ObjectLocation(_segments.Concat(new[] { new ObjectLocationSegment.PropertySegment(name) }).ToArray());
        public ObjectLocation Index(int index) { if (index.CompareTo(0) == -1) throw new ArgumentOutOfRangeException(nameof(index)); return new ObjectLocation(_segments.Concat(new[] { new ObjectLocationSegment.IndexSegment(index) }).ToArray()); }
        public ObjectLocation PrefixedBy(ObjectLocation prefix) => new ObjectLocation(prefix._segments.Concat(_segments).ToArray());
        public string ModelPath => Render(name => name);
        public string NativePath => Render(PascalCase);
        public string InstancePath => string.Concat(_segments.Select(segment => segment is ObjectLocationSegment.PropertySegment property ? "/" + EscapePointer(LowerCamel(property.Name)) : "/" + ((ObjectLocationSegment.IndexSegment)segment).Value));
        public override string ToString() => NativePath;
        public static implicit operator ObjectLocation(string property) => Property(property);
        private string Render(Func<string, string> propertyName) { var result = ""; foreach (var segment in _segments) { if (segment is ObjectLocationSegment.IndexSegment index) result += $"[{index.Value}]"; else { var property = (ObjectLocationSegment.PropertySegment)segment; result += (result.Length == 0 ? "" : ".") + propertyName(property.Name); } } return result; }
        private static string PascalCase(string name) => string.Concat(name.Split('_').Select(Capitalize));
        private static string LowerCamel(string name) { var parts = name.Split('_'); return parts[0] + string.Concat(parts.Skip(1).Select(Capitalize)); }
        private static string Capitalize(string value) => value.Length == 0 ? "" : char.ToUpperInvariant(value[0]) + value.Substring(1);
        private static string EscapePointer(string value) => value.Replace("~", "~0").Replace("/", "~1");
    }
    public record CheckResult(string RuleId, ObjectLocation Location, object? InputValue = null, object? SystemValue = null, string? Message = null)
    {
        public string ModelPath => Location.ModelPath;
        public string NativePath => Location.NativePath;
        public string InstancePath => Location.InstancePath;
    }
    public enum FixEvidenceSource { Clock, Context }
    public sealed record FixEvidence(string EntityType, string ModelPath, FixEvidenceSource Source, string SourceLabel);
    public sealed class CheckException(IReadOnlyList<CheckResult> violations) : Exception("Check failed") { public IReadOnlyList<CheckResult> Violations { get; } = violations; }
    public sealed record ContextEntityRef(string EntityType, long Id);
    public sealed class ContextRootException(string message) : InvalidOperationException(message) { }
    public interface IEntityChecker { IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTime now); }

    public class Value
    {
        public object Raw { get; }
        protected Value(object raw) { Raw = raw; }
        public class TextValue : Value { public TextValue(string v) : base(v) {} }
        public class I64Value : Value { public I64Value(long v) : base(v) {} }
        public class BoolValue : Value { public BoolValue(bool v) : base(v) {} }
        public class F64Value : Value { public F64Value(double v) : base(v) {} }
        public class DateValue : Value { public DateValue(DateTime v) : base(v) {} }
        public class DecimalValue : Value { public DecimalValue(decimal v) : base(v) {} }
        public class TimeValue : Value { public TimeValue(TimeSpan v) : base(v) {} }
        public class DateTimeValue : Value { public DateTimeValue(DateTime v) : base(v) {} }
        public class JsonValue : Value { public JsonValue(string v) : base(v) {} }
        public class ObjectValue : Value { public ObjectValue(object v) : base(v) {} }
        public class NullValue : Value { public NullValue() : base(null) {} }
    }

    public class Record : Dictionary<string, Value> { }
    public sealed record EntityKey(string EntityType, long Id);
    public sealed class EntityRoot {
        private readonly Dictionary<EntityKey, Record> _changes = new();
        private readonly Dictionary<EntityKey, long> _versions = new();
        private readonly HashSet<EntityKey> _new = new(), _deleted = new();
        public void Set(EntityKey key, string field, Value value) { if (!_changes.TryGetValue(key, out var record)) _changes[key] = record = new Record(); record[field] = value; }
        public Record Change(EntityKey key) { var result = new Record(); if (_changes.TryGetValue(key, out var values)) foreach (var item in values) result[item.Key] = item.Value; return result; }
        public void SetOriginalVersion(EntityKey key, long version) => _versions[key] = version;
        public void MarkAsNew(EntityKey key) => _new.Add(key);
        public void MarkAsPersisted(EntityKey key) { _new.Remove(key); _deleted.Remove(key); }
        public void MarkAsDeleted(EntityKey key) { _changes.Remove(key); _deleted.Add(key); }
        public void MergeFrom(EntityRoot other) { foreach (var item in other._changes) foreach (var value in item.Value) Set(item.Key, value.Key, value.Value); foreach (var key in other._new) _new.Add(key); foreach (var key in other._deleted) _deleted.Add(key); foreach (var item in other._versions) _versions[item.Key] = item.Value; }
        public void Rekey(EntityKey oldKey, EntityKey newKey) { if (oldKey == newKey) return; if (_changes.Remove(oldKey, out var values)) foreach (var value in values) Set(newKey, value.Key, value.Value); if (_versions.Remove(oldKey, out var version)) _versions[newKey] = version; if (_new.Remove(oldKey)) _new.Add(newKey); if (_deleted.Remove(oldKey)) _deleted.Add(newKey); }
        public void ClearEntity(EntityKey key) { _changes.Remove(key); _new.Remove(key); _deleted.Remove(key); }
        public bool IsEmpty => _changes.Count == 0 && _new.Count == 0 && _deleted.Count == 0;
    }

    public class InsertCommand { public string Entity { get; set; } public Record Values { get; set; } }
    public class UpdateCommand { public string Entity { get; set; } public Value Id { get; set; } public Record Values { get; set; } }
    public class DeleteCommand { public string Entity { get; set; } public Value Id { get; set; } public Value Version { get; set; } }
    public class MutationRequest { public object Command { get; set; } public string Comment { get; set; } public EntityKey LedgerKey { get; set; } public EntityRoot LedgerRoot { get; set; } }

    public class FilterExpression
    {
        public string Operator { get; set; }
        public string Field { get; set; }
        public object Expected { get; set; }
        public EntityDescriptor SubqueryEntity { get; set; }
        public SelectQuery Subquery { get; set; }
        public string SubqueryField { get; set; }
    }

    public class EntityDescriptor
    {
        public string Name { get; set; }
        public static EntityDescriptor New(string name) => new EntityDescriptor { Name = name };
    }

    public class RelationQueryLoad
    {
        public string Name { get; set; }
        public string TargetEntity { get; set; }
        public string LocalKey { get; set; }
        public string ForeignKey { get; set; }
        public bool Many { get; set; }
        public SelectQuery Query { get; set; }
    }

    public class RelationAggregateLoad
    {
        public string RelationName { get; set; }
        public string TargetEntity { get; set; }
        public string LocalKey { get; set; }
        public string ForeignKey { get; set; }
        public string Alias { get; set; }
        public bool SingleResult { get; set; }
        public SelectQuery Query { get; set; }
    }

    public class Expr
    {
        public static object Eq(string field, object value) { return new FilterExpression { Operator = "eq", Field = field, Expected = value }; }
        public static object Ne(string field, object value) { return new FilterExpression { Operator = "ne", Field = field, Expected = value }; }
        public static object Contain(string field, object value) { return new FilterExpression { Operator = "contain", Field = field, Expected = value }; }
        public static object NotContain(string field, object value) { return new FilterExpression { Operator = "not_contain", Field = field, Expected = value }; }
        public static object BeginWith(string field, object value) { return new FilterExpression { Operator = "begin_with", Field = field, Expected = value }; }
        public static object NotBeginWith(string field, object value) { return new FilterExpression { Operator = "not_begin_with", Field = field, Expected = value }; }
        public static object EndWith(string field, object value) { return new FilterExpression { Operator = "end_with", Field = field, Expected = value }; }
        public static object NotEndWith(string field, object value) { return new FilterExpression { Operator = "not_end_with", Field = field, Expected = value }; }
        public static object SoundLike(string field, object value) { return new FilterExpression { Operator = "sound_like", Field = field, Expected = value }; }
        public static object In(string field, object value) { return new FilterExpression { Operator = "in", Field = field, Expected = value }; }
        public static object NotIn(string field, object value) { return new FilterExpression { Operator = "not_in", Field = field, Expected = value }; }
        public static object Gt(string field, object value) { return new FilterExpression { Operator = "gt", Field = field, Expected = value }; }
        public static object Gte(string field, object value) { return new FilterExpression { Operator = "gte", Field = field, Expected = value }; }
        public static object Lt(string field, object value) { return new FilterExpression { Operator = "lt", Field = field, Expected = value }; }
        public static object Lte(string field, object value) { return new FilterExpression { Operator = "lte", Field = field, Expected = value }; }
        public static object Between(string field, object lower, object upper) { return new FilterExpression { Operator = "between", Field = field, Expected = new object[] { lower, upper } }; }
        public static object IsNull(string field) { return new FilterExpression { Operator = "is_null", Field = field }; }
        public static object IsNotNull(string field) { return new FilterExpression { Operator = "is_not_null", Field = field }; }
        public static object InSubquery(string field, EntityDescriptor entity, SelectQuery query, string projectedField)
        {
            return new FilterExpression { Operator = "in_subquery", Field = field, SubqueryEntity = entity, Subquery = query, SubqueryField = projectedField };
        }
        public static object NotInSubquery(string field, EntityDescriptor entity, SelectQuery query, string projectedField)
        {
            return new FilterExpression { Operator = "not_in_subquery", Field = field, SubqueryEntity = entity, Subquery = query, SubqueryField = projectedField };
        }
    }

    public class SelectQuery
    {
        public string Entity { get; }
        public int? LimitValue { get; private set; }
        public int OffsetValue { get; private set; }
        public string PartitionBy { get; set; }
        public List<FilterExpression> Filters { get; } = new List<FilterExpression>();
        public List<string> Projections { get; } = new List<string>();
        public List<(string Field, string Direction)> Orders { get; } = new List<(string, string)>();
        public List<string> GroupFields { get; } = new List<string>();
        public List<(string Function, string Field, string Alias)> Aggregates { get; } = new List<(string, string, string)>();
        public List<RelationQueryLoad> Relations { get; } = new List<RelationQueryLoad>();
        public List<RelationAggregateLoad> RelationAggregates { get; } = new List<RelationAggregateLoad>();
        public List<FacetRequest> Facets { get; } = new List<FacetRequest>();
        [System.Text.Json.Serialization.JsonIgnore]
        public ContinuousPageFetchOptions ContinuousPageFetch { get; private set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public IdSetPaginationOptions IdSetPagination { get; private set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public int? TopNProbeThreshold { get; private set; }
        public string CommentText { get; private set; }
        public string PurposeText { get; private set; }
        public List<string> TracePath { get; } = new List<string>();
        public SelectQuery(string entity) { Entity = entity; }
        public void Comment(string c) { CommentText = c; }
        public void Purpose(string p) { PurposeText = p; }
        public void Limit(int n)
        {
            if (n is < 1 or > 10_000)
                throw new ArgumentOutOfRangeException(nameof(n), "materialized query limit must be between 1 and 10,000");
            LimitValue = n;
        }
        public void Offset(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
            OffsetValue = n;
        }
        public void OrderBy(string f, string d) { Orders.Add((f, d)); }
        public void GroupBy(string f) { GroupFields.Add(f); }
        public void Aggregate(string func, string field, string retName) { Aggregates.Add((func, field, retName)); }
        public void AndFilter(object expr) { if (expr is FilterExpression filter) Filters.Add(filter); }
        public void Project(string field) { if (!Projections.Contains(field)) Projections.Add(field); }
        public void RelationQuery(string name, string targetEntity, string foreignKey, bool many, SelectQuery query)
        {
            Relations.Add(new RelationQueryLoad { Name = name, TargetEntity = targetEntity, LocalKey = "id", ForeignKey = foreignKey, Many = many, Query = query });
        }
        public void ForwardRelationQuery(string name, string targetEntity, string localKey, SelectQuery query)
        {
            Relations.Add(new RelationQueryLoad { Name = name, TargetEntity = targetEntity, LocalKey = localKey, ForeignKey = "id", Many = false, Query = query });
        }
        public void RelationAggregate(string name, string targetEntity, string foreignKey, string alias, SelectQuery query, bool singleResult)
        {
            RelationAggregates.Add(new RelationAggregateLoad { RelationName = name, TargetEntity = targetEntity, LocalKey = "id", ForeignKey = foreignKey, Alias = alias, Query = query, SingleResult = singleResult });
        }
        public void OptimizeForContinuousPageFetch() => OptimizeForContinuousPageFetchWith("default", 600);
        public void OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            if (string.IsNullOrWhiteSpace(namespaceName)) throw new ArgumentException("namespace must not be empty", nameof(namespaceName));
            if (ttlSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(ttlSeconds));
            ContinuousPageFetch = new ContinuousPageFetchOptions(namespaceName, ttlSeconds);
        }
        internal void ClearContinuousPageFetch() { ContinuousPageFetch = null; }
        public void OptimizePaginationWithIdSet() => OptimizePaginationWithIdSet("default", 600, 3_000_000);
        public void OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            if (string.IsNullOrWhiteSpace(namespaceName)) throw new ArgumentException("namespace must not be empty", nameof(namespaceName));
            if (ttlSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(ttlSeconds));
            if (maxIds <= 0) throw new ArgumentOutOfRangeException(nameof(maxIds));
            IdSetPagination = new IdSetPaginationOptions(namespaceName, ttlSeconds, maxIds);
        }
        internal void ClearIdSetPagination() { IdSetPagination = null; }
        public void TopNProbeParentThreshold(int threshold)
        {
            if (threshold < 0) throw new ArgumentOutOfRangeException(nameof(threshold));
            TopNProbeThreshold = threshold;
        }
        internal void ClearPagination() { LimitValue = null; OffsetValue = 0; }
        public SelectQuery Copy()
        {
            var copy = new SelectQuery(Entity) { PartitionBy = PartitionBy };
            if (LimitValue.HasValue) copy.Limit(LimitValue.Value);
            copy.Offset(OffsetValue);
            copy.Filters.AddRange(Filters);
            copy.Projections.AddRange(Projections);
            copy.Orders.AddRange(Orders);
            copy.GroupFields.AddRange(GroupFields);
            copy.Aggregates.AddRange(Aggregates);
            copy.Relations.AddRange(Relations);
            copy.RelationAggregates.AddRange(RelationAggregates);
            copy.Facets.AddRange(Facets);
            copy.CommentText = CommentText;
            copy.PurposeText = PurposeText;
            copy.TracePath.AddRange(TracePath);
            if (ContinuousPageFetch != null) copy.OptimizeForContinuousPageFetchWith(ContinuousPageFetch.Namespace, ContinuousPageFetch.TtlSeconds);
            if (IdSetPagination != null) copy.OptimizePaginationWithIdSet(IdSetPagination.Namespace, IdSetPagination.TtlSeconds, IdSetPagination.MaxIds);
            if (TopNProbeThreshold.HasValue) copy.TopNProbeParentThreshold(TopNProbeThreshold.Value);
            return copy;
        }
    }

    public sealed record FacetRequest(
        string Name, string RelationName, SelectQuery Query, bool IncludeAllFacets);

    public sealed class ContinuousPageFetchOptions
    {
        public string Namespace { get; }
        public int TtlSeconds { get; }
        public ContinuousPageFetchOptions(string namespaceName, int ttlSeconds) { Namespace = namespaceName; TtlSeconds = ttlSeconds; }
    }

    public sealed record IdSetPaginationOptions(string Namespace, int TtlSeconds, int MaxIds);

    public class QueryRequest { public SelectQuery Query { get; } public QueryRequest(SelectQuery q) { Query = q; } }
    public class QueryResult {
        public List<Record> Rows { get; set; } = new List<Record>();
        public Dictionary<string, SmartList<Record>> Facets { get; set; } = new();
    }

    public class SmartList<T> : List<T>
    {
        public long? TotalCount { get; set; }
        public Record Aggregations { get; set; } = new();
        public Record Summary { get; set; } = new();
        public Dictionary<string, SmartList<Record>> Facets { get; set; } = new();
        public bool IsLoaded { get; set; } = true;

        public SmartList() { }
        public SmartList(IEnumerable<T> items) : base(items) { }

        public SmartList<U> Map<U>(Func<T, U> mapper)
            => new(this.Select(mapper)) { TotalCount = TotalCount };

        public SmartList<T> Filter(Func<T, bool> predicate)
            => new(this.Where(predicate)) { TotalCount = TotalCount };

        public T FirstOrDefaultValue() => Count == 0 ? default : this[0];
    }

    internal sealed class ContinuousPageExecution
    {
        public string QueryKey; public string Entity; public string Direction; public int PageSize;
        public int OriginalOffset; public int TtlSeconds; public bool Optimized; public string CursorId;
    }

    internal static class ContinuousPageRuntime
    {
        public static (SelectQuery Query, ContinuousPageExecution Execution) Prepare(UserContext context, SelectQuery source)
        {
            var query = Clone(source);
            var options = source.ContinuousPageFetch;
            if (options == null) { context.ObserveContinuousPage("DISABLED"); return (query, null); }
            if (!query.LimitValue.HasValue || query.LimitValue.Value <= 0 || query.Orders.Count != 1 ||
                query.Orders[0].Field != "id" || query.Aggregates.Count != 0 || query.GroupFields.Count != 0 ||
                !string.IsNullOrEmpty(query.PartitionBy))
            {
                context.ObserveContinuousPage("OFFSET_FALLBACK:UNSUPPORTED_QUERY_SHAPE");
                return (query, null);
            }
            var normalized = Clone(query);
            normalized.Offset(0);
            normalized.ClearContinuousPageFetch();
            var owner = context.UserIdentifier ?? "";
            var payload = JsonSerializer.Serialize(normalized);
            var keyBytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(options.Namespace + "|" + owner + "|" + payload));
            var queryKey = "teaql:continuous-page:v1:" + Convert.ToHexString(keyBytes).ToLowerInvariant();
            var execution = new ContinuousPageExecution { QueryKey = queryKey, Entity = query.Entity,
                Direction = query.Orders[0].Direction.ToLowerInvariant(), PageSize = query.LimitValue.Value,
                OriginalOffset = query.OffsetValue, TtlSeconds = options.TtlSeconds };
            if (query.OffsetValue == 0) { context.ObserveContinuousPage("OFFSET_FALLBACK:FIRST_PAGE"); return (query, execution); }
            var cursor = context.GetContinuousPageCursor(queryKey, query.OffsetValue);
            if (cursor == null) { context.ObserveContinuousPage("OFFSET_FALLBACK:CACHE_MISS"); return (query, execution); }
            query.Filters.Add(new FilterExpression { Operator = execution.Direction == "desc" ? "lt" : "gt", Field = "id", Expected = cursor.Boundary });
            query.Offset(0);
            execution.Optimized = true;
            execution.CursorId = cursor.CursorId;
            context.ObserveContinuousPage("CURSOR_SEEK", cursor.CursorId);
            return (query, execution);
        }

        public static void Register(UserContext context, ContinuousPageExecution execution, List<Record> rows)
        {
            if (execution == null || rows.Count != execution.PageSize || rows.Count == 0 || !rows[^1].TryGetValue("id", out var boundary)) return;
            context.PutContinuousPageCursor(execution.QueryKey, execution.OriginalOffset + rows.Count,
                new ContinuousPageCursor { CursorId = "cpg_" + Guid.NewGuid().ToString("N"), Boundary = boundary.Raw,
                    ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(execution.TtlSeconds) });
            if (execution.Optimized) context.ObserveContinuousPage("CURSOR_SEEK", execution.CursorId);
        }

        private static SelectQuery Clone(SelectQuery source)
            => source.Copy();
    }

    internal sealed class ContinuousPageCursor { public string CursorId; public object Boundary; public DateTimeOffset ExpiresAt; }
    internal sealed record RetainedIdSet(long[] Ids, DateTimeOffset ExpiresAt);
    internal static class IdSetPaginationRuntime
    {
        private static readonly ConcurrentDictionary<string, RetainedIdSet> Sets = new();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();

        public static async Task<QueryResult> ExecuteAsync(
            UserContext context, SelectQuery source, Func<SelectQuery, Task<QueryResult>> executeCore)
        {
            var options = source.IdSetPagination;
            if (options == null) { context.ObserveIdSet("ID_SET_DISABLED"); return await executeCore(source); }
            if (!source.LimitValue.HasValue || source.LimitValue.Value <= 0 || source.Aggregates.Count != 0 ||
                source.GroupFields.Count != 0 || !string.IsNullOrEmpty(source.PartitionBy))
            {
                context.ObserveIdSet("ID_SET_FALLBACK_UNSUPPORTED_SHAPE");
                var fallback = source.Copy(); fallback.ClearIdSetPagination(); return await executeCore(fallback);
            }

            var stable = source.Copy();
            if (!stable.Orders.Any(order => order.Field == "id")) stable.OrderBy("id", "asc");
            var normalized = stable.Copy(); normalized.ClearPagination(); normalized.ClearIdSetPagination();
            normalized.Projections.Clear(); normalized.Relations.Clear(); normalized.RelationAggregates.Clear(); normalized.Facets.Clear();
            var payload = options.Namespace + "|" + context.IdSetSecurityScope() + "|" + JsonSerializer.Serialize(normalized);
            var key = "teaql:id-set:v1:" + Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
            var plan = "ID_SET_HIT";
            if (!Sets.TryGetValue(key, out var retained) || retained.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                var gate = Locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await gate.WaitAsync();
                try
                {
                    if (!Sets.TryGetValue(key, out retained) || retained.ExpiresAt <= DateTimeOffset.UtcNow)
                    {
                        var idQuery = stable.Copy(); idQuery.ClearPagination(); idQuery.ClearIdSetPagination(); idQuery.ClearContinuousPageFetch();
                        idQuery.Projections.Clear(); idQuery.Project("id"); idQuery.Relations.Clear(); idQuery.RelationAggregates.Clear(); idQuery.Facets.Clear();
                        idQuery.Limit(options.MaxIds == int.MaxValue ? int.MaxValue : options.MaxIds + 1);
                        var idResult = await executeCore(idQuery);
                        var ids = idResult.Rows.Select(row => row.TryGetValue("id", out var id) ? Convert.ToInt64(id.Raw) : 0)
                            .Where(id => id > 0).ToArray();
                        if (ids.Length > options.MaxIds)
                        {
                            context.ObserveIdSet("ID_SET_FALLBACK_LIMIT_EXCEEDED", "LOWER_BOUND", ids.LongLength);
                            var fallback = source.Copy(); fallback.ClearIdSetPagination(); return await executeCore(fallback);
                        }
                        retained = new RetainedIdSet(ids, DateTimeOffset.UtcNow.AddSeconds(options.TtlSeconds));
                        if (Sets.Count >= 64)
                        {
                            var oldest = Sets.OrderBy(pair => pair.Value.ExpiresAt).First(); Sets.TryRemove(oldest.Key, out _);
                        }
                        Sets[key] = retained; plan = "ID_SET_BUILD";
                    }
                }
                finally { gate.Release(); }
            }
            context.ObserveIdSet(plan, "EXACT", retained.Ids.LongLength);
            var pageIds = retained.Ids.Skip(source.OffsetValue).Take(source.LimitValue.Value).ToArray();
            if (pageIds.Length == 0) return new QueryResult();
            var page = source.Copy(); page.ClearPagination(); page.ClearIdSetPagination(); page.ClearContinuousPageFetch();
            page.Filters.Add(new FilterExpression { Operator = "in", Field = "id", Expected = pageIds.Cast<object>().ToList() });
            var result = await executeCore(page);
            var order = pageIds.Select((id, index) => (id, index)).ToDictionary(item => item.id, item => item.index);
            result.Rows = result.Rows.Where(row => row.TryGetValue("id", out var id) && order.ContainsKey(Convert.ToInt64(id.Raw)))
                .OrderBy(row => order[Convert.ToInt64(row["id"].Raw)]).ToList();
            return result;
        }
    }
    public class MutationResult { public bool Success { get; set; } public long Id { get; set; } public long Version { get; set; } public bool Deleted { get; set; } public Record PersistedRecord { get; set; } }
    public class AuditFieldChange { public string Field { get; set; } public object OldValue { get; set; } public object NewValue { get; set; } }
    public class RawAuditEvent { public string Kind { get; set; } public string Entity { get; set; } public long Id { get; set; } public string Reason { get; set; } public IReadOnlyList<AuditFieldChange> Changes { get; set; } }
    public class SafeAuditField { public string Field { get; set; } public string Value { get; set; } public bool Masked { get; set; } public bool Truncated { get; set; } }
    public class SafeAuditEvent { public string Kind { get; set; } public string Entity { get; set; } public long Id { get; set; } public string Reason { get; set; } public IReadOnlyList<SafeAuditField> Fields { get; set; } }
    public interface IRawAuditEventSink { Task OnEventAsync(UserContext context, RawAuditEvent auditEvent); }
    public interface IAppAuditEventSink { Task OnSafeEventAsync(UserContext context, SafeAuditEvent auditEvent); }
    public class InMemoryRawAuditEventSink : IRawAuditEventSink
    {
        public List<RawAuditEvent> Events { get; } = new();
        public Task OnEventAsync(UserContext context, RawAuditEvent auditEvent) { Events.Add(auditEvent); return Task.CompletedTask; }
    }
    public class InMemoryAppAuditEventSink : IAppAuditEventSink
    {
        public List<SafeAuditEvent> Events { get; } = new();
        public Task OnSafeEventAsync(UserContext context, SafeAuditEvent auditEvent) { Events.Add(auditEvent); return Task.CompletedTask; }
    }
    public sealed class SqlExecutionEvidence
    {
        public string Operation { get; set; } = "";
        public string Comment { get; set; }
        public string Purpose { get; set; }
        public string AuditReason { get; set; }
        public IReadOnlyList<string> TracePath { get; set; } = Array.Empty<string>();
        public string ParameterizedSql { get; set; } = "";
        public string DebugSql { get; set; } = "";
        public IReadOnlyList<object> Parameters { get; set; } = Array.Empty<object>();
        public long ElapsedMicros { get; set; }
        public int? ResultCount { get; set; }
        public long? AffectedRows { get; set; }
        public string ResultSummary { get; set; } = "";
    }
    public interface IDiagnosticSqlLogSink { void Write(SqlExecutionEvidence evidence); }
    public sealed class TextDiagnosticSqlLogSink : IDiagnosticSqlLogSink
    {
        private readonly TextWriter _writer;
        public TextDiagnosticSqlLogSink(TextWriter writer = null) { _writer = writer ?? Console.Error; }
        public void Write(SqlExecutionEvidence evidence)
        {
            if (string.IsNullOrWhiteSpace(evidence.DebugSql)) return;
            _writer.WriteLine($"[TeaQL SQL][{evidence.Operation}][{evidence.ElapsedMicros}us] {evidence.ResultSummary} comment={evidence.Comment} purpose={evidence.Purpose} auditReason={evidence.AuditReason} tracePath=[{string.Join(" -> ", evidence.TracePath)}]");
            _writer.WriteLine($"Parameterized SQL: {evidence.ParameterizedSql} params=[{string.Join(", ", evidence.Parameters)}]");
            _writer.WriteLine($"Debug SQL: {evidence.DebugSql}");
        }
    }
    public class UserContext
    {
        private List<FixEvidence> _currentFixEvidence = new();
        private IReadOnlyList<FixEvidence> _lastFixEvidence = Array.Empty<FixEvidence>();
        public IReadOnlyList<FixEvidence> LastFixEvidence => _lastFixEvidence;
        public void RecordFixEvidence(FixEvidence evidence)
        {
            var normalized = (evidence.SourceLabel ?? "").ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(evidence.EntityType) || string.IsNullOrWhiteSpace(evidence.ModelPath)
                || string.IsNullOrWhiteSpace(evidence.SourceLabel) || normalized.Contains("authorization")
                || normalized.Contains("cookie") || normalized.Contains("token="))
                throw new ArgumentException("Fix evidence must contain only safe framework provenance labels");
            _currentFixEvidence.Add(evidence);
        }
        private string _localeCode = "en";
        public UserContext SetLocaleCode(string code)
        {
            _localeCode = string.Equals(code, "zh-CN", StringComparison.OrdinalIgnoreCase) ? "zh-CN" : "en";
            return this;
        }
        public UserContext SetLanguageCode(string code) => SetLocaleCode(code);
        private CheckResult TranslateCheckResult(CheckResult result)
        {
            if (!string.IsNullOrWhiteSpace(result.Message) && !result.Message.StartsWith("checker.")) return result;
            var location = result.Location?.NativePath ?? "value";
            var message = result.RuleId switch
            {
                "required" => _localeCode == "zh-CN" ? $"{location} 是必填项" : $"The {location} is required",
                "invalid_type" => _localeCode == "zh-CN" ? $"{location} 的类型无效" : $"The {location} has an invalid type",
                _ => _localeCode == "zh-CN" ? $"{location} 不符合 {result.RuleId} 规则" : $"The {location} violates {result.RuleId}"
            };
            return result with { Message = message };
        }
        private ContextEntityRef _activeRoot;
        public UserContext WithActiveRoot(string entityType, long id)
        {
            if (string.IsNullOrWhiteSpace(entityType) || id <= 0) throw new ArgumentException("A typed, positive active root is required");
            _activeRoot = new ContextEntityRef(entityType, id);
            return this;
        }
        public ContextEntityRef RequireActiveRoot(string expectedEntityType)
        {
            if (_activeRoot == null) throw new ContextRootException("Active root is missing from UserContext");
            if (!string.Equals(_activeRoot.EntityType, expectedEntityType, StringComparison.Ordinal)) throw new ContextRootException($"Active root type is {_activeRoot.EntityType}, expected {expectedEntityType}");
            return _activeRoot;
        }
        public IReadOnlyList<string> RuntimeEntities { get; private set; } = Array.Empty<string>();
        private RuntimeModule _runtimeModule;
        public UserContext Install(RuntimeModule module)
        {
            RuntimeEntities = module.Entities;
            _checkers = module.Checkers;
            _runtimeModule = module;
            return this;
        }
        public Task EnsureSchemaAsync()
        {
            if (_runtimeModule == null) throw new InvalidOperationException("Install a RuntimeModule before EnsureSchemaAsync().");
            if (RequireDataService() is not ISchemaExecutor schema)
                throw new NotSupportedException("The configured DataService does not support schema reconciliation.");
            return schema.EnsureSchemaAsync(_runtimeModule);
        }
        private IReadOnlyDictionary<string, IEntityChecker> _checkers = new Dictionary<string, IEntityChecker>();
        private sealed class GraphSaveSession { }
        private readonly System.Threading.AsyncLocal<GraphSaveSession> _ambientGraphSave = new();
        private GraphSaveSession _activeGraphSave;
        private readonly System.Threading.SemaphoreSlim _graphSaveGate = new(1, 1);
        private List<Action> _graphCommitActions = new();
        private List<Action> _graphRollbackActions = new();
        private DateTime? _graphFixTime;
        public void CheckAndFix(MutationRequest request) {
            var entity = request.Command is InsertCommand insert ? insert.Entity : request.Command is UpdateCommand update ? update.Entity : request.Command is DeleteCommand delete ? delete.Entity : "";
            if (!_checkers.TryGetValue(entity, out var checker)) return;
            var results = checker.CheckAndFix(this, request, _graphFixTime ?? DateTime.UtcNow);
            var values = request.Command is InsertCommand checkedInsert ? checkedInsert.Values : request.Command is UpdateCommand checkedUpdate ? checkedUpdate.Values : null;
            if (request.LedgerKey != null && request.LedgerRoot != null && values != null) foreach (var item in values) request.LedgerRoot.Set(request.LedgerKey, item.Key, item.Value);
            if (results.Count != 0) throw new CheckException(results.Select(TranslateCheckResult).ToList());
        }
        public IDataService DataService { get; set; }
        public async Task<T> ExecuteGraphSaveAsync<T>(Func<Task<T>> work)
        {
            if (_ambientGraphSave.Value != null && ReferenceEquals(_ambientGraphSave.Value, _activeGraphSave)) return await work();
            await _graphSaveGate.WaitAsync();
            if (RequireDataService() is not IGraphTransactionDataService transaction)
            { _graphSaveGate.Release(); throw new NotSupportedException("The configured DataService does not support atomic graph saves."); }
            var session = new GraphSaveSession();
            _activeGraphSave = session;
            _ambientGraphSave.Value = session;
            _graphFixTime = DateTime.UtcNow;
            _currentFixEvidence = new List<FixEvidence>();
            _graphCommitActions = new List<Action>();
            _graphRollbackActions = new List<Action>();
            T result;
            List<Action> committedActions = null;
            var transactionBegun = false;
            try
            {
                await transaction.BeginGraphTransactionAsync();
                transactionBegun = true;
                result = await work();
                await transaction.CommitGraphTransactionAsync();
                committedActions = _graphCommitActions;
            }
            catch
            {
                try { if (transactionBegun) await transaction.RollbackGraphTransactionAsync(); }
                finally { for (var index = _graphRollbackActions.Count - 1; index >= 0; index--) _graphRollbackActions[index](); }
                throw;
            }
            finally
            {
                _lastFixEvidence = _currentFixEvidence.AsReadOnly();
                _ambientGraphSave.Value = null;
                _activeGraphSave = null;
                _graphFixTime = null;
                _graphCommitActions = new List<Action>();
                _graphRollbackActions = new List<Action>();
                _graphSaveGate.Release();
            }
            foreach (var action in committedActions) action();
            return result;
        }
        public void AfterGraphCommit(Action action) { if (_ambientGraphSave.Value == null || !ReferenceEquals(_ambientGraphSave.Value, _activeGraphSave)) throw new InvalidOperationException("No graph save is active"); _graphCommitActions.Add(action); }
        public void AfterGraphRollback(Action action) { if (_ambientGraphSave.Value == null || !ReferenceEquals(_ambientGraphSave.Value, _activeGraphSave)) throw new InvalidOperationException("No graph save is active"); _graphRollbackActions.Add(action); }
        public List<string> SqlTrace { get; } = new List<string>();
        private IRawAuditEventSink _standardAuditSink;
        private IAppAuditEventSink _appAuditSink;
        private readonly Dictionary<string, HashSet<string>> _auditMaskFields = new();
        private readonly Dictionary<string, int> _auditValueMaxLengths = new();
        private readonly Dictionary<string, ContinuousPageCursor> _continuousPageCursors = new();
        public string UserIdentifier { get; set; } = "";
        public string ContinuousPagePlan { get; private set; } = "DISABLED";
        public string ContinuousPageCursorId { get; private set; }
        public string IdSetPlan { get; private set; } = "ID_SET_DISABLED";
        public long IdSetCount { get; private set; }
        public string IdSetCountAccuracy { get; private set; } = "UNKNOWN";
        private readonly List<SqlExecutionEvidence> _sqlEvidence = new();
        private bool _captureSelectSql = true;
        private bool _captureMutationSql = true;
        private IDiagnosticSqlLogSink _diagnosticSqlLogSink = new TextDiagnosticSqlLogSink();
        internal string CurrentMutationAuditReason { get; set; }
        public IReadOnlyList<SqlExecutionEvidence> SqlEvidence => _sqlEvidence.AsReadOnly();

        public void EnableAllSqlEvidence()
        { _captureSelectSql = true; _captureMutationSql = true; _sqlEvidence.Clear(); }
        public void EnableSelectSqlEvidence()
        { _captureSelectSql = true; _captureMutationSql = false; _sqlEvidence.Clear(); }
        public void EnableMutationSqlEvidence()
        { _captureSelectSql = false; _captureMutationSql = true; _sqlEvidence.Clear(); }
        public void DisableSqlEvidence()
        { _captureSelectSql = false; _captureMutationSql = false; _sqlEvidence.Clear(); }
        public void DisableSelectSqlEvidence() { _captureSelectSql = false; }
        public void DisableMutationSqlEvidence() { _captureMutationSql = false; }
        public UserContext WithDiagnosticSqlLogSink(IDiagnosticSqlLogSink sink)
        { _diagnosticSqlLogSink = sink; return this; }
        internal void RecordSqlEvidence(SqlExecutionEvidence evidence)
        {
            var select = evidence.Operation == "query";
            if (!((select && _captureSelectSql) || (!select && _captureMutationSql))) return;
            _diagnosticSqlLogSink?.Write(evidence);
            _sqlEvidence.Add(evidence);
        }

        internal ContinuousPageCursor GetContinuousPageCursor(string queryKey, int offset)
        {
            var key = queryKey + ":" + offset;
            if (!_continuousPageCursors.TryGetValue(key, out var cursor)) return null;
            if (cursor.ExpiresAt > DateTimeOffset.UtcNow) return cursor;
            _continuousPageCursors.Remove(key); return null;
        }
        internal void PutContinuousPageCursor(string queryKey, int offset, ContinuousPageCursor cursor)
        {
            if (_continuousPageCursors.Count >= 4096)
            {
                var oldest = _continuousPageCursors.OrderBy(pair => pair.Value.ExpiresAt).First();
                _continuousPageCursors.Remove(oldest.Key);
            }
            _continuousPageCursors[queryKey + ":" + offset] = cursor;
        }
        internal void ObserveContinuousPage(string plan, string cursorId = null)
        { ContinuousPagePlan = plan; ContinuousPageCursorId = cursorId; }
        internal void ObserveIdSet(string plan, string accuracy = "UNKNOWN", long count = 0)
        { IdSetPlan = plan; IdSetCountAccuracy = accuracy; IdSetCount = count; }
        internal string IdSetSecurityScope()
            => (UserIdentifier ?? "") + "|" + (_activeRoot == null ? "" : _activeRoot.EntityType + ":" + _activeRoot.Id);

        public UserContext InitializeAudit(IRawAuditEventSink standardSink, IAppAuditEventSink appSink = null)
        {
            _standardAuditSink = standardSink;
            _appAuditSink = appSink;
            return this;
        }

        public UserContext ConfigureAuditPolicy(string entity, IEnumerable<string> maskFields, int? maxLength = null)
        {
            _auditMaskFields[entity] = new HashSet<string>(maskFields ?? Array.Empty<string>());
            if (maxLength.HasValue) _auditValueMaxLengths[entity] = maxLength.Value;
            return this;
        }

        public async Task EmitMutationAuditAsync(object command, MutationResult result, string reason)
        {
            var kind = command is InsertCommand ? "created" : command is UpdateCommand ? "updated" : "deleted";
            var entity = command is InsertCommand insert ? insert.Entity : command is UpdateCommand update ? update.Entity : ((DeleteCommand)command).Entity;
            var values = command is InsertCommand inserted ? inserted.Values : command is UpdateCommand updated ? updated.Values : new Record();
            var raw = new RawAuditEvent { Kind = kind, Entity = entity, Id = result.Id, Reason = reason,
                Changes = values.Select(pair => new AuditFieldChange { Field = pair.Key, NewValue = pair.Value.Raw }).ToList() };
            if (_standardAuditSink != null) await _standardAuditSink.OnEventAsync(this, raw);
            if (_appAuditSink != null)
            {
                _auditMaskFields.TryGetValue(entity, out var masks);
                _auditValueMaxLengths.TryGetValue(entity, out var limit);
                var safeFields = raw.Changes.Select(change => BuildSafeField(change, masks, limit)).ToList();
                await _appAuditSink.OnSafeEventAsync(this, new SafeAuditEvent { Kind = kind, Entity = entity, Id = result.Id, Reason = reason, Fields = safeFields });
            }
        }

        private static SafeAuditField BuildSafeField(AuditFieldChange change, HashSet<string> masks, int limit)
        {
            var value = change.NewValue == null ? null : change.NewValue.ToString();
            var masked = masks != null && masks.Contains(change.Field);
            if (value != null && masked) value = value.Length < 8 ? new string('*', value.Length) : value.Substring(0, 2) + new string('*', value.Length - 4) + value.Substring(value.Length - 2);
            var truncated = value != null && limit > 0 && value.Length > limit;
            if (truncated) value = limit <= 3 ? new string('*', limit) : value.Substring(0, limit - 3) + "...";
            return new SafeAuditField { Field = change.Field, Value = value, Masked = masked, Truncated = truncated };
        }

        public IDataService RequireDataService()
        {
            return DataService ?? throw new InvalidOperationException("Required UserContext resource is missing: DataService");
        }
    }
    public interface IDataService
    {
        Task<QueryResult> QueryAsync(UserContext context, QueryRequest req);
        Task<object> MutateAsync(UserContext context, MutationRequest req);
    }
    public interface IGraphTransactionDataService : IDataService
    {
        Task BeginGraphTransactionAsync();
        Task CommitGraphTransactionAsync();
        Task RollbackGraphTransactionAsync();
    }
    // Provider SPI. Deliberately internal: application code must use
    // UserContext.EnsureSchemaAsync() so the execution context always participates.
    internal interface ISchemaExecutor
    {
        Task EnsureSchemaAsync(RuntimeModule module);
    }

    public sealed class StreamChunk
    {
        public List<Record> Rows { get; set; } = new();
        public int ChunkIndex { get; set; }
        public bool IsLast { get; set; }
    }

    // Implemented by a runtime/provider that owns a real streaming cursor.
    // Ordinary IDataService and the federation protocol do not emulate it.
    public interface IStreamingDataService : IDataService
    {
        IAsyncEnumerable<StreamChunk> QueryStreamAsync(
            UserContext context,
            QueryRequest req,
            int chunkSize,
            CancellationToken cancellationToken = default);
    }

    public class JsonFileDataService : IGraphTransactionDataService, ISchemaExecutor
    {
        private readonly string _path;
        private readonly object _gate = new object();
        private StoreState _state;
        private StoreState _graphSnapshot;

        private class StoreState
        {
            public Dictionary<string, Dictionary<string, Dictionary<string, JsonElement>>> Data { get; set; } = new();
            public Dictionary<string, long> NextIds { get; set; } = new();
        }

        public JsonFileDataService(string path)
        {
            _path = path;
            _state = File.Exists(path)
                ? JsonSerializer.Deserialize<StoreState>(File.ReadAllText(path)) ?? new StoreState()
                : new StoreState();
        }

        Task ISchemaExecutor.EnsureSchemaAsync(RuntimeModule module) => throw new NotSupportedException(
            "EnsureSchemaAsync is only supported by a schema-aware database provider; JsonFileDataService has no database schema.");

        public Task BeginGraphTransactionAsync()
        {
            lock (_gate)
            {
                if (_graphSnapshot != null) throw new InvalidOperationException("A graph transaction is already active");
                _graphSnapshot = JsonSerializer.Deserialize<StoreState>(JsonSerializer.Serialize(_state)) ?? new StoreState();
            }
            return Task.CompletedTask;
        }

        public Task CommitGraphTransactionAsync()
        {
            lock (_gate) { if (_graphSnapshot == null) throw new InvalidOperationException("No graph transaction is active"); Persist(); _graphSnapshot = null; }
            return Task.CompletedTask;
        }

        public Task RollbackGraphTransactionAsync()
        {
            lock (_gate) { if (_graphSnapshot == null) throw new InvalidOperationException("No graph transaction is active"); _state = _graphSnapshot; _graphSnapshot = null; Persist(); }
            return Task.CompletedTask;
        }

        public async Task<object> MutateAsync(UserContext context, MutationRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Comment))
                throw new InvalidOperationException("Security audit failure: mutation audit reason is required");
            context.CheckAndFix(req);
            MutationResult result;
            lock (_gate)
            {
                result = req.Command is InsertCommand insert ? Insert(insert)
                    : req.Command is UpdateCommand update ? Update(update)
                    : req.Command is DeleteCommand delete ? Delete(delete)
                    : throw new InvalidOperationException("Unsupported mutation command");
            }
            await context.EmitMutationAuditAsync(req.Command, result, req.Comment);
            return result;
        }

        private MutationResult Insert(InsertCommand command)
        {
            var table = Table(command.Entity);
            var id = command.Values.TryGetValue("id", out var idValue) ? Convert.ToInt64(idValue.Raw) : NextId(command.Entity);
            var record = ToStoredRecord(command.Values);
            record["id"] = JsonSerializer.SerializeToElement(id);
            record["version"] = JsonSerializer.SerializeToElement(1L);
            table[id.ToString()] = record;
            if (_graphSnapshot == null) Persist();
            return new MutationResult { Success = true, Id = id, Version = 1, PersistedRecord = ToRecord(record) };
        }

        private MutationResult Update(UpdateCommand command)
        {
            var id = Convert.ToInt64(command.Id.Raw);
            var table = Table(command.Entity);
            if (!table.TryGetValue(id.ToString(), out var record)) throw new KeyNotFoundException($"{command.Entity}({id}) does not exist");
            var expectedVersion = command.Values.TryGetValue("version", out var submittedVersion)
                ? Convert.ToInt64(submittedVersion.Raw) : 0;
            var currentVersion = record.TryGetValue("version", out var current) ? current.GetInt64() : 0;
            if (currentVersion != expectedVersion)
                throw new InvalidOperationException($"Optimistic lock failed for {command.Entity}({id}) at version {expectedVersion}");
            foreach (var pair in ToStoredRecord(command.Values))
                if (pair.Key != "id" && pair.Key != "version") record[pair.Key] = pair.Value;
            var version = currentVersion + 1;
            record["version"] = JsonSerializer.SerializeToElement(version);
            if (_graphSnapshot == null) Persist();
            return new MutationResult { Success = true, Id = id, Version = version, PersistedRecord = ToRecord(record) };
        }

        private MutationResult Delete(DeleteCommand command)
        {
            var id = Convert.ToInt64(command.Id.Raw);
            var table = Table(command.Entity);
            if (!table.TryGetValue(id.ToString(), out var record)) throw new KeyNotFoundException($"{command.Entity}({id}) does not exist");
            var expectedVersion = Convert.ToInt64(command.Version.Raw);
            var currentVersion = record.TryGetValue("version", out var storedVersion) ? storedVersion.GetInt64() : 0;
            if (currentVersion != expectedVersion)
                throw new InvalidOperationException($"Optimistic lock failed for {command.Entity}({id}) at version {expectedVersion}");
            var deletedVersion = -(expectedVersion + 1);
            record["version"] = JsonSerializer.SerializeToElement(deletedVersion);
            if (_graphSnapshot == null) Persist();
            return new MutationResult { Success = true, Id = id, Version = deletedVersion, Deleted = true, PersistedRecord = ToRecord(record) };
        }

        public Task<QueryResult> QueryAsync(UserContext context, QueryRequest req)
            => IdSetPaginationRuntime.ExecuteAsync(context, req.Query,
                query => QueryCoreAsync(context, new QueryRequest(query)));

        private Task<QueryResult> QueryCoreAsync(UserContext context, QueryRequest req)
        {
            var prepared = ContinuousPageRuntime.Prepare(context, req.Query);
            var query = prepared.Query;
            lock (_gate)
            {
                IEnumerable<Dictionary<string, JsonElement>> rows = Table(query.Entity).Values;
                foreach (var filter in query.Filters)
                {
                    rows = rows.Where(row => Matches(row, filter));
                }
                if (query.Aggregates.Count != 0 && query.GroupFields.Count == 0 &&
                    query.Aggregates.All(aggregate => aggregate.Function.Equals("Count", StringComparison.OrdinalIgnoreCase)))
                {
                    var countRecord = new Record();
                    foreach (var aggregate in query.Aggregates)
                        countRecord[aggregate.Alias] = new Value.I64Value(rows.LongCount());
                    return Task.FromResult(new QueryResult { Rows = new List<Record> { countRecord } });
                }
                foreach (var order in query.Orders.AsEnumerable().Reverse())
                {
                    rows = order.Direction.Equals("desc", StringComparison.OrdinalIgnoreCase)
                        ? rows.OrderByDescending(row => SortValue(row, order.Field))
                        : rows.OrderBy(row => SortValue(row, order.Field));
                }
                rows = rows.Skip(query.OffsetValue);
                if (query.LimitValue.HasValue) rows = rows.Take(query.LimitValue.Value);
                var records = rows.Select(ToRecord).ToList();
                EnhanceJsonRelations(query, records);
                if (query.Projections.Count != 0)
                {
                    var retained = query.Projections.Concat(query.Relations.Select(relation => relation.Name))
                        .ToHashSet(StringComparer.Ordinal);
                    foreach (var record in records)
                        foreach (var field in record.Keys.Where(field => !retained.Contains(field)).ToList())
                            record.Remove(field);
                }
                var result = new QueryResult { Rows = records };
                ContinuousPageRuntime.Register(context, prepared.Execution, result.Rows);
                return Task.FromResult(result);
            }
        }

        private void EnhanceJsonRelations(SelectQuery query, List<Record> parents)
        {
            foreach (var load in query.Relations)
            {
                var keys = parents.Where(parent => parent.ContainsKey(load.LocalKey))
                    .Select(parent => parent[load.LocalKey].Raw?.ToString()).Where(key => key != null)
                    .ToHashSet(StringComparer.Ordinal);
                IEnumerable<Dictionary<string, JsonElement>> candidates = Table(load.TargetEntity).Values;
                foreach (var filter in load.Query.Filters)
                    candidates = candidates.Where(row => Matches(row, filter));
                var children = candidates.Where(row =>
                        row.TryGetValue(load.ForeignKey, out var value) && keys.Contains(value.ToString()))
                    .Select(ToRecord).ToList();
                foreach (var parent in parents)
                {
                    var key = parent.TryGetValue(load.LocalKey, out var value) ? value.Raw?.ToString() : null;
                    var related = key == null
                        ? new List<Record>()
                        : children.Where(child => child.TryGetValue(load.ForeignKey, out var foreign)
                            && foreign.Raw?.ToString() == key).ToList();
                    parent[load.Name] = new Value.ObjectValue(load.Many ? related : related.Take(1).ToList());
                }
            }
        }

        private bool Matches(Dictionary<string, JsonElement> row, FilterExpression filter)
        {
            var present = row.TryGetValue(filter.Field, out var actual) && actual.ValueKind != JsonValueKind.Null;
            if (filter.Operator == "is_null") return !present;
            if (filter.Operator == "is_not_null") return present;
            if (!present) return false;
            var expected = filter.Expected is Value value ? value.Raw : filter.Expected;
            if (filter.Operator == "contain") return actual.ToString().Contains(expected?.ToString() ?? "", StringComparison.Ordinal);
            if (filter.Operator == "not_contain") return !actual.ToString().Contains(expected?.ToString() ?? "", StringComparison.Ordinal);
            if (filter.Operator == "begin_with") return actual.ToString().StartsWith(expected?.ToString() ?? "", StringComparison.Ordinal);
            if (filter.Operator == "not_begin_with") return !actual.ToString().StartsWith(expected?.ToString() ?? "", StringComparison.Ordinal);
            if (filter.Operator == "end_with") return actual.ToString().EndsWith(expected?.ToString() ?? "", StringComparison.Ordinal);
            if (filter.Operator == "not_end_with") return !actual.ToString().EndsWith(expected?.ToString() ?? "", StringComparison.Ordinal);
            if (filter.Operator == "sound_like") return Soundex(actual.ToString()) == Soundex(expected?.ToString());
            if (filter.Operator == "gt") return Convert.ToDecimal(actual.ToString()) > Convert.ToDecimal(expected);
            if (filter.Operator == "gte") return Convert.ToDecimal(actual.ToString()) >= Convert.ToDecimal(expected);
            if (filter.Operator == "lt") return Convert.ToDecimal(actual.ToString()) < Convert.ToDecimal(expected);
            if (filter.Operator == "lte") return Convert.ToDecimal(actual.ToString()) <= Convert.ToDecimal(expected);
            if (filter.Operator == "in" || filter.Operator == "not_in")
            {
                var found = ((IEnumerable<object>)filter.Expected).Any(item => actual.ToString() == (item is Value v ? v.Raw : item)?.ToString());
                return filter.Operator == "in" ? found : !found;
            }
            if (filter.Operator == "between")
            {
                var bounds = ((IEnumerable<object>)filter.Expected).ToArray();
                var number = Convert.ToDecimal(actual.ToString());
                return number >= Convert.ToDecimal(bounds[0]) && number <= Convert.ToDecimal(bounds[1]);
            }
            return filter.Operator == "ne" ? actual.ToString() != expected?.ToString() : actual.ToString() == expected?.ToString();
        }

        private static string Soundex(string value)
        {
            var letters = new string((value ?? "").ToUpperInvariant().Where(char.IsLetter).ToArray());
            if (letters.Length == 0) return "";
            static char Code(char c) => "BFPV".Contains(c) ? '1' : "CGJKQSXZ".Contains(c) ? '2' : "DT".Contains(c) ? '3' : c == 'L' ? '4' : "MN".Contains(c) ? '5' : c == 'R' ? '6' : '0';
            var result = letters[0].ToString();
            var previous = Code(letters[0]);
            foreach (var c in letters.Skip(1)) { var code = Code(c); if (code != '0' && code != previous) result += code; previous = code; if (result.Length == 4) break; }
            return result.PadRight(4, '0')[..4];
        }

        private static string SortValue(Dictionary<string, JsonElement> row, string field) => row.TryGetValue(field, out var value) ? value.ToString() : "";
        private Dictionary<string, Dictionary<string, JsonElement>> Table(string entity) => _state.Data.TryGetValue(entity, out var table) ? table : _state.Data[entity] = new();
        private long NextId(string entity) { var id = _state.NextIds.TryGetValue(entity, out var value) ? value : 1; _state.NextIds[entity] = id + 1; return id; }
        private static Dictionary<string, JsonElement> ToStoredRecord(Record record) => record.ToDictionary(pair => pair.Key, pair => JsonSerializer.SerializeToElement(pair.Value.Raw));
        private static Record ToRecord(Dictionary<string, JsonElement> stored) { var result = new Record(); foreach (var pair in stored) result[pair.Key] = FromElement(pair.Value); return result; }
        private static Value FromElement(JsonElement value) => value.ValueKind switch { JsonValueKind.Null => new Value.NullValue(), JsonValueKind.Number when value.TryGetInt64(out var number) => new Value.I64Value(number), JsonValueKind.Number => new Value.F64Value(value.GetDouble()), JsonValueKind.True => new Value.BoolValue(true), JsonValueKind.False => new Value.BoolValue(false), _ => new Value.TextValue(value.ToString()) };
        private void Persist() { var directory = Path.GetDirectoryName(Path.GetFullPath(_path)); if (directory != null) Directory.CreateDirectory(directory); var temporary = _path + ".tmp"; File.WriteAllText(temporary, JsonSerializer.Serialize(_state)); File.Move(temporary, _path, true); }
    }

    public abstract class AdoNetSqlDataService : IDataService, IGraphTransactionDataService, ISchemaExecutor, IAsyncDisposable
    {
        private DbConnection _graphConnection;
        private DbTransaction _graphTransaction;
        protected abstract DbConnection CreateConnection();
        protected abstract string QuoteSafeIdentifier(string identifier);
        protected abstract string ContainsSql(string column, string parameter);
        protected abstract string StartsWithSql(string column, string parameter);
        protected abstract string EndsWithSql(string column, string parameter);
        protected abstract string ApplyPagination(string statement, SelectQuery query, bool hasOrder);
        protected abstract string CreateTableSql(string table);
        protected abstract string ColumnExistsSql(string table, string column);
        protected abstract string AddColumnSql(string table, string column, string type);
        protected abstract string CreateIdSpaceSql { get; }
        protected abstract string ColumnType(Value value);
        protected virtual string DatabaseKind => GetType().Name
            .Replace("DataService", "", StringComparison.Ordinal).ToLowerInvariant();

        public async ValueTask DisposeAsync()
        {
            if (_graphTransaction != null) await _graphTransaction.DisposeAsync();
            if (_graphConnection != null) await _graphConnection.DisposeAsync();
            _graphTransaction = null;
            _graphConnection = null;
        }

        public async Task BeginGraphTransactionAsync()
        {
            if (_graphTransaction != null) throw new InvalidOperationException("A graph transaction is already active");
            _graphConnection = await OpenConnectionAsync();
            try { _graphTransaction = await _graphConnection.BeginTransactionAsync(); }
            catch { await _graphConnection.DisposeAsync(); _graphConnection = null; throw; }
        }

        public async Task CommitGraphTransactionAsync()
        {
            if (_graphTransaction == null) throw new InvalidOperationException("No graph transaction is active");
            try { await _graphTransaction.CommitAsync(); }
            finally { await CloseGraphTransactionAsync(); }
        }

        public async Task RollbackGraphTransactionAsync()
        {
            if (_graphTransaction == null) throw new InvalidOperationException("No graph transaction is active");
            try { await _graphTransaction.RollbackAsync(); }
            finally { await CloseGraphTransactionAsync(); }
        }

        private async Task CloseGraphTransactionAsync()
        {
            var transaction = _graphTransaction; var connection = _graphConnection;
            _graphTransaction = null; _graphConnection = null;
            if (transaction != null) await transaction.DisposeAsync();
            if (connection != null) await connection.DisposeAsync();
        }

        private sealed class ConnectionLease : IAsyncDisposable
        {
            public DbConnection Connection { get; }
            public DbTransaction Transaction { get; }
            private readonly bool _ownsConnection;
            public ConnectionLease(DbConnection connection, DbTransaction transaction, bool ownsConnection)
            { Connection = connection; Transaction = transaction; _ownsConnection = ownsConnection; }
            public async ValueTask DisposeAsync()
            { if (_ownsConnection) await Connection.DisposeAsync(); }
        }

        private async Task<ConnectionLease> OpenConnectionLeaseAsync()
        {
            if (_graphConnection != null) return new ConnectionLease(_graphConnection, _graphTransaction, false);
            return new ConnectionLease(await OpenConnectionAsync(), null, true);
        }

        private sealed class MutationScope : IAsyncDisposable
        {
            public DbConnection Connection { get; }
            public DbTransaction Transaction { get; }
            private readonly bool _ownsTransaction;
            private bool _completed;
            public MutationScope(DbConnection connection, DbTransaction transaction, bool ownsTransaction)
            { Connection = connection; Transaction = transaction; _ownsTransaction = ownsTransaction; }
            public async Task CommitAsync()
            { if (_ownsTransaction) await Transaction.CommitAsync(); _completed = true; }
            public async ValueTask DisposeAsync()
            {
                if (!_ownsTransaction) return;
                if (!_completed) await Transaction.RollbackAsync();
                await Transaction.DisposeAsync();
                await Connection.DisposeAsync();
            }
        }

        private async Task<MutationScope> OpenMutationScopeAsync()
        {
            if (_graphConnection != null) return new MutationScope(_graphConnection, _graphTransaction, false);
            var connection = await OpenConnectionAsync();
            try { return new MutationScope(connection, await connection.BeginTransactionAsync(), true); }
            catch { await connection.DisposeAsync(); throw; }
        }

        async Task ISchemaExecutor.EnsureSchemaAsync(RuntimeModule module)
        {
            await EnsureProviderCapabilitiesAsync();
            foreach (var entity in module.Entities)
            {
                var sample = module.SchemaSamples.TryGetValue(entity, out var configured)
                    ? configured : new Record();
                var required = module.SchemaRequired.TryGetValue(entity, out var configuredRequired)
                    ? configuredRequired : new Dictionary<string, bool>();
                await EnsureStorageAsync(entity, sample, required);
            }
            foreach (var seed in module.RootEntities) await EnsureBootstrapAsync(seed, false);
            foreach (var seed in module.ConstantEntities) await EnsureBootstrapAsync(seed, true);
        }

        protected virtual Task EnsureProviderCapabilitiesAsync() => Task.CompletedTask;

        private async Task EnsureBootstrapAsync(BootstrapEntity seed, bool reconcile)
        {
            var values = new Record(); foreach (var pair in seed.Values) values[pair.Key] = pair.Value;
            values["id"] = new Value.I64Value(seed.Id); values["version"] = new Value.I64Value(1);
            await EnsureStorageAsync(seed.Entity, values);
            await using var connection = await OpenConnectionAsync();
            await using var count = connection.CreateCommand();
            count.CommandText = $"SELECT COUNT(*) FROM {Quote(TableName(seed.Entity))} WHERE {Quote("id")} = @id";
            AddParameter(count, "id", seed.Id);
            var exists = Convert.ToInt64(await count.ExecuteScalarAsync()) != 0;
            if (!exists)
            {
                await using var insert = connection.CreateCommand();
                var fields = values.Keys.ToList();
                insert.CommandText = $"INSERT INTO {Quote(TableName(seed.Entity))} ({string.Join(", ", fields.Select(Quote))}) VALUES ({string.Join(", ", fields.Select((_, i) => "@p" + i))})";
                for (var i = 0; i != fields.Count; i++) AddParameter(insert, "p" + i, ToDbValue(values[fields[i]]));
                await insert.ExecuteNonQueryAsync();
            }
            else if (reconcile && seed.Values.Count > 0)
            {
                await using var update = connection.CreateCommand();
                var fields = seed.Values.Keys.Where(field => field != "id" && field != "version").ToList();
                update.CommandText = $"UPDATE {Quote(TableName(seed.Entity))} SET {string.Join(", ", fields.Select((field, i) => Quote(field) + " = @p" + i))}, {Quote("version")} = {Quote("version")} + 1 WHERE {Quote("id")} = @id AND ({string.Join(" OR ", fields.Select((field, i) => Quote(field) + " <> @p" + i))})";
                for (var i = 0; i != fields.Count; i++) AddParameter(update, "p" + i, ToDbValue(seed.Values[fields[i]]));
                AddParameter(update, "id", seed.Id);
                if (fields.Count > 0) await update.ExecuteNonQueryAsync();
            }
            await EnsureIdFloorAsync(seed.Entity, seed.Id);
        }

        public async Task<object> MutateAsync(UserContext context, MutationRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Comment))
                throw new InvalidOperationException("Security audit failure: mutation audit reason is required");
            context.CheckAndFix(req);
            context.CurrentMutationAuditReason = req.Comment;
            try
            {
                var result = req.Command is InsertCommand insert ? await InsertAsync(context, insert)
                    : req.Command is UpdateCommand update ? await UpdateAsync(context, update)
                    : req.Command is DeleteCommand delete ? await DeleteAsync(context, delete)
                    : throw new InvalidOperationException("Unsupported mutation command");
                await context.EmitMutationAuditAsync(req.Command, result, req.Comment);
                return result;
            }
            finally { context.CurrentMutationAuditReason = null; }
        }

        private async Task<MutationResult> InsertAsync(UserContext context, InsertCommand command)
        {
            var values = new Record();
            foreach (var pair in command.Values) values[pair.Key] = pair.Value;
            var id = values.TryGetValue("id", out var idValue)
                ? Convert.ToInt64(idValue.Raw)
                : await NextIdAsync(command.Entity);
            if (idValue != null) await EnsureIdFloorAsync(command.Entity, id);
            values["id"] = new Value.I64Value(id);
            values["version"] = new Value.I64Value(1);
            await EnsureStorageAsync(command.Entity, values);

            var fields = values.Keys.ToList();
            await using var scope = await OpenMutationScopeAsync();
            var connection = scope.Connection;
            var transaction = scope.Transaction;
            await using var sql = connection.CreateCommand();
            sql.Transaction = transaction;
            sql.CommandText = $"INSERT INTO {Quote(TableName(command.Entity))} ({string.Join(", ", fields.Select(Quote))}) VALUES ({string.Join(", ", fields.Select((_, index) => "@p" + index))})";
            for (var index = 0; index != fields.Count; index++)
            {
                AddParameter(sql, "p" + index, ToDbValue(values[fields[index]]));
            }
            var startedAt = DateTimeOffset.UtcNow;
            var affected = await sql.ExecuteNonQueryAsync();
            var elapsed = DateTimeOffset.UtcNow - startedAt;
            var evidence = BuildSqlEvidence("insert", sql, elapsed, null, affected);
            evidence.AuditReason = context.CurrentMutationAuditReason;
            evidence.TracePath = new[] { "operation:mutation", $"entity:{command.Entity}", "provider:" + DatabaseKind, "sql:insert" };
            context.RecordSqlEvidence(evidence);
            if (affected != 1) throw new InvalidOperationException($"Insert affected {affected} rows");
            var persisted = await ReadPersistedRecordAsync(connection, transaction, command.Entity, id);
            await scope.CommitAsync();
            return new MutationResult { Success = true, Id = id, Version = 1, PersistedRecord = persisted };
        }

        private async Task<MutationResult> UpdateAsync(UserContext context, UpdateCommand command)
        {
            var id = Convert.ToInt64(command.Id.Raw);
            var oldVersion = command.Values.TryGetValue("version", out var versionValue)
                ? Convert.ToInt64(versionValue.Raw)
                : await CurrentVersionAsync(command.Entity, id);
            var nextVersion = oldVersion + 1;
            var values = new Record();
            foreach (var pair in command.Values)
            {
                if (pair.Key != "id" && pair.Key != "version") values[pair.Key] = pair.Value;
            }
            await EnsureStorageAsync(command.Entity, values);

            var fields = values.Keys.ToList();
            var assignments = fields.Select((field, index) => $"{Quote(field)} = @p{index}").ToList();
            assignments.Add($"{Quote("version")} = @nextVersion");
            await using var scope = await OpenMutationScopeAsync();
            var connection = scope.Connection;
            var transaction = scope.Transaction;
            await using var sql = connection.CreateCommand();
            sql.Transaction = transaction;
            sql.CommandText = $"UPDATE {Quote(TableName(command.Entity))} SET {string.Join(", ", assignments)} WHERE {Quote("id")} = @id AND {Quote("version")} = @oldVersion";
            for (var index = 0; index != fields.Count; index++)
            {
                AddParameter(sql, "p" + index, ToDbValue(values[fields[index]]));
            }
            AddParameter(sql, "nextVersion", nextVersion);
            AddParameter(sql, "id", id);
            AddParameter(sql, "oldVersion", oldVersion);
            var startedAt = DateTimeOffset.UtcNow;
            var affected = await sql.ExecuteNonQueryAsync();
            var elapsed = DateTimeOffset.UtcNow - startedAt;
            var evidence = BuildSqlEvidence("update", sql, elapsed, null, affected);
            evidence.AuditReason = context.CurrentMutationAuditReason;
            evidence.TracePath = new[] { "operation:mutation", $"entity:{command.Entity}", "provider:" + DatabaseKind, "sql:update" };
            context.RecordSqlEvidence(evidence);
            if (affected != 1) throw new InvalidOperationException($"Optimistic lock failed for {command.Entity}({id}) at version {oldVersion}");
            var persisted = await ReadPersistedRecordAsync(connection, transaction, command.Entity, id);
            await scope.CommitAsync();
            return new MutationResult { Success = true, Id = id, Version = nextVersion, PersistedRecord = persisted };
        }

        private async Task<MutationResult> DeleteAsync(UserContext context, DeleteCommand command)
        {
            var id = Convert.ToInt64(command.Id.Raw);
            var version = Convert.ToInt64(command.Version.Raw);
            await using var scope = await OpenMutationScopeAsync();
            var connection = scope.Connection;
            var transaction = scope.Transaction;
            await using var sql = connection.CreateCommand();
            sql.Transaction = transaction;
            var deletedVersion = -(version + 1);
            sql.CommandText = $"UPDATE {Quote(TableName(command.Entity))} SET {Quote("version")} = @deletedVersion WHERE {Quote("id")} = @id AND {Quote("version")} = @version";
            AddParameter(sql, "id", id);
            AddParameter(sql, "version", version);
            AddParameter(sql, "deletedVersion", deletedVersion);
            var startedAt = DateTimeOffset.UtcNow;
            var affected = await sql.ExecuteNonQueryAsync();
            var elapsed = DateTimeOffset.UtcNow - startedAt;
            var evidence = BuildSqlEvidence("delete", sql, elapsed, null, affected);
            evidence.AuditReason = context.CurrentMutationAuditReason;
            evidence.TracePath = new[] { "operation:mutation", $"entity:{command.Entity}", "provider:" + DatabaseKind, "sql:delete" };
            context.RecordSqlEvidence(evidence);
            if (affected != 1) throw new InvalidOperationException($"Optimistic lock failed for {command.Entity}({id}) at version {version}");
            var persisted = await ReadPersistedRecordAsync(connection, transaction, command.Entity, id);
            await scope.CommitAsync();
            return new MutationResult { Success = true, Id = id, Version = deletedVersion, Deleted = true, PersistedRecord = persisted };
        }

        private string CompileFilter(DbCommand sql, FilterExpression filter)
        {
            if (filter.Operator == "is_null" || filter.Operator == "is_not_null")
                return $"{Quote(filter.Field)} IS {(filter.Operator == "is_null" ? "" : "NOT ")}NULL";
            if (filter.Operator == "in_subquery" || filter.Operator == "not_in_subquery")
            {
                if (filter.Subquery == null || string.IsNullOrWhiteSpace(filter.SubqueryField))
                    throw new InvalidOperationException("Relation subquery requires a query and projected field");
                var negative = filter.Operator == "not_in_subquery";
                var childWhere = filter.Subquery.Filters.Select(child => CompileFilter(sql, child)).ToList();
                if (negative) childWhere.Add($"{Quote(filter.SubqueryField)} IS NOT NULL");
                var childSql = $"SELECT {Quote(filter.SubqueryField)} FROM {Quote(TableName(filter.Subquery.Entity))}";
                if (childWhere.Count != 0) childSql += " WHERE " + string.Join(" AND ", childWhere);
                return $"{Quote(filter.Field)} {(negative ? "NOT IN" : "IN")} ({childSql})";
            }
            if (filter.Operator == "in" || filter.Operator == "not_in")
            {
                var placeholders = new List<string>();
                foreach (var expected in (IEnumerable<object>)filter.Expected)
                {
                    var parameter = "p" + sql.Parameters.Count;
                    placeholders.Add("@" + parameter);
                    AddParameter(sql, parameter, expected ?? DBNull.Value);
                }
                return placeholders.Count == 0
                    ? (filter.Operator == "in" ? "1 = 0" : "1 = 1")
                    : $"{Quote(filter.Field)} {(filter.Operator == "in" ? "IN" : "NOT IN")} ({string.Join(", ", placeholders)})";
            }
            if (filter.Operator == "between")
            {
                var bounds = ((IEnumerable<object>)filter.Expected).ToArray();
                var lower = "p" + sql.Parameters.Count;
                AddParameter(sql, lower, bounds[0] ?? DBNull.Value);
                var upper = "p" + sql.Parameters.Count;
                AddParameter(sql, upper, bounds[1] ?? DBNull.Value);
                return $"{Quote(filter.Field)} BETWEEN @{lower} AND @{upper}";
            }
            if (filter.Operator == "sound_like")
            {
                var parameter = "p" + sql.Parameters.Count;
                AddParameter(sql, parameter, filter.Expected ?? DBNull.Value);
                return $"SOUNDEX({Quote(filter.Field)}) = SOUNDEX(@{parameter})";
            }
            var scalarParameter = "p" + sql.Parameters.Count;
            var predicate = filter.Operator switch
            {
                "contain" => ContainsSql(Quote(filter.Field), scalarParameter),
                "not_contain" => $"NOT ({ContainsSql(Quote(filter.Field), scalarParameter)})",
                "begin_with" => StartsWithSql(Quote(filter.Field), scalarParameter),
                "not_begin_with" => $"NOT ({StartsWithSql(Quote(filter.Field), scalarParameter)})",
                "end_with" => EndsWithSql(Quote(filter.Field), scalarParameter),
                "not_end_with" => $"NOT ({EndsWithSql(Quote(filter.Field), scalarParameter)})",
                "ne" => $"{Quote(filter.Field)} <> @{scalarParameter}",
                "gte" => $"{Quote(filter.Field)} >= @{scalarParameter}",
                "lte" => $"{Quote(filter.Field)} <= @{scalarParameter}",
                "gt" => $"{Quote(filter.Field)} > @{scalarParameter}",
                "lt" => $"{Quote(filter.Field)} < @{scalarParameter}",
                _ => $"{Quote(filter.Field)} = @{scalarParameter}"
            };
            var scalarExpected = filter.Expected is Value value ? value.Raw : filter.Expected;
            AddParameter(sql, scalarParameter, scalarExpected ?? DBNull.Value);
            return predicate;
        }

        public async Task<QueryResult> QueryAsync(UserContext context, QueryRequest req)
            => await IdSetPaginationRuntime.ExecuteAsync(context, req.Query,
                query => QueryCoreAsync(context, new QueryRequest(query)));

        private async Task<QueryResult> QueryCoreAsync(UserContext context, QueryRequest req)
        {
            var prepared = ContinuousPageRuntime.Prepare(context, req.Query);
            var query = prepared.Query;
            var where = new List<string>();
            await using var connection = await OpenConnectionAsync();
            await using var sql = connection.CreateCommand();
            foreach (var filter in query.Filters) where.Add(CompileFilter(sql, filter));
            var projection = query.Projections.Count == 0
                ? "*"
                : string.Join(", ", query.Projections.Select(Quote));
            if (query.Aggregates.Count != 0)
            {
                var aggregateFunctions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["count"] = "COUNT", ["sum"] = "SUM", ["avg"] = "AVG",
                    ["min"] = "MIN", ["max"] = "MAX"
                };
                var projections = query.GroupFields.Select(Quote).ToList();
                foreach (var aggregate in query.Aggregates)
                {
                    if (!aggregateFunctions.TryGetValue(aggregate.Function, out var sqlFunction))
                        throw new NotSupportedException($"Unsupported aggregate function: {aggregate.Function}");
                    projections.Add($"{sqlFunction}({Quote(aggregate.Field)}) AS {Quote(aggregate.Alias)}");
                }
                projection = string.Join(", ", projections);
            }
            if (!string.IsNullOrEmpty(query.PartitionBy))
            {
                var windowOrder = query.Orders.Count == 0 ? "" : " ORDER BY " + string.Join(", ", query.Orders.Select(order =>
                    $"{Quote(order.Field)} {(order.Direction.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC")}"));
                projection += $", ROW_NUMBER() OVER (PARTITION BY {Quote(query.PartitionBy)}{windowOrder}) AS {Quote("__teaql_partition_rank")}";
            }
            var statement = $"SELECT {projection} FROM {Quote(TableName(query.Entity))}";
            if (where.Count != 0) statement += " WHERE " + string.Join(" AND ", where);
            if (query.GroupFields.Count != 0) statement += " GROUP BY " + string.Join(", ", query.GroupFields.Select(Quote));
            if (!string.IsNullOrEmpty(query.PartitionBy))
            {
                var rank = Quote("__teaql_partition_rank");
                var rankWhere = $"{rank} > {query.OffsetValue}";
                if (query.LimitValue.HasValue) rankWhere += $" AND {rank} <= {query.OffsetValue + query.LimitValue.Value}";
                statement = $"SELECT * FROM ({statement}) AS {Quote("__teaql_partitioned")} WHERE {rankWhere} ORDER BY {rank}";
            }
            else if (query.Orders.Count != 0)
            {
                statement += " ORDER BY " + string.Join(", ", query.Orders.Select(order =>
                    $"{Quote(order.Field)} {(order.Direction.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC")}"));
            }
            sql.CommandText = string.IsNullOrEmpty(query.PartitionBy)
                ? ApplyPagination(statement, query, query.Orders.Count != 0)
                : statement;
            context.SqlTrace.Add(sql.CommandText);

            var result = new QueryResult();
            var startedAt = DateTimeOffset.UtcNow;
            await using (var reader = await sql.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var record = new Record();
                    for (var index = 0; index != reader.FieldCount; index++)
                    {
                        record[reader.GetName(index)] = reader.IsDBNull(index)
                            ? new Value.NullValue()
                            : FromDbValue(reader.GetValue(index));
                    }
                    result.Rows.Add(record);
                }
            }
            var elapsed = DateTimeOffset.UtcNow - startedAt;
            var evidence = BuildSqlEvidence("query", sql, elapsed, result.Rows.Count, null);
            evidence.Comment = query.CommentText;
            evidence.Purpose = query.PurposeText;
            evidence.TracePath = new[] { "operation:query", $"request:{query.Entity}" }
                .Concat(query.TracePath)
                .Concat(new[] { "provider:" + DatabaseKind, "sql:select" }).ToArray();
            context.RecordSqlEvidence(evidence);
            await EnhanceRelationsAsync(context, query, result.Rows);
            await EnhanceRelationAggregatesAsync(context, query, result.Rows);
            ContinuousPageRuntime.Register(context, prepared.Execution, result.Rows);
            return result;
        }

        private async Task EnhanceRelationsAsync(UserContext context, SelectQuery query, List<Record> parents)
        {
            if (parents.Count == 0 || query.Relations.Count == 0) return;
            foreach (var load in query.Relations)
            {
                var parentIds = parents
                    .Where(parent => parent.ContainsKey(load.LocalKey))
                    .Select(parent => parent[load.LocalKey].Raw)
                    .Cast<object>().ToList();
                var child = CloneQuery(load.Query, load.TargetEntity);
                child.Comment(query.CommentText);
                child.Purpose(query.PurposeText);
                child.TracePath.AddRange(query.TracePath);
                child.TracePath.Add($"relation:{query.Entity}.{load.Name}");
                child.ClearContinuousPageFetch();
                if (child.LimitValue.HasValue && !child.Orders.Any(order => order.Field == "id"))
                    child.OrderBy("id", "asc");
                var useProbes = child.LimitValue.HasValue &&
                    (!child.TopNProbeThreshold.HasValue ||
                     child.TopNProbeThreshold.Value > 0 && parentIds.Count <= child.TopNProbeThreshold.Value);
                var children = new List<Record>();
                if (useProbes)
                {
                    foreach (var parentId in parentIds)
                    {
                        var probe = CloneQuery(child, load.TargetEntity);
                        probe.PartitionBy = null;
                        probe.Filters.Add(new FilterExpression { Operator = "eq", Field = load.ForeignKey, Expected = parentId });
                        children.AddRange((await QueryAsync(context, new QueryRequest(probe))).Rows);
                    }
                }
                else
                {
                    child.Filters.Add(new FilterExpression { Operator = "in", Field = load.ForeignKey, Expected = parentIds });
                    if (child.LimitValue.HasValue) child.PartitionBy = load.ForeignKey;
                    children.AddRange((await QueryAsync(context, new QueryRequest(child))).Rows);
                }
                foreach (var row in children) row.Remove("__teaql_partition_rank");
                var buckets = children
                    .Where(row => row.ContainsKey(load.ForeignKey))
                    .GroupBy(row => row[load.ForeignKey].Raw?.ToString())
                    .ToDictionary(group => group.Key, group => group.ToList());
                foreach (var parent in parents)
                {
                    var key = parent.TryGetValue(load.LocalKey, out var value) ? value.Raw?.ToString() : null;
                    var related = key != null && buckets.TryGetValue(key, out var rows) ? rows : new List<Record>();
                    parent[load.Name] = new Value.ObjectValue(load.Many ? related : related.Take(1).ToList());
                }
            }
        }

        private async Task EnhanceRelationAggregatesAsync(UserContext context, SelectQuery query, List<Record> parents)
        {
            if (parents.Count == 0 || query.RelationAggregates.Count == 0) return;
            foreach (var aggregate in query.RelationAggregates)
            {
                var parentIds = parents.Where(parent => parent.ContainsKey(aggregate.LocalKey))
                    .Select(parent => parent[aggregate.LocalKey].Raw).Cast<object>().ToList();
                var child = CloneQuery(aggregate.Query, aggregate.TargetEntity);
                child.ClearContinuousPageFetch();
                child.Projections.Clear(); child.Orders.Clear(); child.Relations.Clear(); child.RelationAggregates.Clear();
                if (child.Aggregates.Count == 0) child.Aggregate("Count", "id", aggregate.Alias);
                if (!child.GroupFields.Contains(aggregate.ForeignKey)) child.GroupFields.Add(aggregate.ForeignKey);
                child.Filters.Add(new FilterExpression { Operator = "in", Field = aggregate.ForeignKey, Expected = parentIds });
                var rows = (await QueryAsync(context, new QueryRequest(child))).Rows;
                var buckets = rows.Where(row => row.ContainsKey(aggregate.ForeignKey))
                    .ToDictionary(row => row[aggregate.ForeignKey].Raw?.ToString(), row => row);
                foreach (var parent in parents)
                {
                    var key = parent.TryGetValue(aggregate.LocalKey, out var local) ? local.Raw?.ToString() : null;
                    if (key == null || !buckets.TryGetValue(key, out var row))
                    {
                        parent[aggregate.Alias] = aggregate.SingleResult && (aggregate.Query.Aggregates.Count == 0 || aggregate.Query.Aggregates[0].Function.Equals("Count", StringComparison.OrdinalIgnoreCase))
                            ? new Value.I64Value(0) : new Value.NullValue();
                        continue;
                    }
                    if (aggregate.SingleResult)
                    {
                        var innerAlias = child.Aggregates.Count == 0 ? aggregate.Alias : child.Aggregates[0].Alias;
                        parent[aggregate.Alias] = row.TryGetValue(innerAlias, out var value) ? value : new Value.NullValue();
                    }
                    else parent[aggregate.Alias] = new Value.ObjectValue(row);
                }
            }
        }

        private static SelectQuery CloneQuery(SelectQuery source, string entity)
        {
            var clone = new SelectQuery(entity) { PartitionBy = source.PartitionBy };
            clone.Comment(source.CommentText);
            clone.Purpose(source.PurposeText);
            clone.TracePath.AddRange(source.TracePath);
            if (source.LimitValue.HasValue) clone.Limit(source.LimitValue.Value);
            clone.Offset(source.OffsetValue);
            foreach (var filter in source.Filters) clone.Filters.Add(filter);
            foreach (var projection in source.Projections) clone.Projections.Add(projection);
            foreach (var order in source.Orders) clone.Orders.Add(order);
            foreach (var group in source.GroupFields) clone.GroupFields.Add(group);
            foreach (var aggregate in source.Aggregates) clone.Aggregates.Add(aggregate);
            foreach (var relation in source.Relations) clone.Relations.Add(relation);
            foreach (var aggregate in source.RelationAggregates) clone.RelationAggregates.Add(aggregate);
            if (source.TopNProbeThreshold.HasValue) clone.TopNProbeParentThreshold(source.TopNProbeThreshold.Value);
            return clone;
        }

        protected virtual async Task EnsureStorageAsync(string entity, Record values, IReadOnlyDictionary<string, bool> required = null)
        {
            var table = TableName(entity);
            await using var lease = await OpenConnectionLeaseAsync();
            var connection = lease.Connection;
            await using (var create = connection.CreateCommand())
            {
                create.Transaction = lease.Transaction;
                create.CommandText = CreateTableSql(table);
                await create.ExecuteNonQueryAsync();
            }
            foreach (var pair in values)
            {
                if (pair.Key == "id" || pair.Key == "version") continue;
                await using var checkColumn = connection.CreateCommand();
                checkColumn.Transaction = lease.Transaction;
                checkColumn.CommandText = ColumnExistsSql(table, pair.Key);
                if (await checkColumn.ExecuteScalarAsync() != null) continue;
                await using var addColumn = connection.CreateCommand();
                addColumn.Transaction = lease.Transaction;
                var columnType = ColumnType(pair.Value)
                    + (required != null && required.TryGetValue(pair.Key, out var isRequired) && isRequired ? " NOT NULL" : "");
                addColumn.CommandText = AddColumnSql(table, pair.Key, columnType);
                await addColumn.ExecuteNonQueryAsync();
            }
        }

        protected virtual async Task<long> NextIdAsync(string entity)
        {
            for (var attempt = 1; attempt <= 100; attempt++)
            {
                await using var lease = await OpenConnectionLeaseAsync();
                var connection = lease.Connection;
                await using (var create = connection.CreateCommand())
                {
                    create.Transaction = lease.Transaction;
                    create.CommandText = CreateIdSpaceSql;
                    await create.ExecuteNonQueryAsync();
                }
                await using var select = connection.CreateCommand();
                select.Transaction = lease.Transaction;
                select.CommandText = "SELECT current_level FROM teaql_id_space WHERE type_name = @entity";
                AddParameter(select, "entity", entity);
                var value = await select.ExecuteScalarAsync();
                if (value == null || value == DBNull.Value)
                {
                    try
                    {
                        await using var insert = connection.CreateCommand();
                        insert.Transaction = lease.Transaction;
                        insert.CommandText = "INSERT INTO teaql_id_space(type_name, current_level) VALUES (@entity, 1)";
                        AddParameter(insert, "entity", entity);
                        if (await insert.ExecuteNonQueryAsync() == 1) return 1;
                    }
                    catch
                    {
                        await using var winner = connection.CreateCommand();
                        winner.Transaction = lease.Transaction;
                        winner.CommandText = "SELECT current_level FROM teaql_id_space WHERE type_name = @entity";
                        AddParameter(winner, "entity", entity);
                        if (await winner.ExecuteScalarAsync() == null) throw;
                    }
                    continue;
                }
                var current = Convert.ToInt64(value);
                if (current == long.MaxValue) throw new OverflowException($"ID space overflow for {entity}");
                var next = current + 1;
                await using var update = connection.CreateCommand();
                update.Transaction = lease.Transaction;
                update.CommandText = "UPDATE teaql_id_space SET current_level = @next WHERE type_name = @entity AND current_level = @current";
                AddParameter(update, "next", next);
                AddParameter(update, "entity", entity);
                AddParameter(update, "current", current);
                var changed = await update.ExecuteNonQueryAsync();
                if (changed == 1) return next;
                if (changed != 0) throw new InvalidOperationException($"ID space update for {entity} changed {changed} rows on attempt {attempt}");
            }
            throw new InvalidOperationException($"Unable to allocate ID for {entity} after 100 optimistic-lock attempts");
        }

        protected virtual async Task EnsureIdFloorAsync(string entity, long floor)
        {
            if (floor < 0) throw new ArgumentOutOfRangeException(nameof(floor));
            for (var attempt = 1; attempt <= 100; attempt++)
            {
                await using var lease = await OpenConnectionLeaseAsync();
                var connection = lease.Connection;
                await using (var create = connection.CreateCommand())
                {
                    create.Transaction = lease.Transaction;
                    create.CommandText = CreateIdSpaceSql;
                    await create.ExecuteNonQueryAsync();
                }
                await using var select = connection.CreateCommand();
                select.Transaction = lease.Transaction;
                select.CommandText = "SELECT current_level FROM teaql_id_space WHERE type_name = @entity";
                AddParameter(select, "entity", entity);
                var value = await select.ExecuteScalarAsync();
                if (value == null || value == DBNull.Value)
                {
                    try
                    {
                        await using var insert = connection.CreateCommand();
                        insert.Transaction = lease.Transaction;
                        insert.CommandText = "INSERT INTO teaql_id_space(type_name, current_level) VALUES (@entity, @floor)";
                        AddParameter(insert, "entity", entity);
                        AddParameter(insert, "floor", floor);
                        if (await insert.ExecuteNonQueryAsync() == 1) return;
                    }
                    catch
                    {
                        await using var winner = connection.CreateCommand();
                        winner.Transaction = lease.Transaction;
                        winner.CommandText = "SELECT current_level FROM teaql_id_space WHERE type_name = @entity";
                        AddParameter(winner, "entity", entity);
                        if (await winner.ExecuteScalarAsync() == null) throw;
                    }
                    continue;
                }
                var current = Convert.ToInt64(value);
                if (current >= floor) return;
                await using var update = connection.CreateCommand();
                update.Transaction = lease.Transaction;
                update.CommandText = "UPDATE teaql_id_space SET current_level = @floor WHERE type_name = @entity AND current_level = @current";
                AddParameter(update, "floor", floor);
                AddParameter(update, "entity", entity);
                AddParameter(update, "current", current);
                var changed = await update.ExecuteNonQueryAsync();
                if (changed == 1) return;
                if (changed != 0) throw new InvalidOperationException($"ID space floor update for {entity} changed {changed} rows on attempt {attempt}");
            }
            throw new InvalidOperationException($"Unable to synchronize ID space floor for {entity} after 100 optimistic-lock attempts");
        }

        private async Task<long> CurrentVersionAsync(string entity, long id)
        {
            await using var lease = await OpenConnectionLeaseAsync();
            var connection = lease.Connection;
            await using var sql = connection.CreateCommand();
            sql.Transaction = lease.Transaction;
            sql.CommandText = $"SELECT {Quote("version")} FROM {Quote(TableName(entity))} WHERE {Quote("id")} = @id";
            AddParameter(sql, "id", id);
            var version = await sql.ExecuteScalarAsync();
            if (version == null) throw new KeyNotFoundException($"{entity}({id}) does not exist");
            return Convert.ToInt64(version);
        }

        protected async Task<DbConnection> OpenConnectionAsync()
        {
            var connection = CreateConnection();
            await connection.OpenAsync();
            return connection;
        }

        protected void AddParameter(DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = NormalizeParameterValue(value) ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static SqlExecutionEvidence BuildSqlEvidence(
            string operation,
            DbCommand command,
            TimeSpan elapsed,
            int? resultCount,
            long? affectedRows)
        {
            var parameters = command.Parameters.Cast<DbParameter>()
                .Select(parameter => parameter.Value).ToList();
            return new SqlExecutionEvidence
            {
                Operation = operation,
                ParameterizedSql = command.CommandText,
                DebugSql = RenderDiagnosticSql(command),
                Parameters = parameters,
                ElapsedMicros = (long)(elapsed.TotalMilliseconds * 1000),
                ResultCount = resultCount,
                AffectedRows = affectedRows,
                ResultSummary = resultCount.HasValue
                    ? $"{resultCount.Value} rows returned"
                    : $"{affectedRows ?? 0} rows affected"
            };
        }

        private static string RenderDiagnosticSql(DbCommand command)
        {
            var result = command.CommandText;
            foreach (var parameter in command.Parameters.Cast<DbParameter>()
                         .OrderByDescending(parameter => parameter.ParameterName.Length))
            {
                var token = parameter.ParameterName.StartsWith("@")
                    ? parameter.ParameterName : "@" + parameter.ParameterName;
                result = result.Replace(token, DiagnosticLiteral(parameter.Value));
            }
            return result;
        }

        private static string DiagnosticLiteral(object value)
        {
            if (value == null || value == DBNull.Value) return "NULL";
            if (value is bool boolean) return boolean ? "1" : "0";
            if (value is byte[] bytes) return "X'" + Convert.ToHexString(bytes) + "'";
            if (value is string text) return "'" + text.Replace("'", "''") + "'";
            if (value is DateTime dateTime) return "'" + dateTime.ToString("O", System.Globalization.CultureInfo.InvariantCulture) + "'";
            if (value is IFormattable formattable) return formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            return "'" + value.ToString().Replace("'", "''") + "'";
        }

        protected virtual object NormalizeParameterValue(object value) => value is TimeSpan time
            ? time.ToString("c", System.Globalization.CultureInfo.InvariantCulture)
            : value;

        private static object ToDbValue(Value value) => value.Raw switch
        {
            DateTime dateTime => dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime.ToUniversalTime(),
            _ => value.Raw ?? DBNull.Value
        };

        private async Task<Record> ReadPersistedRecordAsync(DbConnection connection, DbTransaction transaction, string entity, long id)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $"SELECT * FROM {Quote(TableName(entity))} WHERE {Quote("id")} = @id";
            AddParameter(command, "id", id);
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) throw new KeyNotFoundException($"{entity}({id}) does not exist after mutation");
            var record = new Record();
            for (var index = 0; index != reader.FieldCount; index++)
                record[reader.GetName(index)] = reader.IsDBNull(index) ? new Value.NullValue() : FromDbValue(reader.GetValue(index));
            return record;
        }

        private static Value FromDbValue(object value) => value switch
        {
            bool boolean => new Value.BoolValue(boolean),
            byte number => new Value.I64Value(number),
            short number => new Value.I64Value(number),
            int number => new Value.I64Value(number),
            long number => new Value.I64Value(number),
            float number => new Value.F64Value(number),
            double number => new Value.F64Value(number),
            decimal number => new Value.DecimalValue(number),
            DateTime dateTime => new Value.DateTimeValue(dateTime),
            _ => new Value.TextValue(value.ToString() ?? "")
        };

        protected static string TableName(string entity)
        {
            var result = new System.Text.StringBuilder();
            foreach (var character in entity)
            {
                if (char.IsUpper(character) && result.Length != 0) result.Append('_');
                result.Append(char.ToLowerInvariant(character));
            }
            return result.ToString() + "_data";
        }

        protected string Quote(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier) || identifier.Any(character => !(char.IsLetterOrDigit(character) || character == '_')))
                throw new ArgumentException($"Unsafe SQL identifier: {identifier}");
            return QuoteSafeIdentifier(identifier);
        }
    }

    public sealed class SqliteDataService : AdoNetSqlDataService
    {
        private readonly string _connectionString;
        private bool _soundexEnabled;

        public SqliteDataService(string connectionString) { _connectionString = connectionString; }
        protected override object NormalizeParameterValue(object value) => value switch
        {
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff",
                System.Globalization.CultureInfo.InvariantCulture),
            TimeSpan time => time.ToString("c", System.Globalization.CultureInfo.InvariantCulture),
            _ => value
        };
        protected override DbConnection CreateConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            if (_soundexEnabled)
                connection.CreateFunction<string?, string>("soundex", Soundex, isDeterministic: true);
            return connection;
        }
        protected override Task EnsureProviderCapabilitiesAsync()
        {
            _soundexEnabled = true;
            return Task.CompletedTask;
        }
        private static string Soundex(string? value)
        {
            var letters = new string((value ?? "").ToUpperInvariant().Where(c => c is >= 'A' and <= 'Z').ToArray());
            if (letters.Length == 0) return "?000";
            static char Code(char c) => "BFPV".Contains(c) ? '1' : "CGJKQSXZ".Contains(c) ? '2' : "DT".Contains(c) ? '3' : c == 'L' ? '4' : "MN".Contains(c) ? '5' : c == 'R' ? '6' : '0';
            var result = letters[0].ToString();
            var previous = Code(letters[0]);
            foreach (var c in letters.Skip(1)) { var code = Code(c); if (code != '0' && code != previous) result += code; previous = code; if (result.Length == 4) break; }
            return result.PadRight(4, '0')[..4];
        }
        protected override string QuoteSafeIdentifier(string identifier) => $"\"{identifier}\"";
        protected override string ContainsSql(string column, string parameter) => $"CAST({column} AS TEXT) LIKE '%' || @{parameter} || '%'";
        protected override string StartsWithSql(string column, string parameter) => $"CAST({column} AS TEXT) LIKE @{parameter} || '%'";
        protected override string EndsWithSql(string column, string parameter) => $"CAST({column} AS TEXT) LIKE '%' || @{parameter}";
        protected override string ApplyPagination(string statement, SelectQuery query, bool hasOrder)
        {
            if (query.LimitValue.HasValue) statement += " LIMIT " + query.LimitValue.Value;
            if (query.OffsetValue != 0) statement += " OFFSET " + query.OffsetValue;
            return statement;
        }
        protected override string CreateTableSql(string table) => $"CREATE TABLE IF NOT EXISTS {Quote(table)} ({Quote("id")} INTEGER PRIMARY KEY, {Quote("version")} INTEGER NOT NULL)";
        protected override string ColumnExistsSql(string table, string column) => $"SELECT 1 FROM pragma_table_info('{table}') WHERE name = '{column}'";
        protected override string AddColumnSql(string table, string column, string type) => $"ALTER TABLE {Quote(table)} ADD COLUMN {Quote(column)} {type}";
        protected override string CreateIdSpaceSql => "CREATE TABLE IF NOT EXISTS teaql_id_space (type_name TEXT PRIMARY KEY, current_level INTEGER NOT NULL)";
        protected override string ColumnType(Value value) => value switch
        {
            Value.BoolValue => "INTEGER",
            Value.I64Value => "INTEGER",
            Value.F64Value => "REAL",
            Value.DecimalValue => "NUMERIC",
            Value.DateValue => "TEXT",
            Value.TimeValue => "TEXT",
            Value.DateTimeValue => "TEXT",
            Value.JsonValue => "TEXT",
            _ => "TEXT"
        };
    }
}