using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;

namespace CallQuality.Core.Manager.OperatorAssignmentManager.Models;

public sealed class CqManagerVM
{
    public CqManagerVM()
    {
    }

    public CqManagerVM(CqManagerDto dto)
    {
        ManagerID = dto.ManagerID;
        ManagerName = dto.ManagerName;
        ManagerEmail = dto.ManagerEmail;
        ManagerExtension = dto.ManagerExtension;
        JobTitle = dto.JobTitle;
    }

    public string ManagerID { get; set; } = "";
    public string? ManagerName { get; set; }
    public string? ManagerEmail { get; set; }
    public string? ManagerExtension { get; set; }
    public string? JobTitle { get; set; }
}


public sealed class CqUserUnderManagerVM
{
    public CqUserUnderManagerVM()
    {
    }

    public CqUserUnderManagerVM(CqUserUnderManagerDto dto)
    {
        ManagerName = dto.ManagerName;
        ManagerID = dto.ManagerID;
        UserName = dto.UserName;
        UserID = dto.UserID;
    }

    public string? ManagerName { get; set; }
    public string? ManagerID { get; set; }
    public string? UserName { get; set; }
    public string? UserID { get; set; }
}