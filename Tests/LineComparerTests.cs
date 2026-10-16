using Altium.Dudko.Core;
using Altium.Dudko.SortApp;

namespace Altium.Dudko.Tests;

public class LineComparerTests
{
    [Theory]
    [InlineData("1. Apple", "415. Apple")]
    [InlineData("2. Banana", "1. Cherry")]
    [InlineData("5. Apple", "1. Apple is red")]
    [InlineData("1. APPLE", "1. Apple")]
    [InlineData("1. Apple", "1. apple")]
    [InlineData("9. Zebra", "1. apple")]
    public void Compare_FirstLineIsSmaller(string first, string second)
    {
        var x = Line.Parse(first);
        var y = Line.Parse(second);

        Assert.True(LineComparer.Instance.Compare(x, y) < 0);
        Assert.True(LineComparer.Instance.Compare(y, x) > 0);
    }

    [Fact]
    public void Compare_EqualLines_ReturnsZero()
    {
        var x = new Line(1, "Apple");
        var y = new Line(1, "Apple");

        Assert.Equal(0, LineComparer.Instance.Compare(x, y));
    }

    [Fact]
    public void Sort_SpecExample_ReturnsExpectedOrder()
    {
        var lines = new List<Line>
        {
            Line.Parse("415. Apple"),
            Line.Parse("30432. Something something something"),
            Line.Parse("1. Apple"),
            Line.Parse("32. Cherry is the best"),
            Line.Parse("2. Banana is yellow"),
        };

        lines.Sort(LineComparer.Instance);

        Assert.Equal(
            ["1. Apple", "415. Apple", "2. Banana is yellow", "32. Cherry is the best", "30432. Something something something"],
            lines.Select(x => x.ToString()));
    }
}