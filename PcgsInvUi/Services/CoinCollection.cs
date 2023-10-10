using System.Collections.Generic;
using PcgsInvUi.Models;

namespace PcgsInvUi.Services;

public class CoinCollection
{
    public IEnumerable<Coin> GetItems() => new[]
    {
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
        new Coin(),
    };
}