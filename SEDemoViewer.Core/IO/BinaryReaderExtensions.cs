using System.Text;

namespace SEDemoViewer.Core.IO;

/// <summary>
/// Helper methods for reading null-terminated strings from a <see cref="BinaryReader"/>.
/// </summary>
public class BinaryReaderExtensions
{
    private const char NullTerminate = '\0';

    /// <summary>
    /// Reads characters from <paramref name="reader"/> until a null terminator ('\0')
    /// is encountered, returning the accumulated string. The terminator is consumed
    /// but not included in the result.
    /// </summary>
    /// <param name="reader">Reader to read characters from.</param>
    /// <returns>String read up to, but excluding, the null terminator.</returns>
    /// <exception cref="EndOfStreamException">
    /// The end of the stream is reached before a null terminator is found.
    /// </exception>
    public static string ReadNullTerminatedString(BinaryReader reader)
    {
        var stringBuilder = new StringBuilder();
        char currentChar;

        while ((currentChar = reader.ReadChar()) != NullTerminate)
        {
            stringBuilder.Append(currentChar);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Reads exactly <paramref name="size"/> characters from <paramref name="reader"/>,
    /// returning them as a string with any null terminators ('\0') removed. All
    /// <paramref name="size"/> characters are consumed regardless of their content.
    /// </summary>
    /// <param name="reader">The reader to read characters from.</param>
    /// <param name="size">The number of characters to read. Must be greater than zero.</param>
    /// <returns>The characters read, excluding any null terminators.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="reader"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is less than 1.</exception>
    /// <exception cref="EndOfStreamException">
    /// The end of the stream is reached before <paramref name="size"/> characters are read.
    /// </exception>
    public static string ReadSizedNullString(BinaryReader reader, int size)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < size; i++)
        {
            var currentChar = reader.ReadChar();
            if (currentChar != NullTerminate)
                stringBuilder.Append(currentChar);
        }

        return stringBuilder.ToString();
    }
}