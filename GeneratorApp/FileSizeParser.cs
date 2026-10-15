namespace Altium.Dudko.GeneratorApp;

internal static class FileSizeParser
{
    public static bool TryParseFileSize(string input, out long fileSize)
    {
        fileSize = 0;
        var upperInput = input.Trim().ToUpperInvariant();

        long multiplier = 1;
        if (upperInput.EndsWith("KB"))
        {
            multiplier = 1024;
        }
        else if (upperInput.EndsWith("MB"))
        {
            multiplier = 1024 * 1024;
        }
        else if (upperInput.EndsWith("GB"))
        {
            multiplier = 1024 * 1024 * 1024;
        }

        if (multiplier != 1)
        {
            upperInput = upperInput[..^2];
        }

        if (long.TryParse(upperInput, out var value) && value > 0 && value <= long.MaxValue / multiplier)
        {
            fileSize = value * multiplier;

            return true;
        }

        return false;
    }
}