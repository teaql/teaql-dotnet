namespace TeaQL.Runtime;

/// <summary>Concrete optional module service validated after passive assembly.</summary>
public interface IRuntimeCapabilityResource
{
    string Capability { get; }
    Task ValidateReadyAsync(UserContext context);
}
