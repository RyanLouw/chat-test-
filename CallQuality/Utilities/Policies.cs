using CallQuality.Core.DataAccess.ADUsersDataAccess;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CallQuality.Utilities;

public sealed class ManagerRequirement : IAuthorizationRequirement { }

public sealed class AssessorRequirement : IAuthorizationRequirement { }

public sealed class ItRequirement : IAuthorizationRequirement { }

public sealed class AssessorOrManagerRequirement : IAuthorizationRequirement { }

public static class ClaimsHelpers
{
    public static Guid? GetUserId(ClaimsPrincipal user)
    {
        var idValue =
            user.FindFirst("oid")?.Value ??
            user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value ??
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            user.FindFirst("sub")?.Value;

        return Guid.TryParse(idValue, out var id) ? id : null;
    }

    public static string GetUserName(ClaimsPrincipal user)
    {
        var name =
            user.FindFirst("name")?.Value ??
            user.FindFirst(ClaimTypes.Name)?.Value ??
            user.FindFirst("preferred_username")?.Value ??
            user.FindFirst("upn")?.Value ??
            user.FindFirst(ClaimTypes.Email)?.Value ??
            user.Identity?.Name ??
            "Unknown";

        return FormatDisplayName(name);
    }

    private static string FormatDisplayName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Unknown";

        var displayName = name;

        if (displayName.Contains("@"))
            displayName = displayName.Split('@')[0];

        if (displayName.Contains("\\"))
            displayName = displayName.Split('\\').Last();

        displayName = displayName.Replace(".", " ");

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(displayName.ToLower());
    }
}

public sealed class ManagerHandler : AuthorizationHandler<ManagerRequirement>
{
    private readonly IADUsersDataAccess _roles;

    public ManagerHandler(IADUsersDataAccess roles)
    {
        _roles = roles;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ManagerRequirement requirement)
    {
        var userId = ClaimsHelpers.GetUserId(context.User);

        if (userId is not Guid id)
            return;

        if (await _roles.IsManagerAsync(id))
            context.Succeed(requirement);
    }
}

public sealed class AssessorHandler : AuthorizationHandler<AssessorRequirement>
{
    private readonly IADUsersDataAccess _roles;

    public AssessorHandler(IADUsersDataAccess roles)
    {
        _roles = roles;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AssessorRequirement requirement)
    {
        var userId = ClaimsHelpers.GetUserId(context.User);

        if (userId is not Guid id)
            return;

        if (await _roles.IsAssessorAsync(id))
            context.Succeed(requirement);
    }
}

public sealed class ItHandler : AuthorizationHandler<ItRequirement>
{
    private readonly IADUsersDataAccess _roles;

    public ItHandler(IADUsersDataAccess roles)
    {
        _roles = roles;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ItRequirement requirement)
    {
        var userId = ClaimsHelpers.GetUserId(context.User);

        if (userId is not Guid id)
            return;

        if (await _roles.IsItAsync(id))
            context.Succeed(requirement);
    }
}

public sealed class AssessorOrManagerHandler : AuthorizationHandler<AssessorOrManagerRequirement>
{
    private readonly IADUsersDataAccess _roles;

    public AssessorOrManagerHandler(IADUsersDataAccess roles)
    {
        _roles = roles;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AssessorOrManagerRequirement requirement)
    {
        var userId = ClaimsHelpers.GetUserId(context.User);

        if (userId is not Guid id)
            return;

        if (await _roles.IsAssessorOrManagerAsync(id))
            context.Succeed(requirement);
    }
}