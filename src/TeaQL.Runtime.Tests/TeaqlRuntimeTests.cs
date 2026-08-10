using System;
using Xunit;
using TeaQL.Runtime;
using TeaQL.Core;
using Microsoft.Extensions.DependencyInjection;

namespace TeaQL.Runtime.Tests
{
    public class UserContextTests
    {
        [Fact]
        public void UserContext_Initialization_SetsDefaultValues()
        {
            var ctx = new UserContext();
            Assert.NotNull(ctx.TraceId);
            Assert.NotNull(ctx.UserIdentifier);
            Assert.Equal("UTC", ctx.Timezone);
        }

        [Fact]
        public void WithMetadata_SetsMetadata()
        {
            var ctx = new UserContext();
            var meta = new InMemoryMetadataStore();
            ctx.WithMetadata(meta);
            Assert.Same(meta, ctx.Metadata);
        }

        [Fact]
        public void GetEntity_ReturnsEntity_FromMetadata()
        {
            var ctx = new UserContext();
            var meta = new InMemoryMetadataStore();
            var ed = new EntityDescriptor { Name = "test" };
            meta.Register(ed);
            ctx.WithMetadata(meta);

            var result = ctx.GetEntity("test");
            Assert.Same(ed, result);
            Assert.Null(ctx.GetEntity("missing"));
            Assert.Same(ed, ctx.RequireEntity("test"));
            Assert.Throws<InvalidOperationException>(() => ctx.RequireEntity("missing"));
        }

        [Fact]
        public void Resources_Typed_SetAndGet()
        {
            var ctx = new UserContext();
            var res = new object();
            ctx.InsertResource(res);
            Assert.Same(res, ctx.GetResource<object>());
            Assert.Same(res, ctx.RequireResource<object>());
        }

        [Fact]
        public void Resources_Named_SetAndGet()
        {
            var ctx = new UserContext();
            var res = new object();
            ctx.InsertNamedResource("name", res);
            Assert.Same(res, ctx.GetNamedResource<object>("name"));
            Assert.Same(res, ctx.RequireNamedResource<object>("name"));
            Assert.Null(ctx.GetNamedResource<object>("missing"));
            Assert.Throws<InvalidOperationException>(() => ctx.RequireNamedResource<object>("missing"));
        }

        [Fact]
        public void Locals_SetGetRemove()
        {
            var ctx = new UserContext();
            var val = new Value.I64Value(42);
            ctx.PutLocal("key", val);
            Assert.Same(val, ctx.GetLocal("key"));
            Assert.Same(val, ctx.RemoveLocal("key"));
            Assert.Null(ctx.GetLocal("key"));
        }

        [Fact]
        public void GetResource_FallsBackToServiceProvider()
        {
            var mockProvider = new Moq.Mock<IServiceProvider>();
            mockProvider.Setup(p => p.GetService(typeof(string))).Returns("hello");
            
            var ctx = new UserContext { ServiceProvider = mockProvider.Object };
            Assert.Equal("hello", ctx.GetResource<string>());
        }
    }

    public class InMemoryMetadataStoreTests
    {
        [Fact]
        public void RegisterAndGet_ReturnsEntities()
        {
            var store = new InMemoryMetadataStore();
            var ed1 = new EntityDescriptor { Name = "e1" };
            store.Register(ed1); // Call Register explicitly
            store.WithEntity(ed1);
            
            store.RecordMetadataLog(new TeaQL.DataService.ExecutionMetadata()); // test dummy method

            Assert.Same(ed1, store.GetEntity("e1"));
            var all = store.GetAllEntities();
            Assert.Single(all);
            Assert.Same(ed1, all[0]);
        }
    }

    public class InMemoryEntityRegistryTests
    {
        [Fact]
        public void RegisterAndContains_Works()
        {
            var registry = new InMemoryEntityRegistry();
            registry.Register("e1"); // test explicitly
            registry.WithEntity("e1");
            Assert.True(registry.Contains("e1"));
            Assert.False(registry.Contains("e2"));
        }
    }
}
