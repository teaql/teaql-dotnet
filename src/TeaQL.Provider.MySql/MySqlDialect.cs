using System;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.MySql;

public class MySqlDialect : SqlDialect
{
    public override DatabaseKind Kind => DatabaseKind.MySql;

    public override string QuoteIdent(string ident)
    {
        return $"`{ident.Replace("`", "``")}`";
    }

    public override string Placeholder(int index)
    {
        return "?";
    }

    public override string SchemaTypeSql(DataType dataType, PropertyDescriptor property)
    {
        return dataType switch
        {
            DataType.Bool => "BOOLEAN",
            DataType.I64 => "BIGINT",
            DataType.U64 => "BIGINT UNSIGNED",
            DataType.F64 => "DOUBLE",
            DataType.Decimal => "DECIMAL(38, 18)",
            DataType.Text => "VARCHAR(255)",
            DataType.LargeText => "LONGTEXT",
            DataType.Json => "JSON",
            DataType.Date => "DATE",
            DataType.Timestamp => "DATETIME(3)",
            _ => base.SchemaTypeSql(dataType, property)
        };
    }
}
