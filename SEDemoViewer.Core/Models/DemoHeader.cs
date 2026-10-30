namespace SEDemoViewer.Core.Models;

/// <summary>
/// Represents the fixed 1072-byte header at the start of every
/// Source Engine file header.
/// </summary>
public sealed record DemoHeader
{
    /// <summary>
    /// Gets text identifying the file format of Source games demo
    /// like <c>GMODDEMO</c> or <c>HL2DEMO</c>.
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
    
    /// <summary>
    /// Gets server that recorded demo.
    /// </summary>
    public required string ServerName { get; init; }
    
    /// <summary>
    /// Gets client that recorded demo.
    /// </summary>
    public required string ClientName { get; init; }
    
    /// <summary>
    /// Gets map name that demo was recorded on.
    /// </summary>
    public required string MapName { get; init; }
    
    public required string GameDirectory { get; init; }
    
    /// <summary>
    /// Length of demo in seconds.
    /// </summary>
    public float PlaybackTime { get; init; } 
    
    /// <summary>
    /// Gets total number of ticks captured in demo.
    /// </summary>
    public int Ticks { get; init; }
    
    /// <summary>
    /// Gets total number of frames captured in demo.
    /// </summary>
    public int Frames { get; init; }
    
    /// <summary>
    /// Length of signon data (init for first frame).
    /// </summary>
    public int SignonLength { get; init; }
    
    /// <summary>
    /// Gets the server tick rate in ticks per second, or 0 when
    /// <see cref="PlaybackTime"/> is 0.
    /// </summary>
    public double TickRate => PlaybackTime > 0 ? Ticks / PlaybackTime : 0;
    
    /// <summary>Gets <see cref="PlaybackTime"/> as a <see cref="TimeSpan"/>.</summary>
    public TimeSpan Duration => TimeSpan.FromSeconds(PlaybackTime);
}