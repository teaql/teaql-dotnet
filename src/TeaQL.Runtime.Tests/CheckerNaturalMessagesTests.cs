using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

[Collection("Log privacy environment")]
public sealed class CheckerNaturalMessagesTests
{
    private static JsonObject Fixture(string name)
    {
        using var stream = typeof(CheckerNaturalMessagesTests).Assembly.GetManifestResourceStream(
            "TeaQL.Runtime.Tests.Fixtures.NaturalMessages." + name)!;
        return JsonNode.Parse(stream)!.AsObject();
    }
    private static readonly JsonObject Cases = Fixture("cases-v1.json");
    private static CheckerPresentationModel Model()
    {
        var model = (JsonObject)Cases.DeepClone(); model["schema"] = "teaql.checker-model/v1";
        model.Remove("origin"); model.Remove("cases"); return CheckerPresentationModel.FromJson(model.ToJsonString());
    }
    private static UserContext Context() => new UserContext().InstallCheckerPresentationModel(Model()).InstallCheckerMessageWarning(_ => { });
    private static CheckResult Error(string field = "name", string rule = "required", object? input = null, object? system = null) =>
        new() { RuleId = rule, EntityType = "Entry", Location = ObjectLocation.Property(field), InputValue = input, SystemValue = system };
    private static string Override(string shape, string text, string? item = null) => JsonSerializer.Serialize(new {
        schema = "teaql.checker-messages/v1", defaultLocale = "en", locales = new Dictionary<string, object> {
            ["en"] = item == null ? new { rules = (object)new Dictionary<string, object> { ["checker.required"] = new Dictionary<string,string> { [shape] = text } } }
                : (object)new { rules = new Dictionary<string, object> { ["checker.required"] = new Dictionary<string,string> { [shape] = text } },
                    locationPatterns = new { reference = "the {relation}", item, separator = " → " } } } });

    [Fact]
    public void AllTwentyFourSharedGoldensPreserveShapesAndMachinePaths()
    {
        var roots = Fixture("root-cases-v1.json"); var count = 0;
        foreach (var item in Cases["cases"]!.AsArray().Concat(roots["cases"]!.AsArray()))
        {
            var test = item!.AsObject(); var location = ObjectLocation.Root();
            foreach (var part in test["path"]!.AsArray())
                location = part!.GetValueKind() == JsonValueKind.Number ? location.Index(part.GetValue<int>()) : location.PropertyAt(part.GetValue<string>());
            var locale = TeaQLLocales.Parse(test["locale"]!.GetValue<string>());
            var origin = test.ContainsKey("origin") ? test["origin"]?.GetValue<string>() : "Catalog";
            var snapshot = CheckerMessagePresentation.Capture(Model(), origin, location);
            var arguments = test["arguments"]?.AsObject().ToDictionary(p => p.Key, p => p.Value!.ToString()) ?? [];
            var output = snapshot.Render(CheckerMessageCatalog.Builtin, CheckerMessageCatalog.Builtin.Select(locale, test["rule"]!.GetValue<string>(), snapshot.Shape), arguments);
            Assert.Equal(test["shape"]!.GetValue<string>(), snapshot.Shape.ToString().ToUpperInvariant());
            Assert.Equal(test["expected"]!.GetValue<string>(), output.Message);
            if (test["instancePath"] != null) Assert.Equal(test["instancePath"]!.GetValue<string>(), location.InstancePath);
            count++;
        }
        Assert.Equal(24, count);
    }

    [Fact]
    public void AllTwoHundredSeventyBuiltinTemplatesArePresent()
    {
        var count = 0;
        foreach (var locale in TeaQLLocales.All) foreach (var rule in new[] { "required", "min", "max", "minLength", "maxLength", "invalid" })
            foreach (var shape in Enum.GetValues<CheckerMessageShape>())
            { Assert.NotNull(CheckerMessageCatalog.Builtin.Lookup(locale, "checker." + rule, shape)); count++; }
        Assert.Equal(270, count);
    }

    [Fact]
    public void EnglishDefaultIgnoresHostCultureAndExplicitInvalidLocaleIsAtomic()
    {
        var prior = CultureInfo.CurrentUICulture;
        try {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN"); var context = Context(); var error = Error();
            context.TranslateCheckResultsFrom("Catalog", [error]);
            Assert.Equal("The name is required.", Assert.Single(context.TranslateCheckResultsFrom("Catalog", [error])).Message);
            Assert.Throws<UnsupportedLocaleException>(() => context.SetLocaleCode("unsupported"));
            Assert.Equal(TeaQLLocale.English, context.Locale);
        } finally { CultureInfo.CurrentUICulture = prior; }
    }

    [Theory]
    [InlineData("{field", false)]
    [InlineData("{field} {secret}", false)]
    [InlineData("{field.ToString()}", false)]
    [InlineData("{field} {input}", false)]
    [InlineData("{field} {object}", true)]
    [InlineData("{{literal}} {field}", true)]
    public void TemplatesAreDataOnlyAndRuleArgumentsAreWhitelisted(string text, bool valid)
    {
        var json = Override("one", text);
        if (valid && !text.Contains("object")) Assert.NotNull(CheckerMessageCatalog.FromJson(json));
        else Assert.Throws<ArgumentException>(() => CheckerMessageCatalog.FromJson(json));
    }

    [Fact]
    public void DuplicateJsonAndNormalizedLocaleAliasesAreRejected()
    {
        Assert.Throws<ArgumentException>(() => CheckerMessageCatalog.FromJson(Override("one", "{field}").Replace("\"schema\":", "\"schema\":\"bad\",\"schema\":")));
        var json = JsonNode.Parse(Override("one", "{field}"))!;
        json["locales"]!["en-US"] = json["locales"]!["en"]!.DeepClone();
        Assert.Throws<ArgumentException>(() => CheckerMessageCatalog.FromJson(json.ToJsonString()));
    }

    [Fact]
    public void LiteralValuesDoNotExecuteAndRolesDoNotCollapseToPerson()
    {
        var context = Context().InstallCheckerMessageCatalog(Override("one", "{{literal}} {field}"));
        Assert.Equal("{literal} name", Assert.Single(context.TranslateCheckResultsFrom("Catalog", [Error()])).Message);
        foreach (var role in new[] { "payer", "beneficiary" }) {
            var error = Error().PrefixedBy(ObjectLocation.Property(role));
            Assert.Equal($"The name of the {role} is required.", Assert.Single(Context().TranslateCheckResultsFrom("Catalog", [error])).Message);
            Assert.Equal($"/{role}/name", error.InstancePath);
        }
    }

    [Fact]
    public void SafePlansPreserveUtf16LengthAcrossRelocationAndLocaleChanges()
    {
        const string secret = "😀😀😀😀😀"; var context = Context();
        var error = Error("password", "max_length", secret, 4);
        var original = Assert.Single(context.TranslateCheckResultsFrom("Entry", [error]));
        var prefixed = original.PrefixedBy(ObjectLocation.Property("entry_list").Index(1));
        var relocated = Assert.Single(context.TranslateCheckResultsFrom("Catalog", [prefixed]));
        Assert.Contains("10", relocated.Message); Assert.DoesNotContain(secret, relocated.Message);
        Assert.Contains("2nd entry", relocated.Message); Assert.Equal("/entryList/1/password", relocated.InstancePath);
        Assert.Contains("10", Assert.Single(context.SetLocaleCode("zh-CN").TranslateCheckResults([relocated])).Message);
        Assert.Equal("/password", original.InstancePath);
    }

    [Fact]
    public void RetainedCustomNodesAreProjectedAgainstNewBatchSecrets()
    {
        const string secret = "LATER-PRIVATE-CANARY";
        var context = Context().InstallCheckerMessageCatalog(Override("one", secret + " {field}", secret + " {ordinal} {relation}"));
        var retained = Assert.Single(context.TranslateCheckResultsFrom("Catalog", [Error()])); Assert.Contains(secret, retained.Message);
        var projected = context.TranslateCheckResults([retained, Error("password", "max_length", secret, 128)]);
        Assert.DoesNotContain(secret, projected[0].Message);
        var relocated = Assert.Single(context.TranslateCheckResultsFrom("Catalog", [projected[0].PrefixedBy(ObjectLocation.Property("entry_list").Index(1))]));
        Assert.DoesNotContain(secret, relocated.Message); Assert.Contains("2nd", relocated.Message);
        Assert.Contains("128", projected[1].Message);
    }

    [Fact]
    public void CustomerRendererRunsOnceAndFailureWarningsAreValueFree()
    {
        var calls = 0; var context = Context().InstallCheckerMessageRenderer(request => { calls++; return new("customer literal", []); });
        var result = Assert.Single(context.TranslateCheckResultsFrom("Entry", [Error()]));
        var relocated = Assert.Single(context.TranslateCheckResultsFrom("Catalog", [result.PrefixedBy(ObjectLocation.Property("entry_list").Index(1))]));
        Assert.Equal("customer literal", relocated.Message); Assert.Equal(1, calls);
        var warnings = new List<CheckerMessageWarning>();
        context.InstallCheckerMessageWarning(warnings.Add).InstallCheckerMessageRenderer(_ => throw new InvalidOperationException("PRIVATE-CAUSE"));
        var fallback = Assert.Single(context.TranslateCheckResultsFrom("Catalog", [Error()]));
        Assert.Equal("The name is invalid.", fallback.Message); Assert.Contains(CheckerMessageWarning.RenderFallback, warnings);
        Assert.DoesNotContain("PRIVATE-CAUSE", new CheckException([fallback]).ToString());
    }

    [Fact]
    public void ExplicitCustomerLiteralRemainsAuthoritative()
    {
        var original = Error(); original.Message = "Contact support";
        Assert.Equal("Contact support", Assert.Single(Context().TranslateCheckResultsFrom("Catalog", [original])).Message);
        Assert.Equal("Contact support", original.Message);
    }

    [Fact]
    public void PrefixCopyPreservesAllDiagnosticMetadataWithoutChangingItsOwner()
    {
        var original = new CheckResult { RuleId = "max_length", EntityType = "PrivateEntry",
            Location = ObjectLocation.Property("description"), SourceInstancePath = "/payload/note~1alias",
            InputValue = "abc", SystemValue = 8, Message = "Customer message" };
        var copied = original.PrefixedBy(ObjectLocation.Property("entry_list").Index(1));
        Assert.NotSame(original, copied); Assert.Equal("/description", original.InstancePath);
        Assert.Equal("/entryList/1/description", copied.InstancePath);
        Assert.Equal(original.RuleId, copied.RuleId); Assert.Equal(original.EntityType, copied.EntityType);
        Assert.Equal(original.SourceInstancePath, copied.SourceInstancePath);
        Assert.Equal(original.InputValue, copied.InputValue); Assert.Equal(original.SystemValue, copied.SystemValue);
        Assert.Equal(original.Message, copied.Message);
    }

    private sealed class LengthChecker : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest mutation, DateTimeOffset now) => [Error("description", "max_length", system: 8)];
    }

    [Fact]
    public void NormalCheckerRetainsMeasurementWhenGeneratedCheckerOmitsInput()
    {
        var context = Context().Install(new RuntimeModule().Checker("Entry", new LengthChecker()));
        var command = new InsertCommand("Entry"); command.Values["description"] = Value.FromObject("密钥🔑abcdef");
        var error = Assert.Throws<CheckException>(() => context.CheckAndFix(new InsertMutationRequest(command, "Verify measured error")));
        var value = Assert.Single(error.Violations); Assert.Contains("10", value.Message); Assert.Contains("8", value.Message);
        Assert.Null(value.InputValue); Assert.Equal("Entry", value.EntityType);
    }
}
