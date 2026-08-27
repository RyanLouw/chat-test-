

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public partial class AssessorDTO
{
    public int AssessorId { get; set; }

    public string? AssessorName { get; set; }

    public virtual ICollection<AssessmentDTO> Assessments { get; set; } = new List<AssessmentDTO>();

    public virtual ICollection<OperatorAssignmentDTO> OperatorAssignmentAssessorIdSecondaryNavigations { get; set; } = new List<OperatorAssignmentDTO>();

    public virtual ICollection<OperatorAssignmentDTO> OperatorAssignmentAssessors { get; set; } = new List<OperatorAssignmentDTO>();
}
