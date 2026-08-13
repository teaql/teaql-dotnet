using System.Text.Json;
using Xunit;
using TeaQL.WebIntegration.AspNetCore;

namespace TeaQL.WebIntegration.AspNetCore.Tests
{
    public class TeaQLRequestTests
    {
        [Fact]
        public void Operation_DefaultsToQuery()
        {
            var request = new TeaQLRequest();
            Assert.Equal("Query", request.Operation);
        }

        [Fact]
        public void CanSetPayload()
        {
            var request = new TeaQLRequest();
            using var doc = JsonDocument.Parse("{\"key\":\"value\"}");
            request.Payload = doc.RootElement;
            Assert.Equal("value", request.Payload.GetProperty("key").GetString());
        }
    }
}
