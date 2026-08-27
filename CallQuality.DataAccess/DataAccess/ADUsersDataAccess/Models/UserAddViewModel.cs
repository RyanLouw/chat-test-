using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CallQuality.Core.DataAccess.ADUsersDataAccess.Models;

public class UserAdd
{
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
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("groupId")]
        public string GroupId { get; set; }
    }

    public class Member
    {
        public string MemberId { get; set; }
        public string DisplayName { get; set; }
    }


    public class NewTrainingRegisterDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime? TrainingDate { get; set; }

        [Required]
        public DateTime? TrainingDueDate { get; set; }

        public bool AddNewAssessment { get; set; }

        [Required]
        public string selectedTimeDuration { get; set; }

        public UserAdd trainingFacilitator { get; set; }

        public string trainingFacilitatorMail { get; set; }

        [Required]
        public string FacilitatorSigned { get; set; }

        public Guid? SystemID { get; set; }

        public bool? IsCallQuality { get; set; }

        public NewTrainingRegisterDTO()
        {
            this.Description = string.Empty;
            this.AddNewAssessment = false;
            this.Name = string.Empty;
            this.selectedTimeDuration = string.Empty;
            this.FacilitatorSigned = string.Empty;
            this.trainingFacilitator = null;
            this.trainingFacilitatorMail = string.Empty;
        }
    }
}
