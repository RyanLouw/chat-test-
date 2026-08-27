using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;


namespace CallQuality.Core.Manager.ReportManager.Models;

public sealed class AssessorBreakdownPercentageDTO
{
    public AssessorBreakdownPercentageDTO()
    {
        Results = new List<AssessorBreakdownPercentageResultDTO>();
    }

    public AssessorBreakdownPercentageDTO(
        string assessorName,
        IEnumerable<AssessorBreakdownPercentageResultDTO> results)
    {
        AssessorName = assessorName;
        Results = results.ToList();
    }

    public string AssessorName { get; set; } = string.Empty;

    public List<AssessorBreakdownPercentageResultDTO> Results { get; set; }
}
public sealed class AssessorBreakdownPercentageResultDTO
{
    public AssessorBreakdownPercentageResultDTO()
    {
    }

    public AssessorBreakdownPercentageResultDTO(AssessorBreakdownPercentageResult entity)
    {
        Date = entity.Date;
        DischemPRP = entity.DischemPRP;
        DischemSRS = entity.DischemSRS;
        PRP = entity.PRP;
        PSP = entity.PSP;
        CRM = entity.CRM;
       
    }

    public DateTime? Date { get; set; }

    public int DischemPRP { get; set; }

    public int DischemSRS { get; set; }

    public int PRP { get; set; }

    public int PSP { get; set; }
    public int CRM { get; set; }

}