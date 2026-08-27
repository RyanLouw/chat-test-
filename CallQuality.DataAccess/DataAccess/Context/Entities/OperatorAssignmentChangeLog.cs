
namespace CallQuality.Core.DataAccess.Context.Entities;

public sealed class OperatorAssignmentChangeLog
{
    public long LogId { get; set; }

    public int? OperatorAssignmentRowKey { get; set; }
    public string? OperatorId { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public int? OldAssessorId { get; set; }
    public int? NewAssessorId { get; set; }

    public int? OldAssessorIdSecondary { get; set; }
    public int? NewAssessorIdSecondary { get; set; }

    public DateTime? OldSecondaryStartDate { get; set; }
    public DateTime? NewSecondaryStartDate { get; set; }

    public DateTime? OldSecondaryEndDate { get; set; }
    public DateTime? NewSecondaryEndDate { get; set; }

    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedOn { get; set; }

    public string? ChangeSummary { get; set; }
}
