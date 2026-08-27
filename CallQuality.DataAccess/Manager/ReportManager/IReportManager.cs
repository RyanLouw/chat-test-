using CallQuality.Core.Manager.ReportManager.Models;



namespace CallQuality.Core.Manager.ReportManager;

public interface IReportManager
{
    Task<List<AssessorReportRowVM>> AssessorReportMonthAssessmentAsync(DateTime startDate, string assessor);
    Task<List<AssessorAccuracyReportViewModel>> GetAccuracyReportAsync(DateTime start, DateTime end, string assessorOrReAssessor);
    Task<List<AssessorBreakdownPercentageDTO>> GetAssessorBreakdown_PercentageAsync(DateTime fromDate, DateTime toDate);
    Task<AssessorListeningTimeVM> GetAssessorListeningTimeAsync(DateTime start, DateTime end);
    Task<List<string>> GetAssessorsAsync(DateTime startDate, DateTime endDate);
    Task<List<AssessorTracking_Report>> GetAssessorTrackingReportAsync(DateTime start, DateTime end);
    Task<ManagerDashDTO> GetManagerHomeOverviewAsync();
    Task<ViewAssessmentPageVM?> GetOperatorAssessmentAsync(int assessmentId);
    Task<List<OperatorNumberOfAssessmentReportDTO>> GetOperator_NumberOfAssessment_Reports(DateTime? start);
    Task<QuestionWrongStat_ReportVM> GetQuestionWrongStatsReportAsync(string? typeName);
}
