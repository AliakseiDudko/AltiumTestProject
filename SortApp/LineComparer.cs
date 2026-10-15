namespace Altium.Dudko.SortApp;

internal sealed class LineComparer : IComparer<Line>
{
    public static readonly LineComparer Instance = new();

    private LineComparer()
    {
    }

    public int Compare(Line x, Line y)
    {
        var textComparison = string.Compare(x.Text, y.Text, StringComparison.Ordinal);

        return textComparison != 0
            ? textComparison
            : x.Number.CompareTo(y.Number);
    }
}