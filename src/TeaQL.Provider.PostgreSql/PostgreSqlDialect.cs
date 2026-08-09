using System;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.PostgreSql;

public class PostgreSqlDialect : SqlDialect
{
    public override DatabaseKind Kind => DatabaseKind.PostgreSql;

    public override string QuoteIdent(string ident)
    {
        return DialectUtils.QuoteIdentifierIfNeeded(ident, '"');
    }

    public override string Placeholder(int index)
    {
        return $"${index}";
    }
}
