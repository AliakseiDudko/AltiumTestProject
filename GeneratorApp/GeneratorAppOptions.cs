namespace Altium.Dudko.GeneratorApp;

public sealed class GeneratorAppOptions
{
    public const string SectionName = "GeneratorApp";

    public int MaxNumber { get; init; }
    public string SeedPhraseFilePath { get; init; }
    public int DuplicateRate { get; init; }
    public int OutputFileBufferSize { get; init; }
    public string OutputFilePath { get; init; }
}