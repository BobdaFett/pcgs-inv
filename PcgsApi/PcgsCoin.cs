using System.Diagnostics;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace PcgsApi;

public partial class PcgsCoin : ReactiveObject {
    [Reactive] private string _pcgsNumber;
    [Reactive] private string _name;
    [Reactive] private int _year;
    [Reactive] private string _denomination;
    [Reactive] private string _mintMark;
    [Reactive] private string _grade;
    [Reactive] private string _coinFactsLink;
    [Reactive] private string _majorVariety;
    [Reactive] private string _minorVariety;
    [Reactive] private string _dieVariety;
    [Reactive] private string _seriesName;
    [Reactive] private string _category;
    [Reactive] private string _designation;
    [Reactive] private int _certificateNumber;
    [Reactive] private double _paidFor;
    [Reactive] private string _notes;
    [Reactive] private int _quantity;
    [Reactive] private double _priceGuideValue;
    [ObservableAsProperty] private double _totalPrice;
    
    public PcgsCoin() {
        _totalPriceHelper = this.WhenAnyValue(
                x => x.Quantity,
                x => x.PriceGuideValue,
                (quantity, price) => quantity * price
            )
            .Do(_ => Debug.WriteLine("PcgsCoin total price calculated"))
            .ToProperty(this, nameof(TotalPrice));
    }
}