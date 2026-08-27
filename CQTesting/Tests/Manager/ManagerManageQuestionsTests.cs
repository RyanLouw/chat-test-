using CallQualityUITesting.Pages.Manager;
using Microsoft.Playwright;

namespace CallQualityUITesting.Tests.Manager;

[Collection("Manager Tests")]
public class ManagerManageQuestionsTests
{
    private readonly ManagerBrowserFixture _browser;

    public ManagerManageQuestionsTests(ManagerBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Manager_Can_View_Questions()
    {
        var page = new ManageQuestionsPage(_browser.Page, _browser.BaseUrl);

        await page.GoToAsync();

        await Assertions.Expect(page.Heading).ToBeVisibleAsync();
        Assert.True(await page.Questions.CountAsync() > 0, "Expected at least one question.");
    }

    [Fact]
    public async Task Manager_Can_Search_Questions()
    {
        var page = new ManageQuestionsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        var question = (await page.Questions.First.InnerTextAsync()).Trim();

        await page.SearchAsync(question);

        await Assertions.Expect(page.Questions.First).ToBeVisibleAsync();
        await Assertions.Expect(page.Questions.First).ToContainTextAsync(question);
    }

    [Fact]
    public async Task Manager_Can_View_Questions_By_Subgroup_Type()
    {
        var page = new ManageQuestionsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();

        await page.OpenByTypeAsync();

        await Assertions.Expect(page.ByTypeTab).ToHaveAttributeAsync("aria-selected", "true");
        await Assertions.Expect(page.SubgroupTypeSelect).ToBeVisibleAsync();
        await Assertions.Expect(page.QuestionsByTypeTable).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Manager_Can_Open_New_Question_Form()
    {
        var page = new ManageQuestionsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();

        await page.OpenNewQuestionAsync();

        await Assertions.Expect(page.NewQuestionModal).ToBeVisibleAsync();
        await Assertions.Expect(page.NewQuestionModal).ToContainTextAsync("Create New Question");
    }
}
