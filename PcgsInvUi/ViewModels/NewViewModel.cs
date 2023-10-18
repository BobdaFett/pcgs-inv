using System.Reactive;
using System.Collections.Generic;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class NewViewModel : ViewModelBase
{
    private string _pcgsNumber = "";
    private string _grade = "";
    private string _quantity = "";
    
    public string PcgsNumber
    {
        get => _pcgsNumber;
        set => this.RaiseAndSetIfChanged(ref _pcgsNumber, value);
    }
    public string Grade
    {
        get => _grade;
        set => this.RaiseAndSetIfChanged(ref _grade, value);
    }
    public string Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }
    public List<string> GradesList { get; }

    public NewViewModel()
    {
        var okEnabled = this.WhenAnyValue(
            x => x.PcgsNumber,
            x => x.Grade,
            x => x.Quantity,
            (id, grade, quantity) => !string.IsNullOrWhiteSpace(id) &&
                                     !string.IsNullOrWhiteSpace(grade) &&
                                     !string.IsNullOrWhiteSpace(quantity));

        OkCommand = ReactiveCommand.Create(
            () => (int.Parse(PcgsNumber), Grade, int.Parse(Quantity)),
            okEnabled);
        
        ClearCommand = ReactiveCommand.Create(() =>
        {
            PcgsNumber = "";
            Grade = "";
            Quantity = "";
        });

        // Initialize grade values for use in the view.
        // This must be typed out manually, as there is no way to get a list of grades from the API.
        GradesList = new List<string>
        {
            "P-01",
            "FR-02",
            "AG-03",
            "G-04",
            "G-06",
            "VG-08",
            "VG-10",
            "F-12",
            "F-15",
            "VF-20",
            "VF-25",
            "VF-30",
            "VF-35",
            "EF-40",
            "EF-45",
            "AU-50",
            "AU-53",
            "AU-55",
            "AU-58",
            "MS-60",
            "MS-61",
            "MS-62",
            "MS-63",
            "MS-64",
            "MS-65",
            "MS-66",
            "MS-67",
            "MS-68",
            "MS-69",
            "MS-70"
        };
    }
    
    public ReactiveCommand<Unit, (int, string, int)> OkCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }

}