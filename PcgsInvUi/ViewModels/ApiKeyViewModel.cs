using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PcgsInvUi.ViewModels;

public partial class ApiKeyViewModel : ViewModelBase {
    [ObservableProperty]
    public partial string ApiKeyInput { get; set; }

    [ObservableProperty]
    public partial bool OkEnabled { get; set; }

    [RelayCommand]
    public void OkCommand() {
        Debug.WriteLine("Running OkCommand from ApiKeyViewModel");
    }
}