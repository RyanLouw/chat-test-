namespace CallQuality.Core.DataAccess.ADUsersDataAccess.Models;

public sealed class CqManagerDto
{
    public string ManagerID { get; set; } = "";
    public string? ManagerName { get; set; }
    public string? ManagerEmail { get; set; }
    public string? ManagerExtension { get; set; }
    public string? JobTitle { get; set; }
}
