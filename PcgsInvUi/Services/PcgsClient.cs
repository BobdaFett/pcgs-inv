using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PcgsInvUi.Models;
using System.Text.Json;
using ReactiveUI;

namespace PcgsInvUi.Services;

public class PcgsClient : ReactiveObject {
    public enum ErrorType {
        None,
        NoCoinFound,
        InvalidRequestFormat,
        ApiKeyInvalid
    }
    
    public int NumApiRequests {
        get => _numApiRequests;
        set => this.RaiseAndSetIfChanged(ref _numApiRequests, value);
    }
    
    private string _apiUri = "https://api.pcgs.com/publicapi";
    private readonly string _apiKey;
    private int _numApiRequests;

    public PcgsClient(string apiKey) {
        _apiKey = apiKey;
            // "E8xivTVN_k4Q8ubk8Xv_uU11U91WX0wDht4S4m4BAf2HPmKnGrZ0J2f9nccM7Ns1VkGoT1SXYR74I7aNaMKqXlJGEy3oobseKV0nGiqu1IKvFkOTVjhV37SWUVExSDFMC6FLzs7xJQIHwkcxr5JFOQNmc4ZytEh_-hmCN3NL-z0fEk98tzBmPJ_F7bq_xNru86QMD2A9yalu_dWe-lCysfyE2bCA2EDqwvWenRYf-0XnNtp6qlxxCMlCd-TDqqRKLC3qefCwpXSemwBiNSP5gbC9pwcKrZugkruuJ0nyxxThZymm";
    }

    public async Task<(ErrorType, Coin?)> GetCoinFactsByGrade(int pcgsNumber, string grade, bool plusGrade = false) {
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
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var parsedResponse = ParseJsonResponse(jsonResponse);
                return parsedResponse;
            }
        }
        
        return (ErrorType.ApiKeyInvalid, null);
    }

    private int GetGradeFromString(string grade) {
        var actualGrade = int.Parse(grade.Substring(grade.Length - 2));
        Console.WriteLine($"Grade is {actualGrade}");
        return actualGrade;
    }

    // TODO Will eventually create the other methods that are included in the Public API.

    private (ErrorType, Coin?) ParseJsonResponse(string json) {
        Console.WriteLine(json);
        // TODO Check for errors in the response
        if (json.Contains("\"ServerMessage\":\"No data found\"")) return (ErrorType.NoCoinFound, null);
        if (json.Contains("\"IsValidRequest\":false")) return (ErrorType.InvalidRequestFormat, null);

        var newCoin = JsonSerializer.Deserialize<Coin>(json);
        return (ErrorType.None, newCoin);
    }
}

// Exception for invalid request format
class InvalidRequestFormatException : Exception {
    public InvalidRequestFormatException() { }
    public InvalidRequestFormatException(string message) : base(message) { }
    public InvalidRequestFormatException(string message, Exception inner) : base(message, inner) { }
}

// Exception for no coin found
class CoinNotFoundException : Exception {
    public CoinNotFoundException() { }
    public CoinNotFoundException(string message) : base(message) { }
    public CoinNotFoundException(string message, Exception inner) : base(message, inner) { }
}
