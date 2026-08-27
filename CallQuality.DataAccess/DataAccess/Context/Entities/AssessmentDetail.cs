
namespace CallQuality.Core.DataAccess.Context.Entities;

public class AssessmentDetail
{
    public int RowKey { get; set; }

    public int? AssessmentId { get; set; }

    public int? QuestionId { get; set; }

    public bool? AssessorAnswer { get; set; }

    public bool? ReassessorAnswer { get; set; }

    public int? Score { get; set; }

    public string? ReassessorNote { get; set; }

    public bool IsNa { get; set; }

    public virtual Assessment? Assessment { get; set; }

    public virtual Questions? Question { get; set; }
}