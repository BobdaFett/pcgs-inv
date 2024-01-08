using ReactiveUI;
using System.Reactive;

namespace PcgsInvUi.ViewModels;

public class ApiKeyViewModel : ViewModelBase {
    public string ApiKeyInput {
        get => _apiKeyInput;
        set => this.RaiseAndSetIfChanged(ref _apiKeyInput, value);
    }
    private string _apiKeyInput = "";

    public bool OkEnabled {
        get => _okEnabled;
        set => this.RaiseAndSetIfChanged(ref _okEnabled, value);
    }
    private bool _okEnabled;

    public ReactiveCommand<Unit, string> OkCommand { get; }

    public ApiKeyViewModel() {
        var okEnabled = this.WhenAnyValue(
            x => x.ApiKeyInput,
            x => !string.IsNullOrWhiteSpace(x));


        // Get the API key from the user.
        OkCommand = ReactiveCommand.Create(() => ApiKeyInput, okEnabled);
    }
}
