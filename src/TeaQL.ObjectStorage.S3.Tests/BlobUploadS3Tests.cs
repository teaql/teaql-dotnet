using System.Security.Cryptography;
using System.Text;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core;
using TeaQL.Core.Storage;
using TeaQL.ObjectStorage.S3;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Tests.ObjectStorageS3;

public class BlobUploadS3Tests
{
    private sealed class UploadPermission : IBlobUploadAuthorization
    {
        public bool Allowed = true;
        public string Scope(UserContext actor) => "f4-s3-upload";
        public BlobFieldSpec Field(UserContext actor, BlobUploadTarget target) =>
            new("Asset", "file", ["pdf"], true, "default");
        public Task<bool> AuthorizeAsync(UserContext actor, BlobUploadTarget target,
            string action, BlobUploadInput input) => Task.FromResult(Allowed &&
                target.Entity == "Asset" && target.Field == "file" &&
                action is "Begin" or "Write" or "Confirm");
        public async Task<BlobUploadAuthorizationState> CurrentOnLeaseAsync(UserContext actor,
            BlobUploadTarget target, string action, BlobUploadInput input, ISqlTransport lease) =>
            new(Scope(actor), Field(actor, target),
                await AuthorizeAsync(actor, target, action, input));
    }

    // Fixture-only full-byte parser; production format detection has a separate gate.
    private sealed class PdfVerifier : IBlobContentVerifier
    {
        public int Calls;
        public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
            long maximumBytes)
        {
            Calls++;
            using var output = new MemoryStream();
            await content.CopyToAsync(output);
            var bytes = output.ToArray();
            if (bytes.LongLength > maximumBytes || extension != "pdf" ||
                !Encoding.UTF8.GetString(bytes).StartsWith("%PDF-") ||
                !Encoding.UTF8.GetString(bytes).Contains("%%EOF"))
                throw new InvalidOperationException("invalid PDF fixture");
            return new BlobVerifiedContent(bytes.LongLength,
                Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),
                "application/pdf");
        }
    }

    [Fact]
    public async Task RealSQLiteBlobUploadThroughS3CreateOnlyConfirmAndReplay()
    {
        if (Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BLOB_UPLOAD") != "1") return;
        var endpoint = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ENDPOINT");
        var bucket = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BUCKET");
        var access = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ACCESS_KEY");
        var secret = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_SECRET_KEY");
        Assert.All(new[] { endpoint, bucket, access, secret }, value => Assert.False(string.IsNullOrEmpty(value)));
        using var provider = new S3ObjectStorage(new AmazonS3Client(
            new BasicAWSCredentials(access!, secret!), new AmazonS3Config {
                ServiceURL = endpoint, AuthenticationRegion = "us-east-1", ForcePathStyle = true
            }), bucket!);
        var actor = new UserContext().WithObjectStorage(provider);
        var store = actor.ObjectStorage("verify S3 Blob upload");
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
        await catalog.EnsureSchemaAsync();
        var permission = new UploadPermission();
        var verifier = new PdfVerifier();
        var service = new SqliteBlobUploadService(catalog, permission, verifier, 10_000);
        var bytes = Encoding.UTF8.GetBytes("%PDF-1.7\nreal S3 Blob upload\n%%EOF");
        var session = await service.BeginAsync(actor, new BlobUploadTarget("Asset", "1", "file"),
            new BlobUploadInput("report.pdf", bytes.Length, "application/pdf"),
            Guid.NewGuid().ToString("N"));
        var key = new ObjectKey(await Scalar(db,
            "SELECT object_key FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
        try
        {
            Assert.Equal(bytes.Length,
                await service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)));
            permission.Allowed = false;
            Assert.Equal("BLOB_UPLOAD_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, session.UploadId))).Message);
            permission.Allowed = true;
            Assert.Equal(session.Reference, await service.CompleteAsync(actor, session.UploadId));
            Assert.Equal(session.Reference, await service.CompleteAsync(actor, session.UploadId));
            Assert.Equal(1, verifier.Calls);
            Assert.Equal(bytes.Length,
                await service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)));
            Assert.Equal(bytes, await store.GetBytesAsync(key, 10_000));
            var head = await store.HeadAsync(key);
            Assert.NotNull(head);
            Assert.Equal("uploaded_unbound", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            Assert.Equal("application/pdf", await Scalar(db,
                "SELECT content_type FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            Assert.Equal(bytes.Length.ToString(), await Scalar(db,
                "SELECT size_bytes FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            Assert.Equal(head.ETag, await Scalar(db,
                "SELECT content_version FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
        }
        finally { await store.DeleteAsync(key); }
    }

    private static async Task<string> Scalar(SqliteConnection db, string sql, string key)
    {
        await using var command = db.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@key", key);
        return Convert.ToString(await command.ExecuteScalarAsync())!;
    }
}
