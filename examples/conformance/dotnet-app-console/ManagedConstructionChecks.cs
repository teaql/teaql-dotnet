using Generated;
using Generated.Models;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;

// Application-owned checks: use generated Q/E without inspecting implementation.
static class ManagedConstructionChecks
{
    sealed class Allocator : IInternalIdAllocator
    {
        public int Calls;
        public Task<ulong> NextIdAsync(string entity) => Task.FromResult((ulong)(700 + ++Calls));
        public Task EnsureIdFloorAsync(string entity, ulong floor) => throw new Exception("Unexpected ID floor mutation");
    }
    static Task<WorkItem> New(UserContext context) => Q.WorkItems().Comment("Prepare a managed work item")
        .Purpose("Verify the initializer postcondition").NewEntityAsync(context);
    static void Require(bool ok, string message) { if (!ok) throw new Exception(message); }
    public static async Task<int> Run()
    {
        foreach (var round in new[] { 1, 2 })
        {
            foreach (var bad in new long?[] { null, 0, -1, 999 })
            {
                var ids = new Allocator();
                var context = new UserContext().Install(GeneratedRuntimeModule.Module).WithInternalIdAllocator(ids);
                context.RegisterEntityInitializer("WorkItem", (_, value) => ((WorkItem)value).Id = bad);
                Exception? caught = null;
                try { await New(context); } catch (EntityCreationException error) { caught = error; }
                Require(caught is EntityCreationException { Code: "ENTITY_CREATION_CONTEXT_REQUIRED" }, "Factory returned a changed reserved identity");
                Require(context.ManagedEntities.Count == 0 && ids.Calls == 1, "Rejected initialization was registered or reallocated");
            }
            {
                var ids = new Allocator();
                var context = new UserContext().Install(GeneratedRuntimeModule.Module).WithInternalIdAllocator(ids);
                context.RegisterEntityInitializer("WorkItem", (_, value) => {
                    var item = (WorkItem)value;
                    Require(item.Id == 701, "Initializer did not see reservation"); item.UpdateTitle("trusted defaults");
                });
                var item = await New(context);
                Require(item.Id == 701 && E.WorkItem(item).Title().Eval() == "trusted defaults"
                    && context.ManagedEntities.Count == 1, "Business initializer was lost");
            }
            {
                var ids = new Allocator(); var calls = 0;
                var context = new UserContext().Install(GeneratedRuntimeModule.Module).WithInternalIdAllocator(ids);
                var controlled = new InvalidOperationException("Controlled initializer failure");
                context.RegisterEntityInitializer("WorkItem", (_, _) => { if (++calls == 1) throw controlled; });
                try { await New(context); throw new Exception("Initializer failure swallowed"); }
                catch (InvalidOperationException error) { Require(ReferenceEquals(controlled, error), "Initializer failure replaced"); }
                Require(context.ManagedEntities.Count == 0, "Failed initializer leaked registration");
                var item = await New(context);
                Require(item.Id == 702 && ids.Calls == 2 && context.ManagedEntities.Count == 1, "Initializer recovery changed allocator");
            }
            Console.WriteLine($"PASS Managed creation initializer admission: 6/6, round {round}");
        }
        return 0;
    }
}
