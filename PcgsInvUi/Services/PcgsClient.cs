using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PcgsInvUi.Models;

namespace PcgsInvUi.Services;

public class PcgsClient
{
    private string _apiUri = "https://api.pcgs.com/publicapi";
    private readonly string _apiKey;

    public PcgsClient(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<Coin> GetCoinFactsByGrade(int pcgsNumber, int grade, bool plusGrade)
    {
        Coin newCoin = null;
        using (HttpClient client = new HttpClient())
        {
            string requestUri =
                $"{_apiUri}/coindetail/GetCoinFactsByGrade/?PCGSNo={pcgsNumber}&GradeNo={grade}&PlusGrade={plusGrade}";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _apiKey);
            HttpResponseMessage response = await client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                newCoin = ParseJsonResponse(jsonResponse);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        return newCoin;
    }
        
    // TODO Will eventually create the other methods that are included in the Public API.

    private Coin ParseJsonResponse(string json)
    {
        Coin newCoin = JsonConvert.DeserializeObject<Coin>(json);
        return newCoin;
    }
}