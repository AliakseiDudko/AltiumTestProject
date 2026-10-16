using System.Globalization;

namespace Altium.Dudko.Core;

public readonly record struct Line(long Number, string Text)
{
    public const string Separator = ". ";

    public static Line Parse(string s)
    {
        var dotSeparatorIndex = s.IndexOf(Separator, StringComparison.Ordinal);
        if (dotSeparatorIndex <= 0)
        {
            throw new FormatException($"String '{s}' does not contain a dot-separator character.");
        }

        if (!long.TryParse(s.AsSpan(0, dotSeparatorIndex), NumberStyles.None, CultureInfo.InvariantCulture, out var number))
        {
            throw new FormatException($"String '{s}' does not contain a valid Number.");
        }

        var text = s[(dotSeparatorIndex + 2)..];

        return new Line(number, text);
    }

    public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Number}{Separator}{Text}");
}