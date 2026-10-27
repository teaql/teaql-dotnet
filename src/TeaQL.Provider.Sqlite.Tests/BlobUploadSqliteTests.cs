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
        public bool CheckSqlPolicy;
        public string[] Extensions = ["pdf"];
        public string Scope(UserContext actor) => CurrentScope;
        public BlobFieldSpec Field(UserContext actor, BlobUploadTarget target) =>
            new("Asset", "file", Extensions, true, "default");
        public Task<bool> AuthorizeAsync(UserContext actor, BlobUploadTarget target,
            string action, BlobUploadInput input) => Task.FromResult(Allowed &&
                target.Entity == "Asset" && target.Field == "file" &&
                action is "Begin" or "Write" or "Confirm");
        public async Task<BlobUploadAuthorizationState> CurrentOnLeaseAsync(UserContext actor,
            BlobUploadTarget target, string action, BlobUploadInput input, ISqlTransport lease)
        {
            var allowed = await AuthorizeAsync(actor, target, action, input);
            if (CheckSqlPolicy)
            {
                var rows = await lease.FetchAllSqlAsync(new CompiledQuery(
                    "SELECT allowed FROM upload_policy", []));
                allowed &= rows.Count == 1 && rows[0]["allowed"].TryI64() == 1;
            }
            return new(Scope(actor), Field(actor, target), allowed);
        }
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

    private sealed class PausedReadStream(byte[] bytes) : MemoryStream(bytes)
    {
        public TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private bool _paused;
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            if (!_paused)
            {
                _paused = true;
                Entered.SetResult();
                await Release.Task.WaitAsync(cancellationToken);
            }
            return await base.ReadAsync(buffer, cancellationToken);
        }
    }

    private sealed class FailedReplayStream : MemoryStream
    {
        public override ValueTask<int> ReadAsync(Memory<byte> buffer,
            CancellationToken cancellationToken = default) =>
            throw new IOException("PRIVATE-REPLAY-SOURCE-CANARY");
    }

    private sealed class CountingStore(IObjectStorageProvider inner) : IObjectStorageProvider
    {
        public int Puts;
        public Task<ObjectMetadata> PutAsync(StorageInvocation invocation, ObjectKey key,
            Stream content, ObjectPutOptions options)
        { Puts++; return inner.PutAsync(invocation, key, content, options); }
        public Task<ObjectContent> GetAsync(StorageInvocation invocation, ObjectKey key) =>
            inner.GetAsync(invocation, key);
        public Task<ObjectMetadata?> HeadAsync(StorageInvocation invocation, ObjectKey key) =>
            inner.HeadAsync(invocation, key);
        public Task DeleteAsync(StorageInvocation invocation, ObjectKey key) =>
            inner.DeleteAsync(invocation, key);
        public Task<ObjectReadUrl> GetReadUrlAsync(StorageInvocation invocation, ObjectKey key,
            ObjectReadUrlOptions options) => inner.GetReadUrlAsync(invocation, key, options);
    }

    private sealed class LostCommitTransport(ISqlTransactionTransport inner) : ISqlTransactionTransport
    {
        public bool LoseNextCommitReply;
        public Task<List<TeaQL.Core.Record>> FetchAllSqlAsync(CompiledQuery query) => inner.FetchAllSqlAsync(query);
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public async Task<ISqlTransaction> BeginSqlAsync()
        {
            var tx = await inner.BeginSqlAsync();
            if (!LoseNextCommitReply) return tx;
            LoseNextCommitReply = false;
            return new LostCommitTx(tx);
        }

        private sealed class LostCommitTx(ISqlTransaction inner) : ISqlTransaction
        {
            public Task<List<TeaQL.Core.Record>> FetchAllSqlAsync(CompiledQuery query) => inner.FetchAllSqlAsync(query);
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
            public async Task CommitSqlAsync()
            {
                await inner.CommitSqlAsync();
                throw new IOException("PRIVATE-COMMIT-CANARY");
            }
            public Task RollbackSqlAsync() => inner.RollbackSqlAsync();
            public void Dispose() => inner.Dispose();
        }
    }

    private sealed class PausedVerifier : IBlobContentVerifier
    {
        public TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int Calls;
        public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
            long maximumBytes)
        {
            Calls++;
            Entered.SetResult();
            await Release.Task;
            return await new PdfVerifier().VerifyAsync(content, extension, maximumBytes);
        }
    }

    private static async Task<string> Scalar(SqliteConnection db, string sql, string key)
    {
        await using var command = db.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@key", key);
        return (string)(await command.ExecuteScalarAsync())!;
    }

    private static byte[] ImageFixture(string extension) => File.ReadAllBytes(
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample." +
            (extension == "jpeg" ? "jpg" : extension)));

    [Theory]
    [InlineData("jpg", "image/jpeg")]
    [InlineData("jpeg", "image/jpeg")]
    [InlineData("png", "image/png")]
    public async Task RealImageVerifierDecodesFullPixelsAndOriginalDigest(string extension, string mime)
    {
        var bytes = ImageFixture(extension);
        var verifier = new StbImageBlobContentVerifier();
        var verified = await verifier.VerifyAsync(new MemoryStream(bytes), extension, 100_000);
        Assert.Equal(bytes.Length, verified.SizeBytes);
        Assert.Equal(Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(), verified.Sha256);
        Assert.Equal(mime, verified.ContentType);
        Assert.Equal("BLOB_FORMAT_NOT_ALLOWED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => verifier.VerifyAsync(new MemoryStream(bytes[..^2]), extension, 100_000))).Message);
        Assert.Equal("BLOB_FORMAT_NOT_ALLOWED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => verifier.VerifyAsync(new MemoryStream(ImageFixture(extension == "png" ? "jpg" : "png")),
                extension, 100_000))).Message);
        Assert.Equal("BLOB_SIZE_EXCEEDED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => verifier.VerifyAsync(new MemoryStream(bytes), extension, bytes.Length - 1))).Message);
    }

    [Fact]
    public async Task RealImageVerifierBoundsPixelsAndConcurrentAdmissions()
    {
        var bytes = ImageFixture("png");
        var verifier = new StbImageBlobContentVerifier(maximumDecodedBytes: 4);
        Assert.Equal("BLOB_FORMAT_NOT_ALLOWED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => verifier.VerifyAsync(new MemoryStream(bytes), "png", 100_000))).Message);
        verifier = new StbImageBlobContentVerifier(concurrentVerifications: 1);
        var slow = new PausedReadStream(bytes);
        var first = verifier.VerifyAsync(slow, "png", 100_000);
        await slow.Entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal("BLOB_VERIFICATION_BUSY", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => verifier.VerifyAsync(new MemoryStream(bytes), "png", 100_000))).Message);
        slow.Release.SetResult();
        Assert.Equal(bytes.Length, (await first).SizeBytes);
    }

    [Fact]
    public async Task RealImageVerifierRejectsPngChunkCrcMutation()
    {
        var bytes = ImageFixture("png");
        // The pinned 16x16 fixture starts its IDAT payload at byte 41.
        bytes[41] ^= 1;
        var verifier = new StbImageBlobContentVerifier();
        Assert.Equal("BLOB_FORMAT_NOT_ALLOWED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => verifier.VerifyAsync(new MemoryStream(bytes), "png", 100_000))).Message);
    }

    [Theory]
    [InlineData("jpg")]
    [InlineData("png")]
    public async Task RealImageUploadCompletesOnlyAfterFullDecode(string extension)
    {
        var root = Path.Combine(Path.GetTempPath(), "teaql-image-upload-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            await using var db = new SqliteConnection("Data Source=:memory:");
            await db.OpenAsync();
            var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
            await catalog.EnsureSchemaAsync();
            await using var store = new FileObjectStorage(Path.Combine(root, "private"),
                RandomNumberGenerator.GetBytes(32));
            var actor = new UserContext().WithObjectStorage(store);
            var policy = new UploadAuthorization { Extensions = ["jpg", "png"] };
            var service = new SqliteBlobUploadService(catalog, policy,
                new StbImageBlobContentVerifier(), 100_000);
            var bytes = ImageFixture(extension);
            var target = new BlobUploadTarget("Asset", "1", "file");
            var input = new BlobUploadInput("sample." + extension, bytes.Length);
            var session = await service.BeginAsync(actor, target, input, "image-valid");
            await service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes));
            Assert.Equal(session.Reference, await service.CompleteAsync(actor, session.UploadId));
            Assert.Equal("uploaded_unbound", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));

            var corrupt = bytes[..^2];
            var invalid = await service.BeginAsync(actor, target,
                new BlobUploadInput("sample." + extension, corrupt.Length), "image-truncated");
            await service.WriteAsync(actor, invalid.UploadId, new MemoryStream(corrupt));
            Assert.Equal("BLOB_FORMAT_NOT_ALLOWED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, invalid.UploadId))).Message);
            Assert.Equal("initiated", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", invalid.Reference.Key));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Fact]
    public async Task TwoConnectionsSerializeWriteAndConfirmationAcrossExternalIO()
    {
        var root = Path.Combine(Path.GetTempPath(), "teaql-blob-owner-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var path = Path.Combine(root, "upload.db");
            await using var db1 = new SqliteConnection($"Data Source={path};Default Timeout=5");
            await using var db2 = new SqliteConnection($"Data Source={path};Default Timeout=5");
            await db1.OpenAsync();
            await db2.OpenAsync();
            var catalog1 = new SqliteBlobCatalog(new SqliteTransport(db1));
            var catalog2 = new SqliteBlobCatalog(new SqliteTransport(db2));
            await catalog1.EnsureSchemaAsync();
            await using var store = new FileObjectStorage(Path.Combine(root, "private"),
                RandomNumberGenerator.GetBytes(32));
            var actor1 = new UserContext().WithObjectStorage(store);
            var actor2 = new UserContext().WithObjectStorage(store);
            var policy = new UploadAuthorization { CheckSqlPolicy = true };
            await using (var command = db1.CreateCommand())
            {
                command.CommandText = "CREATE TABLE upload_policy (allowed INTEGER NOT NULL); INSERT INTO upload_policy VALUES (1)";
                await command.ExecuteNonQueryAsync();
            }
            var verifier = new PausedVerifier();
            var first = new SqliteBlobUploadService(catalog1, policy, verifier, 10_000);
            var second = new SqliteBlobUploadService(catalog2, policy, verifier, 10_000);
            var bytes = Encoding.UTF8.GetBytes("%PDF-1.7\nconcurrent content\n%%EOF");
            var target = new BlobUploadTarget("Asset", "1", "file");
            var input = new BlobUploadInput("report.pdf", bytes.Length, "application/pdf");
            var denied = await first.BeginAsync(actor1, target, input, "policy-change");
            await using (var command = db1.CreateCommand())
            {
                command.CommandText = "UPDATE upload_policy SET allowed=0";
                await command.ExecuteNonQueryAsync();
            }
            Assert.Equal("BLOB_UPLOAD_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => first.WriteAsync(actor1, denied.UploadId, new MemoryStream(bytes)))).Message);
            await using (var command = db1.CreateCommand())
            {
                command.CommandText = "UPDATE upload_policy SET allowed=1";
                await command.ExecuteNonQueryAsync();
            }
            var session = await first.BeginAsync(actor1, target, input, "concurrent");
            var pausedSource = new PausedReadStream(bytes);
            var firstWrite = first.WriteAsync(actor1, session.UploadId, pausedSource);
            await pausedSource.Entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
            var competingWrite = Task.Run(() => second.WriteAsync(actor2, session.UploadId,
                new MemoryStream(bytes)));
            await Task.Delay(75);
            Assert.False(competingWrite.IsCompleted, "competing write passed the SQL owner during File I/O");
            pausedSource.Release.SetResult();
            Assert.Equal(bytes.Length, await firstWrite.WaitAsync(TimeSpan.FromSeconds(5)));
            Assert.Equal(bytes.Length, await competingWrite.WaitAsync(TimeSpan.FromSeconds(5)));

            var firstConfirm = first.CompleteAsync(actor1, session.UploadId);
            await verifier.Entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
            var competingConfirm = Task.Run(() => second.CompleteAsync(actor2, session.UploadId));
            await Task.Delay(75);
            Assert.False(competingConfirm.IsCompleted,
                "competing confirmation passed the SQL owner during parser I/O");
            verifier.Release.SetResult();
            Assert.Equal(session.Reference, await firstConfirm.WaitAsync(TimeSpan.FromSeconds(5)));
            Assert.Equal(session.Reference, await competingConfirm.WaitAsync(TimeSpan.FromSeconds(5)));
            Assert.Equal(1, verifier.Calls);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Fact]
    public async Task CommittedWriteWithLostReplyReplaysOnlySameBytesAndGeneration()
    {
        var root = Path.Combine(Path.GetTempPath(), "teaql-blob-commit-unknown-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            await using var db = new SqliteConnection("Data Source=:memory:");
            await db.OpenAsync();
            var transport = new LostCommitTransport(new SqliteTransport(db));
            var catalog = new SqliteBlobCatalog(transport);
            await catalog.EnsureSchemaAsync();
            await using var file = new FileObjectStorage(Path.Combine(root, "private"),
                RandomNumberGenerator.GetBytes(32));
            var store = new CountingStore(file);
            var actor = new UserContext().WithObjectStorage(store);
            var policy = new UploadAuthorization();
            var verifier = new PdfVerifier();
            var service = new SqliteBlobUploadService(catalog, policy, verifier, 10_000);
            var bytes = Encoding.UTF8.GetBytes("%PDF-1.7\nSQL commit succeeded, reply lost\n%%EOF");
            var session = await service.BeginAsync(actor, new BlobUploadTarget("Asset", "1", "file"),
                new BlobUploadInput("report.pdf", bytes.Length), "unknown-write");
            transport.LoseNextCommitReply = true;
            Assert.Equal("BLOB_CATALOG_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)))).Message);
            Assert.Equal("written", await Scalar(db,
                "SELECT state FROM teaql_blob_upload WHERE key=@key", session.Reference.Key));
            Assert.Equal("initiated", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key));
            Assert.Equal(1, store.Puts);
            policy.Allowed = false;
            Assert.Equal("BLOB_UPLOAD_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)))).Message);
            policy.Allowed = true;
            Assert.Equal("BLOB_IDEMPOTENCY_CONFLICT", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, session.UploadId,
                    new MemoryStream(Enumerable.Repeat((byte)'x', bytes.Length).ToArray())))).Message);
            Assert.Equal("BLOB_IDEMPOTENCY_CONFLICT", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes[..^1])))).Message);
            Assert.Equal("BLOB_STORE_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, session.UploadId, new FailedReplayStream()))).Message);
            Assert.Equal(bytes.Length, await service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)));
            Assert.Equal(1, store.Puts);
            transport.LoseNextCommitReply = true;
            Assert.Equal("BLOB_CATALOG_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, session.UploadId))).Message);
            Assert.Equal("completed", await Scalar(db,
                "SELECT state FROM teaql_blob_upload WHERE key=@key", session.Reference.Key));
            policy.Allowed = false;
            Assert.Equal("BLOB_UPLOAD_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteAsync(actor, session.UploadId))).Message);
            policy.Allowed = true;
            Assert.Equal(session.Reference, await service.CompleteAsync(actor, session.UploadId));
            Assert.Equal(1, verifier.Calls);
            Assert.Equal(bytes.Length, await service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)));
            Assert.Equal(1, verifier.Calls);
            var objectKey = await Scalar(db,
                "SELECT object_key FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key);
            await actor.ObjectStorage("replace File generation").PutBytesAsync(new ObjectKey(objectKey),
                bytes, new ObjectPutOptions(bytes.Length, ObjectWriteMode.Replace, "application/pdf"));
            Assert.Equal("BLOB_IDEMPOTENCY_CONFLICT", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)))).Message);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Fact]
    public async Task RetryRecoversOnlyIdenticalFileAfterJournalPromotionRollback()
    {
        var root = Path.Combine(Path.GetTempPath(), "teaql-blob-recover-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            await using var db = new SqliteConnection("Data Source=:memory:");
            await db.OpenAsync();
            var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
            await catalog.EnsureSchemaAsync();
            await using var store = new FileObjectStorage(Path.Combine(root, "private"),
                RandomNumberGenerator.GetBytes(32));
            var actor = new UserContext().WithObjectStorage(store);
            var verifier = new PdfVerifier();
            var service = new SqliteBlobUploadService(catalog, new UploadAuthorization(), verifier, 10_000);
            var bytes = Encoding.UTF8.GetBytes("%PDF-1.7\nrecover committed File content\n%%EOF");
            var session = await service.BeginAsync(actor, new BlobUploadTarget("Asset", "1", "file"),
                new BlobUploadInput("report.pdf", bytes.Length), "recovery-request");
            await using (var command = db.CreateCommand())
            {
                command.CommandText = """
                    CREATE TRIGGER reject_upload_write BEFORE UPDATE ON teaql_blob_upload
                    WHEN NEW.state='written' BEGIN SELECT RAISE(ABORT, 'injected'); END
                    """;
                await command.ExecuteNonQueryAsync();
            }
            Assert.Equal("BLOB_CATALOG_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes)))).Message);
            var objectKey = await Scalar(db,
                "SELECT object_key FROM teaql_blob_catalog WHERE key=@key", session.Reference.Key);
            var stable = await actor.ObjectStorage("check recovery candidate").HeadAsync(new ObjectKey(objectKey));
            Assert.NotNull(stable);
            Assert.Equal("initiated", await Scalar(db,
                "SELECT state FROM teaql_blob_upload WHERE key=@key", session.Reference.Key));
            await using (var command = db.CreateCommand())
            {
                command.CommandText = "DROP TRIGGER reject_upload_write";
                await command.ExecuteNonQueryAsync();
            }
            Assert.Equal("BLOB_STORE_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.WriteAsync(actor, session.UploadId,
                    new MemoryStream(Enumerable.Repeat((byte)'x', bytes.Length).ToArray())))).Message);
            Assert.Equal(bytes.Length, await service.WriteAsync(actor, session.UploadId,
                new MemoryStream(bytes)));
            Assert.Equal(stable.ETag, (await actor.ObjectStorage("check recovered generation")
                .HeadAsync(new ObjectKey(objectKey)))?.ETag);
            Assert.Equal(session.Reference, await service.CompleteAsync(actor, session.UploadId));
            Assert.Equal(1, verifier.Calls);
        }
        finally { Directory.Delete(root, recursive: true); }
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
