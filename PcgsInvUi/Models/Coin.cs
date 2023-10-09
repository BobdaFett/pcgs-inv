using System;
using ReactiveUI;

namespace PcgsInvUi.Models;

public class Coin : ReactiveObject
{
    public int PcgsNumber
    {
        get => _pcgsNumber;
        set => this.RaiseAndSetIfChanged(ref _pcgsNumber, value);
    }
    private int _pcgsNumber;

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    private string _name;

    public int Year
    {
        get => _year;
        set => this.RaiseAndSetIfChanged(ref _year, value);
    }
    private int _year;

    public string Denomination
    {
        get => _denomination;
        set => this.RaiseAndSetIfChanged(ref _denomination, value);
    }
    private string _denomination;

    public string MintMark
    {
        get => _mintMark;
        set => this.RaiseAndSetIfChanged(ref _mintMark, value);
    }
    private string _mintMark;

    public string Grade
    {
        get => _grade;
        set => this.RaiseAndSetIfChanged(ref _grade, value);
    }
    private string _grade;

    public double PriceGuideValue
    {
        get => _priceGuideValue;
        set => this.RaiseAndSetIfChanged(ref _priceGuideValue, value);
    }
    private double _priceGuideValue;

    public string CoinFactsLink
    {
        get => _coinFactsLink;
        set => this.RaiseAndSetIfChanged(ref _coinFactsLink, value);
    }
    private string _coinFactsLink;

    public string MajorVariety
    {
        get => _majorVariety;
        set => this.RaiseAndSetIfChanged(ref _majorVariety, value);
    }
    private string _majorVariety;

    public string MinorVariety
    {
        get => _minorVariety;
        set => this.RaiseAndSetIfChanged(ref _minorVariety, value);
    }
    private string _minorVariety;

    public string DieVariety
    {
        get => _dieVariety;
        set => this.RaiseAndSetIfChanged(ref _dieVariety, value);
    }
    private string _dieVariety;

    public string Series
    {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
    }
    private string _series;

    public string Category
    {
        get => _category;
        set => this.RaiseAndSetIfChanged(ref _category, value);
    }
    private string _category;

    public string Designation
    {
        get => _designation;
        set => this.RaiseAndSetIfChanged(ref _designation, value);
    }
    private string _designation;

    public int CertificateNumber
    {
        get => _certificateNumber;
        set => this.RaiseAndSetIfChanged(ref _certificateNumber, value);
    }
    private int _certificateNumber;

    public int Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }
    private int _quantity;
    
    public double TotalPrice => Quantity * PriceGuideValue;

    public double PaidFor
    {
        get => _paidFor;
        set => this.RaiseAndSetIfChanged(ref _paidFor, value);
    }
    private double _paidFor;

    public String Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }
    private string _notes;
}