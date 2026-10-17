using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using TeaQL.Core.Storage;
using TeaQL.ObjectStorage.S3;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;

static void Require(bool condition, string check) { if (!condition) throw new InvalidOperationException("object storage assertion: " + check); }
static async Task Expect(StorageErrorCode code, Func<Task> work)
{
    try { await work(); } catch (StorageException error) { Require(error.Code == code, "failure category"); return; }
    throw new InvalidOperationException("expected storage failure: " + code);
}
static async Task Run()
{
    var bucket = Environment.GetEnvironmentVariable("TEAQL_S3_BUCKET")!;
    var client = new AmazonS3Client(new BasicAWSCredentials(Environment.GetEnvironmentVariable("TEAQL_S3_ACCESS_KEY"), Environment.GetEnvironmentVariable("TEAQL_S3_SECRET_KEY")), new AmazonS3Config { ServiceURL = Environment.GetEnvironmentVariable("TEAQL_S3_ENDPOINT"), AuthenticationRegion = "us-east-1", ForcePathStyle = true, MaxErrorRetry = 0, Timeout = TimeSpan.FromSeconds(5) });
    try { await client.HeadBucketAsync(new HeadBucketRequest { BucketName = bucket }); }
    catch (AmazonS3Exception error) when (error.StatusCode == System.Net.HttpStatusCode.NotFound) { await client.PutBucketAsync(new PutBucketRequest { BucketName = bucket }); }
    using var provider = new S3ObjectStorage(client, bucket); var observer = new Observer();
    var context = new UserContext { TraceId = "storage-example" }.WithObjectStorage(provider).WithObjectStorageObserver(observer);
    var api = context.ObjectStorage("verify binary object storage"); var prefix = "dotnet-example/" + Guid.NewGuid().ToString("N") + "/";
    var key = new ObjectKey(prefix + "收据 +% binary"); var empty = new ObjectKey(prefix + "empty"); var race = new ObjectKey(prefix + "race");
    var data = Enumerable.Range(0, 256 * 1024).Select(i => (byte)(i % 251)).ToArray();
    try
    {
        Require(!await api.ExistsAsync(key), "missing object"); await Expect(StorageErrorCode.NotFound, () => api.GetAsync(key));
        await api.PutBytesAsync(key, data, new(data.Length, ObjectWriteMode.CreateOnly, metadata: new Dictionary<string, string> { ["example"] = "binary" }));
        var metadata = await api.HeadAsync(key); Require(metadata?.Size == data.Length && metadata.CustomMetadata["example"] == "binary" && metadata.ETag is not null, "metadata");
        Require((await api.GetBytesAsync(key, data.Length)).SequenceEqual(data), "binary bytes");
        await Expect(StorageErrorCode.Conflict, () => api.PutBytesAsync(key, [], new(0, ObjectWriteMode.CreateOnly)));
        await Expect(StorageErrorCode.TooLarge, () => api.GetBytesAsync(key, 1));
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var link = await api.GetReadUrlAsync(key, new(disposition: "attachment", filename: "receipt.bin"));
        using (var response = await http.GetAsync(link.Url)) { Require(response.IsSuccessStatusCode && (await response.Content.ReadAsByteArrayAsync()).SequenceEqual(data), "URL download"); Require(response.Content.Headers.ContentDisposition?.ToString() == "attachment; filename=receipt.bin" || response.Content.Headers.ContentDisposition?.ToString() == "attachment; filename=\"receipt.bin\"", "URL disposition"); }
        var expired = await api.GetReadUrlAsync(key, new(TimeSpan.FromSeconds(1))); await Task.Delay(2200);
        using (var response = await http.GetAsync(expired.Url)) Require(response.StatusCode == System.Net.HttpStatusCode.Forbidden, "URL expiry");
        await api.PutBytesAsync(empty, [], new(0, ObjectWriteMode.CreateOnly)); Require((await api.GetBytesAsync(empty, 0)).Length == 0, "empty object");
        await Expect(StorageErrorCode.InvalidInput, () => api.PutBytesAsync(key, [1, 2], new(1, ObjectWriteMode.Replace))); Require((await api.GetBytesAsync(key, data.Length)).SequenceEqual(data), "invalid upload preserved bytes");
        await api.PutBytesAsync(key, [1, 2, 3], new(3, ObjectWriteMode.Replace)); Require((await api.GetBytesAsync(key, 3)).SequenceEqual(new byte[] { 1, 2, 3 }), "replacement");
        var other = new UserContext { TraceId = "storage-other" }.WithObjectStorage(provider).WithObjectStorageObserver(observer).ObjectStorage("verify atomic create");
        async Task<bool> Create(TeaQL.Runtime.Storage.ObjectStorage storage) { try { await storage.PutBytesAsync(race, [1], new(1, ObjectWriteMode.CreateOnly)); return true; } catch (StorageException error) { Require(error.Code == StorageErrorCode.Conflict, "concurrent conflict"); return false; } }
        var results = await Task.WhenAll(Create(api), Create(other)); Require(results.Count(result => result) == 1, "one creator");
        var missing = new UserContext().WithObjectStorage(new S3ObjectStorage(client, "missing-" + Guid.NewGuid().ToString("N"))).ObjectStorage("verify missing bucket"); await Expect(StorageErrorCode.Unavailable, () => missing.ExistsAsync(key));
        foreach (var item in new[] { key, empty, race }) { await api.DeleteAsync(item); await api.DeleteAsync(item); Require(!await api.ExistsAsync(item), "repeated delete"); }
        Require(observer.Events.Select(e => e.TraceId).Distinct().Order().SequenceEqual(new[] { "storage-example", "storage-other" }.Order()), "distinct context observations");
        Require(!string.Join(";", observer.Events).Contains(prefix), "safe events");
        Console.WriteLine("PASS .NET S3: binary, empty, metadata, URL download/expiry, atomic create, replace, bounded reads, missing bucket, repeated delete");
    }
    finally { foreach (var item in new[] { key, empty, race }) await api.DeleteAsync(item); }
}
try { await Run(); }
catch (StorageException error) { Console.Error.WriteLine(error.Message); Environment.ExitCode = 1; }
catch (Exception error)
{
    Console.Error.WriteLine(error is InvalidOperationException ? error.Message : "Object storage example failed type=" + error.GetType().Name);
    // Stack frames identify the failing assertion without retaining SDK messages or URLs.
    Console.Error.WriteLine(error.StackTrace);
    Environment.ExitCode = 1;
}

sealed class Observer : IObjectStorageObserver
{
    public System.Collections.Concurrent.ConcurrentBag<ObjectStorageEvent> Events { get; } = [];
    public void Record(ObjectStorageEvent observation) => Events.Add(observation);
}
