
namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public partial class AssessmentDetailDTO
{
    public int RowKey { get; set; }

    public int? AssessmentId { get; set; }

    public int? QuestionId { get; set; }

    public bool? AssessorAnswer { get; set; }

    public bool? ReassessorAnswer { get; set; }

    public int? Score { get; set; }

    public string? ReassessorNote { get; set; }

    public bool IsNa { get; set; }

    public  AssessmentDTO? Assessment { get; set; }

    public  QuestionDTO? Question { get; set; }
}
