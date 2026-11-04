using System.Text;
using SEDemoViewer.Core.Services;

namespace SEDemoViewer.Core.Tests.Services;

public class DemoHeaderReaderTests
{
    private const int StampLength = 8;
    private const int MaxPathLength = 260;

    /// <summary>
    /// Builds a 1072-byte demo header with defaults. Caller can 
    /// set what values they specify manually for its given test.
    /// </summary>
    private static byte[] FakeHeader(
        string stamp = "GMODEMO",
        int demoProtocol = 3,
        int networkProtocol = 24,
        string serverName = "127.0.0.1:27015",
        string clientName = "Player",
        string mapName = "gm_construct",
        string gameDirectory = "garrysmod",
        float playbackTime = 60f,
        int ticks = 4000,
        int frames = 1800,
        int signonLength = 200_000)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(Field(stamp, StampLength));
            writer.Write(demoProtocol);
            writer.Write(networkProtocol);
            writer.Write(Field(serverName, MaxPathLength));
            writer.Write(Field(clientName, MaxPathLength));
            writer.Write(Field(mapName, MaxPathLength));
            writer.Write(Field(gameDirectory, MaxPathLength));
            writer.Write(playbackTime);
            writer.Write(ticks);
            writer.Write(frames);
            writer.Write(signonLength);
        }

        return stream.ToArray();
    }
    
    /// <summary>Encodes <paramref name="text"/> as UTF-8 into a zero-padded field of <paramref name="size"/> bytes.</summary>
    private static byte[] Field(string text, int size)
    {
        var field = new byte[size];
        Encoding.UTF8.GetBytes(text).CopyTo(field, 0);
        return field;
    }
    
    [Fact]
    public void Read_ValidHeader_ReturnsEveryField()
    {
        var bytes = FakeHeader(serverName: "1.2.3.4:27015", clientName: "Username", mapName: "gm_construct",
                               playbackTime: 767.5f, ticks: 51_167, frames: 23_025, signonLength: 231_527);

        var header = new DemoHeaderReader().Read(new MemoryStream(bytes));

        Assert.Equal("GMODEMO", header.Stamp);
        Assert.Equal(3, header.DemoProtocol);
        Assert.Equal(24, header.NetworkProtocol);
        Assert.Equal("1.2.3.4:27015", header.ServerName);
        Assert.Equal("Username", header.ClientName);
        Assert.Equal("gm_construct", header.MapName);
        Assert.Equal("garrysmod", header.GameDirectory);
        Assert.Equal(767.5f, header.PlaybackTime);
        Assert.Equal(51_167, header.Ticks);
        Assert.Equal(23_025, header.Frames);
        Assert.Equal(231_527, header.SignonLength);
    }

    [Fact]
    public void Read_Hl2DemoStamp_IsAccepted()
    {
        var header = new DemoHeaderReader().Read(new MemoryStream(FakeHeader(stamp: "HL2DEMO")));

        Assert.Equal("HL2DEMO", header.Stamp);
    }

    [Fact]
    public void Read_UnknownStamp_ThrowsDemoParseExceptionNamingTheStamp()
    {
        var bytes = FakeHeader(stamp: "PKZIP");

        var ex = Assert.Throws<DemoParseException>(() => new DemoHeaderReader().Read(new MemoryStream(bytes)));

        Assert.Contains("PKZIP", ex.Message);
    }

    [Fact]
    public void Read_StreamShorterThanHeader_ThrowsDemoParseExceptionWithEndOfStreamCause()
    {
        var truncated = FakeHeader()[..500];

        var ex = Assert.Throws<DemoParseException>(() => new DemoHeaderReader().Read(new MemoryStream(truncated)));

        Assert.IsType<EndOfStreamException>(ex.InnerException);
    }

    [Fact]
    public void Read_JunkAfterNullInMapName_ReturnsCleanMapName()
    {
        var bytes = FakeHeader(mapName: "ttt_bank\0uct_big");

        var header = new DemoHeaderReader().Read(new MemoryStream(bytes));

        Assert.Equal("ttt_bank", header.MapName);
    }

    [Fact]
    public void Read_Stream_LeavesCallersStreamOpen()
    {
        var stream = new MemoryStream(FakeHeader());

        new DemoHeaderReader().Read(stream);

        Assert.True(stream.CanRead);
    }

    [Fact]
    public void Read_Path_ReadsHeaderFromFile()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(path, FakeHeader(mapName: "gm_flatgrass"));

            var header = new DemoHeaderReader().Read(path);

            Assert.Equal("gm_flatgrass", header.MapName);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Read_RealGmodHeader_ReturnsKnownValues()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TestData", "gmod_header.dem");

        var header = new DemoHeaderReader().Read(path);

        Assert.Equal("GMODEMO", header.Stamp);
        Assert.Equal(3, header.DemoProtocol);
        Assert.Equal(24, header.NetworkProtocol);
        Assert.Equal("123.456.7.890:27015", header.ServerName);
        Assert.Equal("Username", header.ClientName);
        Assert.Equal("gm_construct", header.MapName);
        Assert.Equal("garrysmod", header.GameDirectory);
        Assert.Equal(1.035, header.PlaybackTime, 0.001);
        Assert.Equal(69, header.Ticks);
        Assert.Equal(103, header.Frames);
        Assert.Equal(162_744, header.SignonLength);
    }
}