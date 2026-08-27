using CallQuality.Core.Manager.AssessmentsManager.Models;
using System.ComponentModel.DataAnnotations;
using DataAccessExistingRegisterItem =
    CallQuality.Core.DataAccess.TrainingRegisterDataAccess.Models.ExistingRegisterItem;
using DataAccessOperatorAssignment =
    CallQuality.Core.DataAccess.Context.Entities.OperatorAssignment;
using DataAccessTrainingRegister =
    CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.TrainingRegister;
using DataAccessTrainingUserTraining =
    CallQuality.Core.DataAccess.TrainingRegisterDataAccess.Models.TrainingUserTraining;

namespace CallQuality.Core.Manager.TrainingManager.Models;

public sealed class TrainingVM
{
    public List<TrainingUserTrainingVM> UserTrainings { get; set; } = new();

    public List<OperatorAssignmentReportVM> OperatorAssignments { get; set; } = new();

    public List<ExistingRegisterItemVM> ExistingRegisters { get; set; } = new();

    public List<TrainingRegisterSuggestedGroupedVM> SuggestedTrainings { get; set; } = new();
}

public class TrainingUserTrainingVM
{
    public TrainingUserTrainingVM()
    {
    }

    public TrainingUserTrainingVM(DataAccessTrainingUserTraining entity)
    {
        UserGuid = entity.UserGuid;
        UserName = entity.UserName ?? string.Empty;
        Department = entity.Department ?? string.Empty;
        TrainingTopic = entity.TrainingTopic ?? string.Empty;
        TrainingDate = entity.TrainingDate;
        UserSigned = entity.UserSigned;
        FacilitatorGuid = entity.FacilitatorGuid ?? string.Empty;
        Facilitator = entity.Facilitator ?? string.Empty;
        UploadedBy = entity.UploadedBy ?? string.Empty;
    }

    public Guid UserGuid { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string TrainingTopic { get; set; } = string.Empty;

    public DateTime? TrainingDate { get; set; }

    public bool? UserSigned { get; set; }

    public string FacilitatorGuid { get; set; } = string.Empty;

    public string Facilitator { get; set; } = string.Empty;

    public string UploadedBy { get; set; } = string.Empty;
}

public class OperatorAssignmentReportVM
{
    public OperatorAssignmentReportVM()
    {
    }

    public OperatorAssignmentReportVM(DataAccessOperatorAssignment entity)
    {
        AssessorId = entity.AssessorId ?? 0;

        PrimaryAssessorName = entity.Assessor?.AssessorName ?? string.Empty;

        Operator_ID = entity.OperatorId ?? string.Empty;
    }

    public int AssessorId { get; set; }

    public string PrimaryAssessorName { get; set; } = string.Empty;

    public string Operator_ID { get; set; } = string.Empty;
}

public class ExistingRegisterItemVM
{
    public ExistingRegisterItemVM()
    {
    }

    public ExistingRegisterItemVM(DataAccessExistingRegisterItem entity)
    {
        TrainingRegisterId = entity.TrainingRegisterId;
        TrainingRegisterName = entity.TrainingRegisterName ?? string.Empty;
        TrainingRegisterDescription = entity.TrainingRegisterDescription ?? string.Empty;
        TrainingRegisterTimeDuration = entity.TrainingRegisterTimeDuration ?? string.Empty;
        TrainingRegisterDate = entity.TrainingRegisterDate;
        TrainingRegisterDueDate = entity.TrainingRegisterDueDate;
        Facilitator = entity.Facilitator ?? string.Empty;
        CreatedOn = entity.CreatedOn;
        UploadedBy = entity.UploadedBy ?? string.Empty;
        HasTrainingFiles = entity.HasTrainingFiles;
        AssessmentId = entity.AssessmentId ?? string.Empty;
        TotalUsers = entity.TotalUsers;
        TotalSigned = entity.TotalSigned;
        TotalNotSigned = entity.TotalNotSigned;
        PercentageSigned = entity.PercentageSigned;
        SystemId = entity.SystemId;
    }

    public long TrainingRegisterId { get; set; }

    public string TrainingRegisterName { get; set; } = string.Empty;

    public string TrainingRegisterDescription { get; set; } = string.Empty;

    public string TrainingRegisterTimeDuration { get; set; } = string.Empty;

    public DateTime? TrainingRegisterDate { get; set; }

    public DateTime? TrainingRegisterDueDate { get; set; }

    public string Facilitator { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public string UploadedBy { get; set; } = string.Empty;

    public bool? HasTrainingFiles { get; set; }

    public string AssessmentId { get; set; } = string.Empty;

    public int? TotalUsers { get; set; }

    public int? TotalSigned { get; set; }

    public int? TotalNotSigned { get; set; }

    public decimal? PercentageSigned { get; set; }

    public Guid? SystemId { get; set; }
}

public class TrainingRegisterSuggestedGroupedVM
{
    public TrainingRegisterSuggestedGroupedVM()
    {
    }

    public TrainingRegisterSuggestedGroupedVM(DataAccessTrainingRegister entity)
    {
        ID_GUID = entity.ID_GUID;
        AgentName = entity.AgentName ?? string.Empty;
        Extension = entity.Extension ?? string.Empty;
        Department = entity.Department ?? string.Empty;
        AverageScore = entity.AverageScore;
    }

    public Guid ID_GUID { get; set; }

    public string AgentName { get; set; } = string.Empty;

    public string Extension { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public double AverageScore { get; set; }

    public List<MissedQuestion> MissedQuestions { get; set; } = new();
}

public class MissedQuestion
{
    public MissedQuestion()
    {
    }

    public MissedQuestion(DataAccessTrainingRegister entity)
    {
        QuestionValue = entity.QuestionValue ?? string.Empty;
        MissedCount = entity.MissedCount;
    }

    public string QuestionValue { get; set; } = string.Empty;

    public int MissedCount { get; set; }
}




public class NewTrainingRegister
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

    public UserAddVM trainingFacilitator { get; set; }

    public string trainingFacilitatorMail { get; set; }

    [Required]
    public string FacilitatorSigned { get; set; }

    public Guid? SystemID { get; set; }

    public bool? IsCallQuality { get; set; }

    public NewTrainingRegister()
    {
        this.Description = string.Empty;
      
        this.Name = string.Empty;
        this.selectedTimeDuration = string.Empty;
        this.FacilitatorSigned = string.Empty;
        this.trainingFacilitator = null;
        this.trainingFacilitatorMail = string.Empty;
    }
}