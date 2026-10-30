namespace SEDemoViewer.Core.Models;

/// <summary>
/// Represents a demo file (<c>.dem</c>) file on disk.
/// </summary>
/// <remarks>
/// All properties are set when the demo is scanned. If file contents
/// or metadata change, the demo must be rescanned.
/// </remarks>
public sealed record Demo
{
    /// <summary>
    /// Gets path to demo file.
    /// </summary>
    public required string Path { get; init; }
    
    public string FileName => System.IO.Path.GetFileName(Path);
    
    /// <summary>
    /// Gets size of demo file in bytes.
    /// </summary>
    public long SizeBytes { get; init; }
    
    /// <summary>
    /// Gets creation time of demo file in UTC.
    /// </summary>
    public DateTime CreatedUtc { get; init; }
    
    /// <summary>
    /// Gets last write time of demo file in UTC.
    /// </summary>
    public DateTime ModifiedUtc { get; init; }
    
    /// <summary>
    /// Gets file hash.
    /// </summary>
    public string? Hash { get; init; }
    
    /// <summary>
    /// Gets demo header, or <see langword="null"/> if header could not be parsed.
    /// </summary>
    public DemoHeader? Header { get; init; }
    
    /// <summary>
    /// Gets parse error message, or <see langword="null"/> if demo was parsed successfully.
    /// </summary>
    public string? ParseError { get; init; }
    
    /// <summary>
    /// Gets time when demo was recorded in UTC.
    /// </summary>
    public DateTime RecordedUtc => CreatedUtc < ModifiedUtc 
        ? CreatedUtc 
        : ModifiedUtc;
    
    /// <summary>
    /// Gets the key that links this demo to its <see cref="DemoUserData"/>
    /// (favourite, watch later, tags, and notes).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the content <see cref="Hash"/> when available, so user data follows the file
    /// through renames and moves. If the file couldn't be hashed, it falls back to
    /// <c>"path:"</c> followed by <see cref="Path"/> which does not follow after renames.
    /// </para>
    /// <para>
    /// This is <b>not</b> a unique identifier for the demo. Identical copies share the same key
    /// (and hence same user data) and the key changes once when GMod finishes writing a
    /// recording. Use <see cref="Path"/> to identify a specific file.
    /// </para>
    /// </remarks>
    public string UserDataKey => Hash ?? $"path:{Path}";
}