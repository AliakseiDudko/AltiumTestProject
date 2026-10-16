using System.Text;
using Altium.Dudko.Core;

namespace Altium.Dudko.SortApp;

internal static class ChunkSplitter
{
    public static IReadOnlyList<string> Split(string filePath, SortAppOptions options)
    {
        var chunkFilePaths = new List<string>();

        var currentLines = new List<Line>(options.ChunkListCapacity);
        var currentFileSize = 0L;
        var maxChunkChars = options.ChunkSizeMb * 1024L * 1024;

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, options.OutputFileBufferSize, FileOptions.SequentialScan);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, options.OutputFileBufferSize);
        while (true)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                if (currentLines.Count > 0)
                {
                    var chunkFilePath = SortAndWriteChunkFile(currentLines, chunkFilePaths.Count, options);
                    chunkFilePaths.Add(chunkFilePath);
                }

                break;
            }

            var newLine = Line.Parse(line);
            currentLines.Add(newLine);
            currentFileSize += line.Length + 1;

            if (currentFileSize >= maxChunkChars)
            {
                var chunkFilePath = SortAndWriteChunkFile(currentLines, chunkFilePaths.Count, options);
                chunkFilePaths.Add(chunkFilePath);

                currentLines.Clear();
                currentFileSize = 0;
            }
        }

        return chunkFilePaths;
    }

    private static string SortAndWriteChunkFile(List<Line> lines, int fileIndex, SortAppOptions options)
    {
        lines.Sort(LineComparer.Instance);

        var filePath = Path.Combine(options.TempDirOutputPath, $"chunk-{fileIndex:D5}.txt");
        using var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, options.OutputFileBufferSize);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false), options.OutputFileBufferSize);

        foreach (var line in lines)
        {
            LineWriter.Write(writer, line);
        }

        return filePath;
    }
}