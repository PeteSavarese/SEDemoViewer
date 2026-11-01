using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SEDemoViewer.Services;

namespace SEDemoViewer.ViewModels;

public sealed partial class MainWindowViewModel(FakeDemoScanner scanner) : ObservableObject
{
    public ObservableCollection<DemoItemViewModel> Demos { get; } = [];

    [ObservableProperty]
    public partial string? FolderPath { get; set; }

    [ObservableProperty]
    public partial string StatusText { get; set; } = "Choose \"garrysmod/demos\" folder to get started.";

    [ObservableProperty]
    public partial bool IsScanning { get; set; }

    /// <summary>Scans <paramref name="folder"/> and updates list with what was found.</summary>
    public async Task LoadFolderAsync(string folder)
    {
        FolderPath = folder;
        IsScanning = true;
        StatusText = "Scanning…";

        try
        {
            // Keep IO off the UI thread so window doesnt freeze
            var demos = await Task.Run(() => scanner.Scan([folder], CancellationToken.None));

            Demos.Clear();
            foreach (var demo in demos.OrderByDescending(d => d.RecordedUtc))
                Demos.Add(new DemoItemViewModel(demo));

            StatusText = demos.Count == 0 
                ? "No .dem files found in folder." 
                : $"{demos.Count} demos";
        }
        catch (Exception ex)
        {
            StatusText = $"Scan failed: {ex.Message}";
        }
        finally
        {
            IsScanning = false;
        }
    }
}
