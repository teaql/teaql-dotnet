using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.TfpEndpoint;

const string goldenToken = "tqr1.AAAAAjMzMzMzMzMzMzMzM3bKiZgRSQQhfIj2cBXRDZIloUGHWLBp8QrXL_aejwIXPFtvV_E71O7wbOXy3cvYo_SwxvuS-89x572T9CO_pDAY4tbjWCNv";

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static async Task VerifyLogBoundaryAsync()
{
    var ordinary = new StringWriter();
    var sensitive = new StringWriter();
    var provider = new ExampleDataService();
    var context = new UserContext()
        .WithDiagnosticSqlLogSink(new TextDiagnosticSqlLogSink(ordinary))
        .WithSensitiveDiagnosticSqlLogSink(new SensitiveDiagnosticSqlLogSink(sensitive))
        .WithDataService(provider);
    await context.RequireResource<IDataService>().QueryAsync(new QueryRequest
    {
        Query = new SelectQuery("CustomerOrder").Limit(1),
        Comment = "what: demonstrate safe SQL logging",
        Purpose = "why: retain a runnable security example"
    });
    Require(!ordinary.ToString().Contains(ExampleDataService.Secret, StringComparison.Ordinal)
            && !ordinary.ToString().Contains("Debug SQL:", StringComparison.Ordinal),
        "ordinary SQL log leaked a value");
    Require(ordinary.ToString().Contains("parameterCount=1", StringComparison.Ordinal),
        "ordinary SQL log lost parameter count");
    Require(!sensitive.ToString().Contains(ExampleDataService.Secret, StringComparison.Ordinal),
        "sensitive sink bypassed plaintext acknowledgement gate");
}

static async Task VerifyTrustedTfpAsync()
{
    var provider = new ExampleDataService();
    var endpoint = new TfpEndpointHandler(provider);
    var trusted = new TrustedFederalContext
    {
        TenantField = "tenant_id",
        TenantId = new Value.I64Value(7),
        AuthenticatedUser = "example-user",
        ApprovedPurpose = "example",
        AllowedEntities = new HashSet<string> { "Order" },
        ReadableFields = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            ["Order"] = new Dictionary<string, string> { ["id"] = "id", ["status"] = "status" }
        },
        WritableFields = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            ["Order"] = new Dictionary<string, string> { ["status"] = "status" }
        },
        AllowedActions = new Dictionary<string, ISet<string>>
        {
            ["Order"] = new HashSet<string> { "Update" }
        },
        MaxPageSize = 100,
        MaxOffset = 1_000
    };
    foreach (var payload in new[]
    {
        "{\"entity\":\"Order\",\"limitValue\":10,\"tenantId\":99,\"commentText\":\"list\",\"purposeText\":\"example\"}",
        "{\"entity\":\"Order\",\"limitValue\":10,\"rawSql\":\"select * from secrets\",\"commentText\":\"list\",\"purposeText\":\"example\"}"
    })
    {
        await AssertRejectedAsync(() => endpoint.HandleQueryAsync(trusted, payload));
    }
    await endpoint.HandleMutationAsync(trusted,
        "{\"entity\":\"Order\",\"action\":\"Update\",\"id\":42,\"expectedVersion\":3,\"payload\":{\"status\":\"PAID\"},\"comment\":\"mark paid\"}");
    var update = ((UpdateMutationRequest)provider.LastMutation!).Command;
    Require(update.Guards.TryGetValue("tenant_id", out var tenant) && tenant is Value.I64Value { Value: 7 },
        "trusted tenant guard was not retained");

    var entity = EntityDescriptor.New("Order").TableName("customer_order_data")
        .Property(PropertyDescriptor.New("id", DataType.U64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version())
        .Property(PropertyDescriptor.New("status", DataType.Text))
        .Property(PropertyDescriptor.New("tenant_id", DataType.I64).ColumnName("tenant_id"));
    var compiled = new SqliteDialect().CompileUpdate(entity, update);
    var where = compiled.Sql.Split(" WHERE ", 2, StringSplitOptions.None);
    Require(where.Length == 2 && where[1].Contains("id", StringComparison.Ordinal)
        && where[1].Contains("version", StringComparison.Ordinal)
        && where[1].Contains("tenant_id", StringComparison.Ordinal),
        $"ID, version, and tenant are not guarded in one SQL statement: {compiled.Sql}");
}

static async Task AssertRejectedAsync(Func<Task<Dictionary<string, object>>> action)
{
    try
    {
        await action();
        throw new InvalidOperationException("untrusted TFP control was accepted");
    }
    catch (TfpEndpointException)
    {
        // Expected fail-closed protocol boundary.
    }
}

static void VerifyOpaqueReference()
{
    var now = new DateTimeOffset(2026, 9, 26, 12, 0, 0, TimeSpan.Zero);
    var codec = new AeadEntityReferenceCodec(2, new Dictionary<uint, byte[]>
    {
        [1] = Enumerable.Repeat((byte)0x11, 32).ToArray(),
        [2] = Enumerable.Repeat((byte)0x22, 32).ToArray()
    }).WithClock(() => now).WithNonceSource(() => Enumerable.Repeat((byte)0x33, 12).ToArray());
    var context = new UserContext().WithEntityReferenceCodec(codec);
    var token = context.EncodeEntityReference("OrderItem", 42, 7, "edit-order", TimeSpan.FromHours(1));
    Require(token == goldenToken, ".NET token does not match the portable golden vector");
    var claims = context.DecodeEntityReference(token, "OrderItem", "edit-order");
    Require(claims.Id == 42 && claims.Version == 7, "opaque reference did not round-trip");
    try
    {
        context.DecodeEntityReference(token, "OrderItem", "other-purpose");
        throw new InvalidOperationException("wrong-purpose token did not fail closed");
    }
    catch (EntityReferenceTokenException)
    {
        // Expected uniform token failure.
    }
}

static void VerifyBusinessClock()
{
    var expected = new DateTimeOffset(2026, 10, 1, 14, 20, 0, TimeSpan.FromHours(8));
    var context = new UserContext().WithBusinessClock(new FixedBusinessClock(expected));
    Require(context.BusinessTime == expected, "context ignored the fixed business clock");
    Require(context.BusinessDate == new DateOnly(2026, 10, 1),
        "business date did not derive from the context-owned clock");
}

await VerifyLogBoundaryAsync();
await VerifyTrustedTfpAsync();
VerifyOpaqueReference();
VerifyBusinessClock();
Console.WriteLine("PASS .NET security foundations example");

sealed class ExampleDataService : IDataService
{
    internal const string Secret = "4111111111111111";
    internal const string DebugSql = "SELECT * FROM customer_order_data WHERE card_number = '4111111111111111'";
    public MutationRequest? LastMutation { get; private set; }
    public DataServiceCapabilities Capabilities { get; } = new() { Query = true, Mutation = true };

    public Task<QueryResult> QueryAsync(QueryRequest request)
    {
        var started = DateTimeOffset.UtcNow;
        return Task.FromResult(new QueryResult
        {
            Metadata = new ExecutionMetadata
            {
                Backend = "example",
                Operation = DataServiceOperation.Query,
                ParameterizedQuery = "SELECT * FROM customer_order_data WHERE card_number = ?",
                Parameters = new Value[] { new Value.TextValue(Secret) },
                DebugQuery = DebugSql,
                StartedAt = started,
                EndedAt = started.AddTicks(100),
                ResultCount = 0,
                Comment = request.Comment,
                Purpose = request.Purpose
            }
        });
    }

    public Task<MutationResult> MutateAsync(MutationRequest request)
    {
        LastMutation = request;
        return Task.FromResult(new MutationResult { AffectedRows = 1 });
    }
}
