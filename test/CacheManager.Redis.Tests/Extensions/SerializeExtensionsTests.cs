using System.Text.Json;
using CacheManager.Redis.Extensions;
using CacheManager.Redis.Tests.Models;
using FluentAssertions;
using Xunit;

namespace CacheManager.Redis.Tests.Extensions
{
    public sealed class SerializeExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid serialized string")]
        public void TryDeserialize_ShouldReturnFalseWithNullResponse_WhenCalledOnAnInvalidString(string? input)
        {
            // Arrange

            // Act
            var result = input.TryDeserialize<SampleObject>(out var response);
            
            // Assert
            result.Should().BeFalse();
            response.Should().BeNull();
        }

        [Fact]
        public void TryDeserialize_ShouldReturnTrueWithDeserializedObject_WhenCalledOnAValidString()
        {
            // Arrange
            var sampleObject = new SampleObject
            {
                Name = "RF",
                SomeProp = 10
            };
            var serializedString = JsonSerializer.Serialize(sampleObject);
            
            // Act
            var result = serializedString.TryDeserialize<SampleObject>(out var response);
            
            // Assert
            result.Should().BeTrue();
            response.Should().BeEquivalentTo(sampleObject);
        }
    }
}