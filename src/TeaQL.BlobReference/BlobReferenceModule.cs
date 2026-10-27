using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.DataService;
using TeaQL.Sql;

namespace TeaQL.BlobReference;

/// <summary>Passive optional assembly of SQLite catalog, verified upload and authorized reads.</summary>
public sealed class BlobReferenceModule
{
    public SqliteBlobCatalog Catalog { get; }
    public SqliteBlobUploadService Upload { get; }
    public SqlBlobResponseContributor Read { get; }
    public IBlobBindingAuthorization BindingAuthorization { get; }

    private sealed class BlobCapability(BlobReferenceModule module, UserContext owner,
        IBlobMutationParticipant participant) : IRuntimeCapabilityResource
    {
        public string Capability => "blob.v1";
        public async Task ValidateReadyAsync(UserContext context)
        {
            if (!ReferenceEquals(owner, context) ||
                !ReferenceEquals(context.GetResource<SqliteBlobCatalog>(), module.Catalog) ||
                !ReferenceEquals(context.GetResource<SqliteBlobUploadService>(), module.Upload) ||
                !ReferenceEquals(context.GetResource<IBlobResponseContributor>(), module.Read) ||
                !ReferenceEquals(context.GetResource<IBlobMutationParticipant>(), participant) ||
                context.GetResource<IObjectStorageProvider>() == null ||
                context.GetResource<ITransactionExecutor>() is not SqlDataServiceExecutor transactions ||
                transactions.Dialect.Kind != DatabaseKind.Sqlite ||
                !ReferenceEquals(transactions.Transport, module.Catalog.Transport))
                throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
            await module.Catalog.ValidateSchemaAsync().ConfigureAwait(false);
            using var transaction = await transactions.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                if (transaction is not IBlobMutationOwner || transaction is not IPhysicalSqlOwner physical)
                    throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
                await physical.WithPhysicalSqlAsync(async lease =>
                {
                    await lease.FetchAllAsync("SELECT key,scope,status FROM teaql_blob_catalog WHERE 1 = 0",
                        Array.Empty<TeaQL.Core.Value>()).ConfigureAwait(false);
                    await lease.FetchAllAsync("SELECT scope,entity,entity_id,field,key,version FROM teaql_blob_binding WHERE 1 = 0",
                        Array.Empty<TeaQL.Core.Value>()).ConfigureAwait(false);
                    return true;
                }).ConfigureAwait(false);
            }
            finally { await transaction.RollbackAsync().ConfigureAwait(false); }
        }
    }

    public BlobReferenceModule(SqliteBlobCatalog catalog, SqliteBlobUploadService upload,
        IBlobReadAuthorization readAuthorization, IBlobBindingAuthorization bindingAuthorization,
        int maximumUrlSeconds = 600)
    {
        Catalog = catalog ?? throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        Upload = upload ?? throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        if (!ReferenceEquals(upload.Catalog, catalog) || readAuthorization == null ||
            bindingAuthorization == null)
            throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        BindingAuthorization = bindingAuthorization;
        Read = new SqlBlobResponseContributor(catalog, readAuthorization, maximumUrlSeconds);
    }

    public void Install(UserContext context)
    {
        if (context == null || context.GetResource<SqliteBlobCatalog>() != null ||
            context.GetResource<SqliteBlobUploadService>() != null ||
            context.GetResource<IBlobResponseContributor>() != null ||
            context.GetResource<IBlobMutationParticipant>() != null ||
            context.GetNamedResource<IRuntimeCapabilityResource>("teaql.runtime.capability:blob.v1") != null)
            throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        var participant = new SqliteBlobMutationParticipant(Catalog, BindingAuthorization, context);
        var capability = new BlobCapability(this, context, participant);
        try { context.InstallRuntimeCapability(capability); }
        catch { throw new InvalidOperationException("BLOB_CAPABILITY_MISSING"); }
        context.InsertNamedResource<IRuntimeCapabilityResource>("teaql.runtime.capability:blob.v1", capability);
        context.InsertResource(Catalog);
        context.InsertResource(Upload);
        context.InsertResource<IBlobResponseContributor>(Read);
        context.InsertResource<IBlobMutationParticipant>(participant);
    }

    public Task ReadyAsync(UserContext context)
    {
        var capability = context?.GetNamedResource<IRuntimeCapabilityResource>("teaql.runtime.capability:blob.v1") as BlobCapability;
        if (capability == null) throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        return capability.ValidateReadyAsync(context!);
    }
}
