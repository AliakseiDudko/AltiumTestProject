using System.Globalization;

namespace Altium.Dudko.SortApp;

internal readonly record struct Line(long Number, string Text)
{
    public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Number}. {Text}");
}