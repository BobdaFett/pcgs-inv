using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using PcgsInvUi.Models;
using PcgsInvUi.Services;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

// TODO Find window.
// TODO Export window and export function.
// TODO Error handling.
// TODO Manual handling of API key.
// TODO Use CoinFacts link.
// TODO Take giant sheet of coins to integrate into the application.
// TODO Update all coins.
// TODO API request tracker (1000 per day)
// TODO API update schedules (per month? manual override? needs to store a date of last update in the coin object)

public class MainWindowViewModel : ViewModelBase
{
    public ViewModelBase SidebarContent
    {
        get => _sideContent;
        private set => this.RaiseAndSetIfChanged(ref _sideContent, value);
    }
    public ObservableCollection<Coin> CoinCollection { get; set; }
    public ObservableCollection<Coin> DisplayedList { get; set; }
    public Coin? SelectedCoin
    {
        get => _selectedCoin;
        set => this.RaiseAndSetIfChanged(ref _selectedCoin, value);
    }
    public double TotalValue
    {
        get => _totalValue;
        set => this.RaiseAndSetIfChanged(ref _totalValue, value);
    }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public Interaction<DeleteWindowViewModel, Boolean> ShowDeleteWindow { get; }
    public ReactiveCommand<Unit, Coin> FindCommand { get; }
    public Interaction<FindWindowViewModel, int> ShowFindWindow { get; }
    
    private ViewModelBase _sideContent;
    private Coin? _selectedCoin;
    private double _totalValue;
    private PcgsClient _pcgsClient;

    public MainWindowViewModel(CoinCollection coins)
    {
        var newViewModel = new NewViewModel();
        _sideContent = newViewModel;
        DisplayedList = CoinCollection = new ObservableCollection<Coin>(coins.GetItems());
        _pcgsClient = new PcgsClient("eAb8gS0I2XAvT_5gJeiGJaglMia1Tk-oB4kJUK6kuafyrny_S61vIJY-Ikl4nCQM67wrdxzUqLVWTV2kBSxD3d5XNBHxHnYBhcSS6dOPug0hZaF3qAv56df3gYSzOGh9Tif5y0eP3Iw0LrqKDr1Hj-dk6SV6GKog2IIqCPQhhHH8FMTWBTYO-_O8cx7qLdM5GM8KlTsic6g3VRUhM8EA_4OO04dCfmNLGhqINRl3jGZ0Q4ziI8fng2bVWsIyteqiPzUn10rIQ3-OPpqVZG_DxeOmOejj4GzbUNyqUOajy-nr5rYY");
        
        // Set up the ability to open the delete window.
        ShowDeleteWindow = new Interaction<DeleteWindowViewModel, bool>();
        var deleteEnabled = this.WhenAnyValue(x => x.SelectedCoin)
            .Select(x => x != null);
        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var deleteViewModel = new DeleteWindowViewModel();
            var result = await ShowDeleteWindow.Handle(deleteViewModel);
            if (result) CoinCollection.Remove(SelectedCoin);  // possible null deref - ignored due to button being disabled.
        }, deleteEnabled);

        // Set up the ability to open the find window.
        ShowFindWindow = new Interaction<FindWindowViewModel, int>();
        var findEnabled = this.WhenAnyValue(x => x.CoinCollection.Count,
            x => x > 0);
        FindCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var findViewModel = new FindWindowViewModel();
            var result = await ShowFindWindow.Handle(findViewModel);
            // Find the coin with the matching ID. The issue is that there could be multiple - so we need to return a list.
            // Perhaps a secondary viewmodel?
            return new Coin();
        }, findEnabled);
        
        // Subscribe to SidebarContent.OkCommand, which is an IObservable<(int, string, int)>
        // This is the same as the above, but with a different syntax.
        newViewModel.OkCommand
            .Subscribe(async requestStructure =>
            {
                var newCoin = await _pcgsClient.GetCoinFactsByGrade(requestStructure.Item1, requestStructure.Item2);
                newCoin.Quantity = requestStructure.Item3;
                CoinCollection.Add(newCoin);
                TotalValue += newCoin.Quantity * newCoin.PriceGuideValue;
            });
        
        // Change TotalValue anytime SelectedCoin.TotalPrice changes.
        // Possible null deref - ignored due to null values summing to 0.
        this.WhenAnyValue(x => x.SelectedCoin.TotalPrice)
            .Subscribe(_ => TotalValue = CoinCollection.Sum(x => x.TotalPrice) );
    }
}
