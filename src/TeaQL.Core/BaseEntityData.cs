using System.Collections.Generic;
using System.Linq;

namespace TeaQL.Core;

public record BaseEntityData
{
    public ulong Id { get; set; }
    public long Version { get; set; }
    public Record Dynamic { get; set; } = new();

    public BaseEntityData WithId(ulong id)
    {
        Id = id;
        return this;
    }

    public BaseEntityData WithVersion(long version)
    {
        Version = version;
        return this;
    }

    public BaseEntityData WithDynamic(string key, Value value)
    {
        Dynamic[key] = value;
        return this;
    }

    public Value? DynamicValue(string key) => Dynamic.TryGetValue(key, out var v) ? v : null;

    public long? DynamicI64(string key) => DynamicValue(key) is Value.I64Value i ? i.Value : null;
    public ulong? DynamicU64(string key) => DynamicValue(key) is Value.U64Value u ? u.Value : null;
    public decimal? DynamicDecimal(string key) => DynamicValue(key) is Value.DecimalValue d ? d.Value : null;
    public double? DynamicF64(string key) => DynamicValue(key) is Value.F64Value f ? f.Value : null;
    public string? DynamicText(string key) => DynamicValue(key) is Value.TextValue t ? t.Value : null;
    public bool? DynamicBool(string key) => DynamicValue(key) is Value.BoolValue b ? b.Value : null;

    public Value? PutDynamic(string key, Value value)
    {
        Dynamic.TryGetValue(key, out var old);
        Dynamic[key] = value;
        return old;
    }

    public Value? RemoveDynamic(string key)
    {
        if (Dynamic.TryGetValue(key, out var old))
        {
            Dynamic.Remove(key);
            return old;
        }
        return null;
    }

    public Record ToRecord()
    {
        var record = new Record
        {
            ["id"] = new Value.U64Value(Id),
            ["version"] = new Value.I64Value(Version)
        };
        foreach (var kvp in Dynamic)
        {
            record[kvp.Key] = kvp.Value;
        }
        return record;
    }

    public static BaseEntityData FromRecord(Record record)
    {
        ulong id = 0;
        if (record.TryGetValue("id", out var idVal))
        {
            if (idVal is Value.U64Value u) id = u.Value;
            else if (idVal is Value.I64Value i && i.Value >= 0) id = (ulong)i.Value;
            else if (idVal is not Value.NullValue) throw new EntityError("BaseEntity", "invalid id field");
        }

        long version = 0;
        if (record.TryGetValue("version", out var vVal))
        {
            if (vVal is Value.I64Value i) version = i.Value;
            else if (vVal is not Value.NullValue) throw new EntityError("BaseEntity", "invalid version field");
        }

        var dynamic = new Record(record.Where(k => k.Key != "id" && k.Key != "version").ToDictionary(k => k.Key, k => k.Value));

        return new BaseEntityData { Id = id, Version = version, Dynamic = dynamic };
    }
}
