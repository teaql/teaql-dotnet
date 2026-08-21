namespace TeaQL.Runtime;

public sealed record ContextEntityRef(string EntityType, long Id);

public sealed class ContextRootException : InvalidOperationException
{
    public string ExpectedEntityType { get; }
    public ContextEntityRef? ActualRoot { get; }

    public ContextRootException(string expectedEntityType, ContextEntityRef? actualRoot, string message)
        : base(message)
    {
        ExpectedEntityType = expectedEntityType;
        ActualRoot = actualRoot;
    }
}
