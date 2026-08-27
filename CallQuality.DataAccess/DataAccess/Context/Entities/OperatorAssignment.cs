namespace CallQuality.Core.DataAccess.Context.Entities;

public class OperatorAssignment
{
    public int RowKey { get; set; }

    public int? AssessorId { get; set; }

    public string? OperatorId { get; set; }

    public DateTime? SecondaryStartDate { get; set; }

    public DateTime? SecondaryEndDate { get; set; }

    public int? AssessorIdSecondary { get; set; }

    public virtual Assessor? Assessor { get; set; }

    public virtual Assessor? AssessorIdSecondaryNavigation { get; set; }
}