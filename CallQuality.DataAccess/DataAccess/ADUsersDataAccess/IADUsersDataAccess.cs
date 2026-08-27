using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;
using CallQuality.Core.DataAccess.Context.Entities;

namespace CallQuality.Core.DataAccess.ADUsersDataAccess;

public interface IADUsersDataAccess
{
    Task<List<string>> GetUserRolesAsync(Guid? userId);

    Task<bool> IsUserInRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default);

    Task<bool> IsManagerAsync(Guid userId, CancellationToken ct = default);
    Task<bool> IsAssessorAsync(Guid userId, CancellationToken ct = default);
    Task<bool> IsItAsync(Guid userId, CancellationToken ct = default);
    Task<bool> IsAssessorOrManagerAsync(Guid userId, CancellationToken ct = default);

    Task<List<CqUserUnderManagerDto>> GetUsersUnderManagerAsync(string managerId);
    Task<List<CqManagerDto>> GetManagersAsync();

    Task<string> GetUserEmailByExtensionAsync(string extension);
    Task<ADUser?> GetAdUserByExtensionAsync(string? extension);
    Task<ADUser?> GetUserByName(string userId);
}