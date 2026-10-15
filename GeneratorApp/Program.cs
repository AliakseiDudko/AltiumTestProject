using Altium.Dudko.Core;

namespace Altium.Dudko.GeneratorApp;

internal static class Program
{
    private static int Main(string[] args)
    {
        var fileSize = ReadFileSize(args);
        Console.WriteLine($"FileSize: {fileSize} bytes.");

        var seedPhrases = File.ReadAllLines("./Resources/SeedPhrases.txt");
        Console.WriteLine($"Loaded {seedPhrases.Length} seed phrases.");
        var options = OptionsManager.Get<GeneratorAppOptions>(GeneratorAppOptions.SectionName);

        var seedPhrases = File.ReadAllLines(options.SeedPhraseFilePath);
        Console.WriteLine($"Loaded {seedPhrases.Length} seed phrases from {options.SeedPhraseFilePath}.");

        return 0;
    }

    private static long ReadFileSize(string[] args)
    {
        if (args.Length > 0)
        {
            if (FileSizeParser.TryParseFileSize(args[0], out var fileSize))
            {
                return fileSize;
            }

            Console.WriteLine($"Invalid file size: {args[0]}.");
        }

        while (true)
        {
            Console.Write("Enter file size (100, 100KB, 100MB, 100GB): ");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            if (FileSizeParser.TryParseFileSize(input, out var fileSize))
            {
                return fileSize;
            }
        }
    }
}