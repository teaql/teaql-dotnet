using TeaQL.Runtime;

namespace TeaQL.SecureDocumentReference;

/// <summary>Trusted current-state source; submitted JSON must never implement this interface.</summary>
public interface ISecureDocumentAssembly
{
    TrustedReferencePrincipal Principal(UserContext context);
    Task<PreparedBoundDocument> SnapshotAsync(UserContext context, AuthorizedResponseGraph graph);
}

public sealed record PreparedBoundDocument(BoundDocumentSnapshot Document, TimeSpan Lifetime);

public partial class SecureDocumentReferenceModule
{
    private sealed class SecureCapability(SecureDocumentReferenceModule module, UserContext owner) : IRuntimeCapabilityResource
    {
        public string Capability => "secure_document.v1";
        public bool BelongsTo(SecureDocumentReferenceModule candidate) => ReferenceEquals(module, candidate);
        public Contributor? Contributor { get; set; }
        public Task ValidateReadyAsync(UserContext context)
        {
            if (!ReferenceEquals(context, owner) || Contributor == null ||
                !ReferenceEquals(context.GetResource<ISecureDocumentContributor>(), Contributor) ||
                !ReferenceEquals(Contributor.Module, module))
                throw new ViewPreparationException("DOCUMENT_CONFIGURATION");
            return Task.CompletedTask;
        }
    }

    private sealed class Contributor(SecureDocumentReferenceModule module, UserContext owner,
        ISecureDocumentAssembly assembly, SecureCapability capability) : ISecureDocumentContributor
    {
        public SecureDocumentReferenceModule Module => module;

        public async Task<PreparedSecureDocument> PrepareAsync(UserContext context,
            AuthorizedResponseGraph graph, ViewOptions options)
        {
            if (!ReferenceEquals(context, owner) || options.Mode != "governed")
                throw new ViewPreparationException("DOCUMENT_CONFIGURATION");
            await capability.ValidateReadyAsync(context).ConfigureAwait(false);
            TrustedReferencePrincipal principal;
            PreparedBoundDocument prepared;
            try
            {
                principal = assembly.Principal(context);
                prepared = await assembly.SnapshotAsync(context, graph).ConfigureAwait(false);
            }
            catch (ReferenceException error) { throw new ViewPreparationException(error.Code); }
            catch { throw new ViewPreparationException("DOCUMENT_CONFIGURATION"); }
            var document = prepared.Document;
            foreach (var slot in graph.BlobSlots)
            {
                if (!ulong.TryParse(slot.EntityId, out var id) || id == 0 || id.ToString() != slot.EntityId ||
                    slot.BindingVersion < 0)
                    throw new ViewPreparationException("DOCUMENT_PROJECTION_VIOLATION");
                var row = document.Snapshot.Entities.FirstOrDefault(entity =>
                    entity.Identity.EntityType == slot.Entity && entity.Identity.Id == id);
                var binding = document.Bindings.FirstOrDefault(value => row != null &&
                    value.RowKey == row.RowKey && value.CanonicalField == slot.Field);
                if (binding == null || binding.EntityType != slot.Entity ||
                    !Equals(binding.Reference, slot.Reference) ||
                    binding.BindingVersion != (ulong)slot.BindingVersion)
                    throw new ViewPreparationException("DOCUMENT_PROJECTION_VIOLATION");
            }
            try
            {
                var token = module.IssueBoundDocumentSnapshot(principal, document, prepared.Lifetime);
                var verified = module.ConsumeBoundDocumentSnapshot(principal, token,
                    document.Snapshot.Scope.AggregateType, document.Snapshot.Scope.Purpose);
                return new PreparedSecureDocument(graph,
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    { ["businessId"] = document.Snapshot.BusinessId, ["documentToken"] = token },
                    verified.ExpiresAt);
            }
            catch (ReferenceException error) { throw new ViewPreparationException(error.Code); }
        }
    }

    /// <summary>Passive installation; readiness and schema belong to explicit host startup.</summary>
    public void Install(UserContext context, ISecureDocumentAssembly assembly)
    {
        if (context == null || assembly == null || context.GetResource<ISecureDocumentContributor>() != null ||
            context.GetNamedResource<IRuntimeCapabilityResource>("teaql.runtime.capability:secure_document.v1") != null)
            throw new ViewPreparationException("DOCUMENT_CONFIGURATION");
        var capability = new SecureCapability(this, context);
        var contributor = new Contributor(this, context, assembly, capability);
        capability.Contributor = contributor;
        try { context.InstallRuntimeCapability(capability); }
        catch { throw new ViewPreparationException("DOCUMENT_CONFIGURATION"); }
        context.InsertNamedResource<IRuntimeCapabilityResource>("teaql.runtime.capability:secure_document.v1", capability);
        context.InsertResource<ISecureDocumentContributor>(contributor);
    }

    /// <summary>Validate this exact module's installation in the supplied context.</summary>
    public Task ReadyAsync(UserContext context)
    {
        var capability = context?.GetNamedResource<IRuntimeCapabilityResource>("teaql.runtime.capability:secure_document.v1") as SecureCapability;
        if (capability == null || !capability.BelongsTo(this))
            throw new ViewPreparationException("DOCUMENT_CONFIGURATION");
        return capability.ValidateReadyAsync(context!);
    }
}
