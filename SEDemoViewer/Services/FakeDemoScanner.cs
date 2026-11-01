using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SEDemoViewer.Core.Models;

namespace SEDemoViewer.Services;

/// <summary>
/// Temp shim for actualy scanner to play around with UI.
/// This doesn't actually scan the selected folder.
/// </summary>
/// <remarks>Delete once <c>IDemoScanner</c> returns <see cref="Demo"/> objects.</remarks>
public sealed class FakeDemoScanner
{
    public IReadOnlyList<Demo> Scan(IEnumerable<string> folders, CancellationToken ct)
    {
        var folder = string.Join(", ", folders);
        Thread.Sleep(600); // pretend to do some disk work so "Scanning…" state text is visible

        return
        [
            Fake(folder, "deathrun.dem", "dr_winterland", "172.106.104.11:27015", "Recorder1", 767, daysAgo: 1),
            Fake(folder, "ttt_minecraft_b5.dem", "ttt_minecraft_b5", "172.106.104.11:27015", "Recorder1", 492, daysAgo: 2),
            Fake(folder, "hacker.dem", "rp_downtown", "74.91.113.2", "Recorder1", 1443, daysAgo: 4),
            Fake(folder, "test.dem", "ttt_map", "172.106.104.11:27015", "67", 931, daysAgo: 5),
            Fake(folder, "propkill.dem", "gm_map", "74.91.113.2:27015", "SixSeven", 199, daysAgo: 9),
            new Demo
            {
                Path = Path.Combine(folder, "broken_recording.dem"),
                SizeBytes = 512,
                CreatedUtc = DateTime.UtcNow.AddDays(-12),
                ModifiedUtc = DateTime.UtcNow.AddDays(-12),
                ParseError = "Malformed header or payload.",
            },
        ];
    }

    private static Demo Fake(string folder, string file, string map, string server, string recorder,
                             float seconds, int daysAgo) => new()
    {
        Path = Path.Combine(folder, file),
        SizeBytes = (long)(seconds * 280_000),
        CreatedUtc = DateTime.UtcNow.AddDays(-daysAgo),
        ModifiedUtc = DateTime.UtcNow.AddDays(-daysAgo),
        Header = new DemoHeader
        {
            Stamp = "GMODEMO",
            DemoProtocol = 3,
            NetworkProtocol = 24,
            ServerName = server,
            ClientName = recorder,
            MapName = map,
            GameDirectory = "garrysmod",
            PlaybackTime = seconds,
            Ticks = (int)(seconds * 66.67f),
            Frames = (int)(seconds * 30),
        },
    };
}
