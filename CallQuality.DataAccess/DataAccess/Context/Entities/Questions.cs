

namespace CallQuality.Core.DataAccess.Context.Entities;

public class Questions
{
    public int QuestionId { get; set; }

    public string? QuestionValue { get; set; }

    public string? DefaultFeedback { get; set; }

    public virtual ICollection<AssessmentDetail> AssessmentDetails { get; set; } = new List<AssessmentDetail>();

    public virtual ICollection<QuestionInType> QuestionInType { get; set; } = new List<QuestionInType>();
}
