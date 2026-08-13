using System;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.Sqlite;

public class SqliteDialect : SqlDialect
{
    public override DatabaseKind Kind => DatabaseKind.Sqlite;

    public override string QuoteIdent(string ident)
    {
        return DialectUtils.QuoteIdentifierIfNeeded(ident, '"');
    }

    public override string Placeholder(int index)
    {
        return $"@p{index - 1}";
    }

    public override string SchemaTypeSql(DataType dataType, PropertyDescriptor property)
    {
        return dataType switch
        {
            DataType.Bool => "INTEGER",
            DataType.I64 or DataType.U64 when property.IsId => "INTEGER",
            DataType.I64 or DataType.U64 => "INTEGER",
            DataType.F64 => "REAL",
            DataType.Decimal => "NUMERIC",
            DataType.Text => "VARCHAR(255)",
            DataType.LargeText => "TEXT",
            DataType.Json => "JSON",
            DataType.Date => "DATE",
            DataType.Timestamp => "TIMESTAMP",
            _ => throw SqlCompileException.UnsupportedSchemaType(dataType)
        };
    }

    public override string CompileAddColumn(EntityDescriptor entity, PropertyDescriptor property)
    {
        var def = ColumnDefinitionSql(property);
        var defWithoutNotNull = def.Replace(" NOT NULL", "");
        return $"ALTER TABLE {QuoteIdent(entity.TableNameValue)} ADD COLUMN {defWithoutNotNull}";
    }
}
