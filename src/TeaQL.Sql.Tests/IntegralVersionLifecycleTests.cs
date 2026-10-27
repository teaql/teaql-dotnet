using TeaQL.Core;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Sql.Tests;

// The version carrier is a signed integer, not an unrestricted arithmetic value.
public sealed class IntegralVersionLifecycleTests
{
    private readonly TestSqlDialect _dialect = new();
    private static EntityDescriptor Entity() => EntityDescriptor.New("LifecycleProbe")
        .TableName("lifecycle_probe_data")
        .Property(PropertyDescriptor.New("id", DataType.U64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version())
        .Property(PropertyDescriptor.New("name", DataType.Text));

    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(-902L)]
    [InlineData(-1L)]
    [InlineData(0L)]
    [InlineData(long.MaxValue)]
    public void Update_rejects_invalid_or_exhausted_version(long version)
    {
        var command = new UpdateCommand("LifecycleProbe", new Value.U64Value(1))
            .ExpectedVersion(version).Value("name", "changed");
        Assert.Throws<SqlCompileException>(() => _dialect.CompileUpdate(Entity(), command));
    }

    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(-902L)]
    [InlineData(-1L)]
    [InlineData(0L)]
    public void Repeated_soft_delete_cannot_recover_or_rewrite_a_tombstone(long version)
    {
        var command = new DeleteCommand("LifecycleProbe", new Value.U64Value(1)).ExpectedVersion(version);
        Assert.Throws<SqlCompileException>(() => _dialect.CompileDelete(Entity(), command));
    }

    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(long.MinValue + 1)]
    public void Recovery_rejects_unrepresentable_successor(long version)
    {
        var command = new RecoverCommand { Entity = "LifecycleProbe", Id = new Value.U64Value(1), ExpectedVersionValue = version };
        Assert.Throws<SqlCompileException>(() => _dialect.CompileRecover(Entity(), command));
    }

    [Fact]
    public void Batch_update_rejects_exhaustion_in_any_row()
    {
        var command = new BatchUpdateCommand
        {
            Entity = "LifecycleProbe",
            BatchIds = new() { new Value.U64Value(1), new Value.U64Value(2) },
            BatchExpectedVersions = new() { 1, long.MaxValue },
            BatchValues = new() { new Record { ["name"] = Value.FromObject("first") }, new Record { ["name"] = Value.FromObject("second") } },
            UpdateFields = new() { "name" }
        };
        Assert.Throws<SqlCompileException>(() => _dialect.CompileBatchUpdate(Entity(), command));
    }

    [Fact]
    public void Versionless_soft_delete_still_requires_a_live_row()
    {
        var sql = _dialect.CompileDelete(Entity(), new DeleteCommand("LifecycleProbe", new Value.U64Value(1)));
        Assert.Contains("\"version\" >= 1", sql.Sql);
    }

    [Fact]
    public void Adjacent_legal_boundaries_retain_the_full_exact_domain()
    {
        var update = _dialect.CompileUpdate(Entity(), new UpdateCommand("LifecycleProbe", new Value.U64Value(1))
            .ExpectedVersion(long.MaxValue - 1).Value("name", "changed"));
        Assert.Equal(new Value.I64Value(long.MaxValue), update.Params[1]);
        var delete = _dialect.CompileDelete(Entity(), new DeleteCommand("LifecycleProbe", new Value.U64Value(1)).ExpectedVersion(long.MaxValue));
        Assert.Equal(new Value.I64Value(long.MinValue), delete.Params[0]);
        var recover = _dialect.CompileRecover(Entity(), new RecoverCommand { Entity = "LifecycleProbe", Id = new Value.U64Value(1), ExpectedVersionValue = long.MinValue + 2 });
        Assert.Equal(new Value.I64Value(long.MaxValue), recover.Params[0]);
    }

    [Fact]
    public void Hard_delete_keeps_its_distinct_meaning_for_a_tombstone()
    {
        var sql = _dialect.CompileDelete(Entity(), new DeleteCommand("LifecycleProbe", new Value.U64Value(1))
            .ExpectedVersion(long.MinValue).HardDelete());
        Assert.StartsWith("DELETE FROM ", sql.Sql);
        Assert.Equal(new Value.I64Value(long.MinValue), sql.Params[1]);
    }
}
