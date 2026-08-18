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
            var context = new UserContext();
            Assert.Throws<InvalidOperationException>(() => context.RequireEntity("missing"));
            
            var store = new InMemoryMetadataStore();
            store.WithEntity(new EntityDescriptor { Name = "found" });
            context.WithMetadata(store);
            
            Assert.NotNull(context.RequireEntity("found"));
        }

        [Fact]
        public void RequireResource_ThrowsIfMissing()
        {
            var context = new UserContext();
            Assert.Throws<InvalidOperationException>(() => context.RequireResource<string>());
            
            context.InsertResource("hello");
            Assert.Equal("hello", context.RequireResource<string>());
        }

        [Fact]
        public void RequireNamedResource_ThrowsIfMissing()
        {
            var context = new UserContext();
            Assert.Throws<InvalidOperationException>(() => context.RequireNamedResource<string>("missing"));
            
            context.InsertNamedResource("found", "value");
            Assert.Equal("value", context.RequireNamedResource<string>("found"));
        }

        [Fact]
        public void WithModule_AppliesModule()
        {
            var context = new UserContext();
            var module = new RuntimeModule();
            module.Entity(new EntityDescriptor { Name = "test" });
            context.WithModule(module);
            
            Assert.NotNull(context.GetEntity("test"));
        }

        [Fact]
        public void RuntimeModule_IsComposableAndInstallDoesNotEnsureSchema()
        {
            var first = new RuntimeModule().Entity(new EntityDescriptor { Name = "first" });
            var second = new RuntimeModule().Entity(new EntityDescriptor { Name = "second" });
            var context = new UserContext().Install(first.And(second));

            Assert.NotNull(context.GetEntity("first"));
            Assert.NotNull(context.GetEntity("second"));
        }

        [Fact]
        public void Constructor_InitializesTraceIdAndUserIdentifier()
        {
            var context = new UserContext();
            Assert.NotNull(context.TraceId);
            Assert.NotNull(context.UserIdentifier);
            Assert.Equal("UTC", context.Timezone);
        }
    }
}
