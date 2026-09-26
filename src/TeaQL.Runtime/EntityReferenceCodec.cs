using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TeaQL.Runtime;

public sealed record EntityReferenceClaims(
    string EntityType, ulong Id, long Version, DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt, string Purpose, uint KeyVersion = 0);

public sealed class EntityReferenceTokenException(string code) : Exception(code)
{
    public string Code { get; } = code;
}

public interface IEntityReferenceCodec
{
    string Encode(string entityType, ulong id, long version, string purpose, TimeSpan lifetime);
    EntityReferenceClaims Decode(string token, string expectedEntityType, string purpose);
}

public sealed class AeadEntityReferenceCodec : IEntityReferenceCodec
{
    public const string AssociatedData = "teaql.entity-reference.v1";
    private const string Prefix = "tqr1.";
    private readonly uint _activeKeyVersion;
    private readonly IReadOnlyDictionary<uint, byte[]> _keys;
    private Func<DateTimeOffset> _clock = () => DateTimeOffset.UtcNow;
    private Func<byte[]> _nonceSource = () => RandomNumberGenerator.GetBytes(12);

    public AeadEntityReferenceCodec(uint activeKeyVersion, IReadOnlyDictionary<uint, byte[]> keys)
    {
        if (!keys.TryGetValue(activeKeyVersion, out var active) || active.Length != 32)
            throw new ArgumentException("Active entity reference key must contain 32 bytes", nameof(keys));
        if (keys.Any(pair => pair.Value.Length != 32))
            throw new ArgumentException("Every entity reference key must contain 32 bytes", nameof(keys));
        _activeKeyVersion = activeKeyVersion;
        _keys = keys.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
    }

    public AeadEntityReferenceCodec WithClock(Func<DateTimeOffset> clock) { _clock = clock; return this; }
    public AeadEntityReferenceCodec WithNonceSource(Func<byte[]> source) { _nonceSource = source; return this; }

    public string Encode(string entityType, ulong id, long version, string purpose, TimeSpan lifetime)
    {
        if (string.IsNullOrWhiteSpace(entityType) || id == 0 || lifetime <= TimeSpan.Zero)
            throw new EntityReferenceTokenException("ENTITY_REFERENCE_INVALID");
        var now = _clock().ToUniversalTime();
        var plain = EncodeClaims(new EntityReferenceClaims(entityType, id, version, now, now.Add(lifetime), purpose));
        var nonce = _nonceSource();
        if (nonce.Length != 12) throw new ArgumentException("AES-GCM nonce must contain 12 bytes");
        var cipher = new byte[plain.Length];
        var tag = new byte[16];
        using (var aes = new AesGcm(_keys[_activeKeyVersion], 16))
            aes.Encrypt(nonce, plain, cipher, tag, Encoding.UTF8.GetBytes(AssociatedData));
        var envelope = new byte[4 + nonce.Length + cipher.Length + tag.Length];
        BinaryPrimitives.WriteUInt32BigEndian(envelope.AsSpan(0, 4), _activeKeyVersion);
        nonce.CopyTo(envelope, 4); cipher.CopyTo(envelope, 16); tag.CopyTo(envelope, 16 + cipher.Length);
        return Prefix + Base64UrlEncode(envelope);
    }

    public EntityReferenceClaims Decode(string token, string expectedEntityType, string purpose)
    {
        try
        {
            if (!token.StartsWith(Prefix, StringComparison.Ordinal)) throw Invalid();
            var envelope = Base64UrlDecode(token[Prefix.Length..]);
            if (envelope.Length < 32) throw Invalid();
            var keyVersion = BinaryPrimitives.ReadUInt32BigEndian(envelope.AsSpan(0, 4));
            if (!_keys.TryGetValue(keyVersion, out var key)) throw Invalid();
            var cipherLength = envelope.Length - 4 - 12 - 16;
            if (cipherLength <= 0) throw Invalid();
            var plain = new byte[cipherLength];
            using (var aes = new AesGcm(key, 16))
                aes.Decrypt(envelope.AsSpan(4, 12), envelope.AsSpan(16, cipherLength),
                    envelope.AsSpan(16 + cipherLength, 16), plain, Encoding.UTF8.GetBytes(AssociatedData));
            var claims = DecodeClaims(plain) with { KeyVersion = keyVersion };
            var now = _clock().ToUniversalTime();
            if (claims.ExpiresAt <= now || claims.IssuedAt > now.AddMinutes(1)
                || !string.Equals(claims.EntityType, expectedEntityType, StringComparison.Ordinal)
                || !string.Equals(claims.Purpose, purpose, StringComparison.Ordinal)) throw Invalid();
            return claims;
        }
        catch (EntityReferenceTokenException) { throw; }
        catch { throw Invalid(); }
    }

    internal static byte[] EncodeClaims(EntityReferenceClaims claims)
    {
        using var stream = new MemoryStream();
        WriteText(stream, claims.EntityType);
        Span<byte> number = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(number, claims.Id); stream.Write(number);
        BinaryPrimitives.WriteInt64BigEndian(number, claims.Version); stream.Write(number);
        BinaryPrimitives.WriteInt64BigEndian(number, claims.IssuedAt.ToUnixTimeSeconds()); stream.Write(number);
        BinaryPrimitives.WriteInt64BigEndian(number, claims.ExpiresAt.ToUnixTimeSeconds()); stream.Write(number);
        WriteText(stream, claims.Purpose);
        return stream.ToArray();
    }

    internal static EntityReferenceClaims DecodeClaims(byte[] data)
    {
        var offset = 0;
        var entityType = ReadText(data, ref offset);
        if (offset + 32 > data.Length) throw Invalid();
        var id = BinaryPrimitives.ReadUInt64BigEndian(data.AsSpan(offset, 8)); offset += 8;
        var version = BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(offset, 8)); offset += 8;
        var issued = BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(offset, 8)); offset += 8;
        var expires = BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(offset, 8)); offset += 8;
        var purpose = ReadText(data, ref offset);
        if (offset != data.Length || string.IsNullOrEmpty(entityType) || id == 0) throw Invalid();
        return new EntityReferenceClaims(entityType, id, version,
            DateTimeOffset.FromUnixTimeSeconds(issued), DateTimeOffset.FromUnixTimeSeconds(expires), purpose);
    }

    private static void WriteText(Stream stream, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length > ushort.MaxValue) throw new ArgumentException("Entity reference text exceeds 65535 bytes");
        Span<byte> length = stackalloc byte[2]; BinaryPrimitives.WriteUInt16BigEndian(length, (ushort)bytes.Length);
        stream.Write(length); stream.Write(bytes);
    }
    private static string ReadText(byte[] data, ref int offset)
    {
        if (offset + 2 > data.Length) throw Invalid();
        var length = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset, 2)); offset += 2;
        if (offset + length > data.Length) throw Invalid();
        var result = Encoding.UTF8.GetString(data, offset, length); offset += length; return result;
    }
    internal static string Base64UrlEncode(byte[] data) => Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    internal static byte[] Base64UrlDecode(string text) => Convert.FromBase64String(text.Replace('-', '+').Replace('_', '/') + new string('=', (4 - text.Length % 4) % 4));
    private static EntityReferenceTokenException Invalid() => new("ENTITY_REFERENCE_INVALID");
}

internal static class DevelopmentRawEntityReferenceCodec
{
    internal const string EnvironmentName = "TEAQL_UNSAFE_RAW_ENTITY_REFERENCES";
    internal const string Acknowledgement = "I_UNDERSTAND_RAW_ENTITY_IDS_ARE_VISIBLE_FOR_LOCAL_DEVELOPMENT_ONLY";
    private const string Prefix = "tqr0.";
    internal static bool Enabled => Environment.GetEnvironmentVariable(EnvironmentName) == Acknowledgement;
    internal static string Encode(EntityReferenceClaims claims) => Prefix + AeadEntityReferenceCodec.Base64UrlEncode(AeadEntityReferenceCodec.EncodeClaims(claims));
    internal static EntityReferenceClaims Decode(string token)
    {
        if (!token.StartsWith(Prefix, StringComparison.Ordinal)) throw new EntityReferenceTokenException("ENTITY_REFERENCE_INVALID");
        return AeadEntityReferenceCodec.DecodeClaims(AeadEntityReferenceCodec.Base64UrlDecode(token[Prefix.Length..]));
    }
}
