namespace CallQuality.Core.Helpers;

public interface IUserSession
{
    string? GetCurrentUserAuthToken();
    string? GetCurrentUserEmail();
    Guid? GetCurrentUserId();
    string GetUserName();
    Task<List<string>> GetUserRolesAsync();
}