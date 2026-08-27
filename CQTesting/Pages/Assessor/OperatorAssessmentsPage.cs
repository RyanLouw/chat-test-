using Microsoft.Playwright;

namespace CallQualityUITesting.Pages.Assessor;

public class OperatorAssessmentsPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public OperatorAssessmentsPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public ILocator DepartmentSelect => _page.Locator("#departmentSelect");
    public ILocator AgentSelect => _page.Locator("#agentSelect");
    public ILocator StartDate => _page.Locator("#startDate");
    public ILocator EndDate => _page.Locator("#endDate");
    public ILocator SearchButton => _page.Locator("#searchBtn");
    public ILocator TableSearch => _page.Locator("#tableSearch");
    public ILocator Table => _page.Locator("#operatorTable");
    public ILocator AssessmentRows => _page.Locator("#operatorTable tbody tr.clickable-row");
    public ILocator VisibleAssessmentRows =>
        _page.Locator("#operatorTable tbody tr.clickable-row:visible");

    public async Task GoToAsync(string? query = null) =>
        await _page.GotoAsync(
            $"{_baseUrl.TrimEnd('/')}/Assessor/OperatorAssessment{query ?? string.Empty}");

    public async Task SearchAsync(string start, string end)
    {
        await StartDate.FillAsync(start);
        await EndDate.FillAsync(end);
        await SearchButton.ClickAsync();
        await _page.WaitForFunctionAsync("() => getComputedStyle(document.querySelector('#loadingSpinner')).display === 'none'");
    }
}
