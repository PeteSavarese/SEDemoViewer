using SEDemoViewer.Core.Models;
using SEDemoViewer.Core.Services;

namespace SEDemoViewer.Core.Tests.Services;

// xUnit creates a new instance of this class for every test so the constructor and Dispose()
// give each test its own empty temp folder.
public sealed class DemoScannerTests : IDisposable
{
    private readonly DirectoryInfo _root = Directory.CreateTempSubdirectory("sedv_scanner_tests_");

    public void Dispose() => _root.Delete(recursive: true);

    /// <summary>Creates a file (and any missing folders) under the test's temp folder.</summary>
    private string CreateFile(string relativePath, int sizeBytes = 16)
    {
        var path = Path.Combine(_root.FullName, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, new byte[sizeBytes]);
        
        return path;
    }

    private string Folder(string name) => Path.Combine(_root.FullName, name);

    private static DemoScanner CreateScanner() => new(new FakeHeaderReader());

    /// <summary>
    /// Stands in for the real reader so these tests don't depend on header parsing.
    /// Files beginnign with "bad" fail, everything else gets the same fixed header.
    /// </summary>
    private class FakeHeaderReader : IDemoHeaderReader
    {
        public static readonly DemoHeader Header = new()
        {
            Stamp = "GMODEMO",
            ServerName = "127.0.0.1:27015",
            ClientName = "Player",
            MapName = "gm_construct",
            GameDirectory = "garrysmod",
            PlaybackTime = 60f,
            Ticks = 4000,
        };

        public virtual DemoHeader Read(string path) =>
            Path.GetFileName(path).StartsWith("bad", StringComparison.OrdinalIgnoreCase)
                ? throw new DemoParseException("fake: not a demo")
                : Header;

        public DemoHeader Read(Stream stream) => throw new NotSupportedException("The scanner reads by path.");
    }

    /// <summary>Deletes another file the first time it's asked to read to simulate a file vanishing mid-scan.</summary>
    private sealed class DeletingHeaderReader(string fileToDelete) : FakeHeaderReader
    {
        public override DemoHeader Read(string path)
        {
            File.Delete(fileToDelete);
            return base.Read(path);
        }
    }

    /// <summary>Records every report immediately (unlike Progress which posts them for later).</summary>
    private sealed class RecordingProgress : IProgress<ScanProgress>
    {
        public List<ScanProgress> Reports { get; } = [];
        public void Report(ScanProgress value) => Reports.Add(value);
    }

    [Fact]
    public void Scan_ValidDemo_ReturnsDemoWithHeaderAndFileInfo()
    {
        var path = CreateFile("good.dem", sizeBytes: 123);

        var demos = CreateScanner().Scan([_root.FullName], null, CancellationToken.None);

        var demo = Assert.Single(demos);
        Assert.Equal(path, demo.Path);
        Assert.Equal(123, demo.SizeBytes);
        Assert.Same(FakeHeaderReader.Header, demo.Header);
        Assert.Null(demo.ParseError);
    }

    [Fact]
    public void Scan_BadDemo_IsKeptWithParseError()
    {
        CreateFile("bad.dem");

        var demos = CreateScanner().Scan([_root.FullName], null, CancellationToken.None);

        var demo = Assert.Single(demos);
        Assert.Null(demo.Header);
        Assert.Equal("fake: not a demo", demo.ParseError);
    }

    [Fact]
    public void Scan_NonDemoFile_IsIgnored()
    {
        CreateFile("notes.txt");

        var demos = CreateScanner().Scan([_root.FullName], null, CancellationToken.None);

        Assert.Empty(demos);
    }

    [Fact]
    public void Scan_DemoInSubfolder_IsFound()
    {
        CreateFile(Path.Combine("2021", "may", "deep.dem"));

        var demos = CreateScanner().Scan([_root.FullName], null, CancellationToken.None);

        Assert.Equal("deep.dem", Assert.Single(demos).FileName);
    }

    [Fact]
    public void Scan_UpperCaseExtension_IsFound()
    {
        CreateFile("LOUD.DEM");

        var demos = CreateScanner().Scan([_root.FullName], null, CancellationToken.None);

        Assert.Single(demos);
    }

    [Fact]
    public void Scan_MissingFolder_ReturnsEmptyWithoutThrowing()
    {
        var demos = CreateScanner().Scan([Folder("does-not-exist")], null, CancellationToken.None);

        Assert.Empty(demos);
    }

    [Fact]
    public void Scan_BadFileBeforeGoodFile_GoodFileHasNoParseError()
    {
        // Two folders scanned in the order given so the bad file is always read first
        CreateFile(Path.Combine("a", "bad.dem"));
        CreateFile(Path.Combine("b", "good.dem"));

        var demos = CreateScanner().Scan([Folder("a"), Folder("b")], null, CancellationToken.None);

        var good = Assert.Single(demos, d => d.FileName == "good.dem");
        Assert.NotNull(good.Header);
        Assert.Null(good.ParseError);
    }

    [Fact]
    public void Scan_FileDeletedMidScan_IsSkippedWithoutFailingTheScan()
    {
        // Folder "a" is scanned first. Reading its file deletes the file in "b",
        // which the scanner has already listed but not yet reached.
        CreateFile(Path.Combine("a", "first.dem"));
        var vanishing = CreateFile(Path.Combine("b", "vanishing.dem"));
        var scanner = new DemoScanner(new DeletingHeaderReader(vanishing));
        var progress = new RecordingProgress();

        var demos = scanner.Scan([Folder("a"), Folder("b")], progress, CancellationToken.None);

        Assert.Equal("first.dem", Assert.Single(demos).FileName);
        Assert.Equal(new ScanProgress(2, 2), progress.Reports[^1]); // skipped files still count as processed
    }

    [Fact]
    public void Scan_CancelledToken_ThrowsOperationCanceledException()
    {
        CreateFile("demo.dem");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => CreateScanner().Scan([_root.FullName], null, cts.Token));
    }

    [Fact]
    public void Scan_ThreeDemos_ReportsProgressUpToTotal()
    {
        CreateFile("one.dem");
        CreateFile("two.dem");
        CreateFile("three.dem");
        var progress = new RecordingProgress();

        CreateScanner().Scan([_root.FullName], progress, CancellationToken.None);

        Assert.NotEmpty(progress.Reports);
        Assert.All(progress.Reports, report => Assert.Equal(3, report.Total));
        Assert.Equal(new ScanProgress(3, 3), progress.Reports[^1]);
    }

    [Fact]
    public void Scan_WithRealReader_ReadsRealGmodHeader()
    {
        var fixture = Path.Combine(AppContext.BaseDirectory, "TestData", "gmod_header.dem");
        File.Copy(fixture, Path.Combine(_root.FullName, "recording.dem"));

        var demos = new DemoScanner(new DemoHeaderReader()).Scan([_root.FullName], null, CancellationToken.None);

        var demo = Assert.Single(demos);
        Assert.Null(demo.ParseError);
        Assert.Equal("gm_construct", demo.Header?.MapName);
    }
}
