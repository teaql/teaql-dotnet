using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace TeaQL.Core
{
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
        public class DateTimeValue : Value { public DateTimeValue(DateTime v) : base(v) {} }
        public class JsonValue : Value { public JsonValue(string v) : base(v) {} }
        public class ObjectValue : Value { public ObjectValue(object v) : base(v) {} }
        public class NullValue : Value { public NullValue() : base(null) {} }
    }

    public class Record : Dictionary<string, Value> { }

    public class InsertCommand { public string Entity { get; set; } public Record Values { get; set; } }
    public class UpdateCommand { public string Entity { get; set; } public Value Id { get; set; } public Record Values { get; set; } }
    public class DeleteCommand { public string Entity { get; set; } public Value Id { get; set; } }
    public class MutationRequest { public object Command { get; set; } public string Comment { get; set; } }

    public class FilterExpression
    {
        public string Operator { get; set; }
        public string Field { get; set; }
        public object Expected { get; set; }
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

    public class Expr
    {
        public static object Eq(string field, object value) { return new FilterExpression { Operator = "eq", Field = field, Expected = value }; }
        public static object Contain(string field, object value) { return new FilterExpression { Operator = "contain", Field = field, Expected = value }; }
        public static object In(string field, object value) { return new FilterExpression { Operator = "in", Field = field, Expected = value }; }
        public static object Gte(string field, object value) { return new FilterExpression { Operator = "gte", Field = field, Expected = value }; }
        public static object Lte(string field, object value) { return new FilterExpression { Operator = "lte", Field = field, Expected = value }; }
    }

    public class SelectQuery
    {
        public string Entity { get; }
        public int? LimitValue { get; private set; }
        public int OffsetValue { get; private set; }
        public string PartitionBy { get; set; }
        public List<FilterExpression> Filters { get; } = new List<FilterExpression>();
        public List<(string Field, string Direction)> Orders { get; } = new List<(string, string)>();
        public List<string> GroupFields { get; } = new List<string>();
        public List<(string Function, string Field, string Alias)> Aggregates { get; } = new List<(string, string, string)>();
        public List<RelationQueryLoad> Relations { get; } = new List<RelationQueryLoad>();
        public SelectQuery(string entity) { Entity = entity; }
        public void Comment(string c) { }
        public void Purpose(string p) { }
        public void Limit(int n) { LimitValue = n; }
        public void Offset(int n) { OffsetValue = n; }
        public void OrderBy(string f, string d) { Orders.Add((f, d)); }
        public void GroupBy(string f) { GroupFields.Add(f); }
        public void Aggregate(string func, string field, string retName) { Aggregates.Add((func, field, retName)); }
        public void AndFilter(object expr) { if (expr is FilterExpression filter) Filters.Add(filter); }
        public void RelationQuery(string name, string targetEntity, string foreignKey, bool many, SelectQuery query)
        {
            Relations.Add(new RelationQueryLoad { Name = name, TargetEntity = targetEntity, LocalKey = "id", ForeignKey = foreignKey, Many = many, Query = query });
        }
    }

    public class QueryRequest { public SelectQuery Query { get; } public QueryRequest(SelectQuery q) { Query = q; } }
    public class QueryResult { public List<Record> Rows { get; set; } = new List<Record>(); }
    public class MutationResult { public bool Success { get; set; } public long Id { get; set; } public long Version { get; set; } public bool Deleted { get; set; } }
    public class AuditFieldChange { public string Field { get; set; } public object OldValue { get; set; } public object NewValue { get; set; } }
    public class RawAuditEvent { public string Kind { get; set; } public string Entity { get; set; } public long Id { get; set; } public string Reason { get; set; } public IReadOnlyList<AuditFieldChange> Changes { get; set; } }
    public class SafeAuditField { public string Field { get; set; } public string Value { get; set; } public bool Masked { get; set; } public bool Truncated { get; set; } }
    public class SafeAuditEvent { public string Kind { get; set; } public string Entity { get; set; } public long Id { get; set; } public string Reason { get; set; } public IReadOnlyList<SafeAuditField> Fields { get; set; } }
    public interface IRawAuditEventSink { Task OnEventAsync(UserContext ctx, RawAuditEvent auditEvent); }
    public interface IAppAuditEventSink { Task OnSafeEventAsync(UserContext ctx, SafeAuditEvent auditEvent); }
    public class InMemoryRawAuditEventSink : IRawAuditEventSink
    {
        public List<RawAuditEvent> Events { get; } = new();
        public Task OnEventAsync(UserContext ctx, RawAuditEvent auditEvent) { Events.Add(auditEvent); return Task.CompletedTask; }
    }
    public class InMemoryAppAuditEventSink : IAppAuditEventSink
    {
        public List<SafeAuditEvent> Events { get; } = new();
        public Task OnSafeEventAsync(UserContext ctx, SafeAuditEvent auditEvent) { Events.Add(auditEvent); return Task.CompletedTask; }
    }
    public class UserContext
    {
        public IDataService DataService { get; set; }
        public List<string> SqlTrace { get; } = new List<string>();
        private IRawAuditEventSink _standardAuditSink;
        private IAppAuditEventSink _appAuditSink;
        private readonly Dictionary<string, HashSet<string>> _auditMaskFields = new();
        private readonly Dictionary<string, int> _auditValueMaxLengths = new();

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
        Task<QueryResult> QueryAsync(UserContext ctx, QueryRequest req);
        Task<object> MutateAsync(UserContext ctx, MutationRequest req);
    }

    public class JsonFileDataService : IDataService
    {
        private readonly string _path;
        private readonly object _gate = new object();
        private StoreState _state;

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

        public async Task<object> MutateAsync(UserContext ctx, MutationRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Comment))
                throw new InvalidOperationException("Security audit failure: mutation audit reason is required");
            MutationResult result;
            lock (_gate)
            {
                result = req.Command is InsertCommand insert ? Insert(insert)
                    : req.Command is UpdateCommand update ? Update(update)
                    : req.Command is DeleteCommand delete ? Delete(delete)
                    : throw new InvalidOperationException("Unsupported mutation command");
            }
            await ctx.EmitMutationAuditAsync(req.Command, result, req.Comment);
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
            Persist();
            return new MutationResult { Success = true, Id = id, Version = 1 };
        }

        private MutationResult Update(UpdateCommand command)
        {
            var id = Convert.ToInt64(command.Id.Raw);
            var table = Table(command.Entity);
            if (!table.TryGetValue(id.ToString(), out var record)) throw new KeyNotFoundException($"{command.Entity}({id}) does not exist");
            foreach (var pair in ToStoredRecord(command.Values)) record[pair.Key] = pair.Value;
            var version = record.TryGetValue("version", out var current) ? current.GetInt64() + 1 : 1;
            record["version"] = JsonSerializer.SerializeToElement(version);
            Persist();
            return new MutationResult { Success = true, Id = id, Version = version };
        }

        private MutationResult Delete(DeleteCommand command)
        {
            var id = Convert.ToInt64(command.Id.Raw);
            if (!Table(command.Entity).Remove(id.ToString())) throw new KeyNotFoundException($"{command.Entity}({id}) does not exist");
            Persist();
            return new MutationResult { Success = true, Id = id, Deleted = true };
        }

        public Task<QueryResult> QueryAsync(UserContext ctx, QueryRequest req)
        {
            lock (_gate)
            {
                IEnumerable<Dictionary<string, JsonElement>> rows = Table(req.Query.Entity).Values;
                foreach (var filter in req.Query.Filters)
                {
                    rows = rows.Where(row => Matches(row, filter));
                }
                foreach (var order in req.Query.Orders.AsEnumerable().Reverse())
                {
                    rows = order.Direction.Equals("desc", StringComparison.OrdinalIgnoreCase)
                        ? rows.OrderByDescending(row => SortValue(row, order.Field))
                        : rows.OrderBy(row => SortValue(row, order.Field));
                }
                rows = rows.Skip(req.Query.OffsetValue);
                if (req.Query.LimitValue.HasValue) rows = rows.Take(req.Query.LimitValue.Value);
                return Task.FromResult(new QueryResult { Rows = rows.Select(ToRecord).ToList() });
            }
        }

        private bool Matches(Dictionary<string, JsonElement> row, FilterExpression filter)
        {
            if (!row.TryGetValue(filter.Field, out var actual)) return false;
            var expected = filter.Expected is Value value ? value.Raw : filter.Expected;
            if (filter.Operator == "contain") return actual.ToString().Contains(expected?.ToString() ?? "", StringComparison.Ordinal);
            return actual.ToString() == expected?.ToString();
        }

        private static string SortValue(Dictionary<string, JsonElement> row, string field) => row.TryGetValue(field, out var value) ? value.ToString() : "";
        private Dictionary<string, Dictionary<string, JsonElement>> Table(string entity) => _state.Data.TryGetValue(entity, out var table) ? table : _state.Data[entity] = new();
        private long NextId(string entity) { var id = _state.NextIds.TryGetValue(entity, out var value) ? value : 1; _state.NextIds[entity] = id + 1; return id; }
        private static Dictionary<string, JsonElement> ToStoredRecord(Record record) => record.ToDictionary(pair => pair.Key, pair => JsonSerializer.SerializeToElement(pair.Value.Raw));
        private static Record ToRecord(Dictionary<string, JsonElement> stored) { var result = new Record(); foreach (var pair in stored) result[pair.Key] = FromElement(pair.Value); return result; }
        private static Value FromElement(JsonElement value) => value.ValueKind switch { JsonValueKind.Null => new Value.NullValue(), JsonValueKind.Number when value.TryGetInt64(out var number) => new Value.I64Value(number), JsonValueKind.Number => new Value.F64Value(value.GetDouble()), JsonValueKind.True => new Value.BoolValue(true), JsonValueKind.False => new Value.BoolValue(false), _ => new Value.TextValue(value.ToString()) };
        private void Persist() { var directory = Path.GetDirectoryName(Path.GetFullPath(_path)); if (directory != null) Directory.CreateDirectory(directory); var temporary = _path + ".tmp"; File.WriteAllText(temporary, JsonSerializer.Serialize(_state)); File.Move(temporary, _path, true); }
    }

    public abstract class AdoNetSqlDataService : IDataService, IAsyncDisposable
    {
        protected abstract DbConnection CreateConnection();
        protected abstract string QuoteSafeIdentifier(string identifier);
        protected abstract string ContainsSql(string column, string parameter);
        protected abstract string ApplyPagination(string statement, SelectQuery query, bool hasOrder);
        protected abstract string CreateTableSql(string table);
        protected abstract string ColumnExistsSql(string table, string column);
        protected abstract string AddColumnSql(string table, string column, string type);
        protected abstract string CreateIdSpaceSql { get; }
        protected abstract string NextIdSql { get; }
        protected abstract string ColumnType(Value value);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public async Task<object> MutateAsync(UserContext ctx, MutationRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Comment))
                throw new InvalidOperationException("Security audit failure: mutation audit reason is required");
            var result = req.Command is InsertCommand insert ? await InsertAsync(insert)
                : req.Command is UpdateCommand update ? await UpdateAsync(update)
                : req.Command is DeleteCommand delete ? await DeleteAsync(delete)
                : throw new InvalidOperationException("Unsupported mutation command");
            await ctx.EmitMutationAuditAsync(req.Command, result, req.Comment);
            return result;
        }

        private async Task<MutationResult> InsertAsync(InsertCommand command)
        {
            var values = new Record();
            foreach (var pair in command.Values) values[pair.Key] = pair.Value;
            var id = values.TryGetValue("id", out var idValue)
                ? Convert.ToInt64(idValue.Raw)
                : await NextIdAsync(command.Entity);
            values["id"] = new Value.I64Value(id);
            values["version"] = new Value.I64Value(1);
            await EnsureStorageAsync(command.Entity, values);

            var fields = values.Keys.ToList();
            await using var connection = await OpenConnectionAsync();
            await using var sql = connection.CreateCommand();
            sql.CommandText = $"INSERT INTO {Quote(TableName(command.Entity))} ({string.Join(", ", fields.Select(Quote))}) VALUES ({string.Join(", ", fields.Select((_, index) => "@p" + index))})";
            for (var index = 0; index != fields.Count; index++)
            {
                AddParameter(sql, "p" + index, ToDbValue(values[fields[index]]));
            }
            var affected = await sql.ExecuteNonQueryAsync();
            if (affected != 1) throw new InvalidOperationException($"Insert affected {affected} rows");
            return new MutationResult { Success = true, Id = id, Version = 1 };
        }

        private async Task<MutationResult> UpdateAsync(UpdateCommand command)
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
            await using var connection = await OpenConnectionAsync();
            await using var sql = connection.CreateCommand();
            sql.CommandText = $"UPDATE {Quote(TableName(command.Entity))} SET {string.Join(", ", assignments)} WHERE {Quote("id")} = @id AND {Quote("version")} = @oldVersion";
            for (var index = 0; index != fields.Count; index++)
            {
                AddParameter(sql, "p" + index, ToDbValue(values[fields[index]]));
            }
            AddParameter(sql, "nextVersion", nextVersion);
            AddParameter(sql, "id", id);
            AddParameter(sql, "oldVersion", oldVersion);
            var affected = await sql.ExecuteNonQueryAsync();
            if (affected != 1) throw new InvalidOperationException($"Optimistic lock failed for {command.Entity}({id}) at version {oldVersion}");
            return new MutationResult { Success = true, Id = id, Version = nextVersion };
        }

        private async Task<MutationResult> DeleteAsync(DeleteCommand command)
        {
            var id = Convert.ToInt64(command.Id.Raw);
            await using var connection = await OpenConnectionAsync();
            await using var sql = connection.CreateCommand();
            sql.CommandText = $"DELETE FROM {Quote(TableName(command.Entity))} WHERE {Quote("id")} = @id";
            AddParameter(sql, "id", id);
            var affected = await sql.ExecuteNonQueryAsync();
            if (affected != 1) throw new KeyNotFoundException($"{command.Entity}({id}) does not exist");
            return new MutationResult { Success = true, Id = id, Deleted = true };
        }

        public async Task<QueryResult> QueryAsync(UserContext ctx, QueryRequest req)
        {
            var query = req.Query;
            var where = new List<string>();
            await using var connection = await OpenConnectionAsync();
            await using var sql = connection.CreateCommand();
            foreach (var filter in query.Filters)
            {
                if (filter.Operator == "in")
                {
                    var placeholders = new List<string>();
                    foreach (var expected in (IEnumerable<object>)filter.Expected)
                    {
                        var parameter = "p" + sql.Parameters.Count;
                        placeholders.Add("@" + parameter);
                        AddParameter(sql, parameter, expected ?? DBNull.Value);
                    }
                    where.Add(placeholders.Count == 0 ? "1 = 0" : $"{Quote(filter.Field)} IN ({string.Join(", ", placeholders)})");
                }
                else
                {
                    var parameter = "p" + sql.Parameters.Count;
                    where.Add(filter.Operator switch
                    {
                        "contain" => ContainsSql(Quote(filter.Field), parameter),
                        "gte" => $"{Quote(filter.Field)} >= @{parameter}",
                        "lte" => $"{Quote(filter.Field)} <= @{parameter}",
                        _ => $"{Quote(filter.Field)} = @{parameter}"
                    });
                    var expected = filter.Expected is Value value ? value.Raw : filter.Expected;
                    AddParameter(sql, parameter, expected ?? DBNull.Value);
                }
            }
            var projection = "*";
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
            ctx.SqlTrace.Add(sql.CommandText);

            var result = new QueryResult();
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
            await EnhanceRelationsAsync(ctx, query, result.Rows);
            return result;
        }

        private async Task EnhanceRelationsAsync(UserContext ctx, SelectQuery query, List<Record> parents)
        {
            if (parents.Count == 0 || query.Relations.Count == 0) return;
            foreach (var load in query.Relations)
            {
                var parentIds = parents
                    .Where(parent => parent.ContainsKey(load.LocalKey))
                    .Select(parent => parent[load.LocalKey].Raw)
                    .Cast<object>().ToList();
                var child = CloneQuery(load.Query, load.TargetEntity);
                child.Filters.Add(new FilterExpression { Operator = "in", Field = load.ForeignKey, Expected = parentIds });
                if (child.LimitValue.HasValue) child.PartitionBy = load.ForeignKey;
                var children = (await QueryAsync(ctx, new QueryRequest(child))).Rows;
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

        private static SelectQuery CloneQuery(SelectQuery source, string entity)
        {
            var clone = new SelectQuery(entity) { PartitionBy = source.PartitionBy };
            if (source.LimitValue.HasValue) clone.Limit(source.LimitValue.Value);
            clone.Offset(source.OffsetValue);
            foreach (var filter in source.Filters) clone.Filters.Add(filter);
            foreach (var order in source.Orders) clone.Orders.Add(order);
            foreach (var group in source.GroupFields) clone.GroupFields.Add(group);
            foreach (var aggregate in source.Aggregates) clone.Aggregates.Add(aggregate);
            foreach (var relation in source.Relations) clone.Relations.Add(relation);
            return clone;
        }

        protected virtual async Task EnsureStorageAsync(string entity, Record values)
        {
            var table = TableName(entity);
            await using var connection = await OpenConnectionAsync();
            await using (var create = connection.CreateCommand())
            {
                create.CommandText = CreateTableSql(table);
                await create.ExecuteNonQueryAsync();
            }
            foreach (var pair in values)
            {
                if (pair.Key == "id" || pair.Key == "version") continue;
                await using var checkColumn = connection.CreateCommand();
                checkColumn.CommandText = ColumnExistsSql(table, pair.Key);
                if (await checkColumn.ExecuteScalarAsync() != null) continue;
                await using var addColumn = connection.CreateCommand();
                addColumn.CommandText = AddColumnSql(table, pair.Key, ColumnType(pair.Value));
                await addColumn.ExecuteNonQueryAsync();
            }
        }

        protected virtual async Task<long> NextIdAsync(string entity)
        {
            await using var connection = await OpenConnectionAsync();
            await using (var create = connection.CreateCommand())
            {
                create.CommandText = CreateIdSpaceSql;
                await create.ExecuteNonQueryAsync();
            }
            await using var sql = connection.CreateCommand();
            sql.CommandText = NextIdSql;
            AddParameter(sql, "entity", entity);
            return Convert.ToInt64(await sql.ExecuteScalarAsync());
        }

        private async Task<long> CurrentVersionAsync(string entity, long id)
        {
            await using var connection = await OpenConnectionAsync();
            await using var sql = connection.CreateCommand();
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

        protected virtual object NormalizeParameterValue(object value) => value;

        private static object ToDbValue(Value value) => value.Raw switch
        {
            DateTime dateTime => dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime.ToUniversalTime(),
            _ => value.Raw ?? DBNull.Value
        };

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

        public SqliteDataService(string connectionString) { _connectionString = connectionString; }
        protected override object NormalizeParameterValue(object value) => value is DateTime dateTime
            ? dateTime.ToString(dateTime.TimeOfDay == TimeSpan.Zero ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture)
            : value;
        protected override DbConnection CreateConnection() => new SqliteConnection(_connectionString);
        protected override string QuoteSafeIdentifier(string identifier) => $"\"{identifier}\"";
        protected override string ContainsSql(string column, string parameter) => $"CAST({column} AS TEXT) LIKE '%' || @{parameter} || '%'";
        protected override string ApplyPagination(string statement, SelectQuery query, bool hasOrder)
        {
            if (query.LimitValue.HasValue) statement += " LIMIT " + query.LimitValue.Value;
            if (query.OffsetValue != 0) statement += " OFFSET " + query.OffsetValue;
            return statement;
        }
        protected override string CreateTableSql(string table) => $"CREATE TABLE IF NOT EXISTS {Quote(table)} ({Quote("id")} INTEGER PRIMARY KEY, {Quote("version")} INTEGER NOT NULL)";
        protected override string ColumnExistsSql(string table, string column) => $"SELECT 1 FROM pragma_table_info('{table}') WHERE name = '{column}'";
        protected override string AddColumnSql(string table, string column, string type) => $"ALTER TABLE {Quote(table)} ADD COLUMN {Quote(column)} {type}";
        protected override string CreateIdSpaceSql => "CREATE TABLE IF NOT EXISTS teaql_id_space (entity TEXT PRIMARY KEY, next_id INTEGER NOT NULL)";
        protected override string NextIdSql => "INSERT INTO teaql_id_space(entity, next_id) VALUES (@entity, 1000) ON CONFLICT(entity) DO UPDATE SET next_id = teaql_id_space.next_id + 1 RETURNING next_id";
        protected override string ColumnType(Value value) => value switch
        {
            Value.BoolValue => "INTEGER",
            Value.I64Value => "INTEGER",
            Value.F64Value => "REAL",
            Value.DecimalValue => "NUMERIC",
            Value.DateValue => "TEXT",
            Value.DateTimeValue => "TEXT",
            Value.JsonValue => "TEXT",
            _ => "TEXT"
        };
    }
}