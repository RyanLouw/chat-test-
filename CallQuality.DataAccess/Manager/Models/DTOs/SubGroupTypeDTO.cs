

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public partial class SubGroupTypeDTO
{
    public int SubGroupTypeId { get; set; }

    public string? SubGroupValue { get; set; }

    public int? AssessmentTypeId { get; set; }

    public  AssessmentTypeDTO? AssessmentType { get; set; }

    public  ICollection<QuestionInTypeDTO> QuestionInType { get; set; } = new List<QuestionInTypeDTO>();
}
