#pragma warning disable CS8604
#pragma warning disable CS8625

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
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TryGet_ShouldReturnFalseAndNull_WhenKeyIsNullOrEmpty(string? input)
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            byte[]? bytes = null;
            redisDistributedCache.Get(input).Returns(bytes);
            
            // Act
            var result = redisCacheManager.TryGet(input, out var outResult);

            // Assert
            result.Should().BeFalse();
            outResult.Should().BeNull();
        }
        
        [Fact]
        public async Task GetAsync_ShouldReturnTheEntity_WhenRedisCacheManagerReturnAValidValue()
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
            var result = await redisCacheManager.GetAsync("key");

            // Assert
            result.Should().BeEquivalentTo(expectedOut);
        }
        
        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenRedisCacheManagerReturnNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            byte[]? bytes = null;
            redisDistributedCache.GetAsync("key").Returns(bytes);
            
            // Act
            var result = await redisCacheManager.GetAsync("key");

            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public async Task GetAsync_ShouldThrowException_WhenKeyIsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            var act = () => redisCacheManager.GetAsync(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
        
        [Fact]
        public async Task GetAsync_ShouldThrowException_WhenKeyIsEmpty()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            var act = () => redisCacheManager.GetAsync(string.Empty);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
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
        public void Set_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            
            // Act
            Action act = () => redisCacheManager.Set(null, entity);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("key");
        }
        
                
        [Fact]
        public void Set_ShouldThrowArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            
            // Act
            Action act = () => redisCacheManager.Set(string.Empty, entity);
            
            // Assert
            act.Should().Throw<ArgumentException>().WithParameterName("key");
        }
        
        [Fact]
        public void TrySet_ShouldCallRedisDistributedCacheAndReturnTrue_WhenCalledWithAKey()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            
            // Act
            var result = redisCacheManager.TrySet("key", entity);
            
            // Assert
            redisDistributedCache
                .Received(1)
                .Set("key", Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
            result.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TrySet_ShouldReturnFalse_WhenKeyIsNullOrEmpty(string? input)
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            
            // Act
            var result = redisCacheManager.TrySet(input, entity);
            
            // Assert
            result.Should().BeFalse();
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
        public async Task SetAsync_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            
            // Act
            Func<Task> act = async () => await redisCacheManager.SetAsync(null, entity);
            
            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("key");
        }
        
                
        [Fact]
        public async Task SetAsync_ShouldThrowArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            
            // Act
            Func<Task> act = async () => await redisCacheManager.SetAsync("", entity);

            
            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("key");
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
        public void Refresh_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            Action act = () => redisCacheManager.Refresh(null);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("key");
        }
        
                
        [Fact]
        public void Refresh_ShouldThrowArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            Action act = () => redisCacheManager.Refresh(string.Empty);
            
            // Assert
            act.Should().Throw<ArgumentException>().WithParameterName("key");
        }
        
        [Fact]
        public void TryRefresh_ShouldCallRedisDistributedCacheAndReturnTrue_WhenCalledWithAKey()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            var result = redisCacheManager.TryRefresh("key");
            
            // Assert
            redisDistributedCache
                .Received(1)
                .Refresh("key");
            result.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TryRefresh_ShouldReturnFalse_WhenKeyIsNullOrEmpty(string? input)
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            var result = redisCacheManager.TryRefresh(input);
            
            // Assert
            result.Should().BeFalse();
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
        public async Task RefreshAsync_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            Func<Task> act = async () => await redisCacheManager.RefreshAsync(null);
            
            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("key");
        }
        
                
        [Fact]
        public async Task RefreshAsync_ShouldThrowArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            Func<Task> act = async () => await redisCacheManager.RefreshAsync(string.Empty);

            
            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("key");
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
        public void Remove_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            Action act = () => redisCacheManager.Remove(null);
            
            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("key");
        }
        
                
        [Fact]
        public void Remove_ShouldThrowArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            Action act = () => redisCacheManager.Remove(string.Empty);
            
            // Assert
            act.Should().Throw<ArgumentException>().WithParameterName("key");
        }
        
        [Fact]
        public void TryRemove_ShouldCallRedisDistributedCacheAndReturnTrue_WhenCalledWithAKey()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            var result = redisCacheManager.TryRemove("key");
            
            // Assert
            redisDistributedCache
                .Received(1)
                .Remove("key");
            result.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TryRemove_ShouldReturnFalse_WhenKeyIsNullOrEmpty(string? input)
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            var result = redisCacheManager.TryRemove(input);
            
            // Assert
            result.Should().BeFalse();
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
        
                
        [Fact]
        public async Task RemoveAsync_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            Func<Task> act = async () => await redisCacheManager.RemoveAsync(null);
            
            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("key");
        }
        
                
        [Fact]
        public async Task RemoveAsync_ShouldThrowArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            
            // Act
            Func<Task> act = async () => await redisCacheManager.RefreshAsync(string.Empty);

            
            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithParameterName("key");
        }

        [Fact]
        public void TryGet_ShouldReturnFalse_WhenPayloadCannotBeDeserialized()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            redisDistributedCache.Get("key").Returns(new byte[] { 1, 2, 3 });

            // Act
            var result = redisCacheManager.TryGet("key", out var response);

            // Assert
            result.Should().BeFalse();
            response.Should().BeNull();
        }
    }
#pragma warning restore CS8625
#pragma warning restore CS8604
}