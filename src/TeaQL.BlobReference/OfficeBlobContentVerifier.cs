using System.Buffers.Binary;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace TeaQL.BlobReference;

/// <summary>Passive, bounded DOCX/XLSX OOXML package and XML structure verifier.</summary>
public sealed class OfficeBlobContentVerifier : IBlobContentVerifier
{
    private const string PackageNamespace = "http://schemas.openxmlformats.org/package/2006/";
    private const string DocumentNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/";
    private const string WordNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    private const string SpreadsheetNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);
    private static readonly uint[] CrcTable = MakeCrcTable();
    private readonly int _maximumInputBytes;
    private readonly int _maximumDecodedBytes;
    private readonly int _maximumParts;
    private readonly int _maximumElements;
    private readonly int _concurrentVerifications;
    private int _active;

    public OfficeBlobContentVerifier(int maximumInputBytes = 32 * 1024 * 1024,
        int maximumDecodedBytes = 100 * 1024 * 1024, int maximumParts = 1000,
        int maximumElements = 200_000, int concurrentVerifications = 2)
    {
        if (maximumInputBytes is < 1 or > 100 * 1024 * 1024 ||
            maximumDecodedBytes is < 1 or > 100 * 1024 * 1024 ||
            maximumParts is < 1 or > 10_000 || maximumElements is < 1 or > 2_000_000 ||
            concurrentVerifications is < 1 or > 32)
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        _maximumInputBytes = maximumInputBytes;
        _maximumDecodedBytes = maximumDecodedBytes;
        _maximumParts = maximumParts;
        _maximumElements = maximumElements;
        _concurrentVerifications = concurrentVerifications;
    }

    public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
        long maximumBytes)
    {
        var mime = extension switch
        {
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => throw Error("BLOB_FORMAT_NOT_ALLOWED")
        };
        if (content == null || maximumBytes < 1)
            throw Error("BLOB_POLICY_INVALID");
        if (Interlocked.Increment(ref _active) > _concurrentVerifications)
        {
            Interlocked.Decrement(ref _active);
            throw Error("BLOB_VERIFICATION_BUSY");
        }
        try
        {
            var bound = Math.Min(maximumBytes, _maximumInputBytes);
            using var output = new MemoryStream();
            var buffer = new byte[64 * 1024];
            try
            {
                while (true)
                {
                    var count = await content.ReadAsync(buffer).ConfigureAwait(false);
                    if (count == 0) break;
                    if (count > bound - output.Length) throw Error("BLOB_SIZE_EXCEEDED");
                    output.Write(buffer, 0, count);
                }
            }
            catch (InvalidOperationException error) when (error.Message == "BLOB_SIZE_EXCEEDED")
            { throw; }
            catch { throw Error("BLOB_STORE_FAILED"); }
            var data = output.ToArray();
            try { CheckPackage(data, extension); }
            catch (InvalidOperationException error) when (error.Message == "BLOB_FORMAT_NOT_ALLOWED")
            { throw; }
            catch { throw Error("BLOB_FORMAT_NOT_ALLOWED"); }
            return new BlobVerifiedContent(data.LongLength,
                Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant(), mime);
        }
        finally { Interlocked.Decrement(ref _active); }
    }

    private static InvalidOperationException Error(string code) => new(code);

    private static uint[] MakeCrcTable()
    {
        var table = new uint[256];
        for (uint index = 0; index < table.Length; index++)
        {
            var value = index;
            for (var bit = 0; bit < 8; bit++)
                value = (value & 1) == 0 ? value >> 1 : 0xedb88320u ^ (value >> 1);
            table[index] = value;
        }
        return table;
    }

    private static uint Crc32(ReadOnlySpan<byte> content)
    {
        var crc = 0xffffffffu;
        foreach (var value in content)
            crc = CrcTable[(crc ^ value) & 0xff] ^ (crc >> 8);
        return crc ^ 0xffffffffu;
    }

    private static bool PartName(string name) => name.Length is > 0 and <= 1024 &&
        !name.StartsWith('/') && !name.Contains('\\') && !name.Any(char.IsControl) &&
        name.Split('/').All(part => part.Length > 0 && part is not "." and not "..");

    private sealed record CentralPart(string Name, ushort Method, ushort Flags, uint Crc,
        uint Compressed, uint Expanded, uint LocalOffset);

    // Require an exact single-disk central directory, with no appended payload.
    private static IReadOnlyList<CentralPart> CentralDirectory(byte[] data, int maximumParts)
    {
        if (data.Length < 22 || !data.AsSpan(0, 4).SequenceEqual("PK\x03\x04"u8))
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var eocd = -1;
        for (var index = data.Length - 22; index >= Math.Max(0, data.Length - 22 - 65535); index--)
        {
            if (data.AsSpan(index, 4).SequenceEqual("PK\x05\x06"u8) &&
                index + 22 + BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(index + 20)) == data.Length)
            { eocd = index; break; }
        }
        if (eocd < 0 || BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(eocd + 4)) != 0 ||
            BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(eocd + 6)) != 0)
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var count = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(eocd + 10));
        if (count is 0 or 65535 || count > maximumParts ||
            count != BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(eocd + 8)))
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var size = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(eocd + 12));
        var offset = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(eocd + 16));
        if (size == uint.MaxValue || offset == uint.MaxValue || (ulong)size + offset != (ulong)eocd)
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var parts = new List<CentralPart>(count);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var cursor = checked((int)offset);
        for (var position = 0; position < count; position++)
        {
            if ((ulong)cursor + 46 > (ulong)eocd ||
                !data.AsSpan(cursor, 4).SequenceEqual("PK\x01\x02"u8))
                throw Error("BLOB_FORMAT_NOT_ALLOWED");
            var flags = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(cursor + 8));
            var method = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(cursor + 10));
            var crc = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(cursor + 16));
            var compressed = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(cursor + 20));
            var expanded = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(cursor + 24));
            var nameLength = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(cursor + 28));
            var extraLength = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(cursor + 30));
            var commentLength = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(cursor + 32));
            var disk = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(cursor + 34));
            var localOffset = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(cursor + 42));
            var next = (ulong)cursor + 46u + nameLength + extraLength + commentLength;
            if (next > (ulong)eocd || nameLength == 0 || disk != 0 ||
                (flags & 1) != 0 || method is not (0 or 8) ||
                compressed == uint.MaxValue || expanded == uint.MaxValue ||
                localOffset == uint.MaxValue || localOffset >= offset)
                throw Error("BLOB_FORMAT_NOT_ALLOWED");
            var name = StrictUtf8.GetString(data, cursor + 46, nameLength);
            if (!PartName(name) || !seen.Add(name)) throw Error("BLOB_FORMAT_NOT_ALLOWED");
            var local = checked((int)localOffset);
            if ((ulong)local + 30 > (ulong)offset ||
                !data.AsSpan(local, 4).SequenceEqual("PK\x03\x04"u8) ||
                BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(local + 6)) != flags ||
                BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(local + 8)) != method ||
                BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(local + 26)) != nameLength ||
                (ulong)local + 30u + nameLength > (ulong)offset ||
                !data.AsSpan(local + 30, nameLength).SequenceEqual(data.AsSpan(cursor + 46, nameLength)))
                throw Error("BLOB_FORMAT_NOT_ALLOWED");
            parts.Add(new CentralPart(name, method, flags, crc, compressed, expanded, localOffset));
            cursor = checked((int)next);
        }
        if ((uint)cursor != offset + size || parts.Min(part => part.LocalOffset) != 0)
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        return parts;
    }

    private static string RootAndScan(byte[] data, ref int elements, int maximum,
        Action<XmlReader>? visit = null)
    {
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null, MaxCharactersInDocument = data.LongLength,
            ConformanceLevel = ConformanceLevel.Document };
        using var stream = new MemoryStream(data, writable: false);
        using var reader = XmlReader.Create(stream, settings);
        string? root = null;
        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Element) continue;
            if (reader.Depth == 0) root = reader.NamespaceURI + "|" + reader.LocalName;
            elements++;
            if (reader.Depth > 127 || elements > maximum)
                throw Error("BLOB_FORMAT_NOT_ALLOWED");
            visit?.Invoke(reader);
        }
        return root ?? throw Error("BLOB_FORMAT_NOT_ALLOWED");
    }

    private void CheckPackage(byte[] data, string extension)
    {
        var central = CentralDirectory(data, _maximumParts);
        using var stream = new MemoryStream(data, writable: false);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
        if (archive.Entries.Count != central.Count) throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var parts = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        var expanded = 0L;
        var elements = 0;
        for (var index = 0; index < central.Count; index++)
        {
            var entry = archive.Entries[index];
            var expected = central[index];
            if (entry.FullName != expected.Name || entry.Length != expected.Expanded ||
                entry.CompressedLength != expected.Compressed ||
                expected.Expanded > _maximumDecodedBytes - expanded)
                throw Error("BLOB_FORMAT_NOT_ALLOWED");
            using var member = entry.Open();
            using var output = new MemoryStream();
            var buffer = new byte[64 * 1024];
            while (true)
            {
                var read = member.Read(buffer, 0, (int)Math.Min(buffer.Length,
                    _maximumDecodedBytes - expanded - output.Length + 1));
                if (read == 0) break;
                output.Write(buffer, 0, read);
                if (output.Length > _maximumDecodedBytes - expanded)
                    throw Error("BLOB_FORMAT_NOT_ALLOWED");
            }
            var content = output.ToArray();
            if (content.LongLength != expected.Expanded || Crc32(content) != expected.Crc)
                throw Error("BLOB_FORMAT_NOT_ALLOWED");
            expanded += content.Length;
            if (expected.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) ||
                expected.Name.EndsWith(".rels", StringComparison.OrdinalIgnoreCase))
                RootAndScan(content, ref elements, _maximumElements);
            parts.Add(expected.Name, content);
        }
        var main = extension == "docx" ? "word/document.xml" : "xl/workbook.xml";
        var mainType = extension == "docx"
            ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"
            : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";
        if (!parts.TryGetValue("[Content_Types].xml", out var types) ||
            !parts.TryGetValue("_rels/.rels", out var relationships) ||
            !parts.TryGetValue(main, out var document))
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var typesMatch = false;
        var count = 0;
        var root = RootAndScan(types, ref count, _maximumElements, reader => {
            if (reader.Depth == 1 && reader.NamespaceURI == PackageNamespace + "content-types" &&
                reader.LocalName == "Override" && reader.GetAttribute("PartName") == "/" + main &&
                reader.GetAttribute("ContentType") == mainType) typesMatch = true;
        });
        if (root != PackageNamespace + "content-types|Types" || !typesMatch)
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var relationMatch = false;
        count = 0;
        root = RootAndScan(relationships, ref count, _maximumElements, reader => {
            if (reader.Depth == 1 && reader.NamespaceURI == PackageNamespace + "relationships" &&
                reader.LocalName == "Relationship" &&
                reader.GetAttribute("Type") == DocumentNamespace + "relationships/officeDocument" &&
                reader.GetAttribute("Target") == main && reader.GetAttribute("TargetMode") != "External")
                relationMatch = true;
        });
        if (root != PackageNamespace + "relationships|Relationships" || !relationMatch)
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        if (extension == "docx")
        {
            var body = false;
            count = 0;
            root = RootAndScan(document, ref count, _maximumElements, reader => {
                if (reader.Depth == 1 && reader.NamespaceURI == WordNamespace &&
                    reader.LocalName == "body") body = true;
            });
            if (root != WordNamespace + "|document" || !body)
                throw Error("BLOB_FORMAT_NOT_ALLOWED");
            return;
        }
        var sheetIds = new HashSet<string>(StringComparer.Ordinal);
        count = 0;
        root = RootAndScan(document, ref count, _maximumElements, reader => {
            if (reader.Depth == 2 && reader.NamespaceURI == SpreadsheetNamespace &&
                reader.LocalName == "sheet")
            {
                var id = reader.GetAttribute("id", DocumentNamespace + "relationships");
                if (!string.IsNullOrEmpty(id)) sheetIds.Add(id);
            }
        });
        if (root != SpreadsheetNamespace + "|workbook" || sheetIds.Count == 0 ||
            !parts.TryGetValue("xl/_rels/workbook.xml.rels", out var workbookRels))
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var matched = new HashSet<string>(StringComparer.Ordinal);
        count = 0;
        root = RootAndScan(workbookRels, ref count, _maximumElements, reader => {
            if (reader.Depth != 1 || reader.NamespaceURI != PackageNamespace + "relationships" ||
                reader.LocalName != "Relationship" ||
                reader.GetAttribute("Type") != DocumentNamespace + "relationships/worksheet" ||
                reader.GetAttribute("TargetMode") == "External") return;
            var id = reader.GetAttribute("Id");
            var target = reader.GetAttribute("Target");
            if (id == null || target == null || !sheetIds.Contains(id) || !PartName(target)) return;
            var resolved = "xl/" + target;
            if (!resolved.StartsWith("xl/worksheets/", StringComparison.Ordinal) ||
                !parts.TryGetValue(resolved, out var worksheet)) return;
            var innerCount = 0;
            if (RootAndScan(worksheet, ref innerCount, _maximumElements) ==
                SpreadsheetNamespace + "|worksheet") matched.Add(id);
        });
        if (root != PackageNamespace + "relationships|Relationships" ||
            matched.Count != sheetIds.Count) throw Error("BLOB_FORMAT_NOT_ALLOWED");
    }
}
