using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PcgsInvUi.ViewModels;

public partial class CoinListViewModel : ViewModelBase {
    public CoinListViewModel(IEnumerable<PcgsCoin> items) {
        CoinCollection = new ObservableCollection<PcgsCoin>(items);
        TotalValue = 0;
    }

    public ObservableCollection<PcgsCoin> CoinCollection { get; set; }

    [ObservableProperty]
    public partial float TotalValue { get; set; }

    [ObservableProperty]
    public partial PcgsCoin? SelectedCoin { get; set; }
}