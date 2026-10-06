using TeaQL.Core;
using TeaQL.Runtime;

namespace TeaQL.Runtime.Tests;

public class QueryPolicyTests
{
    [Fact]
    public void PolicyAppliesOnceToEveryEntityQueryWithoutMutatingCallerGraph()
    {
        var policy = new TenantPolicy();
        var context = new UserContext().WithRequestPolicy(policy);
        var root = new SelectQuery("Order").Comment("load order graph").Purpose("verify nested query policy");
        var item = new SelectQuery("OrderItem");
        root.RelationLoads.Add(new RelationLoad("items", item));
        root.Facets.Add(new FacetRequest("items", "item", item, true));

        var authorized = context.ApplyRequestPolicy(root);

        Assert.NotSame(root, authorized);
        Assert.Null(root.FilterCondition);
        Assert.Null(item.FilterCondition);
        Assert.NotNull(authorized.FilterCondition);
        Assert.NotNull(authorized.RelationLoads[0].Query!.FilterCondition);
        Assert.NotNull(authorized.Facets[0].Query.FilterCondition);
        Assert.Same(authorized.RelationLoads[0].Query, authorized.Facets[0].Query);
        Assert.Equal(new[] { "Order", "OrderItem" }, policy.Entities);
    }

    [Fact]
    public void MissingPolicyUsesADeepExecutionClone()
    {
        var child = new SelectQuery("OrderItem");
        var root = new SelectQuery("Order").Comment("load order graph").Purpose("verify execution clone isolation");
        root.RelationLoads.Add(new RelationLoad("items", child));

        var prepared = new UserContext().ApplyRequestPolicy(root);
        prepared.RelationLoads[0].Query!.AndFilter(Expr.Eq("tenant_id", 7));

        Assert.Null(child.FilterCondition);
    }

    [Fact]
    public void ReplacementPolicyPreservesSharedQueryIdentity()
    {
        var shared = new SelectQuery("OrderItem");
        var root = new SelectQuery("Order").Comment("load order graph").Purpose("verify shared query identity after policy");
        root.RelationLoads.Add(new RelationLoad("items", shared));
        root.Facets.Add(new FacetRequest("items", "item", shared, true));

        var authorized = new UserContext().WithRequestPolicy(new ReplacementPolicy())
            .ApplyRequestPolicy(root);

        Assert.Same(authorized.RelationLoads[0].Query, authorized.Facets[0].Query);
        Assert.Contains("authorized", authorized.RelationLoads[0].Query!.Projection);
    }

    private sealed class TenantPolicy : IRequestPolicy
    {
        public List<string> Entities { get; } = new();

        public SelectQuery Apply(SelectQuery query)
        {
            Entities.Add(query.Entity);
            query.AndFilter(Expr.Eq("tenant_id", 7));
            return query;
        }
    }

    private sealed class ReplacementPolicy : IRequestPolicy
    {
        public SelectQuery Apply(SelectQuery query)
        {
            var replacement = new SelectQuery(query.Entity)
            {
                RelationLoads = query.RelationLoads,
                Facets = query.Facets
            };
            replacement.Project("authorized");
            return replacement;
        }
    }
}
