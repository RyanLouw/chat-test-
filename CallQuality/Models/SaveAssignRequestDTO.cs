using System.ComponentModel.DataAnnotations;

namespace CallQuality.Models;

public class SaveAssignRequestDTO
{

    [Range(1, int.MaxValue, ErrorMessage = "AssessorId is required.")]
    public int AssessorId { get; set; }

    [Required(ErrorMessage = "Select at least one operator.")]
    [MinLength(1, ErrorMessage = "Select at least one operator.")]
    public List<string> OperatorIds { get; set; } = new();
}

public sealed class DeleteAssignmentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "RowKey required.")]
    public int RowKey { get; set; }
}



public sealed class UpdateSecondaryAssignmentRequest : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "RowKey required.")]
    public int RowKey { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Invalid secondary assessor.")]
    public int? AssessorIdSecondary { get; set; }

    public DateTime? SecondaryStartDate { get; set; }

    public DateTime? SecondaryEndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SecondaryStartDate.HasValue &&
            SecondaryEndDate.HasValue &&
            SecondaryEndDate.Value.Date < SecondaryStartDate.Value.Date)
        {
            yield return new ValidationResult(
                "End date cannot be before start date.",
                new[]
                {
                    nameof(SecondaryStartDate),
                    nameof(SecondaryEndDate)
                });
        }
    }
}


public sealed class UpdateAssignmentRequest : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Invalid assignment row.")]
    public int RowKey { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select an assessor.")]
    public int AssessorId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Invalid secondary assessor.")]
    public int? AssessorIdSecondary { get; set; }

    public DateTime? SecondaryStartDate { get; set; }

    public DateTime? SecondaryEndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AssessorIdSecondary.HasValue &&
            AssessorIdSecondary.Value == AssessorId)
        {
            yield return new ValidationResult(
                "Secondary assessor cannot be the same as the primary assessor.",
                new[] { nameof(AssessorIdSecondary) });
        }

        if (SecondaryStartDate.HasValue &&
            SecondaryEndDate.HasValue &&
            SecondaryStartDate.Value.Date > SecondaryEndDate.Value.Date)
        {
            yield return new ValidationResult(
                "Secondary start date cannot be after secondary end date.",
                new[] { nameof(SecondaryStartDate), nameof(SecondaryEndDate) });
        }
    }
}