using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;
using ReactiveUI;

namespace PcgsInvUi.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel> {
    public MainWindow() {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.ShowDeleteWindow.RegisterHandler(ShowDeleteWindowAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowExportWindow.RegisterHandler(ShowFilePickerAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowErrorWindow.RegisterHandler(ShowErrorWindowAsync)));
        // this.WhenActivated(d => d(ViewModel!.ShowFindWindow.RegisterHandler(ShowFindWindowAsync)));
    }

    private async Task ShowDeleteWindowAsync(InteractionContext<DeleteWindowViewModel, bool> interaction) {
        var window = new DeleteWindow();
        window.DataContext = interaction.Input;

        var result = await window.ShowDialog<bool>(this);
        interaction.SetOutput(result);
    }

    private async Task ShowFindWindowAsync(InteractionContext<FindWindowViewModel, int> interaction) {
        var window = new FindWindow();
        window.DataContext = interaction.Input;

        var result = await window.ShowDialog<int>(this);
        interaction.SetOutput(result);
    }

    private async Task ShowFilePickerAsync(InteractionContext<Unit, Uri> interationContext) {
        var topLevel = GetTopLevel(this);
        if (topLevel is null) throw new Exception("Could not find top level of the file path.");
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
            Title = "Save CSV...",
            DefaultExtension = ".csv",
            ShowOverwritePrompt = true,
            SuggestedFileName = "coins.csv",
            SuggestedStartLocation = await StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents)
        });

        if (file is not null) {
            interationContext.SetOutput(file.Path);
        }
        else {
            // TODO Handle user closing the dialog.
            throw new Exception("User cancelled the file picker.");
        }
    }
    
    private async Task ShowErrorWindowAsync(InteractionContext<ErrorWindowViewModel, bool> interaction) {
        var window = new ErrorWindow();
        window.DataContext = interaction.Input;

        var result = await window.ShowDialog<bool>(this);
        interaction.SetOutput(result);
    }
}
