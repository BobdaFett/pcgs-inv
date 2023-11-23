using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;

namespace PcgsInvUi.Views; 

public partial class ErrorWindow : ReactiveWindow<ErrorWindowViewModel> {
    public ErrorWindow()
    {
        InitializeComponent();
    }
}