using CallQuality.Core.DataAccess.Context.Entities;

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public partial class AssessmentTypeDTO
{
    public AssessmentTypeDTO()
    {
    }

    public AssessmentTypeDTO(AssessmentType entity)
    {
        AssessmentTypeId = entity.AssessmentTypeId;
        TypeName = entity.TypeName;
        ShowInFrontend = entity.ShowInFrontend;
    }

    public int AssessmentTypeId { get; set; }

    public string? TypeName { get; set; }

    public bool ShowInFrontend { get; set; }

    public List<AssessmentDTO> Assessments { get; set; } = new();

    public List<SubGroupTypeDTO> SubGroupTypes { get; set; } = new();
}