using CallQualityUITesting.Pages.IT;
using Microsoft.Playwright;

namespace CallQualityUITesting.Tests.IT;

[Collection("IT Tests")]
public class OperatorAssignmentsTests
{
    private readonly ITBrowserFixture _browser;

    public OperatorAssignmentsTests(ITBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Happy_IT_Can_View_And_Search_Current_Assignments()
    {
        var page = new OperatorAssignmentsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        await Assertions.Expect(page.Heading).ToBeVisibleAsync();
        Assert.True(await page.AssignmentGroups.CountAsync() > 0,
            "Expected at least one current assignment group.");
        var assessor = (await page.AssignmentGroups.First.InnerTextAsync()).Trim();

        await page.AssignmentSearch.FillAsync(assessor);

        Assert.True(await page.VisibleAssignmentGroups.CountAsync() > 0);
        await Assertions.Expect(page.VisibleAssignmentGroups.First).ToContainTextAsync(assessor);
    }

    [Fact]
    public async Task Sad_Unassigned_Assignment_Requires_An_Assessor()
    {
        var page = new OperatorAssignmentsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        await page.OpenUnassignedAsync();

        await page.AssignUnassignedButton.ClickAsync();

        await Assertions.Expect(page.UnassignedError).ToBeVisibleAsync();
        await Assertions.Expect(page.UnassignedError)
            .ToContainTextAsync("Please select an assessor");
    }

    [Fact]
    public async Task Sad_Bulk_Assignment_Requires_An_Assessor()
    {
        var page = new OperatorAssignmentsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();
        await page.OpenBulkAsync();

        await page.BulkAssignButton.ClickAsync();

        await Assertions.Expect(page.BulkError).ToBeVisibleAsync();
        await Assertions.Expect(page.BulkError)
            .ToContainTextAsync("Please select an assessor");
    }

    [Fact]
    public async Task Neutral_IT_Can_View_All_Assignment_Modes()
    {
        var page = new OperatorAssignmentsPage(_browser.Page, _browser.BaseUrl);
        await page.GoToAsync();

        await Assertions.Expect(page.CurrentTab).ToBeVisibleAsync();
        await Assertions.Expect(page.UnassignedTab).ToBeVisibleAsync();
        await Assertions.Expect(page.BulkTab).ToBeVisibleAsync();

        await page.OpenUnassignedAsync();
        await Assertions.Expect(page.UnassignedAssessor).ToBeVisibleAsync();

        await page.OpenBulkAsync();
        await Assertions.Expect(page.ManagerSelect).ToBeVisibleAsync();
        await Assertions.Expect(page.AssessorSelect).ToBeVisibleAsync();
    }
}
