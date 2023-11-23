using System.Reactive;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class FindWindowViewModel : ViewModelBase {
    public ReactiveCommand<Unit, object> OkCommand { get; } = ReactiveCommand.Create(() => (object)1);
}