using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CacheManager.Redis.Interfaces;
using CacheManager.Redis.Services;
using CacheManager.Redis.Tests.Models;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Xunit;

namespace CacheManager.Redis.Tests.Services
{
    public sealed class RedisCacheManagerTests
    {
        [Fact]
        public void TryGet_ShouldReturnTrueAndTheEntity_WhenRedisCacheManagerReturnAValidValue()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var expectedOut = new SampleObject
            {
                SomeProp = 10
            };
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expectedOut));
            redisDistributedCache.Get("key").Returns(bytes);
            
            // Act
            var result = redisCacheManager.TryGet("key", out var outResult);

            // Assert
            result.Should().BeTrue();
            outResult.Should().BeEquivalentTo(expectedOut);
        }
        
        [Fact]
        public void TryGet_ShouldReturnFalseAndNull_WhenRedisCacheManagerReturnNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            byte[]? bytes = null;
            redisDistributedCache.Get("key").Returns(bytes);
            
            // Act
            var result = redisCacheManager.TryGet("key", out var outResult);

            // Assert
            result.Should().BeFalse();
            outResult.Should().BeNull();
        }
        
        [Fact]
        public async Task TryGetAsync_ShouldReturnTheEntity_WhenRedisCacheManagerReturnAValidValue()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var expectedOut = new SampleObject
            {
                SomeProp = 10
            };
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expectedOut));
            redisDistributedCache.GetAsync("key").Returns(bytes);
            
            // Act
            var result = await redisCacheManager.TryGetAsync("key");

            // Assert
            result.Should().BeEquivalentTo(expectedOut);
        }
        
        [Fact]
        public async Task TryGetAsync_ShouldReturnFalseAndNull_WhenRedisCacheManagerReturnNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            byte[]? bytes = null;
            redisDistributedCache.GetAsync("key").Returns(bytes);
            
            // Act
            var result = await redisCacheManager.TryGetAsync("key");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Set_ShouldCallRedisDistributedCache_WhenCalled()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            
            // Act
            redisCacheManager.Set("key", entity);
            
            // Assert
            redisDistributedCache
                .Received(1)
                .Set("key", Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }
        
        [Fact]
        public async Task SetAsync_ShouldCallRedisDistributedCacheWithDefaultCacheOptions_WhenCalledWithoutCacheOptions()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            
            // Act
            await redisCacheManager.SetAsync("key", entity);
            
            // Assert
            await redisDistributedCache
                .Received(1)
                .SetAsync("key", Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task SetAsync_ShouldCallRedisDistributedCacheWithCustomCacheOptions_WhenCalledWithCacheOptions()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            var cacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            };
            
            // Act
            await redisCacheManager.SetAsync("key", entity, cacheOptions);
            
            // Assert
            await redisDistributedCache
                .Received(1)
                .SetAsync("key", Arg.Any<byte[]>(), cacheOptions, Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Refresh_ShouldCallDistributedCacheRefresh_WhenCalled()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            redisCacheManager.Refresh("key");
            
            // Assert
            redisDistributedCache
                .Received(1)
                .Refresh("key");
        }
        
        [Fact]
        public async Task RefreshAsync_ShouldCallDistributedCacheRefresh_WhenCalled()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            await redisCacheManager.RefreshAsync("key");
            
            // Assert
            await redisDistributedCache
                .Received(1)
                .RefreshAsync("key", Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public void Remove_ShouldCallDistributedCacheRefresh_WhenCalled()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            redisCacheManager.Remove("key");
            
            // Assert
            redisDistributedCache
                .Received(1)
                .Remove("key");
        }
        
        [Fact]
        public async Task RemoveAsync_ShouldCallDistributedCacheRefresh_WhenCalled()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            await redisCacheManager.RemoveAsync("key");
            
            // Assert
            await redisDistributedCache
                .Received(1)
                .RemoveAsync("key", Arg.Any<CancellationToken>());
        }
    }
}