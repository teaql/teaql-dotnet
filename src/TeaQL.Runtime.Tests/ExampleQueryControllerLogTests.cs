using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TeaQL.ExampleApi.Controllers;
using Xunit;

namespace TeaQL.Runtime.Tests;

[Collection("Log privacy environment")]
public class ExampleQueryControllerLogTests
{
    [Fact]
    public void CompiledQueryUsesGovernedDiagnosticLogInsteadOfRawParameters()
    {
        using var output = new StringWriter();
        using var input = JsonDocument.Parse("{\"entity\":\"User\"}");
        var originalError = Console.Error;
        try
        {
            Console.SetError(output);
            Assert.IsType<OkObjectResult>(new QueryController().Post(input.RootElement));
        }
        finally
        {
            Console.SetError(originalError);
        }

        var log = output.ToString();
        Assert.Contains("[TeaQL SQL]", log);
        Assert.Contains("compiled-not-executed", log);
        Assert.Contains("what: compile example query", log);
        // status is a plain field in this example, so the safe SQL stays replayable.
        Assert.Contains("-- TeaQL SAFE", log);
        Assert.Contains("'active'", log);
        Assert.DoesNotContain("Params:", log);
    }
}
