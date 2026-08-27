using Microsoft.Playwright;

namespace CallQualityUITesting.Pages.Manager.Reports;

public class QuestionWrongStatsReportPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public QuestionWrongStatsReportPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public ILocator Heading =>
        _page.GetByRole(AriaRole.Heading, new() { Name = "Question Wrong Stats" });
    public ILocator ThisMonthTab =>
        _page.Locator("#wrongStatsTabs button", new() { HasText = "This month" });
    public ILocator ComparisonTab =>
        _page.Locator("#wrongStatsTabs button", new() { HasText = "Compare vs last month" });
    public ILocator TypeSelect => _page.Locator("select[name='type']");
    public ILocator ApplyButton =>
        _page.GetByRole(AriaRole.Button, new() { Name = "Apply" });
    public ILocator ThisMonthRows => _page.Locator("#this tbody tr");
    public ILocator QuestionSearch => _page.Locator("#questionSearch");
    public ILocator ComparisonRows => _page.Locator(".comparison-row");
    public ILocator VisibleComparisonRows => _page.Locator(".comparison-row:visible");
    public ILocator ComparisonChart => _page.Locator("#wrongPctChart");

    public async Task GoToAsync() =>
        await _page.GotoAsync($"{_baseUrl.TrimEnd('/')}/Manager/QuestionWrongStatsReport");

    public async Task OpenComparisonAsync() => await ComparisonTab.ClickAsync();
}
