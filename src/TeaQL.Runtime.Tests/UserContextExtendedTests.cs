using System;
using System.Collections.Generic;
using Xunit;
using TeaQL.DataService;
using Moq;
using TeaQL.Runtime;
using TeaQL.Core;

namespace TeaQL.Runtime.Tests
{
    public class UserContextExtendedTests
    {
        [Fact]
        public void RequireEntity_ThrowsIfMissing()
        {
            var ctx = new UserContext();
            Assert.Throws<InvalidOperationException>(() => ctx.RequireEntity("missing"));
            
            var store = new InMemoryMetadataStore();
            store.WithEntity(new EntityDescriptor { Name = "found" });
            ctx.WithMetadata(store);
            
            Assert.NotNull(ctx.RequireEntity("found"));
        }

        [Fact]
        public void RequireResource_ThrowsIfMissing()
        {
            var ctx = new UserContext();
            Assert.Throws<InvalidOperationException>(() => ctx.RequireResource<string>());
            
            ctx.InsertResource("hello");
            Assert.Equal("hello", ctx.RequireResource<string>());
        }

        [Fact]
        public void RequireNamedResource_ThrowsIfMissing()
        {
            var ctx = new UserContext();
            Assert.Throws<InvalidOperationException>(() => ctx.RequireNamedResource<string>("missing"));
            
            ctx.InsertNamedResource("found", "value");
            Assert.Equal("value", ctx.RequireNamedResource<string>("found"));
        }

        [Fact]
        public void WithModule_AppliesModule()
        {
            var ctx = new UserContext();
            var module = new RuntimeModule();
            module.Entity(new EntityDescriptor { Name = "test" });
            ctx.WithModule(module);
            
            Assert.NotNull(ctx.GetEntity("test"));
        }

        [Fact]
        public void Constructor_InitializesTraceIdAndUserIdentifier()
        {
            var ctx = new UserContext();
            Assert.NotNull(ctx.TraceId);
            Assert.NotNull(ctx.UserIdentifier);
            Assert.Equal("UTC", ctx.Timezone);
        }
    }
}
