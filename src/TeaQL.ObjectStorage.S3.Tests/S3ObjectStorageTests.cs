using System.Net;
using System.Net.Sockets;
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
