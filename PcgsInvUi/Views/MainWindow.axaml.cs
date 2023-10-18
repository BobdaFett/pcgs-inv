using System;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;
using ReactiveUI;

namespace PcgsInvUi.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    // TODO Find window.
    // TODO Export window and export function.
    // TODO Error handling.
    // TODO Manual handling of API key.
    // TODO Use CoinFacts link.
    // TODO Take giant sheet of coins to integrate into the application.
    // TODO Update all coins.
    // TODO API request tracker (1000 per day)
    // TODO API update schedules (per month? manual override? needs to store a date of last update in the coin object)

    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.ShowDeleteWindow.RegisterHandler(ShowDeleteWindowAsync)));
    }

    public async Task ShowDeleteWindowAsync(InteractionContext<DeleteWindowViewModel, Boolean> interaction)
    {
        var window = new DeleteWindow();
        window.DataContext = interaction.Input;

        var result = await window.ShowDialog<bool>(this);
        interaction.SetOutput(result);
    }
}