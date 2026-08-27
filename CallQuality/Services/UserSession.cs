using CallQuality.Core.DataAccess.ADUsersDataAccess;
using CallQuality.Core.Helpers;
using CallQuality.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;
using Serilog;
using System.Security.Claims;

namespace CallQuality.Services;

public class UserSession : IUserSession
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IADUsersDataAccess _adUsersDataAccess;

    public UserSession(IHttpContextAccessor httpContextAccessor, IADUsersDataAccess adUsersDataAccess)
    {
        _httpContextAccessor = httpContextAccessor;
        _adUsersDataAccess = adUsersDataAccess;
    }
    private ClaimsPrincipal? GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.Identity?.IsAuthenticated ?? false ? user : null;
    }
    private IEnumerable<Claim> GetCurrentUserInfo()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return Enumerable.Empty<Claim>();
        }

        return user.Claims;
    }

    public Guid? GetCurrentUserId()
    {
        var user = GetCurrentUser();
        if (user == null)
            return null;

        return ClaimsHelpers.GetUserId(user);
    }


    public string GetUserName()
    {
        var claims = GetCurrentUserInfo();

        return claims
            .FirstOrDefault(claim => claim.Type == "name")
            ?.Value
            ?? "Unknown";
    }

    public async Task<List<string>> GetUserRolesAsync()
    {
        return  await _adUsersDataAccess.GetUserRolesAsync(GetCurrentUserId());
    }


    public string? GetCurrentUserEmail()
    {
        var user = GetCurrentUser();

        if (user == null)
            return null;

        return user.FindFirst(ClaimTypes.Email)?.Value
            ?? user.FindFirst("preferred_username")?.Value
            ?? user.FindFirst("upn")?.Value
            ?? user.Identity?.Name;
    }

    public string? GetCurrentUserAuthToken()
    {
        var user = GetCurrentUser();

        if (user == null)
            return null;

        return user.FindFirst("oid")?.Value
            ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
    }

}
