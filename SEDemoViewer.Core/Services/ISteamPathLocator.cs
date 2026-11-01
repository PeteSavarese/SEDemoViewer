namespace SEDemoViewer.Core.Services;

/// <summary>Finds Garry's Mod demo folders in user's Steam libraries.</summary>
public interface ISteamPathLocator
{
    /// <summary>
    /// Gets every existing <c>garrysmod/demos</c> folder across all Steam libraries.
    /// </summary>
    IReadOnlyList<string> FindGmodDemoFolders();
}