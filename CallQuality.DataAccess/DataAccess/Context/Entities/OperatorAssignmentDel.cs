namespace CallQuality.Core.DataAccess.Context.Entities;

public class OperatorAssignmentDel
{
    public long RowKey { get; set; }

    public int AssessorId { get; set; }

    public string OperatorId { get; set; } = null!;

    public DateTime? SecondaryStartDate { get; set; }

    public DateTime? SecondaryEndDate { get; set; }

    public int? AssessorIdSecondary { get; set; }

    public DateTime ActionDate { get; set; }
}
