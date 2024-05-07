using CacheManager.Redis.Extensions;
using CacheManager.Redis.Interfaces;
using CacheManager.Redis.Tests.Fakers;
using CacheManager.Redis.Tests.Models;
using FluentAssertions;
using Xunit;

namespace CacheManager.Redis.Tests.Extensions
{
    public sealed class TypeExtensionsTests
    {
        [Fact]
        public void IsAssignableToGenericType_ShouldReturnFalse_WhenCalledOnAnUnAssignableType()
        {
            // Arrange
            var input = typeof(SampleObject);
            var genericType = typeof(IRedisCacheManager<>);
            
            // Act
            var result = input.IsAssignableToGenericType(genericType);
            
            // Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public void IsAssignableToGenericType_ShouldReturnTrue_WhenCalledOnAnAssignableType()
        {
            // Arrange
            var input = typeof(FakeCustomCacheManager<>);
            var genericType = typeof(IRedisCacheManager<>);
            
            // Act
            var result = input.IsAssignableToGenericType(genericType);
            
            // Assert
            result.Should().BeTrue();
        }
    }
}