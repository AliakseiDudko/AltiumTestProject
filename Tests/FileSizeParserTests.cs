using Altium.Dudko.GeneratorApp;

namespace Altium.Dudko.Tests;

public class FileSizeParserTests
{
    [Theory]
    [InlineData("1048576", 1048576L)]
    [InlineData("1KB", 1024L)]
    [InlineData("500mb", 500L * 1024 * 1024)]
    [InlineData("100GB", 100L * 1024 * 1024 * 1024)]
    [InlineData(" 10 KB ", 10L * 1024)]
    [InlineData("8589934591GB", 8589934591L * 1024 * 1024 * 1024)]
    public void TryParseFileSize_ValidInput_ReturnsSize(string input, long expected)
    {
        var result = FileSizeParser.TryParseFileSize(input, out var fileSize);

        Assert.True(result);
        Assert.Equal(expected, fileSize);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-5")]
    [InlineData("abc")]
    [InlineData("KB")]
    [InlineData("")]
    [InlineData("1.5GB")]
    [InlineData("99999999999GB")]
    public void TryParseFileSize_InvalidInput_ReturnsFalse(string input)
    {
        var result = FileSizeParser.TryParseFileSize(input, out var fileSize);

        Assert.False(result);
        Assert.Equal(0, fileSize);
    }
}