using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;

namespace PcgsInvUi.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    // TODO New window.
    // TODO Edit window.
    // TODO Delete window.
    // TODO Find window.
    // TODO Export window.
    // TODO Error handling.
    // TODO Manual handling of API key.
    // TODO Use CoinFacts link.
    // TODO Export to CSV.
    // TODO Take giant sheet of coins to integrate into the application.
    // TODO Update all coins.
    // TODO API request tracker (1000 per day)
    // TODO API update schedules (per month? manual override? needs to store a date of last update in the coin object)


    public MainWindow()
    {
        InitializeComponent();
    }
}