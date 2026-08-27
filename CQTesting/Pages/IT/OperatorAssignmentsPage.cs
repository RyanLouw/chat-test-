using Microsoft.Playwright;

namespace CallQualityUITesting.Pages.IT;

public class OperatorAssignmentsPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public OperatorAssignmentsPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public ILocator Heading =>
        _page.GetByRole(AriaRole.Heading, new() { Name = "Operator Assignments" });
    public ILocator CurrentTab => _page.Locator("#tab-current");
    public ILocator UnassignedTab => _page.Locator("#tab-unassigned");
    public ILocator BulkTab => _page.Locator("#tab-bulk");
    public ILocator AssignmentSearch => _page.Locator("#searchAssignmentsInput");
    public ILocator AssignmentGroups => _page.Locator(".assignment-accordion-item");
    public ILocator VisibleAssignmentGroups =>
        _page.Locator(".assignment-accordion-item:visible");
    public ILocator UnassignedAssessor => _page.Locator("#unassignedAssessorSelect");
    public ILocator UnassignedOperators => _page.Locator(".unassigned-operator-checkbox");
    public ILocator AssignUnassignedButton => _page.Locator("#btnAssignUnassigned");
    public ILocator UnassignedError => _page.Locator("#unassignedError");
    public ILocator ManagerSelect => _page.Locator("#managerSelect");
    public ILocator AssessorSelect => _page.Locator("#assessorSelect");
    public ILocator BulkOperators => _page.Locator(".operator-checkbox");
    public ILocator BulkAssignButton => _page.Locator("#btnAssign");
    public ILocator BulkError => _page.Locator("#bulkError");

    public async Task GoToAsync(string? query = null) =>
        await _page.GotoAsync($"{_baseUrl.TrimEnd('/')}/IT{query ?? string.Empty}");

    public async Task OpenUnassignedAsync() => await UnassignedTab.ClickAsync();

    public async Task OpenBulkAsync() => await BulkTab.ClickAsync();
}
