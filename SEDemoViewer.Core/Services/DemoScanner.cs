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
        
        foreach (var filePath in files)
        {
            ct.ThrowIfCancellationRequested();
            
            var fileInfo = new FileInfo(filePath);
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
        }

        return demoList;
    }
}
