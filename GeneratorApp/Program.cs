using System.Text;
using Altium.Dudko.Core;

namespace Altium.Dudko.GeneratorApp;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            var fileSize = ReadFileSize(args);
            Console.WriteLine($"FileSize: {fileSize} bytes.");

            var options = OptionsManager.Get<GeneratorAppOptions>(GeneratorAppOptions.SectionName);

            var seedPhrases = LoadSeedPhrases(options);
            Console.WriteLine($"Loaded {seedPhrases.Length} seed phrases from {options.SeedPhraseFilePath}.");

            var writtenCharacters = WriteFile(fileSize, seedPhrases, options);
            Console.WriteLine($"{writtenCharacters} characters were written to {options.OutputFilePath}.");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);

            return 1;
        }
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
            if (input is null)
            {
                throw new InvalidOperationException("File size was not provided: input stream is closed.");
            }

            if (FileSizeParser.TryParseFileSize(input, out var fileSize))
            {
                return fileSize;
            }
        }
    }

    private static string[] LoadSeedPhrases(GeneratorAppOptions options)
    {
        var seedPhraseFilePath = Path.Combine(AppContext.BaseDirectory, options.SeedPhraseFilePath);
        var seedPhrases = File
            .ReadAllLines(seedPhraseFilePath)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
        if (seedPhrases.Length == 0)
        {
            throw new InvalidOperationException("No seed phrases were provided.");
        }

        return seedPhrases;
    }

    private static long WriteFile(long fileSize, string[] seedPhrases, GeneratorAppOptions options)
    {
        var fullPath = FilePathManager.Prepare(options.OutputFilePath);
        using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, options.OutputFileBufferSize);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false), options.OutputFileBufferSize);

        var random = new Random();
        var phrase = seedPhrases[random.Next(seedPhrases.Length)];

        long writtenCharacters = 0;
        while (writtenCharacters < fileSize)
        {
            var number = random.NextInt64(1, options.MaxNumber + 1L);
            var line = new Line(number, phrase);
            writtenCharacters += LineWriter.Write(writer, line);

            if (random.Next(100) >= options.DuplicateRate)
            {
                phrase = seedPhrases[random.Next(seedPhrases.Length)];
            }
        }

        return writtenCharacters;
    }
}