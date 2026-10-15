using System.ComponentModel.DataAnnotations;

namespace Altium.Dudko.SortApp;

internal sealed class SortAppOptions
{
    public const string SectionName = "SortApp";

    [Range(1, 4096)]
    public int ChunkSizeMb { get; init; }
    
    [Range(0, int.MaxValue)]
    public int ChunkListCapacity { get; init; }

    [Required]
    public required string TempDirOutputPath { get; init; }

    [Required]
    public required string OutputFilePath { get; init; }
}