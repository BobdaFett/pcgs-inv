using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;
using ReactiveUI;
using System;

namespace PcgsInvUi.Views;

public partial class FindWindow : ReactiveWindow<FindWindowViewModel>
{
    public FindWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.OkCommand.Subscribe(Close)));
    }
}