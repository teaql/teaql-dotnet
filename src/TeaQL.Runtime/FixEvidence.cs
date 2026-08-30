namespace TeaQL.Runtime;

public enum FixEvidenceSource { Clock, Context }

public sealed record FixEvidence(
    string EntityType,
    string ModelPath,
    FixEvidenceSource Source,
    string SourceLabel);
