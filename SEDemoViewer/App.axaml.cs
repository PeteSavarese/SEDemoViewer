using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SEDemoViewer.Services;
using SEDemoViewer.ViewModels;

namespace SEDemoViewer;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // TODO: replace with real scanner.
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(new FakeDemoScanner()),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}