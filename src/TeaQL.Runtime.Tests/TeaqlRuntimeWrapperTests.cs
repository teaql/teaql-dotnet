using Xunit;
using TeaQL.Runtime;

namespace TeaQL.Runtime.Tests
{
    public class TeaqlRuntimeWrapperTests
    {
        [Fact]
        public void Constructor_SetsContext()
        {
            var context = new UserContext();
            var runtime = new TeaqlRuntime(context);
            Assert.Same(context, runtime.UserContext);
        }
    }
}
