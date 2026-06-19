using System;
using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;
using ReactiveUI;

namespace PcgsInvUi.Views;

public partial class FindWindow : ReactiveWindow<FindWindowViewModel> {
    public FindWindow() {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.OkCommand.Subscribe(Close)));
    }
}