using TeaQL.SecureDocumentReference;
using TeaQL.Core;
using System.Security.Cryptography;
using TeaQL.Runtime;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public sealed class SecureReferenceProtocolTests
{
    private const string RustTqr1 = "tqr1.k2.MzMzMzMzMzMzMzMzmhCET15vBdSc1BbFJcrvMMnRP1wAyjunShYlFPnDZL7Gk5V1LuptWyrbiQ8LQSDHFUmN3o6vb-MEgXZuh3N-YkeO55CYLeVgiyBBYslzuSu-fvWC2KMZcnZ5D8BRmWpGYA2Ok5sWe1Fg1aIyadTjVoKB-H2vPW_0okj-";

    [Fact]
    public void Tqr1MatchesRustBytesAndRejectsForeignContext()
    {
        var now = DateTimeOffset.Parse("2026-09-27T00:00:00Z");
        var keys = new StaticReferenceKeyProvider("k2", new ReferenceKey("k1", Enumerable.Repeat((byte)0x11, 32).ToArray()),
            new ReferenceKey("k2", Enumerable.Repeat((byte)0x22, 32).ToArray()));
        var runtime = new ContextBoundReferenceRuntime("order-service", "test-a", keys,
            (actor, _, identity) => actor.Subject == "alice" && identity.EntityType == "OrderItem" && identity.Id == 42,
            () => now, () => Enumerable.Repeat((byte)0x33, 12).ToArray());
        var alice = new TrustedReferencePrincipal("oidc", "alice", "Platform", 7);
        var scope = new ReferenceDocumentScope("doc-100", "edit-order", "Order", 100, 9);
        var identity = new ReferenceIdentity("OrderItem", 42, 3);
        Assert.Equal(RustTqr1, runtime.Issue(alice, scope, identity, TimeSpan.FromSeconds(600)));
        Assert.Equal(identity, runtime.Consume(alice, scope, RustTqr1, "OrderItem").Identity);
        Assert.Equal("ROUND_TRIP_REFERENCE_SCOPE_MISMATCH", Assert.Throws<ReferenceException>(() =>
            runtime.Consume(alice with { Subject = "bob" }, scope, RustTqr1, "OrderItem")).Code);
        Assert.Equal("ROUND_TRIP_REFERENCE_SCOPE_MISMATCH", Assert.Throws<ReferenceException>(() =>
            runtime.Consume(alice, scope with { DocumentId = "doc-101" }, RustTqr1, "OrderItem")).Code);
        Assert.Equal("ROUND_TRIP_REFERENCE_INVALID", Assert.Throws<ReferenceException>(() =>
            runtime.Consume(alice, scope, RustTqr1 + "=", "OrderItem")).Code);
        now = now.AddMinutes(10);
        Assert.Equal("ROUND_TRIP_REFERENCE_EXPIRED", Assert.Throws<ReferenceException>(() =>
            runtime.Consume(alice, scope, RustTqr1, "OrderItem")).Code);
    }

    private static (SecureDocumentReferenceModule Runtime, TrustedReferencePrincipal Actor, DocumentSnapshot Snapshot) DocumentFixture()
    {
        var keys = new StaticReferenceKeyProvider("k1", new ReferenceKey("k1", Enumerable.Repeat((byte)0x42, 32).ToArray()));
        var runtime = new SecureDocumentReferenceModule("order-service", "test", keys, (_, _, _) => true,
            () => DateTimeOffset.FromUnixTimeSeconds(1_800_000_000),
            () => Enumerable.Repeat((byte)0x33, 12).ToArray());
        var actor = new TrustedReferencePrincipal("oidc", "alice", "Merchant", 7);
        var scope = new ReferenceDocumentScope("doc-1", "edit-order", "Order", 10, 4);
        var snapshot = new DocumentSnapshot("model-v1", "ORD-X1", scope,
            new ReferenceIdentity("Order", 10, 4),
            [new DocumentEntitySnapshot("item:0", new ReferenceIdentity("OrderItem", 20, 2),
                ["product_code", "quantity"], ["quantity"], ["UPDATE"])],
            [new DocumentRelationSnapshot("items", ["item:0"], "COMPLETE", true,
                ["CREATE_CHILD", "REMOVE_CHILD"])]);
        return (runtime, actor, snapshot);
    }

    private static string Sha(string text) => Convert.ToHexString(SHA256.HashData(System.Text.Encoding.ASCII.GetBytes(text))).ToLowerInvariant();

    [Fact]
    public async Task SecureModuleReadyRejectsAnotherModuleInstallation()
    {
        var (installed, principal, snapshot) = DocumentFixture();
        var (foreign, _, _) = DocumentFixture();
        var context = new UserContext();
        installed.Install(context, new Assembly(principal, new BoundDocumentSnapshot(snapshot, [])));
        await installed.ReadyAsync(context);
        Assert.Equal("DOCUMENT_CONFIGURATION", (await Assert.ThrowsAsync<ViewPreparationException>(
            () => foreign.ReadyAsync(context))).Code);
        await installed.ReadyAsync(context);
    }

    [Fact]
    public void Tqd1MatchesRustHashAndReauthorizesOnConsumption()
    {
        var (runtime, actor, snapshot) = DocumentFixture();
        var token = runtime.IssueDocumentSnapshot(actor, snapshot, TimeSpan.FromSeconds(600));
        Assert.Equal("69fee1ec01cd4a5b69dd3f795aeaffb3a3546e50d48e25cfc9b8f169a527b267", Sha(token));
        Assert.Equal("ORD-X1", runtime.ConsumeDocumentSnapshot(actor, token, "Order", "edit-order").Document.BusinessId);
        Assert.Equal("ROUND_TRIP_REFERENCE_SCOPE_MISMATCH", Assert.Throws<ReferenceException>(() =>
            runtime.ConsumeDocumentSnapshot(actor with { Subject = "bob" }, token, "Order", "edit-order")).Code);
        Assert.Equal("ROUND_TRIP_REFERENCE_INVALID", Assert.Throws<ReferenceException>(() =>
            runtime.ConsumeDocumentSnapshot(actor, token, "Order", "read-order")).Code);
    }

    [Fact]
    public void Tqd2AndTqs1MatchSharedJavaRustHashes()
    {
        var (runtime, actor, original) = DocumentFixture();
        var snapshot = original with
        {
            Entities = [original.Entities[0] with
            {
                LoadedFields = ["attachment", "product_code", "quantity"],
                WritableFields = ["attachment", "quantity"]
            }]
        };
        var document = new BoundDocumentSnapshot(snapshot,
            [new DocumentBlobBindingSnapshot("item:0", "attachment", "OrderItem",
                new BlobRef("blob_A"), ulong.MaxValue, true, true)]);
        var token = runtime.IssueBoundDocumentSnapshot(actor, document, TimeSpan.FromSeconds(300));
        Assert.Equal("e7c81ca089b0a3e0585faefcc275aeb44b456d666d9a4a17949c92b1891089d7", Sha(token));
        Assert.Equal(ulong.MaxValue, runtime.ConsumeBoundDocumentSnapshot(actor, token, "Order", "edit-order")
            .Document.Bindings[0].BindingVersion);
        var target = new BlobSelectionTarget(snapshot.Scope, "OrderItem", "attachment", "EXISTING_ROW", "item:0", "command-1");
        var claim = new BlobSelectionClaim(target, new BlobRef("blob_B"));
        var selection = runtime.IssueBlobSelection(actor, token, claim, TimeSpan.FromSeconds(120));
        Assert.Equal("31303e3e251dd0e8963bf2a4d8897d1eccde63a485aaf287f88f29d40757ba96", Sha(selection));
        Assert.Equal("blob_B", runtime.ConsumeBlobSelection(actor, token, selection, target).Document.Reference.Key);
        Assert.Equal("ROUND_TRIP_REFERENCE_SCOPE_MISMATCH", Assert.Throws<ReferenceException>(() =>
            runtime.ConsumeBlobSelection(actor, token, selection, target with { CommandId = "other-command" })).Code);
        Assert.Equal("ROUND_TRIP_REFERENCE_SCOPE_MISMATCH", Assert.Throws<ReferenceException>(() =>
            runtime.ConsumeBlobSelection(actor with { Subject = "bob" }, token, selection, target)).Code);
    }

    private sealed class Assembly(TrustedReferencePrincipal actor, BoundDocumentSnapshot document) : ISecureDocumentAssembly
    {
        public TrustedReferencePrincipal Principal(UserContext _) => actor;
        public Task<PreparedBoundDocument> SnapshotAsync(UserContext _, AuthorizedResponseGraph __)
            => Task.FromResult(new PreparedBoundDocument(document, TimeSpan.FromSeconds(300)));
    }
    private sealed class Authorizer(AuthorizedBlobSlot slot) : IResponseGraphAuthorizer
    {
        public Task<AuthorizedResponseGraph> AuthorizeAsync(UserContext _, object? __, ViewOptions ___)
            => Task.FromResult(new AuthorizedResponseGraph(
                new Dictionary<string, object?> { ["attachment"] = slot.Reference }, [slot]));
    }
    private sealed class Projector : IBlobResponseContributor
    {
        public Task<IReadOnlyList<ProjectedBlobUrl>> ProjectBatchAsync(UserContext _,
            AuthorizedResponseGraph __, DateTimeOffset? ___)
            => Task.FromResult<IReadOnlyList<ProjectedBlobUrl>>(
                [new ProjectedBlobUrl("https://files.example.invalid/document", DateTimeOffset.UtcNow.AddMinutes(1))]);
    }

    [Fact]
    public async Task ModuleChecksEncryptedBindingBeforeGovernedBlobProjection()
    {
        var (module, actor, original) = DocumentFixture();
        var snapshot = original with { Entities = [original.Entities[0] with
        { LoadedFields = ["attachment", "product_code", "quantity"], WritableFields = ["attachment", "quantity"] }] };
        var reference = new BlobRef("blob_A");
        var document = new BoundDocumentSnapshot(snapshot,
            [new DocumentBlobBindingSnapshot("item:0", "attachment", "OrderItem", reference, 4, true, true)]);
        var slot = new AuthorizedBlobSlot(["attachment"], "OrderItem", "20", "attachment", "tenant-a", 4, reference);
        var context = new UserContext(); var assembly = new Assembly(actor, document);
        module.Install(context, assembly); await module.ReadyAsync(context);
        context.InsertResource<IResponseGraphAuthorizer>(new Authorizer(slot));
        context.InsertResource<IBlobResponseContributor>(new Projector());
        var view = await new ViewPreparationService().PrepareAsync(context, null,
            new ViewOptions("governed", "committed"));
        var envelope = Assert.IsType<Dictionary<string, object?>>(view.Document);
        Assert.Equal("ORD-X1", envelope["businessId"]);
        _ = module.ConsumeBoundDocumentSnapshot(actor, Assert.IsType<string>(envelope["documentToken"]),
            "Order", "edit-order");
        Assert.Equal("https://files.example.invalid/document",
            Assert.IsType<Dictionary<string, object?>>(view.Presentation)["attachment"]);
        Assert.Throws<ViewPreparationException>(() => module.Install(context, assembly));
        context.InsertResource<IResponseGraphAuthorizer>(new Authorizer(slot with { BindingVersion = 5 }));
        var failure = await Assert.ThrowsAsync<ViewPreparationException>(() =>
            new ViewPreparationService().PrepareAsync(context, null, new ViewOptions("governed", "committed")));
        Assert.Equal("DOCUMENT_PROJECTION_VIOLATION", failure.Code);
        Assert.Equal("committed", failure.BusinessState);
    }
}
