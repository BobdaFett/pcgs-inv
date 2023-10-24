using System;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;
using ReactiveUI;

namespace PcgsInvUi.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel> {
    public MainWindow() {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.ShowDeleteWindow.RegisterHandler(ShowDeleteWindowAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowFindWindow.RegisterHandler(ShowFindWindowAsync)));
    }

    public async Task ShowDeleteWindowAsync(InteractionContext<DeleteWindowViewModel, Boolean> interaction) {
        var window = new DeleteWindow();
        window.DataContext = interaction.Input;

        var result = await window.ShowDialog<bool>(this);
        interaction.SetOutput(result);
    }

    public async Task ShowFindWindowAsync(InteractionContext<FindWindowViewModel, int> interaction) {
        var window = new FindWindow();
        window.DataContext = interaction.Input;

        var result = await window.ShowDialog<int>(this);
        interaction.SetOutput(result);
    }
}