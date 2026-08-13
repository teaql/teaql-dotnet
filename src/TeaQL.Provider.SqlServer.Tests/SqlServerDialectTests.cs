using System;
using TeaQL.Core;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.SqlServer.Tests;

public class SqlServerDialectTests
{
    [Fact]
    public void Kind_ReturnsSqlServer()
    {
        var dialect = new SqlServerDialect();
        Assert.Equal(DatabaseKind.SqlServer, dialect.Kind);
    }

    [Fact]
    public void QuoteIdent_AddsBrackets_WhenNeeded()
    {
        var dialect = new SqlServerDialect();
        
        Assert.Equal("[order]", dialect.QuoteIdent("order"));
        Assert.Equal("[user name]", dialect.QuoteIdent("user name"));
        Assert.Equal("id", dialect.QuoteIdent("id"));
        Assert.Equal("[bracket]", dialect.QuoteIdent("[bracket]"));
    }

    [Fact]
    public void Placeholder_ReturnsPIndex()
    {
        var dialect = new SqlServerDialect();
        
        Assert.Equal("@p1", dialect.Placeholder(1));
        Assert.Equal("@p2", dialect.Placeholder(2));
    }
}
