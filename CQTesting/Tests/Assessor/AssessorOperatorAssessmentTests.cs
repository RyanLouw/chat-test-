using CallQualityUITesting.Pages.Assessor;
using Microsoft.Playwright;

namespace CallQualityUITesting.Tests.Assessor;

[Collection("Assessor Tests")]
public class AssessorOperatorAssessmentTests
{
    private readonly AssessorBrowserFixture _browser;

    public AssessorOperatorAssessmentTests(AssessorBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Happy_Assessor_Can_Search_Operator_Assessments()
    {
        var page = new OperatorAssessmentsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();

        await page.SearchAsync("2026-07-01", "2026-07-31");

        await Assertions.Expect(page.Table).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Sad_Table_Search_Hides_Non_Matching_Assessments()
    {
        var page = new OperatorAssessmentsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync("?start=2026-07-01&end=2026-07-31");
        Assert.True(await page.AssessmentRows.CountAsync() > 0,
            "Expected assessments for July 2026.");

        await page.TableSearch.FillAsync($"not-found-{Guid.NewGuid()}");

        await Assertions.Expect(page.VisibleAssessmentRows).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task Neutral_Assessor_Can_Open_Assessment_Details()
    {
        var page = new OperatorAssessmentsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync("?start=2026-07-01&end=2026-07-31");
        Assert.True(await page.AssessmentRows.CountAsync() > 0,
            "Expected an assessment to open.");

        await page.AssessmentRows.First.ClickAsync();
        var details = new ViewOperatorAssessmentPage(_browser.Page);

        await Assertions.Expect(details.Heading).ToBeVisibleAsync();
        await Assertions.Expect(details.QuestionsHeading).ToBeVisibleAsync();
        Assert.True(await details.Questions.CountAsync() > 0);
        await details.OpenRecordingsAsync();
        await Assertions.Expect(details.RecordingsModal).ToBeVisibleAsync();
    }
}
