using Generated;
using Generated.Models;
using Generated.Requests;
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

    var school = Q.Schools().Comment("create School Query conformance fixture")
        .Purpose("execute the shared School example").NewEntity(context);
    school.UpdatePlatform(Platform.Refer(1));
    school.UpdateSchoolTypeToPrimary();
    school.UpdateName("Riverside Primary School");
    school.UpdateAddress("12 River Road, Springfield");
    school.UpdateEstablishedDate(new DateTime(1995, 9, 1));
    school.UpdateStudentCapacity(800);
    school.UpdateActive(1);
    await school.AuditAs("create School Query conformance fixture").SaveAsync(context);

    var queryCases = new List<(string Label, SchoolRequest Request, int Expected)>
    {
        ("string equality", Q.Schools().WithNameIs("Riverside Primary School"), 1),
        ("string inequality", Q.Schools().WithNameIsNot("Another School"), 1),
        ("string membership", Q.Schools().WithNameIn("Riverside Primary School", "Another School"), 1),
        ("negative membership", Q.Schools().WithNameNotIn("Another School"), 1),
        ("contains", Q.Schools().WithNameContaining("Primary"), 1),
        ("negative contains", Q.Schools().WithNameNotContaining("Secondary"), 1),
        ("starts with", Q.Schools().WithNameStartingWith("Riverside"), 1),
        ("negative starts with", Q.Schools().WithNameNotStartingWith("Lakeside"), 1),
        ("ends with", Q.Schools().WithNameEndingWith("School"), 1),
        ("negative ends with", Q.Schools().WithNameNotEndingWith("Academy"), 1),
        ("number range", Q.Schools().WithStudentCapacityBetween(700, 900), 1),
        ("strict comparison", Q.Schools().WithStudentCapacityGreaterThan(799).WithStudentCapacityLessThan(801), 1),
        ("date range", Q.Schools().WithEstablishedDateBetween(new DateTime(1995, 1, 1), new DateTime(1995, 12, 31)), 1),
        ("known", Q.Schools().WithAddressIsKnown(), 1),
        ("unknown", Q.Schools().WithAddressIsUnknown(), 0),
        ("boolean", Q.Schools().WhichAreActive(), 1),
        ("constant relation", Q.Schools().WithSchoolTypeIsPrimary(), 1),
    };
    foreach (var (label, request, expected) in queryCases)
    {
        var result = await request.Comment($"Query parity: {label}")
            .Purpose("Execute the shared School Query conformance case")
            .ExecuteForListAsync(context);
        Require(result.Count == expected, $"{label}: expected {expected}, got {result.Count}");
    }
    var projected = await Q.Schools().SelectName().OrderByIdDescending()
        .Comment("Query parity: projection and ordering")
        .Purpose("Execute the shared School Query conformance case")
        .ExecuteForListAsync(context);
    Require(projected.Count == 1 && projected[0].Name == "Riverside Primary School",
        "projection/order query did not preserve typed School result");

    Console.WriteLine("PASS .NET School bootstrap and portable Query parity");
}
finally
{
    if (File.Exists(database)) File.Delete(database);
}
