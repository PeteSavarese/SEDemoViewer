using SEDemoViewer.Core.Models;

namespace SEDemoViewer.Core.Services;

public interface IDemoScanner
{
    IReadOnlyList<Demo> Scan(IEnumerable<string> folders, IProgress<ScanProgress>? progress, CancellationToken ct);
}