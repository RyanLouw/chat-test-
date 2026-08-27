using Microsoft.Playwright;

namespace CallQualityUITesting.Pages.Assessor;

public class AssessorReportPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public AssessorReportPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public ILocator Heading =>
        _page.GetByRole(AriaRole.Heading, new() { Name = "Operator Assessments Report" });
    public ILocator MonthInput => _page.Locator("input[name='month']");
    public ILocator LoadButton =>
        _page.GetByRole(AriaRole.Button, new() { NameRegex = new("Load") });
    public ILocator ClearLink => _page.GetByRole(AriaRole.Link, new() { Name = "Clear" });
    public ILocator SearchInput => _page.Locator("#searchInput");
    public ILocator Rows => _page.Locator("#reportTable tbody tr");
    public ILocator VisibleRows => _page.Locator("#reportTable tbody tr:visible");
    public ILocator EmptyState => _page.GetByText("No results yet");

    public async Task GoToAsync() =>
        await _page.GotoAsync($"{_baseUrl.TrimEnd('/')}/Assessor/Report");

    public async Task LoadAsync(string month)
    {
        await MonthInput.FillAsync(month);
        await LoadButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
