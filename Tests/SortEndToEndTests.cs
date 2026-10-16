using System.Text;
using Altium.Dudko.Core;
using Altium.Dudko.SortApp;

namespace Altium.Dudko.Tests;

public sealed class SortEndToEndTests : IDisposable
{
    private readonly string _rootPath = Path.Combine(Path.GetTempPath(), $"SortEndToEndTests-{Guid.NewGuid():N}");
    private readonly SortAppOptions _options;

    public SortEndToEndTests()
    {
        _options = new SortAppOptions
        {
            ChunkSizeMb = 1,
            ChunkListCapacity = 1000,
            TempDirOutputPath = Path.Combine(_rootPath, "Temp"),
            OutputFileBufferSize = 4096,
            OutputFilePath = Path.Combine(_rootPath, "Output", "SortedLines.txt"),
        };

        DirectoryManager.Create(_options.TempDirOutputPath);
    }

    public void Dispose() => Directory.Delete(_rootPath, recursive: true);

    [Fact]
    public void Sort_EmptyFile_WritesEmptyOutput()
    {
        var output = Sort("");

        Assert.Equal("", output);
    }

    [Fact]
    public void Sort_OneLine_WritesSameLine()
    {
        var output = Sort("415. Apple\n");

        Assert.Equal("415. Apple\n", output);
    }

    [Fact]
    public void Sort_NoTrailingLineBreak_WritesAllLines()
    {
        var output = Sort("2. Banana\n1. Apple");

        Assert.Equal("1. Apple\n2. Banana\n", output);
    }

    [Fact]
    public void Sort_WindowsLineEndings_WritesUnixLineEndings()
    {
        var output = Sort("2. Banana\r\n1. Apple\r\n");

        Assert.Equal("1. Apple\n2. Banana\n", output);
    }

    [Fact]
    public void Sort_SpecExample_WritesExpectedOrder()
    {
        var output = Sort("415. Apple\n30432. Something something something\n1. Apple\n32. Cherry is the best\n2. Banana is yellow\n");

        Assert.Equal("1. Apple\n415. Apple\n2. Banana is yellow\n32. Cherry is the best\n30432. Something something something\n", output);
    }

    [Fact]
    public void Sort_FileLargerThanChunk_MergesSeveralChunks()
    {
        var inputLines = GenerateLines(10 * 1024 * 1024);
        var inputFilePath = WriteInput(string.Concat(inputLines.Select(x => x + "\n")));

        var chunkFilePaths = ChunkSplitter.Split(inputFilePath, _options);
        FileMerger.Merge(chunkFilePaths, _options);

        var expected = inputLines.Select(Line.Parse).Order(LineComparer.Instance).Select(x => x.ToString());
        var actual = File.ReadAllLines(_options.OutputFilePath);
        Assert.True(chunkFilePaths.Count > 1, $"Expected several chunks, got {chunkFilePaths.Count}.");
        Assert.Equal(expected, actual);
    }

    private string Sort(string input)
    {
        var inputFilePath = WriteInput(input);

        var chunkFilePaths = ChunkSplitter.Split(inputFilePath, _options);
        FileMerger.Merge(chunkFilePaths, _options);

        return File.ReadAllText(_options.OutputFilePath);
    }

    private string WriteInput(string content)
    {
        var inputFilePath = Path.Combine(_rootPath, "Lines.txt");
        File.WriteAllText(inputFilePath, content, new UTF8Encoding(false));

        return inputFilePath;
    }

    private static List<string> GenerateLines(long size)
    {
        string[] phrases = ["Apple", "Apple is red", "apple", "APPLE", "Banana is yellow", "Cherry is the best", "Something something something"];
        var random = new Random(42);
        var lines = new List<string>();
        long written = 0;
        while (written < size)
        {
            var line = new Line(random.NextInt64(0, 1000), phrases[random.Next(phrases.Length)]).ToString();
            lines.Add(line);
            written += line.Length + 1;
        }

        return lines;
    }
}