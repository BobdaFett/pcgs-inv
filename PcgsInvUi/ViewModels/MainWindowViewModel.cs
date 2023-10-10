using System;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PcgsInvUi.Models;
using PcgsInvUi.Services;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public bool PaneOpen
    {
        get => _paneOpen;
        set => this.RaiseAndSetIfChanged(ref _paneOpen, value);
    }
    public ViewModelBase SidebarContent
    {
        get => _sideContent;
        private set => this.RaiseAndSetIfChanged(ref _sideContent, value);
    }
    public ObservableCollection<Coin> CoinCollection { get; set; }
    public Coin SelectedCoin
    {
        get => _selectedCoin;
        set => this.RaiseAndSetIfChanged(ref _selectedCoin, value);
    }
    public double TotalValue
    {
        get => _totalValue;
        set => this.RaiseAndSetIfChanged(ref _totalValue, value);
    }
    
    private ViewModelBase? _sideContent;
    private Coin? _selectedCoin;
    private bool _paneOpen;
    private double _totalValue;

    public MainWindowViewModel(CoinCollection coins)
    {
        CoinCollection = new ObservableCollection<Coin>(coins.GetItems());
    }

    public void AddItem()
    {
        var newViewModel = new NewViewModel();

        Observable.Merge(
                newViewModel.OkCommand,
                newViewModel.CancelCommand.Select(_ => (Coin?)null))
            .Take(1)
            .Subscribe(newCoin =>
            {
                if (newCoin != null)
                {
                    // This raises a CollectionChanged event, which *should* update the UI, but doesn't.
                    CoinCollection.Add(newCoin);
                    // Update view
                    TotalValue += newCoin.TotalPrice;
                    
                    // Setting main content to null and then back to the main view model works just fine.
                    // TotalValue updates just fine.
                }

                SidebarContent = null;
                PaneOpen = false;
            });
        
        SidebarContent = newViewModel;
        PaneOpen = true;
    }

    public void EditItem()
    {
        var editViewModel = new EditViewModel(SelectedCoin);
    }

    public void DeleteItem()
    {
        
    }

    public void FindItem()
    {
        
    }
}
