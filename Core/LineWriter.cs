using System.Globalization;

namespace Altium.Dudko.Core;

public static class LineWriter
{
    public static int Write(StreamWriter writer, Line line)
    {
        Span<char> numberBuffer = stackalloc char[20];
        line.Number.TryFormat(numberBuffer, out var numberDigits, provider: CultureInfo.InvariantCulture);

        writer.Write(numberBuffer[..numberDigits]);
        writer.Write(Line.Separator);
        writer.Write(line.Text);
        writer.Write('\n');
        
        return numberDigits + Line.Separator.Length + line.Text.Length + 1;
    }
}