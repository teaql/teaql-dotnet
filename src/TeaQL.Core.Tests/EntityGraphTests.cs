using System.Collections.Generic;
using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class EntityGraphTests
{
    private class DummyEntity : IEntity
    {
        public Record Record { get; set; } = new();

        public static string EntityName => "Dummy";
        public static EntityDescriptor EntityDescriptor()
        {
            return TeaQL.Core.EntityDescriptor.New("Dummy")
                .WithProperty(TeaQL.Core.PropertyDescriptor.New("id", DataType.I64).Id());
        }

        public Record IntoRecord() => Record;
    }

    [Fact]
    public void TestEntityGraphBuilderAnnotationsAndChildOperations()
    {
        var entity1 = new DummyEntity { Record = new Record { ["id"] = new Value.I64Value(1) } };
        var entity2 = new DummyEntity { Record = new Record { ["id"] = new Value.I64Value(2) } };

        var graph = EntityGraph.New(entity1)
            .Comment("Parent creation")
            .Child("dummy_items", EntityGraph.New(entity2).Comment("Child deletion").Delete())
            .Build();

        var root = graph.Root;
        Assert.Equal("Dummy", root.EntityType);
        Assert.Equal("Parent creation", root.Comment);
        Assert.Equal(EntityGraphOperation.Save, root.Operation);
        Assert.Single(root.Children);

        var (relName, childNode) = root.Children[0];
        Assert.Equal("dummy_items", relName);
        Assert.Equal("Dummy", childNode.EntityType);
        Assert.Equal("Child deletion", childNode.Comment);
        Assert.Equal(EntityGraphOperation.Delete, childNode.Operation);
    }
}
