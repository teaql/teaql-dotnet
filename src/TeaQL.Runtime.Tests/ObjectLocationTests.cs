using Xunit;

namespace TeaQL.Runtime.Tests;

public class ObjectLocationTests
{
    [Fact]
    public void RendersCanonicalNativeAndExternalPaths()
    {
        var location = ObjectLocation.Property("order_items").Index(2).PropertyAt("user_url");

        Assert.Equal("order_items[2].user_url", location.ModelPath);
        Assert.Equal("OrderItems[2].UserUrl", location.NativePath);
        Assert.Equal("/orderItems/2/userUrl", location.InstancePath);
    }

    [Fact]
    public void EscapesJsonPointerPropertySegments()
    {
        var location = ObjectLocation.Property("a~/b");

        Assert.Equal("/a~0~1b", location.InstancePath);
    }

    [Fact]
    public void LegacyStringInitializerRemainsSourceCompatible()
    {
        var result = new CheckResult { RuleId = "required", Location = "user_email" };

        Assert.Equal("user_email", result.ModelPath);
        Assert.Equal("UserEmail", result.NativePath);
        Assert.Equal("/userEmail", result.InstancePath);
    }
}
