using CallQuality.Core.DataAccess.ADUsersDataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CallQuality.Core.Manager.AssessmentsManager.Models;

public class UserAddVM
{
    public UserAddVM()
    {
    }

    public UserAddVM(UserAdd entity)
    {
        UserId = entity.UserId;
        DisplayName = entity.DisplayName;
        GivenName = entity.GivenName;
        Surname = entity.Surname;
        EmailAddress = entity.EmailAddress;
        Department = entity.Department;
        Title = entity.Title;
        EmployeeId = entity.EmployeeId;
        Extension = entity.Extension;
        LeaderId = entity.LeaderId;
    }

    [Required(ErrorMessage = "Facilitator is required")]
    public string? UserId { get; set; }

    public string? DisplayName { get; set; }

    public string? GivenName { get; set; }

    public string? Surname { get; set; }

    public string? EmailAddress { get; set; }

    public string? Department { get; set; }

    public string? Title { get; set; }

    public string? EmployeeId { get; set; }

    [NotMapped]
    public string? Extension { get; set; }

    [NotMapped]
    public string? LeaderId { get; set; }

    public class Group
    {
        public Group()
        {
        }

        public Group(UserAdd.Group entity)
        {
            DisplayName = entity.DisplayName ?? string.Empty;
            GroupId = entity.GroupId ?? string.Empty;
        }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("groupId")]
        public string GroupId { get; set; } = string.Empty;
    }

    public class Member
    {
        public string MemberId { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
    }
}
