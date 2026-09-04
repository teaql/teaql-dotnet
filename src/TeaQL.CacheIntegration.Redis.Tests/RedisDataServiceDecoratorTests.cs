using System;
using Xunit;
using TeaQL.CacheIntegration.Redis;
using Moq;
using TeaQL.DataService;
using StackExchange.Redis;

namespace TeaQL.CacheIntegration.Redis.Tests
{
    public class RedisDataServiceDecoratorTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenInnerServiceIsNull()
        {
            var mockRedis = new Mock<IConnectionMultiplexer>();
            Assert.Throws<ArgumentNullException>(() => new RedisDataServiceDecorator(null!, mockRedis.Object, TimeSpan.FromMinutes(5)));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenConnectionIsNull()
        {
            var mockInnerService = new Mock<IDataService>();
            Assert.Throws<ArgumentNullException>(() => new RedisDataServiceDecorator(mockInnerService.Object, null!, TimeSpan.FromMinutes(5)));
        }

        [Fact]
        public void Capabilities_DelegatesToInnerService()
        {
            var mockInnerService = new Mock<IDataService>();
            mockInnerService.Setup(s => s.Capabilities).Returns(new DataServiceCapabilities { Query = true });
            var mockRedis = new Mock<IConnectionMultiplexer>();
            mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(new Mock<IDatabase>().Object);

            var decorator = new RedisDataServiceDecorator(mockInnerService.Object, mockRedis.Object, TimeSpan.FromMinutes(5));
            Assert.True(decorator.Capabilities.Query);
        }
    }
}
