namespace SEDemoViewer.Core.Models;

/// <summary>
/// Everything the user adds to a demo. Mutable; saved through <c>ILibraryStore</c>.
/// </summary>
public sealed class DemoUserData
{
    /// <summary>
    /// Gets the key of the demo(s) data belongs to; equals <see cref="Demo.UserDataKey"/>.
    /// </summary>
    public required string Key { get; init; }
    
    /// <summary>
    /// Gets or sets the path where demo file was last seen. This is informational only
    /// and should never be used for lookups.
    /// </summary>
    public string? LastPath { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the demo is is marked as a favorite.
    /// </summary>
    public bool IsFavorite { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the demo is queued to watch later.
    /// </summary>
    public bool IsWatchLater { get; set; }

    /// <summary>
    /// Gets or sets free-text notes a user can add to a demo.
    /// </summary>
    public string? Notes { get; set; } = "";

    /// <summary>
    /// Gets or sets tags a user can add to a demo.
    /// </summary>
    public List<string> Tags { get; set; } = [];
}