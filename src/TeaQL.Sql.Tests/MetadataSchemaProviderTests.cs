using TeaQL.Core;

namespace TeaQL.Sql.Tests;

public class MetadataSchemaProviderTests
{
    [Fact]
    public void DelegatesEntityLookup()
    {
        var entity = EntityDescriptor.New("School");
        var provider = new MetadataSchemaProvider(name => name == "School" ? entity : null);

        Assert.Same(entity, provider.GetEntity("School"));
        Assert.Null(provider.GetEntity("Other"));
    }
}
