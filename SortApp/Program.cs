using System.Diagnostics;
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
            ChunkSplitter.Split(filePath, options);
            Console.WriteLine($@"Splitting into sorted chunks time: {phaseTime.Elapsed:hh\:mm\:ss\.fff}.");

            Console.WriteLine("Start merging into output file...");
            phaseTime.Restart();
            FileMerger.Merge(options);
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
}