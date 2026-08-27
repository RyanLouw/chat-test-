namespace CallQuality.Core.DataAccess.Context.Entities;

public class AssessmentType
{
    public int AssessmentTypeId { get; set; }

    public string? TypeName { get; set; }

    public bool ShowInFrontend { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual ICollection<SubGroupType> SubGroupTypes { get; set; } = new List<SubGroupType>();
}
