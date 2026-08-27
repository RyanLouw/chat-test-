using CallQuality.Core.DataAccess.PSPDataAccess;
using CallQuality.Core.DataAccess.PSPDataAccess.Models;
using HW.CentralConfig.Package.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http.Headers;
using System.Text;


namespace CallQuality.DataAccess.DataAccess.PSPDataAccess;

public class PSPDataAccess : IPSPDataAccess
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    private readonly ISecurityTokenServiceProvider _securityProvider;

    public PSPDataAccess(
        IOptionsSnapshot<PSPApi> options,
        HttpClient httpClient,
        ISecurityTokenServiceProvider securityProvider )
    {
        var config = options.Get("NormalPSP");

        if (string.IsNullOrWhiteSpace(config.BaseUrl))
        {
            throw new InvalidOperationException(
                "ApiAccess:PSP:BaseUrl is not configured.");
        }

        _baseUrl = config.BaseUrl.TrimEnd('/');
        _httpClient = httpClient;
        _securityProvider = securityProvider;

    }

    public async Task<List<PSPInteractionsDTO>> GetPSPInteractionsAsync(
        DateTime startDate,
        DateTime endDate,
        string extension)
    {
        var url = $"{_baseUrl}/CallQualityInteractions";
        string token = await _securityProvider.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var payload = new
        {
            startDate,
            endDate,
            extention = extension
        };

        var jsonBody = JsonConvert.SerializeObject(payload);

        using var content = new StringContent(
            jsonBody,
            Encoding.UTF8,
            "application/json");

        Log.Information(
            "Calling normal PSP API. Url={Url}, StartDate={StartDate}, Extension={Extension}",
            url,
            startDate.ToString("yyyy-MM-dd"),
            extension);

        using var response = await _httpClient.PostAsync(url, content);

        var responseBody = await response.Content.ReadAsStringAsync();
        Log.Information($"psp  CQ /CallQualityInteractions {response.StatusCode.ToString()}");

        if (!response.IsSuccessStatusCode)
        {
            Log.Error(
                "Normal PSP API call failed. Status={StatusCode}, Reason={Reason}, Response={Response}",
                response.StatusCode,
                response.ReasonPhrase,
                responseBody);

            return [];
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return [];
        }

        return JsonConvert.DeserializeObject<List<PSPInteractionsDTO>>(
                   responseBody)
               ?? [];
    }


    
}