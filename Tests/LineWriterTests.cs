using System.Text;
using Altium.Dudko.Core;

namespace Altium.Dudko.Tests;

public class LineWriterTests
{
    [Fact]
    public void Write_WritesNumberSeparatorTextAndNewLine()
    {
        var output = Write(new Line(415, "Apple is red"), out _);

        Assert.Equal("415. Apple is red\n", output);
    }

    [Fact]
    public void Write_ReturnsNumberOfWrittenCharacters()
    {
        var output = Write(new Line(415, "Apple is red"), out var written);

        Assert.Equal(output.Length, written);
    }

    [Theory]
    [InlineData(0, "Apple")]
    [InlineData(long.MaxValue, "Apple. Banana is yellow")]
    [InlineData(7, "")]
    public void Write_ThenParse_ReturnsSameLine(long number, string text)
    {
        var line = new Line(number, text);

        var output = Write(line, out _);

        Assert.Equal(line, Line.Parse(output.TrimEnd('\n')));
    }

    private static string Write(Line line, out int written)
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true))
        {
            written = LineWriter.Write(writer, line);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}