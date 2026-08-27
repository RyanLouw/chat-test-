
namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public sealed class AssessmentsDTO
{
    public int AssessmentId { get; set; }

    public long? ContactId { get; set; }

    public string? FamilyIdentifier { get; set; }

    public long? PatientId { get; set; }

    public long? ScriptId { get; set; }

    public string? SystemName { get; set; }

    public long? InteractionId { get; set; }

    public string? Extension { get; set; }

    public string? PharmacyGroup { get; set; }

    public string? PharmacyName { get; set; }

    public int? AssessmentScore { get; set; }

    public string? AssessedBy { get; set; }

    public DateTime? AssessedOn { get; set; }

    public bool? IsReassessed { get; set; }

    public int? ReassessmentScore { get; set; }

    public string? ReassessedBy { get; set; }

    public DateTime? ReassessedOn { get; set; }

    public int? AssessmentTypeId { get; set; }

    public long? OrderId { get; set; }

    public string? ContactPerson { get; set; }

    public string? AeId { get; set; }

    public string? AeSystemType { get; set; }

    public bool? CorrectlyIdentified { get; set; }

    public int? AssessedById { get; set; }

    public string? OperatorId { get; set; }

    public string? ProfileNumber { get; set; }

    public DateTime? OrderMadeOn { get; set; }

    public bool? IsManualAssessment { get; set; }

    public string? OrderPharmacyName { get; set; }

    public  AssessorsDTO? AssessedByNavigation { get; set; }

    public ICollection<AssessmentDetailsDTO> AssessmentDetails { get; set; } = new List<AssessmentDetailsDTO>();

    public AssessmentTypeDTO? AssessmentType { get; set; }

    public ICollection<CallDetailDTO> CallDetails { get; set; } = new List<CallDetailDTO>();

    public  ICollection<FeedbackDTO> Feedback { get; set; } = new List<FeedbackDTO>();
}
