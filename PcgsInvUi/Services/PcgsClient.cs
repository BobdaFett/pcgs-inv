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

    /// <summary>
    /// Sends a request to the API to verify the credentials.
    /// </summary>
    /// <returns>A boolean indicating whether or not the credentials are valid.</returns>
    public async Task<bool> VerifyCredentials() {
        // Create a way to check if the api key given is valid
        // I think the best way to do this is to make a request to the api and see if it returns the expected response.
        var result = await this.GetCoinFactsByGrade(1, "AG-03");  // This is a known invalid coin id - this is because the api is very fast with invalid requests.
        if (result.Item1 == ErrorType.ApiKeyInvalid) return false;
        return true;
    }

    /// <summary>
    /// Gets the coin information for a given PCGS number and grade.
    /// </summary>
    /// <param name="pcgsNumber">The PCGS number of the coin.</param>
    /// <param name="grade">The grade of the coin.</param>
    /// <param name="plusGrade">Whether or not the coin is a plus grade.</param>
    /// <returns>A tuple containing the error type and the coin information.</returns>
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

    /// <summary>
    /// Utility function to parse the grade number from the grade string passed from the front end.
    /// </summary>
    /// <param name="grade">The grade string.</param>
    /// <returns>The grade number.</returns>
    private int GetGradeFromString(string grade) {
        var actualGrade = int.Parse(grade.Substring(grade.Length - 2));  // Throws an exception if the grade doesn't exist.
        Console.WriteLine($"Grade is {actualGrade}");
        return actualGrade;
    }

    /// <summary>
    /// Parses the json response from the api.
    /// </summary>
    /// <param name="json">The json response.</param>
    /// <returns>A tuple containing an error type and the coin information.</returns>
    private (ErrorType, Coin?) ParseJsonResponse(string json) {
        Console.WriteLine(json);
        // Check for errors in the response
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
