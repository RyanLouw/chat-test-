

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public partial class CallDetailDTO
{
    public int RowKey { get; set; }

    public string? NumberAssessedOn { get; set; }

    public string? RecordingLength { get; set; }

    public string? RecordingUrl { get; set; }

    public int? AssessmentId { get; set; }

    public TimeOnly? RecordingTime { get; set; }

    public bool? DidAssessmentOn { get; set; }

    public virtual AssessmentDTO? Assessment { get; set; }

    public int recordingId { get; set; }
    public DateTime CallDate { get; set; }
}
