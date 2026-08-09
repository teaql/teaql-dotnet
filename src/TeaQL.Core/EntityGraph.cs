using System.Collections.Generic;

namespace TeaQL.Core;

public enum EntityGraphOperation
{
    Save,
    Delete
}

public class EntityGraphNode
{
    public string EntityType { get; set; } = "";
    public Record Record { get; set; } = new();
    public string? Comment { get; set; }
    public EntityGraphOperation Operation { get; set; } = EntityGraphOperation.Save;
    public List<(string Relation, EntityGraphNode Node)> Children { get; set; } = new();
}

public class EntityGraphBuilder
{
    internal EntityGraphNode Node { get; }

    internal EntityGraphBuilder(EntityGraphNode node)
    {
        Node = node;
    }

    public EntityGraphBuilder Comment(string comment)
    {
        Node.Comment = comment;
        return this;
    }

    public EntityGraphBuilder Delete()
    {
        Node.Operation = EntityGraphOperation.Delete;
        return this;
    }

    public EntityGraphBuilder Child(string relation, EntityGraphBuilder child)
    {
        Node.Children.Add((relation, child.Node));
        return this;
    }

    public EntityGraph Build()
    {
        return new EntityGraph(Node);
    }
}

public class EntityGraph
{
    public EntityGraphNode Root { get; }

    internal EntityGraph(EntityGraphNode root)
    {
        Root = root;
    }

    public static EntityGraphBuilder New<T>(T entity) where T : IEntity
    {
        var node = new EntityGraphNode
        {
            EntityType = T.EntityDescriptor().Name,
            Record = entity.IntoRecord(),
            Operation = EntityGraphOperation.Save
        };
        return new EntityGraphBuilder(node);
    }
}
