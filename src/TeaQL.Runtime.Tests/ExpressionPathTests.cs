using System.Text.Json;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class ExpressionPathTests
{
    [Fact]
    public void EarlyRepeatedRelationPreservesSuffixAndOnePositionalBreakpoint()
    {
        var parent = ExpressionPath.Root("E.Node(<root>)").Member("parent", "Parent").MarkMissing();
        var leaf = parent.Member("parent", "Parent").Member("user_url", "UserUrl");
        var error = new TeaQLNotLoadedException(parent, "parent").WithPath(leaf).Terminal("Eval()");
        var wire = error.ToWire();
        Assert.Contains("<broken>", error.Message);
        Assert.Equal("NOT_LOADED", wire.Code);
        Assert.Equal("/parent", wire.InstancePath);
        Assert.Equal("/parent/parent/userUrl", wire.AttemptedInstancePath);
        Assert.Equal("E.Node(<root>).Parent().Parent().UserUrl().Eval()", wire.OriginalExpression);
        Assert.Equal("E.Node(<root>).Parent()<broken>.Parent().UserUrl().Eval()", wire.ExpressionWithBrokenPoint);
        Assert.Equal(0, wire.BreakPointIndex);
        Assert.Equal(wire.Location, wire.SuggestedFix.Location);
    }

    [Theory]
    [InlineData(JsonFieldNamingProfile.CamelCase, "/orderItems/12/userUrl")]
    [InlineData(JsonFieldNamingProfile.SnakeCase, "/order_items/12/user_url")]
    [InlineData(JsonFieldNamingProfile.PascalCase, "/OrderItems/12/UserUrl")]
    public void IndicesNativeOperationsAndWireProfilesRemainSeparate(JsonFieldNamingProfile profile, string attempted)
    {
        var items = ExpressionPath.Root("E.Order(<root>)", profile).Member("order_items", "OrderItems").MarkMissing();
        var error = new TeaQLNotLoadedException(items.At(12).Member("user_url", "UserUrl"), "order_items").Terminal("Eval()");
        Assert.Equal(attempted, error.ToWire().AttemptedInstancePath);
        Assert.Equal(12, error.ToWire().AttemptedLocation[1].Index);
        Assert.Contains(".Get(12).UserUrl().Eval()", error.ToWire().OriginalExpression);
        Assert.Contains(".First().Eval()", new TeaQLNotLoadedException(items.At(0, true), "order_items").Terminal("Eval()").ToWire().OriginalExpression);
        Assert.Single(new TeaQLNotLoadedException(items.Operation("Size()"), "order_items").ToWire().AttemptedLocation);
    }

    [Fact]
    public void SiblingsAndLiteralPunctuationAreNotParsedAndWireSerializationOmitsInput()
    {
        var parent = ExpressionPath.Root("E.Node(<root>)").Member("a~/b.c", "SafeMethod").MarkMissing();
        var first = new TeaQLNotLoadedException(parent.Member("name", "Name"), "a~/b.c").Terminal("OrIfNull(<fallback>)");
        var second = new TeaQLNotLoadedException(parent.Member("code", "Code"), "a~/b.c").Terminal("Eval()");
        Assert.Equal("/a~0~1b.c", first.ToWire().InstancePath);
        Assert.DoesNotContain(".Code()", first.ToWire().OriginalExpression);
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(second.ToWire()));
        Assert.False(json.RootElement.TryGetProperty("root", out _));
        Assert.False(json.RootElement.TryGetProperty("inputValue", out _));
        Assert.Equal("property", json.RootElement.GetProperty("location")[0].GetProperty("kind").GetString());
    }
}
