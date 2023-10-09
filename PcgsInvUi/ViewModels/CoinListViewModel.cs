using System.Collections.Generic;
using System.Collections.ObjectModel;
using PcgsInvUi.Models;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class CoinListViewModel : ViewModelBase
{
    public ObservableCollection<Coin> CoinCollection { get; set; }
    private float _totalValue;
    public float TotalValue
    {
        get => _totalValue;
        set => this.RaiseAndSetIfChanged(ref _totalValue, value);
    }
    public Coin? SelectedCoin { get; set; }
    
    public CoinListViewModel(IEnumerable<Coin> items)
    {
        CoinCollection = new ObservableCollection<Coin>(items);
        TotalValue = 0;
    }
}