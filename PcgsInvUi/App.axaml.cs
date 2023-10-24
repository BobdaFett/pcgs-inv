using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PcgsInvUi.Services;
using PcgsInvUi.ViewModels;
using PcgsInvUi.Views;
using PcgsInvUi.Models;

namespace PcgsInvUi;

public partial class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        base.OnFrameworkInitializationCompleted();
        
        // Create a connection to SQLite
        var database = new Database();
        var coin = database.GetCoin(1003, "AG-3");
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var coins = new Services.CoinCollection();

            desktop.MainWindow = new MainWindow {
                DataContext = new MainWindowViewModel(coins),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}