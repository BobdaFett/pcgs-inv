using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using PcgsInvUi.Models;
using PcgsInvUi.Services;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

// TODO Error handling for API calls.
// TODO Manual handling of API key.
// TODO Use CoinFacts link.
// TODO Splash screen during startup.
// TODO Take giant sheet of coins to integrate into the application.
// TODO Update all coins. Choose those that are older first.
// TODO API request tracker (1000 per day)
// TODO Ok button greys out while request is happening.
// TODO Track when coins were last updated.
// TODO Image for coin in notes view.

public class MainWindowViewModel : ViewModelBase {
    public ViewModelBase SidebarContent {
        get => _sideContent;
        private set => this.RaiseAndSetIfChanged(ref _sideContent, value);
    }
    public ObservableCollection<Coin> DisplayedList { get; set; }
    public CoinDatabase ConnectedCoinDatabase { get; set; }
    public Coin? SelectedCoin {
        get => _selectedCoin;
        set => this.RaiseAndSetIfChanged(ref _selectedCoin, value);
    }
    public double TotalValue {
        get => _totalValue;
        set => this.RaiseAndSetIfChanged(ref _totalValue, value);
    }

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public Interaction<DeleteWindowViewModel, Boolean> ShowDeleteWindow { get; }
    public ReactiveCommand<Unit, Unit> ExportCommand { get; }
    public Interaction<Unit, Uri> ShowExportWindow { get; }

    private ViewModelBase _sideContent;
    private Coin? _selectedCoin;
    private double _totalValue;
    private PcgsClient _pcgsClient;
    
    public MainWindowViewModel(CoinDatabase coins) {
        ConnectedCoinDatabase = coins;
        var newViewModel = new NewViewModel();
        _sideContent = newViewModel;
        // DisplayedList = CoinCollection = new ObservableCollection<Coin>(coins.GetItems());
        DisplayedList = ConnectedCoinDatabase.Collection;
        TotalValue = DisplayedList.Sum(x => x.TotalPrice);
        
        // Set up the ability to open the delete window.
        ShowDeleteWindow = new Interaction<DeleteWindowViewModel, bool>();
        var deleteEnabled = this.WhenAnyValue(x => x.SelectedCoin)
            .Select(x => x != null);
        DeleteCommand = ReactiveCommand.CreateFromTask(async () => {
            var deleteViewModel = new DeleteWindowViewModel();
            var result = await ShowDeleteWindow.Handle(deleteViewModel);
            if (result && SelectedCoin is not null) {
                ConnectedCoinDatabase.DeleteCoin(SelectedCoin);
                ConnectedCoinDatabase.Collection.Remove(SelectedCoin); // possible null deref - ignored due to button being disabled.
            }
        }, deleteEnabled);

        ShowExportWindow = new Interaction<Unit, Uri>();
        ExportCommand = ReactiveCommand.CreateFromTask(async () => {
            try {
                var filePath = await ShowExportWindow.Handle(Unit.Default);
                var stream = File.Open(filePath.AbsolutePath, FileMode.OpenOrCreate);
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(ConnectedCoinDatabase.CollectionToCSV("CollectionTable"));
                await writer.FlushAsync();
            }
            catch (IOException e) {
                // Something is wrong with the file after it's been created.
                Console.WriteLine(e.Message);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        });
        
        // Subscribe to SidebarContent.OkCommand, which is an IObservable<(int, string, int)>
        // This is the same as the above, but with a different syntax.
        newViewModel.OkCommand
            .Subscribe(async requestStructure => {
                ConnectedCoinDatabase.CreateCoin(requestStructure.Item1, requestStructure.Item2, requestStructure.Item3);
            });

        // Change TotalValue anytime SelectedCoin.TotalPrice changes.
        // Possible null deref - ignored due to null values summing to 0.
        this.WhenAnyValue(x => x.SelectedCoin.TotalPrice)
            .Subscribe(_ => TotalValue = ConnectedCoinDatabase.Collection.Sum(x => x.TotalPrice));
    }
}