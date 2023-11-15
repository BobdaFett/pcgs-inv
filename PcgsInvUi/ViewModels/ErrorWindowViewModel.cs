using System;
using System.Reactive;
using ReactiveUI;
using PcgsInvUi.Views;

namespace PcgsInvUi.ViewModels; 

public class ErrorWindowViewModel : ViewModelBase {

    public Exception DisplayedException {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    private Exception _message;

    public ReactiveCommand<ErrorWindow, Unit> CloseCommand { get; }
    
    public ErrorWindowViewModel(string message) {
        CloseCommand = ReactiveCommand.Create<ErrorWindow>(window => { window.Close(); });
    }
}