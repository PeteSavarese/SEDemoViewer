using SEDemoViewer.Core.IO;
using SEDemoViewer.Core.Models;

namespace SEDemoViewer.Core.Services;

public static class DemoHeaderReader
{
    private const string GmodDemoHeader = "GMODEMO";
    
    public static DemoHeader? ReadDemoFile(BinaryReader reader)
    {
        var headerStamp = BinaryReaderExtensions.ReadNullTerminatedString(reader);

        return null;
    }
}