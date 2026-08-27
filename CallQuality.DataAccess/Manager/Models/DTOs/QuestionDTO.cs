

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

using System.ComponentModel.DataAnnotations;

public sealed class QuestionDTO
{
    public int QuestionId { get; set; }

    [Required(ErrorMessage = "Question text is required.")]
    [StringLength(1000, ErrorMessage = "Question text is too long.")]
    public string? QuestionValue { get; set; }

    [StringLength(2000, ErrorMessage = "Default feedback is too long.")]
    public string? DefaultFeedback { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Order number cannot be negative.")]
    public int OrderNumber { get; set; }

    public List<AssessmentDetailDTO> AssessmentDetails { get; set; } = new();

    public List<QuestionInTypeDTO> QuestionInTypes { get; set; } = new();
}