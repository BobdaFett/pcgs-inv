﻿using System;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using PcgsInvUi.ViewModels;
using ReactiveUI;

namespace PcgsInvUi.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.ShowDeleteWindow.RegisterHandler(ShowDeleteWindowAsync)));
    }

    public async Task ShowDeleteWindowAsync(InteractionContext<DeleteWindowViewModel, Boolean> interaction)
    {
        var window = new DeleteWindow();
        window.DataContext = interaction.Input;

        var result = await window.ShowDialog<bool>(this);
        interaction.SetOutput(result);
    }
}