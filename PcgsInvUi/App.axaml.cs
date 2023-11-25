using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PcgsInvUi.Services;
using PcgsInvUi.ViewModels;
using PcgsInvUi.Views;

namespace PcgsInvUi;

public partial class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        base.OnFrameworkInitializationCompleted();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var coins = new CoinDatabase();
            desktop.MainWindow = new MainWindow {
                DataContext = new MainWindowViewModel(),
            };
            
            // Allow the application to save the database when it exits.
            // The issue is that this will break if the database is not initialized here.
            desktop.Exit += (sender, args) => {
                coins.UpdateCollection("CollectionTable");
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
}
