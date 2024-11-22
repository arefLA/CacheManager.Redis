using System.Linq;
using CacheManager.Redis.Extensions;
using CacheManager.Redis.Interfaces;
using CacheManager.Redis.Services;
using CacheManager.Redis.Tests.Fakers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CacheManager.Redis.Tests.Extensions
{
    public sealed class SetupTests
    {
        [Fact]
        public void
            AddRedisCacheManager_ShouldAddRedisDistributedCacheServiceAsSingleton_WhenCalledOnAServiceCollection()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            
            // Act
            serviceCollection.AddRedisCacheManager("connectionString");
            var result =
                serviceCollection.FirstOrDefault(service => service.ServiceType == typeof(IRedisDistributedCache));

            
            // Assert
            result.Should().NotBeNull();
            result!.Lifetime.Should().Be(ServiceLifetime.Transient);
        }
        
        [Fact]
        public void
            AddRedisCacheManager_ShouldAddRedisCacheManagerAsScoped_WhenCalledOnAServiceCollection()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            
            // Act
            serviceCollection.AddRedisCacheManager("connectionString");
            var result =
                serviceCollection.FirstOrDefault(service => service.ServiceType == typeof(IRedisCacheManager<>));

            
            // Assert
            result.Should().NotBeNull();
            result!.Lifetime.Should().Be(ServiceLifetime.Transient);
            result.ImplementationType.Should().Be(typeof(RedisCacheManager<>));
        }
        
        [Fact]
        public void
            AddRedisCacheManager_ShouldAddCustomCacheManagerAsScoped_WhenCalledOnAServiceCollection_WithACustomCacheManager()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            
            // Act
            serviceCollection.AddRedisCacheManager("connectionString", options =>
            {
                options.CustomImplementation = typeof(FakeCustomCacheManager<>);
            });
            var result =
                serviceCollection.FirstOrDefault(service => service.ServiceType == typeof(IRedisCacheManager<>));

            
            // Assert
            result.Should().NotBeNull();
            result!.Lifetime.Should().Be(ServiceLifetime.Transient);
            result.ImplementationType.Should().Be(typeof(FakeCustomCacheManager<>));
        }
    }
}