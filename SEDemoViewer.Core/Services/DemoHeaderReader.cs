using SEDemoViewer.Core.IO;
using SEDemoViewer.Core.Models;

namespace SEDemoViewer.Core.Services;

/// <summary>Reads the fixed 1072-byte header at the start of a Source Engine demo file.</summary>
public class DemoHeaderReader : IDemoHeaderReader
{
    /// <summary>Total size of the header in bytes.</summary>
    private const int HeaderSize = 1072;
    
    // https://developer.valvesoftware.com/wiki/DEM_(file_format)#File_format
    private const int StampSize = 8;
    private const int StringSize = 260;
    
    // TODO: Make enums later
    private const string GmodDemoHeader = "GMODEMO";
    private const string Hl2DemoHeader = "HL2DEMO";
    
    public DemoHeader Read(string path)
    {
        return Read(File.OpenRead(path));
    }

    public DemoHeader Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        
        using var binaryReader = new BinaryReader(stream);

        try
        {
            var stamp = binaryReader.ReadNullTerminatedString(StampSize);

            if (!stamp.Equals(GmodDemoHeader, StringComparison.InvariantCultureIgnoreCase)
                && !stamp.Equals(Hl2DemoHeader, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new DemoParseException("Invalid demo header");
            }

            return new DemoHeader
            {
                Stamp = stamp,
                DemoProtocol = binaryReader.ReadInt32(),
                NetworkProtocol = binaryReader.ReadInt32(),
                ServerName = binaryReader.ReadNullTerminatedString(StringSize),
                ClientName = binaryReader.ReadNullTerminatedString(StringSize),
                MapName = binaryReader.ReadNullTerminatedString(StringSize),
                GameDirectory = binaryReader.ReadNullTerminatedString(StringSize),
                PlaybackTime = binaryReader.ReadSingle(),
                Ticks = binaryReader.ReadInt32(),
                Frames = binaryReader.ReadInt32(),
                SignonLength = binaryReader.ReadInt32()
            };
        } catch (EndOfStreamException)
        {
            throw new DemoParseException($"File is shorted than demo header ({HeaderSize} bytes).");
        }
    }
}