namespace Altium.Dudko.Core;

public sealed class LineComparer : IComparer<Line>
{
    public int Compare(Line x, Line y)
    {
        var textComparison = string.Compare(x.Text, y.Text, StringComparison.Ordinal);

        return textComparison != 0
            ? textComparison
            : x.Number.CompareTo(y.Number);
    }
}