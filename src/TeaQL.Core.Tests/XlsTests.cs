using System.Text.Json.Nodes;
using TeaQL.Core;
using Xunit;

namespace TeaQL.Core.Tests;

public class XlsTests
{
    [Fact]
    public void TestXlsBlock()
    {
        var block = new XlsBlock("page1", 10, 20, JsonValue.Create("test"));
        Assert.Equal("page1", block.Page);
        Assert.Equal(10, block.Left);
        Assert.Equal(10, block.Right);
        Assert.Equal(20, block.Top);
        Assert.Equal(20, block.Bottom);

        block.Span(2, 3);
        Assert.Equal(11, block.Right);
        Assert.Equal(22, block.Bottom);

        Assert.Equal(2, block.Width());
        Assert.Equal(3, block.Height());

        Assert.True(block.Contains(10, 20));
        Assert.True(block.Contains(11, 22));
        Assert.False(block.Contains(9, 20));
    }

    [Fact]
    public void TestXlsBlockBuildContext()
    {
        var context = XlsBlockBuildContext.Page("page1");
        Assert.Equal("page1", context.PageName);
        Assert.Equal(0, context.X);
        Assert.Equal(0, context.Y);

        var ctx2 = context.Next();
        Assert.Equal(1, ctx2.X);
        Assert.Equal(0, ctx2.Y);

        var ctx3 = ctx2.NextLine();
        Assert.Equal(0, ctx3.X); // StartX is 0
        Assert.Equal(1, ctx3.Y);
    }

    [Fact]
    public void TestXlsWorkbook()
    {
        var workbook = new XlsWorkbook();
        var page = new XlsPage("Sheet1");
        
        var block = new XlsBlock { Page = "Sheet1", Top = 0, Left = 0, Bottom = 10, Right = 10 };
        page.PushBlock(block);
        
        workbook.PushPage(page);
        
        Assert.NotNull(workbook.Page("Sheet1"));
        Assert.NotNull(workbook.Page("Sheet1")?.BlockAt(0, 0));
        Assert.Null(workbook.Page("Sheet1")?.BlockAt(11, 11));
        
        var json = workbook.ToJsonValue();
        Assert.NotNull(json);
        Assert.Equal("Sheet1", json["pages"]![0]!["name"]!.GetValue<string>());
    }
}
