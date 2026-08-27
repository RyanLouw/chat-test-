using CallQualityUITesting.Models;
using CallQualityUITesting.Pages;
using CallQualityUITesting.SystemUnderTest;
using Microsoft.Playwright;

namespace CallQualityUITesting.Tests.IT;

[CollectionDefinition("IT Tests")]
public class ITTestCollection : ICollectionFixture<ITBrowserFixture>
{
}

public sealed class ITBrowserFixture : IAsyncLifetime
{
    private readonly CallQualityServiceSut _sut;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public IBrowserContext Context { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;
    public string BaseUrl => _sut.BaseUrl;

    public ITBrowserFixture(CallQualityServiceSut sut)
    {
        _sut = sut;
    }

    public async ValueTask InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 300
        });
        Context = await _browser.NewContextAsync();
        Page = await Context.NewPageAsync();

        var loginPage = new LoginPage(Page, _sut.BaseUrl);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(TestConfiguration.Users.IT);

        await Page.GetByRole(
            AriaRole.Heading,
            new() { Name = "Operator Assignments" })
            .WaitForAsync(new() { Timeout = 120_000 });
    }

    public async ValueTask DisposeAsync()
    {
        if (Context is not null)
            await Context.DisposeAsync();
        if (_browser is not null)
            await _browser.DisposeAsync();
        _playwright?.Dispose();
    }
}
