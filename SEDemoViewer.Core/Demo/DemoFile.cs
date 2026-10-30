using System.IO;
using System.Text;
using SEDemoViewer.Core.Models;
using SEDemoViewer.Core.Services;

namespace SEDemoViewer.Core;

public sealed class DemoFile
{
    public required DemoHeader? Header{ get; set; }

    public static DemoFile FromStream(Stream stream)
    {
        using var binaryReader = new BinaryReader(stream, Encoding.Latin1, leaveOpen: true);
        return new DemoFile { Header = DemoHeaderReader.ReadDemoFile(binaryReader) };

    }
}