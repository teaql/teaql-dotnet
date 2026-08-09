using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class NamingTests
{
    [Fact]
    public void TestDefaultTableName()
    {
        Assert.Equal("user_data", Naming.DefaultTableName("User"));
        Assert.Equal("user_profile_data", Naming.DefaultTableName("UserProfile"));
        Assert.Equal("abc_data", Naming.DefaultTableName("abc"));
    }
}
