using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TeaQL.Core.Tests;

public class SmartListTests
{
    private class Item
    {
        public ulong Id { get; set; }
        public string Value { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            return obj is Item item && Id == item.Id && Value == item.Value;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Id, Value);
        }
    }

    [Fact]
    public void MergeBy_ReplacesInPlaceAndAppendsNewKeys()
    {
        var items = new SmartList<Item>
        {
            new Item { Id = 1, Value = "one" },
            new Item { Id = 2, Value = "old two" },
            new Item { Id = 4, Value = "four" },
        };

        items.MergeBy(new[]
        {
            new Item { Id = 2, Value = "new two" },
            new Item { Id = 3, Value = "three" }
        }, item => item.Id);

        Assert.Equal(4, items.Count);
        
        Assert.Equal(1ul, items[0].Id);
        Assert.Equal("one", items[0].Value);

        Assert.Equal(2ul, items[1].Id);
        Assert.Equal("new two", items[1].Value);

        Assert.Equal(4ul, items[2].Id);
        Assert.Equal("four", items[2].Value);

        Assert.Equal(3ul, items[3].Id);
        Assert.Equal("three", items[3].Value);
    }

    [Fact]
    public void MergeBy_KeepsOnePositionForRepeatedIncomingKeys()
    {
        var items = new SmartList<Item>
        {
            new Item { Id = 1, Value = "one" }
        };

        items.MergeBy(new[]
        {
            new Item { Id = 2, Value = "first two" },
            new Item { Id = 2, Value = "final two" }
        }, item => item.Id);

        Assert.Equal(2, items.Count);

        Assert.Equal(1ul, items[0].Id);
        Assert.Equal("one", items[0].Value);

        Assert.Equal(2ul, items[1].Id);
        Assert.Equal("final two", items[1].Value);
    }
}
