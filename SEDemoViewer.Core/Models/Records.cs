namespace SEDemoViewer.Core.Models;


/// <summary>How far a scan has progressed.</summary>
/// <param name="Processed">Amount of files scanned.</param>
/// <param name="Total">Amount of files scanned in total.</param>
public record ScanProgress(int Processed, int Total);