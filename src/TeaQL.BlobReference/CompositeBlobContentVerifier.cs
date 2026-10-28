namespace TeaQL.BlobReference;

/// <summary>Passive host-selected routes for independently optional parsers.</summary>
public sealed class CompositeBlobContentVerifier : IBlobContentVerifier, IBlobVerifierReadiness
{
    private readonly IReadOnlyDictionary<string, IBlobContentVerifier> _routes;
    private readonly IReadOnlyList<IBlobContentVerifier> _ready;

    public CompositeBlobContentVerifier(IReadOnlyDictionary<string, IBlobContentVerifier> routes)
    {
        if (routes == null || routes.Count is < 1 or > 32)
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        var selected = new Dictionary<string, IBlobContentVerifier>(StringComparer.Ordinal);
        var ready = new HashSet<IBlobContentVerifier>(ReferenceEqualityComparer.Instance);
        foreach (var (extension, verifier) in routes)
        {
            if (extension.Length is < 1 or > 16 ||
                extension.Any(character => character is not (>= 'a' and <= 'z') and not (>= '0' and <= '9')) ||
                verifier == null || !selected.TryAdd(extension, verifier))
                throw new InvalidOperationException("BLOB_POLICY_INVALID");
            ready.Add(verifier);
        }
        _routes = selected;
        _ready = ready.ToArray();
    }

    public IReadOnlyList<string> SupportedExtensions => _routes.Keys.Order(StringComparer.Ordinal).ToArray();

    public async Task ValidateReadyAsync()
    {
        foreach (var verifier in _ready)
            if (verifier is IBlobVerifierReadiness readiness)
                await readiness.ValidateReadyAsync().ConfigureAwait(false);
    }

    public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
        long maximumBytes)
    {
        if (!_routes.TryGetValue(extension, out var verifier))
            throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
        return await verifier.VerifyAsync(content, extension, maximumBytes).ConfigureAwait(false);
    }
}
