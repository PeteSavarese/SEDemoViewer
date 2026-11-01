using System;
using SEDemoViewer.Core.Models;

namespace SEDemoViewer.ViewModels;

/// <summary>One row in demo list. A <see cref="Demo"/> formatted for display.</summary>
public sealed class DemoItemViewModel(Demo demo)
{
    private const string Missing = "N/A"; // TODO: Placeholder

    public Demo Demo { get; } = demo;

    public string Name => Demo.FileName;
    public string Map => Demo.Header?.MapName ?? Missing;
    public string Server => Demo.Header?.ServerName ?? Missing;
    public string Recorder => Demo.Header?.ClientName ?? Missing;
    public string Length => Demo.Header is { } header ? FormatLength(header.Duration) : Missing;
    public string Recorded => Demo.RecordedUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm");

    /// <summary>Why the demo file header couldn't be read, or <see langword="null"/> if header was read.</summary>
    public string? Error => Demo.ParseError;

    private static string FormatLength(TimeSpan length) => length.TotalHours >= 1
        ? length.ToString(@"h\:mm\:ss")
        : length.ToString(@"mm\:ss");
}
