using System.Globalization;

namespace Altium.Dudko.SortApp;

internal static class LineWriter
{
    public static void Write(StreamWriter writer, Line line)
    {
        Span<char> numberBuffer = stackalloc char[20];
        line.Number.TryFormat(numberBuffer, out var numberDigits, provider: CultureInfo.InvariantCulture);

        writer.Write(numberBuffer[..numberDigits]);
        writer.Write(". ");
        writer.Write(line.Text);
        writer.Write('\n');
    }
}