using SEDemoViewer.Core.IO;
using SEDemoViewer.Core.Models;

namespace SEDemoViewer.Core.Services;

public class DemoHeaderReader : IDemoHeaderReader
{
    private const string GmodDemoHeader = "GMODEMO";
    
    public DemoHeader Read(string path)
    {
        throw new NotImplementedException();
    }

    public DemoHeader Read(Stream stream)
    {
        throw new NotImplementedException();
    }
}