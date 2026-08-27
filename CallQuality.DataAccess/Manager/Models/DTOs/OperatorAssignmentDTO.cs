

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public partial class OperatorAssignmentDTO
{
    public int RowKey { get; set; }

    public int? AssessorId { get; set; }

    public string? OperatorId { get; set; }

    public DateTime? SecondaryStartDate { get; set; }

    public DateTime? SecondaryEndDate { get; set; }

    public int? AssessorIdSecondary { get; set; }

    public virtual AssessorDTO? Assessor { get; set; }

    public virtual AssessorDTO? AssessorIdSecondaryNavigation { get; set; }
}
