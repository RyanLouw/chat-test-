using System.ComponentModel.DataAnnotations;

namespace CallQuality.Core.Manager.AssessmentsManager.Models;

public class ReassessmentSaveRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "AssessmentId must be greater than 0.")]
    public int AssessmentId { get; set; }

    [Required(ErrorMessage = "Answers are required.")]
    [MinLength(1, ErrorMessage = "At least one answer is required.")]
    public Dictionary<int, string> Answers { get; set; } = new();

    public Dictionary<int, string> Notes { get; set; } = new();
}