

namespace CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;

public sealed class AssessorTracking_Report
{
    public string Operator { get; set; }
    public string Department { get; set; }
    public string Assessor { get; set; }
    public int NumberOfAssessments { get; set; }
    public double Percentage { get; set; }
}
