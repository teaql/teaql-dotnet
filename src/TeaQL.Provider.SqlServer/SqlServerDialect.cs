using System;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.SqlServer;

public class SqlServerDialect : SqlDialect
{
    public override DatabaseKind Kind => DatabaseKind.SqlServer;

    public override string QuoteIdent(string ident)
    {
        return DialectUtils.QuoteIdentifierIfNeeded(ident, '[', ']');
    }

    public override string Placeholder(int index)
    {
        return $"@p{index}";
    }
}
