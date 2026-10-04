using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TeaQL.Core;

namespace TeaQL.Sql;

public enum DatabaseKind
{
    PostgreSql,
    Sqlite,
    MySql,
    SqlServer
}

public class CompiledQuery
{
    public string Sql { get; }
    public List<Value> Params { get; }
    public string? Comment { get; }
    public IReadOnlyList<SqlParameterLogPolicy> ParameterLogPolicies { get; }
    public bool GeneratedSql { get; }
    internal IReadOnlyList<(Value Value, SqlParameterLogPolicy Policy)> IntentOperands { get; }

    public CompiledQuery(string sql, List<Value> @params, string? comment = null)
    {
        Sql = sql;
        Params = @params;
        Comment = comment;
        ParameterLogPolicies = SqlLogBindings.Policies(@params);
        GeneratedSql = SqlLogBindings.IsGenerated(@params);
        IntentOperands = SqlLogBindings.IntentOperands(@params);
    }

    public string SqlWithComment()
    {
        if (!string.IsNullOrEmpty(Comment))
        {
            var escaped = Comment.Replace("*/", "* /");
            return $"/* {escaped} */ {Sql}";
        }
        return Sql;
    }

    public string DebugSql(DatabaseKind kind) =>
        SqlLogRenderer.Render(SqlWithComment(), Params.Count,
            index => SqlLogRenderer.Literal(Params[index], kind.ToString()), kind.ToString());
}

public class SqlCompileException : Exception
{
    public SqlCompileException(string message) : base(message) { }
    public SqlCompileException(string message, Exception innerException) : base(message, innerException) { }

    public static SqlCompileException UnknownEntity(string entity) => new($"unknown entity: {entity}");
    public static SqlCompileException UnknownField(string field) => new($"unknown field: {field}");
    public static SqlCompileException EmptyInList() => new("IN requires at least one value");
    public static SqlCompileException MissingIdProperty(string entity) => new($"entity {entity} has no id property");
    public static SqlCompileException MissingVersionProperty(string entity) => new($"entity {entity} has no version property");
    public static SqlCompileException EmptyMutation(string kind) => new($"{kind} requires at least one writable field");
    public static SqlCompileException InvalidRecoverVersion(long version) => new($"recover requires a negative version, got {version}");
    public static SqlCompileException UnsupportedSchemaType(DataType dataType) => new($"unsupported schema type: {dataType}");
    public static SqlCompileException InvalidFunctionArguments(string message) => new(message);
    public static SqlCompileException InvalidSubQueryOperator(string op) => new($"subquery does not support operator: {op}");
}

public interface ISchemaProvider
{
    EntityDescriptor? GetEntity(string name);
}

/// <summary>Provider-specific idempotent index installation for dialects without CREATE INDEX IF NOT EXISTS.</summary>
public interface ISchemaIndexInstaller
{
    Task EnsureSchemaIndexesAsync(SqlDialect dialect, EntityDescriptor entity);
}
