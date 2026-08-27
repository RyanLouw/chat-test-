using CallQuality.Core.DataAccess.Context.Entities;

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public partial class OperatorAssignmentsDTO
{
    public OperatorAssignmentsDTO()
    {
    }

    public OperatorAssignmentsDTO(OperatorAssignment entity)
    {
        RowKey = entity.RowKey;
        AssessorId = entity.AssessorId;
        OperatorId = entity.OperatorId;
        SecondaryStartDate = entity.SecondaryStartDate;
        SecondaryEndDate = entity.SecondaryEndDate;
        AssessorIdSecondary = entity.AssessorIdSecondary;

        Assessor = entity.Assessor == null
            ? null
            : new AssessorsDTO(entity.Assessor);

        AssessorIdSecondaryNavigation = entity.AssessorIdSecondaryNavigation == null
            ? null
            : new AssessorsDTO(entity.AssessorIdSecondaryNavigation);
    }

    public int RowKey { get; set; }

    public int? AssessorId { get; set; }

    public string? OperatorId { get; set; }

    public DateTime? SecondaryStartDate { get; set; }

    public DateTime? SecondaryEndDate { get; set; }

    public int? AssessorIdSecondary { get; set; }

    public virtual AssessorsDTO? Assessor { get; set; }

    public virtual AssessorsDTO? AssessorIdSecondaryNavigation { get; set; }
}