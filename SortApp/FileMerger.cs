using System.Text;
using Altium.Dudko.Core;

namespace Altium.Dudko.SortApp;

internal static class FileMerger
{
    public static void Merge(SortAppOptions options)
    {
        var tempDirFullPath = Path.GetFullPath(options.TempDirOutputPath);
        var chunkFiles = Directory.GetFiles(tempDirFullPath, "chunk-*.txt");

        var readers = new StreamReader[chunkFiles.Length];
        try
        {
            var queue = new PriorityQueue<int, Line>(LineComparer.Instance);

            for (var i = 0; i < chunkFiles.Length; i++)
            {
                var readStream = new FileStream(chunkFiles[i], FileMode.Open, FileAccess.Read, FileShare.Read, options.OutputFileBufferSize, FileOptions.SequentialScan);
                readers[i] = new StreamReader(readStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, options.OutputFileBufferSize);

                if (TryReadNextLine(readers[i], out var newLine))
                {
                    queue.Enqueue(i, newLine);
                }
            }

            var fullPath = FilePathManager.Prepare(options.OutputFilePath);
            using var writeStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, options.OutputFileBufferSize);
            using var writer = new StreamWriter(writeStream, new UTF8Encoding(false), options.OutputFileBufferSize);
            while (queue.TryDequeue(out var runIndex, out var line))
            {
                LineWriter.Write(writer, line);

                if (TryReadNextLine(readers[runIndex], out var next))
                {
                    queue.Enqueue(runIndex, next);
                }
            }
        }
        finally
        {
            foreach (var reader in readers)
            {
                reader?.Dispose();
            }
        }
    }

    private static bool TryReadNextLine(StreamReader reader, out Line newLine)
    {
        var line = reader.ReadLine();
        if (line == null)
        {
            newLine = default;

            return false;
        }

        newLine = Line.Parse(line);

        return true;
    }
}