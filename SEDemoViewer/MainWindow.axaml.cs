using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SEDemoViewer.ViewModels;

namespace SEDemoViewer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;

    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }

    #region Menu Bar buttons
    private void TitleBar_DoubleTapped(object? sender, RoutedEventArgs e) => ToggleMaximize();

    private void MinimizeButton_Click(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void MaximizeButton_Click(object? sender, RoutedEventArgs e) => ToggleMaximize();

    private void CloseButton_Click(object? sender, RoutedEventArgs e) => Close();
    #endregion

    private async void ChooseFolderButton_Click(object? sender, RoutedEventArgs e)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open Garry's Mod folder",
            AllowMultiple = false,
        });

        if (folders.Count < 1 || folders[0].TryGetLocalPath() is not { } path)
            return;

        await ViewModel.LoadFolderAsync(path);
    }

    private void ToggleMaximize() => WindowState = WindowState == WindowState.Maximized
        ? WindowState.Normal
        : WindowState.Maximized;
}
