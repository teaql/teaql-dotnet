using System.Collections.Generic;
using System.Linq;
using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class DescriptorsTests
{
    [Fact]
    public void TestPropertyDescriptorBuilder()
    {
        var prop = TeaQL.Core.PropertyDescriptor.New("username", DataType.Text)
            .ColumnName("user_name")
            .NotNull()
            .Id()
            .Version();

        Assert.Equal("username", prop.Name);
        Assert.Equal("user_name", prop.ColumnNameString);
        Assert.Equal(DataType.Text, prop.DataType);
        Assert.False(prop.Nullable);
        Assert.True(prop.IsId);
        Assert.True(prop.IsVersion);
    }

    [Fact]
    public void TestRelationDescriptorBuilder()
    {
        var rel = TeaQL.Core.RelationDescriptor.New("orders", "Order")
            .LocalKey("user_id")
            .ForeignKey("customer_id")
            .Many()
            .Detached()
            .KeepMissing();

        Assert.Equal("orders", rel.Name);
        Assert.Equal("Order", rel.TargetEntity);
        Assert.Equal("user_id", rel.LocalKeyValue);
        Assert.Equal("customer_id", rel.ForeignKeyValue);
        Assert.True(rel.IsMany);
        Assert.False(rel.IsAttach);
        Assert.False(rel.IsDeleteMissing);
    }

    [Fact]
    public void TestEntityDescriptorBuilderAndLookups()
    {
        var entity = TeaQL.Core.EntityDescriptor.New("User")
            .TableName("users")
            .DataService("auth_db")
            .AuditMaskFields(new List<string> { "password" })
            .AuditValueMaxLen(255);

        var idProp = TeaQL.Core.PropertyDescriptor.New("id", DataType.I64).Id();
        var nameProp = TeaQL.Core.PropertyDescriptor.New("name", DataType.Text);
        var versionProp = TeaQL.Core.PropertyDescriptor.New("version", DataType.I64).Version();

        var ordersRel = TeaQL.Core.RelationDescriptor.New("orders", "Order");

        entity.Property(idProp)
              .Property(nameProp)
              .Property(versionProp)
              .Relation(ordersRel);

        Assert.Equal("User", entity.Name);
        Assert.Equal("users", entity.TableNameValue);
        Assert.Equal("auth_db", entity.DataServiceName);
        Assert.Equal(new List<string> { "password" }, entity.AuditMaskFieldList);
        Assert.Equal(255, entity.AuditValueMaxLenValue);

        // Lookups
        Assert.Equal(nameProp, entity.PropertyByName("name"));
        Assert.Null(entity.PropertyByName("missing"));

        Assert.Equal(ordersRel, entity.RelationByName("orders"));
        Assert.Null(entity.RelationByName("missing"));

        Assert.Equal(idProp, entity.IdProperty());
        Assert.Equal(versionProp, entity.VersionProperty());

        var writable = entity.WritableProperties().ToList();
        Assert.Equal(2, writable.Count);
        Assert.Contains(nameProp, writable);
        Assert.Contains(versionProp, writable);
    }
}
