using System.Text;
using Altium.Dudko.Core;

namespace Altium.Dudko.SortApp;

internal static class ChunkSplitter
{
    public static void Split(string filePath, SortAppOptions options)
    {
        var currentLines = new List<Line>(options.ChunkListCapacity);
        var currentFileIndex = 0;
        long currentFileSize = 0;
        long maxChunkChars = options.ChunkSizeMb * 1024L * 1024;

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, options.OutputFileBufferSize, FileOptions.SequentialScan);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, options.OutputFileBufferSize);
        while (true)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                SortAndWriteChunkFile(currentLines, currentFileIndex, options);

                break;
            }

            var newLine = Line.Parse(line);
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

    private static void SortAndWriteChunkFile(List<Line> lines, int fileIndex, SortAppOptions options)
    {
        if (lines.Count == 0)
        {
            return;
        }

        lines.Sort(LineComparer.Instance);

        var filePath = Path.Combine(options.TempDirOutputPath, $"chunk-{fileIndex:D5}.txt");
        using var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, options.OutputFileBufferSize);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false), options.OutputFileBufferSize);

        foreach (var line in lines)
        {
            LineWriter.Write(writer, line);
        }
    }
}