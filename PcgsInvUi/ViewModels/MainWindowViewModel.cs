using System;
using System.Reactive.Linq;
using PcgsInvUi.Models;
using PcgsInvUi.Services;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public CoinListViewModel List { get; }
    private ViewModelBase _mainContent;
    private ViewModelBase _sideContent;
    
    public MainWindowViewModel(CoinCollection coins)
    {
        List = new CoinListViewModel(coins.GetItems());
        MainContent = List;
    }
    
    public ViewModelBase MainContent
    {
        get => _mainContent;
        private set => this.RaiseAndSetIfChanged(ref _mainContent, value);
    }

    public ViewModelBase SidebarContent
    {
        get => _sideContent;
        private set => this.RaiseAndSetIfChanged(ref _sideContent, value);
    }

    public void AddItem()
    {
        var newViewModel = new NewViewModel();

        Observable.Merge(
                newViewModel.OkCommand,
                newViewModel.CancelCommand.Select(_ => (Coin?)null))
            .Take(1)
            .Subscribe(model =>
            {
                if (model != null)
                    List.CoinCollection.Add(model);

                MainContent = List;
            });
        
        MainContent = newViewModel;
    }

    public void EditItem()
    {
        var editViewModel = new EditViewModel(List.SelectedCoin);
    }

    public void DeleteItem()
    {
        
    }

    public void FindItem()
    {
        
    }
}
