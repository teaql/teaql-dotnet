using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class TraceNodeTests
{
    [Fact]
    public void TestTraceNodeNew()
    {
        var node = new TraceNode("User", 123, "created user");
        Assert.Equal("User", node.EntityType);
        Assert.Equal(123UL, node.EntityId);
        Assert.Equal("created user", node.Comment);

        var node2 = new TraceNode("System", null, "system startup");
        Assert.Equal("System", node2.EntityType);
        Assert.Null(node2.EntityId);
        Assert.Equal("system startup", node2.Comment);
    }
}
