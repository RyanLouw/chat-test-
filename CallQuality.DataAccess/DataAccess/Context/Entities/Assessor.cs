namespace CallQuality.Core.DataAccess.Context.Entities;

public class Assessor
{
    public int AssessorId { get; set; }

    public string? AssessorName { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual ICollection<OperatorAssignment> OperatorAssignmentsAssessor { get; set; } = new List<OperatorAssignment>();

    public virtual ICollection<OperatorAssignment> OperatorAssignmentsAssessorIdSecondaryNavigation { get; set; } = new List<OperatorAssignment>();
}