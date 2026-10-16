using Altium.Dudko.Core;

namespace Altium.Dudko.Tests;

public class LineTests
{
    [Theory]
    [InlineData("415. Apple", 415, "Apple")]
    [InlineData("0. Apple", 0, "Apple")]
    [InlineData("9223372036854775807. Apple", long.MaxValue, "Apple")]
    [InlineData("1. Apple. Banana is yellow", 1, "Apple. Banana is yellow")]
    [InlineData("2. ", 2, "")]
    public void Parse_ValidLine_ReturnsNumberAndText(string input, long number, string text)
    {
        var line = Line.Parse(input);

        Assert.Equal(new Line(number, text), line);
    }

    [Theory]
    [InlineData("415 Apple")]
    [InlineData("415.Apple")]
    [InlineData(". Apple")]
    [InlineData("abc. Apple")]
    [InlineData("-1. Apple")]
    [InlineData("+1. Apple")]
    [InlineData("9223372036854775808. Apple")]
    public void Parse_InvalidLine_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(() => Line.Parse(input));
    }

    [Fact]
    public void Parse_EmptyNumber_ReportsInvalidNumber()
    {
        var exception = Assert.Throws<FormatException>(() => Line.Parse(". Apple"));

        Assert.Contains("valid Number", exception.Message);
    }

    [Fact]
    public void ToString_ReturnsNumberSeparatorText()
    {
        var line = new Line(415, "Apple is red");

        Assert.Equal("415. Apple is red", line.ToString());
    }
}