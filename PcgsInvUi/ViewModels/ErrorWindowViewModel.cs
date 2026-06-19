using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PcgsInvUi.ViewModels;

public partial class ErrorWindowViewModel : ViewModelBase {
    private readonly Window _dialog;

    public ErrorWindowViewModel(Window dialog, string message) {
        _dialog = dialog;
        Message = message;
    }

    [ObservableProperty]
    public partial string Message { get; set; }

    [RelayCommand]
    public void Close() {
        _dialog.Close();
    }
}