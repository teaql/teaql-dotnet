using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace TeaQL.Core;

public class XlsBlock
{
    [JsonPropertyName("page")]
    public string Page { get; set; } = string.Empty;

    [JsonPropertyName("top")]
    public int Top { get; set; }

    [JsonPropertyName("bottom")]
    public int Bottom { get; set; }

    [JsonPropertyName("left")]
    public int Left { get; set; }

    [JsonPropertyName("right")]
    public int Right { get; set; }

    [JsonPropertyName("styleReferBlock")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public XlsBlock? StyleReferBlock { get; set; }

    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonNode? NodeValue { get; set; }

    [JsonPropertyName("properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, JsonNode?> Properties { get; set; } = new();

    public XlsBlock() {}

    public XlsBlock(string page, int x, int y, JsonNode? value)
    {
        Page = page;
        Top = y;
        Bottom = y;
        Left = x;
        Right = x;
        NodeValue = value;
    }

    public static XlsBlock FromContext(XlsBlockBuildContext context, JsonNode? value)
    {
        return new XlsBlock(context.PageName, context.X, context.Y, value);
    }

    public XlsBlock Region(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
        return this;
    }

    public XlsBlock Span(int width, int height)
    {
        Right = Left + Math.Max(0, width - 1);
        Bottom = Top + Math.Max(0, height - 1);
        return this;
    }

    public XlsBlock Value(JsonNode? value)
    {
        NodeValue = value;
        return this;
    }

    public XlsBlock AddProperty(string name, JsonNode? value)
    {
        Properties[name] = value;
        return this;
    }

    public void SetProperty(string name, JsonNode? value)
    {
        Properties[name] = value;
    }

    public XlsBlock Style(XlsBlock style)
    {
        StyleReferBlock = style;
        return this;
    }

    public int Width() => Right - Left + 1;

    public int Height() => Bottom - Top + 1;

    public bool Contains(int x, int y)
    {
        return x >= Left && x <= Right && y >= Top && y <= Bottom;
    }

    public JsonNode? ToJsonValue()
    {
        return JsonSerializer.SerializeToNode(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}

public class XlsBlockBuildContext
{
    [JsonPropertyName("page")]
    public string PageName { get; set; } = string.Empty;

    [JsonPropertyName("startX")]
    public int StartX { get; set; }

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    public XlsBlockBuildContext() {}

    public XlsBlockBuildContext(string page, int x, int y)
    {
        X = Math.Max(0, x);
        Y = Math.Max(0, y);
        PageName = page;
        StartX = X;
    }

    public static XlsBlockBuildContext FromPage(string page)
    {
        return new XlsBlockBuildContext(page, 0, 0);
    }

    public static XlsBlockBuildContext Page(string page) => FromPage(page);

    public XlsBlockBuildContext Next()
    {
        return new XlsBlockBuildContext
        {
            PageName = PageName,
            StartX = StartX,
            X = X + 1,
            Y = Y
        };
    }

    public XlsBlockBuildContext NewLine()
    {
        return new XlsBlockBuildContext
        {
            PageName = PageName,
            StartX = StartX,
            X = 0,
            Y = Y + 1
        };
    }

    public XlsBlockBuildContext NextLine()
    {
        return new XlsBlockBuildContext
        {
            PageName = PageName,
            StartX = StartX,
            X = StartX,
            Y = Y + 1
        };
    }

    public XlsBlock ToBlock(JsonNode? value)
    {
        return XlsBlock.FromContext(this, value);
    }
}

public class XlsPage
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("blocks")]
    public List<XlsBlock> Blocks { get; set; } = new();

    public XlsPage() {}

    public XlsPage(string name)
    {
        Name = name;
    }

    public XlsPage AddBlock(XlsBlock block)
    {
        Blocks.Add(block);
        return this;
    }

    public void PushBlock(XlsBlock block)
    {
        Blocks.Add(block);
    }

    public XlsBlock? BlockAt(int x, int y)
    {
        return Blocks.Find(block => block.Contains(x, y));
    }
}

public class XlsWorkbook
{
    [JsonPropertyName("pages")]
    public List<XlsPage> Pages { get; set; } = new();

    public XlsWorkbook() {}

    public XlsWorkbook AddPage(XlsPage page)
    {
        Pages.Add(page);
        return this;
    }

    public void PushPage(XlsPage page)
    {
        Pages.Add(page);
    }

    public XlsPage? Page(string name)
    {
        return Pages.Find(page => page.Name == name);
    }

    public JsonNode? ToJsonValue()
    {
        return JsonSerializer.SerializeToNode(this, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}
