namespace SEDemoViewer.Core.Models;

/// <summary>
/// Represents the fixed 1072-byte header at the start of every
/// Source/GMod demo.
/// </summary>
public sealed class DemoHeader
{
    /// <summary>
    /// <c>GMODDEMO</c> or <c>HL2DEMO</c>
    /// </summary>
    public required string Stamp { get; init; }
    
    /// <summary>
    /// Gets Source Engine demo protocol version
    /// </summary>
    public int DemoProtocol { get; init; }
    
    /// <summary>
    /// Network protocol version number
    /// </summary>
    public int NetworkProtocol { get; init; }
    
    public required string ServerName { get; init; }
    
    public required string ClientName { get; init; }
    
    public required string MapName { get; init; }
    
    public required string GameDirectory { get; init; }
    
    /// <summary>
    /// Length of demo in seocnds.
    /// </summary>
    public float PlaybackTime { get; init; } 
    
    /// <summary>
    /// Gets total amount of ticks captured in demo.
    /// </summary>
    public int Ticks { get; init; }
    
    /// <summary>
    /// Gets total amount of frames captured in demo.
    /// </summary>
    public int Frames { get; init; }
    
    /// <summary>
    /// Length of signon data (init for first frame).
    /// </summary>
    public int SignonLength { get; init; }
    

    public double TickRate => PlaybackTime > 0 ? Ticks / PlaybackTime : 0;
    public TimeSpan Duration => TimeSpan.FromSeconds(PlaybackTime);
}