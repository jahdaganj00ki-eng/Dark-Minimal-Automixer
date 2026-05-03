using AutoDJMix.Domain.Music;
using Xunit;

namespace AutoDJMix.Domain.Tests.Music;

public sealed class CamelotKeyTests
{
    [Theory]
    [InlineData("1A")]
    [InlineData("2B")]
    [InlineData("6B")]
    public void Parse_valid_values(string value)
    {
        var key = CamelotKey.Parse(value);
        Assert.Equal(value, key.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0A")]
    [InlineData("1C")]
    [InlineData("AA")]
    public void Parse_invalid_values_throw(string value)
    {
        Assert.ThrowsAny<Exception>(() => CamelotKey.Parse(value));
    }
}
