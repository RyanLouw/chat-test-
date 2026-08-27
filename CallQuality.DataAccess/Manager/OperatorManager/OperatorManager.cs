using CallQuality.Core.DataAccess.ADUsersDataAccess;
using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;
using CallQuality.Core.DataAccess.CallQualityDataAccess.CQ;
using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.Helpers;
using CallQuality.Core.Manager.Models.CallQualityDTOs;
using CallQuality.Core.Manager.OperatorAssignmentManager.Models;
using CallQuality.DataAccess.CallQualityDataAccess.Models;
using Microsoft.Extensions.Logging;

namespace CallQuality.Core.Manager.OperatorAssignmentManager;

public class OperatorManager : IOperatorManager
{
    private readonly ILogger<OperatorManager> _logger;
    private readonly ICallQualityDataAccess _CallQualityDataAccess;
    private readonly IADUsersDataAccess _ADUsersDataAccess;
    private readonly IUserSession _userSession;

    public OperatorManager(
        ILogger<OperatorManager> logger,
        ICallQualityDataAccess callQualityDataAccess,
        IADUsersDataAccess adUsersDataAccess,
        IUserSession userSession   )
    {
        _logger = logger;
        _CallQualityDataAccess = callQualityDataAccess;
        _ADUsersDataAccess = adUsersDataAccess;
        _userSession = userSession;
    }

    public async Task<AssignmentVM> GetAssignmentAsync(string? managerId)
    {
        var vm = new AssignmentVM();

        var allUsers = await _CallQualityDataAccess.GetAllUsersAsync();
        var managers = await _ADUsersDataAccess.GetManagersAsync();
        var assessors = await _CallQualityDataAccess.GetAllAssessorsAsync();
        var operatorAssignments = await _CallQualityDataAccess.GetOperatorAssignmentsAsync();

        var usersUnderManager = string.IsNullOrWhiteSpace(managerId)
            ? new List<CqUserUnderManagerDto>()
            : await _ADUsersDataAccess.GetUsersUnderManagerAsync(managerId);

        vm.AllUsers = allUsers
            .Select(u => new ADUserDTO(u))
            .ToList();

        vm.Managers = managers
            .Select(x => new CqManagerVM(x))
            .ToList();

        vm.SelectedManagerId = managerId;

        vm.UsersUnderManager = string.IsNullOrWhiteSpace(managerId)
            ? null
            : usersUnderManager
                .Select(x => new CqUserUnderManagerVM(x))
                .ToList();

        vm.Assessors = assessors
            .Select(x => new AssessorsDTO(x))
            .ToList();

        vm.OperatorAssignments = operatorAssignments
            .Select(x => new OperatorAssignmentsDTO(x))
            .ToList();

        var assignedOperatorIds = operatorAssignments
            .Where(x => !string.IsNullOrWhiteSpace(x.OperatorId))
            .Select(x => x.OperatorId.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var availableOperatorsQuery = allUsers
            .Where(x => x.AccountEnabled == true)
            .Where(x => !string.IsNullOrWhiteSpace(x.EmployeeId))
            .Where(x => !string.IsNullOrWhiteSpace(x.ID.ToString()))
            .Where(x => !IsExcludedOperatorDepartment(x.Department))
            .Where(x => !assignedOperatorIds.Contains(x.ID.ToString().Trim()));

        if (!string.IsNullOrWhiteSpace(managerId))
        {
            var usersUnderManagerIds = usersUnderManager
                .Select(x => x.UserID.ToString().Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            availableOperatorsQuery = availableOperatorsQuery
                .Where(x => usersUnderManagerIds.Contains(x.ID.ToString().Trim()));
        }

        vm.AvailableOperators = availableOperatorsQuery
            .OrderBy(x => x.GivenName)
            .ThenBy(x => x.Surname)
            .Select(x => new ADUserDTO(x))
            .ToList();

        return vm;
    }

    private static bool IsExcludedOperatorDepartment(string? department)
    {
        if (string.IsNullOrWhiteSpace(department))
            return false;

        var value = department.Trim();

        if (value.Equals("IT", StringComparison.OrdinalIgnoreCase))
            return true;

        if (value.StartsWith("IT ", StringComparison.OrdinalIgnoreCase))
            return true;

        if (value.StartsWith("IT-", StringComparison.OrdinalIgnoreCase))
            return true;

        if (value.StartsWith("IT_", StringComparison.OrdinalIgnoreCase))
            return true;

        var blockedDepartmentTerms = new[]
        {
        "IT Infrastructure",
        "IT Development",
        "Development",
        "Developer",
        "Legal",
        "Team Leader",
        "Management",
        "Finance"
    };

        return blockedDepartmentTerms.Any(term =>
            value.Contains(term, StringComparison.OrdinalIgnoreCase));
    }




    public async Task<int> SaveAssignOperatorsAsync(int assessorId, List<string> operatorIds)
    {
        var user = _userSession.GetCurrentUserId().ToString();
        return await _CallQualityDataAccess.SaveAssignOperatorsAsync(assessorId, operatorIds, user);
    }



    public Task DeleteAssignmentAsync(int rowKey)
    {
        var user = _userSession.GetCurrentUserId().ToString();
        return  _CallQualityDataAccess.DeleteAssignmentAsync(rowKey, user);
    }


    public  Task UpdateSecondaryAssignmentAsync(int rowKey, int? secondaryAssessorId, DateTime? start, DateTime? end)
    {
        var user = _userSession.GetCurrentUserId().ToString();
        return  _CallQualityDataAccess.UpdateSecondaryAssignmentAsync(rowKey, secondaryAssessorId, start, end, user);
    }



    public Task UpdateAssignmentAsync(
        int rowKey,
        int assessorId,
        int? secondaryAssessorId,
        DateTime? start,
        DateTime? end)
    {
        var user = _userSession.GetCurrentUserId();
        return _CallQualityDataAccess.UpdateAssignmentAsync(
            rowKey,
            assessorId,
            secondaryAssessorId,
            start,
            end, user.ToString());
    }


}
