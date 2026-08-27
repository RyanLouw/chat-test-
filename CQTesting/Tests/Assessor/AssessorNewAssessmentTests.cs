using CallQualityUITesting.Pages.Assessor;
using Microsoft.Playwright;

namespace CallQualityUITesting.Tests.Assessor;

[Collection("Assessor Tests")]
public class AssessorNewAssessmentTests
{
    private readonly AssessorBrowserFixture _browser;

    public AssessorNewAssessmentTests(AssessorBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Happy_Assessor_Can_Search_For_Manual_Calls()
    {
        var page = new NewAssessmentPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        Assert.True(await page.AgentOptions.CountAsync() > 0, "Expected an assigned agent.");

        await page.SelectFirstAgentAsync();
        await page.SearchManualCallsAsync();

        Assert.False(
            string.IsNullOrWhiteSpace(await page.ManualResults.InnerTextAsync()),
            "Expected manual call results or the no-calls message.");
    }

    [Fact]
    public async Task Sad_Search_Requires_An_Agent()
    {
        var page = new NewAssessmentPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        string? message = null;
        async void DialogHandler(object? _, IDialog dialog)
        {
            message = dialog.Message;
            await dialog.AcceptAsync();
        }

        _browser.Page.Dialog += DialogHandler;

        try
        {
            await page.SearchButton.ClickAsync();
        }
        finally
        {
            _browser.Page.Dialog -= DialogHandler;
        }

        await Assertions.Expect(page.AgentSelect).ToHaveValueAsync(string.Empty);
        Assert.Equal("Please select an agent first.", message);
    }

    [Fact]
    public async Task Neutral_New_Assessment_Shows_Both_Search_Modes()
    {
        var page = new NewAssessmentPage(_browser.Page, _browser.BaseUrl);

        await page.GoToAsync();

        await Assertions.Expect(page.AgentSelect).ToBeVisibleAsync();
        await Assertions.Expect(page.SearchButton).ToBeVisibleAsync();
        await Assertions.Expect(page.ManualAssessmentButton).ToBeVisibleAsync();
    }
}
