using CallQuality.Core.Manager.OperatorAssignmentManager.Models;


namespace CallQuality.Core.Manager.OperatorAssignmentManager;

public interface IOperatorManager
{
    Task<AssignmentVM> GetAssignmentAsync(string? managerId);
    Task<int> SaveAssignOperatorsAsync(int assessorId, List<string> operatorIds);
    Task DeleteAssignmentAsync(int rowKey);
    Task UpdateSecondaryAssignmentAsync(int rowKey, int? secondaryAssessorId, DateTime? start, DateTime? end);
    Task UpdateAssignmentAsync(int rowKey, int assessorId, int? secondaryAssessorId, DateTime? start, DateTime? end);
}
