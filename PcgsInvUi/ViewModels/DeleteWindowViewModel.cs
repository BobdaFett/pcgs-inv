using System.Reactive;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class DeleteWindowViewModel : ViewModelBase {
    public DeleteWindowViewModel() {
        OkCommand = ReactiveCommand.Create(() => (object)true);
        CancelCommand = ReactiveCommand.Create(() => (object)false);
    }

    public ReactiveCommand<Unit, object> OkCommand { get; }
    public ReactiveCommand<Unit, object> CancelCommand { get; }
}