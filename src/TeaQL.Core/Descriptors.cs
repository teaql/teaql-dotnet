using System.Collections.Generic;
using System.Linq;

namespace TeaQL.Core;

public record PropertyDescriptor(
    string Name,
    DataType DataType,
    bool Nullable = true,
    string ColumnNameValue = "",
    bool IsId = false,
    bool IsVersion = false
)
{
    public string ColumnNameString { get; init; } = string.IsNullOrEmpty(ColumnNameValue) ? Name : ColumnNameValue;

    public static PropertyDescriptor New(string name, DataType dataType)
    {
        return new PropertyDescriptor(name, dataType);
    }

    public PropertyDescriptor ColumnName(string columnName) => this with { ColumnNameString = columnName };
    public PropertyDescriptor NotNull() => this with { Nullable = false };
    public PropertyDescriptor Id() => this with { IsId = true };
    public PropertyDescriptor Version() => this with { IsVersion = true };
}

public record RelationDescriptor(
    string Name,
    string TargetEntity,
    string LocalKeyValue = "id",
    string ForeignKeyValue = "id",
    bool IsMany = false,
    bool IsAttach = true,
    bool IsDeleteMissing = true
)
{
    public static RelationDescriptor New(string name, string targetEntity)
    {
        return new RelationDescriptor(name, targetEntity);
    }

    public RelationDescriptor LocalKey(string key) => this with { LocalKeyValue = key };
    public RelationDescriptor ForeignKey(string key) => this with { ForeignKeyValue = key };
    public RelationDescriptor Many(bool many = true) => this with { IsMany = many };
    public RelationDescriptor Attach(bool attach = true) => this with { IsAttach = attach };
    public RelationDescriptor Detached() => this with { IsAttach = false };
    public RelationDescriptor DeleteMissing(bool deleteMissing = true) => this with { IsDeleteMissing = deleteMissing };
    public RelationDescriptor KeepMissing() => this with { IsDeleteMissing = false };
}

public record EntityDescriptor
{
    public string Name { get; init; } = "";
    public string TableNameValue { get; init; } = "";
    public string? DataServiceName { get; init; }
    public List<PropertyDescriptor> Properties { get; init; } = new();
    public List<RelationDescriptor> Relations { get; init; } = new();
    public List<string> AuditMaskFieldList { get; init; } = new();
    public int? AuditValueMaxLenValue { get; init; }

    public static EntityDescriptor New(string name)
    {
        return new EntityDescriptor
        {
            Name = name,
            TableNameValue = Naming.DefaultTableName(name)
        };
    }

    public EntityDescriptor TableName(string tableName) => this with { TableNameValue = tableName };
    public EntityDescriptor DataService(string dataService) => this with { DataServiceName = dataService };
    public EntityDescriptor Property(PropertyDescriptor property)
    {
        Properties.Add(property);
        return this;
    }
    public EntityDescriptor Relation(RelationDescriptor relation)
    {
        Relations.Add(relation);
        return this;
    }
    public EntityDescriptor AuditMaskFields(List<string> fields) => this with { AuditMaskFieldList = fields };
    public EntityDescriptor AuditValueMaxLen(int? maxLen) => this with { AuditValueMaxLenValue = maxLen };

    public PropertyDescriptor? PropertyByName(string name) => Properties.FirstOrDefault(p => p.Name == name);
    public RelationDescriptor? RelationByName(string name) => Relations.FirstOrDefault(r => r.Name == name);
    public PropertyDescriptor? IdProperty() => Properties.FirstOrDefault(p => p.IsId);
    public PropertyDescriptor? VersionProperty() => Properties.FirstOrDefault(p => p.IsVersion);
    public IEnumerable<PropertyDescriptor> WritableProperties() => Properties.Where(p => !p.IsId);
}
