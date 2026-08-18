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
            var context = new UserContext();
            Assert.NotNull(context.TraceId);
            Assert.NotNull(context.UserIdentifier);
            Assert.Equal("UTC", context.Timezone);
        }

        [Fact]
        public void WithMetadata_SetsMetadata()
        {
            var context = new UserContext();
            var meta = new InMemoryMetadataStore();
            context.WithMetadata(meta);
            Assert.Same(meta, context.Metadata);
        }

        [Fact]
        public void GetEntity_ReturnsEntity_FromMetadata()
        {
            var context = new UserContext();
            var meta = new InMemoryMetadataStore();
            var ed = new EntityDescriptor { Name = "test" };
            meta.Register(ed);
            context.WithMetadata(meta);

            var result = context.GetEntity("test");
            Assert.Same(ed, result);
            Assert.Null(context.GetEntity("missing"));
            Assert.Same(ed, context.RequireEntity("test"));
            Assert.Throws<InvalidOperationException>(() => context.RequireEntity("missing"));
        }

        [Fact]
        public void Resources_Typed_SetAndGet()
        {
            var context = new UserContext();
            var res = new object();
            context.InsertResource(res);
            Assert.Same(res, context.GetResource<object>());
            Assert.Same(res, context.RequireResource<object>());
        }

        [Fact]
        public void Resources_Named_SetAndGet()
        {
            var context = new UserContext();
            var res = new object();
            context.InsertNamedResource("name", res);
            Assert.Same(res, context.GetNamedResource<object>("name"));
            Assert.Same(res, context.RequireNamedResource<object>("name"));
            Assert.Null(context.GetNamedResource<object>("missing"));
            Assert.Throws<InvalidOperationException>(() => context.RequireNamedResource<object>("missing"));
        }

        [Fact]
        public void Locals_SetGetRemove()
        {
            var context = new UserContext();
            var val = new Value.I64Value(42);
            context.PutLocal("key", val);
            Assert.Same(val, context.GetLocal("key"));
            Assert.Same(val, context.RemoveLocal("key"));
            Assert.Null(context.GetLocal("key"));
        }

        [Fact]
        public void GetResource_FallsBackToServiceProvider()
        {
            var mockProvider = new Moq.Mock<IServiceProvider>();
            mockProvider.Setup(p => p.GetService(typeof(string))).Returns("hello");
            
            var context = new UserContext { ServiceProvider = mockProvider.Object };
            Assert.Equal("hello", context.GetResource<string>());
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
