using System;
using System.Linq;
using System.Text.Json;
using CacheManager.Redis.Extensions;
using CacheManager.Redis.Interfaces;
using CacheManager.Redis.Services;
using CacheManager.Redis.Tests.Fakers;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
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

        [Fact]
        public void AddRedisCacheManager_ShouldPassConfiguredOptions_ToRedisDistributedCache()
        {
            // Arrange
            var serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var cacheOptions = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
            var services = new ServiceCollection();

            // Act
            services.AddRedisCacheManager("connectionString", options =>
            {
                options.SerializerOptions = serializerOptions;
                options.DefaultCacheOptions = cacheOptions;
            });

            using var provider = services.BuildServiceProvider();

            var distributedCache = provider.GetRequiredService<IRedisDistributedCache>();

            // Assert
            distributedCache.SerializerOptions.Should().Be(serializerOptions);
            distributedCache.CacheOptions.Should().Be(cacheOptions);
        }
    }
}