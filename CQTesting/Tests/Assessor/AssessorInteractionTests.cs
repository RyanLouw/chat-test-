using CallQualityUITesting.Pages.Assessor;
using Microsoft.Playwright;

namespace CallQualityUITesting.Tests.Assessor;

[Collection("Assessor Tests")]
public class AssessorInteractionTests
{
    private readonly AssessorBrowserFixture _browser;

    public AssessorInteractionTests(AssessorBrowserFixture browser)
    {
        _browser = browser;
    }

    [Fact]
    public async Task Happy_Assessor_Can_Select_A_Question_Group()
    {
        var page = await OpenFirstInteractionAsync();
        Assert.True(await page.SubgroupChoices.CountAsync() > 0,
            "Expected at least one assessment category.");

        await page.SelectFirstSubgroupAsync();

        Assert.True(await page.Questions.CountAsync() > 0,
            "Expected questions for the selected category.");
    }

    [Fact]
    public async Task Sad_Assessment_Category_Is_Required()
    {
        var page = await OpenFirstInteractionAsync();

        await page.ContinueWithoutSelectionAsync();

        await Assertions.Expect(page.ValidationMessage).ToBeVisibleAsync();
        await Assertions.Expect(page.ValidationMessage)
            .ToContainTextAsync("Please select at least one category");
    }

    [Fact]
    public async Task Neutral_Assessment_Shows_Call_And_Category_Choices()
    {
        var page = await OpenFirstInteractionAsync();

        await Assertions.Expect(page.CallInformationHeading).ToBeVisibleAsync();
        await Assertions.Expect(page.SubgroupModal).ToBeVisibleAsync();
        await Assertions.Expect(page.ContinueButton).ToBeVisibleAsync();
    }

    private async Task<AssessInteractionPage> OpenFirstInteractionAsync()
    {
        var newAssessment = new NewAssessmentPage(_browser.Page, _browser.BaseUrl);
        await newAssessment.GoToAsync();
        Assert.True(await newAssessment.AgentOptions.CountAsync() > 0,
            "Expected an assigned agent.");
        await newAssessment.SelectFirstAgentAsync();
        await newAssessment.SearchButton.ClickAsync();

        var interaction = _browser.Page.Locator(".clickable-row").First;
        await interaction.WaitForAsync(new() { Timeout = 30_000 });
        await interaction.ClickAsync();

        var page = new AssessInteractionPage(_browser.Page);
        await page.CallInformationHeading.WaitForAsync(new() { Timeout = 30_000 });
        return page;
    }
}
