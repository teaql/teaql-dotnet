using TeaQL.Core;

namespace TeaQL.DataService;

/// <summary>A graph-owned SQL lease for optional modules; it cannot commit or roll back.</summary>
public interface IPhysicalSqlLease
{
    Task<List<Record>> FetchAllAsync(string sql, IReadOnlyList<Value> parameters);
    Task<ulong> ExecuteAsync(string sql, IReadOnlyList<Value> parameters);
}

/// <summary>Implemented only by data-service transactions with a physical SQL owner.</summary>
public interface IPhysicalSqlOwner
{
    Task<T> WithPhysicalSqlAsync<T>(Func<IPhysicalSqlLease, Task<T>> work);
}
