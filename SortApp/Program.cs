using System.Globalization;
using System.Text;
using Altium.Dudko.Core;

namespace Altium.Dudko.SortApp;

internal static class Program
{
    private static int Main(string[] args)
    {
        SortAppOptions options = null;

        try
        {
            options = OptionsManager.Get<SortAppOptions>(SortAppOptions.SectionName);

            CreateTempDirectory(options.TempDirOutputPath);
            Console.WriteLine($"Temp directory {options.TempDirOutputPath} created.");

            var filePath = GetInputFilePath(args);
            SortFile(filePath, options);

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);

            return 1;
        }
        finally
        {
            if (options != null)
            {
                ClearTempDirectory(options.TempDirOutputPath);
                Console.WriteLine($"Temp directory {options.TempDirOutputPath} cleared.");
            }
        }
    }

    private static string GetInputFilePath(string[] args)
    {
        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
        {
            throw new InvalidOperationException("Input file is not specified.");
        }

        var inputFilePath = Path.GetFullPath(args[0]);
        if (!File.Exists(inputFilePath))
        {
            throw new FileNotFoundException($"Input file '{inputFilePath}' not found.", inputFilePath);
        }

        return inputFilePath;
    }

    private static void SortFile(string filePath, SortAppOptions options)
    {
        DivideIntoFiles(filePath, options);
    }

    private static void DivideIntoFiles(string filePath, SortAppOptions options)
    {
        var currentLines = new List<Line>(options.ChunkListCapacity);
        var currentFileIndex = 0;
        long currentFileSize = 0;
        long maxChunkChars = options.ChunkSizeMb * 1024 * 1024;

        using var reader = new StreamReader(filePath);
        while (true)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                SortAndWriteChunkFile(currentLines, currentFileIndex, options);

                break;
            }

            var newLine = ParseLine(line);
            currentLines.Add(newLine);
            currentFileSize += line.Length + 1;

            if (currentFileSize >= maxChunkChars)
            {
                SortAndWriteChunkFile(currentLines, currentFileIndex++, options);
                currentLines.Clear();
                currentFileSize = 0;
            }
        }
    }

    private static Line ParseLine(string line)
    {
        var dotSeparatorIndex = line.IndexOf(". ", StringComparison.Ordinal);
        var number = long.Parse(line.AsSpan(0, dotSeparatorIndex), NumberStyles.None, CultureInfo.InvariantCulture);
        var text = line[(dotSeparatorIndex + 2)..];

        return new Line(number, text);
    }

    private static void SortAndWriteChunkFile(List<Line> lines, int fileIndex, SortAppOptions options)
    {
        lines.Sort(LineComparer.Instance);

        var filePath = Path.Combine(options.TempDirOutputPath, $"chuck-{fileIndex:D5}.txt");
        using var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, options.OutputFileBufferSize);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false), options.OutputFileBufferSize);

        foreach (var line in lines)
        {
            LineWriter.Write(writer, line);
        }
    }

    private static void CreateTempDirectory(string tempDirOutputPath)
    {
        var fullPath = Path.GetFullPath(tempDirOutputPath);
        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }

        Directory.CreateDirectory(fullPath);
    }

    private static void ClearTempDirectory(string tempDirOutputPath)
    {
        var fullPath = Path.GetFullPath(tempDirOutputPath);
        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }
    }
}