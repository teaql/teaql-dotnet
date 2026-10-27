using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class MutationVersionTests
{
    [Fact]
    public void Exact_successors_keep_all_representable_endpoints()
    {
        Assert.Equal(2, MutationVersion.Updated(1));
        Assert.Equal(long.MaxValue, MutationVersion.Updated(long.MaxValue - 1));
        Assert.Equal(-2, MutationVersion.Deleted(1));
        Assert.Equal(long.MinValue, MutationVersion.Deleted(long.MaxValue));
        Assert.Equal(2, MutationVersion.Recovered(-1));
        Assert.Equal(long.MaxValue, MutationVersion.Recovered(long.MinValue + 2));
    }

    [Theory]
    [InlineData(MutationKind.Update, long.MaxValue)]
    [InlineData(MutationKind.Recover, long.MinValue)]
    [InlineData(MutationKind.Recover, long.MinValue + 1)]
    public void Exhaustion_is_explicit_and_never_wraps(MutationKind operation, long version)
    {
        var error = Assert.Throws<MutationVersionException>(() => MutationVersion.Next(operation, version));
        Assert.True(error.Exhausted); Assert.Equal(version, error.Version); Assert.Equal(operation, error.Operation);
    }

    [Theory]
    [InlineData(MutationKind.Update, long.MinValue)]
    [InlineData(MutationKind.Update, -1L)]
    [InlineData(MutationKind.Update, 0L)]
    [InlineData(MutationKind.Delete, long.MinValue)]
    [InlineData(MutationKind.Delete, -1L)]
    [InlineData(MutationKind.Delete, 0L)]
    [InlineData(MutationKind.Recover, 0L)]
    [InlineData(MutationKind.Recover, 1L)]
    public void Invalid_states_are_not_exhaustion(MutationKind operation, long version)
    {
        var error = Assert.Throws<MutationVersionException>(() => MutationVersion.Next(operation, version));
        Assert.False(error.Exhausted); Assert.Equal(version, error.Version); Assert.Equal(operation, error.Operation);
    }
}
