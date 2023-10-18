using System.Reactive;
using PcgsInvUi.Views;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class DeleteWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, object> OkCommand { get; }
    public ReactiveCommand<DeleteWindow, Unit> CancelCommand { get; }

    public DeleteWindowViewModel()
    {
        OkCommand  = ReactiveCommand.Create(() => (object)true);
        CancelCommand = ReactiveCommand.Create<DeleteWindow>(window => window.Close());
    }
}