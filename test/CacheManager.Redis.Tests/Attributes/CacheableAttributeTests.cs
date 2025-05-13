using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Redis.Attributes;
using CacheManager.Redis.Enums;
using CacheManager.Redis.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Filters;
using NSubstitute;
using Xunit;

namespace CacheManager.Redis.Tests.Attributes;

public sealed class CacheableAttributeTests
{
    [Fact]
    public void Constructor_ShouldSetKey_WhenKeyTypeIsFromRouteOrQuery_AndPrefixIsProvided()
    {
        // Arrange
        var cacheManager = Substitute.For<IRedisCacheManager<object>>();
        var key = "testKey";
        var keyType = CacheableKeyType.FromRouteOrQuery;
        var prefix = "testPrefix";

        // Act
        var attribute = new CacheableAttribute<object>(cacheManager, key, keyType, prefix);

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<CacheableAttribute<object>>();
        attribute.GetType().GetProperty("Key")?.GetValue(attribute).Should().Be(key);
        attribute.GetType().GetProperty("KeyType")?.GetValue(attribute).Should().Be(keyType);
        attribute.GetType().GetProperty("Prefix")?.GetValue(attribute).Should().Be("prefix");
        
    }
    
    [Fact]
    public void Constructor_ShouldSetKey_WhenKeyTypeIsFromRouteOrQuery_AndPrefixNotProvided()
    {
        // Arrange
        var cacheManager = Substitute.For<IRedisCacheManager<object>>();
        var key = "testKey";
        var keyType = CacheableKeyType.FromRouteOrQuery;

        // Act
        var attribute = new CacheableAttribute<object>(cacheManager, key, keyType);

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<CacheableAttribute<object>>();
        attribute.GetType().GetProperty("Key")?.GetValue(attribute).Should().Be(key);
        attribute.GetType().GetProperty("KeyType")?.GetValue(attribute).Should().Be(keyType);
        attribute.GetType().GetProperty("Prefix")?.GetValue(attribute).Should().NotBe(string.Empty);
    }
    
    [Fact]
    public void Constructor_ShouldSetKey_WhenKeyTypeIsFromModel_AndPrefixIsProvided()
    {
        // Arrange
        var cacheManager = Substitute.For<IRedisCacheManager<object>>();
        var key = "testKey";
        var keyType = CacheableKeyType.FromModel;
        var prefix = "testPrefix";

        // Act
        var attribute = new CacheableAttribute<object>(cacheManager, key, keyType, prefix);

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<CacheableAttribute<object>>();
        attribute.GetType().GetProperty("Key")?.GetValue(attribute).Should().Be(key);
        attribute.GetType().GetProperty("KeyType")?.GetValue(attribute).Should().Be(keyType);
        attribute.GetType().GetProperty("Prefix")?.GetValue(attribute).Should().Be(prefix);
    }

    [Fact]
    public void Constructor_ShouldSetKey_WhenKeyTypeIsFromModel_AndPrefixNotProvided()
    {
        // Arrange
        var cacheManager = Substitute.For<IRedisCacheManager<object>>();
        var key = "testKey";
        var keyType = CacheableKeyType.FromModel;

        // Act
        var attribute = new CacheableAttribute<object>(cacheManager, key, keyType);

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<CacheableAttribute<object>>();
        attribute.GetType().GetProperty("Key")?.GetValue(attribute).Should().Be(key);
        attribute.GetType().GetProperty("KeyType")?.GetValue(attribute).Should().Be(keyType);
        attribute.GetType().GetProperty("Prefix")?.GetValue(attribute).Should().NotBe(string.Empty);
    }
    
    [Fact]
    public void Constructor_ShouldSetKey_WhenKeyTypeIsMethodName()
    {
        // Arrange
        var cacheManager = Substitute.For<IRedisCacheManager<object>>();
        var key = "testKey";
        var keyType = CacheableKeyType.MethodName;

        // Act
        var attribute = new CacheableAttribute<object>(cacheManager, key, keyType);

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<CacheableAttribute<object>>();
        attribute.GetType().GetProperty("Key")?.GetValue(attribute).Should().Be(key);
        attribute.GetType().GetProperty("KeyType")?.GetValue(attribute).Should().Be(keyType);
    }
    
    [Fact]
    public void Constructor_ShouldSetKey_WhenKeyTypeIsProvidedValue()
    {
        // Arrange
        var cacheManager = Substitute.For<IRedisCacheManager<object>>();
        var key = "testKey";
        var keyType = CacheableKeyType.FromProvidedValue;

        // Act
        var attribute = new CacheableAttribute<object>(cacheManager, key, keyType);

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<CacheableAttribute<object>>();
        attribute.GetType().GetProperty("Key")?.GetValue(attribute).Should().Be(key);
        attribute.GetType().GetProperty("KeyType")?.GetValue(attribute).Should().Be(keyType);
    }
    
    [Fact]
    public void Constructor_ShouldSetKey_WhenNoArgumentsProvided()
    {
        // Arrange
        var cacheManager = Substitute.For<IRedisCacheManager<object>>();

        // Act
        var attribute = new CacheableAttribute<object>(cacheManager);

        // Assert
        attribute.Should().NotBeNull();
    }
}