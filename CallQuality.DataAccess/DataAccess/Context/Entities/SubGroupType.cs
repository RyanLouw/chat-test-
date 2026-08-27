
namespace CallQuality.Core.DataAccess.Context.Entities;

public class SubGroupType
{
    public int SubGroupTypeId { get; set; }

    public string? SubGroupValue { get; set; }

    public int? AssessmentTypeId { get; set; }

    public virtual AssessmentType? AssessmentType { get; set; }

    public virtual ICollection<QuestionInType> QuestionInType { get; set; } = new List<QuestionInType>();
}
