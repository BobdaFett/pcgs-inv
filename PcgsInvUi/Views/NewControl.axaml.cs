using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
namespace PcgsInvUi.Views;

public partial class NewControl : UserControl
{
    public string SelectedGrade { get; set; } = "";
    
    public void AcceptClick(object source, RoutedEventArgs args)
    {
        // TODO Send a message to the MainWindow to add a new coin and close the NewControl.
    }
    
    public void CancelClick(object source, RoutedEventArgs args)
    {
        // TODO Send a message to the MainWindow to close the NewControl.
    }

    public NewControl()
    {
        InitializeComponent();
        GradeChoices.SelectedItem = SelectedGrade;
        // Had to type this out - there's no pattern to follow.
        GradeChoices.ItemsSource = new ObservableCollection<string>()
        {
            "PO01",
            "FR02",
            "AG03",
            "G04",
            "G06",
            "VG08",
            "VG10",
            "F12",
            "F15",
            "VF20",
            "VF25",
            "VF30",
            "VF35",
            "EF40",
            "EF45",
            "AU50",
            "AU53",
            "AU55",
            "AU58",
            "MS60",
            "MS61",
            "MS62",
            "MS63",
            "MS64",
            "MS65",
            "MS66",
            "MS67",
            "MS68",
            "MS69",
            "MS70"
        };
    }
    
    public NewControl(object? context = null) : this()
    {
        DataContext = context;
    }
}