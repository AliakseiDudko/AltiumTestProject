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

        if (string.IsNullOrWhiteSpace(dataFilePath))
        {
            throw new InvalidOperationException("File path not specified.");   
            Directory.Delete(tempDirFullPath, true);

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