using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using PcgsInvUi.Models;
using PcgsInvUi.Services;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
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
    public ICommand DeleteCommand { get; }
    public Interaction<DeleteWindowViewModel, Boolean> ShowDeleteWindow { get; }
    
    private ViewModelBase? _sideContent;
    private Coin? _selectedCoin;
    private double _totalValue;
    private PcgsClient _pcgsClient;

    public MainWindowViewModel(CoinCollection coins)
    {
        var newViewModel = new NewViewModel();
        SidebarContent = newViewModel;
        CoinCollection = new ObservableCollection<Coin>(coins.GetItems());
        _pcgsClient = new PcgsClient("eAb8gS0I2XAvT_5gJeiGJaglMia1Tk-oB4kJUK6kuafyrny_S61vIJY-Ikl4nCQM67wrdxzUqLVWTV2kBSxD3d5XNBHxHnYBhcSS6dOPug0hZaF3qAv56df3gYSzOGh9Tif5y0eP3Iw0LrqKDr1Hj-dk6SV6GKog2IIqCPQhhHH8FMTWBTYO-_O8cx7qLdM5GM8KlTsic6g3VRUhM8EA_4OO04dCfmNLGhqINRl3jGZ0Q4ziI8fng2bVWsIyteqiPzUn10rIQ3-OPpqVZG_DxeOmOejj4GzbUNyqUOajy-nr5rYY");
        
        ShowDeleteWindow = new Interaction<DeleteWindowViewModel, bool>();
        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var deleteViewModel = new DeleteWindowViewModel();
            var result = await ShowDeleteWindow.Handle(deleteViewModel);
            if (result) CoinCollection.Remove(SelectedCoin);
        });
        
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
    }

    public void FindItem()
    {
        
    }
}
