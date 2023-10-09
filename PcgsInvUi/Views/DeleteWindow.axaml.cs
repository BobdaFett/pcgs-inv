using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PcgsInvUi.Views;

public partial class DeleteWindow : Window
{
    public bool Accepted { get; set; }
    
    public DeleteWindow()
    {
        InitializeComponent();
        Accepted = false;
    }
    
    public void AcceptClicked(object source, RoutedEventArgs args)
    {
        // TODO Delete the SelectedCoin?
        Accepted = true;
        Close();
    }
    
    public void CancelClicked(object source, RoutedEventArgs args)
    {
        Close();
    }
}