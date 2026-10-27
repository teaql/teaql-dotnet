namespace TeaQL.Core;

/// <summary>Checked transitions of the signed optimistic-version carrier.</summary>
public static class MutationVersion
{
    public static long Updated(long version)
    {
        if (version <= 0) throw new MutationVersionException(MutationKind.Update, version, false);
        if (version == long.MaxValue) throw new MutationVersionException(MutationKind.Update, version, true);
        return version + 1;
    }

    public static long Deleted(long version)
    {
        if (version <= 0) throw new MutationVersionException(MutationKind.Delete, version, false);
        // Unlike -(version + 1), this stays representable at long.MaxValue.
        return -version - 1;
    }

    public static long Recovered(long version)
    {
        if (version >= 0) throw new MutationVersionException(MutationKind.Recover, version, false);
        if (version <= long.MinValue + 1) throw new MutationVersionException(MutationKind.Recover, version, true);
        return -version + 1;
    }

    public static long Next(MutationKind operation, long version) => operation switch
    {
        MutationKind.Update => Updated(version),
        MutationKind.Delete => Deleted(version),
        MutationKind.Recover => Recovered(version),
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, "Insert has no predecessor version")
    };
}

public sealed class MutationVersionException(MutationKind operation, long version, bool exhausted)
    : InvalidOperationException(exhausted
        ? $"{operation} has no representable successor for version {version}"
        : $"{operation} requires a {(operation == MutationKind.Recover ? "negative" : "positive")} version, got {version}")
{
    public MutationKind Operation { get; } = operation;
    public long Version { get; } = version;
    public bool Exhausted { get; } = exhausted;
}
