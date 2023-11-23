using System.Reactive;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class DeleteWindowViewModel : ViewModelBase {
    public ReactiveCommand<Unit, object> OkCommand { get; }
    public ReactiveCommand<Unit, object> CancelCommand { get; }

    public DeleteWindowViewModel() {
        OkCommand = ReactiveCommand.Create(() => (object)true);
        CancelCommand = ReactiveCommand.Create(() => (object)false);
    }
}
