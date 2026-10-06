using System.Text.Json.Nodes;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

// A log projection only: never modify provider parameters or business records.
internal static class LogPrivacy
{
    // Unicode scalar length, ASCII numeric IDs: shared seven-runtime mask contract.
    internal static string MaskAuditValue(string value)
    {
        var scalars = value.EnumerateRunes().ToArray();
        if (scalars.Length < 8 || scalars.All(c => c.Value >= '0' && c.Value <= '9'))
            return new string('*', scalars.Length);
        return string.Concat(scalars.Take(2)) + new string('*', scalars.Length - 4)
            + string.Concat(scalars.TakeLast(2));
    }

    internal const string EnvironmentName = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
    internal const string Acknowledgement = "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK";
    internal const string RedactedSql = "[REDACTED SQL; NOT REPLAYABLE]";
    private const string DebugLabel = "-- TeaQL DEBUG PLAINTEXT; EXPLICIT OPT-IN\n";
    private static int _warned;

    internal static bool PlaintextEnabled()
    {
        if (Environment.GetEnvironmentVariable(EnvironmentName) != Acknowledgement) return false;
        if (Interlocked.Exchange(ref _warned, 1) == 0)
            Console.Error.WriteLine("[TeaQL WARNING] Sensitive plaintext logging enabled; business data may be written to disk. Credentials remain redacted.");
        return true;
    }

    private static IEnumerable<string> Strings(JsonNode? node)
    {
        if (node is JsonObject obj)
            return obj.SelectMany(pair => Strings(pair.Value));
        if (node is JsonArray array) return array.SelectMany(Strings);
        return node == null ? [] : [node.ToString()];
    }

    internal static bool HasCredentials(JsonNode? value) => value switch
    {
        JsonObject obj => obj.Any(pair => SensitiveLogNames.IsCredential(pair.Key) || HasCredentials(pair.Value)),
        JsonArray array => array.Any(HasCredentials),
        _ => false
    };

    internal static Value Copy(Value value) => value switch
    {
        Value.JsonValue v => new Value.JsonValue(v.Value?.DeepClone()),
        Value.ObjectValue v => new Value.ObjectValue(new Record(v.Value.ToDictionary(pair => pair.Key, pair => Copy(pair.Value)))),
        Value.ListValue v => new Value.ListValue(v.Values.Select(Copy).ToList()),
        _ => value
    };

    private static Value Mask(Value value) => value switch
    {
        Value.NullValue or Value.TypedNullValue => value,
        Value.JsonValue or Value.ObjectValue => new Value.TextValue("[REDACTED]"),
        Value.ListValue v => new Value.ListValue(v.Values.Select(Mask).ToList()),
        _ => new Value.TextValue(MaskAuditValue(value.ToJsonValue()?.ToString() ?? ""))
    };

    internal static ExecutionMetadata Project(ExecutionMetadata source, bool allowPlaintext = false)
    {
        // Safe records cannot regain plaintext by being passed to a debug sink.
        if (source.LogMode == "SAFE") allowPlaintext = false;
        bool wasDebug = source.LogMode == "DEBUG PLAINTEXT"
            || source.DebugQuery?.StartsWith(DebugLabel, StringComparison.Ordinal) == true;
        var alternative = wasDebug ? SqlLogProjectionState.Restore(source) : null;
        if (wasDebug && !allowPlaintext && alternative != null) return alternative;
        var result = ProjectCore(source, allowPlaintext, wasDebug && !allowPlaintext);
        if (allowPlaintext)
        {
            // The weak-key cache retains only a fingerprint and already-safe data.
            // Reconstructed/mutated debug records have lost their original provenance.
            alternative ??= ProjectCore(source, false, wasDebug);
            SqlLogProjectionState.Remember(result, alternative);
        }
        return result;
    }

    private static ExecutionMetadata ProjectCore(ExecutionMetadata source, bool allowPlaintext, bool hideIntent)
    {
        var policies = source.ParameterLogPolicies;
        bool invalid = policies.Count != 0 && policies.Count != source.Parameters.Count;
        bool invalidMask = source.MaskedParameters.Count != 0 && source.MaskedParameters.Count != source.Parameters.Count;
        bool statementCredential = (!source.GeneratedSql || policies.Count == 0)
            && SensitiveLogNames.IsCredential(source.ParameterizedQuery ?? "");
        var values = new List<Value>();
        var masked = new List<bool>();
        var effective = new List<SqlParameterLogPolicy>();
        var sensitive = new List<string>();
        for (int i = 0; i < source.Parameters.Count; i++)
        {
            var raw = source.Parameters[i];
            var policy = invalid || invalidMask || policies.Count == 0 ? SqlParameterLogPolicy.Unknown : policies[i];
            if (!Enum.IsDefined(policy)) policy = SqlParameterLogPolicy.Unknown;
            bool forced = statementCredential || policy == SqlParameterLogPolicy.Credential
                || HasCredentials(raw.ToJsonValue());
            bool priorMasked = source.MaskedParameters.Count == source.Parameters.Count && source.MaskedParameters[i];
            bool hide = forced || invalid || invalidMask || priorMasked || policy == SqlParameterLogPolicy.Unknown
                || (!allowPlaintext && policy != SqlParameterLogPolicy.Plain);
            masked.Add(hide); effective.Add(policy);
            if (hide) sensitive.AddRange(Strings(raw.ToJsonValue()));
            values.Add(priorMasked ? Copy(raw) : !hide ? Copy(raw)
                : raw is Value.NullValue or Value.TypedNullValue ? raw
                : !forced && policy == SqlParameterLogPolicy.Masked ? Mask(raw) : new Value.TextValue("[REDACTED]"));
        }
        if (source.IntentSource is { } intentSource)
        {
            var safeIntent = Project(intentSource, allowPlaintext);
            for (int i = 0; i < intentSource.Parameters.Count; i++)
                if (safeIntent.MaskedParameters[i]) sensitive.AddRange(Strings(intentSource.Parameters[i].ToJsonValue()));
            sensitive.AddRange(intentSource.IntentValues.SelectMany(value => Strings(value.ToJsonValue())));
        }
        sensitive.AddRange(source.IntentValues.SelectMany(value => Strings(value.ToJsonValue())));
        var secrets = sensitive.Where(s => s.Length > 0).Distinct().OrderByDescending(s => s.Length).ToArray();
        string? Scrub(string? text)
        {
            if (text == null) return null;
            if (hideIntent) return "[REDACTED]";
            foreach (var value in secrets) text = text.Replace(value, "[REDACTED]", StringComparison.Ordinal);
            return text;
        }
        var sql = source.ParameterizedQuery;
        var bare = System.Text.RegularExpressions.Regex.Replace(sql ?? "", @"\$[0-9]+|@p[0-9]+", "?");
        bool unsafeInline = !source.GeneratedSql &&
            System.Text.RegularExpressions.Regex.IsMatch(bare, @"['""`$]|--|/\*|\b[0-9]+\b");
        string? reason = source.SqlOmissionReason switch
        {
            null => null,
            "missing_sql" or "policy_count_mismatch" or "mask_count_mismatch" or "untrusted_inline_sql"
                or "unsupported_literal_or_binding_mismatch" => source.SqlOmissionReason,
            _ => "previously_omitted"
        };
        reason ??= sql == null ? "missing_sql" : invalid ? "policy_count_mismatch"
            : invalidMask ? "mask_count_mismatch"
            : unsafeInline ? "untrusted_inline_sql" : null;
        string rendered = RedactedSql;
        if (reason == null)
        {
            try
            {
                rendered = SqlLogRenderer.Render(sql!, values.Count, index =>
                    SqlLogRenderer.Literal(values[index], source.Backend) + (masked[index] ? " /* masked */" : ""), source.Backend);
                var header = allowPlaintext ? DebugLabel : "-- TeaQL SAFE\n";
                if (masked.Contains(true)) header += "-- MASKED; NOT REPLAYABLE\n";
                rendered = header + rendered;
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or NotSupportedException)
            {
                reason = "unsupported_literal_or_binding_mismatch";
            }
        }
        if (reason != null) rendered = "-- TeaQL SQL OMITTED; NOT REPLAYABLE; reason=" + reason;
        return new ExecutionMetadata
        {
            Backend = source.Backend, Operation = source.Operation,
            ExecutionOutcome = source.ExecutionOutcome,
            StartedAt = source.StartedAt, EndedAt = source.EndedAt,
            ResultCount = source.ResultCount, AffectedRows = source.AffectedRows,
            BackendRequestId = Scrub(source.BackendRequestId),
            Comment = Scrub(source.Comment), Purpose = Scrub(source.Purpose), AuditReason = Scrub(source.AuditReason),
            TraceChain = source.TraceChain.Select(node => node with
            {
                Comment = Scrub(node.Comment)!, Detail = Scrub(node.Detail)!, Name = Scrub(node.Name)!,
                EntityType = Scrub(node.EntityType)!, Kind = Scrub(node.Kind)!
            }).ToList(),
            MutationLineage = Array.AsReadOnly(source.MutationLineage.Select(node => node with {
                Comment = Scrub(node.Comment)!, Detail = Scrub(node.Detail)!, Name = Scrub(node.Name)!,
                EntityType = Scrub(node.EntityType)!, Kind = Scrub(node.Kind)!
            }).ToArray()),
            ParameterizedQuery = reason == null ? sql : RedactedSql,
            Parameters = values, ParameterCount = source.ParameterCount,
            DebugQuery = rendered, ParameterLogPolicies = effective, MaskedParameters = masked,
            GeneratedSql = source.GeneratedSql, SqlOmissionReason = reason,
            LogMode = allowPlaintext ? "DEBUG PLAINTEXT" : "SAFE",
            Statements = source.Statements.Select(s => Project(s, allowPlaintext)).ToArray()
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
