using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;

namespace TeaQL.SecureDocumentReference;

public sealed record DocumentOpenRequest(string AggregateType, string BusinessId,
    string Purpose, TimeSpan Lifetime);
public sealed record DocumentAcceptRequest(string AggregateType, string Purpose,
    SubmittedDocumentCommand Submitted);

public sealed record ContextualDocument(string BusinessId, string DocumentToken, JsonElement Body)
{
    public override string ToString() => "ContextualDocument[redacted]";
}

public sealed record AcceptedDocument(ContextualDocument Document,
    long PreviousRevision, long NewRevision, bool Replayed)
{
    public override string ToString() => "AcceptedDocument[redacted]";
}

public sealed record CurrentBoundDocument(BoundDocumentSnapshot Document,
    JsonElement Body, TimeSpan Lifetime);
public sealed record VerifiedDocumentBlobSelection(BlobSelectionTarget Target, BlobRef Reference,
    string RowKey, string EntityId, long ExpectedVersion, long NextVersion)
{
    public override string ToString() => "VerifiedDocumentBlobSelection[redacted]";
}
public sealed record DocumentBlobSelectionClaim(BlobSelectionTarget Target, BlobRef Reference)
{
    public override string ToString() => "DocumentBlobSelectionClaim[redacted]";
}

/// <summary>Application-owned current projection and allowed model mutation plan.</summary>
public interface ISqliteContextBoundDocumentHost
{
    TrustedReferencePrincipal Principal(UserContext actor);
    Task AuthorizeAsync(UserContext actor, TrustedReferencePrincipal principal,
        ReferenceDocumentScope scope, IPhysicalSqlLease sql);
    Task<CurrentBoundDocument> LoadAsync(UserContext actor, string businessId,
        IPhysicalSqlLease sql);
    Task ApplyAsync(UserContext actor, BoundDocumentSnapshot issued,
        SubmittedDocumentCommand submitted, IPhysicalSqlLease sql);
    Task PresentAsync(UserContext actor, ContextualDocument document);
}

/// <summary>Optional trusted model-field and Blob-catalog binding on the current lease.</summary>
public interface ISqliteDocumentBlobSelectionHost
{
    Task BindBlobSelectionAsync(UserContext actor, GraphMutationSession graph,
        VerifiedDocumentBlobSelection selection, IPhysicalSqlLease sql);
}

/// <summary>Resolve a sealed client key after its child is persisted on the current lease.</summary>
public interface ISqliteDocumentNewBlobSelectionHost
{
    Task<(string RowKey, string EntityId)> ResolveNewBlobSelectionAsync(UserContext actor,
        BoundDocumentSnapshot issued, SubmittedDocumentCommand submitted,
        DocumentBlobSelectionClaim selection, IPhysicalSqlLease sql);
}

/// <summary>Current SQL-authorized tqd2 open and durable accept on the native graph owner.</summary>
public sealed class SqliteContextBoundDocumentService
{
    private sealed class TransactionLease(ISqlTransaction transaction) : IPhysicalSqlLease
    {
        public Task<List<Record>> FetchAllAsync(string sql, IReadOnlyList<Value> parameters) =>
            transaction.FetchAllSqlAsync(new CompiledQuery(sql, parameters.ToList()));
        public Task<ulong> ExecuteAsync(string sql, IReadOnlyList<Value> parameters) =>
            transaction.ExecuteSqlAsync(new CompiledQuery(sql, parameters.ToList()));
    }

    private readonly UserContext _actor;
    private readonly ISqlTransactionTransport _transport;
    private readonly SecureDocumentReferenceModule _module;
    private readonly SqliteDocumentCommandJournal _journal;
    private readonly ISqliteContextBoundDocumentHost _host;

    public SqliteContextBoundDocumentService(UserContext actor, ISqlTransactionTransport transport,
        SecureDocumentReferenceModule module, SqliteDocumentCommandJournal journal,
        ISqliteContextBoundDocumentHost host)
    {
        _actor = actor ?? throw new InvalidOperationException("DOCUMENT_CONFIGURATION");
        _transport = transport ?? throw new InvalidOperationException("DOCUMENT_CONFIGURATION");
        _module = module ?? throw new InvalidOperationException("DOCUMENT_CONFIGURATION");
        _journal = journal ?? throw new InvalidOperationException("DOCUMENT_CONFIGURATION");
        _host = host ?? throw new InvalidOperationException("DOCUMENT_CONFIGURATION");
    }

    private static InvalidOperationException Error(string code) => new(code);

    private static void Text(string? text)
    {
        if (string.IsNullOrEmpty(text) || text != text.Trim() || text.Any(char.IsControl))
            throw Error("DOCUMENT_COMMAND_INVALID");
        try { DocumentWire.Text(text); }
        catch { throw Error("DOCUMENT_COMMAND_INVALID"); }
    }

    private TrustedReferencePrincipal Current(UserContext actor)
    {
        if (!ReferenceEquals(actor, _actor)) throw Error("DOCUMENT_CONFIGURATION");
        try
        {
            var principal = _host.Principal(actor);
            ReferenceWire.Principal(principal);
            return principal;
        }
        catch { throw Error("DOCUMENT_AUTHORIZATION_REQUIRED"); }
    }

    private static byte[] Fingerprint(BoundDocumentSnapshot document) =>
        DocumentWire.EncodeBound(document, new byte[32], 0, 1);

    private IReadOnlyList<DocumentBlobSelectionClaim> VerifiedSelections(
        TrustedReferencePrincipal principal, BoundDocumentSnapshot issued,
        SubmittedDocumentCommand command)
    {
        var result = new List<DocumentBlobSelectionClaim>();
        var seen = new HashSet<(string, string)>();
        foreach (var selected in command.BlobSelections ?? [])
        {
            var target = new BlobSelectionTarget(issued.Snapshot.Scope,
                selected.TargetEntity, selected.CanonicalField, selected.Kind,
                selected.TargetKey, command.CommandId);
            if (!seen.Add((target.TargetKey, target.CanonicalField)))
                throw Error("DOCUMENT_COMMAND_INVALID");
            var claim = _module.ConsumeBlobSelection(principal, command.DocumentToken,
                selected.Token, target).Document;
            result.Add(new DocumentBlobSelectionClaim(target, claim.Reference));
        }
        return result;
    }

    private static VerifiedDocumentBlobSelection MapExisting(BoundDocumentSnapshot issued,
        DocumentBlobSelectionClaim selection)
    {
        var target = selection.Target;
        var row = issued.Snapshot.Entities.SingleOrDefault(e => e.RowKey == target.TargetKey);
        var binding = issued.Bindings.SingleOrDefault(b => b.RowKey == target.TargetKey &&
            b.CanonicalField == target.CanonicalField);
        if (row == null || binding == null || row.Identity.EntityType != target.TargetEntity ||
            !binding.Writable || binding.BindingVersion >= long.MaxValue)
            throw Error("DOCUMENT_PROJECTION_VIOLATION");
        return new VerifiedDocumentBlobSelection(target, selection.Reference, target.TargetKey,
            row.Identity.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
            (long)binding.BindingVersion, (long)binding.BindingVersion + 1);
    }

    private static VerifiedDocumentBlobSelection MapNew(BoundDocumentSnapshot issued,
        DocumentBlobSelectionClaim selection, (string RowKey, string EntityId) resolved)
    {
        try { Text(resolved.RowKey); }
        catch { throw Error("DOCUMENT_PROJECTION_VIOLATION"); }
        if (!ulong.TryParse(resolved.EntityId, System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture, out var id) || id == 0 ||
            id.ToString(System.Globalization.CultureInfo.InvariantCulture) != resolved.EntityId ||
            issued.Snapshot.Entities.Any(e => e.RowKey == resolved.RowKey))
            throw Error("DOCUMENT_PROJECTION_VIOLATION");
        return new VerifiedDocumentBlobSelection(selection.Target, selection.Reference,
            resolved.RowKey, resolved.EntityId, 0, 1);
    }

    private static bool ValidNewChild(BoundDocumentSnapshot issued, BoundDocumentSnapshot after,
        VerifiedDocumentBlobSelection selection)
    {
        var row = after.Snapshot.Entities.SingleOrDefault(e => e.RowKey == selection.RowKey);
        return row != null && row.Identity.EntityType == selection.Target.TargetEntity &&
            row.Identity.Id.ToString(System.Globalization.CultureInfo.InvariantCulture) == selection.EntityId &&
            row.WritableFields.Contains(selection.Target.CanonicalField) &&
            after.Snapshot.Relations.Any(relation =>
                relation.RowKeys.Contains(selection.RowKey) && relation.Completeness == "COMPLETE" &&
                relation.Writable && relation.AllowedMutations.Contains("CREATE_CHILD") &&
                issued.Snapshot.Relations.Any(previous => previous.Name == relation.Name &&
                    previous.Completeness == "COMPLETE" && previous.Writable &&
                    previous.AllowedMutations.Contains("CREATE_CHILD") &&
                    !previous.RowKeys.Contains(selection.RowKey)));
    }

    private static JsonElement Validate(CurrentBoundDocument current, string businessId,
        string aggregateType, string purpose)
    {
        if (current == null || current.Document?.Snapshot == null ||
            current.Document.Snapshot.BusinessId != businessId ||
            current.Document.Snapshot.Scope.AggregateType != aggregateType ||
            current.Document.Snapshot.Scope.Purpose != purpose || current.Lifetime <= TimeSpan.Zero)
            throw Error("DOCUMENT_PROJECTION_VIOLATION");
        try
        {
            _ = Fingerprint(current.Document);
            return SqliteDocumentCommandJournal.CopyBody(current.Body);
        }
        catch { throw Error("DOCUMENT_PROJECTION_VIOLATION"); }
    }

    public async Task<ContextualDocument> OpenAsync(UserContext actor, DocumentOpenRequest request)
    {
        if (request == null || request.Lifetime <= TimeSpan.Zero)
            throw Error("DOCUMENT_COMMAND_INVALID");
        Text(request.AggregateType); Text(request.BusinessId); Text(request.Purpose);
        var principal = Current(actor);
        using var tx = await _transport.BeginSqlAsync().ConfigureAwait(false);
        var committed = false;
        try
        {
            var sql = new TransactionLease(tx);
            var current = await _host.LoadAsync(actor, request.BusinessId, sql).ConfigureAwait(false);
            var body = Validate(current, request.BusinessId, request.AggregateType, request.Purpose);
            if (Current(actor) != principal) throw Error("DOCUMENT_AUTHORIZATION_REQUIRED");
            try
            {
                await _host.AuthorizeAsync(actor, principal, current.Document.Snapshot.Scope, sql)
                    .ConfigureAwait(false);
            }
            catch { throw Error("DOCUMENT_AUTHORIZATION_REQUIRED"); }
            if (Current(actor) != principal) throw Error("DOCUMENT_AUTHORIZATION_REQUIRED");
            var lifetime = request.Lifetime < current.Lifetime ? request.Lifetime : current.Lifetime;
            var token = _module.IssueBoundDocumentSnapshot(principal, current.Document, lifetime);
            var result = new ContextualDocument(request.BusinessId, token, body);
            await tx.CommitSqlAsync().ConfigureAwait(false);
            committed = true;
            await _host.PresentAsync(actor, result).ConfigureAwait(false);
            return result;
        }
        finally
        {
            if (!committed)
                try { await tx.RollbackSqlAsync().ConfigureAwait(false); }
                catch { /* Preserve the read or commit failure. */ }
        }
    }

    public async Task<AcceptedDocument> AcceptAsync(UserContext actor, DocumentAcceptRequest request)
    {
        if (request == null || request.Submitted == null) throw Error("DOCUMENT_COMMAND_INVALID");
        Text(request.AggregateType); Text(request.Purpose); Text(request.Submitted.BusinessId);
        var principal = Current(actor);
        var issued = _module.ConsumeBoundDocumentSnapshot(principal,
            request.Submitted.DocumentToken, request.AggregateType, request.Purpose).Document;
        var scope = issued.Snapshot.Scope;
        if (issued.Snapshot.BusinessId != request.Submitted.BusinessId)
            throw Error("DOCUMENT_PROJECTION_VIOLATION");

        var (receipt, replayed) = await actor.ExecuteGraphSaveAsync(
            "accept protected document command", graph => _journal.ApplyInGraphAsync(
                graph, actor, scope, request.Submitted,
                async (_, expected, sql) =>
                {
                    var current = Current(actor);
                    if (current != principal || expected != scope)
                        throw Error("DOCUMENT_AUTHORIZATION_REQUIRED");
                    await _host.AuthorizeAsync(actor, current, expected, sql).ConfigureAwait(false);
                    return current;
                },
                async (sql, frozen) =>
                {
                    var before = await _host.LoadAsync(actor, frozen.BusinessId, sql).ConfigureAwait(false);
                    _ = Validate(before, frozen.BusinessId, request.AggregateType, request.Purpose);
                    if (!Fingerprint(before.Document).SequenceEqual(Fingerprint(issued)))
                        throw Error("DOCUMENT_REVISION_CONFLICT");
                    var selections = VerifiedSelections(principal, issued, frozen);
                    if (selections.Count > 0 && _host is not ISqliteDocumentBlobSelectionHost)
                        throw Error("DOCUMENT_CAPABILITY_MISSING");
                    if (selections.Any(s => s.Target.Kind == "NEW_CLIENT_KEY") &&
                        _host is not ISqliteDocumentNewBlobSelectionHost)
                        throw Error("DOCUMENT_CAPABILITY_MISSING");
                    await _host.ApplyAsync(actor, issued, frozen, sql).ConfigureAwait(false);
                    var mapped = new List<VerifiedDocumentBlobSelection>();
                    var mappedTargets = new HashSet<(string, string)>();
                    foreach (var selection in selections)
                    {
                        var bound = selection.Target.Kind == "EXISTING_ROW"
                            ? MapExisting(issued, selection)
                            : MapNew(issued, selection, await ((ISqliteDocumentNewBlobSelectionHost)_host)
                                .ResolveNewBlobSelectionAsync(actor, issued, frozen, selection, sql)
                                .ConfigureAwait(false));
                        if (!mappedTargets.Add((bound.RowKey, bound.Target.CanonicalField)))
                            throw Error("DOCUMENT_COMMAND_INVALID");
                        mapped.Add(bound);
                    }
                    if (_host is ISqliteDocumentBlobSelectionHost binder)
                        foreach (var selection in mapped)
                            await binder.BindBlobSelectionAsync(actor, graph, selection, sql)
                                .ConfigureAwait(false);
                    var after = await _host.LoadAsync(actor, frozen.BusinessId, sql).ConfigureAwait(false);
                    var body = Validate(after, frozen.BusinessId, request.AggregateType, request.Purpose);
                    var next = after.Document.Snapshot.Scope;
                    if (next.DocumentId != scope.DocumentId || next.AggregateId != scope.AggregateId ||
                        next.AggregateType != scope.AggregateType || next.Purpose != scope.Purpose ||
                        next.AggregateRevision < scope.AggregateRevision)
                        throw Error("DOCUMENT_PROJECTION_VIOLATION");
                    foreach (var selection in mapped)
                    {
                        var binding = after.Document.Bindings.SingleOrDefault(b =>
                            b.RowKey == selection.RowKey &&
                            b.CanonicalField == selection.Target.CanonicalField);
                        if (binding == null || binding.EntityType != selection.Target.TargetEntity ||
                            binding.Reference?.Key != selection.Reference.Key ||
                            binding.BindingVersion != (ulong)selection.NextVersion)
                            throw Error("DOCUMENT_PROJECTION_VIOLATION");
                        if (selection.Target.Kind == "NEW_CLIENT_KEY" &&
                            !ValidNewChild(issued, after.Document, selection))
                            throw Error("DOCUMENT_PROJECTION_VIOLATION");
                    }
                    try { await _host.AuthorizeAsync(actor, principal, next, sql).ConfigureAwait(false); }
                    catch { throw Error("DOCUMENT_AUTHORIZATION_REQUIRED"); }
                    var token = _module.IssueBoundDocumentSnapshot(principal,
                        after.Document, after.Lifetime);
                    var response = JsonSerializer.Serialize(new
                    {
                        businessId = frozen.BusinessId, documentToken = token, body
                    });
                    return new DocumentCommandReceipt(scope.AggregateRevision,
                        next.AggregateRevision, response);
                })).ConfigureAwait(false);

        try
        {
            using var response = JsonDocument.Parse(receipt.Response);
            var value = response.RootElement;
            if (value.ValueKind != JsonValueKind.Object || value.EnumerateObject().Count() != 3 ||
                !value.TryGetProperty("businessId", out var businessId) ||
                !value.TryGetProperty("documentToken", out var tokenValue) ||
                !value.TryGetProperty("body", out var bodyValue) ||
                businessId.GetString() != request.Submitted.BusinessId ||
                tokenValue.ValueKind != JsonValueKind.String)
                throw Error("DOCUMENT_CATALOG_FAILED");
            var token = tokenValue.GetString()!;
            if (Current(actor) != principal) throw Error("DOCUMENT_AUTHORIZATION_REQUIRED");
            var verified = _module.ConsumeBoundDocumentSnapshot(principal,
                token, request.AggregateType, request.Purpose).Document;
            var next = verified.Snapshot.Scope;
            if (verified.Snapshot.BusinessId != request.Submitted.BusinessId ||
                next.DocumentId != scope.DocumentId || next.AggregateId != scope.AggregateId ||
                next.AggregateRevision != receipt.NewRevision ||
                receipt.PreviousRevision != scope.AggregateRevision)
                throw Error("DOCUMENT_CATALOG_FAILED");
            var document = new ContextualDocument(request.Submitted.BusinessId,
                token, SqliteDocumentCommandJournal.CopyBody(bodyValue));
            await _host.PresentAsync(actor, document).ConfigureAwait(false);
            return new AcceptedDocument(document, receipt.PreviousRevision,
                receipt.NewRevision, replayed);
        }
        catch (Exception error) { throw new GraphCommittedException([error]); }
    }
}
