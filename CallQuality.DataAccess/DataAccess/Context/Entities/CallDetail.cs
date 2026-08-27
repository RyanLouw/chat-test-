using CallQuality.Core.DataAccess.Context.Entities;

public class CallDetail
{
    private string? _recordingLength;

    public int RowKey { get; set; }

    public string? NumberAssessedOn { get; set; }

    public string? RecordingLength { get; set; }

    public int? RecordingMinutes { get; set; }

    public int? RecordingSeconds { get; set; }

    public string? RecordingUrl { get; set; }

    public int? AssessmentId { get; set; }

    public TimeOnly? RecordingTime { get; set; }

    public bool? DidAssessmentOn { get; set; }

    public DateTime? CallDate { get; set; }

    public long? RecordingId { get; set; }

    public virtual Assessment? Assessment { get; set; }

  
}