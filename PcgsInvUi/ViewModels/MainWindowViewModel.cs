using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using PcgsInvUi.Models;
using PcgsInvUi.Services;
using ReactiveUI;
using System.Threading.Tasks;

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

    public MainWindowViewModel(CoinDatabase coins) {
        ConnectedCoinDatabase = coins;
        var keyFound = Task.Run(() => coins.TryInitApiClient()).Result;
        Console.WriteLine("Function completed.");
        var newViewModel = new NewViewModel();
        var apiKeyViewModel = new ApiKeyViewModel();
        // DisplayedList = CoinCollection = new ObservableCollection<Coin>(coins.GetItems());
        DisplayedList = ConnectedCoinDatabase.Collection;
        TotalValue = DisplayedList.Sum(x => x.TotalPrice);

        // Setup interaction to show ApiKeyWindow
        apiKeyViewModel.OkCommand.Subscribe(async apiKey => {
            if (await ConnectedCoinDatabase.TryInitApiClient(apiKey))
                SidebarContent = newViewModel;  // Must assign this to the observable property rather than private variable.
            });

        // Check if API key must be entered.
        if (!keyFound) {
            Console.WriteLine("API key must be entered by user.");
            // Show the API key input view.
            _sideContent = apiKeyViewModel;
        } else _sideContent = newViewModel;
        
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
                
                // Clear the text boxes.
                newViewModel.ClearText();
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
