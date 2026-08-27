using Microsoft.Playwright;
using System.Net;
using System.Text.Json;

namespace CallQualityUITesting.Tests.Assessor;

[Collection("Assessor Tests")]
public class AssessorControllerTests
{
    private readonly AssessorBrowserFixture _browser;

    public AssessorControllerTests(AssessorBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Happy_CalculateScore_Returns_A_Score()
    {
        var response = await PostJsonAsync(
            "/Assessor/CalculateScore",
            """{"selectedQuestions":[{"questionId":1,"score":5}],"answers":{"1":"yes"}}""");

        Assert.Equal((int)HttpStatusCode.OK, response.Status);
        var body = await response.JsonAsync();
        Assert.True(body.HasValue);
        Assert.True(body.Value.GetProperty("success").GetBoolean());
        Assert.Contains("5 / 5", body.Value.GetProperty("scoreHtml").GetString());
    }

    [Fact]
    public async Task Happy_SearchInteractions_Returns_An_Assessment_Model()
    {
        await _browser.Page.GotoAsync(
            $"{_browser.BaseUrl.TrimEnd('/')}/Assessor/NewAssessment");
        var option = _browser.Page.Locator("#agentSelect option:not([value=''])").First;
        Assert.True(await option.CountAsync() > 0, "Expected an assigned agent.");
        var agent = await option.EvaluateAsync<string>(
            """option => JSON.stringify({ ID_Guid: option.value, DisplayName: option.dataset.name, Extension: String(option.dataset.ext), Department: option.dataset.dept, MonthlyAssessmentCount: Number(option.dataset.count), StatusColor: option.dataset.color })""");

        var response = await PostJsonAsync("/Assessor/SearchInteractions", agent);

        Assert.Equal((int)HttpStatusCode.OK, response.Status);
        using var model = JsonDocument.Parse(await response.TextAsync());
        Assert.True(
            model.RootElement.TryGetProperty("interactions", out _) ||
            model.RootElement.TryGetProperty("pspInteractions", out _));
    }

    [Fact]
    public async Task Sad_SelectSubGroup_Rejects_Null_Request()
    {
        var response = await PostJsonAsync("/Assessor/SelectSubGroup", "null");

        Assert.Equal((int)HttpStatusCode.BadRequest, response.Status);
        Assert.Contains("Invalid request", await response.TextAsync());
    }

    [Fact]
    public async Task Sad_SelectSubGroup_Rejects_Missing_Assessment_Model()
    {
        var response = await PostJsonAsync(
            "/Assessor/SelectSubGroup",
            """{"fullModel":"","selectedSubGroupId":[1]}""");

        Assert.Equal((int)HttpStatusCode.BadRequest, response.Status);
        Assert.Contains("model was not supplied", await response.TextAsync());
    }

    [Fact]
    public async Task Sad_DownloadCall_Rejects_Missing_Recording_Data()
    {
        var response = await PostJsonAsync("/Assessor/DownloadCall", "null");

        Assert.Equal((int)HttpStatusCode.BadRequest, response.Status);
        Assert.Contains("Invalid recording data", await response.TextAsync());
    }

    [Fact]
    public async Task Sad_SubmitFinal_Rejects_Missing_Assessment()
    {
        var response = await PostJsonAsync("/Assessor/SubmitFinal", "null");

        Assert.Equal((int)HttpStatusCode.BadRequest, response.Status);
        Assert.Contains("could not be deserialized", await response.TextAsync());
    }

    [Fact]
    public async Task Neutral_CalculateScore_Handles_No_Questions()
    {
        var response = await PostJsonAsync(
            "/Assessor/CalculateScore",
            """{"selectedQuestions":[],"answers":{}}""");

        Assert.Equal((int)HttpStatusCode.OK, response.Status);
        Assert.Contains("0 / 0 (0%)", await response.TextAsync());
    }

    private async Task<IAPIResponse> PostJsonAsync(string path, string data) =>
        await _browser.Context.APIRequest.PostAsync(
            $"{_browser.BaseUrl.TrimEnd('/')}{path}",
            new()
            {
                Data = data,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                }
            });
}
