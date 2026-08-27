using Microsoft.Playwright;

namespace CallQualityUITesting.Pages.Assessor;

public class ViewOperatorAssessmentPage
{
    private readonly IPage _page;

    public ViewOperatorAssessmentPage(IPage page)
    {
        _page = page;
    }

    public ILocator Heading =>
        _page.GetByRole(AriaRole.Heading, new() { Name = "Assessment Details" });
    public ILocator QuestionsHeading =>
        _page.GetByRole(AriaRole.Heading, new() { Name = "Questions & Answers" });
    public ILocator Questions => _page.Locator("#questionsTable tbody tr");
    public ILocator RecordingsButton =>
        _page.GetByRole(AriaRole.Button, new() { Name = "View Recordings" });
    public ILocator RecordingsModal => _page.Locator("#RecordingsModal");

    public async Task OpenRecordingsAsync() => await RecordingsButton.ClickAsync();
}
