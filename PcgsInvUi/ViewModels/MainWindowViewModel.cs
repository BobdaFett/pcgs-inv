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
    public Interaction<ErrorWindowViewModel, bool> ShowErrorWindow { get; }

    private ViewModelBase _sideContent;
    private Coin? _selectedCoin;
    private double _totalValue;
    
    public MainWindowViewModel(CoinDatabase coins, bool askForApiKey) {
        ConnectedCoinDatabase = coins;
        var newViewModel = new NewViewModel();
        _sideContent = newViewModel;
        // DisplayedList = CoinCollection = new ObservableCollection<Coin>(coins.GetItems());
        DisplayedList = ConnectedCoinDatabase.Collection;
        TotalValue = DisplayedList.Sum(x => x.TotalPrice);
        
        // Set up the ability to show the error window.
        ShowErrorWindow = new Interaction<ErrorWindowViewModel, bool>();
        
        // Set up the ability to open the delete window.
        ShowDeleteWindow = new Interaction<DeleteWindowViewModel, bool>();
        var deleteEnabled = this.WhenAnyValue(x => x.SelectedCoin)
            .Select(x => x != null);
        DeleteCommand = ReactiveCommand.CreateFromTask(async () => {
            var deleteViewModel = new DeleteWindowViewModel();
            var result = await ShowDeleteWindow.Handle(deleteViewModel);
            if (result && SelectedCoin is not null) {
                ConnectedCoinDatabase.DeleteCoin(SelectedCoin);
                ConnectedCoinDatabase.Collection.Remove(SelectedCoin);
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
                await ShowErrorWindow.Handle(new ErrorWindowViewModel(e.Message));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                await ShowErrorWindow.Handle(new ErrorWindowViewModel(e.Message));
            }
        });
        
        // Subscribe to SidebarContent.OkCommand, which is an IObservable<(int, string, int)>
        // This is the same as the above, but with a different syntax.
        newViewModel.OkCommand
            .Subscribe(async requestStructure => {
                var result = await ConnectedCoinDatabase.CreateCoin(requestStructure.Item1, requestStructure.Item2,
                    requestStructure.Item3);
                
                switch (result) {
                    case PcgsClient.ErrorType.ApiKeyInvalid:
                        Console.WriteLine("API key is invalid - showing window.");
                        await ShowErrorWindow.Handle(new ErrorWindowViewModel("API key is invalid - authorization failed."));
                        break;
                    case PcgsClient.ErrorType.InvalidRequestFormat:
                        Console.WriteLine("Request format is invalid - showing window.");
                        await ShowErrorWindow.Handle(new ErrorWindowViewModel("Request format is invalid."));
                        break;
                    case PcgsClient.ErrorType.NoCoinFound:
                        Console.WriteLine("No coin was found with the given parameters.");
                        await ShowErrorWindow.Handle(new ErrorWindowViewModel("No coin was found with the given parameters."));
                        break;
                }
                
                // TODO Clear the text boxes.
            });
        
        // Update TotalValue anytime a coin's quantity changes.
        // Possible null reference is currently ignored - SelectedCoin will never be null.
        this.WhenAnyValue(x => x.SelectedCoin.TotalPrice)
            .Subscribe(_ => TotalValue = ConnectedCoinDatabase.Collection.Sum(x => x.TotalPrice));

        // Update TotalValue anytime a coin is added or removed.
        ConnectedCoinDatabase.Collection.CollectionChanged += (sender, args) => {
            TotalValue = ConnectedCoinDatabase.Collection.Sum(x => x.TotalPrice);
        };
    }

    // Create destructor to save the database when the application exits.
    // This is necessary because the database is initialized in this class.
    ~MainWindowViewModel() {
        Console.WriteLine("Running MainWindowViewModel destructor.");
        ConnectedCoinDatabase.UpdateCollection("CollectionTable");
    }
}
