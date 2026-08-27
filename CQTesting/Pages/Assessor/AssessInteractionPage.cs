using Microsoft.Playwright;

namespace CallQualityUITesting.Pages.Assessor;

public class AssessInteractionPage
{
    private readonly IPage _page;

    public AssessInteractionPage(IPage page)
    {
        _page = page;
    }

    public ILocator CallInformationHeading =>
        _page.GetByRole(AriaRole.Heading, new() { Name = "Call Information" });
    public ILocator SubgroupModal => _page.Locator("#subGroupModal");
    public ILocator SubgroupChoices => _page.Locator(".subgroup-checkbox");
    public ILocator ContinueButton =>
        SubgroupModal.GetByRole(AriaRole.Button, new() { Name = "Continue" });
    public ILocator ValidationMessage => _page.Locator("#subGroupValidation");
    public ILocator Questions => _page.Locator("#questionsForm .question-item");

    public async Task ContinueWithoutSelectionAsync() => await ContinueButton.ClickAsync();

    public async Task SelectFirstSubgroupAsync()
    {
        await SubgroupChoices.First.CheckAsync();
        await ContinueButton.ClickAsync();
        await Questions.First.WaitForAsync();
    }
}
