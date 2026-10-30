using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using TeaQL.Core;

namespace TeaQL.SecureDocumentReference;

public sealed record DocumentEntitySnapshot(string RowKey, ReferenceIdentity Identity,
    IReadOnlyList<string> LoadedFields, IReadOnlyList<string> WritableFields,
    IReadOnlyList<string> AllowedMutations);
public sealed record DocumentRelationSnapshot(string Name, IReadOnlyList<string> RowKeys,
    string Completeness, bool Writable, IReadOnlyList<string> AllowedMutations);
public sealed record DocumentSnapshot(string ModelFingerprint, string BusinessId,
    ReferenceDocumentScope Scope, ReferenceIdentity Aggregate,
    IReadOnlyList<DocumentEntitySnapshot> Entities, IReadOnlyList<DocumentRelationSnapshot> Relations);
public sealed record DocumentBlobBindingSnapshot(string RowKey, string CanonicalField,
    string EntityType, BlobRef? Reference, ulong BindingVersion, bool Nullable, bool Writable);
public sealed record BoundDocumentSnapshot(DocumentSnapshot Snapshot,
    IReadOnlyList<DocumentBlobBindingSnapshot> Bindings);
public sealed record BlobSelectionTarget(ReferenceDocumentScope Scope, string TargetEntity,
    string CanonicalField, string Kind, string TargetKey, string CommandId);
public sealed record BlobSelectionClaim(BlobSelectionTarget Target, BlobRef Reference);
public sealed record VerifiedDocument<T>(T Document, string KeyId,
    DateTimeOffset IssuedAt, DateTimeOffset ExpiresAt);

internal static class DocumentWire
{
    internal static ReferenceException Projection() => new("DOCUMENT_PROJECTION_VIOLATION");
    internal static void Text(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value!.Length > 16384) throw ReferenceWire.Invalid();
        try { _ = new UTF8Encoding(false, true).GetByteCount(value); }
        catch (EncoderFallbackException) { throw ReferenceWire.Invalid(); }
    }
    internal static void Count(Stream stream, int count)
    {
        Span<byte> data = stackalloc byte[4]; BinaryPrimitives.WriteUInt32BigEndian(data, checked((uint)count)); stream.Write(data);
    }
    internal static void WriteText(Stream stream, string value)
    {
        Text(value); var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length > 16384) throw ReferenceWire.Invalid();
        Count(stream, bytes.Length); stream.Write(bytes);
    }
    internal static int ReadCount(Stream stream, int maximum)
    {
        var count = BinaryPrimitives.ReadUInt32BigEndian(ReferenceWire.ReadExactly(stream, 4));
        if (count > maximum) throw ReferenceWire.Invalid(); return checked((int)count);
    }
    internal static string ReadText(Stream stream)
    {
        var length = ReadCount(stream, 16384);
        string text;
        try { text = new UTF8Encoding(false, true).GetString(ReferenceWire.ReadExactly(stream, length)); }
        catch (DecoderFallbackException) { throw ReferenceWire.Invalid(); }
        Text(text); return text;
    }
    internal static byte ReadByte(Stream stream)
    {
        var value = stream.ReadByte(); if (value < 0) throw ReferenceWire.Invalid(); return (byte)value;
    }
    internal static bool ReadBool(Stream stream) => ReadByte(stream) switch
    {
        0 => false, 1 => true, _ => throw ReferenceWire.Invalid()
    };
    internal static void WriteBool(Stream stream, bool value) => stream.WriteByte(value ? (byte)1 : (byte)0);
    internal static void WriteScope(Stream stream, ReferenceDocumentScope scope)
    {
        ReferenceWire.Scope(scope);
        WriteText(stream, scope.DocumentId); WriteText(stream, scope.Purpose); WriteText(stream, scope.AggregateType);
        ReferenceWire.WriteU64(stream, scope.AggregateId); ReferenceWire.WriteI64(stream, scope.AggregateRevision);
    }
    internal static ReferenceDocumentScope ReadScope(Stream stream)
    {
        var result = new ReferenceDocumentScope(ReadText(stream), ReadText(stream), ReadText(stream),
            ReferenceWire.ReadU64(stream), ReferenceWire.ReadI64(stream));
        ReferenceWire.Scope(result); return result;
    }
    internal static void WriteIdentity(Stream stream, ReferenceIdentity identity)
    {
        ReferenceWire.Identity(identity); WriteText(stream, identity.EntityType);
        ReferenceWire.WriteU64(stream, identity.Id); ReferenceWire.WriteI64(stream, identity.Version);
    }
    internal static ReferenceIdentity ReadIdentity(Stream stream)
    {
        var result = new ReferenceIdentity(ReadText(stream), ReferenceWire.ReadU64(stream), ReferenceWire.ReadI64(stream));
        ReferenceWire.Identity(result); return result;
    }
    private static readonly Dictionary<string, byte> MutationCodes = new(StringComparer.Ordinal)
    { ["UPDATE"] = 1, ["CREATE_CHILD"] = 2, ["REMOVE_CHILD"] = 3 };
    private static readonly string[] MutationNames = ["", "UPDATE", "CREATE_CHILD", "REMOVE_CHILD"];
    internal static void WriteSet(Stream stream, IReadOnlyList<string> values, int maximum)
    {
        if (values.Count > maximum || values.Distinct(StringComparer.Ordinal).Count() != values.Count)
            throw Projection();
        Count(stream, values.Count);
        foreach (var value in values.OrderBy(v => v, StringComparer.Ordinal)) WriteText(stream, value);
    }
    internal static IReadOnlyList<string> ReadSet(Stream stream, int maximum)
    {
        var count = ReadCount(stream, maximum); var values = new string[count];
        for (var i = 0; i < count; i++) values[i] = ReadText(stream);
        if (values.Distinct(StringComparer.Ordinal).Count() != values.Length) throw ReferenceWire.Invalid();
        return values;
    }
    internal static void WriteMutations(Stream stream, IReadOnlyList<string> values)
    {
        if (values.Count > 3 || values.Distinct(StringComparer.Ordinal).Count() != values.Count ||
            values.Any(v => !MutationCodes.ContainsKey(v))) throw Projection();
        Count(stream, values.Count);
        foreach (var code in values.Select(v => MutationCodes[v]).Order()) stream.WriteByte(code);
    }
    internal static IReadOnlyList<string> ReadMutations(Stream stream)
    {
        var count = ReadCount(stream, 3); var values = new string[count];
        for (var i = 0; i < count; i++)
        {
            var code = ReadByte(stream); if (code is < 1 or > 3) throw ReferenceWire.Invalid();
            values[i] = MutationNames[code];
        }
        if (values.Distinct(StringComparer.Ordinal).Count() != count) throw ReferenceWire.Invalid();
        return values;
    }
    internal static byte[] EncodeSnapshot(DocumentSnapshot snapshot, byte version, byte[] actor, long issued, long expires)
    {
        ReferenceWire.Scope(snapshot.Scope); ReferenceWire.Identity(snapshot.Aggregate);
        if (snapshot.Aggregate.EntityType != snapshot.Scope.AggregateType || snapshot.Aggregate.Id != snapshot.Scope.AggregateId ||
            snapshot.Aggregate.Version != snapshot.Scope.AggregateRevision || snapshot.Entities.Count > 10000 || snapshot.Relations.Count > 1024)
            throw Projection();
        using var stream = new MemoryStream(); stream.WriteByte(version); stream.Write(actor);
        ReferenceWire.WriteI64(stream, issued); ReferenceWire.WriteI64(stream, expires);
        WriteText(stream, snapshot.ModelFingerprint); WriteText(stream, snapshot.BusinessId);
        WriteScope(stream, snapshot.Scope); WriteIdentity(stream, snapshot.Aggregate);
        var rows = new HashSet<string>(StringComparer.Ordinal); Count(stream, snapshot.Entities.Count);
        foreach (var entity in snapshot.Entities)
        {
            if (!rows.Add(entity.RowKey) || entity.AllowedMutations.Count == 0 ||
                entity.WritableFields.Except(entity.LoadedFields, StringComparer.Ordinal).Any()) throw Projection();
            WriteText(stream, entity.RowKey); WriteIdentity(stream, entity.Identity);
            WriteSet(stream, entity.LoadedFields, 1024); WriteSet(stream, entity.WritableFields, 1024);
            WriteMutations(stream, entity.AllowedMutations);
        }
        Count(stream, snapshot.Relations.Count);
        foreach (var relation in snapshot.Relations)
        {
            if (relation.RowKeys.Count > 10000 || relation.RowKeys.Distinct(StringComparer.Ordinal).Count() != relation.RowKeys.Count ||
                relation.RowKeys.Any(row => !rows.Contains(row)) || !relation.Writable && relation.AllowedMutations.Count != 0)
                throw Projection();
            WriteText(stream, relation.Name);
            stream.WriteByte(relation.Completeness switch { "COMPLETE" => (byte)1, "PARTIAL" => (byte)2, _ => throw Projection() });
            WriteBool(stream, relation.Writable); WriteMutations(stream, relation.AllowedMutations);
            Count(stream, relation.RowKeys.Count); foreach (var row in relation.RowKeys) WriteText(stream, row);
        }
        return stream.ToArray();
    }
    internal static (DocumentSnapshot Snapshot, byte[] Actor, long Issued, long Expires) DecodeSnapshot(Stream stream, byte version)
    {
        if (ReadByte(stream) != version) throw ReferenceWire.Invalid();
        var actor = ReferenceWire.ReadExactly(stream, 32);
        var issued = ReferenceWire.ReadI64(stream); var expires = ReferenceWire.ReadI64(stream);
        var fingerprint = ReadText(stream); var businessId = ReadText(stream);
        var scope = ReadScope(stream); var aggregate = ReadIdentity(stream);
        var count = ReadCount(stream, 10000); var entities = new DocumentEntitySnapshot[count];
        for (var i = 0; i < count; i++) entities[i] = new DocumentEntitySnapshot(ReadText(stream), ReadIdentity(stream),
            ReadSet(stream, 1024), ReadSet(stream, 1024), ReadMutations(stream));
        count = ReadCount(stream, 1024); var relations = new DocumentRelationSnapshot[count];
        for (var i = 0; i < count; i++)
        {
            var name = ReadText(stream); var completeness = ReadByte(stream) switch
            { 1 => "COMPLETE", 2 => "PARTIAL", _ => throw ReferenceWire.Invalid() };
            var writable = ReadBool(stream); var mutations = ReadMutations(stream);
            var rows = new string[ReadCount(stream, 10000)];
            for (var j = 0; j < rows.Length; j++) rows[j] = ReadText(stream);
            relations[i] = new DocumentRelationSnapshot(name, rows, completeness, writable, mutations);
        }
        var snapshot = new DocumentSnapshot(fingerprint, businessId, scope, aggregate, entities, relations);
        try { _ = EncodeSnapshot(snapshot, version, actor, issued, expires); }
        catch (ReferenceException) { throw ReferenceWire.Invalid(); }
        return (snapshot, actor, issued, expires);
    }
    internal static void ValidateBindings(BoundDocumentSnapshot document)
    {
        if (document.Bindings.Count > 10000) throw Projection();
        (string Row, string Field)? previous = null;
        foreach (var binding in document.Bindings)
        {
            foreach (var text in new[] { binding.RowKey, binding.CanonicalField, binding.EntityType })
                if (string.IsNullOrEmpty(text) || text != text.Trim()) throw Projection();
                else Text(text);
            if (previous is { } pair && (string.CompareOrdinal(binding.RowKey, pair.Row) < 0 ||
                binding.RowKey == pair.Row && string.CompareOrdinal(binding.CanonicalField, pair.Field) <= 0)) throw Projection();
            previous = (binding.RowKey, binding.CanonicalField);
            var entity = document.Snapshot.Entities.FirstOrDefault(row => row.RowKey == binding.RowKey);
            if (entity == null || entity.Identity.EntityType != binding.EntityType ||
                !entity.LoadedFields.Contains(binding.CanonicalField) ||
                binding.Writable && !entity.WritableFields.Contains(binding.CanonicalField)) throw Projection();
        }
    }
    internal static byte[] EncodeBound(BoundDocumentSnapshot document, byte[] actor, long issued, long expires)
    {
        ValidateBindings(document);
        var prefix = EncodeSnapshot(document.Snapshot, 2, actor, issued, expires);
        using var stream = new MemoryStream(); stream.Write(prefix); Count(stream, document.Bindings.Count);
        foreach (var binding in document.Bindings)
        {
            WriteText(stream, binding.RowKey); WriteText(stream, binding.CanonicalField); WriteText(stream, binding.EntityType);
            WriteBool(stream, binding.Reference != null);
            if (binding.Reference != null) WriteText(stream, binding.Reference.Key);
            ReferenceWire.WriteU64(stream, binding.BindingVersion);
            WriteBool(stream, binding.Nullable); WriteBool(stream, binding.Writable);
        }
        return stream.ToArray();
    }
    internal static (BoundDocumentSnapshot Document, byte[] Actor, long Issued, long Expires) DecodeBound(byte[] payload)
    {
        using var stream = new MemoryStream(payload);
        var (snapshot, actor, issued, expires) = DecodeSnapshot(stream, 2);
        var bindings = new DocumentBlobBindingSnapshot[ReadCount(stream, 10000)];
        for (var i = 0; i < bindings.Length; i++)
        {
            var row = ReadText(stream); var field = ReadText(stream); var entity = ReadText(stream);
            BlobRef? reference = null;
            if (ReadBool(stream))
            {
                try { reference = new BlobRef(ReadText(stream)); }
                catch (ArgumentException) { throw ReferenceWire.Invalid(); }
            }
            bindings[i] = new DocumentBlobBindingSnapshot(row, field, entity, reference,
                ReferenceWire.ReadU64(stream), ReadBool(stream), ReadBool(stream));
        }
        if (stream.Position != stream.Length) throw ReferenceWire.Invalid();
        var document = new BoundDocumentSnapshot(snapshot, bindings);
        try { ValidateBindings(document); }
        catch (ReferenceException) { throw ReferenceWire.Invalid(); }
        return (document, actor, issued, expires);
    }
    internal static void ValidateTarget(BlobSelectionTarget target)
    {
        ReferenceWire.Scope(target.Scope);
        foreach (var text in new[] { target.TargetEntity, target.CanonicalField, target.TargetKey, target.CommandId })
        { Text(text); if (text != text.Trim()) throw ReferenceWire.Invalid(); }
        if (target.Kind is not ("EXISTING_ROW" or "NEW_CLIENT_KEY")) throw ReferenceWire.Invalid();
    }
    internal static void RequireTarget(BoundDocumentSnapshot document, BlobSelectionTarget target)
    {
        ValidateTarget(target);
        if (document.Snapshot.Scope != target.Scope) throw ReferenceWire.Mismatch();
        var allowed = target.Kind == "EXISTING_ROW"
            ? document.Snapshot.Entities.Any(row => row.RowKey == target.TargetKey && row.Identity.EntityType == target.TargetEntity &&
                row.WritableFields.Contains(target.CanonicalField)) &&
              document.Bindings.Any(binding => binding.RowKey == target.TargetKey && binding.CanonicalField == target.CanonicalField && binding.Writable)
            : document.Snapshot.Relations.Any(relation => relation.Completeness == "COMPLETE" && relation.Writable &&
                relation.AllowedMutations.Contains("CREATE_CHILD"));
        if (!allowed) throw Projection();
    }
    internal static byte[] EncodeSelection(BlobSelectionClaim claim, byte[] actor, long issued, long expires)
    {
        ValidateTarget(claim.Target);
        using var stream = new MemoryStream(); stream.WriteByte(1); stream.Write(actor);
        ReferenceWire.WriteI64(stream, issued); ReferenceWire.WriteI64(stream, expires);
        WriteScope(stream, claim.Target.Scope); WriteText(stream, claim.Target.TargetEntity);
        WriteText(stream, claim.Target.CanonicalField);
        stream.WriteByte(claim.Target.Kind == "EXISTING_ROW" ? (byte)1 : (byte)2);
        WriteText(stream, claim.Target.TargetKey); WriteText(stream, claim.Reference.Key);
        WriteText(stream, claim.Target.CommandId); return stream.ToArray();
    }
    internal static (BlobSelectionClaim Claim, byte[] Actor, long Issued, long Expires) DecodeSelection(byte[] payload)
    {
        using var stream = new MemoryStream(payload); if (ReadByte(stream) != 1) throw ReferenceWire.Invalid();
        var actor = ReferenceWire.ReadExactly(stream, 32);
        var issued = ReferenceWire.ReadI64(stream); var expires = ReferenceWire.ReadI64(stream);
        var scope = ReadScope(stream); var entity = ReadText(stream); var field = ReadText(stream);
        var kind = ReadByte(stream) switch { 1 => "EXISTING_ROW", 2 => "NEW_CLIENT_KEY", _ => throw ReferenceWire.Invalid() };
        var row = ReadText(stream); BlobRef reference;
        try { reference = new BlobRef(ReadText(stream)); }
        catch (ArgumentException) { throw ReferenceWire.Invalid(); }
        var command = ReadText(stream);
        if (stream.Position != stream.Length) throw ReferenceWire.Invalid();
        var target = new BlobSelectionTarget(scope, entity, field, kind, row, command);
        ValidateTarget(target); return (new BlobSelectionClaim(target, reference), actor, issued, expires);
    }
}

public partial class SecureDocumentReferenceModule : ContextBoundReferenceRuntime
{
    public SecureDocumentReferenceModule(string service, string environment, IReferenceKeyProvider keys,
        Func<TrustedReferencePrincipal, ReferenceDocumentScope, ReferenceIdentity, bool> authorize,
        Func<DateTimeOffset>? clock = null, Func<byte[]>? nonce = null)
        : base(service, environment, keys, authorize, clock, nonce) { }

    private void AuthorizeSnapshot(TrustedReferencePrincipal principal, DocumentSnapshot snapshot)
    {
        Authorize(principal, snapshot.Scope, snapshot.Aggregate);
        foreach (var entity in snapshot.Entities) Authorize(principal, snapshot.Scope, entity.Identity);
    }
    private VerifiedDocument<T> Verify<T>(T document, TrustedReferencePrincipal principal,
        ReferenceKey key, byte[] actor, long issued, long expires, ReferenceDocumentScope scope,
        string expectedAggregate, string expectedPurpose)
    {
        var now = Now().ToUnixTimeSeconds(); if (expires <= now) throw ReferenceWire.Expired();
        if (issued > now + 60) throw ReferenceWire.Invalid();
        if (!CryptographicOperations.FixedTimeEquals(actor, Actor(key, principal)) ||
            scope.AggregateType != expectedAggregate || scope.Purpose != expectedPurpose) throw ReferenceWire.Mismatch();
        try { return new VerifiedDocument<T>(document, key.KeyId,
            DateTimeOffset.FromUnixTimeSeconds(issued), DateTimeOffset.FromUnixTimeSeconds(expires)); }
        catch (ArgumentOutOfRangeException) { throw ReferenceWire.Invalid(); }
    }
    public string IssueDocumentSnapshot(TrustedReferencePrincipal principal, DocumentSnapshot snapshot, TimeSpan lifetime)
    {
        ReferenceWire.Principal(principal); var (issued, expires) = Times(lifetime);
        var key = CurrentKey(); var payload = DocumentWire.EncodeSnapshot(snapshot, 1, Actor(key, principal), issued, expires);
        AuthorizeSnapshot(principal, snapshot);
        return Seal("tqd1", "document-snapshot-encryption", key, snapshot.Scope.Purpose, payload, 256000);
    }
    public VerifiedDocument<DocumentSnapshot> ConsumeDocumentSnapshot(TrustedReferencePrincipal principal,
        string token, string aggregateType, string purpose)
    {
        ReferenceWire.Principal(principal); DocumentWire.Text(aggregateType); DocumentWire.Text(purpose);
        var (key, payload) = Open("tqd1", "document-snapshot-encryption", purpose, token, 256000);
        using var stream = new MemoryStream(payload);
        var (snapshot, actor, issued, expires) = DocumentWire.DecodeSnapshot(stream, 1);
        if (stream.Position != stream.Length) throw ReferenceWire.Invalid();
        var verified = Verify(snapshot, principal, key, actor, issued, expires, snapshot.Scope, aggregateType, purpose);
        AuthorizeSnapshot(principal, snapshot); return verified;
    }
    public string IssueBoundDocumentSnapshot(TrustedReferencePrincipal principal,
        BoundDocumentSnapshot document, TimeSpan lifetime)
    {
        ReferenceWire.Principal(principal); var (issued, expires) = Times(lifetime);
        var key = CurrentKey(); var payload = DocumentWire.EncodeBound(document, Actor(key, principal), issued, expires);
        AuthorizeSnapshot(principal, document.Snapshot);
        return Seal("tqd2", "document-snapshot-v2-encryption", key, document.Snapshot.Scope.Purpose, payload, 256000);
    }
    public VerifiedDocument<BoundDocumentSnapshot> ConsumeBoundDocumentSnapshot(TrustedReferencePrincipal principal,
        string token, string aggregateType, string purpose)
    {
        ReferenceWire.Principal(principal); DocumentWire.Text(aggregateType); DocumentWire.Text(purpose);
        var (key, payload) = Open("tqd2", "document-snapshot-v2-encryption", purpose, token, 256000);
        var (document, actor, issued, expires) = DocumentWire.DecodeBound(payload);
        var verified = Verify(document, principal, key, actor, issued, expires,
            document.Snapshot.Scope, aggregateType, purpose);
        AuthorizeSnapshot(principal, document.Snapshot); return verified;
    }
    public string IssueBlobSelection(TrustedReferencePrincipal principal, string documentToken,
        BlobSelectionClaim claim, TimeSpan lifetime)
    {
        DocumentWire.ValidateTarget(claim.Target);
        var document = ConsumeBoundDocumentSnapshot(principal, documentToken,
            claim.Target.Scope.AggregateType, claim.Target.Scope.Purpose);
        DocumentWire.RequireTarget(document.Document, claim.Target);
        var (issued, expires) = Times(lifetime);
        if (expires > document.ExpiresAt.ToUnixTimeSeconds()) throw ReferenceWire.Invalid();
        var key = CurrentKey(); var payload = DocumentWire.EncodeSelection(claim, Actor(key, principal), issued, expires);
        return Seal("tqs1", "blob-selection-v1-encryption", key, claim.Target.Scope.Purpose, payload, 256000);
    }
    public VerifiedDocument<BlobSelectionClaim> ConsumeBlobSelection(TrustedReferencePrincipal principal,
        string documentToken, string token, BlobSelectionTarget target)
    {
        DocumentWire.ValidateTarget(target);
        var document = ConsumeBoundDocumentSnapshot(principal, documentToken,
            target.Scope.AggregateType, target.Scope.Purpose);
        DocumentWire.RequireTarget(document.Document, target);
        var (key, payload) = Open("tqs1", "blob-selection-v1-encryption", target.Scope.Purpose, token, 256000);
        var (claim, actor, issued, expires) = DocumentWire.DecodeSelection(payload);
        if (expires > document.ExpiresAt.ToUnixTimeSeconds()) throw ReferenceWire.Invalid();
        if (claim.Target != target) throw ReferenceWire.Mismatch();
        return Verify(claim, principal, key, actor, issued, expires, claim.Target.Scope,
            target.Scope.AggregateType, target.Scope.Purpose);
    }
}
