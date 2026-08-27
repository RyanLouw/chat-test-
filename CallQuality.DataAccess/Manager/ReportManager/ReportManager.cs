using CallQuality.Core.DataAccess.CallQualityDataAccess.CQ;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Questions;
using CallQuality.Core.Helpers;
using CallQuality.Core.Manager.ReportManager.Models;

using System.Data;

namespace CallQuality.Core.Manager.ReportManager
{
    public class ReportManager : IReportManager
    {
        private readonly ICallQualityDataAccess _db;
        private readonly IUserSession _userSession;

        public ReportManager(ICallQualityDataAccess db, IUserSession userSession)
        {
            _db = db;
            _userSession = userSession;
        }

        public async Task<List<AssessorReportRowVM>> AssessorReportMonthAssessmentAsync(DateTime startDate, string assessor)
        {
            var rows = (await _db.AssessorReportMonthAssessmentAsync(startDate, assessor)).Select(r => new AssessorReportRowVM(r)).ToList();
            return rows;
        }


        public async Task<AssessorListeningTimeVM> GetAssessorListeningTimeAsync(DateTime start, DateTime end)
        {
            var assessors = await _db.GetAssessorsAsync(start, end);

            var model = new AssessorListeningTimeVM
            {
                SelectedMonth = start.ToString("MMMM yyyy"),
                Assessors = new List<AssessorChartData>()
            };

            foreach (var name in assessors)
            {

                var AssessorReportMonthAssessment = await _db.AssessorReportMonthAssessmentAsync(start, name);

                var chartData = new AssessorChartData
                {
                    AssessorName = name,
                    Days = AssessorReportMonthAssessment.Select(r => r.Day).ToList(),
                    Departments = new List<DepartmentSeries>
                            {
                                new() { DepartmentName = "Dischem PRP", Color = "#2b7a78", HoursListened = AssessorReportMonthAssessment.Select(r => r.DischemPRP_TimeListened / 60.0).ToList() },
                                new() { DepartmentName = "Dischem SRS", Color = "#b33939", HoursListened = AssessorReportMonthAssessment.Select(r => r.DischemSRS_TimeListened / 60.0).ToList() },
                                new() { DepartmentName = "PRP", Color = "#cc8e35", HoursListened = AssessorReportMonthAssessment.Select(r => r.PRP_TimeListened / 60.0).ToList() },
                                new() { DepartmentName = "PSP", Color = "#6a1b9a", HoursListened = AssessorReportMonthAssessment.Select(r => r.PSP_TimeListened / 60.0).ToList() },
                                new() { DepartmentName = "AE", Color = "#556b2f", HoursListened = AssessorReportMonthAssessment.Select(r => r.AE_TimeListened / 60.0).ToList() }
                            }
                };

                model.Assessors.Add(chartData);
            }
            return model;
        }



        public async Task<List<AssessorAccuracyReportViewModel>> GetAccuracyReportAsync(DateTime start, DateTime end, string assessorOrReAssessor)
        {

            var assessors = assessorOrReAssessor == "Assessor"
                ? await _db.GetAssessorsAsync(start, end)
                : await _db.GetReassessorsAsync(start, end);

            var model = new List<AssessorAccuracyReportViewModel>();

            foreach (var name in assessors)
            {
                var reportTable = (await _db.GetAccuracyReportAsync(start, end, name)).Select(r => new AssessorAccuracyReportRowVM(r)).ToList();
                var chartData = new AssessorAccuracyReportViewModel
                {
                    AssessorName = name,
                    Scores = reportTable
                };

                model.Add(chartData);
            }
            return model;
        }


        public async Task<List<AssessorTracking_Report>> GetAssessorTrackingReportAsync(DateTime start, DateTime end)
        {
            var list = (await _db.GetAssessorTrackingReportAsync(start, end)).Select(r => new AssessorTracking_Report(r)).ToList();
            return list;
        }
         

        public async Task<List<AssessorBreakdownPercentageDTO>> GetAssessorBreakdown_PercentageAsync(
          DateTime fromDate, DateTime toDate)
        {
            var list = new List<AssessorBreakdownPercentageDTO>();
            var assessors = await _db.GetAssessorsAsync(fromDate, toDate);

            foreach (var assessor in assessors)
            {
                var results = (await _db.AssessorBreakdown_PercentageAsync(fromDate, toDate, assessor)).Select(r => new AssessorBreakdownPercentageResultDTO(r)).ToList();

                var dto = new AssessorBreakdownPercentageDTO
                {
                    AssessorName = assessor,
                    Results = results
                };

                list.Add(dto);
            }

            return list;
        }



        public async Task<ManagerDashDTO> GetManagerHomeOverviewAsync()
        {
            var rawRows = await _db.GetManagerHomeOverviewAsync();

            var allowedDepartments = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
                {
                    "Dischem PRP",
                    "Dischem SRS",
                    "PRP",
                    "PSP",
                    "CRM"
                };

            var departments = rawRows
                .Where(row => !string.IsNullOrWhiteSpace(row.Department))
                .Where(row => allowedDepartments.Contains(row.Department))
                .GroupBy(row => row.Department)
                .Select(group => new DepartmentViewModel(
                    departmentName: group.Key,
                    agents: group.Select(row => new AgentViewModel(row))))
                .ToList();

            

            return new ManagerDashDTO(departments);
        }


        public async Task<List<string>> GetAssessorsAsync(DateTime startDate, DateTime endDate)
        {
            var result = await _db.GetAssessorsAsync(startDate, endDate);
            return result;
        }


        public async Task<ViewAssessmentPageVM?> GetOperatorAssessmentAsync(int assessmentId)
        {
            var assessment = await _db.GetAssessmentViewAsync(assessmentId);

            if (assessment == null)
                return null;

            var feedbackItems = await _db.GetFeedbackForAssessmentAsync(assessmentId);

            return new ViewAssessmentPageVM(
                assessment,
                feedbackItems);
        }


        public async Task<List<OperatorNumberOfAssessmentReportDTO>> GetOperator_NumberOfAssessment_Reports(DateTime? start)
        {
            if (start == null)
                return new List<OperatorNumberOfAssessmentReportDTO>();
            var userid = _userSession.GetCurrentUserId();
            var reports = await _db.GetOperatorNumberOfAssessmentReportsAsync(start.Value, userid);
            return reports.Select(r => new OperatorNumberOfAssessmentReportDTO(r)).ToList();
        }



        public async Task<QuestionWrongStat_ReportVM> GetQuestionWrongStatsReportAsync(string? typeName)
        {
            var today = DateTime.Today;

            var thisMonthStart = new DateTime(today.Year, today.Month, 1);
            var thisMonthEnd = today;

            var lastMonthEnd = thisMonthStart.AddDays(-1);
            var lastMonthStart = new DateTime(lastMonthEnd.Year, lastMonthEnd.Month, 1);

            var thisMonthEntities = await _db.GetQuestionWrongStatsRangeAsync(
                thisMonthStart,
                thisMonthEnd,
                typeName) ?? new List<QuestionWrongStat>();

            var lastMonthEntities = await _db.GetQuestionWrongStatsRangeAsync(
                lastMonthStart,
                lastMonthEnd,
                typeName) ?? new List<QuestionWrongStat>();

            var allThisMonthEntities = await _db.GetQuestionWrongStatsRangeAsync(
                thisMonthStart,
                thisMonthEnd,
                null) ?? new List<QuestionWrongStat>();

            var thisMonth = thisMonthEntities
                .Select(x => new QuestionWrongStatVM(x))
                .ToList();

            var lastMonth = lastMonthEntities
                .Select(x => new QuestionWrongStatVM(x))
                .ToList();

            var allThisMonth = allThisMonthEntities
                .Select(x => new QuestionWrongStatVM(x))
                .ToList();

            var typeNames = allThisMonth
                .Where(x => !string.IsNullOrWhiteSpace(x.TypeName))
                .Select(x => x.TypeName!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            var thisMonthSorted = thisMonth
                .OrderByDescending(x => x.WrongPct)
                .ThenByDescending(x => x.TotalAnswered)
                .ThenByDescending(x => x.TotalWrong)
                .ToList();

            var thisMonthLookup = thisMonth
                .GroupBy(x => BuildQuestionKey(x.TypeName, x.QuestionID))
                .ToDictionary(g => g.Key, g => g.First());

            var lastMonthLookup = lastMonth
                .GroupBy(x => BuildQuestionKey(x.TypeName, x.QuestionID))
                .ToDictionary(g => g.Key, g => g.First());

            var allKeys = thisMonthLookup.Keys
                .Union(lastMonthLookup.Keys)
                .ToList();

            var comparison = allKeys
                .Select(key =>
                {
                    thisMonthLookup.TryGetValue(key, out var thisMonthItem);
                    lastMonthLookup.TryGetValue(key, out var lastMonthItem);

                    return new QuestionWrongComparisonVM(
                        thisMonthItem,
                        lastMonthItem);
                })
                .OrderByDescending(x => x.DeltaWrongPct)
                .ThenByDescending(x => x.ThisWrongPct)
                .ThenByDescending(x => x.ThisTotalAnswered)
                .ToList();

            return new QuestionWrongStat_ReportVM(
                thisMonth: thisMonthSorted,
                lastMonth: lastMonth,
                comparison: comparison,
                typeNames: typeNames,
                selectedTypeName: typeName);
        }

        private static string BuildQuestionKey(string? typeName, int questionId)
        {
            return $"{typeName ?? ""}|{questionId}";
        }

    }
}
