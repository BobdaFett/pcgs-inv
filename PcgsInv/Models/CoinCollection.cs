using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using PcgsApi;

namespace PcgsInv.Models;

/// <summary>
///     Contains all information related to a collection of coins.
/// </summary>
public partial class CoinCollection : ReactiveObject {
    /// <summary>
    ///     The name of the collection.
    /// </summary>
    [Reactive] private string _name;

    /// <summary>
    ///     The collection of <see cref="PcgsCoin"/> objects.
    /// </summary>
    [Reactive] private ObservableCollection<PcgsCoin> _coins;

    /// <summary>
    ///     The total value of the collection.
    /// </summary>
    [ObservableAsProperty] private double _totalValue;

    /// <summary>
    /// Creates a new CoinCollection instance.
    /// </summary>
    public CoinCollection() {
        Name = string.Empty;
        Coins = [];

        Coins.ToObservableChangeSet()
            .AutoRefresh(x => x.TotalPrice)
            .ToCollection()
            .Select(coins => coins.Sum(c => c.TotalPrice))
            .ToProperty(this, nameof(TotalValue), out _totalValueHelper);
    }
}