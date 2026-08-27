using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;
using CallQuality.Core.DataAccess.Context;
using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data;

namespace CallQuality.Core.DataAccess.ADUsersDataAccess;

public class ADUsersDataAccess : IADUsersDataAccess
{
    private readonly ADUsersDbContext _context;

    private readonly Guid _managerRole;
    private readonly Guid _assessorRole;
    private readonly Guid _itRole;

    public ADUsersDataAccess(
        ADUsersDbContext context,
        IConfiguration configuration)
    {
        _context = context;

        _managerRole = configuration.GetRequiredGuid("Roles:Manager");
        _assessorRole = configuration.GetRequiredGuid("Roles:Assessor");
        _itRole = configuration.GetRequiredGuid("Roles:IT");
    }

    public async Task<List<string>> GetUserRolesAsync(Guid? userId)
    {
        var roles = new List<string>();

        Log.Information("Resolved menu/display userId: {UserId}", userId);

        if (userId is null)
            return roles;

        if (await IsManagerAsync(userId.Value))
            roles.Add("Manager");

        if (await IsAssessorAsync(userId.Value))
            roles.Add("Assessor");

        if (await IsItAsync(userId.Value))
            roles.Add("IT");

        return roles;
    }

    public Task<bool> IsManagerAsync(Guid userId, CancellationToken ct = default)
    {
        return IsUserInRoleAsync(userId, _managerRole, ct);
    }

    public Task<bool> IsAssessorAsync(Guid userId, CancellationToken ct = default)
    {
        return IsUserInRoleAsync(userId, _assessorRole, ct);
    }

    public Task<bool> IsItAsync(Guid userId, CancellationToken ct = default)
    {
        return IsUserInRoleAsync(userId, _itRole, ct);
    }

    public async Task<bool> IsAssessorOrManagerAsync(Guid userId, CancellationToken ct = default)
    {
        if (await IsAssessorAsync(userId, ct))
            return true;

        if (await IsManagerAsync(userId, ct))
            return true;

        return false;
    }

    public async Task<bool> IsUserInRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var connectionString = _context.Database.GetConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Missing ADUsers database connection string.");

        await using var conn = new SqlConnection(connectionString);

        await using var cmd = new SqlCommand("App_IsUserInRole", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier).Value = userId;
        cmd.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier).Value = roleId;

        await conn.OpenAsync(ct);

        var result = await cmd.ExecuteScalarAsync(ct);

        return result is not null && Convert.ToBoolean(result);
    }

    public async Task<List<CqUserUnderManagerDto>> GetUsersUnderManagerAsync(string managerId)
    {
        const string sql = "EXEC dbo.CQ_GetUsersUnderManager @ManagerId";

        var pManagerId = new SqlParameter("@ManagerId", managerId);

        return await _context.Database
            .SqlQueryRaw<CqUserUnderManagerDto>(sql, pManagerId)
            .ToListAsync();
    }

    public async Task<List<CqManagerDto>> GetManagersAsync()
    {
        const string sql = "EXEC dbo.CQ_GetManagers";

        return await _context.Database
            .SqlQueryRaw<CqManagerDto>(sql)
            .ToListAsync();
    }

    public async Task<string> GetUserEmailByExtensionAsync(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return string.Empty;

        const string sql = "EXEC dbo.CQ_GetUserEmailByExtension @Extension";

        var pExtension = new SqlParameter("@Extension", extension.Trim());

        var results = await _context.Database
            .SqlQueryRaw<string>(sql, pExtension)
            .ToListAsync();

        return results.FirstOrDefault() ?? string.Empty;
    }

    public async Task<ADUser?> GetAdUserByExtensionAsync(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return null;

        var cleanExtension = extension.Trim();

        return await _context.ADUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Extension == cleanExtension);
    }


    public async Task<ADUser> GetUserByName(string name)
    {
        return await _context.ADUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DisplayName == name);
    }





}