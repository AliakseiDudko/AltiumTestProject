using Altium.Dudko.Core;

namespace Altium.Dudko.SortApp;

internal static class Program
{
    private static int Main(string[] args)
    {
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
    }

    private static string GetFilePath(string[] args)
    {
        string dataFilePath = null;
        if (args.Length > 0)
        {
            dataFilePath = Path.Combine(AppContext.BaseDirectory, args[0]);
        }

        if (string.IsNullOrWhiteSpace(dataFilePath))
        {
            throw new InvalidOperationException("File path not specified.");   
        }

        if (!File.Exists(dataFilePath))
        {
            throw new FileNotFoundException($"File {dataFilePath} not found.");
        }
        
        return dataFilePath;
    }
}