using System.Net;
using System.Net.Sockets;
using System.Net.Http.Headers;
using System.Text;
using Amazon.Runtime;
using Amazon.S3;
using TeaQL.Core.Storage;
using TeaQL.ObjectStorage.S3;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using Xunit;

namespace TeaQL.Tests.ObjectStorageS3;

public class S3ObjectStorageTests
{
    [Fact]
    public async Task MinioRealSignedRangeAndExpiry()
    {
        // Explicit local S3-compatible gate; the host owns the disposable bucket.
        var endpoint = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ENDPOINT");
        var bucket = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BUCKET");
        var access = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ACCESS_KEY");
        var secret = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_SECRET_KEY");
        if (new[] { endpoint, bucket, access, secret }.Any(string.IsNullOrEmpty)) return;
        using var provider = new S3ObjectStorage(new AmazonS3Client(new BasicAWSCredentials(access!, secret!),
            new AmazonS3Config { ServiceURL = endpoint, AuthenticationRegion = "us-east-1", ForcePathStyle = true }), bucket!);
        var storage = new UserContext().WithObjectStorage(provider).ObjectStorage("F4 real S3-compatible .NET");
        var key = new ObjectKey("f4/dotnet/" + Guid.NewGuid().ToString("N"));
        var bytes = Encoding.ASCII.GetBytes("%PDF-1.7\nF4 real S3-compatible bytes\n%%EOF");
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        try
        {
            var written = await storage.PutBytesAsync(key, bytes, new(bytes.Length, ObjectWriteMode.Replace, "application/pdf"));
            var head = await storage.HeadAsync(key);
            Assert.NotNull(head);
            Assert.Equal(bytes.Length, head.Size);
            Assert.Equal(written.ETag, head.ETag);
            Assert.Equal("application/pdf", head.ContentType);
            Assert.Equal(bytes, await storage.GetBytesAsync(key, 1024));
            var ticket = await storage.GetReadUrlAsync(key, new(TimeSpan.FromSeconds(2), "attachment", "report.pdf"));
            using var request = new HttpRequestMessage(HttpMethod.Get, ticket.Url);
            request.Headers.Range = new RangeHeaderValue(0, 7);
            using var response = await http.SendAsync(request);
            Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);
            Assert.Equal(bytes[..8], await response.Content.ReadAsByteArrayAsync());
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
            Assert.Contains("report.pdf", response.Content.Headers.ContentDisposition?.ToString());
            var delay = ticket.ExpiresAt.AddMilliseconds(1300) - DateTimeOffset.UtcNow;
            if (delay > TimeSpan.Zero) await Task.Delay(delay);
            using var expired = await http.GetAsync(ticket.Url);
            Assert.Equal(HttpStatusCode.Forbidden, expired.StatusCode);
        }
        finally { await storage.DeleteAsync(key); }
    }

    [Fact]
    public async Task ObjectStorageSignedUrlHonorsConfiguredProtocolBeforeSigning()
    {
        using var provider = new S3ObjectStorage(new AmazonS3Client(new BasicAWSCredentials("test", "secret"), new AmazonS3Config { ServiceURL = "http://127.0.0.1:19010", AuthenticationRegion = "us-east-1", ForcePathStyle = true }), "bucket");
        var api = new UserContext().WithObjectStorage(provider).ObjectStorage("verify signed URL configuration");
        var link = await api.GetReadUrlAsync(new ObjectKey("PRIVATE-KEY-CANARY"));
        Assert.Equal("http", link.Url.Scheme);
        Assert.Equal("127.0.0.1", link.Url.Host);
        Assert.DoesNotContain("CANARY", link.ToString());
        Assert.InRange((link.ExpiresAt - DateTimeOffset.UtcNow).TotalSeconds, 295, 301);
    }
    private sealed class WireServer : IAsyncDisposable
    {
        private readonly HttpListener _server = new();
        private readonly Task _worker;
        public int Status = 403, Calls;
        public string Endpoint { get; }
        public WireServer()
        {
            var port = new TcpListener(IPAddress.Loopback, 0); port.Start(); var number = ((IPEndPoint)port.LocalEndpoint).Port; port.Stop();
            Endpoint = $"http://127.0.0.1:{number}"; _server.Prefixes.Add(Endpoint + "/"); _server.Start();
            _worker = Task.Run(async () =>
            {
                while (_server.IsListening)
                {
                    HttpListenerContext request;
                    try { request = await _server.GetContextAsync(); } catch { break; }
                    Interlocked.Increment(ref Calls); var status = Volatile.Read(ref Status);
                    if (request.Request.Url!.AbsolutePath.TrimEnd('/') == "/bucket" && status == 404) status = 200;
                    request.Response.StatusCode = status; request.Response.ContentLength64 = 0; request.Response.Close();
                }
            });
        }
        public async ValueTask DisposeAsync() { _server.Close(); await _worker; }
    }
    private sealed class FailingInput : MemoryStream
    {
        public override ValueTask<int> ReadAsync(Memory<byte> data, CancellationToken token = default) => ValueTask.FromException<int>(new IOException("PRIVATE-INPUT-CANARY"));
    }
    private static async Task Expect(StorageErrorCode code, Func<Task> work)
    { var error = await Assert.ThrowsAsync<StorageException>(work); Assert.Equal(code, error.Code); Assert.Null(error.InnerException); Assert.DoesNotContain("CANARY", error.ToString()); }
    [Fact]
    public async Task ObjectStorageActualHttpErrorsAndInvalidStreamsAreSafe()
    {
        await using var server = new WireServer();
        using var provider = new S3ObjectStorage(new AmazonS3Client(new BasicAWSCredentials("test", "secret"), new AmazonS3Config { ServiceURL = server.Endpoint, AuthenticationRegion = "us-east-1", ForcePathStyle = true, MaxErrorRetry = 0, Timeout = TimeSpan.FromSeconds(3) }), "bucket");
        var api = new UserContext().WithObjectStorage(provider).ObjectStorage("verify HTTP error semantics"); var key = new ObjectKey("PRIVATE-KEY-CANARY");
        foreach (var (status, code) in new[] { (403, StorageErrorCode.AccessDenied), (500, StorageErrorCode.Unavailable), (503, StorageErrorCode.Unavailable), (408, StorageErrorCode.Timeout), (504, StorageErrorCode.Timeout), (501, StorageErrorCode.Unsupported) })
        { Volatile.Write(ref server.Status, status); await Expect(code, () => api.ExistsAsync(key)); }
        Volatile.Write(ref server.Status, 404); Assert.False(await api.ExistsAsync(key));
        Volatile.Write(ref server.Status, 200); await Expect(StorageErrorCode.Unavailable, () => api.ExistsAsync(key));
        var before = Volatile.Read(ref server.Calls);
        await Expect(StorageErrorCode.InvalidInput, () => api.PutBytesAsync(key, [1, 2], new(1, ObjectWriteMode.CreateOnly)));
        await Expect(StorageErrorCode.InvalidInput, () => api.PutBytesAsync(key, [1], new(2, ObjectWriteMode.CreateOnly)));
        var input = new FailingInput(); await Expect(StorageErrorCode.Io, () => api.PutAsync(key, input, new(1, ObjectWriteMode.CreateOnly))); Assert.False(input.CanRead);
        Assert.Equal(before, Volatile.Read(ref server.Calls));
        await server.DisposeAsync(); await Expect(StorageErrorCode.Unavailable, () => api.ExistsAsync(key));
    }
}
