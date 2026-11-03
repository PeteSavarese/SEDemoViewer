using SEDemoViewer.Core.Models;

namespace SEDemoViewer.Core.Services;

/// <summary>Finds <c>.dem</c> files in an enumerable of folders and turns each into a <see cref="Demo"/>.</summary>
public interface IDemoScanner
{
    /// <summary>
    /// Finds every <c>.dem</c> file in <paramref name="folders"/> recursively and
    /// returns one <see cref="Demo"/> per file.
    /// </summary>
    /// <param name="folders">Folders to search.</param>
    /// <param name="progress">Receives progress after each file, or <see langword="null"/> to not report progress.</param>
    /// <param name="ct">Cancels the scan between files.</param>
    /// <returns>
    /// One <see cref="Demo"/> per file found.
    /// </returns>
    /// <remarks>
    /// A single unreadable or broken file never fails the whole scan. A file whose header couldn't be read is still
    /// included, with <see cref="Demo.Header"/> set to <see langword="null"/> and
    /// <see cref="Demo.ParseError"/> explaining why.
    /// </remarks>
    /// <exception cref="OperationCanceledException"><paramref name="ct"/> was cancelled.</exception>
    IReadOnlyList<Demo> Scan(IEnumerable<string> folders, IProgress<ScanProgress>? progress, CancellationToken ct);
}