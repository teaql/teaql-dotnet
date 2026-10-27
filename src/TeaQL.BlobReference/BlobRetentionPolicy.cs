using TeaQL.Runtime;
using TeaQL.Sql;
using TeaQL.DataService;

namespace TeaQL.BlobReference;

/// <summary>Immutable host-selected retention policy and shared deadline rules.</summary>
public sealed class BlobRetentionPolicy
{
    public const long MaximumEpochMilliseconds = 8_640_000_000_000_000;
    public string Profile { get; }
    public string UploadMode { get; }
    public long UnboundRetentionSeconds { get; }
    public long UnreferencedRetentionSeconds { get; }
    public long MinimumRetentionSeconds { get; }

    public BlobRetentionPolicy(string profile, string uploadMode, long unboundRetentionSeconds,
        long unreferencedRetentionSeconds, long minimumRetentionSeconds)
    {
        if (string.IsNullOrWhiteSpace(profile) || profile != profile.Trim() ||
            System.Text.Encoding.UTF8.GetByteCount(profile) > 256 || profile.Any(char.IsControl) ||
            uploadMode is not ("server_stream" or "direct") ||
            !ValidSeconds(unboundRetentionSeconds) || !ValidSeconds(unreferencedRetentionSeconds) ||
            !ValidSeconds(minimumRetentionSeconds))
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        Profile = profile;
        UploadMode = uploadMode;
        UnboundRetentionSeconds = unboundRetentionSeconds;
        UnreferencedRetentionSeconds = unreferencedRetentionSeconds;
        MinimumRetentionSeconds = minimumRetentionSeconds;
    }

    public static bool ValidTime(long value) => value is >= 0 and <= MaximumEpochMilliseconds;
    public static bool ValidSeconds(long value) => value is >= 0 and <= MaximumEpochMilliseconds / 1000;

    public static long Deadline(long baseMilliseconds, long seconds)
    {
        if (!ValidTime(baseMilliseconds)) throw new InvalidOperationException("BLOB_REFERENCE_INVALID");
        if (!ValidSeconds(seconds) || seconds > (MaximumEpochMilliseconds - baseMilliseconds) / 1000)
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        return baseMilliseconds + seconds * 1000;
    }

    public static long EffectiveRetention(IEnumerable<long> accepted, long proposed)
    {
        if (!ValidSeconds(proposed)) throw new InvalidOperationException("BLOB_POLICY_INVALID");
        foreach (var duration in accepted)
        {
            if (!ValidSeconds(duration)) throw new InvalidOperationException("BLOB_POLICY_INVALID");
            proposed = Math.Max(proposed, duration);
        }
        return proposed;
    }

    public static long UnreferencedEligibleAt(long lastUnboundMilliseconds, long seconds,
        long? floorUntilMilliseconds)
    {
        var deadline = Deadline(lastUnboundMilliseconds, seconds);
        if (floorUntilMilliseconds is not null)
        {
            if (!ValidTime(floorUntilMilliseconds.Value))
                throw new InvalidOperationException("BLOB_REFERENCE_INVALID");
            deadline = Math.Max(deadline, floorUntilMilliseconds.Value);
        }
        return deadline;
    }

    public static void RequireBindable(string status, long nowMilliseconds, long? completedMilliseconds,
        long unboundSeconds, long? unreferencedEligibleMilliseconds)
    {
        if (!ValidTime(nowMilliseconds)) throw new InvalidOperationException("BLOB_REFERENCE_INVALID");
        if (status == "initiated") throw new InvalidOperationException("BLOB_UPLOAD_INCOMPLETE");
        if (status is "deleting" or "deleted")
            throw new InvalidOperationException("BLOB_REFERENCE_EXPIRED");
        if (status == "uploaded_unbound")
        {
            if (completedMilliseconds is null)
                throw new InvalidOperationException("BLOB_REFERENCE_INVALID");
            if (nowMilliseconds >= Deadline(completedMilliseconds.Value, unboundSeconds))
                throw new InvalidOperationException("BLOB_REFERENCE_EXPIRED");
        }
        else if (status == "available")
        {
            if (unreferencedEligibleMilliseconds is not null)
            {
                if (!ValidTime(unreferencedEligibleMilliseconds.Value))
                    throw new InvalidOperationException("BLOB_REFERENCE_INVALID");
                if (nowMilliseconds >= unreferencedEligibleMilliseconds.Value)
                    throw new InvalidOperationException("BLOB_REFERENCE_EXPIRED");
            }
        }
        else throw new InvalidOperationException("BLOB_REFERENCE_INVALID");
    }
}

/// <summary>Optional trusted upload-policy source on the physical SQL lease.</summary>
public interface IBlobUploadPolicyAuthorization
{
    Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor, ISqlTransport lease,
        BlobUploadTarget target);
}

/// <summary>Current target-field policy on the native graph's physical SQL lease.</summary>
public interface IBlobBindingPolicyAuthorization
{
    Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor, IPhysicalSqlLease lease,
        BlobBindingTarget target);
}
