using CacheManager.Redis.Extensions;
using FluentAssertions;
using Xunit;

namespace CacheManager.Redis.Tests.Extensions;

public sealed class HelpersTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("value", true)]
    public void HasValue_ShouldReturnExpectedResult(string? input, bool expected)
    {
        var result = input.HasValue();

        result.Should().Be(expected);
    }
}

