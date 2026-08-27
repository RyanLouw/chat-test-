using CallQuality.Middleware.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace CallQuality.Middleware;

public sealed class AuthTokenProvide : IAuthTokenProvide
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string? _token;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AuthTokenProvide(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private static string ComputeHash(string rawData)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
        StringBuilder builder = new();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }

    private async Task<string> AuthenticateAsync()
    {
        string? clientId = _configuration["CentralConfig:ClientId"];
        string correlationId = Guid.NewGuid().ToString();

        // Prepare credentials
        var credentials = new
        {
            ClientId = clientId,
            CorrelationId = correlationId,
            Signature = ComputeHash($"{clientId}.{correlationId}.{_configuration["CentralConfig:AuthSalt"]}"),
        };

        var requestBody = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("security/auth", requestBody);
        response.EnsureSuccessStatusCode();

        var content = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());

        if ((bool?)content?["success"] != true)
        {
            throw new HttpRequestException();
        }

        _token = content["token"]?.ToString() ?? throw new NullReferenceException();
        return _token;
    }

    public async Task<string> GetTokenAsync()
    {
        if (IsTokenValid())
        {
            return _token!;
        }

        await _semaphore.WaitAsync();
        try
        {
            // Double-check after acquiring the lock
            if (IsTokenValid())
            {
                return _token!;
            }

            return await AuthenticateAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private bool IsTokenValid()
    {
        if (_token is not null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(_token);

            return jwtToken.ValidTo > DateTime.Now.AddMinutes(1);
        }

        return false;
    }
}
