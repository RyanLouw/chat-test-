namespace CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;

public class AssessorBreakdownPercentageResult
{
    public Nullable<System.DateTime> Date { get; set; }
    public int DischemPRP { get; set; }
    public int DischemSRS { get; set; }
    public int PRP { get; set; }
    public int PSP { get; set; }
    public int CRM { get; set; }

}
