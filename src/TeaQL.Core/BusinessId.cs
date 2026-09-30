namespace TeaQL.Core;

public enum BusinessIdErrorCode
{
    BusinessIdProfileNotFound,
    BusinessIdDefinitionInvalid,
    BusinessIdRangeExhausted,
    BusinessIdAllocationRetryExhausted,
    BusinessIdFormatInvalid,
    BusinessIdDuplicate,
    BusinessIdImmutable,
    BusinessIdKeyNotFound,
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

    public string CanonicalKey => string.Join("|", new[]
    {
        Escape(DomainRootKey), Escape(AggregateType), Escape(Namespace), Escape(PeriodKey)
    });

    private static string Escape(string value) => value.Replace("%", "%25").Replace("|", "%7C");
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

public sealed record BusinessIdDefinition
{
    public const string DefaultProfile = "daily-permuted-v1";
    public const string DefaultDateFormat = "yyyyMMdd";
    public const int DefaultDigits = 6;

    public string FieldName { get; }
    public string Profile { get; }
    public string Prefix { get; }
    public string DateFormat { get; }
    public string Reset { get; }
    public int Digits { get; }
    public string Separator { get; }
    public string Namespace { get; }
    public int PolicyVersion { get; }

    public BusinessIdDefinition(string fieldName, string profile, string prefix,
        string dateFormat, string reset, int digits, string separator,
        string @namespace, int policyVersion)
    {
        FieldName = Require(fieldName, nameof(fieldName));
        Profile = Require(profile, nameof(profile));
        Prefix = Require(prefix, nameof(prefix));
        DateFormat = Require(dateFormat, nameof(dateFormat));
        Reset = Require(reset, nameof(reset));
        Separator = Require(separator, nameof(separator));
        Namespace = Require(@namespace, nameof(@namespace));
        if (profile == DefaultProfile && digits != DefaultDigits)
            throw Invalid("daily-permuted-v1 requires exactly 6 digits");
        if (digits is < 1 or > 18) throw Invalid("digits must be between 1 and 18");
        if (policyVersion < 1) throw Invalid("policyVersion must be positive");
        Digits = digits;
        PolicyVersion = policyVersion;
    }

    public static BusinessIdDefinition DailyPermuted(string fieldName, string prefix,
        string @namespace) => new(fieldName, DefaultProfile, prefix,
        DefaultDateFormat, "daily", DefaultDigits, "-", @namespace, 1);

    public ulong MaximumSequence
    {
        get
        {
            if (Profile == DefaultProfile) return 2_176_782_335UL;
            ulong value = 1;
            for (var index = 0; index < Digits; index++) value = checked(value * 10);
            return value - 1;
        }
    }

    private static string Require(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw Invalid($"{name} must not be blank");
        return value;
    }

    private static BusinessIdException Invalid(string message) =>
        new(BusinessIdErrorCode.BusinessIdDefinitionInvalid, message);
}

public sealed record BusinessIdGenerationRequest(BusinessIdDefinition Definition,
    string DomainRootKey, string AggregateType, DateOnly BusinessDate);

public sealed record BusinessIdPlan
{
    public BusinessIdDefinition Definition { get; }
    public BusinessIdScope Scope { get; }
    public DateOnly BusinessDate { get; }
    public string DateText { get; }
    public ulong InitialSequence { get; }
    public ulong MaximumSequence { get; }

    public BusinessIdPlan(BusinessIdDefinition definition, BusinessIdScope scope,
        DateOnly businessDate, string dateText, ulong initialSequence, ulong maximumSequence)
    {
        if (maximumSequence < initialSequence)
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdDefinitionInvalid,
                "Business ID allocation range must satisfy initialSequence <= maximumSequence");
        Definition = definition;
        Scope = scope;
        BusinessDate = businessDate;
        DateText = dateText;
        InitialSequence = initialSequence;
        MaximumSequence = maximumSequence;
    }
}

public sealed record BusinessIdAllocation(BusinessIdScope Scope, ulong Sequence);

public sealed record BusinessIdValue
{
    public string Value { get; }
    public string Profile { get; }
    public int PolicyVersion { get; }

    public BusinessIdValue(string value, string profile, int policyVersion)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdFormatInvalid,
                "Business ID value must not be blank");
        Value = value;
        Profile = profile;
        PolicyVersion = policyVersion;
    }
}

public interface IBusinessIdSlot
{
    string? CurrentValue { get; }
    bool IsNewAggregate { get; }
    void AssignCanonicalValue(string value);
}

public interface IBusinessIdAllocator
{
    Task<BusinessIdAllocation> AllocateAsync(BusinessIdPlan plan);
}
