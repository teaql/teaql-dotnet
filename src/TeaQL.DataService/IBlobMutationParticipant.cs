using TeaQL.Core;

namespace TeaQL.DataService;

/// <summary>A trusted optional module hook invoked before a physical mutation owner commits.</summary>
public interface IBlobMutationParticipant
{
    Task ApplyAsync(IPhysicalSqlOwner owner, MutationRequest request,
        EntityDescriptor entity, Record? previous, Record? persisted);
}

/// <summary>Transaction-local installation; a participant cannot replace the physical owner.</summary>
public interface IBlobMutationOwner
{
    void InstallBlobMutationParticipant(IBlobMutationParticipant participant);
}
