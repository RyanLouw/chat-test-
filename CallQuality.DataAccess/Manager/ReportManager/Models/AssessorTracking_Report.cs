namespace CallQuality.Core.Manager.ReportManager.Models;

public sealed class AssessorTracking_Report
{
    public AssessorTracking_Report()
    {
    }

    public AssessorTracking_Report(
        CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments.AssessorTracking_Report row)
    {
        Operator = row.Operator;
        Department = row.Department;
        Assessor = row.Assessor;
        NumberOfAssessments = row.NumberOfAssessments;
        Percentage = row.Percentage;
    }

    public string Operator { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Assessor { get; set; } = string.Empty;

    public int NumberOfAssessments { get; set; }

    public double Percentage { get; set; }
}