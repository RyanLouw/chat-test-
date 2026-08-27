using Microsoft.Playwright;
using System.Net;

namespace CallQualityUITesting.Tests.IT;

[Collection("IT Tests")]
public class ITControllerTests
{
    private readonly ITBrowserFixture _browser;

    public ITControllerTests(ITBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Happy_Index_Returns_Operator_Assignments()
    {
        var response = await _browser.Context.APIRequest.GetAsync(
            $"{_browser.BaseUrl.TrimEnd('/')}/IT");

        Assert.Equal((int)HttpStatusCode.OK, response.Status);
        Assert.Contains("Operator Assignments", await response.TextAsync());
    }

    [Fact]
    public async Task Sad_BulkAssign_Rejects_A_Null_Request()
    {
        var response = await PostJsonAsync("/IT/BulkAssign", "null");

        Assert.Equal((int)HttpStatusCode.BadRequest, response.Status);
        Assert.Contains("Missing request body", await response.TextAsync());
    }

    [Fact]
    public async Task Sad_BulkAssign_Rejects_An_Empty_Request()
    {
        var response = await PostJsonAsync("/IT/BulkAssign", "{}");

        Assert.Equal((int)HttpStatusCode.BadRequest, response.Status);
        Assert.Contains("Invalid bulk assignment request", await response.TextAsync());
    }

    [Theory]
    [InlineData("/IT/DeleteAssignment")]
    [InlineData("/IT/UpdateSecondaryAssignment")]
    [InlineData("/IT/UpdateAssignment")]
    public async Task Sad_Mutation_Endpoints_Reject_Null_Requests(string path)
    {
        var response = await PostJsonAsync(path, "null");

        Assert.Equal((int)HttpStatusCode.BadRequest, response.Status);
        Assert.Contains("Invalid request", await response.TextAsync());
    }

    [Fact]
    public async Task Neutral_UpdateAssignment_Validates_Date_Order()
    {
        var response = await PostJsonAsync(
            "/IT/UpdateAssignment",
            """{"rowKey":1,"assessorId":1,"secondaryStartDate":"2026-07-31","secondaryEndDate":"2026-07-01"}""");

        Assert.Equal((int)HttpStatusCode.BadRequest, response.Status);
        Assert.Contains("start date cannot be after", await response.TextAsync());
    }

    [Fact]
    public async Task Neutral_UpdateSecondaryAssignment_Validates_Date_Order()
    {
        var response = await PostJsonAsync(
            "/IT/UpdateSecondaryAssignment",
            """{"rowKey":1,"secondaryStartDate":"2026-07-31","secondaryEndDate":"2026-07-01"}""");

        Assert.Equal((int)HttpStatusCode.BadRequest, response.Status);
        Assert.Contains("End date cannot be before", await response.TextAsync());
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
