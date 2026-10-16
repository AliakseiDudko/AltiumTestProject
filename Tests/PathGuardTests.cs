using Altium.Dudko.SortApp;

namespace Altium.Dudko.Tests;

public class PathGuardTests
{
    [Fact]
    public void Validate_DifferentPathsOutsideTemp_DoesNotThrow()
    {
        PathGuard.Validate("./Input/Lines.txt", "./Output/SortedLines.txt", "./Temp");
    }

    [Theory]
    [InlineData("./Output/Lines.txt", "./Output/Lines.txt")]
    [InlineData("./Output/Lines.txt", "Output/../Output/Lines.txt")]
    public void Validate_OutputEqualsInput_Throws(string input, string output)
    {
        Assert.Throws<InvalidOperationException>(() => PathGuard.Validate(input, output, "./Temp"));
    }

    [Fact]
    public void Validate_InputInsideTemp_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => PathGuard.Validate("./Temp/Lines.txt", "./Output/SortedLines.txt", "./Temp"));
    }

    [Fact]
    public void Validate_OutputInsideTemp_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => PathGuard.Validate("./Lines.txt", "./Temp/SortedLines.txt", "./Temp"));
    }
}