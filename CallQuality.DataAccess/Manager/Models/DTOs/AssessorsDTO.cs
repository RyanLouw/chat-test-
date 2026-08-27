using CallQuality.Core.DataAccess.Context.Entities;

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public partial class AssessorsDTO
{
    public AssessorsDTO()
    {
    }

    public AssessorsDTO(Assessor entity)
    {
        AssessorId = entity.AssessorId;
        AssessorName = entity.AssessorName;
    }

    public int AssessorId { get; set; }

    public string? AssessorName { get; set; }

    public virtual ICollection<AssessmentsDTO> Assessments { get; set; } = new List<AssessmentsDTO>();

    public virtual ICollection<OperatorAssignmentsDTO> OperatorAssignmentsAssessor { get; set; } = new List<OperatorAssignmentsDTO>();

    public virtual ICollection<OperatorAssignmentsDTO> OperatorAssignmentsAssessorIdSecondaryNavigation { get; set; } = new List<OperatorAssignmentsDTO>();
}