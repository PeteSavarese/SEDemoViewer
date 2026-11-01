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

        var result = reader.ReadNullTerminatedString(field.Length);
        Assert.Equal(text, result);
    }
    
    [Fact]
    public void ReadNullTerminatedString_JunkAfterNull_ReturnsTextOnly()
    {
        const string text = "de_dust";
        var field = Encoding.ASCII.GetBytes($"{text}\0junk_stuff");;
        
        using var reader = new BinaryReader(new MemoryStream(field));

        var result = reader.ReadNullTerminatedString(field.Length);
        Assert.Equal(text, result);
    }
    
    [Fact]
    public void ReadNullTerminatedString_TextFillsField_ReturnsAllText()
    {
        const string text = "de_cobblestone";
        var field = Encoding.ASCII.GetBytes($"{text}");;
        
        using var reader = new BinaryReader(new MemoryStream(field));

        var result = reader.ReadNullTerminatedString(field.Length);
        Assert.Equal(text, result);
    }
    [Fact]
    public void ReadNullTerminatedString_NullFirstByte_ReturnsEmptyString()
    {
        var field = Encoding.ASCII.GetBytes("\0junk_stuff");;
        
        using var reader = new BinaryReader(new MemoryStream(field));

        var result = reader.ReadNullTerminatedString(field.Length);
        Assert.Equal("", result);
    }
    
    /// <summary>
    /// Assert that each call to <see cref="BinaryReaderExtensions.ReadNullTerminatedString"/> consume exactly <c>size</c> bytes.
    /// </summary>
    [Fact]
    public void ReadNullTerminatedString_TwoFieldsInARow_ReadsEachFieldFromItsBoundary()
    {
        // Two 16-byte fields back to back like ServerName followed by ClientName in a header.
        const int fieldSize = 16;
        byte[] data = [.. FixedLengthField("Surf Server 2", fieldSize), .. FixedLengthField("ClientUserName", fieldSize)];
        using var reader = new BinaryReader(new MemoryStream(data));

        var firstField = reader.ReadNullTerminatedString(fieldSize);
        var positionAfterFirst = reader.BaseStream.Position;
        var secondField = reader.ReadNullTerminatedString(fieldSize);

        Assert.Equal("Surf Server 2", firstField);
        Assert.Equal(fieldSize, positionAfterFirst);
        Assert.Equal("ClientUserName", secondField);
    }
    
    [Theory]
    [InlineData("SODA 🥤")]
    [InlineData("単体テスト Code")]
    [InlineData("♦Username♦")]
    public void ReadNullTerminatedString_NonAsciiFields_ReturnsValidUtf8String(string text)
    {
        var field = Encoding.UTF8.GetBytes(text);
        using var reader = new BinaryReader(new MemoryStream(field));

        var result = reader.ReadNullTerminatedString(field.Length);
        Assert.Equal(text, result);
    }
    
    [Fact]
    public void ReadNullTerminatedString_FewerBytesThanSize_ThrowsEndOfStreamException()
    {
        const int fieldSize = 16;
        var data = FixedLengthField("de_dust", 10);
        using var reader = new BinaryReader(new MemoryStream(data));

        Assert.Throws<EndOfStreamException>(() => reader.ReadNullTerminatedString(fieldSize));
    }
    
    [Fact]
    public void ReadNullTerminatedString_FieldSizeIsZero_ThrowsArgumentOutOfRangeException()
    {
        var data = FixedLengthField("de_dust", 10);
        using var reader = new BinaryReader(new MemoryStream(data));

        Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadNullTerminatedString(0));
    }
    
    [Fact]
    public void ReadNullTerminatedString_NullReader_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => BinaryReaderExtensions.ReadNullTerminatedString(null!, 16));
    }
    
    /// <summary>Builds fixed-length field <paramref name="text"/> as UTF-8 padded with zero bytes.</summary>
    private static byte[] FixedLengthField(string text, int size)
    {
        var field = new byte[size];
        Encoding.UTF8.GetBytes(text).CopyTo(field, 0);
        
        return field;
    }
}