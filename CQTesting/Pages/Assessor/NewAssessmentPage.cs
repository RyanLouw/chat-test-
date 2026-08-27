using Microsoft.Playwright;

namespace CallQualityUITesting.Pages.Assessor;

public class NewAssessmentPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public NewAssessmentPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public ILocator AgentSelect => _page.Locator("#agentSelect");
    public ILocator AgentOptions => AgentSelect.Locator("option:not([value=''])");
    public ILocator SearchButton => _page.Locator("#btnSearchInteractions");
    public ILocator ManualAssessmentButton => _page.Locator("#btnManualAssessment");
    public ILocator Results => _page.Locator("#randomInteractionsSection, #allInteractionsSection");
    public ILocator ManualResults => _page.Locator("#allInteractionsSection");

    public async Task GoToAsync() =>
        await _page.GotoAsync($"{_baseUrl.TrimEnd('/')}/Assessor/NewAssessment");

    public async Task SelectFirstAgentAsync()
    {
        var value = await AgentOptions.First.GetAttributeAsync("value");
        await AgentSelect.SelectOptionAsync(value!);
    }

    public async Task SearchManualCallsAsync()
    {
        await ManualAssessmentButton.ClickAsync();
        await _page.WaitForFunctionAsync(
            "() => document.querySelector('#allInteractionsSection')?.innerText.trim().length > 0");
    }
}
