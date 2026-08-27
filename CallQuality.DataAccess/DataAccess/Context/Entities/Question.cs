
namespace CallQuality.Core.DataAccess.Context.Entities;

public  class Question
{
    public int QuestionId { get; set; }

    public string? QuestionValue { get; set; }

    public string? DefaultFeedback { get; set; }

    public virtual ICollection<AssessmentDetail> AssessmentDetails { get; set; } = new List<AssessmentDetail>();

    public virtual ICollection<QuestionInType> QuestionInTypes { get; set; } = new List<QuestionInType>();
}
