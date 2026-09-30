using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;
using TeaQL.Core;

namespace TeaQL.Runtime;

public interface IBusinessIdKeyProvider
{
    BusinessIdEncodingKey CurrentKey(UserContext context,
        BusinessIdDefinition definition, BusinessIdScope scope);
}

public interface IBusinessIdProfile
{
    BusinessIdPlan Plan(BusinessIdGenerationRequest request);
    BusinessIdValue Format(BusinessIdPlan plan, BusinessIdAllocation allocation);
    BusinessIdValue Validate(BusinessIdDefinition definition, string value);
}

public interface IBusinessIdProfileFactory
{
    IBusinessIdProfile Create(UserContext context, BusinessIdDefinition definition);
}

public interface IBusinessIdService
{
    Task<BusinessIdValue> EnsureAsync(UserContext context,
        BusinessIdDefinition definition, string domainRootKey,
        string aggregateType, IBusinessIdSlot slot);
}

public sealed class StaticBusinessIdKeyProvider(BusinessIdEncodingKey key) : IBusinessIdKeyProvider
{
    public BusinessIdEncodingKey CurrentKey(UserContext context,
        BusinessIdDefinition definition, BusinessIdScope scope) => key;
}

public sealed class InMemoryBusinessIdAllocator : IBusinessIdAllocator
{
    private sealed class Counter(ulong value) { public ulong Value = value; }
    private readonly ConcurrentDictionary<BusinessIdScope, Counter> _counters = new();

    public Task<BusinessIdAllocation> AllocateAsync(BusinessIdPlan plan)
    {
        var counter = _counters.GetOrAdd(plan.Scope,
            _ => new Counter(plan.InitialSequence));
        lock (counter)
        {
            if (counter.Value > plan.MaximumSequence)
                throw new BusinessIdException(BusinessIdErrorCode.BusinessIdRangeExhausted,
                    $"Business ID range exhausted for {plan.Scope.CanonicalKey}");
            return Task.FromResult(new BusinessIdAllocation(plan.Scope, counter.Value++));
        }
    }
}

public sealed class PermutedDailyBusinessIdProfile(
    UserContext context, IBusinessIdKeyProvider keyProvider) : IBusinessIdProfile
{
    public BusinessIdPlan Plan(BusinessIdGenerationRequest request)
    {
        var definition = request.Definition;
        if (definition.Profile != BusinessIdDefinition.DefaultProfile
            || definition.Reset != "daily"
            || definition.DateFormat != BusinessIdDefinition.DefaultDateFormat
            || definition.Digits != BusinessIdPermutationV1.Width)
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdDefinitionInvalid,
                "daily-permuted-v1 requires reset=daily, dateFormat=yyyyMMdd and digits=6");
        var dateText = request.BusinessDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        return new BusinessIdPlan(definition,
            new BusinessIdScope(request.DomainRootKey, request.AggregateType,
                definition.Namespace, dateText),
            request.BusinessDate, dateText, 0, BusinessIdPermutationV1.MaxSequence);
    }

    public BusinessIdValue Format(BusinessIdPlan plan, BusinessIdAllocation allocation)
    {
        if (allocation.Scope != plan.Scope)
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdDefinitionInvalid,
                "Allocation scope does not match Business ID plan");
        var key = keyProvider.CurrentKey(context, plan.Definition, plan.Scope)
            ?? throw new BusinessIdException(BusinessIdErrorCode.BusinessIdKeyNotFound,
                "Business ID key provider returned no current key");
        var code = BusinessIdPermutationV1.Encode(allocation.Sequence, plan.Scope, key);
        return new BusinessIdValue(string.Join(plan.Definition.Separator,
                plan.Definition.Prefix, plan.DateText, code),
            plan.Definition.Profile, plan.Definition.PolicyVersion);
    }

    public BusinessIdValue Validate(BusinessIdDefinition definition, string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var pattern = $"^{Regex.Escape(definition.Prefix)}{Regex.Escape(definition.Separator)}" +
                      $"(\\d{{8}}){Regex.Escape(definition.Separator)}([0-9A-Z]{{6}})$";
        var match = Regex.Match(value, pattern,
            RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
        if (!match.Success || !DateOnly.TryParseExact(match.Groups[1].Value,
                "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdFormatInvalid,
                $"Invalid daily-permuted-v1 Business ID: {value}");
        return new BusinessIdValue(value, definition.Profile, definition.PolicyVersion);
    }
}

public sealed class DefaultBusinessIdProfileFactory : IBusinessIdProfileFactory
{
    public IBusinessIdProfile Create(UserContext context, BusinessIdDefinition definition)
    {
        if (definition.Profile != BusinessIdDefinition.DefaultProfile)
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdProfileNotFound,
                $"Business ID profile is not registered: {definition.Profile}");
        var provider = context.BusinessIdKeyProvider
            ?? throw new BusinessIdException(BusinessIdErrorCode.BusinessIdKeyNotFound,
                "Business ID key provider is not registered");
        return new PermutedDailyBusinessIdProfile(context, provider);
    }
}

public sealed class DefaultBusinessIdService(IBusinessIdAllocator allocator) : IBusinessIdService
{
    public async Task<BusinessIdValue> EnsureAsync(UserContext context,
        BusinessIdDefinition definition, string domainRootKey,
        string aggregateType, IBusinessIdSlot slot)
    {
        ArgumentNullException.ThrowIfNull(slot);
        var factory = context.BusinessIdProfileFactory
            ?? throw new BusinessIdException(BusinessIdErrorCode.BusinessIdProfileNotFound,
                "Business ID profile factory is not registered");
        var profile = factory.Create(context, definition);
        if (!string.IsNullOrWhiteSpace(slot.CurrentValue))
            return profile.Validate(definition, slot.CurrentValue);
        if (!slot.IsNewAggregate)
            throw new BusinessIdException(BusinessIdErrorCode.BusinessIdImmutable,
                "An established Aggregate cannot be assigned a new Business ID");
        var plan = profile.Plan(new BusinessIdGenerationRequest(definition,
            domainRootKey, aggregateType, context.BusinessDate));
        var value = profile.Format(plan, await allocator.AllocateAsync(plan)
            .ConfigureAwait(false));
        slot.AssignCanonicalValue(value.Value);
        return value;
    }
}
