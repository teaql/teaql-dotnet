namespace TeaQL.Core;

/// <summary>Trusted model restrictions; server policy may narrow formats.</summary>
public sealed class BlobFieldSpec
{
    public const string Capability = "blob.v1";
    private static readonly IReadOnlyDictionary<string, string> Mime = new Dictionary<string, string>
    {
        ["jpg"] = "image/jpeg", ["jpeg"] = "image/jpeg", ["png"] = "image/png",
        ["mp4"] = "video/mp4", ["mp3"] = "audio/mpeg",
        ["doc"] = "application/msword",
        ["docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        ["xls"] = "application/vnd.ms-excel",
        ["xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        ["pdf"] = "application/pdf"
    };

    /// <summary>Resolve only the frozen Blob format set, never a client MIME hint.</summary>
    public static string? MimeForExtension(string extension) =>
        extension is not null && Mime.TryGetValue(extension, out var value) ? value : null;

    public string Entity { get; }
    public string Field { get; }
    public bool Nullable { get; }
    public string PolicyProfile { get; }
    public IReadOnlyList<string> AllowedExtensions { get; }
    public IReadOnlyList<string> AllowedContentTypes { get; }
    public string ProtocolVersion => Capability;
    public int MaxFiles => 1;

    public BlobFieldSpec(string entity, string field, IEnumerable<string> extensions,
        bool nullable, string policyProfile)
    {
        if (string.IsNullOrWhiteSpace(entity) || entity != entity.Trim()
            || string.IsNullOrWhiteSpace(field) || field != field.Trim()
            || string.IsNullOrWhiteSpace(policyProfile) || policyProfile != policyProfile.Trim())
            throw new ArgumentException("BLOB_POLICY_INVALID");
        ArgumentNullException.ThrowIfNull(extensions);
        var normalized = extensions.Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        if (normalized.Length == 0 || normalized.Any(x => x is null || !Mime.ContainsKey(x)))
            throw new ArgumentException("BLOB_FORMAT_NOT_ALLOWED");
        Entity = entity;
        Field = field;
        Nullable = nullable;
        PolicyProfile = policyProfile;
        AllowedExtensions = Array.AsReadOnly(normalized);
        AllowedContentTypes = Array.AsReadOnly(normalized.Select(x => Mime[x]).Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal).ToArray());
    }
}
