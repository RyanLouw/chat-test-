namespace CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults;

public class ManagerHomeOverviewRow
{
    public string Department { get; set; } = string.Empty;
    public string DisplayName { get; set; } = null!;
    public int AssessmnetsDone { get; set; }
}
