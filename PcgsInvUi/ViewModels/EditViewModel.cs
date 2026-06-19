using System.Reactive;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class EditViewModel : ViewModelBase {
    public EditViewModel(PcgsCoin pcgsCoin) {
        CurrentPcgsCoin = pcgsCoin;

        var okEnabled = this.WhenAnyValue(
            x => x.CurrentPcgsCoin.PcgsNumber,
            x => x.CurrentPcgsCoin.Grade,
            x => x.CurrentPcgsCoin.Quantity,
            (id, grade, quantity) => !string.IsNullOrWhiteSpace(id) &&
                                     !string.IsNullOrWhiteSpace(grade) &&
                                     quantity != 0
        );

        // Setup OkCommand - all Coin-related properties must be non-whitespace.
        OkCommand = ReactiveCommand.Create(() => { });
    }

    public PcgsCoin CurrentPcgsCoin { get; set; }

    public ReactiveCommand<Unit, Unit> OkCommand { get; }
}