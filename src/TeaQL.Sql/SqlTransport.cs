using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TeaQL.Core;

namespace TeaQL.Sql;

public interface ISqlTransport
{
    Task<List<Record>> FetchAllSqlAsync(CompiledQuery query);
    Task<ulong> ExecuteSqlAsync(CompiledQuery query);
}

public interface ISchemaConnectionInitializer
{
    Task EnsureSchemaFunctionsAsync();
}

public interface IStreamingSqlTransport : ISqlTransport
{
    IAsyncEnumerable<Record> StreamSqlAsync(CompiledQuery query, CancellationToken cancellationToken = default);
}

public interface ISqlTransaction : ISqlTransport, IDisposable
{
    Task CommitSqlAsync();
    Task RollbackSqlAsync();
}

public interface ISqlTransactionTransport : ISqlTransport
{
    Task<ISqlTransaction> BeginSqlAsync();
}

/// Providers implementing this marker run each public mutation and authoritative refresh atomically.
public interface IAutomaticMutationTransactionTransport : ISqlTransactionTransport { }
