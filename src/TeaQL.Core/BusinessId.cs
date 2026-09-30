namespace TeaQL.Core;

public enum BusinessIdErrorCode
{
    BusinessIdDefinitionInvalid,
    BusinessIdRangeExhausted,
    BusinessIdEncodingFailed
}

public sealed class BusinessIdException(BusinessIdErrorCode code, string message) : Exception(message)
{
    public BusinessIdErrorCode Code { get; } = code;
}

/// <summary>The complete logical allocation and permutation scope.</summary>
public sealed record BusinessIdScope
{
    public string DomainRootKey { get; }
    public string AggregateType { get; }
    public string Namespace { get; }
    public string PeriodKey { get; }

    public BusinessIdScope(string domainRootKey, string aggregateType, string @namespace, string periodKey)
    {
        DomainRootKey = Require(domainRootKey, nameof(domainRootKey));
        AggregateType = Require(aggregateType, nameof(aggregateType));
        Namespace = Require(@namespace, nameof(@namespace));
        PeriodKey = Require(periodKey, nameof(periodKey));
    }

    private static string Require(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdDefinitionInvalid,
                $"{name} must not be blank");
        return value;
    }
}

/// <summary>Versioned 256-bit key material supplied by an application-owned provider.</summary>
public sealed class BusinessIdEncodingKey
{
    private readonly byte[] _bytes;
    public uint Version { get; }

    public BusinessIdEncodingKey(uint version, byte[] bytes)
    {
        if (version == 0)
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdDefinitionInvalid,
                "Business ID key version must be positive");
        if (bytes is not { Length: 32 })
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdDefinitionInvalid,
                "Business ID V1 key must contain exactly 32 bytes");
        Version = version;
        _bytes = bytes.ToArray();
    }

    public byte[] Bytes => _bytes.ToArray();
}
