using Generated;
using TeaQL.Core;

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

var database = Path.Combine(Path.GetTempPath(), $"teaql-school-dotnet-{Guid.NewGuid():N}.sqlite");
try
{
    await using var service = new SqliteDataService($"Data Source={database}");
    var module = GeneratedRuntimeModule.Module;
    var context = new UserContext { DataService = service }.Install(module);

    await context.EnsureSchemaAsync();
    await context.EnsureSchemaAsync();

    var platforms = await Q.Platforms().Comment("verify seeded root")
        .Purpose("local runtime verification").ExecuteForListAsync(context);
    var constants = await Q.SchoolTypes().OrderByIdAscending().Comment("verify seeded constants")
        .Purpose("local runtime verification").ExecuteForListAsync(context);
    Require(platforms.Count == 1 && platforms[0].Id == 1, "Platform id 1 was not seeded");
    Require(constants.Count == 2 && constants[0].Id == 1001 && constants[1].Id == 1002,
        "SchoolType constants were not seeded");
    Require(constants[0].Version == 1 && constants[1].Version == 1,
        "Repeated ensureSchema was not idempotent");

    module.ConstantEntities[0].Values["name"] = new Value.TextValue("Primary School");
    await context.EnsureSchemaAsync();
    var changed = await Q.SchoolTypes().WithIdIs(1001).Comment("verify constant reconciliation")
        .Purpose("local runtime verification").ExecuteForOneAsync(context);
    Require(changed?.Name == "Primary School" && changed.Version == 2,
        "Changed constant was not reconciled exactly once");

    Console.WriteLine("PASS .NET School bootstrap with local generated runtime");
}
finally
{
    if (File.Exists(database)) File.Delete(database);
}
