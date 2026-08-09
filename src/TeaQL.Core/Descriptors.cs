using System.Collections.Generic;

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
}

public record RelationDescriptor(
    string Name,
    string TargetEntity,
    string LocalKey = "id",
    string ForeignKey = "id",
    bool Many = false,
    bool Attach = true,
    bool DeleteMissing = true
);

public record EntityDescriptor
{
    public string Name { get; init; } = "";
    public string TableName { get; init; } = "";
    public string? DataService { get; init; }
    public List<PropertyDescriptor> Properties { get; init; } = new();
    public List<RelationDescriptor> Relations { get; init; } = new();
    public List<string> AuditMaskFields { get; init; } = new();
    public int? AuditValueMaxLen { get; init; }
}
