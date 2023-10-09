using System;
using System.Reactive;
using PcgsInvUi.Models;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class EditViewModel : ViewModelBase
{
    public Coin CurrentCoin { get; set; }
    
    public ReactiveCommand<Unit, Unit> OkCommand { get; }

    public EditViewModel(Coin coin)
    {
        CurrentCoin = coin;

        var okEnabled = this.WhenAnyValue(
            x => x.CurrentCoin.PcgsNumber,
            x => x.CurrentCoin.Grade,
            x => x.CurrentCoin.Quantity,
            (id, grade, quantity) => (id != 0) &&
                                     !string.IsNullOrWhiteSpace(grade) &&
                                     (quantity != 0)
        );
        
        // Setup OkCommand - all Coin-related properties must be non-whitespace.
        OkCommand = ReactiveCommand.Create(() => { });
    }
}