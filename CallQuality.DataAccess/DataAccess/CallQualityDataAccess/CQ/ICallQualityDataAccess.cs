using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;
using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Questions;
using System.Data;


namespace CallQuality.Core.DataAccess.CallQualityDataAccess.CQ;

public interface ICallQualityDataAccess
{
    Task<List<AssessorBreakdownPercentageResult>> AssessorBreakdown_PercentageAsync(DateTime fromDate, DateTime toDate, string assessor);
    Task<List<AssessorReportRow>> AssessorReportMonthAssessmentAsync(DateTime startDate, string assessor);
    Task<bool> CreateNewQuestionAsync(Questions request);
    Task<bool> CreateSubGroupWithQuestionsAsync(SubGroupType request);
    Task DeleteAssignmentAsync(int rowKey, string changedBy);
    Task<List<AssessorAccuracyReportRow>> GetAccuracyReportAsync(DateTime fromDate, DateTime toDate, string assessedBy);
    Task<List<AgentAssignedModel>> GetAgentsAssignedToAssessorAsync(string userGuid);
    Task<List<AssessmentType>> GetAllAssessmentTypesAsync();
    Task<List<Assessor>> GetAllAssessorsAsync();
    Task<List<Questions>> GetAllQuestionsAsync();
    Task<List<ADUser>> GetAllUsersAsync();
    Task<List<UserAdd>> GetAllUsersTraining();
    Task<Assessment?> GetAssessmentForReassessmentAsync(int assessmentId);

    Task<List<AssessmentDateRangeResult>> GetAssessmentsByDateRangeAsync(DateTime startDate, CancellationToken cancellationToken = default);
    Task<int?> GetAssessmentTypeIdByDepartmentAsync(string? department);
    Task<List<AssessmentType>> GetAssessmentTypesAsync();
    Task<Assessment?> GetAssessmentViewAsync(int assessmentId);
    Task<string?> GetAssessorNameFromAssessment(int assessmentID);
    Task<List<string?>> GetAssessorsAsync(DateTime startDate, DateTime endDate);
    Task<List<AssessorTracking_Report>> GetAssessorTrackingReportAsync(DateTime start, DateTime end);
    Task<List<Feedback>> GetFeedbackForAssessmentAsync(int assessmentId);
    Task<List<ManagerHomeOverviewRow>> GetManagerHomeOverviewAsync();
    Task<List<UserAdd>> GetManagersAsync();
    Task<List<Assessment>> GetOperatorAssessmentsAsync(string ext, DateOnly start, DateOnly end);
    Task<List<OperatorAssignment>> GetOperatorAssignmentReportAsync();
    Task<List<OperatorAssignment>> GetOperatorAssignmentsAsync(int? assessorId = null);

    Task<List<Operator_NumberOfAssessment_Report>> GetOperatorNumberOfAssessmentReportsAsync(DateTime month, Guid? assessorIdGuid = null);
    Task<List<TrainingRegister>> GetOperatorQuestionsMissedReportAsync(DateTime startDate, DateTime? endDate = null);
    Task<List<Questions>> GetQuestionWithTypesAsync();
    Task<List<QuestionWrongStat>> GetQuestionWrongStatsRangeAsync(DateTime startDate, DateTime endDate, string? typeName = null);
    Task<List<string?>> GetReassessorsAsync(DateTime startDate, DateTime endDate);
    Task<List<SubGroupType>> GetSubGroupsAndQuestionsAsync(string typeName);
    Task<List<SubGroupType>> GetSubGroupTypesWithQuestionsAsync();
    Task<List<UsersInDepartment>> GetUsersByDepartmentAsync(string department);
    Task<int> SaveAssessmentAsync(Assessment assessment);
   
    Task<int> SaveAssignOperatorsAsync(int assessorId, List<string> operatorIds, string changedBy);
    Task<bool> SaveReassessmentAsync(Assessment request);
  
    Task UpdateAssignmentAsync(int rowKey, int assessorId, int? secondaryAssessorId, DateTime? start, DateTime? end, string changedBy);
    Task<bool> UpdateQuestionAsync(Questions request);
    Task<bool> UpdateQuestionOrderAsync(SubGroupType request);
  
    Task UpdateSecondaryAssignmentAsync(int rowKey, int? secondaryAssessorId, DateTime? start, DateTime? end, string changedBy);
}
