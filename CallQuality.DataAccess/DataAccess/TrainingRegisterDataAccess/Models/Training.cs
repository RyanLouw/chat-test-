

namespace CallQuality.Core.DataAccess.TrainingRegisterDataAccess.Models;

public sealed class Training
{
    public List<TrainingUserTraining> UserTrainings { get; set; } = new();
    public List<OperatorAssignmentReport> OperatorAssignments { get; set; } = new();
    public List<ExistingRegisterItem> ExistingRegisters { get; set; } = new();
    public List<TrainingRegisterSuggestedGrouped> SuggestedTrainings { get; set; } = new();
}


public class TrainingUserTraining
{
    public Guid UserGuid { get; set; }
    public string UserName { get; set; }
    public string Department { get; set; }
    public string TrainingTopic { get; set; }
    public DateTime? TrainingDate { get; set; }
    public bool? UserSigned { get; set; }
    public string FacilitatorGuid { get; set; }
    public string Facilitator { get; set; }
    public string UploadedBy { get; set; }

}
public class OperatorAssignmentReport
{

    public int AssessorId { get; set; }
    public string PrimaryAssessorName { get; set; }
    public string Operator_ID { get; set; }

}

public class ExistingRegisterItem
{
    public long TrainingRegisterId { get; set; }
    public string TrainingRegisterName { get; set; }
    public string TrainingRegisterDescription { get; set; }
    public string TrainingRegisterTimeDuration { get; set; }
    public DateTime? TrainingRegisterDate { get; set; }
    public DateTime? TrainingRegisterDueDate { get; set; }
    public string Facilitator { get; set; }
    public DateTime CreatedOn { get; set; }
    public string UploadedBy { get; set; }
    public bool? HasTrainingFiles { get; set; }
    public string AssessmentId { get; set; }
    public int? TotalUsers { get; set; }
    public int? TotalSigned { get; set; }
    public int? TotalNotSigned { get; set; }
    public decimal? PercentageSigned { get; set; }
    public Guid? SystemId { get; set; }
}


public class TrainingRegisterSuggestedGrouped
{
    public Guid ID_GUID { get; set; }
    public string AgentName { get; set; }
    public string Extension { get; set; }
    public string Department { get; set; }
    public double AverageScore { get; set; }
    public List<MissedQuestion> MissedQuestions { get; set; } = new();
}

public class MissedQuestion
{
    public string QuestionValue { get; set; }
    public int MissedCount { get; set; }
}
