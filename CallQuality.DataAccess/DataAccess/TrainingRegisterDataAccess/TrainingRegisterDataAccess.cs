
using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;
using CallQuality.Core.DataAccess.TrainingRegisterDataAccess;
using CallQuality.Core.DataAccess.TrainingRegisterDataAccess.Models;
using CallQuality.Core.Manager.AssessmentsManager.Models;
using CallQuality.Core.Manager.TrainingManager.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using static CallQuality.Core.DataAccess.ADUsersDataAccess.Models.UserAdd;


namespace CallQuality.DataAccess.DataAccess.TrainingRegisterDataAccess
{
    public class TrainingRegisterDataAccess : ITrainingRegisterDataAccess
    {
        private readonly HttpClient _httpClient;
        private readonly TrainingRegisterApi _config;

        public TrainingRegisterDataAccess(
            HttpClient httpClient,
            IOptions<TrainingRegisterApi> options)
        {
            _httpClient = httpClient;
            _config = options.Value;

   
            if (!string.IsNullOrEmpty(_config.Key) &&
                !_httpClient.DefaultRequestHeaders.Contains("x-api-key"))
            {
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _config.Key);
            }
        }



        public async Task<List<TrainingUserTraining>> GetTrainingRegisterUserDataAsync()
        {
            var url = "CallQuality/GetTrainingRegisterUserData";

            var fullUrl = _httpClient.BaseAddress == null
                ? url
                : new Uri(_httpClient.BaseAddress, url).ToString();

            var values = _httpClient.DefaultRequestHeaders.TryGetValues("x-api-key", out var apiKeyValues)
                ? apiKeyValues.ToList()
                : new List<string>();

            var firstKey = values.FirstOrDefault();

            Log.Information("x-api-key count: {Count}", values.Count);
            Log.Information("x-api-key length: {Length}", firstKey?.Length ?? 0);
            Log.Information("x-api-key starts with [: {StartsWithBracket}", firstKey?.StartsWith("[") ?? false);
            Log.Information("x-api-key ends with ]: {EndsWithBracket}", firstKey?.EndsWith("]") ?? false);

            var response = await _httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Log.Error(
                    "TrainingRegister API failed. Status: {StatusCode}, Reason: {Reason}, Body: {Body}",
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    body);

                return new List<TrainingUserTraining>();
            }

            if (string.IsNullOrWhiteSpace(body))
                return new List<TrainingUserTraining>();

            return JsonConvert.DeserializeObject<List<TrainingUserTraining>>(body)
                   ?? new List<TrainingUserTraining>();
        }



        public async Task<List<ExistingRegisterItem>> GetAllTrainingRegistersAsync()
        {
            var url = $"CallQuality/GetTrainingRegisterData";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<ExistingRegisterItem>();

            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json))
                return new List<ExistingRegisterItem>();

            return JsonConvert.DeserializeObject<List<ExistingRegisterItem>>(json)
                   ?? new List<ExistingRegisterItem>();
        }

        public async Task<List<UserAdd.Group>> GetGroupAsync()
        {
            var url = $"CallQuality/getApplications";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<UserAdd.Group>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<UserAdd.Group>>(json)
                   ?? new List<UserAdd.Group>();
        }


        public async Task<List<string>> GetGroupUsersAsync(string groupId)
        {
            string url = $"CallQuality/getGroupMembers";

            var groupList = new List<string> { groupId };
            string jsonBody = System.Text.Json.JsonSerializer.Serialize(groupList);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetGroupUsersAsync() failed with HTTP status: {StatusCode}", response.StatusCode);
                return new List<string>();
            }

            string json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json))
            {
                Log.Warning("GetGroupUsersAsync() returned empty JSON.");
                return new List<string>();
            }

            var members = System.Text.Json.JsonSerializer.Deserialize<List<Member>>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return members?.Select(m => m.MemberId).ToList() ?? new List<string>();
        }

        public async Task<bool> SaveNewTrainingAsync(NewTrainingRegister trainingRegister, List<UserAddVM> trainees, List<string> fileNames, string uploadedBy, string uploadedByMail)
        {

            var url = $"CallQuality/SaveNewTrainingRegister";
            if (fileNames == null || !fileNames.Any())
            {
                fileNames = new List<string>();
            }

            var requestBody = new
            {
                TrainingRegister = trainingRegister,
                SelectedUsers = trainees,
                FileNames = fileNames,
                UploadedBy = uploadedBy,
                UploadedByMail = uploadedByMail
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Log.Information("Sending Training Register to API: {Url}", url);

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();


            Log.Information("BaseAddress: {Base}", _httpClient.BaseAddress);
            Log.Information("Relative Url: {Url}", url);
            Log.Information("Full Url (string concat): {Full}", $"{_httpClient.BaseAddress}{url}");


            Log.Error("API ERROR BODY: {Body}", responseContent);

            return response.IsSuccessStatusCode;

        }

        public async Task<string> UploadTrainingFile(byte[] fileContent, string fileName, string authToken)
        {
            var url = $"CallQuality/UploadFiles";

            using var content = new MultipartFormDataContent();

            var byteContent = new ByteArrayContent(fileContent);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            content.Add(byteContent, "myFile", fileName);

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Log.Error("Upload failed: {Status} {Content}", response.StatusCode, responseContent);
                return null;
            }

            dynamic obj = JsonConvert.DeserializeObject(responseContent);
            return obj?.fileId;
        }

    }
}