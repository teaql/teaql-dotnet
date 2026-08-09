using System.Collections.Generic;
using System.Linq;

namespace TeaQL.Core;

public record PropertyDescriptor(
    string Name,
    DataType DataType,
    bool Nullable = true,
    string ColumnName = "",
    bool IsId = false,
    bool IsVersion = false
)
{
    public string ColumnName { get; init; } = string.IsNullOrEmpty(ColumnName) ? Name : ColumnName;

    public static PropertyDescriptor New(string name, DataType dataType)
    {
        return new PropertyDescriptor(name, dataType);
    }

    public PropertyDescriptor WithColumnName(string columnName) => this with { ColumnName = columnName };
    public PropertyDescriptor NotNull() => this with { Nullable = false };
    public PropertyDescriptor Id() => this with { IsId = true };
    public PropertyDescriptor Version() => this with { IsVersion = true };
}

public record RelationDescriptor(
    string Name,
    string TargetEntity,
    string LocalKey = "id",
    string ForeignKey = "id",
    bool Many = false,
    bool Attach = true,
    bool DeleteMissing = true
)
{
    public static RelationDescriptor New(string name, string targetEntity)
    {
        return new RelationDescriptor(name, targetEntity);
    }

    public RelationDescriptor WithLocalKey(string key) => this with { LocalKey = key };
    public RelationDescriptor WithForeignKey(string key) => this with { ForeignKey = key };
    public RelationDescriptor IsMany() => this with { Many = true };
    public RelationDescriptor IsAttach() => this with { Attach = true };
    public RelationDescriptor Detached() => this with { Attach = false };
    public RelationDescriptor DoDeleteMissing() => this with { DeleteMissing = true };
    public RelationDescriptor KeepMissing() => this with { DeleteMissing = false };
}

public record EntityDescriptor
{
    public string Name { get; init; } = "";
    public string TableName { get; init; } = "";
    public string? DataService { get; init; }
    public List<PropertyDescriptor> Properties { get; init; } = new();
    public List<RelationDescriptor> Relations { get; init; } = new();
    public List<string> AuditMaskFields { get; init; } = new();
    public int? AuditValueMaxLen { get; init; }

    public static EntityDescriptor New(string name)
    {
        return new EntityDescriptor
        {
            Name = name,
            TableName = Naming.DefaultTableName(name)
        };
    }

    public EntityDescriptor WithTableName(string tableName) => this with { TableName = tableName };
    public EntityDescriptor WithDataService(string dataService) => this with { DataService = dataService };
    public EntityDescriptor WithProperty(PropertyDescriptor property)
    {
        Properties.Add(property);
        return this;
    }
    public EntityDescriptor WithRelation(RelationDescriptor relation)
    {
        Relations.Add(relation);
        return this;
    }
    public EntityDescriptor WithAuditMaskFields(List<string> fields) => this with { AuditMaskFields = fields };
    public EntityDescriptor WithAuditValueMaxLen(int? maxLen) => this with { AuditValueMaxLen = maxLen };

    public PropertyDescriptor? PropertyByName(string name) => Properties.FirstOrDefault(p => p.Name == name);
    public RelationDescriptor? RelationByName(string name) => Relations.FirstOrDefault(r => r.Name == name);
    public PropertyDescriptor? IdProperty() => Properties.FirstOrDefault(p => p.IsId);
    public PropertyDescriptor? VersionProperty() => Properties.FirstOrDefault(p => p.IsVersion);
    public IEnumerable<PropertyDescriptor> WritableProperties() => Properties.Where(p => !p.IsId);
}
