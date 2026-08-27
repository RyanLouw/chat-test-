using CallQuality.Core.DataAccess.Context.Entities;

namespace CallQuality.DataAccess.CallQualityDataAccess.Models;

public partial class ADUserDTO
{
    public ADUserDTO()
    {
    }

    public ADUserDTO(ADUser entity)
    {
        ADUserID = entity.ADUserID;
        ID = entity.ID;
        ID_Guid = entity.ID_Guid;
        DisplayName = entity.DisplayName;
        GivenName = entity.GivenName;
        Mail = entity.Mail;
        Surname = entity.Surname;
        UserPrincipalName = entity.UserPrincipalName;
        EmployeeId = entity.EmployeeId;
        Department = entity.Department;
        JobTitle = entity.JobTitle;
        Extension = entity.Extension;
        AccountEnabled = entity.AccountEnabled;
        Manager_ID = entity.Manager_ID;
    }

    public long ADUserID { get; set; }

    public string? ID { get; set; }

    public Guid? ID_Guid { get; set; }

    public string? DisplayName { get; set; }

    public string? GivenName { get; set; }

    public string? Mail { get; set; }

    public string? Surname { get; set; }

    public string? UserPrincipalName { get; set; }

    public string? EmployeeId { get; set; }

    public string? Department { get; set; }

    public string? JobTitle { get; set; }

    public string? Extension { get; set; }

    public bool? AccountEnabled { get; set; }

    public string? Manager_ID { get; set; }
}