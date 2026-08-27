using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;


namespace CallQuality.Core.Manager.ReportManager.Models
{
    public sealed class AssessorAccuracyReportViewModel
    {
        public string AssessorName { get; set; }
        public List<AssessorAccuracyReportRowVM> Scores { get; set; } = new();

        public double AverageAssessmentScore =>
            Scores.Any() ? Scores.Average(r => r.AssessmentScore) : 0;

        public double AverageReassessmentScore =>
            Scores.Any() ? Scores.Average(r => r.ReassessmentScore) : 0;
    }
    public class AssessorAccuracyReportRowVM
    {
        public AssessorAccuracyReportRowVM()
        {
        }

        public AssessorAccuracyReportRowVM(AssessorAccuracyReportRow row)
        {
            AssessmentId = row.AssessmentId;
            AssessmentScore = row.AssessmentScore;
            ReassessmentScore = row.ReassessmentScore;
        }

        public long AssessmentId { get; set; }

        public double AssessmentScore { get; set; }

        public double ReassessmentScore { get; set; }
    }

}






