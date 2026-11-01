using System.Text;

namespace SEDemoViewer.Core.IO;

/// <summary>
/// Helper methods for reading null-terminated strings from a <see cref="BinaryReader"/>.
/// </summary>
public static class BinaryReaderExtensions
{
    /// <summary>
    /// Reads a fixed-length, null-padded string field. Always consumes exactly
    /// <paramref name="size"/> bytes and returns the UTF-8 text before first null byte.
    /// </summary>
    /// <exception cref="EndOfStreamException">Fewer than <paramref name="size"/> bytes remain.</exception>
    public static string ReadNullTerminatedString(this BinaryReader reader, int size)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

        var bytes = reader.ReadBytes(size);
        if (bytes.Length < size)
            throw new EndOfStreamException();

        var endPosition = Array.IndexOf(bytes, (byte)0);
        
        return Encoding.UTF8.GetString(bytes, 0, endPosition < 0 ? size : endPosition);
    }
}