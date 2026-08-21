using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class EntityRootTests
{
    [Fact]
    public void TracksFinalValuesVersionsAndLifecycle()
    {
        var root = new EntityRoot();
        var order = new EntityKey("Order", new Value.I64Value(10));
        var line = new EntityKey("OrderLine", new Value.I64Value(20));
        root.SetOriginalVersion(order, 3);
        root.Set(order, "status", new Value.TextValue("pending"));
        root.Set(order, "status", new Value.TextValue("confirmed"));
        root.Set(line, "quantity", new Value.I64Value(2));
        root.MarkAsNew(line);

        Assert.Equal("confirmed", ((Value.TextValue)root.Changes()[order]["status"]).Value);
        Assert.Equal(3, root.OriginalVersion(order));
        Assert.True(root.IsNew(line));

        root.MarkAsDeleted(line);
        Assert.True(root.IsDeleted(line));
        Assert.False(root.Changes().ContainsKey(line));
        root.ClearCommitted();
        Assert.Empty(root.Changes());
        Assert.False(root.IsNew(line));
        Assert.False(root.IsDeleted(line));
    }
}
