using TeaQL.Runtime;

namespace TeaQL.Tests.Runtime;

public class ReferenceCapabilityTests
{
    private sealed class Ready(string capability, bool fail = false) : IRuntimeCapabilityResource
    {
        public string Capability => capability;
        public Task ValidateReadyAsync(UserContext context) => fail
            ? Task.FromException(new InvalidOperationException("private provider diagnostic"))
            : Task.CompletedTask;
    }

    [Fact]
    public async Task PassiveManifestRequiresConcreteCapabilityBeforeGraphOrSchema()
    {
        var blob = new RuntimeModule().WithRequiredCapabilities("blob.v1");
        var secure = new RuntimeModule().WithRequiredCapabilities("secure_document.v1", "blob.v1");
        var both = blob.And(secure);
        Assert.Equal(["blob.v1", "secure_document.v1"], both.RequiredCapabilities);
        Assert.Equal(["blob.v1"], blob.RequiredCapabilities);
        var context = both.IntoContext();
        var called = false;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            context.ExecuteGraphSaveAsync("save document", graph =>
            { called = true; return Task.FromResult(1); }));
        Assert.Equal("BLOB_CAPABILITY_MISSING", error.Message);
        Assert.False(called);
        error = await Assert.ThrowsAsync<InvalidOperationException>(context.EnsureSchemaAsync);
        Assert.Equal("BLOB_CAPABILITY_MISSING", error.Message);

        context.InstallRuntimeCapability(new Ready("blob.v1"));
        error = await Assert.ThrowsAsync<InvalidOperationException>(context.ValidateCapabilitiesAsync);
        Assert.Equal("DOCUMENT_CONFIGURATION", error.Message);
        context.InstallRuntimeCapability(new Ready("secure_document.v1"));
        await context.ValidateCapabilitiesAsync();
        Assert.Throws<InvalidOperationException>(() => context.InstallRuntimeCapability(new Ready("blob.v1")));

        var failing = new RuntimeModule().WithRequiredCapabilities("blob.v1").IntoContext();
        failing.InstallRuntimeCapability(new Ready("blob.v1", true));
        error = await Assert.ThrowsAsync<InvalidOperationException>(failing.ValidateCapabilitiesAsync);
        Assert.Equal("BLOB_CAPABILITY_MISSING", error.Message);
    }
}
