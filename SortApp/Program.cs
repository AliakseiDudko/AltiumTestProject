using System.Diagnostics;
using System.Text;
using Altium.Dudko.Core;

namespace Altium.Dudko.SortApp;

internal static class Program
{
    private static int Main(string[] args)
    {
        SortAppOptions? options = null;
        var tempDirCreated = false;

        try
        {
            options = OptionsManager.Get<SortAppOptions>(SortAppOptions.SectionName);

            var filePath = GetInputFilePath(args);
            PathGuard.Validate(filePath, options.OutputFilePath, options.TempDirOutputPath);

            DirectoryManager.Create(options.TempDirOutputPath);
            tempDirCreated = true;

            var fileSize = new FileInfo(filePath).Length;
            Console.WriteLine($"File size: {fileSize} bytes.");

            var processTime = Stopwatch.StartNew();

            Console.WriteLine("Start splitting into sorted chunks...");
            var phaseTime = Stopwatch.StartNew();
            DivideIntoFiles(filePath, options);
            Console.WriteLine($@"Splitting into sorted chunks time: {phaseTime.Elapsed:hh\:mm\:ss\.fff}.");

            Console.WriteLine("Start merging into output file...");
            phaseTime.Restart();
            MergeFiles(options);
            Console.WriteLine($@"Merging into output file time: {phaseTime.Elapsed:hh\:mm\:ss\.fff}.");

            Console.WriteLine($@"Total sort time: {processTime.Elapsed:hh\:mm\:ss\.fff}.");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);

            return 1;
        }
        finally
        {
            if (options != null && tempDirCreated)
            {
                DirectoryManager.Clear(options.TempDirOutputPath);
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

    private static void DivideIntoFiles(string filePath, SortAppOptions options)
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

    private static void MergeFiles(SortAppOptions options)
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