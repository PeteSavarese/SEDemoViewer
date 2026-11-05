using SEDemoViewer.Core.Models;

namespace SEDemoViewer.Core.Services;

/// <summary>
/// Finds demo files on disk and reads their headers with an <see cref="IDemoHeaderReader"/>.
/// </summary>
/// <param name="reader">
/// Reads each file's header. This is injected so unit tests can supply a fake.
/// </param>
public sealed class DemoScanner(IDemoHeaderReader reader) : IDemoScanner
{
    /// <inheritdoc/>
    public IReadOnlyList<Demo> Scan(IEnumerable<string> folders, IProgress<ScanProgress>? progress, CancellationToken ct)
    {
        var searchOptions = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            MatchCasing = MatchCasing.CaseInsensitive,
        };
        
        var files = folders
            .Where(Directory.Exists)
            .SelectMany(folder => Directory.EnumerateFiles(folder, "*.dem", searchOptions))
            .ToList();

        var demoList = new List<Demo>();
        var processed = 0;

        foreach (var filePath in files)
        {
            ct.ThrowIfCancellationRequested();
            processed++;

            // File may have been deleted or moved since it was listed. Skip it rather than letting
            // FileInfo.Length throw and fail the entire scan
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                progress?.Report(new ScanProgress(processed, files.Count));
                continue;
            }

            string? error = null;

            DemoHeader? header = null;

            try
            {
                header = reader.Read(filePath);
            }
            catch (Exception ex) when (ex is DemoParseException or IOException or UnauthorizedAccessException)
            {
                error = ex.Message;
            }

            demoList.Add(new Demo
            {
                Path = filePath,
                SizeBytes = fileInfo.Length,
                CreatedUtc = fileInfo.CreationTimeUtc,
                ModifiedUtc = fileInfo.LastWriteTimeUtc,
                Hash = null, // TODO: Add later
                Header = header,
                ParseError = error
            });
            
            progress?.Report(new ScanProgress(processed, files.Count));
        }

        return demoList;
    }
}
