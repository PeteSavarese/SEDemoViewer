using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SEDemoViewer.Core.Services;
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
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(new DemoScanner(new DemoHeaderReader())),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}