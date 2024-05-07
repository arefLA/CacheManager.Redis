using System.Text.Json;
using CacheManager.Redis.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Xunit;

namespace CacheManager.Redis.Tests.Services
{
    public sealed class RedisDistributedCacheTests
    {
        [Fact]
        public void SerializerOptions_ShouldBeNull_WhenNotInitializedInTheConstructor()
        {
            // Arrange
            var input = new RedisDistributedCache(new RedisCacheOptions());
            
            // Act
            var result = input.SerializerOptions;

            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public void CacheOptions_ShouldBeNull_WhenNotInitializedInTheConstructor()
        {
            // Arrange
            var input = new RedisDistributedCache(new RedisCacheOptions());
            
            // Act
            var result = input.CacheOptions;

            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public void SerializerOptions_ShouldReturnTheInitializeValue_WhenNotInitializedInTheConstructor()
        {
            // Arrange
            var serializerOptions = new JsonSerializerOptions();
            var input = new RedisDistributedCache(new RedisCacheOptions(), serializerOptions);
            
            // Act
            var result = input.SerializerOptions;

            // Assert
            result.Should().BeEquivalentTo(serializerOptions);
        }
        
        [Fact]
        public void CacheOptions_ShouldReturnTheInitializeValue_WhenNotInitializedInTheConstructor()
        {
            // Arrange
            var cacheOptıons = new DistributedCacheEntryOptions();
            var input = new RedisDistributedCache(new RedisCacheOptions(), cacheOptions:cacheOptıons);
            
            // Act
            var result = input.CacheOptions;

            // Assert
            result.Should().BeEquivalentTo(cacheOptıons);
        }
    }
}