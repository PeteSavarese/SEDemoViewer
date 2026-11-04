namespace SEDemoViewer.Core.Models;


/// <summary>How far a scan has progressed.</summary>
/// <param name="Processed">Amount of files scanned.</param>
/// <param name="Total">Amount of files that are to be scanned.</param>
public record ScanProgress(int Processed, int Total);