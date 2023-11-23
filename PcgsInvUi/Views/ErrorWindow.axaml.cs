using System;
using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;
using ReactiveUI;

namespace PcgsInvUi.Views; 

public partial class ErrorWindow : ReactiveWindow<ErrorWindowViewModel> {
    public ErrorWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.CloseCommand.Subscribe(Close)));
    }
}
