using System.Text.Json;
using TeaQL.Core;

namespace TeaQL.Runtime.Tests;

public sealed class ContextRootMetadataTests
{
    [Theory]
    [InlineData("id", "DomainRoot")]
    [InlineData("version", "DomainRoot")]
    [InlineData("name", "DomainRoot")]
    [InlineData("managedRoot", "DomainRoot")]
    [InlineData("managed_root", "OtherRoot")]
    [InlineData("managed_root", "")]
    public void MetadataRequiresCanonicalModeledForwardReference(string field, string target) =>
        Assert.Throws<ArgumentException>(() => ContextRootAdmissionTests.Module(roots:
            new Dictionary<string, IReadOnlyDictionary<string, string>> {
                ["OwnedRecord"] = new Dictionary<string, string> { [field] = target } }));

    [Fact]
    public void ModuleSnapshotsBindingsAndCompositionAndReinstallationPreserveTheirScope()
    {
        var fields = new Dictionary<string, string> { ["managed_root"] = "DomainRoot" };
        var roots = new Dictionary<string, IReadOnlyDictionary<string, string>> { ["OwnedRecord"] = fields };
        var module = ContextRootAdmissionTests.Module(roots: roots);
        fields["managed_root"] = "MutatedByCaller"; roots.Clear();
        var context = module.And(new RuntimeModule()).IntoContext().WithActiveRoot("DomainRoot", 17);
        context.CheckAndFix(ContextRootAdmissionTests.Update());
        context.WithActiveRoot("DomainRoot", 18);
        Assert.Throws<CheckException>(() => context.CheckAndFix(ContextRootAdmissionTests.Update()));
        new RuntimeModule().ApplyTo(context);
        context.CheckAndFix(ContextRootAdmissionTests.Update()); // no stale binding from the previous module
    }

    [Fact]
    public void UnsignedLoadedIdsWithinTheSignedRangeAreAcceptedWithoutFloatCoercion() =>
        ContextRootAdmissionTests.Module().IntoContext().WithActiveRoot("DomainRoot", 17)
            .CheckAndFix(ContextRootAdmissionTests.Update(new Value.U64Value(17)));

    public static IEnumerable<object[]> DiagnosticCells() =>
        from locale in TeaQLLocales.All
        from profile in Enum.GetValues<JsonFieldNamingProfile>()
        from missing in new[] { false, true }
        select new object[] { locale.Code(), profile, missing };

    [Theory]
    [MemberData(nameof(DiagnosticCells))]
    public void DiagnosticsPreserveCanonicalLocationAndProfileWithoutExposingIds(
        string locale, JsonFieldNamingProfile profile, bool missing)
    {
        var context = ContextRootAdmissionTests.Module().IntoContext().SetLocaleCode(locale);
        if (!missing) context.WithActiveRoot("DomainRoot", 18);
        var error = Assert.Throws<CheckException>(() => context.CheckAndFix(ContextRootAdmissionTests.Update()));
        var violation = Assert.Single(error.Violations);
        Assert.Equal(missing ? "context_root_missing" : "context_root_mismatch", violation.RuleId);
        Assert.Equal("OwnedRecord", violation.EntityType); Assert.Equal("managed_root", violation.ModelPath);
        Assert.Equal("ManagedRoot", violation.NativePath);
        Assert.NotEmpty(violation.Message!); Assert.DoesNotContain("checker.", violation.Message!);
        Assert.DoesNotContain("17", violation.Message!); Assert.DoesNotContain("18", violation.Message!);
        Assert.Null(violation.InputValue); Assert.Null(violation.SystemValue); Assert.Null(violation.SourceInstancePath);
        var wire = violation.ToWire(profile);
        Assert.Equal(profile switch { JsonFieldNamingProfile.SnakeCase => "/managed_root",
            JsonFieldNamingProfile.PascalCase => "/ManagedRoot", _ => "/managedRoot" }, wire.InstancePath);
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(wire));
        Assert.Equal("managed_root", document.RootElement.GetProperty("location")[0].GetProperty("name").GetString());
    }
}
