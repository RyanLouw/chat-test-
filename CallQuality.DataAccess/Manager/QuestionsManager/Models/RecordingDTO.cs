namespace CallQuality.Core.Manager.QuestionsManager.Models;

using CallQuality.Core.DataAccess.Context.Entities;

public class RecordingDTO
{
    public RecordingDTO()
    {
    }

    public RecordingDTO(CallDetail call, string? extension)
    {
        RecordingID = (int)call.RecordingId;
        NumberAssessedOn = call.NumberAssessedOn;
        RecordingLength = call.RecordingLength;
        RecordingURL = call.RecordingUrl;

        RecordingTime = call.RecordingTime.HasValue
            ? call.RecordingTime.Value.ToString("HH:mm:ss")
            : null;


        CallDate = call.CallDate;
        Extension = extension;
    }

    public int RecordingID { get; set; }

    public string? NumberAssessedOn { get; set; }

    public string? RecordingLength { get; set; }

    public string? RecordingURL { get; set; }

    public string? RecordingTime { get; set; }

    public DateTime? CallDate { get; set; }

    public string? Extension { get; set; }
}