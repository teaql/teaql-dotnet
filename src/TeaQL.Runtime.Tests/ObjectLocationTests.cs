using Xunit;

namespace TeaQL.Runtime.Tests;

public class ObjectLocationTests
{
    [Fact]
    public void PrefixPreservesStructuredRelationAndIndexSegments()
    {
        var child = ObjectLocation.Property("product_name");
        var prefix = ObjectLocation.Property("order_item_list").Index(0);

        var location = child.PrefixedBy(prefix);

        Assert.Equal("order_item_list[0].product_name", location.ModelPath);
        Assert.Equal("OrderItemList[0].ProductName", location.NativePath);
        Assert.Equal("/orderItemList/0/productName", location.InstancePath);
        Assert.Equal(3, location.Segments.Count);
    }

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
