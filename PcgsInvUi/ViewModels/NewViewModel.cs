using System;
using System.Reactive;
using PcgsInvUi.Models;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class NewViewModel : ViewModelBase
{
    private string _pcgsNumber;
    private string _grade;
    private string _quantity;
    
    public string PcgsNumber
    {
        get => _pcgsNumber;
        set => this.RaiseAndSetIfChanged(ref _pcgsNumber, value);
    }
    public string Grade
    {
        get => _grade;
        set => this.RaiseAndSetIfChanged(ref _grade, value);
    }
    public string Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }
    
    public NewViewModel()
    {
        var okEnabled = this.WhenAnyValue(
            x => x.PcgsNumber,
            x => !string.IsNullOrWhiteSpace(x));

        OkCommand = ReactiveCommand.Create(
            () => new Coin { PcgsNumber = Int32.Parse(PcgsNumber) },
            okEnabled);
        CancelCommand = ReactiveCommand.Create(() => { });
    }

    public ReactiveCommand<Unit, Coin> OkCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

}