namespace Altium.Dudko.SortApp;

internal static class PathGuard
{
    public static void Validate(string inputFilePath, string outputFilePath, string tempDirectoryPath)
    {
        var input = Path.GetFullPath(inputFilePath);
        var output = Path.GetFullPath(outputFilePath);
        var tempDirectory = Path.GetFullPath(tempDirectoryPath);

        if (input == output)
        {
            throw new InvalidOperationException($"Output file '{output}' must differ from input file '{input}'.");
        }

        if (input.StartsWith(tempDirectory))
        {
            throw new InvalidOperationException($"Input file '{input}' must not be inside temp directory '{tempDirectory}'.");
        }

        if (output.StartsWith(tempDirectory))
        {
            throw new InvalidOperationException($"Output file '{output}' must not be inside temp directory '{tempDirectory}'.");
        }
    }
}