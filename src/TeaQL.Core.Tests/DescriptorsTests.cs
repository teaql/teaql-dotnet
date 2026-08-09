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
            .WithColumnName("user_name")
            .NotNull()
            .Id()
            .Version();

        Assert.Equal("username", prop.Name);
        Assert.Equal("user_name", prop.ColumnName);
        Assert.Equal(DataType.Text, prop.DataType);
        Assert.False(prop.Nullable);
        Assert.True(prop.IsId);
        Assert.True(prop.IsVersion);
    }

    [Fact]
    public void TestRelationDescriptorBuilder()
    {
        var rel = TeaQL.Core.RelationDescriptor.New("orders", "Order")
            .WithLocalKey("user_id")
            .WithForeignKey("customer_id")
            .IsMany()
            .Detached()
            .KeepMissing();

        Assert.Equal("orders", rel.Name);
        Assert.Equal("Order", rel.TargetEntity);
        Assert.Equal("user_id", rel.LocalKey);
        Assert.Equal("customer_id", rel.ForeignKey);
        Assert.True(rel.Many);
        Assert.False(rel.Attach);
        Assert.False(rel.DeleteMissing);
    }

    [Fact]
    public void TestEntityDescriptorBuilderAndLookups()
    {
        var entity = TeaQL.Core.EntityDescriptor.New("User")
            .WithTableName("users")
            .WithDataService("auth_db")
            .WithAuditMaskFields(new List<string> { "password" })
            .WithAuditValueMaxLen(255);

        var idProp = TeaQL.Core.PropertyDescriptor.New("id", DataType.I64).Id();
        var nameProp = TeaQL.Core.PropertyDescriptor.New("name", DataType.Text);
        var versionProp = TeaQL.Core.PropertyDescriptor.New("version", DataType.I64).Version();

        var ordersRel = TeaQL.Core.RelationDescriptor.New("orders", "Order");

        entity.WithProperty(idProp)
              .WithProperty(nameProp)
              .WithProperty(versionProp)
              .WithRelation(ordersRel);

        Assert.Equal("User", entity.Name);
        Assert.Equal("users", entity.TableName);
        Assert.Equal("auth_db", entity.DataService);
        Assert.Equal(new List<string> { "password" }, entity.AuditMaskFields);
        Assert.Equal(255, entity.AuditValueMaxLen);

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
