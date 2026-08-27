
using CallQuality.Core.DataAccess.PSPDataAccess.Models;
using CallQuality.Core.DataAccess.ThreeCXDataAccess;
using CallQuality.Core.DataAccess.ThreeCXDataAccess.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;


namespace CallQuality.DataAccess.DataAccess.ThreeCXDataAccess
{
    public class ThreeCXDataAccess : IThreeCXDataAccess
    {
        private readonly HttpClient _http;

        public ThreeCXDataAccess(IOptions<PSPApi> options, HttpClient http)
        {
            _http = http;
        }


        public async Task<List<CallInteraction>> LookupByExtensionAsync(string extension, DateTime date)
        {
            string callType = "Both";
            var payload = new
            {
                extension,
                callType,
                date = date.ToString("yyyy-MM-ddT00:00:00")
            };

            using var res = await _http.PostAsJsonAsync("by-extension", payload);
            if (!res.IsSuccessStatusCode)
                return new List<CallInteraction>();

            return await res.Content.ReadFromJsonAsync<List<CallInteraction>>()
                   ?? new List<CallInteraction>();
        }


        public async Task<string?> GetDownloadUrlAsync(string recordingId)
        {
            if (string.IsNullOrWhiteSpace(recordingId))
                return null;

            using var res = await _http.GetAsync($"call-url/{Uri.EscapeDataString(recordingId)}");
            if (!res.IsSuccessStatusCode)
                return null;

            var text = await res.Content.ReadAsStringAsync();
            return text?.Trim().Trim('"');
        }


    }
}
