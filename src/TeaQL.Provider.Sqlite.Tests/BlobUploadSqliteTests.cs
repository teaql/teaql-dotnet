using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core;
using TeaQL.Core.Storage;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class BlobUploadSqliteTests
{
    private sealed class UploadAuthorization : IBlobUploadAuthorization
    {
        public string CurrentScope = "tenant-a";
        public bool Allowed = true;
        public string Scope(UserContext actor) => CurrentScope;
        public BlobFieldSpec Field(UserContext actor, BlobUploadTarget target) =>
            new("Asset", "file", ["pdf"], true, "default");
        public Task<bool> AuthorizeAsync(UserContext actor, BlobUploadTarget target,
            string action, BlobUploadInput input) => Task.FromResult(Allowed &&
                target.Entity == "Asset" && target.Field == "file" &&
                action is "Begin" or "Write" or "Confirm");
    }

    private sealed class BindingAuthorization : IBlobBindingAuthorization
    {
        public string Scope(UserContext actor) => "tenant-a";
        public Task<bool> AuthorizeAsync(UserContext actor, BlobBindingTarget target,
            BlobRef? previous, BlobRef? next, string action) =>
            Task.FromResult(target.Entity == "Asset" && target.Field == "file" && action == "Create");
    }

    // Fixture-only full-byte parser, not production PDF or ten-format detection.
    private sealed class PdfVerifier(bool early = false) : IBlobContentVerifier
    {
        public int Calls;
        public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
            long maximumBytes)
        {
            Calls++;
            if (early) return new BlobVerifiedContent(1, new string('0', 64), "application/pdf");
            using var output = new MemoryStream();
            await content.CopyToAsync(output);
            var bytes = output.ToArray();
            if (bytes.LongLength > maximumBytes || extension != "pdf" ||
                !Encoding.UTF8.GetString(bytes).StartsWith("%PDF-") ||
                !Encoding.UTF8.GetString(bytes).Contains("%%EOF"))
                throw new InvalidOperationException("invalid test PDF");
            return new BlobVerifiedContent(bytes.LongLength,
                Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(), "application/pdf");
        }
    }

    private static async Task<string> Scalar(SqliteConnection db, string sql, string key)
    {
        await using var command = db.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@key", key);
        return (string)(await command.ExecuteScalarAsync())!;
    }

    [Fact]
    public async Task RealSQLiteFileUploadRequiresFullVerificationBeforeGraphBind()
    {
        var parent = Path.Combine(Path.GetTempPath(), "teaql-blob-upload-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(parent);
        try
        {
            await using var db = new SqliteConnection("Data Source=:memory:");
            await db.OpenAsync();
            var asset = EntityDescriptor.New("Asset").TableName("asset")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("version", DataType.I64).Version())
                .Property(PropertyDescriptor.New("name", DataType.Text));
            var actor = new RuntimeModule().Entity(asset).IntoContext().WithDataService(
                new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(db),
                    new MetadataSchemaProvider(name => name == "Asset" ? asset : null)));
            await actor.EnsureSchemaAsync();
            var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
            await catalog.EnsureSchemaAsync();
            await catalog.ValidateSchemaAsync();
            await using var provider = new FileObjectStorage(Path.Combine(parent, "private"),
                RandomNumberGenerator.GetBytes(32));
            actor.WithObjectStorage(provider);
            var authorization = new UploadAuthorization();
            var verifier = new PdfVerifier();
            var service = new SqliteBlobUploadService(catalog, authorization, verifier, 10_000);
            var bytes = Encoding.UTF8.GetBytes("%PDF-1.7\nreal file bytes\n%%EOF");
            var target = new BlobUploadTarget("Asset", "1", "file");
            var input = new BlobUploadInput("path/report.pdf", bytes.Length, "application/pdf");
            var session = await service.BeginAsync(actor, target, input, "request-1");
            var replay = await service.BeginAsync(actor, target, input, "request-1");
            Assert.Equal(session.UploadId, replay.UploadId);
            Assert.Equal("BLOB_IDEMPOTENCY_CONFLICT", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.BeginAsync(actor, target, new BlobUploadInput("other.pdf", bytes.Length),
                    "request-1"))).Message);
            Assert.Equal("BLOB_UPLOAD_INCOMPLETE", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, session.UploadId))).Message);
            authorization.CurrentScope = "tenant-b";
            Assert.Equal("BLOB_UPLOAD_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)))).Message);
            authorization.CurrentScope = "tenant-a";
            Assert.Equal(bytes.Length, await service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)));
            Assert.Equal("initiated", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            authorization.Allowed = false;
            Assert.Equal("BLOB_UPLOAD_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, session.UploadId))).Message);
            authorization.Allowed = true;
            var reference = await service.CompleteAsync(actor, session.UploadId);
            Assert.Equal(session.Reference, reference);
            Assert.Equal(1, verifier.Calls);
            Assert.Equal(reference, await service.CompleteAsync(actor, session.UploadId));
            Assert.Equal(1, verifier.Calls);
            authorization.Allowed = false;
            Assert.Equal("BLOB_UPLOAD_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, session.UploadId))).Message);
            authorization.Allowed = true;
            Assert.Equal("uploaded_unbound", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", reference.Key));
            await actor.ExecuteGraphSaveAsync("bind verified upload", async graph =>
            {
                await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Asset")
                    .Value("id", new Value.U64Value(1))
                    .Value("name", new Value.TextValue("document")), "save asset"),
                    graph.Scope("Asset", 1, null));
                await catalog.BindInGraphAsync(graph, actor, new BlobBindingChange(
                    new BlobBindingTarget("Asset", "1", "file"), reference, 0, 1, "Create"),
                    new BindingAuthorization());
                return true;
            });
            Assert.Equal("available", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", reference.Key));
            Assert.Equal(reference, await service.CompleteAsync(actor, session.UploadId));
            Assert.Equal(1, verifier.Calls);
            var completedKey = await Scalar(db,
                "SELECT object_key FROM teaql_blob_catalog WHERE key=@key", reference.Key);
            await actor.ObjectStorage("replace completed generation").PutBytesAsync(new ObjectKey(completedKey),
                bytes, new ObjectPutOptions(bytes.Length, ObjectWriteMode.Replace, "application/pdf"));
            Assert.Equal("BLOB_UPLOAD_INCOMPLETE", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, session.UploadId))).Message);
            Assert.Equal(1, verifier.Calls);

            var second = await service.BeginAsync(actor, target, input, "request-2");
            await service.WriteAsync(actor, second.UploadId, new MemoryStream(bytes));
            var objectKey = await Scalar(db,
                "SELECT object_key FROM teaql_blob_catalog WHERE key=@key", second.Reference.Key);
            await actor.ObjectStorage("replace same bytes").PutBytesAsync(new ObjectKey(objectKey),
                bytes, new ObjectPutOptions(bytes.Length, ObjectWriteMode.Replace, "application/pdf"));
            Assert.Equal("BLOB_UPLOAD_INCOMPLETE", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, second.UploadId))).Message);

            var third = await service.BeginAsync(actor, target, input, "request-3");
            await service.WriteAsync(actor, third.UploadId, new MemoryStream(bytes));
            var early = new SqliteBlobUploadService(catalog, authorization, new PdfVerifier(true), 10_000);
            Assert.Equal("BLOB_UPLOAD_INCOMPLETE", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => early.CompleteAsync(actor, third.UploadId))).Message);

            var fourth = await service.BeginAsync(actor, target, input, "request-4");
            Assert.Equal("BLOB_STORE_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, fourth.UploadId,
                    new MemoryStream([..bytes, (byte)'x'])))).Message);
            var fifth = await service.BeginAsync(actor, target, input, "request-5");
            Assert.Equal("BLOB_STORE_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, fifth.UploadId,
                    new MemoryStream(bytes[..^1])))).Message);

            var clock = DateTimeOffset.FromUnixTimeMilliseconds(1_700_000_000_000);
            var expiring = new SqliteBlobUploadService(catalog, authorization, verifier, 10_000,
                TimeSpan.FromSeconds(1), () => clock);
            var old = await expiring.BeginAsync(actor, target, input, "expired-request");
            Assert.Equal(old.UploadId, (await expiring.BeginAsync(actor, target, input,
                "expired-request")).UploadId);
            clock = old.ExpiresAt;
            Assert.Equal("BLOB_UPLOAD_EXPIRED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => expiring.BeginAsync(actor, target, input, "expired-request"))).Message);
            var fresh = await expiring.BeginAsync(actor, target, input, "fresh-request");
            Assert.NotEqual(old.UploadId, fresh.UploadId);
            Assert.Equal("BLOB_UPLOAD_EXPIRED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => expiring.WriteAsync(actor, old.UploadId, new MemoryStream(bytes)))).Message);
        }
        finally { Directory.Delete(parent, recursive: true); }
    }
}
