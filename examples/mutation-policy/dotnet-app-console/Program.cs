using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

const string policyKey = "Customer.saveGraph";
const string deniedEmail = "blocked@example.com";
const string denialCode = "CUSTOMER_EMAIL_BLOCKED";

var label = Environment.GetEnvironmentVariable("TEAQL_SHOWCASE_ROUND")
    ?? throw new InvalidOperationException("Supply an example round label");
var database = Environment.GetEnvironmentVariable("TEAQL_SHOWCASE_DATABASE")
    ?? throw new InvalidOperationException("Supply an example SQLite path");
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = database, Pooling = false,
}.ToString());
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var service = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
    new MetadataSchemaProvider(module.Metadata.GetEntity));
var context = module.IntoContext().WithDataService(service);
await context.EnsureSchemaAsync();

var identity = new MutationPolicyIdentity("example.customer-onboarding", "1", "sha256:customer-onboarding-v1");
var policy = new CustomerApproval(identity, deniedEmail, denialCode);
var audit = new ApprovalAudit();
context.WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(key => key == policyKey ? policy : null))
    .WithMutationPolicyApprovalProvider(new DelegatingMutationPolicyApprovalProvider(candidate =>
        candidate == identity ? new MutationPolicyApproval(candidate, "customer-review-owner", DateTimeOffset.UnixEpoch.AddSeconds(1)) : null))
    .WithAppAuditEventSink(audit);

var email = $"approved-{label}@example.com";
var channel = $"Email-{label}";
var approved = await PrepareCustomer(context, email, channel);
await approved.AuditAs("Approve customer onboarding and its contact preference").SaveAsync(context);

var blocked = await PrepareCustomer(context, deniedEmail, $"Rejected-{label}");
try
{
    await blocked.AuditAs("Review the blocked customer onboarding submission").SaveAsync(context);
    throw new InvalidOperationException("The blocked customer graph was accepted");
}
catch (MutationPolicyException error) when (error.Message.Contains(denialCode, StringComparison.Ordinal))
{
    // Both operations were submitted together and the graph was denied.
}
Require(policy.Reviewed.SequenceEqual(new[] { 2, 2 }), "expected complete review of both submissions");
Require(audit.Events.Count == 2, "expected audits for only the approved customer and contact");
foreach (var entry in audit.Events)
{
    Require(entry.TryGetValue("mutationGovernance", out var value)
        && value is IReadOnlyDictionary<string, object?> governance
        && Equals(governance["policyId"], identity.Id)
        && Equals(governance["approvalStatus"], MutationPolicyApprovalStatus.Approved.ToString()),
        "the approved graph identity is missing from the audit");
}

var found = await ProfileByEmail(context, email)
    ?? throw new InvalidOperationException("the approved profile was not persisted");
Require(found.Id == approved.Id && found.Email == email, "the stored profile does not match this approval");
var contact = await Q.CustomerContacts().FilterByCustomer(found.Id).WithChannelIs(channel).Limit(2)
    .Comment("Read the approved contact preference")
    .Purpose("Confirm it was saved with its customer").ExecuteForOneAsync(context)
    ?? throw new InvalidOperationException("the approved customer contact was not persisted");
Require(contact.Channel == channel, "the stored contact does not match this approval");
Require(await ProfileByEmail(context, deniedEmail) is null, "the denied customer was persisted");
var deniedContact = await Q.CustomerContacts().WithChannelIs($"Rejected-{label}").Limit(2)
    .Comment("Look for a contact belonging to the denied submission")
    .Purpose("Verify policy rejection left both business tables unchanged").ExecuteForOneAsync(context);
Require(deniedContact is null, "the denied contact was persisted");

Console.WriteLine("CUSTOMER_APPROVAL=" + JsonSerializer.Serialize(new
{
    round = label, customerId = found.Id, email, contactId = contact.Id, channel,
    reviewedOperations = policy.Reviewed, approvedAudits = audit.Events.Count, denial = denialCode,
}));

static async Task<Customer> PrepareCustomer(UserContext context, string email, string channel)
{
    var profile = await Q.Customers().Comment("Initialize the customer profile")
        .Purpose("Prepare one governed customer onboarding submission").NewEntityAsync(context);
    profile.UpdatePlatform(1).UpdateEmail(email);
    var contact = await Q.CustomerContacts().Comment("Initialize the contact preference")
        .Purpose("Include the consent data in the same customer submission").NewEntityAsync(context);
    contact.UpdateCustomer(profile.Id).UpdateChannel(channel);
    profile.CustomerContactList.Add(contact);
    return profile;
}

static Task<Customer?> ProfileByEmail(UserContext context, string email) =>
    Q.Customers().WithEmailIs(email).Limit(2).Comment("Read a customer profile by email")
        .Purpose("Check what was persisted after business approval").ExecuteForOneAsync(context);

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class CustomerApproval(MutationPolicyIdentity identity, string deniedEmail, string denialCode) : IMutationPolicy
{
    public MutationPolicyIdentity Identity { get; } = identity;
    public List<int> Reviewed { get; } = new();

    public MutationDecision Review(UserContext context, MutationPlan plan)
    {
        Reviewed.Add(plan.Operations.Count);
        var names = plan.Operations.Select(operation => operation.Entity).OrderBy(name => name, StringComparer.Ordinal);
        if (plan.RequestKey != "Customer.saveGraph" || plan.RootEntityType != "Customer"
            || !names.SequenceEqual(new[] { "Customer", "CustomerContact" })
            || plan.Operations.Any(operation => operation.Kind != MutationOperationKind.Create))
            return MutationDecision.Denied("CUSTOMER_GRAPH_INCOMPLETE", "profile and contact are both required");
        var profile = plan.Operations.Single(operation => operation.Entity == "Customer");
        return profile.ChangedValues["email"] is Value.TextValue(var email) && email == deniedEmail
            ? MutationDecision.Denied(denialCode, "reserved customer email", "Customer.email")
            : MutationDecision.Allowed();
    }
}

sealed class ApprovalAudit : IAppAuditEventSink
{
    public List<IReadOnlyDictionary<string, object?>> Events { get; } = new();

    public Task RecordAsync(IReadOnlyDictionary<string, object?> safeEvent, CancellationToken cancellationToken = default)
    {
        Events.Add(safeEvent);
        return Task.CompletedTask;
    }
}
