using System.Text.Json.Nodes;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

// A log projection only: never modify provider parameters or business records.
internal static class LogPrivacy
{
    internal const string EnvironmentName = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
    internal const string Acknowledgement = "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK";
    internal const string RedactedSql = "[REDACTED SQL; NOT REPLAYABLE]";
    private static int _warned;

    internal static bool PlaintextEnabled()
    {
        if (Environment.GetEnvironmentVariable(EnvironmentName) != Acknowledgement) return false;
        if (Interlocked.Exchange(ref _warned, 1) == 0)
            Console.Error.WriteLine("[TeaQL WARNING] Sensitive plaintext logging enabled; business data may be written to disk. Credentials remain redacted.");
        return true;
    }

    private static bool Credential(string text)
    {
        var normalized = new string(text.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        return new[] { "password", "passwd", "passphrase", "privatekey", "secret", "accesstoken", "refreshtoken", "idtoken", "apikey", "authorization", "credential", "sessiontoken", "magiclinktoken" }
            .Any(normalized.Contains);
    }

    private static IEnumerable<string> Strings(JsonNode? node)
    {
        if (node is JsonObject obj)
            return obj.SelectMany(pair => Strings(pair.Value));
        if (node is JsonArray array) return array.SelectMany(Strings);
        return node == null ? [] : [node.ToString()];
    }

    internal static ExecutionMetadata Project(ExecutionMetadata source, bool allowPlaintext = false)
    {
        var values = source.Parameters.Select(value => value.ToJsonValue()).ToArray();
        var credentials = Credential(source.ParameterizedQuery ?? "") || Credential(source.DebugQuery ?? "")
            || values.Any(value => Credential(value?.ToJsonString() ?? ""));
        var reveal = allowPlaintext && !credentials;
        var sensitive = values.SelectMany(Strings).Where(value => value.Length > 0)
            .Distinct().OrderByDescending(value => value.Length).ToArray();
        string? Scrub(string? text)
        {
            if (reveal || text == null) return text;
            foreach (var value in sensitive) text = text.Replace(value, "[REDACTED]", StringComparison.Ordinal);
            return text;
        }
        var sql = source.ParameterizedQuery;
        // Unknown literal-bearing SQL cannot safely be reconstructed for logging.
        if (!reveal && sql != null && (sql.IndexOfAny(['\'', '"', '`', '$']) >= 0
            || sql.Contains("--") || sql.Contains("/*") || sql.Any(char.IsDigit))) sql = RedactedSql;
        return new ExecutionMetadata
        {
            Backend = source.Backend, Operation = source.Operation,
            StartedAt = source.StartedAt, EndedAt = source.EndedAt,
            ResultCount = source.ResultCount, AffectedRows = source.AffectedRows,
            BackendRequestId = Scrub(source.BackendRequestId),
            Comment = Scrub(source.Comment), Purpose = Scrub(source.Purpose), AuditReason = Scrub(source.AuditReason),
            TraceChain = source.TraceChain.Select(node => node with
            {
                Comment = Scrub(node.Comment)!, Name = Scrub(node.Name)!, EntityType = Scrub(node.EntityType)!
            }).ToList(),
            ParameterizedQuery = sql, ParameterCount = source.ParameterCount,
            Parameters = reveal ? source.Parameters.ToArray() : Array.Empty<Value>(),
            DebugQuery = reveal ? source.DebugQuery : null
        };
    }

    internal static string? ScrubAuditText(string? text, IEnumerable<Value> values)
    {
        if (text == null) return null;
        // Audit intent is not a payload channel, even during SQL troubleshooting.
        foreach (var value in values.SelectMany(value => Strings(value.ToJsonValue()))
            .Where(value => value.Length > 0).Distinct().OrderByDescending(value => value.Length))
            text = text.Replace(value, "[REDACTED]", StringComparison.Ordinal);
        return text;
    }
}
