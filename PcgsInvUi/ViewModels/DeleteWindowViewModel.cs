using System.Reactive;
using PcgsInvUi.Views;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class DeleteWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, object> OkCommand { get; } = ReactiveCommand.Create(() => (object)true);
    public ReactiveCommand<DeleteWindow, Unit> CancelCommand { get; }

    public DeleteWindowViewModel()
    {
        CancelCommand = ReactiveCommand.Create<DeleteWindow>(window => window.Close());
    }
}