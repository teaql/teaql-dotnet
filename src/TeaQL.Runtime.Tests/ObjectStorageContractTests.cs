using TeaQL.Core.Storage;
using TeaQL.Runtime.Storage;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class ObjectStorageContractTests
{
    private sealed class Stub : IObjectStorageProvider
    {
        public long Size = 3;
        public int Deletes;
        public StorageErrorCode? Error;
        public StorageInvocation? Seen;
        public Task<ObjectMetadata> PutAsync(StorageInvocation invocation, ObjectKey key, Stream content, ObjectPutOptions options) { Seen = invocation; return Task.FromResult(new ObjectMetadata(key, options.Size)); }
        public Task<ObjectContent> GetAsync(StorageInvocation invocation, ObjectKey key) => Task.FromResult(new ObjectContent(new(key, Size), new MemoryStream([1, 2, 3])));
        public Task<ObjectMetadata?> HeadAsync(StorageInvocation invocation, ObjectKey key) { Seen = invocation; if (Error is { } error) throw new StorageException(error); return Task.FromResult<ObjectMetadata?>(null); }
        public Task DeleteAsync(StorageInvocation invocation, ObjectKey key) { Deletes++; return Task.CompletedTask; }
        public Task<ObjectReadUrl> GetReadUrlAsync(StorageInvocation invocation, ObjectKey key, ObjectReadUrlOptions options) => throw new StorageException(StorageErrorCode.Unsupported);
    }
    [Fact]
    public async Task ConditionalDeleteRequiresProviderCapability()
    {
        var stub = new Stub();
        var api = new UserContext().WithObjectStorage(stub).ObjectStorage("conditional delete");
        var key = new ObjectKey("f4/conditional");
        Assert.Equal(StorageErrorCode.InvalidInput,
            (await Assert.ThrowsAsync<StorageException>(() => api.DeleteIfVersionAsync(key, "*"))).Code);
        Assert.Equal(StorageErrorCode.Unsupported,
            (await Assert.ThrowsAsync<StorageException>(() => api.DeleteIfVersionAsync(key, "\"etag\""))).Code);
        Assert.Equal(0, stub.Deletes);
    }
    private sealed class Observer : IObjectStorageObserver
    {
        public List<ObjectStorageEvent> Events = [];
        public bool Fail;
        public void Record(ObjectStorageEvent observation) { if (Fail) throw new Exception("observer failure"); Events.Add(observation); }
    }
    [Fact]
    public async Task ObjectStorageRetainsContextAndFailureCategories()
    {
        Assert.Equal(StorageErrorCode.NotConfigured, Assert.Throws<StorageException>(() => new UserContext().ObjectStorage("read")).Code);
        var stub = new Stub(); var context = new UserContext().WithObjectStorage(stub);
        Assert.Equal(StorageErrorCode.InvalidInput, Assert.Throws<StorageException>(() => context.ObjectStorage("  ")).Code);
        var api = context.ObjectStorage("read attachment"); var key = new ObjectKey("PRIVATE-KEY-CANARY");
        Assert.False(await api.ExistsAsync(key)); Assert.Same(context, stub.Seen!.Context); Assert.Equal("read attachment", stub.Seen.Comment);
        foreach (var code in new[] { StorageErrorCode.AccessDenied, StorageErrorCode.Unavailable })
        { stub.Error = code; var error = await Assert.ThrowsAsync<StorageException>(() => api.ExistsAsync(key)); Assert.Equal(code, error.Code); Assert.Null(error.InnerException); }
    }
    [Fact]
    public async Task ObjectStorageStreamBoundsOwnershipCompletionAndObserver()
    {
        var stub = new Stub(); var observer = new Observer(); var api = new UserContext().WithObjectStorage(stub).WithObjectStorageObserver(observer).ObjectStorage("read attachment"); var key = new ObjectKey("PRIVATE-KEY-CANARY");
        var input = new MemoryStream(); await api.PutAsync(key, input, new(0, ObjectWriteMode.CreateOnly)); Assert.False(input.CanRead);
        await using (var response = await api.GetAsync(key)) { Assert.Equal(1, await response.Stream.ReadAsync(new byte[1])); }
        Assert.Equal("cancelled", observer.Events.Last().Outcome);
        Assert.Equal(new byte[] { 1, 2, 3 }, await api.GetBytesAsync(key, 3)); Assert.Equal("success", observer.Events.Last().Outcome);
        Assert.Equal(StorageErrorCode.TooLarge, (await Assert.ThrowsAsync<StorageException>(() => api.GetBytesAsync(key, 2))).Code);
        foreach (var size in new[] { 2, 4 }) { stub.Size = size; Assert.Equal(StorageErrorCode.Io, (await Assert.ThrowsAsync<StorageException>(() => api.GetBytesAsync(key, 5))).Code); }
        Assert.DoesNotContain("CANARY", string.Join(";", observer.Events)); observer.Fail = true; Assert.False(await api.ExistsAsync(key));
    }
    [Fact]
    public void ObjectStorageValidatesHeadersAndMasksRepresentations()
    {
        foreach (var key in new[] { "", new string('x', 1025), "bad\nkey", "\ud800" }) Assert.Throws<StorageException>(() => new ObjectKey(key));
        Assert.Equal(StorageErrorCode.TooLarge, Assert.Throws<StorageException>(() => new ObjectPutOptions(ObjectPutOptions.MaximumSize + 1, ObjectWriteMode.CreateOnly)).Code);
        Assert.Throws<StorageException>(() => new ObjectPutOptions(0, ObjectWriteMode.CreateOnly, "bad\r\nheader"));
        Assert.Throws<StorageException>(() => new ObjectPutOptions(0, ObjectWriteMode.CreateOnly, metadata: new Dictionary<string, string> { ["Bad Key"] = "value" }));
        foreach (var lifetime in new[] { TimeSpan.Zero, TimeSpan.FromTicks(1), TimeSpan.FromDays(8) }) Assert.Throws<StorageException>(() => new ObjectReadUrlOptions(lifetime));
        Assert.Throws<StorageException>(() => new ObjectReadUrlOptions(filename: "bad\nname", disposition: "attachment"));
        var keyValue = new ObjectKey("PRIVATE-KEY-CANARY"); var source = new Dictionary<string, string> { ["tag"] = "PRIVATE-METADATA-CANARY" }; var options = new ObjectPutOptions(0, ObjectWriteMode.CreateOnly, metadata: source); source["tag"] = "changed"; Assert.Equal("PRIVATE-METADATA-CANARY", options.Metadata["tag"]);
        foreach (var value in new object[] { keyValue, options, new ObjectMetadata(keyValue, 0), new ObjectReadUrl(new Uri("https://example.invalid/PRIVATE-URL-CANARY"), DateTimeOffset.UtcNow) }) Assert.DoesNotContain("CANARY", value.ToString());
    }
}
