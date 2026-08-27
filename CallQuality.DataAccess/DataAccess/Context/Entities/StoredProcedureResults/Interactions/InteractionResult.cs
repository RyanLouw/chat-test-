namespace CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Interactions;

public class InteractionResult
{
    public long? ContactID { get; set; }
    public string? FamilyIdentifier { get; set; }
    public long? OrderID { get; set; }
    public string Extension { get; set; }
    public string? PharmacyGroup { get; set; }
    public string PharmacyName { get; set; }
    public string? Profile { get; set; }
    public string AgentName { get; set; }

    public string? CellNumber { get; set; }
    public string? HomeNumber { get; set; }
    public string? WorkNumber { get; set; }
}
