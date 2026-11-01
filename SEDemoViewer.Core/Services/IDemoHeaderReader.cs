using SEDemoViewer.Core.Models;

namespace SEDemoViewer.Core.Services;

/// <summary>Reads the fixed header at the start of a demo file.</summary>
public interface IDemoHeaderReader
{
    /// <summary>Reads the header of the demo at <paramref name="path"/>.</summary>
    /// <exception cref="DemoParseException">File is not a valid demo.</exception>
    DemoHeader Read(string path);

    /// <summary>Reads a header from the current position of <paramref name="stream"/>.</summary>
    /// <exception cref="DemoParseException">Data is not a valid demo header.</exception>
    DemoHeader Read(Stream stream);
}