using CallQualityUITesting.Pages.Assessor;
using Microsoft.Playwright;

namespace CallQualityUITesting.Tests.Assessor;

[Collection("Assessor Tests")]
public class AssessorReportTests
{
    private readonly AssessorBrowserFixture _browser;

    public AssessorReportTests(AssessorBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Happy_Assessor_Can_Load_Report_With_Data()
    {
        var page = new AssessorReportPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();

        await page.LoadAsync("2026-07");

        Assert.True(await page.Rows.CountAsync() > 0,
            "Expected operator assessment report data for July 2026.");
    }

    [Fact]
    public async Task Sad_Report_Shows_Empty_State_Without_A_Month()
    {
        var page = new AssessorReportPage(_browser.Page, _browser.BaseUrl);

        await page.GoToAsync();

        await Assertions.Expect(page.EmptyState).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Neutral_Assessor_Can_Filter_Loaded_Report()
    {
        var page = new AssessorReportPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        await page.LoadAsync("2026-07");
        var operatorName = (await page.Rows.First.Locator("td").First.InnerTextAsync()).Trim();

        await page.SearchInput.FillAsync(operatorName);

        Assert.True(await page.VisibleRows.CountAsync() > 0);
        await Assertions.Expect(page.VisibleRows.First).ToContainTextAsync(operatorName);
    }
}
