using TeaQL.Core.Storage;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;

namespace TeaQL.BlobReference;

public interface IBlobReadAuthorization
{
    Task<bool> AuthorizeAsync(UserContext context, AuthorizedBlobSlot slot);
}

/// <summary>Reauthorizes and rereads each binding before asking the configured store for a full URL.</summary>
public sealed class SqlBlobResponseContributor : IBlobResponseContributor
{
    private readonly SqliteBlobCatalog _catalog;
    private readonly IBlobReadAuthorization _authorization;
    private readonly int _maximumUrlSeconds;

    public SqlBlobResponseContributor(SqliteBlobCatalog catalog,
        IBlobReadAuthorization authorization, int maximumUrlSeconds = 600)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(authorization);
        if (maximumUrlSeconds is < 1 or > 3600) throw new ArgumentException("BLOB_POLICY_INVALID");
        _catalog = catalog;
        _authorization = authorization;
        _maximumUrlSeconds = maximumUrlSeconds;
    }

    public async Task<IReadOnlyList<ProjectedBlobUrl>> ProjectBatchAsync(UserContext context,
        AuthorizedResponseGraph graph, DateTimeOffset? expiresAt)
    {
        var results = new List<ProjectedBlobUrl>();
        foreach (var slot in graph.BlobSlots)
        {
            bool allowed;
            try { allowed = await _authorization.AuthorizeAsync(context, slot); }
            catch { throw new ViewPreparationException("BLOB_READ_DENIED"); }
            if (!allowed) throw new ViewPreparationException("BLOB_READ_DENIED");
            BlobCatalogRead record;
            try { record = await _catalog.CurrentReadAsync(slot); }
            catch (InvalidOperationException error) when (error.Message is "BLOB_READ_DENIED" or "BLOB_CATALOG_FAILED")
            { throw new ViewPreparationException(error.Message); }
            var seconds = expiresAt is { } ceiling
                ? Math.Min(_maximumUrlSeconds, (int)Math.Floor((ceiling - DateTimeOffset.UtcNow).TotalSeconds) - 1)
                : _maximumUrlSeconds;
            if (seconds < 1) throw new ViewPreparationException("BLOB_REFERENCE_EXPIRED");
            var disposition = record.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                || record.ContentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)
                || record.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
                || record.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase)
                ? "inline" : "attachment";
            try
            {
                var signed = await context.ObjectStorage("prepare authorized Blob view").GetReadUrlAsync(
                    new ObjectKey(record.ObjectKey),
                    new ObjectReadUrlOptions(TimeSpan.FromSeconds(seconds), disposition, record.FileName));
                results.Add(new ProjectedBlobUrl(signed.Url.AbsoluteUri, signed.ExpiresAt));
            }
            catch { throw new ViewPreparationException("BLOB_SIGN_FAILED"); }
        }
        return results;
    }
}
