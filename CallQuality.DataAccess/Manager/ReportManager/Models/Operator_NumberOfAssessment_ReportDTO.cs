
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults;

namespace CallQuality.Core.Manager.ReportManager.Models;

public class OperatorNumberOfAssessmentReportDTO
{
    public OperatorNumberOfAssessmentReportDTO()
    {
    }

    public OperatorNumberOfAssessmentReportDTO(Operator_NumberOfAssessment_Report entity)
    {
      
        Operator = entity.Operator ?? string.Empty;
        Department = entity.Department ?? string.Empty;
        NumberOfAssessment = entity.NumberOfAssessment;
        Percentage = entity.Percentage;
    }



    public string Operator { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public int NumberOfAssessment { get; set; }

    public double? Percentage { get; set; }
}