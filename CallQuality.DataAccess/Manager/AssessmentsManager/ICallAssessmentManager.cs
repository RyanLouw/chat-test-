using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.Manager.AssessmentsManager.Models;
using CallQuality.Core.Manager.Models.CallQualityDTOs;
using CallQuality.Core.Manager.QuestionsManager.Models;
using System.Security.Claims;


namespace CallQuality.Core.Manager.AssessmentsManager;

public interface ICallAssessmentManager
{
    Task<AssessInteractionVM> BuildAssessInteractionAsync(InteractionResultVM? normal, PSPInteractionsVM? psp);
    Task<AssessInteractionVM> BuildcallInteractionAsync(CallInteractionVM normal);
   // Task<ReAssessmentDTO> GetAssessmentByDateRangeAsync(DateTime startDate);
    Task<ReAssessmentDTO> GetAssessmentByDateRangeAsync(DateTime startDate, CancellationToken cancellationToken = default);
    Task<ADUser?> GetAssessorEmailFromAssessment(int assessmentId);
    Task<string> GetDownloadUrlAsync(string recordingID);
    Task<OperatorAssessmentsVM> GetOperatorAssessmentsAsync(string? ext, string? department, DateOnly? start, DateOnly? end);
    Task<PagedAssessmentVM> GetPagedAssessmentsAsync(DateTime startDate, int possiblePage, int reassessedPage, int pageSize, string? search, string? activeTab);
    Task<ReassessmentDTO> GetReassessAsync(int assessmentId);
    Task<List<SubGroupTypeWithQuestionsDTO>> GetSubGroupsWithQuestionsByIdsAsync(List<int> selectedSubGroupIds);
    Task<NewAssessmentVM> NewAssessment(AgentAssignedVM? agent, bool? ManulalAssessment);
    Task<int> SaveAssessmentAsync(AssessInteractionDTO model, ClaimsPrincipal user);
    Task<bool> SaveReassessmentAsync(ReassessmentSaveRequest request);
    string ScoreFeedback(AssessInteractionVM model);


}
