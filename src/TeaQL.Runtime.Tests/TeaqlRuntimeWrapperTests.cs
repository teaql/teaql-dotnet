using Xunit;
using TeaQL.Runtime;

namespace TeaQL.Runtime.Tests
{
    public class TeaqlRuntimeWrapperTests
    {
        [Fact]
        public void Constructor_SetsContext()
        {
            var ctx = new UserContext();
            var runtime = new TeaqlRuntime(ctx);
            Assert.Same(ctx, runtime.UserContext);
        }
    }
}
