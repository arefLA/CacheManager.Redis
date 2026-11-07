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
        public async Task GetAsync_ShouldReturnNull_WhenKeyIsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            var actual = await redisCacheManager.GetAsync(null);

            // Assert
            actual.Should().BeNull();
        }
        
        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenKeyIsEmpty()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            
            // Act
            var actual = await redisCacheManager.GetAsync(string.Empty);

            // Assert
            actual.Should().BeNull();
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
        public void Set_WithOptions_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            var options = new DistributedCacheEntryOptions();

            // Act
            Action act = () => redisCacheManager.Set(null, entity, options);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("key");
        }

        [Fact]
        public void Set_WithOptions_ShouldThrowArgumentException_WhenKeyIsEmpty()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var entity = new SampleObject();
            var options = new DistributedCacheEntryOptions();

            // Act
            Action act = () => redisCacheManager.Set(string.Empty, entity, options);

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
        public void Remove_ShouldCallDistributedCacheRemove_WhenCalled()
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
        public async Task RemoveAsync_ShouldCallDistributedCacheRemove_WhenCalled()
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

            // Act
            Func<Task> act = async () => await redisCacheManager.RemoveAsync(string.Empty);

            
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

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenPayloadCannotBeDeserialized()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            redisDistributedCache.GetAsync("key").Returns(new byte[] { 1, 2, 3 });

            // Act
            var result = await redisCacheManager.GetAsync("key");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldReturnCachedValueWithoutCallingFactory_WhenKeyExists()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var cached = new SampleObject { SomeProp = 10 };
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>())
                .Returns(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cached)));
            var factoryCalled = false;

            // Act
            var result = await redisCacheManager.GetOrSetAsync("key", _ =>
            {
                factoryCalled = true;
                return Task.FromResult<SampleObject?>(new SampleObject { SomeProp = 99 });
            });

            // Assert
            result.Should().BeEquivalentTo(cached);
            factoryCalled.Should().BeFalse();
            await redisDistributedCache.DidNotReceiveWithAnyArgs()
                .SetAsync(default!, default!, default!, default);
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldCallFactoryStoreAndReturnValue_WhenKeyIsMissing()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>()).Returns((byte[]?)null);
            var created = new SampleObject { SomeProp = 5 };

            // Act
            var result = await redisCacheManager.GetOrSetAsync("key", _ => Task.FromResult<SampleObject?>(created));

            // Assert
            result.Should().BeSameAs(created);
            await redisDistributedCache.Received(1)
                .SetAsync("key", Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldUseGivenOptions_WhenKeyIsMissingAndOptionsProvided()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>()).Returns((byte[]?)null);
            redisDistributedCache.CacheOptions.Returns(new DistributedCacheEntryOptions());
            var options = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(1) };

            // Act
            await redisCacheManager.GetOrSetAsync("key",
                _ => Task.FromResult<SampleObject?>(new SampleObject { SomeProp = 1 }), options);

            // Assert
            await redisDistributedCache.Received(1)
                .SetAsync("key", Arg.Any<byte[]>(), options, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldUseDefaultOptions_WhenKeyIsMissingAndNoOptionsProvided()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>()).Returns((byte[]?)null);
            var defaultOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) };
            redisDistributedCache.CacheOptions.Returns(defaultOptions);

            // Act
            await redisCacheManager.GetOrSetAsync("key",
                _ => Task.FromResult<SampleObject?>(new SampleObject { SomeProp = 1 }));

            // Assert
            await redisDistributedCache.Received(1)
                .SetAsync("key", Arg.Any<byte[]>(), defaultOptions, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldTreatCorruptPayloadAsMiss()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>())
                .Returns(Encoding.UTF8.GetBytes("not-json"));
            var created = new SampleObject { SomeProp = 7 };

            // Act
            var result = await redisCacheManager.GetOrSetAsync("key", _ => Task.FromResult<SampleObject?>(created));

            // Assert
            result.Should().BeSameAs(created);
            await redisDistributedCache.Received(1)
                .SetAsync("key", Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldNotStoreAndReturnNull_WhenFactoryReturnsNull()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>()).Returns((byte[]?)null);

            // Act
            var result = await redisCacheManager.GetOrSetAsync("key", _ => Task.FromResult<SampleObject?>(null));

            // Assert
            result.Should().BeNull();
            await redisDistributedCache.DidNotReceiveWithAnyArgs()
                .SetAsync(default!, default!, default!, default);
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            var redisCacheManager = new RedisCacheManager<SampleObject>(Substitute.For<IRedisDistributedCache>());

            var act = () => redisCacheManager.GetOrSetAsync(null, _ => Task.FromResult<SampleObject?>(null));

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldThrowArgumentException_WhenKeyIsEmpty()
        {
            var redisCacheManager = new RedisCacheManager<SampleObject>(Substitute.For<IRedisDistributedCache>());

            var act = () => redisCacheManager.GetOrSetAsync(string.Empty, _ => Task.FromResult<SampleObject?>(null));

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldThrowArgumentNullException_WhenFactoryIsNull()
        {
            var redisCacheManager = new RedisCacheManager<SampleObject>(Substitute.For<IRedisDistributedCache>());

            var act = () => redisCacheManager.GetOrSetAsync<SampleObject?>("key", null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldThrowArgumentException_WhenKeyIsWhiteSpace()
        {
            var redisCacheManager = new RedisCacheManager<SampleObject>(Substitute.For<IRedisDistributedCache>());

            var act = () => redisCacheManager.GetOrSetAsync("   ", _ => Task.FromResult<SampleObject?>(null));

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldPassCancellationTokenToFactory_WhenKeyIsMissing()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>()).Returns((byte[]?)null);
            using var cts = new CancellationTokenSource();
            CancellationToken receivedToken = default;

            // Act
            await redisCacheManager.GetOrSetAsync("key", ct =>
            {
                receivedToken = ct;
                return Task.FromResult<SampleObject?>(new SampleObject { SomeProp = 1 });
            }, cancellationToken: cts.Token);

            // Assert
            receivedToken.Should().Be(cts.Token);
        }

        // A typical loader returns Task<T> (non-nullable). These tests use it the way callers do;
        // they must compile without CS8619 and give back a non-nullable T.
        private static Task<SampleObject> LoadSampleAsync(CancellationToken _)
            => Task.FromResult(new SampleObject { SomeProp = 42 });

        [Fact]
        public async Task GetOrSetAsync_ShouldAcceptNonNullableLoaderMethodGroup_AndStoreResult()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>()).Returns((byte[]?)null);

            // Act
            SampleObject result = await redisCacheManager.GetOrSetAsync("key", LoadSampleAsync);

            // Assert
            result.SomeProp.Should().Be(42);
            await redisDistributedCache.Received(1)
                .SetAsync("key", Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldReturnCachedValue_WhenUsingNonNullableLoaderLambda()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            var cached = new SampleObject { SomeProp = 10 };
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>())
                .Returns(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cached)));

            // Act
            SampleObject result = await redisCacheManager.GetOrSetAsync("key", ct => LoadSampleAsync(ct));

            // Assert
            result.Should().BeEquivalentTo(cached);
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldReturnCachedSubclass_WhenBaseClassIsMarkedWithJsonDerivedType()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<MarkedAnimal>(redisDistributedCache);
            MarkedAnimal cached = new MarkedDog { Name = "Rex", Breed = "Husky" };
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>())
                .Returns(JsonSerializer.SerializeToUtf8Bytes(cached));
            var factoryCalled = false;

            // Act
            MarkedDog result = await redisCacheManager.GetOrSetAsync("key", _ =>
            {
                factoryCalled = true;
                return Task.FromResult(new MarkedDog { Name = "Other", Breed = "Other" });
            });

            // Assert
            result.Name.Should().Be("Rex");
            result.Breed.Should().Be("Husky");
            factoryCalled.Should().BeFalse();
        }

        [Fact]
        public async Task GetOrSetAsync_ShouldThrowInvalidOperationException_WhenCachedValueIsNotTheRequestedSubclass()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<PlainAnimal>(redisDistributedCache);
            PlainAnimal cached = new PlainDog { Name = "Rex", Breed = "Husky" };
            redisDistributedCache.GetAsync("key", Arg.Any<CancellationToken>())
                .Returns(JsonSerializer.SerializeToUtf8Bytes(cached));
            var factoryCalled = false;

            // Act
            var act = () => redisCacheManager.GetOrSetAsync("key", _ =>
            {
                factoryCalled = true;
                return Task.FromResult(new PlainDog());
            });

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*PlainAnimal, not PlainDog*");
            factoryCalled.Should().BeFalse();
            await redisDistributedCache.DidNotReceive()
                .SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task TryRemoveAsync_ShouldReturnTrueAndRemove_WhenKeyIsValid()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            using var cts = new CancellationTokenSource();

            // Act
            var result = await redisCacheManager.TryRemoveAsync("key", cts.Token);

            // Assert
            result.Should().BeTrue();
            await redisDistributedCache.Received(1).RemoveAsync("key", cts.Token);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task TryRemoveAsync_ShouldReturnFalseWithoutCallingCache_WhenKeyIsInvalid(string? key)
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);

            // Act
            var result = await redisCacheManager.TryRemoveAsync(key);

            // Assert
            result.Should().BeFalse();
            await redisDistributedCache.DidNotReceiveWithAnyArgs().RemoveAsync(default!, default);
        }

        [Fact]
        public async Task TryRefreshAsync_ShouldReturnTrueAndRefresh_WhenKeyIsValid()
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);
            using var cts = new CancellationTokenSource();

            // Act
            var result = await redisCacheManager.TryRefreshAsync("key", cts.Token);

            // Assert
            result.Should().BeTrue();
            await redisDistributedCache.Received(1).RefreshAsync("key", cts.Token);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task TryRefreshAsync_ShouldReturnFalseWithoutCallingCache_WhenKeyIsInvalid(string? key)
        {
            // Arrange
            var redisDistributedCache = Substitute.For<IRedisDistributedCache>();
            var redisCacheManager = new RedisCacheManager<SampleObject>(redisDistributedCache);

            // Act
            var result = await redisCacheManager.TryRefreshAsync(key);

            // Assert
            result.Should().BeFalse();
            await redisDistributedCache.DidNotReceiveWithAnyArgs().RefreshAsync(default!, default);
        }
    }
#pragma warning restore CS8625
#pragma warning restore CS8604
}