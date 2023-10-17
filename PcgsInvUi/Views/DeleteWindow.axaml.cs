using PcgsInvUi.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;


namespace PcgsInvUi.Views;

public partial class DeleteWindow : ReactiveWindow<DeleteWindowViewModel>
{
    public DeleteWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.OkCommand.Subscribe(Close)));
    }
}