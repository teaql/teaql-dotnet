using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;

namespace TeaQL.SecureDocumentReference;

public sealed record SubmittedDocumentCommand(string BusinessId, string DocumentToken,
    string CommandId, JsonElement Body)
{
    public override string ToString() => "SubmittedDocumentCommand[redacted]";
}

public sealed record DocumentCommandReceipt(long PreviousRevision, long NewRevision, string Response)
{
    public override string ToString() => "DocumentCommandReceipt[redacted]";
}

/// <summary>Explicit-schema SQLite command receipt on the current graph's physical SQL owner.</summary>
public sealed class SqliteDocumentCommandJournal(ISqlTransport transport)
{
    private const int MaximumBytes = 256000;
    private const long MaximumSafeInteger = 9007199254740991;

    private static InvalidOperationException Error(string code) => new(code);

    private static void Text(string? text, int maximum)
    {
        if (string.IsNullOrEmpty(text) || text != text.Trim() || text.Any(char.IsControl))
            throw Error("DOCUMENT_COMMAND_INVALID");
        try
        {
            if (new UTF8Encoding(false, true).GetByteCount(text) > maximum)
                throw Error("DOCUMENT_COMMAND_INVALID");
        }
        catch (EncoderFallbackException) { throw Error("DOCUMENT_COMMAND_INVALID"); }
    }

    private static string Hash(params string[] values) =>
        Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(values))).ToLowerInvariant();

    private static string ScopeHash(ReferenceDocumentScope scope)
    {
        ReferenceWire.Scope(scope);
        return Hash(scope.DocumentId, scope.Purpose, scope.AggregateType,
            scope.AggregateId.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    private static string ActorHash(TrustedReferencePrincipal principal)
    {
        ReferenceWire.Principal(principal);
        return Hash(principal.AuthenticationRealm, principal.Subject, principal.DomainRootType,
            principal.DomainRootId.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    private static void Canonical(Utf8JsonWriter writer, JsonElement input, ref int remaining, int depth)
    {
        if (--remaining < 0 || depth > 64) throw Error("DOCUMENT_COMMAND_INVALID");
        switch (input.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                var properties = input.EnumerateObject().ToArray();
                if (properties.Select(p => p.Name).Distinct(StringComparer.Ordinal).Count() != properties.Length)
                    throw Error("DOCUMENT_COMMAND_INVALID");
                foreach (var property in properties.OrderBy(p => p.Name, StringComparer.Ordinal))
                {
                    Text(property.Name, 16384);
                    if (property.Name is "__proto__" or "constructor" or "prototype")
                        throw Error("DOCUMENT_COMMAND_INVALID");
                    writer.WritePropertyName(property.Name);
                    Canonical(writer, property.Value, ref remaining, depth + 1);
                }
                writer.WriteEndObject(); return;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var child in input.EnumerateArray())
                    Canonical(writer, child, ref remaining, depth + 1);
                writer.WriteEndArray(); return;
            case JsonValueKind.String:
                var value = input.GetString()!;
                try
                {
                    if (new UTF8Encoding(false, true).GetByteCount(value) > MaximumBytes)
                        throw Error("DOCUMENT_COMMAND_INVALID");
                }
                catch (EncoderFallbackException) { throw Error("DOCUMENT_COMMAND_INVALID"); }
                writer.WriteStringValue(value); return;
            case JsonValueKind.Number:
                if (!input.TryGetInt64(out var number) ||
                    number < -MaximumSafeInteger || number > MaximumSafeInteger)
                    throw Error("DOCUMENT_COMMAND_INVALID");
                writer.WriteNumberValue(number); return;
            case JsonValueKind.True: writer.WriteBooleanValue(true); return;
            case JsonValueKind.False: writer.WriteBooleanValue(false); return;
            case JsonValueKind.Null: writer.WriteNullValue(); return;
            default: throw Error("DOCUMENT_COMMAND_INVALID");
        }
    }

    private static (string Hash, SubmittedDocumentCommand Frozen) Prepare(SubmittedDocumentCommand command)
    {
        if (command == null) throw Error("DOCUMENT_COMMAND_INVALID");
        Text(command.BusinessId, 16384); Text(command.DocumentToken, MaximumBytes);
        Text(command.CommandId, 256);
        if (command.Body.ValueKind != JsonValueKind.Object) throw Error("DOCUMENT_COMMAND_INVALID");
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            var remaining = 100000;
            Canonical(writer, command.Body, ref remaining, 0);
        }
        if (stream.Length > MaximumBytes) throw Error("DOCUMENT_COMMAND_INVALID");
        using var copied = JsonDocument.Parse(stream.ToArray());
        var frozen = command with { Body = copied.RootElement.Clone() };
        var body = Encoding.UTF8.GetString(stream.ToArray());
        if (JsonSerializer.SerializeToUtf8Bytes(new[] { command.BusinessId,
                command.DocumentToken, command.CommandId, body }).Length > MaximumBytes)
            throw Error("DOCUMENT_COMMAND_INVALID");
        return (Hash(command.BusinessId, command.DocumentToken, command.CommandId, body), frozen);
    }

    private static DocumentCommandReceipt Check(DocumentCommandReceipt receipt)
    {
        if (receipt == null || receipt.PreviousRevision < 0 ||
            receipt.NewRevision < receipt.PreviousRevision)
            throw Error("DOCUMENT_COMMAND_INVALID");
        Text(receipt.Response, MaximumBytes);
        return receipt;
    }

    public async Task EnsureSchemaAsync()
    {
        try
        {
            await transport.ExecuteSqlAsync(new CompiledQuery("""
                CREATE TABLE IF NOT EXISTS teaql_document_command_receipt (
                    scope_hash TEXT NOT NULL, actor_hash TEXT NOT NULL,
                    command_id TEXT NOT NULL, request_hash TEXT NOT NULL,
                    previous_revision BIGINT NOT NULL, new_revision BIGINT NOT NULL,
                    response TEXT NOT NULL,
                    PRIMARY KEY (scope_hash, actor_hash, command_id))
                """, [])).ConfigureAwait(false);
        }
        catch { throw Error("DOCUMENT_CATALOG_FAILED"); }
    }

    public async Task ValidateSchemaAsync()
    {
        List<Record> rows;
        try { rows = await transport.FetchAllSqlAsync(new CompiledQuery(
            "PRAGMA table_info(teaql_document_command_receipt)", [])).ConfigureAwait(false); }
        catch { throw Error("DOCUMENT_SCHEMA_MISSING"); }
        string[] expected = ["scope_hash", "actor_hash", "command_id", "request_hash",
            "previous_revision", "new_revision", "response"];
        if (rows.Count != expected.Length) throw Error("DOCUMENT_SCHEMA_MISSING");
        for (var i = 0; i < expected.Length; i++)
        {
            if (rows[i]["name"].TryText() != expected[i] || rows[i]["notnull"].TryI64() != 1 ||
                rows[i]["pk"].TryI64() != (i < 3 ? i + 1 : 0))
                throw Error("DOCUMENT_SCHEMA_MISSING");
        }
    }

    public Task<(DocumentCommandReceipt Receipt, bool Replayed)> ApplyInGraphAsync(
        GraphMutationSession graph, UserContext actor, ReferenceDocumentScope scope,
        SubmittedDocumentCommand command,
        Func<UserContext, ReferenceDocumentScope, IPhysicalSqlLease,
            Task<TrustedReferencePrincipal>> authority,
        Func<IPhysicalSqlLease, SubmittedDocumentCommand,
            Task<DocumentCommandReceipt>> mutate)
    {
        if (graph == null || actor == null || !graph.BelongsTo(actor) ||
            authority == null || mutate == null)
            throw Error("DOCUMENT_TRANSACTION_REQUIRED");
        var scopeHash = ScopeHash(scope);
        var (requestHash, frozen) = Prepare(command);
        return graph.WithPhysicalSqlAsync(async sql =>
        {
            async Task<string> CurrentActor()
            {
                try { return ActorHash(await authority(actor, scope, sql).ConfigureAwait(false)); }
                catch { throw Error("DOCUMENT_AUTHORIZATION_REQUIRED"); }
            }

            var actorHash = await CurrentActor().ConfigureAwait(false);
            List<Value> identity = [new Value.TextValue(scopeHash), new Value.TextValue(actorHash),
                new Value.TextValue(command.CommandId)];
            List<Record> rows;
            try
            {
                rows = await sql.FetchAllAsync("""
                    SELECT request_hash,previous_revision,new_revision,response
                    FROM teaql_document_command_receipt
                    WHERE scope_hash=@p0 AND actor_hash=@p1 AND command_id=@p2
                    """, identity).ConfigureAwait(false);
            }
            catch { throw Error("DOCUMENT_CATALOG_FAILED"); }
            if (rows.Count > 1) throw Error("DOCUMENT_CATALOG_FAILED");
            if (rows.Count == 1)
            {
                var row = rows[0];
                if (row["request_hash"].TryText() != requestHash)
                    throw Error("DOCUMENT_REPLAY_CONFLICT");
                if (row["previous_revision"].TryI64() is not long previous ||
                    row["new_revision"].TryI64() is not long next ||
                    row["response"].TryText() is not string response)
                    throw Error("DOCUMENT_CATALOG_FAILED");
                DocumentCommandReceipt found;
                try { found = Check(new(previous, next, response)); }
                catch { throw Error("DOCUMENT_CATALOG_FAILED"); }
                if (await CurrentActor().ConfigureAwait(false) != actorHash)
                    throw Error("DOCUMENT_AUTHORIZATION_REQUIRED");
                return (found, true);
            }

            var produced = Check(await mutate(sql, frozen).ConfigureAwait(false));
            if (Prepare(frozen).Hash != requestHash || Prepare(command).Hash != requestHash)
                throw Error("DOCUMENT_COMMAND_INVALID");
            if (await CurrentActor().ConfigureAwait(false) != actorHash)
                throw Error("DOCUMENT_AUTHORIZATION_REQUIRED");
            var parameters = identity.Concat(new Value[] { new Value.TextValue(requestHash),
                new Value.I64Value(produced.PreviousRevision), new Value.I64Value(produced.NewRevision),
                new Value.TextValue(produced.Response) }).ToArray();
            ulong affected;
            try
            {
                affected = await sql.ExecuteAsync("""
                    INSERT INTO teaql_document_command_receipt
                    (scope_hash,actor_hash,command_id,request_hash,previous_revision,new_revision,response)
                    VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)
                    """, parameters).ConfigureAwait(false);
            }
            catch { throw Error("DOCUMENT_CATALOG_FAILED"); }
            if (affected != 1) throw Error("DOCUMENT_CATALOG_FAILED");
            return (produced, false);
        });
    }
}
