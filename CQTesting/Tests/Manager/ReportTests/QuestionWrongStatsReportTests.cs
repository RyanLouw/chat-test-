using CallQualityUITesting.Pages.Manager.Reports;
using Microsoft.Playwright;

namespace CallQualityUITesting.Tests.Manager.ReportTests;

[Collection("Manager Tests")]
public class QuestionWrongStatsReportTests
{
    private readonly ManagerBrowserFixture _browser;

    public QuestionWrongStatsReportTests(ManagerBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Manager_Can_View_Question_Wrong_Stats()
    {
        var page = new QuestionWrongStatsReportPage(_browser.Page, _browser.BaseUrl);

        await page.GoToAsync();

        await Assertions.Expect(page.Heading).ToBeVisibleAsync();
        await Assertions.Expect(page.TypeSelect).ToBeVisibleAsync();
        await Assertions.Expect(page.ApplyButton).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Manager_Can_Filter_Question_Wrong_Stats_By_Type()
    {
        var page = new QuestionWrongStatsReportPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        var options = await page.TypeSelect.Locator("option").AllAsync();
        Assert.True(options.Count > 1, "Expected at least one assessment type.");
        var type = await options[1].GetAttributeAsync("value");

        await page.TypeSelect.SelectOptionAsync(type!);
        await page.ApplyButton.ClickAsync();

        await Assertions.Expect(page.TypeSelect).ToHaveValueAsync(type!);
    }

    [Fact]
    public async Task Manager_Can_View_Month_Comparison()
    {
        var page = new QuestionWrongStatsReportPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();

        await page.OpenComparisonAsync();

        await Assertions.Expect(page.ComparisonTab).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("active"));
        await Assertions.Expect(page.QuestionSearch).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Manager_Can_Search_Month_Comparison()
    {
        var page = new QuestionWrongStatsReportPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        await page.OpenComparisonAsync();
        Assert.True(await page.ComparisonRows.CountAsync() > 0,
            "Expected comparison data for the report.");
        var question = await page.ComparisonRows.First.GetAttributeAsync("data-question");
        Assert.False(string.IsNullOrWhiteSpace(question));

        await page.QuestionSearch.FillAsync(question!);

        Assert.True(await page.VisibleComparisonRows.CountAsync() > 0);
        await Assertions.Expect(page.VisibleComparisonRows.First).ToHaveAttributeAsync("data-question", question!);
    }
}
