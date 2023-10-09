using System;

namespace PcgsInvUi.Models;

public class Coin
{
    public int PcgsNumber { get; set; }
    public string Name { get; set; }
    public int Year { get; set; }
    public string Denomination { get; set; }
    public string MintMark { get; set; }
    public string Grade { get; set; }
    public double PriceGuideValue { get; set; }
    public string CoinFactsLink { get; set; }
    public string MajorVariety { get; set; }
    public string MinorVariety { get; set; }
    public string DieVariety { get; set; }
    public string Series { get; set; }
    public string Category { get; set; }
    public string Designation { get; set; }
    public int CertificateNumber { get; set; }
    public int Quantity { get; set; }
    public double TotalPrice => Quantity * PriceGuideValue;
    public double PaidFor { get; set; }
    public String Notes { get; set; }
}