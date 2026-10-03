using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using Microsoft.Data.Sqlite;

// Observes outputs only. No expected TraceNode is inserted into a request.
sealed class BootstrapEvidence(IDiagnosticSqlLogSink text, string database) : IDiagnosticSqlLogSink, IAppAuditEventSink
{
    public List<ExecutionMetadata> Sql { get; } = new();
    public List<IReadOnlyDictionary<string, object?>> Audits { get; } = new();

    public void Write(ExecutionMetadata metadata) { Sql.Add(metadata); text.Write(metadata); }
    public async Task RecordAsync(IReadOnlyDictionary<string, object?> record, CancellationToken token = default)
    {
        // An independent read-only connection must already see the committed
        // version when the sink is invoked, not merely after SaveAsync returns.
        var table = record["entityType"]?.ToString() switch
        {
            "Platform" => "platform_data",
            "SchoolType" => "school_type_data",
            "School" => "school_data",
            _ => throw new InvalidOperationException("Unexpected School example audit entity")
        };
        await using var observer = new SqliteConnection(new SqliteConnectionStringBuilder
        { DataSource = database, Mode = SqliteOpenMode.ReadOnly, DefaultTimeout = 1 }.ToString());
        await observer.OpenAsync(token);
        using var query = observer.CreateCommand();
        query.CommandText = $"SELECT version FROM {table} WHERE id = @id";
        query.Parameters.AddWithValue("@id", record["entityId"]!);
        var version = await query.ExecuteScalarAsync(token);
        Check(version != null && Convert.ToInt64(version) == Convert.ToInt64(record["resultVersion"]),
            "audit published before durable version was visible to an independent connection");
        Audits.Add(record);
    }
    public void Clear() { Sql.Clear(); Audits.Clear(); }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("Bootstrap evidence: " + message);
    }
    public void Verify(int writes, bool logging, int reads)
    {
        Check(Audits.Count == writes, "committed audit cardinality");
        foreach (var audit in Audits)
        {
            Check(Equals(audit["actor"], "teaql-generated-bootstrap"), "bootstrap actor");
            Check(Equals(audit["category"], "runtime-bootstrap"), "bootstrap category");
            Check(!string.IsNullOrWhiteSpace(audit["reason"]?.ToString()), "required mutation reason");
            var lineage = ((IEnumerable<TraceNode>)audit["traceChain"]!).ToArray();
            Check(lineage.Length == 1 && lineage[0].Kind == "auditReason", "automatically derived root lineage");
            Check(lineage[0].Name == audit["entityType"]?.ToString()
                && lineage[0].EntityId == Convert.ToUInt64(audit["entityId"]), "assigned entity identity");
            Check(lineage[0].Detail == audit["reason"]?.ToString(), "reason retained in lineage");
        }
        if (!logging) { Check(Sql.Count == 0, "SQL switches do not suppress committed audit"); return; }
        var mutations = Sql.Where(row => row.Operation != DataServiceOperation.Query).ToArray();
        Check(mutations.Length == writes, "physical write count");
        Check(Sql.Count - mutations.Length == reads, "lookup/readback count");
        foreach (var row in Sql)
        {
            Check(row.TraceChain.Select(node => node.Kind).SequenceEqual(
                new[] { "operation", row.Operation == DataServiceOperation.Query ? "request" : "entity", "provider", "sql" }),
                "canonical physical SQL path");
            Check(row.TraceChain[2].Name == "sqlite", "actual provider");
            if (row.Operation == DataServiceOperation.Query)
                Check(!string.IsNullOrWhiteSpace(row.Comment) && !string.IsNullOrWhiteSpace(row.Purpose), "lookup/readback intent");
            else
            {
                var entity = row.TraceChain[1];
                // Canonical SQL routes identify the entity type; the assigned
                // ID belongs to the independent mutation lineage.
                var identity = row.MutationLineage.Single();
                var audit = Audits.Single(item => item["entityType"]?.ToString() == entity.Name
                    && Convert.ToUInt64(item["entityId"]) == identity.EntityId);
                Check(!string.IsNullOrWhiteSpace(row.AuditReason), "SQL mutation reason");
                // SQL diagnostics always redact target IDs from prose. The
                // committed audit may retain a plain-schema ID in prose; both
                // retain the same typed identity. Do not demand byte equality
                // across differently governed log projections.
                var expectedSqlReason = audit["reason"]!.ToString()!.Replace(
                    identity.EntityId!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture), "[REDACTED]",
                    StringComparison.Ordinal);
                Check(row.AuditReason == expectedSqlReason, "SQL target-ID prose redaction");
                Check(identity.Detail == row.AuditReason, "SQL reason and lineage agree");
                var committed = ((IEnumerable<TraceNode>)audit["traceChain"]!).Single();
                Check((identity.Kind, identity.Name, identity.EntityId, identity.Level)
                    == (committed.Kind, committed.Name, committed.EntityId, committed.Level), "SQL/audit typed lineage agreement");
            }
        }
    }
}
