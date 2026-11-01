using System.Text;
using SEDemoViewer.Core.IO;

namespace SEDemoViewer.Core.Tests.IO;

public class BinaryReaderExtensionsTests
{
    [Fact]
    public void ReadNullTerminatedString_TextShorterThanField_ReturnsTextOnly()
    {
        const string text = "test";
        var field = new byte[16];
        Encoding.UTF8.GetBytes(text).CopyTo(field, 0);
        using var reader = new BinaryReader(new MemoryStream(field));

        var result = reader.ReadNullTerminatedString(16);
        Assert.Equal(text, result);
    }
}