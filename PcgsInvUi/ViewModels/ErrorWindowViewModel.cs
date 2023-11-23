using System.Reactive;
using System.Security.Principal;
using PcgsInvUi.Views;
using ReactiveUI;

namespace PcgsInvUi.ViewModels; 

public class ErrorWindowViewModel : ViewModelBase {

    public string DisplayedMessage {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    private string _message;

    public ReactiveCommand<ErrorWindow, Unit> CloseCommand { get; }
    
    public ErrorWindowViewModel(string message)
    {
        _message = message;

        CloseCommand = ReactiveCommand.Create<ErrorWindow>(window => window.Close());
    }

}
