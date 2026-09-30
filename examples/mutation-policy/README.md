# Mutation Policy example

This focused example proves the .NET runtime's governed graph-mutation boundary:

- a customer policy receives the complete immutable graph plan;
- an exact policy identity approval is recorded;
- an allowed graph commits atomically;
- a denied graph performs zero provider mutations and rolls back;
- policy identity, approval status and operation count reach application audit.

Run against the local runtime source:

```bash
dotnet run --project examples/mutation-policy/mutation-policy.csproj
```

The success marker is `DOTNET_MUTATION_POLICY_PASS`.
