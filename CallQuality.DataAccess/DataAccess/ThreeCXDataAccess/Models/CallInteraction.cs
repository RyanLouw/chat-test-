
namespace CallQuality.Core.DataAccess.ThreeCXDataAccess.Models;

public sealed class CallInteraction
{
    public string? Extension { get; set; }
    public DateTime CallDateTime { get; set; }
    public string? DurationMinutes { get; set; }
    public bool IsAnswered { get; set; }
    public string? Incoming { get; set; }
    public string? RecordingURL { get; set; }
    public string? RecordingID { get; set; }
    public string? CallerNumber { get; set; }
    public string? CallerFullName { get; set; }
}
