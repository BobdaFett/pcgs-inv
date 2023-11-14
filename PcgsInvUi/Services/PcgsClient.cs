using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PcgsInvUi.Models;
using System.Text.Json;
using ReactiveUI;


namespace PcgsInvUi.Services;

/// <summary>
/// Provides an interface into the PCGS Public API.
/// </summary>
public class PcgsClient : ReactiveObject {
    
    public int NumApiRequests {
        get => _numApiRequests;
        set => this.RaiseAndSetIfChanged(ref _numApiRequests, value);
    }
    
    private string _apiUri = "https://api.pcgs.com/publicapi";
    private readonly string _apiKey;
    private int _numApiRequests;

    public PcgsClient(string apiKey) {
        // I'd like to be able to test the API key upon initialization.
        _apiKey =
            "E8xivTVN_k4Q8ubk8Xv_uU11U91WX0wDht4S4m4BAf2HPmKnGrZ0J2f9nccM7Ns1VkGoT1SXYR74I7aNaMKqXlJGEy3oobseKV0nGiqu1IKvFkOTVjhV37SWUVExSDFMC6FLzs7xJQIHwkcxr5JFOQNmc4ZytEh_-hmCN3NL-z0fEk98tzBmPJ_F7bq_xNru86QMD2A9yalu_dWe-lCysfyE2bCA2EDqwvWenRYf-0XnNtp6qlxxCMlCd-TDqqRKLC3qefCwpXSemwBiNSP5gbC9pwcKrZugkruuJ0nyxxThZymm";
    }

    public async Task<Coin> GetCoinFactsByGrade(int pcgsNumber, string grade, bool plusGrade = false) {
        Coin newCoin = null;
        using (HttpClient client = new HttpClient()) {
            var gradeParam = GetGradeFromString(grade);

            string requestUri =
                $"{_apiUri}/coindetail/GetCoinFactsByGrade/?PCGSNo={pcgsNumber}&GradeNo={gradeParam}&PlusGrade={plusGrade}";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _apiKey);
            Console.WriteLine($"Sending request to {requestUri}");
            HttpResponseMessage response = await client.GetAsync(requestUri);
            Console.WriteLine("Got response.");

            if (response.IsSuccessStatusCode) {
                // TODO Check for errors indicated by a successful response.
                string jsonResponse = await response.Content.ReadAsStringAsync();
                newCoin = ParseJsonResponse(jsonResponse);
                Console.WriteLine("Coin created.");
            }
            else {
                // TODO Throw HTTP exception.
                Console.WriteLine("Error getting coin.");
                
            }
        }

        return newCoin;
    }

    private int GetGradeFromString(string grade) {
        var actualGrade = int.Parse(grade.Substring(grade.Length - 2));
        Console.WriteLine($"Grade is {actualGrade}");
        return actualGrade;
    }

    // TODO Will eventually create the other methods that are included in the Public API.

    private Coin ParseJsonResponse(string json) {
        Console.WriteLine(json);
        Coin newCoin = JsonSerializer.Deserialize<Coin>(json);
        Console.WriteLine($"New Coin has pcgs number {newCoin.PCGSNo}");

        string test = JsonSerializer.Serialize(newCoin);
        Console.WriteLine(test);

        return newCoin;
    }
}

/// <summary>
/// Thrown when the API response indicates that the request was invalid.
/// Generally used when the request was formatted incorrectly.
/// </summary>
public class InvalidRequestException : Exception {
    public InvalidRequestException() { }
    public InvalidRequestException(string message) : base(message) { }
    public InvalidRequestException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when the API response indicates that the coin was not found.
/// Generally used when the PCGS number and grade combo that was requested does not exist in the PCGS database.
/// </summary>
public class CoinNotFoundException : Exception {
    public CoinNotFoundException() { }
    public CoinNotFoundException(string message) : base(message) { }
    public CoinNotFoundException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when the API returns an HTTP error 401.
/// Generally indicates that the API key is invalid.
/// </summary>
public class ApiAuthorizationException : Exception {
    public ApiAuthorizationException() { }
    public ApiAuthorizationException(string message) : base(message) { }
    public ApiAuthorizationException(string message, Exception inner) : base(message, inner) { }
}

public 
