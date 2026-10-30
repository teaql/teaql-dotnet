using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public sealed class ModeledBlobSqlTests
{
    [Fact]
    public async Task TypedBlobKeyStaysInSqlAndNativeWritesFailClosedWithoutParticipant()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var spec = new BlobFieldSpec("Asset", "document", ["pdf", "docx"], true, "default");
        var asset = EntityDescriptor.New("Asset").TableName("asset_data")
            .Property(PropertyDescriptor.New("id", DataType.U64).Id().NotNull())
            .Property(PropertyDescriptor.New("document", DataType.Text).Blob(spec));
        var control = EntityDescriptor.New("Control").TableName("control_data")
            .Property(PropertyDescriptor.New("id", DataType.U64).Id().NotNull());
        var dialect = new SqliteDialect();
        var transport = new SqliteTransport(db);
        foreach (var descriptor in new[] { asset, control })
        {
            await using var ddl = db.CreateCommand();
            ddl.CommandText = dialect.CompileCreateTable(descriptor);
            await ddl.ExecuteNonQueryAsync();
        }

        var reference = new BlobRef("blob_dotnet_typed_1");
        var typed = Value.FromObject(reference);
        Assert.IsType<Value.BlobRefValue>(typed);
        var command = new InsertCommand("Asset").Value("id", new Value.U64Value(1))
            .Value("document", typed);
        var compiled = dialect.CompileInsert(asset, command);
        Assert.Contains("[REDACTED]", compiled.DebugSql(DatabaseKind.Sqlite));
        Assert.DoesNotContain(reference.Key, compiled.DebugSql(DatabaseKind.Sqlite));
        Assert.Equal(SqlParameterLogPolicy.Credential, compiled.ParameterLogPolicies[1]);
        Assert.Equal(1UL, await transport.ExecuteSqlAsync(compiled));
        await using (var read = db.CreateCommand())
        {
            read.CommandText = "SELECT document FROM asset_data WHERE id=1";
            Assert.Equal(reference.Key, await read.ExecuteScalarAsync());
        }

        Assert.Throws<SqlCompileException>(() => dialect.CompileInsert(asset,
            new InsertCommand("Asset").Value("id", 2UL).Value("document", reference.Key)));
        Assert.Throws<SqlCompileException>(() => dialect.CompileUpdate(asset,
            new UpdateCommand("Asset", new Value.U64Value(1)).Value("document", reference.Key)));
        Assert.Throws<SqlCompileException>(() => dialect.CompileUpdate(asset,
            new UpdateCommand("Asset", new Value.U64Value(1))
                .Value("document", typed).Guard("document", Value.FromObject(reference.Key))));
        Assert.Throws<SqlCompileException>(() => dialect.CompileInsert(asset,
            new InsertCommand("Asset").Value("id", 2UL)
                .Value("document", new Value.TypedNullValue(DataType.Text))));
        Assert.Throws<InvalidOperationException>(() => typed.ToJsonValue());
        Assert.Throws<SqlCompileException>(() => dialect.CompileBatchInsert(asset,
            new BatchInsertCommand { Entity = "Asset", BatchValues =
            [
                new TeaQL.Core.Record { ["id"] = new Value.U64Value(2), ["document"] = typed },
                new TeaQL.Core.Record { ["id"] = new Value.U64Value(3),
                    ["document"] = Value.FromObject(reference.Key) }
            ] }));
        Assert.NotNull(dialect.CompileUpdate(asset,
            new UpdateCommand("Asset", new Value.U64Value(1)).Value("document", new Value.NullValue())));
        var untrusted = EntityDescriptor.New("Asset").TableName("asset_data")
            .Property(PropertyDescriptor.New("id", DataType.U64).Id())
            .Property(PropertyDescriptor.New("document", DataType.BlobRef));
        Assert.Throws<SqlCompileException>(() => dialect.CompileInsert(untrusted, command));
        var requiredSpec = new BlobFieldSpec("Asset", "document", ["pdf"], false, "default");
        var required = EntityDescriptor.New("Asset").TableName("asset_data")
            .Property(PropertyDescriptor.New("id", DataType.U64).Id())
            .Property(PropertyDescriptor.New("document", DataType.Text).Blob(requiredSpec));
        Assert.Throws<SqlCompileException>(() => dialect.CompileUpdate(required,
            new UpdateCommand("Asset", new Value.U64Value(1)).Value("document", new Value.NullValue())));

        var provider = new SqlDataServiceExecutor(dialect, transport,
            new MetadataSchemaProvider(name => name == "Asset" ? asset : name == "Control" ? control : null));
        using var owner = await provider.BeginTransactionAsync();
        await owner.MutateAsync(new InsertMutationRequest(
            new InsertCommand("Control").Value("id", 1UL), "create control"));
        var rejected = await Assert.ThrowsAsync<SqlExecutorException>(() => owner.MutateAsync(
            new InsertMutationRequest(new InsertCommand("Asset").Value("id", 2UL)
                .Value("document", reference), "create modeled Blob")));
        Assert.Equal("BLOB_CAPABILITY_MISSING", rejected.Message);
        await Assert.ThrowsAsync<InvalidOperationException>(() => owner.CommitAsync());
        await owner.RollbackAsync();
        await using (var count = db.CreateCommand())
        {
            count.CommandText = "SELECT COUNT(*) FROM control_data";
            Assert.Equal(0L, await count.ExecuteScalarAsync());
        }
        await using (var count = db.CreateCommand())
        {
            count.CommandText = "SELECT COUNT(*) FROM asset_data";
            Assert.Equal(1L, await count.ExecuteScalarAsync());
        }
    }
}
