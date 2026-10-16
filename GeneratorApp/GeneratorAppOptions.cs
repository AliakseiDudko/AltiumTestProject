using System.ComponentModel.DataAnnotations;

namespace Altium.Dudko.GeneratorApp;

internal sealed class GeneratorAppOptions
{
    public const string SectionName = "GeneratorApp";

    [Range(0, long.MaxValue)]
    public long MaxNumber { get; init; }

    [Required]
    public required string SeedPhraseFilePath { get; init; }

    [Range(0, 100)]
    public int DuplicateRate { get; init; }

    [Range(4096, int.MaxValue)]
    public int OutputFileBufferSize { get; init; }

    [Required]
    public required string OutputFilePath { get; init; }
}