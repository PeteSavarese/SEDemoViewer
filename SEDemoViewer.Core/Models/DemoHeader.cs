namespace SEDemoViewer.Core.Models;

public class DemoHeader
{
    public string Header { get; set; }
    public int DemoProtocol { get; set; }
    public int NetworkProtocol { get; set; }
    public string ServerName { get; set; }
    public string ClientName { get; set; }
    public string MapName { get; set; }
    public string GameDirectory { get; set; }
    public float PlaybackTime { get; set; }
    public int Ticks { get; set; }
    public int Frames { get; set; }
    public int SignonDataLength { get; set; }
    public int TickRate => (int)Math.Floor(Ticks / PlaybackTime);
}