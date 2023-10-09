using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PcgsInvUi.ViewModels;
using PcgsInvUi.Views;

namespace PcgsInvUi;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var coins = new Services.CoinCollection();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(coins),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}