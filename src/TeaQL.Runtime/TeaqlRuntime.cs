namespace TeaQL.Runtime;

public class TeaqlRuntime : ITeaqlRuntime
{
    public UserContext UserContext { get; }

    public TeaqlRuntime(UserContext userContext)
    {
        UserContext = userContext;
    }
}
