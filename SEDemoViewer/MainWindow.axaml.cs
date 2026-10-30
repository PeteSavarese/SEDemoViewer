using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace SEDemoViewer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

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

    private async void OpenDemoButton_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open demo file",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Source demos") { Patterns = ["*.dem"] }]
        });

        if (files.Count < 1)
            return;

        // Replaced by the Demoreel library UI, which reads demos through IDemoHeaderReader.
    }
    
    private void ToggleMaximize() => WindowState = WindowState == WindowState.Maximized 
        ? WindowState.Normal 
        : WindowState.Maximized;
}