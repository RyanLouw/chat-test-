using Microsoft.Playwright;

namespace CallQualityUITesting.Pages.Manager.Reports;

public class AssessorBreakdownReportPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public AssessorBreakdownReportPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public ILocator FromDate => _page.Locator("#fromDate");
    public ILocator ToDate => _page.Locator("#toDate");
    public ILocator RetrieveButton => _page.Locator("#btnRetrieve");
    public ILocator ExportButton => _page.Locator("#btnExportAll");
    public ILocator SearchInput => _page.Locator("#searchAssessor");
    public ILocator AssessorCards => _page.Locator(".assessor-breakdown-wrapper");
    public ILocator VisibleAssessorCards =>
        _page.Locator(".assessor-breakdown-wrapper:visible");

    public async Task GoToAsync() =>
        await _page.GotoAsync($"{_baseUrl.TrimEnd('/')}/Manager/Report_AssessorBreakdown");

    public async Task RetrieveAsync(string from, string to)
    {
        await FromDate.FillAsync(from);
        await ToDate.FillAsync(to);
        await RetrieveButton.ClickAsync();
        await _page.WaitForFunctionAsync(
            "() => !document.querySelector('#assessorBreakdownContainer')?.innerText.includes('Loading...')");
    }

    public async Task<IDownload> ExportAsync()
    {
        var downloadTask = _page.WaitForDownloadAsync();
        await ExportButton.ClickAsync();
        return await downloadTask;
    }
}
