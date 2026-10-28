using System.Security.Cryptography;
using System.Text;
using System.Net;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core;
using TeaQL.Core.Storage;
using TeaQL.DataService;
using TeaQL.ObjectStorage.S3;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Tests.ObjectStorageS3;

public class BlobUploadS3Tests
{
    private sealed class UploadPermission : IBlobUploadAuthorization, IBlobUploadPolicyAuthorization
    {
        public bool Allowed = true;
        public string Extension = "pdf";
        public string Scope(UserContext actor) => "f4-s3-upload";
        public BlobFieldSpec Field(UserContext actor, BlobUploadTarget target) =>
            new("Asset", "file", [Extension], true, "default");
        public Task<bool> AuthorizeAsync(UserContext actor, BlobUploadTarget target,
            string action, BlobUploadInput input) => Task.FromResult(Allowed &&
                target.Entity == "Asset" && target.Field == "file" &&
                action is "Begin" or "Write" or "Confirm");
        public async Task<BlobUploadAuthorizationState> CurrentOnLeaseAsync(UserContext actor,
            BlobUploadTarget target, string action, BlobUploadInput input, ISqlTransport lease) =>
            new(Scope(actor), Field(actor, target),
                await AuthorizeAsync(actor, target, action, input));
        public Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor,
            ISqlTransport lease, BlobUploadTarget target) =>
            Task.FromResult(new BlobRetentionPolicy("default", "server_stream", 86400, 604800, 3600));
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

    private sealed class BindingPermission : IBlobBindingAuthorization, IBlobBindingPolicyAuthorization
    {
        public string Scope(UserContext actor) => "f4-s3-upload";
        public Task<bool> AuthorizeAsync(UserContext actor, BlobBindingTarget target,
            BlobRef? previous, BlobRef? next, string action) => Task.FromResult(
                target.Entity == "Asset" && target.EntityId == "1" && target.Field == "file" &&
                previous is null && next is not null && action == "Create");
        public Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor,
            IPhysicalSqlLease lease, BlobBindingTarget target) =>
            Task.FromResult(new BlobRetentionPolicy("default", "server_stream", 86400, 604800, 3600));
    }

    private sealed class ReadPermission : IBlobReadAuthorization
    {
        public bool Allowed = true;
        public Task<bool> AuthorizeAsync(UserContext actor, AuthorizedBlobSlot slot) =>
            Task.FromResult(Allowed);
    }

    private sealed class GraphAuthorizer(BlobRef reference) : IResponseGraphAuthorizer
    {
        public long BindingVersion = 1;
        public Task<AuthorizedResponseGraph> AuthorizeAsync(UserContext actor, object? result,
            ViewOptions options) => Task.FromResult(new AuthorizedResponseGraph(
                new Dictionary<string, object?> { ["document"] = reference },
                [new AuthorizedBlobSlot(["document"], "Asset", "1", "file",
                    "f4-s3-upload", BindingVersion, reference)]));
    }

    [Theory]
    [InlineData("pdf")]
    [InlineData("jpg")]
    [InlineData("mp3")]
    [InlineData("mp4")]
    public async Task RealSQLiteBlobUploadThroughS3CreateOnlyConfirmAndReplay(string format)
    {
        if (Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BLOB_UPLOAD") != "1") return;
        if (format == "jpg" && Environment.GetEnvironmentVariable("TEAQL_S3_TEST_REAL_IMAGE") != "1") return;
        if ((format is "mp3" or "mp4") &&
            Environment.GetEnvironmentVariable("TEAQL_S3_TEST_REAL_MEDIA") != "1") return;
        var endpoint = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ENDPOINT");
        var bucket = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BUCKET");
        var access = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ACCESS_KEY");
        var secret = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_SECRET_KEY");
        Assert.All(new[] { endpoint, bucket, access, secret }, value => Assert.False(string.IsNullOrEmpty(value)));
        using var provider = new S3ObjectStorage(new AmazonS3Client(
            new BasicAWSCredentials(access!, secret!), new AmazonS3Config {
                ServiceURL = endpoint, AuthenticationRegion = "us-east-1", ForcePathStyle = true
            }), bucket!);
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var asset = EntityDescriptor.New("Asset").TableName("asset")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("name", DataType.Text));
        var transport = new SqliteTransport(db);
        var actor = new RuntimeModule().Entity(asset).IntoContext().WithDataService(
            new SqlDataServiceExecutor(new SqliteDialect(), transport,
                new MetadataSchemaProvider(name => name == "Asset" ? asset : null)))
            .WithObjectStorage(provider);
        await actor.EnsureSchemaAsync();
        var store = actor.ObjectStorage("verify S3 Blob upload");
        var catalog = new SqliteBlobCatalog(transport);
        await catalog.EnsureSchemaAsync();
        var permission = new UploadPermission { Extension = format };
        var pdfVerifier = new PdfVerifier();
        IBlobContentVerifier verifier = format switch {
            "jpg" => new StbImageBlobContentVerifier(),
            "mp3" or "mp4" => new FfmpegBlobContentVerifier(
                "/usr/bin/ffprobe", "/usr/bin/ffmpeg", "/usr/bin/prlimit"),
            _ => pdfVerifier
        };
        var service = new SqliteBlobUploadService(catalog, permission, verifier, 10_000);
        var bytes = format == "pdf"
            ? Encoding.UTF8.GetBytes("%PDF-1.7\nreal S3 Blob upload\n%%EOF")
            : await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample." + format));
        var contentType = format switch {
            "jpg" => "image/jpeg", "mp3" => "audio/mpeg", "mp4" => "video/mp4",
            _ => "application/pdf"
        };
        var filename = format == "pdf" ? "report.pdf" : "sample." + format;
        var session = await service.BeginAsync(actor, new BlobUploadTarget("Asset", "1", "file"),
            new BlobUploadInput(filename, bytes.Length, contentType),
            Guid.NewGuid().ToString("N"));
        var key = new ObjectKey(await Scalar(db,
            "SELECT object_key FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
        try
        {
            Assert.Equal(bytes.Length,
                await service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)));
            if (format != "pdf")
            {
                var uploaded = await store.GetBytesAsync(key, 10_000);
                Assert.Equal(bytes, uploaded);
                var decoded = await verifier.VerifyAsync(new MemoryStream(uploaded), format, 10_000);
                Assert.Equal(contentType, decoded.ContentType);
            }
            permission.Allowed = false;
            Assert.Equal("BLOB_UPLOAD_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, session.UploadId))).Message);
            permission.Allowed = true;
            Assert.Equal(session.Reference, await service.CompleteAsync(actor, session.UploadId));
            Assert.Equal(session.Reference, await service.CompleteAsync(actor, session.UploadId));
            if (format == "pdf") Assert.Equal(1, pdfVerifier.Calls);
            Assert.Equal(bytes.Length,
                await service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)));
            Assert.Equal(bytes, await store.GetBytesAsync(key, 10_000));
            var head = await store.HeadAsync(key);
            Assert.NotNull(head);
            Assert.Equal("uploaded_unbound", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            Assert.Equal(contentType, await Scalar(db,
                "SELECT content_type FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            Assert.Equal(bytes.Length.ToString(), await Scalar(db,
                "SELECT size_bytes FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            Assert.Equal(head.ETag, await Scalar(db,
                "SELECT content_version FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            await actor.ExecuteGraphSaveAsync("bind S3 Blob in business graph", async graph =>
            {
                await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Asset")
                    .Value("id", new Value.U64Value(1))
                    .Value("name", new Value.TextValue("document")), "save asset"),
                    graph.Scope("Asset", 1, null));
                Assert.Equal(1, await catalog.BindInGraphAsync(graph, actor,
                    new BlobBindingChange(new BlobBindingTarget("Asset", "1", "file"),
                        session.Reference, 0, 1, "Create"), new BindingPermission()));
                return true;
            });
            Assert.Equal(session.Reference.Key, await Scalar(db,
                "SELECT key FROM teaql_blob_binding WHERE entity_id='1'", session.Reference.Key));
            Assert.Equal("available", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));

            var readPermission = new ReadPermission();
            var graphAuthorizer = new GraphAuthorizer(session.Reference);
            actor.InsertResource<IBlobResponseContributor>(
                new SqlBlobResponseContributor(catalog, readPermission, 3));
            actor.InsertResource<IResponseGraphAuthorizer>(graphAuthorizer);
            var options = new ViewOptions("ordinary", "read_only", DateTimeOffset.UtcNow.AddSeconds(60));
            var view = await new ViewPreparationService().PrepareAsync(actor, null, options);
            var link = Assert.IsType<string>(
                Assert.IsType<Dictionary<string, object?>>(view.Presentation)["document"]);
            Assert.StartsWith("http", link);
            Assert.Equal("private, no-store", view.CacheControl);
            using var http = new HttpClient(new HttpClientHandler { UseProxy = false });
            using var request = new HttpRequestMessage(HttpMethod.Get, link);
            request.Headers.TryAddWithoutValidation("Range", "bytes=0-7");
            using var response = await http.SendAsync(request);
            Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);
            Assert.Equal(bytes.AsSpan(0, 8).ToArray(), await response.Content.ReadAsByteArrayAsync());
            Assert.Equal(contentType, response.Content.Headers.ContentType?.MediaType);
            Assert.Contains(filename, response.Content.Headers.ContentDisposition?.ToString());

            readPermission.Allowed = false;
            var denied = await Assert.ThrowsAsync<ViewPreparationException>(() =>
                new ViewPreparationService().PrepareAsync(actor, null, options));
            Assert.Equal("BLOB_READ_DENIED", denied.Code);
            readPermission.Allowed = true;
            graphAuthorizer.BindingVersion = 0;
            var stale = await Assert.ThrowsAsync<ViewPreparationException>(() =>
                new ViewPreparationService().PrepareAsync(actor, null,
                    new ViewOptions("ordinary", "committed", DateTimeOffset.UtcNow.AddSeconds(60))));
            Assert.Equal("BLOB_READ_DENIED", stale.Code);
            Assert.Equal("committed", stale.BusinessState);
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
