using CallQualityUITesting.Pages.Manager.Reports;
using Microsoft.Playwright;

namespace CallQualityUITesting.Tests.Manager.ReportTests;

[Collection("Manager Tests")]
public class AssessorBreakdownReportTests
{
    private readonly ManagerBrowserFixture _browser;

    public AssessorBreakdownReportTests(ManagerBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Manager_Can_Retrieve_Assessor_Breakdown()
    {
        var page = new AssessorBreakdownReportPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();

        await page.RetrieveAsync("2026-07-01", "2026-07-31");

        Assert.True(await page.AssessorCards.CountAsync() > 0,
            "Expected assessor breakdown data for July 2026.");
    }

    [Fact]
    public async Task Manager_Can_Search_Assessor_Breakdown()
    {
        var page = new AssessorBreakdownReportPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        await page.RetrieveAsync("2026-07-01", "2026-07-31");
        var assessorName = (await page.AssessorCards.First
            .Locator(".assessor-breakdown-header").InnerTextAsync()).Trim();

        await page.SearchInput.FillAsync(assessorName);

        Assert.True(await page.VisibleAssessorCards.CountAsync() > 0);
        await Assertions.Expect(page.VisibleAssessorCards.First).ToContainTextAsync(assessorName);
    }

    [Fact]
    public async Task Manager_Can_Export_Assessor_Breakdown()
    {
        var page = new AssessorBreakdownReportPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        await page.FromDate.FillAsync("2026-07-01");
        await page.ToDate.FillAsync("2026-07-31");

        var download = await page.ExportAsync();

        Assert.EndsWith(".xlsx", download.SuggestedFilename);
    }
}
