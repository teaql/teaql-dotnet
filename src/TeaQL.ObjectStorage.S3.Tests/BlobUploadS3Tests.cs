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
using Xunit.Abstractions;

namespace TeaQL.Tests.ObjectStorageS3;

public class BlobUploadS3Tests(ITestOutputHelper output)
{
    private sealed class UploadPermission : IBlobUploadAuthorization, IBlobUploadPolicyAuthorization
    {
        public bool Allowed = true;
        public string Extension = "pdf";
        public BlobRetentionPolicy Policy = new("default", "server_stream", 86400, 604800, 3600);
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
            Task.FromResult(Policy);
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
        public BlobRef? Expected;
        public string Scope(UserContext actor) => "f4-s3-upload";
        public Task<bool> AuthorizeAsync(UserContext actor, BlobBindingTarget target,
            BlobRef? previous, BlobRef? next, string action) => Task.FromResult(
                target.Entity == "Asset" && target.EntityId == "1" && target.Field == "file" &&
                (action == "Create" && previous is null && next?.Equals(Expected) == true ||
                 action == "Update" && previous?.Equals(Expected) == true && next is null));
        public Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor,
            IPhysicalSqlLease lease, BlobBindingTarget target) =>
            Task.FromResult(new BlobRetentionPolicy("default", "server_stream", 86400, 604800, 3600));
    }

    private sealed class SharedBindingPermission(BlobRef reference, BlobRetentionPolicy policy)
        : IBlobBindingAuthorization, IBlobBindingPolicyAuthorization
    {
        public string Scope(UserContext actor) => "f4-s3-upload";
        public Task<bool> AuthorizeAsync(UserContext actor, BlobBindingTarget target,
            BlobRef? previous, BlobRef? next, string action) => Task.FromResult(
                target.Entity == "Asset" && target.Field == "file" &&
                ulong.TryParse(target.EntityId, out var id) && id > 0 &&
                (action == "Create" && previous is null && next?.Equals(reference) == true ||
                 action == "Update" && previous?.Equals(reference) == true && next is null));
        public Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor,
            IPhysicalSqlLease lease, BlobBindingTarget target) => Task.FromResult(policy);
    }

    private sealed class MaintenancePermission : IBlobMaintenanceAuthorization
    {
        public string Scope(UserContext actor) => "f4-s3-upload";
        public Task<bool> CanMaintainAsync(UserContext actor, string scope) =>
            Task.FromResult(scope == "f4-s3-upload");
    }

    private sealed class ReadPermission : IBlobReadAuthorization
    {
        public bool Allowed = true;
        public Task<bool> AuthorizeAsync(UserContext actor, AuthorizedBlobSlot slot) =>
            Task.FromResult(Allowed);
    }

    private sealed class GraphAuthorizer(BlobRef reference, string entityId = "1") : IResponseGraphAuthorizer
    {
        public long BindingVersion = 1;
        public Task<AuthorizedResponseGraph> AuthorizeAsync(UserContext actor, object? result,
            ViewOptions options) => Task.FromResult(new AuthorizedResponseGraph(
                new Dictionary<string, object?> { ["document"] = reference },
                [new AuthorizedBlobSlot(["document"], "Asset", entityId, "file",
                    "f4-s3-upload", BindingVersion, reference)]));
    }

    [Theory]
    [InlineData("pdf")]
    [InlineData("production-pdf")]
    [InlineData("jpg")]
    [InlineData("jpeg")]
    [InlineData("png")]
    [InlineData("mp3")]
    [InlineData("mp4")]
    [InlineData("docx")]
    [InlineData("xlsx")]
    [InlineData("doc")]
    [InlineData("xls")]
    public async Task RealSQLiteBlobUploadThroughS3CreateOnlyConfirmAndReplay(string format)
    {
        if (Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BLOB_UPLOAD") != "1") return;
        if (format is "jpg" or "jpeg" or "png" &&
            Environment.GetEnvironmentVariable("TEAQL_S3_TEST_REAL_IMAGE") != "1") return;
        if ((format is "mp3" or "mp4") &&
            Environment.GetEnvironmentVariable("TEAQL_S3_TEST_REAL_MEDIA") != "1") return;
        if ((format is "docx" or "xlsx") &&
            Environment.GetEnvironmentVariable("TEAQL_S3_TEST_REAL_OFFICE") != "1") return;
        if ((format is "doc" or "xls") &&
            Environment.GetEnvironmentVariable("TEAQL_S3_TEST_REAL_LEGACY_OFFICE") != "1") return;
        if (format == "production-pdf" &&
            Environment.GetEnvironmentVariable("TEAQL_S3_TEST_REAL_PDF") != "1") return;
        if ((format is "doc" or "xls") && !OperatingSystem.IsLinux()) return;
        var legacyScratch = Path.Combine(Path.GetTempPath(), "teaql-s3-legacy-office-" + Guid.NewGuid().ToString("N"));
        if (OperatingSystem.IsLinux() && format is ("doc" or "xls"))
            Directory.CreateDirectory(legacyScratch, UnixFileMode.UserRead |
                UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        try
        {
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
        var clock = DateTimeOffset.UtcNow;
        var extension = format == "production-pdf" ? "pdf" : format;
        var spec = new BlobFieldSpec("Asset", "file", [extension], true, "default");
        var asset = EntityDescriptor.New("Asset").TableName("asset")
            .Property(PropertyDescriptor.New("id", DataType.U64).Id().NotNull())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("name", DataType.Text))
            .Property(PropertyDescriptor.New("file", DataType.Text).Blob(spec));
        var transport = new SqliteTransport(db);
        var actor = new RuntimeModule().Entity(asset).IntoContext().WithDataService(
            new SqlDataServiceExecutor(new SqliteDialect(), transport,
                new MetadataSchemaProvider(name => name == "Asset" ? asset : null)))
            .WithObjectStorage(provider);
        await actor.EnsureSchemaAsync();
        var store = actor.ObjectStorage("verify S3 Blob upload");
        var catalog = new SqliteBlobCatalog(transport, () => clock);
        await catalog.EnsureSchemaAsync();
        var permission = new UploadPermission { Extension = extension };
        var pdfVerifier = new PdfVerifier();
        IBlobContentVerifier verifier = format switch {
            "jpg" or "jpeg" or "png" => new StbImageBlobContentVerifier(),
            "mp3" or "mp4" => new FfmpegBlobContentVerifier(
                "/usr/bin/ffprobe", "/usr/bin/ffmpeg", "/usr/bin/prlimit"),
            "docx" or "xlsx" => new OfficeBlobContentVerifier(),
            "doc" or "xls" => new LegacyOfficeBlobContentVerifier(
                "/usr/bin/soffice", "/usr/bin/bwrap", "/usr/bin/prlimit", legacyScratch,
                maximumInputBytes: 100_000, maximumConvertedBytes: 100_000),
            "production-pdf" => new PopplerBlobContentVerifier(
                "/usr/bin/pdfinfo", "/usr/bin/pdftoppm", "/usr/bin/prlimit"),
            _ => pdfVerifier
        };
        var service = new SqliteBlobUploadService(catalog, permission, verifier, 10_000,
            now: () => clock);
        var bindingPermission = new BindingPermission();
        var readPermission = new ReadPermission();
        var maintenance = new SqliteBlobMaintenanceService(catalog,
            new MaintenancePermission(), 10, () => clock);
        var module = new BlobReferenceModule(catalog, service, readPermission,
            bindingPermission, 3, maintenance);
        module.Install(actor);
        await module.ReadyAsync(actor);
        var bytes = format == "pdf"
            ? Encoding.UTF8.GetBytes("%PDF-1.7\nreal S3 Blob upload\n%%EOF")
            : await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "Fixtures",
                "sample." + (format == "jpeg" ? "jpg" : extension)));
        var contentType = format switch {
            "jpg" or "jpeg" => "image/jpeg", "png" => "image/png",
            "mp3" => "audio/mpeg", "mp4" => "video/mp4",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "doc" => "application/msword", "xls" => "application/vnd.ms-excel",
            _ => "application/pdf"
        };
        var filename = format == "pdf" ? "report.pdf" : "sample." + extension;
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
                var decoded = await verifier.VerifyAsync(new MemoryStream(uploaded), extension, 10_000);
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
            bindingPermission.Expected = session.Reference;
            var ownerFailure = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                actor.ExecuteGraphSaveAsync<bool>("reject S3 Blob business graph", async graph =>
                {
                    await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Asset")
                        .Value("id", new Value.U64Value(1))
                        .Value("name", new Value.TextValue("document"))
                        .Value("file", session.Reference), "save asset"),
                        graph.Scope("Asset", 1, null));
                    throw new InvalidOperationException("outer owner rejected");
                }));
            Assert.Equal("outer owner rejected", ownerFailure.Message);
            Assert.Equal("0", await Scalar(db,
                "SELECT COUNT(*) FROM asset WHERE id=1", session.Reference.Key));
            Assert.Equal("0", await Scalar(db,
                "SELECT COUNT(*) FROM teaql_blob_binding WHERE key=@key", session.Reference.Key));
            Assert.Equal("uploaded_unbound", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            Assert.Equal(head.ETag, (await store.HeadAsync(key))?.ETag);
            output.WriteLine("PASS DotNet " + format + " native module outer owner rollback");
            await actor.ExecuteGraphSaveAsync("bind S3 Blob in business graph", async graph =>
            {
                await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Asset")
                    .Value("id", new Value.U64Value(1))
                    .Value("name", new Value.TextValue("document"))
                    .Value("file", session.Reference), "save asset"),
                    graph.Scope("Asset", 1, null));
                return true;
            });
            Assert.Equal(session.Reference.Key, await Scalar(db,
                "SELECT file FROM asset WHERE id=1", session.Reference.Key));
            Assert.Equal(session.Reference.Key, await Scalar(db,
                "SELECT key FROM teaql_blob_binding WHERE entity_id='1'", session.Reference.Key));
            Assert.Equal("available", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));

            var graphAuthorizer = new GraphAuthorizer(session.Reference);
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
            if (Environment.GetEnvironmentVariable("TEAQL_S3_TEST_END_TO_END_GC") == "1")
            {
                await actor.ExecuteGraphSaveAsync("unbind S3 Blob in business graph", async graph =>
                {
                    await graph.MutateAsync(new UpdateMutationRequest(new UpdateCommand("Asset",
                        new Value.U64Value(1)).ExpectedVersion(1)
                        .Value("file", new Value.NullValue()), "clear asset file"),
                        graph.Scope("Asset", 1, null));
                    return true;
                });
                await using (var business = db.CreateCommand())
                {
                    business.CommandText = "SELECT file,version FROM asset WHERE id=1";
                    await using var row = await business.ExecuteReaderAsync();
                    Assert.True(await row.ReadAsync());
                    Assert.True(row.IsDBNull(0));
                    Assert.Equal(2, row.GetInt64(1));
                }
                await using (var binding = db.CreateCommand())
                {
                    binding.CommandText = "SELECT key,version FROM teaql_blob_binding WHERE " +
                        "scope='f4-s3-upload' AND entity='Asset' AND entity_id='1' AND field='file'";
                    await using var row = await binding.ExecuteReaderAsync();
                    Assert.True(await row.ReadAsync());
                    Assert.True(row.IsDBNull(0));
                    Assert.Equal(2, row.GetInt64(1));
                }
                long lastUnbound, effective, floor;
                await using (var facts = db.CreateCommand())
                {
                    facts.CommandText = "SELECT status,last_unbound_at_ms,effective_unref_seconds," +
                        "retention_floor_until_ms FROM teaql_blob_catalog WHERE key=@key";
                    facts.Parameters.AddWithValue("@key", session.Reference.Key);
                    await using var row = await facts.ExecuteReaderAsync();
                    Assert.True(await row.ReadAsync());
                    Assert.Equal("available", row.GetString(0));
                    lastUnbound = row.GetInt64(1);
                    effective = row.GetInt64(2);
                    floor = row.GetInt64(3);
                }
                Assert.Equal(clock.ToUnixTimeMilliseconds(), lastUnbound);
                Assert.Equal(604800, effective);
                Assert.True(floor >= clock.ToUnixTimeMilliseconds() + 3600_000);
                graphAuthorizer.BindingVersion = 1;
                var unboundView = await Assert.ThrowsAsync<ViewPreparationException>(() =>
                    new ViewPreparationService().PrepareAsync(actor, null, options));
                Assert.Equal("BLOB_READ_DENIED", unboundView.Code);

                var deadline = BlobRetentionPolicy.UnreferencedEligibleAt(lastUnbound, effective, floor);
                clock = DateTimeOffset.FromUnixTimeMilliseconds(deadline - 1);
                Assert.Empty((await maintenance.RunBatchAsync(actor)).Outcomes);
                Assert.NotNull(await store.HeadAsync(key));
                clock = DateTimeOffset.FromUnixTimeMilliseconds(deadline);
                var collected = await maintenance.RunBatchAsync(actor);
                var outcome = Assert.Single(collected.Outcomes);
                Assert.Equal(session.Reference, outcome.Reference);
                Assert.Equal("deleted", outcome.Status);
                Assert.Null(await store.HeadAsync(key));
                Assert.Equal("deleted", await Scalar(db,
                    "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
                output.WriteLine("PASS DotNet " + format +
                    " S3 upload/confirm/graph bind/authorized view/unbind/retention GC");
            }
            if ((format is "doc" or "xls") &&
                Environment.GetEnvironmentVariable("TEAQL_S3_TEST_LEGACY_OFFICE_MARKER_DIR") is { } marker)
                File.WriteAllText(Path.Combine(marker, format + ".pass"), "legacy-office-real-gate-passed");
        }
        finally { await store.DeleteAsync(key); }
        }
        finally { if (Directory.Exists(legacyScratch)) Directory.Delete(legacyScratch, recursive: true); }
    }

    [Fact]
    public async Task RealSQLiteSharedBlobS3FinalUnbindRetainsUntilDeadline()
    {
        if (Environment.GetEnvironmentVariable("TEAQL_S3_TEST_SHARED_BLOB") != "1") return;
        var endpoint = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ENDPOINT");
        var bucket = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BUCKET");
        var access = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ACCESS_KEY");
        var secret = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_SECRET_KEY");
        Assert.All(new[] { endpoint, bucket, access, secret },
            value => Assert.False(string.IsNullOrEmpty(value)));
        using var provider = new S3ObjectStorage(new AmazonS3Client(
            new BasicAWSCredentials(access!, secret!), new AmazonS3Config {
                ServiceURL = endpoint, AuthenticationRegion = "us-east-1", ForcePathStyle = true
            }), bucket!);
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var spec = new BlobFieldSpec("Asset", "file", ["pdf"], true, "default");
        var asset = EntityDescriptor.New("Asset").TableName("shared_s3_asset")
            .Property(PropertyDescriptor.New("id", DataType.U64).Id().NotNull())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("file", DataType.Text).Blob(spec));
        var transport = new SqliteTransport(db);
        var actor = new RuntimeModule().Entity(asset).IntoContext().WithDataService(
            new SqlDataServiceExecutor(new SqliteDialect(), transport,
                new MetadataSchemaProvider(name => name == "Asset" ? asset : null)))
            .WithObjectStorage(provider);
        await actor.EnsureSchemaAsync();
        var now = DateTimeOffset.UtcNow;
        var policy = new BlobRetentionPolicy("default", "server_stream", 10, 20, 0);
        var catalog = new SqliteBlobCatalog(transport, () => now);
        await catalog.EnsureSchemaAsync();
        var upload = new SqliteBlobUploadService(catalog,
            new UploadPermission { Policy = policy }, new PdfVerifier(), 1024, now: () => now);
        var readPermission = new ReadPermission();
        var maintenance = new SqliteBlobMaintenanceService(catalog,
            new MaintenancePermission(), now: () => now);
        var bytes = Encoding.UTF8.GetBytes("%PDF-1.7\nshared S3 native graph\n%%EOF");
        var session = await upload.BeginAsync(actor, new BlobUploadTarget("Asset", "1", "file"),
            new BlobUploadInput("shared.pdf", bytes.Length, "application/pdf"),
            Guid.NewGuid().ToString("N"));
        var key = new ObjectKey(await Scalar(db,
            "SELECT object_key FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
        var storage = actor.ObjectStorage("shared native S3 graph");
        try
        {
            var module = new BlobReferenceModule(catalog, upload, readPermission,
                new SharedBindingPermission(session.Reference, policy), maintenance: maintenance);
            module.Install(actor);
            await module.ReadyAsync(actor);
            await upload.WriteAsync(actor, session.UploadId, new MemoryStream(bytes));
            Assert.Equal(session.Reference, await upload.CompleteAsync(actor, session.UploadId));
            Assert.Equal(bytes, await storage.GetBytesAsync(key, 1024));
            var version = (await storage.HeadAsync(key))?.ETag;
            Assert.NotNull(version);

            var ledger = new EntityRoot();
            var provisional = new[] { new EntityKey("Asset", -1), new EntityKey("Asset", -2) };
            foreach (var item in provisional)
            {
                ledger.MarkAsNew(item);
                ledger.Set(item, "file", Value.FromObject(session.Reference));
            }
            var ids = new ulong[2];
            await actor.ExecuteGraphSaveAsync("bind shared S3 Blob", async graph =>
            {
                for (var index = 0; index < 2; index++)
                {
                    ids[index] = await graph.AllocateIdAsync("Asset");
                    var actual = new EntityKey("Asset", new Value.U64Value(ids[index]));
                    ledger.Rekey(provisional[index], actual);
                    var request = new InsertMutationRequest(new InsertCommand("Asset")
                        .Value("id", ids[index]).Value("file", ledger.Change(actual)["file"]),
                        "bind shared S3 Blob") { LedgerRoot = ledger, LedgerKey = actual };
                    await graph.MutateAsync(request, graph.Scope("Asset", ids[index], null));
                }
                return true;
            });
            Assert.NotEqual(ids[0], ids[1]);
            Assert.Equal("2", await Scalar(db,
                "SELECT COUNT(*) FROM teaql_blob_binding WHERE key=@key", session.Reference.Key));
            Assert.Equal("available", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));

            async Task<long?> LastUnbound()
            {
                await using var command = db.CreateCommand();
                command.CommandText = "SELECT last_unbound_at_ms FROM teaql_blob_catalog WHERE key=@key";
                command.Parameters.AddWithValue("@key", session.Reference.Key);
                var value = await command.ExecuteScalarAsync();
                return value is DBNull or null ? null : Convert.ToInt64(value);
            }
            async Task Unbind(ulong id, bool rejectOuter = false)
            {
                await actor.ExecuteGraphSaveAsync("unbind shared S3 Blob", async graph =>
                {
                    await graph.MutateAsync(new UpdateMutationRequest(new UpdateCommand("Asset",
                        new Value.U64Value(id)).ExpectedVersion(1)
                        .Value("file", new Value.NullValue()), "unbind shared S3 Blob"),
                        graph.Scope("Asset", id, null));
                    if (rejectOuter)
                        throw new InvalidOperationException("outer owner rejected final S3 unbind");
                    return true;
                });
            }
            await Unbind(ids[0]);
            Assert.Equal("1", await Scalar(db,
                "SELECT COUNT(*) FROM teaql_blob_binding WHERE key=@key", session.Reference.Key));
            Assert.Null(await LastUnbound());
            Assert.Equal(version, (await storage.HeadAsync(key))?.ETag);

            var authorizer = new GraphAuthorizer(session.Reference, ids[1].ToString());
            actor.InsertResource<IResponseGraphAuthorizer>(authorizer);
            var options = new ViewOptions("ordinary", "read_only", DateTimeOffset.UtcNow.AddSeconds(60));
            var view = await new ViewPreparationService().PrepareAsync(actor, null, options);
            var link = Assert.IsType<string>(
                Assert.IsType<Dictionary<string, object?>>(view.Presentation)["document"]);
            using (var http = new HttpClient(new HttpClientHandler { UseProxy = false }))
            using (var request = new HttpRequestMessage(HttpMethod.Get, link))
            {
                request.Headers.TryAddWithoutValidation("Range", "bytes=0-7");
                using var response = await http.SendAsync(request);
                Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);
                Assert.Equal(bytes.AsSpan(0, 8).ToArray(), await response.Content.ReadAsByteArrayAsync());
            }
            readPermission.Allowed = false;
            var denied = await Assert.ThrowsAsync<ViewPreparationException>(() =>
                new ViewPreparationService().PrepareAsync(actor, null, options));
            Assert.Equal("BLOB_READ_DENIED", denied.Code);
            readPermission.Allowed = true;

            now = now.AddSeconds(40);
            Assert.Empty((await maintenance.RunBatchAsync(actor)).Outcomes);
            var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() => Unbind(ids[1], true));
            Assert.Equal("outer owner rejected final S3 unbind", rejected.Message);
            Assert.Equal(session.Reference.Key, await Scalar(db,
                $"SELECT file FROM shared_s3_asset WHERE id={ids[1]}", session.Reference.Key));
            Assert.Equal("1", await Scalar(db,
                "SELECT COUNT(*) FROM teaql_blob_binding WHERE key=@key", session.Reference.Key));
            Assert.Null(await LastUnbound());
            Assert.Empty((await maintenance.RunBatchAsync(actor)).Outcomes);
            Assert.Equal(version, (await storage.HeadAsync(key))?.ETag);

            await Unbind(ids[1]);
            Assert.Equal("0", await Scalar(db,
                "SELECT COUNT(*) FROM teaql_blob_binding WHERE key=@key", session.Reference.Key));
            Assert.Equal(now.ToUnixTimeMilliseconds(), await LastUnbound());
            var oldView = await Assert.ThrowsAsync<ViewPreparationException>(() =>
                new ViewPreparationService().PrepareAsync(actor, null, options));
            Assert.Equal("BLOB_READ_DENIED", oldView.Code);
            now = now.AddMilliseconds(19_999);
            Assert.Empty((await maintenance.RunBatchAsync(actor)).Outcomes);
            Assert.Equal(version, (await storage.HeadAsync(key))?.ETag);
            now = now.AddMilliseconds(1);
            var deleted = Assert.Single((await maintenance.RunBatchAsync(actor)).Outcomes);
            Assert.Equal("deleted", deleted.Status);
            Assert.Equal("deleted", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            Assert.Null(await storage.HeadAsync(key));
            output.WriteLine("PASS DotNet shared S3 native graph/final unbind/authorized Range/retention GC");
        }
        finally { await storage.DeleteAsync(key); }
    }

    private static async Task<string> Scalar(SqliteConnection db, string sql, string key)
    {
        await using var command = db.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@key", key);
        return Convert.ToString(await command.ExecuteScalarAsync())!;
    }
}
