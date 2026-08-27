namespace CallQuality.Core.DataAccess.Context.Entities;

public class Feedback
{
    public int FeedbackId { get; set; }

    public string? FeedbackText { get; set; }

    public string? FeedbackSendBy { get; set; }

    public DateTime? FeedbackSendOn { get; set; }

    public string? AssessmentsIncluded { get; set; }

    public string? FeedbackSendTo { get; set; }

    public virtual ICollection<Assessment> Assessment { get; set; } = new List<Assessment>();
}
