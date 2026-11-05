using System.Text;
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
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        
        return Read(stream);
    }

    public DemoHeader Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        
        // Using Latin-1 only affects BinaryReader's char methods (ReadChar, ReadChars, and ReadString) which we
        // don't use since header strings are read as raw bytes and decoded as UTF-8 by ReadNullTerminatedString.
        // Latin-1 is chosen because it maps exactly 1 byte to 1 char. Just in case in the future, for example, a
        // ReadChars(260) will always consume 260 bytes, whereas UTF-8 could consume more and shift every subsequent read.
        //
        // leaveOpen is also set to true since the stream belongs to the caller so we don't want the disposal of BinaryReader
        // to also close the stream.
        using var binaryReader = new BinaryReader(stream, Encoding.Latin1, true);

        try
        {
            var stamp = binaryReader.ReadNullTerminatedString(StampSize);

            if (!stamp.Equals(GmodDemoHeader, StringComparison.InvariantCultureIgnoreCase)
                && !stamp.Equals(Hl2DemoHeader, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new DemoParseException($"Demo header '{stamp}' is invalid.");
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
        } catch (EndOfStreamException ex)
        {
            throw new DemoParseException($"File is shorter than demo header ({HeaderSize} bytes).", ex);
        }
    }
}