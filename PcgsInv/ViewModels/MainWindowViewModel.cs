using System;
using System.Diagnostics;
using PcgsApi;
using PcgsInv.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace PcgsInv.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    [Reactive] private CoinCollection _collection;
    [Reactive] private PcgsCoin? _selectedCoin;

    public MainWindowViewModel() {
        Collection = new CoinCollection();
        _selectedCoin = null;

        Collection.PropertyChanged += (s, e) => {
            Debug.WriteLine($"Collection raised event: {e.PropertyName}");
        };
        
        this.WhenAnyValue(x => x.Collection.TotalValue)
            .Subscribe(x => Debug.WriteLine($"Total value was recomputed: {x}"));
    }

    [ReactiveCommand]
    private void CreateTestingCoin() {
        Collection.Coins.Add(new PcgsCoin());
    }
}