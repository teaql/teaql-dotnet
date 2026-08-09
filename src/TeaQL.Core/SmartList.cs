using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TeaQL.Core;

public class SmartList<T> : List<T>
{
    public long? TotalCount { get; set; }
    public Record Aggregations { get; set; } = new();
    public Record Summary { get; set; } = new();
    public Dictionary<string, SmartList<Record>> Facets { get; set; } = new();
    public bool IsLoaded { get; set; } = true;

    public SmartList() { }
    
    public SmartList(IEnumerable<T> collection) : base(collection) { }

    public static SmartList<T> Empty()
    {
        return new SmartList<T> { IsLoaded = false };
    }

    public Value? IntoValue()
    {
        bool hasChanges = false;
        var items = new List<Value>();

        foreach (var item in this)
        {
            if (item is IEntity entity)
            {
                if (entity.IsNew() || entity.DirtyFields() != null || entity.IsMarkedAsDelete())
                {
                    hasChanges = true;
                }
                items.Add(new Value.ObjectValue(entity.IntoRecord()));
            }
            else
            {
                items.Add(new Value.NullValue());
            }
        }

        if (IsLoaded || hasChanges)
        {
            return new Value.ListValue(items);
        }

        return null;
    }

    public SmartList<T> WithTotalCount(long totalCount)
    {
        TotalCount = totalCount;
        return this;
    }

    public SmartList<T> WithAggregation(string key, Value value)
    {
        Aggregations[key] = value;
        return this;
    }

    public SmartList<T> WithSummary(string key, Value value)
    {
        Summary[key] = value;
        return this;
    }

    public SmartList<T> WithFacet(string key, SmartList<Record> facet)
    {
        Facets[key] = facet;
        return this;
    }

    public void AddFacet(string key, SmartList<Record> facet)
    {
        Facets[key] = facet;
    }

    public SmartList<Record>? Facet(string key)
    {
        return Facets.TryGetValue(key, out var facet) ? facet : null;
    }

    public SmartList<Record>? RemoveFacet(string key)
    {
        if (Facets.TryGetValue(key, out var facet))
        {
            Facets.Remove(key);
            return facet;
        }
        return null;
    }

    public Dictionary<string, SmartList<Record>> TakeFacets()
    {
        var taken = Facets;
        Facets = new Dictionary<string, SmartList<Record>>();
        return taken;
    }

    public void Retain(Predicate<T> filter)
    {
        RemoveAll(x => !filter(x));
    }

    public long TotalCountOrLen()
    {
        return TotalCount ?? Count;
    }

    public Value? Aggregation(string key)
    {
        return Aggregations.TryGetValue(key, out var value) ? value : null;
    }

    public Value? GetSummary(string key)
    {
        return Summary.TryGetValue(key, out var value) ? value : null;
    }

    public JsonNode? AggregationJson()
    {
        return JsonSerializer.SerializeToNode(Aggregations);
    }

    public JsonNode? SummaryJson()
    {
        return JsonSerializer.SerializeToNode(Summary);
    }

    public SmartList<U> Map<U>(Func<T, U> mapper)
    {
        return new SmartList<U>(this.Select(mapper))
        {
            TotalCount = TotalCount,
            Aggregations = Aggregations,
            Summary = Summary,
            Facets = Facets,
            IsLoaded = IsLoaded
        };
    }

    public List<U> ToList<U>(Func<T, U> mapper)
    {
        return this.Select(mapper).ToList();
    }

    public HashSet<U> ToSet<U>(Func<T, U> mapper)
    {
        return new HashSet<U>(this.Select(mapper));
    }

    public Dictionary<K, T> IdentityMap<K>(Func<T, K> keySelector) where K : notnull
    {
        var map = new Dictionary<K, T>();
        foreach (var item in this)
        {
            map[keySelector(item)] = item;
        }
        return map;
    }

    public Dictionary<K, List<T>> GroupBy<K>(Func<T, K> keySelector) where K : notnull
    {
        var groups = new Dictionary<K, List<T>>();
        foreach (var item in this)
        {
            var key = keySelector(item);
            if (!groups.TryGetValue(key, out var list))
            {
                list = new List<T>();
                groups[key] = list;
            }
            list.Add(item);
        }
        return groups;
    }

    public void MergeBy<K>(IEnumerable<T> incoming, Func<T, K> keySelector) where K : notnull
    {
        var positions = new Dictionary<K, int>();
        for (int i = 0; i < Count; i++)
        {
            var key = keySelector(this[i]);
            positions[key] = i; 
        }

        foreach (var item in incoming)
        {
            var key = keySelector(item);
            if (positions.TryGetValue(key, out var index))
            {
                this[index] = item;
            }
            else
            {
                positions[key] = Count;
                Add(item);
            }
        }
    }

    public SmartList<Record> IntoRecords()
    {
        return new SmartList<Record>(this.Select(item => ((IEntity)item!).IntoRecord()))
        {
            TotalCount = TotalCount,
            Aggregations = Aggregations,
            Summary = Summary,
            Facets = Facets,
            IsLoaded = IsLoaded
        };
    }

    public List<Value> Ids()
    {
        return this.Select(item => ((IIdentifiableEntity)item!).IdValue()).ToList();
    }

    public Dictionary<string, T> MapById()
    {
        var map = new Dictionary<string, T>();
        foreach (var item in this)
        {
            var idValue = ((IIdentifiableEntity)item!).IdValue();
            map[IdKey(idValue)] = item;
        }
        return map;
    }

    public List<long> Versions()
    {
        return this.Select(item => ((IVersionedEntity)item!).Version()).ToList();
    }

    private static string IdKey(Value value)
    {
        return value switch
        {
            Value.NullValue => "null",
            Value.BoolValue b => $"b:{b.Value}",
            Value.I64Value i => $"i:{i.Value}",
            Value.U64Value u => $"u:{u.Value}",
            Value.F64Value f => $"f:{f.Value}",
            Value.DecimalValue d => $"decimal:{d.Value}",
            Value.TextValue t => $"t:{t.Value}",
            Value.JsonValue j => $"j:{j.Value}",
            Value.DateValue d => $"date:{d.Value:O}",
            Value.TimestampValue ts => $"ts:{ts.Milliseconds}",
            Value.ObjectValue => "object",
            Value.ListValue => "list",
            Value.TypedNullValue => "null",
            _ => "null"
        };
    }
}
