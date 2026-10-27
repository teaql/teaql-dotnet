using TeaQL.Runtime;
using TeaQL.Runtime.Storage;

namespace TeaQL.BlobReference;

/// <summary>Passive optional assembly of SQLite catalog, verified upload and authorized reads.</summary>
public sealed class BlobReferenceModule
{
    public SqliteBlobCatalog Catalog { get; }
    public SqliteBlobUploadService Upload { get; }
    public SqlBlobResponseContributor Read { get; }

    private sealed class BlobCapability(BlobReferenceModule module, UserContext owner) : IRuntimeCapabilityResource
    {
        public string Capability => "blob.v1";
        public async Task ValidateReadyAsync(UserContext context)
        {
            if (!ReferenceEquals(owner, context) ||
                !ReferenceEquals(context.GetResource<SqliteBlobCatalog>(), module.Catalog) ||
                !ReferenceEquals(context.GetResource<SqliteBlobUploadService>(), module.Upload) ||
                !ReferenceEquals(context.GetResource<IBlobResponseContributor>(), module.Read) ||
                context.GetResource<IObjectStorageProvider>() == null)
                throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
            await module.Catalog.ValidateSchemaAsync().ConfigureAwait(false);
        }
    }

    public BlobReferenceModule(SqliteBlobCatalog catalog, SqliteBlobUploadService upload,
        IBlobReadAuthorization readAuthorization, int maximumUrlSeconds = 600)
    {
        Catalog = catalog ?? throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        Upload = upload ?? throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        if (!ReferenceEquals(upload.Catalog, catalog) || readAuthorization == null)
            throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        Read = new SqlBlobResponseContributor(catalog, readAuthorization, maximumUrlSeconds);
    }

    public void Install(UserContext context)
    {
        if (context == null || context.GetResource<SqliteBlobCatalog>() != null ||
            context.GetResource<SqliteBlobUploadService>() != null ||
            context.GetResource<IBlobResponseContributor>() != null ||
            context.GetNamedResource<IRuntimeCapabilityResource>("teaql.runtime.capability:blob.v1") != null)
            throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        var capability = new BlobCapability(this, context);
        try { context.InstallRuntimeCapability(capability); }
        catch { throw new InvalidOperationException("BLOB_CAPABILITY_MISSING"); }
        context.InsertNamedResource<IRuntimeCapabilityResource>("teaql.runtime.capability:blob.v1", capability);
        context.InsertResource(Catalog);
        context.InsertResource(Upload);
        context.InsertResource<IBlobResponseContributor>(Read);
    }

    public Task ReadyAsync(UserContext context)
    {
        var capability = context?.GetNamedResource<IRuntimeCapabilityResource>("teaql.runtime.capability:blob.v1") as BlobCapability;
        if (capability == null) throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        return capability.ValidateReadyAsync(context!);
    }
}
