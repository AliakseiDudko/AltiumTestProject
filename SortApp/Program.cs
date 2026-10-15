using Altium.Dudko.Core;

namespace Altium.Dudko.SortApp;

internal static class Program
{
    private static int Main(string[] args)
    {
        SortAppOptions options = null;

        try
        {
            var filePath = GetFilePath(args);
            
            var options = OptionsManager.Get<SortAppOptions>(SortAppOptions.SectionName);

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
            }
        }
    }

    private static string GetFilePath(string[] args)
    private static void ClearTempDirectory(string tempDirOutputPath)
    {
        string dataFilePath = null;
        if (args.Length > 0)
        var tempDirFullPath = Path.Combine(AppContext.BaseDirectory, tempDirOutputPath);
        if (Directory.Exists(tempDirFullPath))
        {
            dataFilePath = Path.Combine(AppContext.BaseDirectory, args[0]);
        }

        if (string.IsNullOrWhiteSpace(dataFilePath))
        {
            throw new InvalidOperationException("File path not specified.");   
            Directory.Delete(tempDirFullPath, true);
        }

        if (!File.Exists(dataFilePath))
        {
            throw new FileNotFoundException($"File {dataFilePath} not found.");
        }
        
        return dataFilePath;
    }
}