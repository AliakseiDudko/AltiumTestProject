namespace Altium.Dudko.SortApp;

internal static class DirectoryManager
{
    public static void Create(string tempDirectoryPath)
    {
        var fullPath = Path.GetFullPath(tempDirectoryPath);
        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }

        Directory.CreateDirectory(fullPath);
    }

    public static void Clear(string tempDirectoryPath)
    {
        try
        {
            var fullPath = Path.GetFullPath(tempDirectoryPath);
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, recursive: true);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
}