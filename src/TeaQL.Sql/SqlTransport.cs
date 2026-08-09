using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeaQL.Core;

namespace TeaQL.Sql;

public interface ISqlTransport
{
    Task<List<Record>> FetchAllSqlAsync(CompiledQuery query);
    Task<ulong> ExecuteSqlAsync(CompiledQuery query);
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
