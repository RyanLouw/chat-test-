
namespace CallQuality.Core.DataAccess.PSPDataAccess.Models;

public sealed class PSPInteractionsDTO
{
    public required string PspName { get; set; }
    public long? PatientID { get; set; }
    public required string Extension { get; set; }
    public string? ContactPerson { get; set; }
    public string? HWNumber { get; set; }
    public required string AgentName { get; set; }
    public string? CellNumber { get; set; }
}
