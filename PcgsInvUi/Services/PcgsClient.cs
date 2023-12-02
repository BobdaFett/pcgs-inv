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

    /// <summary>
    /// Creates a new instance of the PcgsClient class.
    /// Although this is a constructor, it's meant to be called only from the CreateClient method.
    /// </summary>
    private PcgsClient(string apiKey) {
        _apiKey = apiKey;
            // "E8xivTVN_k4Q8ubk8Xv_uU11U91WX0wDht4S4m4BAf2HPmKnGrZ0J2f9nccM7Ns1VkGoT1SXYR74I7aNaMKqXlJGEy3oobseKV0nGiqu1IKvFkOTVjhV37SWUVExSDFMC6FLzs7xJQIHwkcxr5JFOQNmc4ZytEh_-hmCN3NL-z0fEk98tzBmPJ_F7bq_xNru86QMD2A9yalu_dWe-lCysfyE2bCA2EDqwvWenRYf-0XnNtp6qlxxCMlCd-TDqqRKLC3qefCwpXSemwBiNSP5gbC9pwcKrZugkruuJ0nyxxThZymm";
    }
    
    /// <summary>
    /// Creates a new instance of the PcgsClient class.
    /// </summary>
    /// <param name="apiKey">The api key to use for requests.</param>
    /// <returns>A new instance of the PcgsClient class, or null if the key is invalid.</returns>
    public static async Task<PcgsClient?> CreateClient(string apiKey) {
        // Check if the api key is valid.
        var testClient = new PcgsClient(apiKey);
        var validCreds = await testClient.VerifyCredentials();
        if (!validCreds) return null;
        return testClient;
    }

    public async Task<bool> VerifyCredentials() {
        // Create a way to check if the api key given is valid
        // I think the best way to do this is to make a request to the api and see if it returns the expected response.
        var result = await this.GetCoinFactsByGrade(1, "AG-03");  // This is a known invalid coin id - this is because the api is very fast with invalid requests.
        if (result.Item1 == ErrorType.ApiKeyInvalid) return false;
        return true;
    }

    public async Task<(ErrorType, Coin?)> GetCoinFactsByGrade(int pcgsNumber, string grade, bool plusGrade = false) {
        using (HttpClient client = new HttpClient()) {
            var gradeParam = GetGradeFromString(grade);

            string requestUri =
                $"{_apiUri}/coindetail/GetCoinFactsByGrade/?PCGSNo={pcgsNumber}&GradeNo={gradeParam}&PlusGrade={plusGrade}";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _apiKey);
            Console.WriteLine($"Sending request to {requestUri}");
            HttpResponseMessage response = await client.GetAsync(requestUri);
            Console.WriteLine("Got response.");

            if (response.IsSuccessStatusCode) {
                // Check response for errors.
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var parsedResponse = ParseJsonResponse(jsonResponse);
                return parsedResponse;
            }
        }
        
        return (ErrorType.ApiKeyInvalid, null);
    }

    private int GetGradeFromString(string grade) {
        var actualGrade = int.Parse(grade.Substring(grade.Length - 2));  // Throws an exception if the grade doesn't exist.
        Console.WriteLine($"Grade is {actualGrade}");
        return actualGrade;
    }

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
