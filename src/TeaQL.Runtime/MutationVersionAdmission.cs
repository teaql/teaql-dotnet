using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

/// <summary>Validate the whole request before callbacks, and again before provider execution.</summary>
internal static class MutationVersionAdmission
{
    internal static void Validate(UserContext context, MutationRequest request)
    {
        if (request is BatchMutationRequest batch)
        {
            foreach (var child in batch.Requests) Validate(context, child);
            return;
        }

        string entity;
        MutationKind operation;
        long version;
        switch (request)
        {
            case UpdateMutationRequest { Command.ExpectedVersionValue: { } expected } update:
                entity = update.Command.Entity; operation = MutationKind.Update; version = expected;
                break;
            case DeleteMutationRequest { Command.SoftDelete: true, Command.ExpectedVersionValue: { } expected } delete:
                entity = delete.Command.Entity; operation = MutationKind.Delete; version = expected;
                break;
            case RecoverMutationRequest recover:
                entity = recover.Command.Entity; operation = MutationKind.Recover; version = recover.Command.ExpectedVersionValue;
                break;
            default:
                // Inserts, versionless commands and physical deletes keep their distinct contracts.
                return;
        }

        try { MutationVersion.Next(operation, version); }
        catch (MutationVersionException error)
        {
            var violations = new[] { new CheckResult {
                RuleId = error.Exhausted ? "version_exhausted" : operation == MutationKind.Delete ? "invalid_deletion_state" : "invalid_version_state",
                EntityType = entity, Location = ObjectLocation.Property("version"),
                InputValue = version, Message = error.Message
            } };
            context.TranslateCheckResults(violations);
            throw new CheckException(violations);
        }
    }
}
