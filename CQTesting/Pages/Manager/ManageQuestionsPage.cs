using Microsoft.Playwright;

namespace CallQualityUITesting.Pages.Manager;

public class ManageQuestionsPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public ManageQuestionsPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public ILocator Heading =>
        _page.GetByRole(AriaRole.Heading, new() { Name = "Manage Questions" });

    public ILocator ByQuestionTab => _page.Locator("#by-question-tab");

    public ILocator ByTypeTab => _page.Locator("#by-type-tab");

    public ILocator QuestionSearch => _page.Locator("#searchQuestionInput");

    public ILocator Questions => _page.Locator("#questionsAccordion .accordion-item");

    public ILocator NewQuestionButton => _page.Locator("#btnNewQuestion");

    public ILocator NewQuestionModal => _page.Locator("#createQuestionModal");

    public ILocator SubgroupTypeSelect => _page.Locator("#subGroupTypeSelect");

    public ILocator QuestionsByTypeTable => _page.Locator("#questionsByTypeTable");

    public async Task GoToAsync() =>
        await _page.GotoAsync($"{_baseUrl.TrimEnd('/')}/Manager/ManageQuestions");

    public async Task SearchAsync(string text) => await QuestionSearch.FillAsync(text);

    public async Task OpenByTypeAsync() => await ByTypeTab.ClickAsync();

    public async Task OpenNewQuestionAsync() => await NewQuestionButton.ClickAsync();
}
