using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

[Collection("Log privacy environment")]
public sealed class CheckerPrivacyTests : IDisposable
{
    private const string Flag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
    private const string Ack = "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK";
    private const string Secret = "CHECKER-PRIVATE-CANARY";
    private readonly string? _previous = Environment.GetEnvironmentVariable(Flag);
    public CheckerPrivacyTests() => Environment.SetEnvironmentVariable(Flag, null);
    public void Dispose() => Environment.SetEnvironmentVariable(Flag, _previous);

    [Fact]
    public void ModeledMaskProtectsNativeWireJsonAndException()
    {
        var value = Violation(Secret);
        Context().TranslateCheckResults([value]);
        Assert.Equal(LogPrivacy.MaskAuditValue(Secret), value.InputValue);
        Safe(value, Secret);
    }

    [Fact]
    public void NumericMaskAndUnicodeUseTheExistingAuditAlgorithm()
    {
        foreach (var input in new[] { "123456789", "ab😀😀😀😀yz" })
        {
            var value = Violation(input); Context().TranslateCheckResults([value]);
            Assert.Equal(LogPrivacy.MaskAuditValue(input), value.InputValue); Safe(value, input);
        }
    }

    [Fact]
    public void CredentialsAndCredentialExpectedValuesAreHiddenWithoutMetadata()
    {
        var value = Violation(Secret, "password", expected: "EXPECTED-CREDENTIAL-CANARY");
        new UserContext().TranslateCheckResults([value]);
        Safe(value, Secret, "EXPECTED-CREDENTIAL-CANARY");
        Assert.Equal("[REDACTED]", value.InputValue);
    }

    [Fact]
    public void DirectCatalogTranslationCannotBypassCredentialProtection()
    {
        var value = Violation(Secret, "access_token");
        I18nCatalog.Builtin.Translate(value, TeaQLLocale.English); Safe(value, Secret);
    }

    [Fact]
    public void OriginalUtf16LengthSurvivesTheShorterRuneMask()
    {
        const string input = "😀😀😀😀😀";
        var value = Violation(input, expected: 4); Context().TranslateCheckResults([value]);
        Assert.Contains(input.Length.ToString(), value.Message);
        Assert.DoesNotContain(" 5 ", value.Message); Safe(value, input);
    }

    [Fact]
    public void OrdinaryValuesAndNumericBoundsKeepTheirNativeTypes()
    {
        var value = Violation("ordinary", "name", expected: 4);
        Context().TranslateCheckResults([value]); Assert.Equal("ordinary", value.InputValue);
        Assert.IsType<int>(value.SystemValue); Assert.Equal(4, value.SystemValue);
        Assert.Contains("Name", value.Message);
    }

    [Fact]
    public void ShortCredentialDoesNotCorruptTypedBoundsPathsOrVocabulary()
    {
        var value = Violation("1", "password", expected: 128);
        new UserContext().TranslateCheckResults([value]);
        Assert.Equal(128, value.SystemValue); Assert.Contains("128", value.Message);
        Assert.Contains("Password", value.Message); Assert.EndsWith(" is 1", value.Message);
        Assert.Equal("[REDACTED]", value.InputValue);
    }

    [Fact]
    public void SiblingRootMessagesUseTheInvocationSensitiveInputs()
    {
        var value = Violation(Secret);
        var root = new CheckResult { RuleId = "CUSTOM_REVIEW", Location = ObjectLocation.Root(), Message = "Review " + Secret };
        Context().TranslateCheckResults([value, root]); Safe(value, Secret); Safe(root, Secret);
    }

    [Fact]
    public void CustomCatalogLiteralIsScrubbedWithoutScrubbingTypedSubstitutions()
    {
        var json = JsonSerializer.Serialize(new { schema = "teaql.i18n/v1", locales = new Dictionary<string, object> {
            ["en"] = new { messages = new Dictionary<string,string> { ["checker.maxLength"] = Secret + ": {location} has {input_len}, max {system}: {input}" }, vocabulary = new { } }
        }});
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var value = Violation(Secret, expected: 128);
        Context().InstallI18nCatalog(I18nCatalog.FromJson(stream, I18nCatalog.Builtin)).TranslateCheckResults([value]);
        Safe(value, Secret); Assert.Contains("128", value.Message); Assert.Contains("ContactPhone", value.Message);
    }

    [Fact]
    public void CanonicalSegmentsAliasAndAllWireProfilesRemainUnchanged()
    {
        var location = ObjectLocation.Property("order_item_list").Index(2).PropertyAt("access_token");
        var value = new CheckResult { RuleId = "max_length", EntityType = "Customer", Location = location,
            SourceInstancePath = "/items/2/access~1token~0alias", InputValue = Secret, SystemValue = 4 };
        new UserContext().TranslateCheckResults([value]);
        Assert.Same(location, value.Location); Assert.Equal("/items/2/access~1token~0alias", value.SourceInstancePath);
        foreach (var profile in Enum.GetValues<JsonFieldNamingProfile>())
        { var wire = value.ToWire(profile); Assert.Equal(location.InstancePathWith(profile), wire.InstancePath); Safe(value, Secret); }
    }

    [Fact]
    public async Task CustomerCachedResultsStayOwnedByTheCustomer()
    {
        var cached = Violation(Secret); var provider = new Provider();
        var context = Context().Install(Module().Checker("Customer", new Checker((_, _) => [cached])))
            .WithDataService(provider);
        var first = await Reject(context); var message = first.Message;
        var second = await Reject(context.SetLocaleCode("zh-CN"));
        Safe(first, Secret); Safe(second, Secret); Assert.Equal(message, first.Message);
        Assert.NotEqual(first.Message, second.Message); Assert.Equal(Secret, cached.InputValue);
        Assert.Equal("customer-owned diagnostic", cached.Message); Assert.Equal(0, provider.Mutations);
    }

    [Fact]
    public void AutomaticMessageCanBeRelocalizedWithoutLosingOriginalLength()
    {
        var context = Context(); var value = Violation("😀😀😀😀😀", expected: 4);
        context.TranslateCheckResults([value]); var english = value.Message;
        context.SetLocaleCode("zh-CN").TranslateCheckResults([value]);
        Assert.NotEqual(english, value.Message); Assert.Contains("10", value.Message); Safe(value, "😀😀😀😀😀");
    }

    [Fact]
    public void NestedJsonCredentialsCannotLeakThroughOrdinaryFields()
    {
        var json = new JsonObject { ["access_token"] = Secret, ["ordinary"] = "allowed" };
        var value = Violation(json, "name"); Context().TranslateCheckResults([value]);
        Safe(value, Secret); Assert.Equal(Secret, json["access_token"]!.GetValue<string>());
    }

    [Fact]
    public void TypedJsonCredentialNamesAreInspectedWithoutCallingToString()
    {
        var value = Violation(new CredentialPayload(Secret), "name");
        Context().TranslateCheckResults([value]); Safe(value, Secret);
    }

    [Fact]
    public void CoreValuesAndTypedNullsHaveSafePresentations()
    {
        var value = Violation(new Value.TextValue(Secret)); Context().TranslateCheckResults([value]); Safe(value, Secret);
        var nil = Violation(new Value.TypedNullValue(DataType.Text)); Context().TranslateCheckResults([nil]);
        Assert.Contains("[omitted]", nil.Message);
    }

    [Fact]
    public void CyclicInputFailsClosedInsteadOfBreakingErrorSerialization()
    {
        var cycle = new Dictionary<string, object?>(); cycle["self"] = cycle;
        var value = Violation(cycle, "name"); Context().TranslateCheckResults([value]);
        Assert.NotEmpty(JsonSerializer.Serialize(value.ToWire()));
        Assert.Equal("[REDACTED]", value.InputValue);
    }

    [Fact]
    public async Task RootMessagesAreScrubbedUsingValuesBeforeAndAfterFix()
    {
        const string next = "CHECKER-AFTER-FIX-CANARY";
        var context = Context().Install(Module().Checker("Customer", new Checker((_, request) => {
            ((InsertMutationRequest)request).Command.Values["contact_phone"] = Value.FromObject(next);
            return [new CheckResult { RuleId = "CUSTOM_REVIEW", Location = ObjectLocation.Root(), Message = Secret + " " + next }];
        }))).WithDataService(new Provider());
        var command = new InsertCommand("Customer").Value("contact_phone", Value.FromObject(Secret));
        var error = await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>()
            .MutateAsync(new InsertMutationRequest(command, "Verify private Fix diagnostics")));
        Safe(Assert.Single(error.Violations), Secret, next); Assert.DoesNotContain(Secret, error.ToString());
        Assert.Equal(next, command.Values["contact_phone"].TryText());
    }

    [Fact]
    public void ExactDebugOptInRevealsModeledValuesButNeverCredentials()
    {
        Environment.SetEnvironmentVariable(Flag, Ack + " "); var denied = Violation(Secret);
        Context().TranslateCheckResults([denied]); Safe(denied, Secret);
        Environment.SetEnvironmentVariable(Flag, Ack); var allowed = Violation(Secret);
        Context().TranslateCheckResults([allowed]); Assert.Equal(Secret, allowed.InputValue);
        var credential = Violation(Secret, "password"); Context().TranslateCheckResults([credential]); Safe(credential, Secret);
    }

    [Fact]
    public void CachedDebugSnapshotReturnsToSafeModeWithoutRetainingRawState()
    {
        Environment.SetEnvironmentVariable(Flag, Ack); var value = Violation(Secret);
        Context().TranslateCheckResults([value]); Assert.Equal(Secret, value.InputValue);
        Environment.SetEnvironmentVariable(Flag, null); new UserContext().TranslateCheckResults([value]); Safe(value, Secret);
    }

    [Fact]
    public void CachedCollectionsCannotMutateTheBusinessValueOrHideNewRawValues()
    {
        var input = new List<object?> { Secret }; var value = Violation(input);
        Context().TranslateCheckResults([value]); Safe(value, Secret); Assert.Equal(Secret, input[0]);
        Assert.NotSame(input, value.InputValue);
        if (value.InputValue is IList<object?> cached) cached[0] = "MUTATED-CHECKER-CANARY";
        new UserContext().TranslateCheckResults([value]); Safe(value, Secret, "MUTATED-CHECKER-CANARY");
    }

    [Fact]
    public void CredentialAncestorsHideOrdinaryLeafNames()
    {
        var value = new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("credentials").PropertyAt("value"), InputValue = Secret, SystemValue = 4 };
        new UserContext().TranslateCheckResults([value]); Safe(value, Secret);
    }

    [Fact]
    public void EscapedFreeFormMessagesCannotRestoreKnownSensitiveStrings()
    {
        const string input = "CHECKER-\"QUOTE\"-PRIVATE";
        var value = Violation(input); var root = new CheckResult { RuleId = "CUSTOM_REVIEW", Location = ObjectLocation.Root(), Message = JsonSerializer.Serialize(input) };
        Context().TranslateCheckResults([value, root]);
        Assert.DoesNotContain("QUOTE", root.Message); Safe(value, input);
    }

    [Fact]
    public async Task IndependentContextsCanConcurrentlyReuseTheSameCustomerResult()
    {
        var original = Violation(Secret);
        var errors = await Task.WhenAll(Enumerable.Range(0, 8).Select(async i => {
            var context = Context().SetLocaleCode(i % 2 == 0 ? "en" : "zh-CN")
                .Install(Module().Checker("Customer", new Checker((_, _) => [original])))
                .WithDataService(new Provider());
            return await Reject(context);
        }));
        foreach (var value in errors) Safe(value, Secret);
        Assert.Equal(Secret, original.InputValue); Assert.Equal("customer-owned diagnostic", original.Message);
    }

    [Fact]
    public void DynamicFieldsUseDeclaredMasksButDoNotBecomeDatabasePaths()
    {
        var metadata = new InMemoryMetadataStore().WithEntity(EntityDescriptor.New("Customer")
            .AuditMaskFields(["#private_note"]));
        var value = Violation(Secret, "#private_note");
        new UserContext().WithMetadata(metadata).TranslateCheckResults([value]);
        Safe(value, Secret); Assert.Equal("#private_note", value.ModelPath);
    }

    [Fact]
    public void PrivatePresentationNeverRetainsTheDebugPlaintextOrContext()
    {
        Environment.SetEnvironmentVariable(Flag, Ack); var value = Violation(Secret);
        Context().TranslateCheckResults([value]);
        var presentation = value.GetType().GetProperty("Presentation", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(value);
        Assert.NotNull(presentation); Assert.DoesNotContain(Secret, JsonSerializer.Serialize(presentation, presentation!.GetType()));
        Assert.DoesNotContain("UserContext", presentation.ToString());
    }

    [Fact]
    public void WireProjectionAloneStillHidesCredentialsWithoutMutatingTheOwner()
    {
        var value = Violation(Secret, "access_token");
        Assert.DoesNotContain(Secret, JsonSerializer.Serialize(value.ToWire())); Assert.Equal(Secret, value.InputValue);
    }

    [Fact]
    public void StoredColumnMaskIsResolvedWithoutChangingCanonicalLocation()
    {
        var metadata = new InMemoryMetadataStore().WithEntity(EntityDescriptor.New("Customer")
            .Property(PropertyDescriptor.New("school_type", DataType.Text).ColumnName("school_type_id"))
            .AuditMaskFields(["school_type_id"]));
        var value = Violation(Secret, "school_type"); new UserContext().WithMetadata(metadata).TranslateCheckResults([value]);
        Safe(value, Secret); Assert.Equal("school_type", value.ModelPath); Assert.Equal("/schoolType", value.InstancePath);
    }

    [Fact]
    public void NestedExpectedCredentialValuesAreHiddenButDecimalBoundsRemainExact()
    {
        var value = Violation("ordinary", "name", new JsonObject { ["access_token"] = Secret });
        Context().TranslateCheckResults([value]); Safe(value, Secret); Assert.Equal("ordinary", value.InputValue);
        var bound = Violation(1m, "name", 0.000000000000000000000000001m);
        Context().TranslateCheckResults([bound]); Assert.IsType<decimal>(bound.SystemValue);
        Assert.Equal(0.000000000000000000000000001m, bound.SystemValue);
    }

    [Fact]
    public void InputPlaceholderTextIsNotRecursivelyInterpretedAsATemplate()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"schema\":\"teaql.i18n/v1\",\"locales\":{\"en\":{\"messages\":{\"checker.maxLength\":\"{input}\"},\"vocabulary\":{}}}}"));
        var value = Violation("{system} {input_len} {location}", "name");
        Context().InstallI18nCatalog(I18nCatalog.FromJson(stream)).TranslateCheckResults([value]);
        Assert.Equal("{system} {input_len} {location}", value.Message);
    }

    [Fact]
    public async Task ACheckerThrownStructuredExceptionUsesTheSameSafeBoundary()
    {
        var context = Context().Install(Module().Checker("Customer", new Checker((_, _) =>
            throw new CheckException([Violation(Secret)])))).WithDataService(new Provider());
        Safe(await Reject(context), Secret);
    }

    [Fact]
    public async Task OldValuesAndGuardsRemainInvocationLocalRedactionInputs()
    {
        var context = Context().Install(Module().Checker("Customer", new Checker((_, _) =>
            [new CheckResult { RuleId = "CUSTOM_REVIEW", Location = ObjectLocation.Root(), Message = Secret }]))).WithDataService(new Provider());
        var command = new UpdateCommand("Customer", new Value.I64Value(1)) {
            OldValues = new TeaQL.Core.Record { ["contact_phone"] = Value.FromObject(Secret) }
        }.Value("name", "ordinary");
        var error = await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>()
            .MutateAsync(new UpdateMutationRequest(command, "Verify old-value redaction")));
        Safe(Assert.Single(error.Violations), Secret);
        Assert.Equal(Secret, command.OldValues!["contact_phone"].TryText());
    }

    [Fact]
    public void PublicRuleVocabularySurvivesACollidingSensitiveInput()
    {
        var value = Violation("length"); Context().TranslateCheckResults([value]);
        Assert.Equal("******", value.InputValue); Assert.Contains("length", value.Message);
    }

    [Fact]
    public void StrongerMetadataCannotReuseAnEarlierPlainFallback()
    {
        var value = Violation(Secret); new UserContext().TranslateCheckResults([value]); Assert.Equal(Secret, value.InputValue);
        Context().TranslateCheckResults([value]); Safe(value, Secret);
    }

    [Fact]
    public void ReadOnlyDictionaryCredentialsKeepTheirJsonKeyMeaning()
    {
        IReadOnlyDictionary<string, string> input = System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(
            new Dictionary<string, string> { ["access_token"] = Secret });
        var value = Violation(input, "name"); Context().TranslateCheckResults([value]); Safe(value, Secret);
    }

    [Fact]
    public void NonfiniteDiagnosticValuesCannotBreakErrorSerialization()
    {
        foreach (var input in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
        { var value = Violation(input, "name"); Context().TranslateCheckResults([value]); Assert.NotEmpty(JsonSerializer.Serialize(value.ToWire())); }
    }

    private static EntityDescriptor Descriptor() => EntityDescriptor.New("Customer").Property(PropertyDescriptor.New("contact_phone", DataType.Text))
            .Property(PropertyDescriptor.New("name", DataType.Text)).Property(PropertyDescriptor.New("password", DataType.Text))
            .AuditMaskFields(["contact_phone"]);
    private static RuntimeModule Module() => new RuntimeModule().Entity(Descriptor());
    private static UserContext Context() => new UserContext().WithMetadata(new InMemoryMetadataStore().WithEntity(Descriptor()));
    private static CheckResult Violation(object? input, string field = "contact_phone", object? expected = null) => new() {
        RuleId = "max_length", EntityType = "Customer", Location = ObjectLocation.Property(field),
        InputValue = input, SystemValue = expected ?? 4, Message = "customer-owned diagnostic"
    };
    private static void Safe(CheckResult value, params string[] secrets)
    {
        var outputs = new[] { JsonSerializer.Serialize(value), JsonSerializer.Serialize(value.ToWire()),
            value.InputValue?.ToString() ?? "", value.Message ?? "", new CheckException([value]).ToString() };
        foreach (var secret in secrets) foreach (var output in outputs) Assert.DoesNotContain(secret, output);
    }
    private static async Task<CheckResult> Reject(UserContext context)
    {
        var error = await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>()
            .MutateAsync(new InsertMutationRequest(new InsertCommand("Customer"), "Verify private Checker output")));
        return Assert.Single(error.Violations);
    }
    private sealed class Checker(Func<UserContext, MutationRequest, IReadOnlyList<CheckResult>> check) : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now) => check(context, request);
    }
    private sealed class Provider : IDataService
    {
        public int Mutations;
        public DataServiceCapabilities Capabilities { get; } = new() { Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new InvalidOperationException("Checker must not query a provider");
        public Task<MutationResult> MutateAsync(MutationRequest request) { Mutations++; return Task.FromResult(new MutationResult()); }
    }
    private sealed record CredentialPayload([property: JsonPropertyName("access_token")] string Token)
    { public override string ToString() => "opaque"; }
}
