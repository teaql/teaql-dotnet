using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace TeaQL.SecureDocumentReference;

public sealed class ReferenceException(string code) : Exception(code)
{
    public string Code { get; } = code;
}

public sealed record TrustedReferencePrincipal(string AuthenticationRealm, string Subject,
    string DomainRootType, ulong DomainRootId);
public sealed record ReferenceDocumentScope(string DocumentId, string Purpose,
    string AggregateType, ulong AggregateId, long AggregateRevision);
public sealed record ReferenceIdentity(string EntityType, ulong Id, long Version);
public sealed record VerifiedEntityReference(ReferenceIdentity Identity, string KeyId,
    DateTimeOffset IssuedAt, DateTimeOffset ExpiresAt);

public sealed class ReferenceKey
{
    public string KeyId { get; }
    private readonly byte[] _master;
    public ReferenceKey(string keyId, byte[] master)
    {
        if (!ReferenceWire.ValidKeyId(keyId) || master is not { Length: 32 })
            throw new ReferenceException("ROUND_TRIP_REFERENCE_CONFIGURATION_INVALID");
        KeyId = keyId; _master = master.ToArray();
    }
    internal byte[] Master() => _master.ToArray();
    public override string ToString() => $"ReferenceKey({KeyId},[REDACTED])";
}

public interface IReferenceKeyProvider
{
    ReferenceKey CurrentKey();
    ReferenceKey? KeyById(string keyId);
}

public sealed class StaticReferenceKeyProvider : IReferenceKeyProvider
{
    private readonly string _current;
    private readonly IReadOnlyDictionary<string, ReferenceKey> _keys;
    public StaticReferenceKeyProvider(string current, params ReferenceKey[] keys)
    {
        if (!ReferenceWire.ValidKeyId(current) || keys.Select(k => k.KeyId).Distinct(StringComparer.Ordinal).Count() != keys.Length)
            throw new ReferenceException("ROUND_TRIP_REFERENCE_CONFIGURATION_INVALID");
        _current = current; _keys = keys.ToDictionary(k => k.KeyId, StringComparer.Ordinal);
        if (!_keys.ContainsKey(current)) throw new ReferenceException("ROUND_TRIP_REFERENCE_CONFIGURATION_INVALID");
    }
    public ReferenceKey CurrentKey() => _keys[_current];
    public ReferenceKey? KeyById(string keyId) => _keys.GetValueOrDefault(keyId);
}

internal static class ReferenceWire
{
    internal static ReferenceException Invalid() => new("ROUND_TRIP_REFERENCE_INVALID");
    internal static ReferenceException Config() => new("ROUND_TRIP_REFERENCE_CONFIGURATION_INVALID");
    internal static ReferenceException Expired() => new("ROUND_TRIP_REFERENCE_EXPIRED");
    internal static ReferenceException Mismatch() => new("ROUND_TRIP_REFERENCE_SCOPE_MISMATCH");
    internal static ReferenceException Denied() => new("ROUND_TRIP_REFERENCE_AUTHORIZATION_REQUIRED");
    internal static bool ValidKeyId(string? value) => value is { Length: >= 1 and <= 64 } &&
        value.All(c => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '-' or '_');
    internal static void Text(string? value, int maximum = 4096)
    {
        if (string.IsNullOrEmpty(value) || value != value.Trim()) throw Invalid();
        try { if (new UTF8Encoding(false, true).GetByteCount(value) > maximum) throw Invalid(); }
        catch (EncoderFallbackException) { throw Invalid(); }
    }
    internal static void WriteText16(Stream stream, string text)
    {
        Text(text); var bytes = Encoding.UTF8.GetBytes(text);
        Span<byte> length = stackalloc byte[2]; BinaryPrimitives.WriteUInt16BigEndian(length, checked((ushort)bytes.Length));
        stream.Write(length); stream.Write(bytes);
    }
    internal static void WriteU64(Stream stream, ulong value)
    {
        Span<byte> data = stackalloc byte[8]; BinaryPrimitives.WriteUInt64BigEndian(data, value); stream.Write(data);
    }
    internal static void WriteI64(Stream stream, long value) => WriteU64(stream, unchecked((ulong)value));
    internal static byte[] ReadExactly(Stream stream, int count)
    {
        var bytes = new byte[count]; var offset = 0;
        while (offset < count)
        {
            var read = stream.Read(bytes, offset, count - offset);
            if (read == 0) throw Invalid(); offset += read;
        }
        return bytes;
    }
    internal static string ReadText16(Stream stream)
    {
        var length = BinaryPrimitives.ReadUInt16BigEndian(ReadExactly(stream, 2));
        string text;
        try { text = new UTF8Encoding(false, true).GetString(ReadExactly(stream, length)); }
        catch (DecoderFallbackException) { throw Invalid(); }
        Text(text); return text;
    }
    internal static ulong ReadU64(Stream stream) => BinaryPrimitives.ReadUInt64BigEndian(ReadExactly(stream, 8));
    internal static long ReadI64(Stream stream) => unchecked((long)ReadU64(stream));
    internal static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    internal static byte[] DecodeBase64Url(string text)
    {
        try
        {
            if (text.Length == 0 || text.Any(c => !(char.IsAsciiLetterOrDigit(c) || c is '-' or '_'))) throw Invalid();
            var bytes = Convert.FromBase64String(text.Replace('-', '+').Replace('_', '/') + new string('=', (4 - text.Length % 4) % 4));
            if (Base64Url(bytes) != text) throw Invalid(); return bytes;
        }
        catch (FormatException) { throw Invalid(); }
    }
    internal static byte[] Derive(ReferenceKey key, string label, string service, string environment)
    {
        using var info = new MemoryStream();
        foreach (var value in new[] { label, service, environment, key.KeyId }) WriteText16(info, value);
        var master = key.Master();
        var prk = HMACSHA256.HashData(Encoding.ASCII.GetBytes("teaql.round-trip-reference.v1"), master);
        var input = info.ToArray().Concat(new byte[] { 1 }).ToArray();
        var expanded = HMACSHA256.HashData(prk, input);
        CryptographicOperations.ZeroMemory(master); CryptographicOperations.ZeroMemory(prk);
        return expanded;
    }
    internal static void Principal(TrustedReferencePrincipal principal)
    {
        Text(principal.AuthenticationRealm); Text(principal.Subject); Text(principal.DomainRootType);
        if (principal.DomainRootId == 0) throw Invalid();
    }
    internal static void Scope(ReferenceDocumentScope scope)
    {
        Text(scope.DocumentId); Text(scope.Purpose); Text(scope.AggregateType);
        if (scope.AggregateId == 0 || scope.AggregateRevision < 0) throw Invalid();
    }
    internal static void Identity(ReferenceIdentity identity)
    {
        Text(identity.EntityType); if (identity.Id == 0 || identity.Version < 0) throw Invalid();
    }
}

/// <summary>Independent governed tqr1 codec; keys and actor come from trusted host code.</summary>
public class ContextBoundReferenceRuntime
{
    private readonly string _service, _environment;
    private readonly IReferenceKeyProvider _keys;
    private readonly Func<TrustedReferencePrincipal, ReferenceDocumentScope, ReferenceIdentity, bool> _authorize;
    private readonly Func<DateTimeOffset> _clock;
    private readonly Func<byte[]> _nonce;

    public ContextBoundReferenceRuntime(string service, string environment, IReferenceKeyProvider keys,
        Func<TrustedReferencePrincipal, ReferenceDocumentScope, ReferenceIdentity, bool> authorize,
        Func<DateTimeOffset>? clock = null, Func<byte[]>? nonce = null)
    {
        try { ReferenceWire.Text(service); ReferenceWire.Text(environment); }
        catch (ReferenceException) { throw ReferenceWire.Config(); }
        _service = service; _environment = environment;
        _keys = keys ?? throw ReferenceWire.Config();
        _authorize = authorize ?? throw ReferenceWire.Config();
        if (_keys.CurrentKey() is not { } key || !ReferenceWire.ValidKeyId(key.KeyId)) throw ReferenceWire.Config();
        _clock = clock ?? (() => DateTimeOffset.UtcNow);
        _nonce = nonce ?? (() => RandomNumberGenerator.GetBytes(12));
    }

    internal ReferenceKey CurrentKey() => _keys.CurrentKey();
    internal ReferenceKey? KeyById(string id) => _keys.KeyById(id);
    internal DateTimeOffset Now() => _clock();
    internal byte[] Nonce() => _nonce();
    internal string Service => _service;
    internal string EnvironmentName => _environment;
    internal void Authorize(TrustedReferencePrincipal principal, ReferenceDocumentScope scope, ReferenceIdentity identity)
    {
        bool allowed;
        try { allowed = _authorize(principal, scope, identity); }
        catch { throw ReferenceWire.Denied(); }
        if (!allowed) throw ReferenceWire.Denied();
    }
    internal byte[] Actor(ReferenceKey key, TrustedReferencePrincipal principal)
    {
        using var canonical = new MemoryStream();
        ReferenceWire.WriteText16(canonical, principal.AuthenticationRealm);
        ReferenceWire.WriteText16(canonical, principal.Subject);
        ReferenceWire.WriteText16(canonical, principal.DomainRootType);
        ReferenceWire.WriteU64(canonical, principal.DomainRootId);
        var secret = ReferenceWire.Derive(key, "actor-fingerprint", _service, _environment);
        var result = HMACSHA256.HashData(secret, canonical.ToArray());
        CryptographicOperations.ZeroMemory(secret); return result;
    }
    internal byte[] Aad(string prefix, ReferenceKey key, string purpose)
    {
        using var stream = new MemoryStream();
        foreach (var value in new[] { prefix, key.KeyId, _service, _environment, purpose })
            if (prefix == "tqr1") ReferenceWire.WriteText16(stream, value);
            else DocumentWire.WriteText(stream, value);
        return stream.ToArray();
    }
    internal string Seal(string prefix, string label, ReferenceKey key, string purpose, byte[] payload, int maxLength)
    {
        var nonce = Nonce(); if (nonce.Length != 12) throw ReferenceWire.Config();
        var ciphertext = new byte[payload.Length]; var tag = new byte[16];
        var secret = ReferenceWire.Derive(key, label, _service, _environment);
        try { using var aes = new AesGcm(secret, 16); aes.Encrypt(nonce, payload, ciphertext, tag, Aad(prefix, key, purpose)); }
        finally { CryptographicOperations.ZeroMemory(secret); }
        var envelope = nonce.Concat(ciphertext).Concat(tag).ToArray();
        var token = prefix + "." + key.KeyId + "." + ReferenceWire.Base64Url(envelope);
        if (token.Length > maxLength) throw ReferenceWire.Invalid(); return token;
    }
    internal (ReferenceKey Key, byte[] Plain) Open(string prefix, string label, string purpose, string token, int maxLength)
    {
        if (token == null || token.Length > maxLength) throw ReferenceWire.Invalid();
        var parts = token.Split('.');
        if (parts.Length != 3 || parts[0] != prefix || !ReferenceWire.ValidKeyId(parts[1])) throw ReferenceWire.Invalid();
        var key = KeyById(parts[1]); if (key == null || key.KeyId != parts[1]) throw ReferenceWire.Invalid();
        var envelope = ReferenceWire.DecodeBase64Url(parts[2]); if (envelope.Length < 28) throw ReferenceWire.Invalid();
        var plaintext = new byte[envelope.Length - 28];
        var secret = ReferenceWire.Derive(key, label, _service, _environment);
        try
        {
            using var aes = new AesGcm(secret, 16);
            aes.Decrypt(envelope.AsSpan(0, 12), envelope.AsSpan(12, plaintext.Length),
                envelope.AsSpan(12 + plaintext.Length, 16), plaintext, Aad(prefix, key, purpose));
        }
        catch (CryptographicException) { throw ReferenceWire.Invalid(); }
        finally { CryptographicOperations.ZeroMemory(secret); }
        return (key, plaintext);
    }
    internal (long Issued, long Expires) Times(TimeSpan lifetime)
    {
        if (lifetime < TimeSpan.FromSeconds(1) || lifetime.Ticks % TimeSpan.TicksPerSecond != 0)
            throw ReferenceWire.Invalid();
        var issued = Now().ToUnixTimeSeconds(); var seconds = checked((long)lifetime.TotalSeconds);
        return (issued, checked(issued + seconds));
    }

    public string Issue(TrustedReferencePrincipal principal, ReferenceDocumentScope scope,
        ReferenceIdentity identity, TimeSpan lifetime)
    {
        ReferenceWire.Principal(principal); ReferenceWire.Scope(scope); ReferenceWire.Identity(identity);
        Authorize(principal, scope, identity);
        var key = CurrentKey(); var (issued, expires) = Times(lifetime);
        using var stream = new MemoryStream();
        stream.Write(Actor(key, principal)); ReferenceWire.WriteText16(stream, identity.EntityType);
        ReferenceWire.WriteU64(stream, identity.Id); ReferenceWire.WriteI64(stream, identity.Version);
        ReferenceWire.WriteText16(stream, scope.DocumentId); ReferenceWire.WriteText16(stream, scope.AggregateType);
        ReferenceWire.WriteU64(stream, scope.AggregateId); ReferenceWire.WriteI64(stream, scope.AggregateRevision);
        ReferenceWire.WriteI64(stream, issued); ReferenceWire.WriteI64(stream, expires);
        return Seal("tqr1", "entity-reference-encryption", key, scope.Purpose, stream.ToArray(), 24000);
    }
    public VerifiedEntityReference Consume(TrustedReferencePrincipal principal, ReferenceDocumentScope scope,
        string token, string expectedEntityType)
    {
        ReferenceWire.Principal(principal); ReferenceWire.Scope(scope); ReferenceWire.Text(expectedEntityType);
        var (key, plaintext) = Open("tqr1", "entity-reference-encryption", scope.Purpose, token, 24000);
        using var stream = new MemoryStream(plaintext);
        var actor = ReferenceWire.ReadExactly(stream, 32);
        var identity = new ReferenceIdentity(ReferenceWire.ReadText16(stream), ReferenceWire.ReadU64(stream), ReferenceWire.ReadI64(stream));
        var documentId = ReferenceWire.ReadText16(stream); var aggregateType = ReferenceWire.ReadText16(stream);
        var aggregateId = ReferenceWire.ReadU64(stream); var revision = ReferenceWire.ReadI64(stream);
        var issued = ReferenceWire.ReadI64(stream); var expires = ReferenceWire.ReadI64(stream);
        if (stream.Position != stream.Length) throw ReferenceWire.Invalid();
        ReferenceWire.Identity(identity);
        var now = Now().ToUnixTimeSeconds(); if (expires <= now) throw ReferenceWire.Expired();
        if (issued > now + 60) throw ReferenceWire.Invalid();
        if (!CryptographicOperations.FixedTimeEquals(actor, Actor(key, principal)) ||
            documentId != scope.DocumentId || aggregateType != scope.AggregateType ||
            aggregateId != scope.AggregateId || revision != scope.AggregateRevision ||
            identity.EntityType != expectedEntityType) throw ReferenceWire.Mismatch();
        Authorize(principal, scope, identity);
        return new VerifiedEntityReference(identity, key.KeyId,
            DateTimeOffset.FromUnixTimeSeconds(issued), DateTimeOffset.FromUnixTimeSeconds(expires));
    }
}
