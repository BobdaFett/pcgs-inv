using System;
using System.Reactive.Linq;
using PcgsInvUi.Models;
using PcgsInvUi.Services;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public CoinListViewModel List { get; }
    private ViewModelBase _content;
    
    public MainWindowViewModel(CoinCollection coins)
    {
        List = new CoinListViewModel(coins.GetItems());
        Content = List;
    }
    
    public ViewModelBase Content
    {
        get => _content;
        private set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public void AddItem()
    {
        Console.WriteLine("You hit the button!");
        var newViewModel = new NewViewModel();

        Observable.Merge(
                newViewModel.OkCommand,
                newViewModel.CancelCommand.Select(_ => (Coin?)null))
            .Take(1)
            .Subscribe(model =>
            {
                if (model != null)
                    List.CoinCollection.Add(model);

                Content = List;
            });
        
        Content = newViewModel;
    }
}
