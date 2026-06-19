using System;
using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;
using ReactiveUI;

namespace PcgsInvUi.Views;

public partial class DeleteWindow : ReactiveWindow<DeleteWindowViewModel> {
    public DeleteWindow() {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.OkCommand.Subscribe(Close)));
        this.WhenActivated(d => d(ViewModel!.CancelCommand.Subscribe(Close)));
    }
}