using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Amazon.Runtime;
using Amazon.S3;
using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Core.Storage;
using TeaQL.ObjectStorage.S3;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.Sql;

static void Require(bool condition, string check)
{
    if (!condition) throw new InvalidOperationException(check);
}

static async Task Expect(StorageErrorCode code, Func<Task> work)
{
    try { await work(); }
    catch (StorageException error)
    {
        Require(error.Code == code, "unexpected storage error");
        return;
    }
    throw new InvalidOperationException("expected typed storage error");
}

static string Setting(string name) =>
    Environment.GetEnvironmentVariable(name) ?? throw new InvalidOperationException("dedicated fixture configuration required");

static string Checksum(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static async Task Verify(string database, string download, int round)
{
    const string filename = "receipt.txt";
    const string contentType = "text/plain";
    var receipt = Encoding.UTF8.GetBytes("TeaQL order receipt fixture\nItem: tea\nTotal: 12.50\n");
    var digest = Checksum(receipt);
    using var provider = new S3ObjectStorage(new AmazonS3Client(
        new BasicAWSCredentials(Setting("TEAQL_S3_ACCESS_KEY"), Setting("TEAQL_S3_SECRET_KEY")),
        new AmazonS3Config
        {
            ServiceURL = Setting("TEAQL_S3_ENDPOINT"), AuthenticationRegion = "us-east-1",
            ForcePathStyle = true, MaxErrorRetry = 0, Timeout = TimeSpan.FromSeconds(5)
        }), Setting("TEAQL_S3_BUCKET"));
    await using var sqlite = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = database }.ToString());
    await sqlite.OpenAsync();
    var module = GeneratedRuntimeModule.Module;
    var executor = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(sqlite), new ModuleSchemaProvider(module));
    var audit = new Audits();
    var observations = new Observations();
    var context = module.IntoContext().WithDataService(executor).WithAppAuditEventSink(audit)
        .WithObjectStorage(provider).WithObjectStorageObserver(observations);
    context.TraceId = $"order-attachment-{round}";
    var storage = context.ObjectStorage("file and retrieve an order receipt");
    await context.EnsureSchemaAsync();
    var platform = await Q.CommercePlatforms().WithIdIs(1).Limit(1).Comment("load the commerce platform")
        .Purpose("associate the new order to its root").ExecuteForOneAsync(context)
        ?? throw new InvalidOperationException("generated platform missing");

    var orderNumber = $"ATTACH-ORDER-{round}";
    var order = await Q.CustomerOrders().Comment("prepare receipt order")
        .Purpose("file a receipt against this order").NewEntityAsync(context);
    order.UpdateOrderNumber(orderNumber).UpdateCommercePlatform(platform.Id!.Value);
    await order.AuditAs("place the order receiving the receipt").SaveAsync(context);
    var orderId = order.Id!.Value;

    var key = new ObjectKey("private-order-attachments/" + Guid.NewGuid().ToString("N"));
    var uploaded = false;
    try
    {
        Require(!await storage.ExistsAsync(key), "new attachment key already exists");
        await storage.PutBytesAsync(key, receipt, new(receipt.Length, ObjectWriteMode.CreateOnly, contentType,
            new Dictionary<string, string> { ["receipt-sha256"] = digest }));
        uploaded = true;
        var attachment = await Q.OrderAttachments().Comment("prepare receipt attachment")
            .Purpose("save the uploaded receipt reference on its order").NewEntityAsync(context);
        attachment.UpdateCustomerOrder(orderId).UpdateFileName(filename).UpdateContentType(contentType)
            .UpdateContentLength(receipt.Length).UpdateContentSha256(digest).UpdateObjectKey(key.Value);
        await attachment.AuditAs("associate the stored receipt with its order").SaveAsync(context);
        var attachmentId = attachment.Id!.Value;
        async Task<OrderAttachment[]> Attachments() => (await Q.OrderAttachments().FilterByCustomerOrder(orderId).Limit(2)
            .Comment("load order attachments").Purpose("retrieve this order's stored receipt")
            .ExecuteForListAsync(context)).ToArray();

        var rows = await Attachments();
        Require(rows.Length == 1 && rows[0].Id == attachmentId, "persisted order association mismatch");
        var expression = E.OrderAttachment(rows[0]);
        Require(expression.CustomerOrderId().Eval() == orderId && expression.FileName().Eval() == filename,
            "stored receipt owner or filename mismatch");
        Require(expression.ContentType().Eval() == contentType && expression.ContentLength().Eval() == receipt.Length
            && expression.ContentSha256().Eval() == digest, "stored receipt bytes metadata mismatch");
        var reference = expression.ObjectKey().Eval();
        Require(reference == key.Value, "stored receipt reference mismatch");
        var persistedKey = new ObjectKey(reference!);
        var metadata = await storage.HeadAsync(persistedKey);
        Require(metadata is not null && metadata.Size == receipt.Length && metadata.ContentType == contentType
            && metadata.CustomMetadata["receipt-sha256"] == digest, "provider metadata mismatch");

        var replacement = Encoding.UTF8.GetBytes("replacement");
        await Expect(StorageErrorCode.Conflict, () => storage.PutBytesAsync(persistedKey, replacement,
            new(replacement.Length, ObjectWriteMode.CreateOnly)));
        await Expect(StorageErrorCode.TooLarge, () => storage.GetBytesAsync(persistedKey, receipt.Length - 1));
        var downloaded = await storage.GetBytesAsync(persistedKey, receipt.Length);
        Require(downloaded.SequenceEqual(receipt) && Checksum(downloaded) == digest, "retrieved receipt changed");
        await File.WriteAllBytesAsync(download, downloaded);
        await storage.DeleteAsync(persistedKey);
        uploaded = false;
        await Expect(StorageErrorCode.NotFound, () => storage.GetAsync(persistedKey));
        Require(!await storage.ExistsAsync(persistedKey), "missing object reported present");
        Require((await Attachments()).Select(item => item.Id).SequenceEqual(new long?[] { attachmentId }),
            "missing object changed the order association");
        Require(!observations.Events.IsEmpty && observations.Events.All(observation => observation.TraceId == context.TraceId),
            "storage observation escaped the attachment Context");
        Require(audit.Events.Select(item => item["entityType"]).SequenceEqual(new object?[] { "CustomerOrder", "OrderAttachment" }),
            "committed order and attachment audits missing");
        Require(!JsonSerializer.Serialize(audit.Events).Contains(key.Value, StringComparison.Ordinal)
            && !JsonSerializer.Serialize(observations.Events).Contains(key.Value, StringComparison.Ordinal),
            "private object key reached safe events");
        Console.WriteLine("ORDER_ATTACHMENT " + JsonSerializer.Serialize(new
        {
            round, order = orderId, attachment = attachmentId, orderNumber, filename, contentType,
            length = downloaded.Length, sha256 = digest, conflict = true, sizeRefused = true, missing = true,
            businessAudits = audit.Events.Count
        }));
    }
    finally
    {
        if (uploaded) await storage.DeleteAsync(key);
    }
}

try
{
    if (args.Length != 3 || !int.TryParse(args[2], out var round) || round is < 1 or > 2)
        throw new InvalidOperationException("provide database, downloaded receipt path and round");
    await Verify(args[0], args[1], round);
}
catch (Exception error)
{
    Console.Error.WriteLine(error is InvalidOperationException ? error.Message : "Order attachment example failed type=" + error.GetType().Name);
    Environment.ExitCode = 1;
}

sealed class Audits : IAppAuditEventSink
{
    public List<IReadOnlyDictionary<string, object?>> Events { get; } = [];
    public Task RecordAsync(IReadOnlyDictionary<string, object?> safeEvent, CancellationToken token = default)
    {
        if (safeEvent["entityType"] is "CustomerOrder" or "OrderAttachment") Events.Add(safeEvent);
        return Task.CompletedTask;
    }
}

sealed class Observations : IObjectStorageObserver
{
    public ConcurrentBag<ObjectStorageEvent> Events { get; } = [];
    public void Record(ObjectStorageEvent observation) => Events.Add(observation);
}
