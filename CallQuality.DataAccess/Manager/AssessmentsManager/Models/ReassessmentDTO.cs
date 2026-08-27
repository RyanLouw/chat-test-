
using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.Manager.QuestionsManager.Models;

namespace CallQuality.Core.Manager.AssessmentsManager.Models;

public class ReassessmentDTO
{
    public ReassessmentDTO()
    {
    }

    public ReassessmentDTO(Assessment assessment, string? userEmail)
    {
        AssessmentID = assessment.AssessmentId;
        UserEmail = userEmail;

        Questions = assessment.AssessmentDetails
            .OrderBy(d => d.QuestionId)
            .Select(d => new QuestionAnswerDTO(d))
            .ToList();

        Recordings = assessment.CallDetails
            .Select(c => new RecordingDTO(c, assessment.Extension))
            .ToList();
    }

    public int AssessmentID { get; set; }

    public string? UserEmail { get; set; }

    public List<QuestionAnswerDTO> Questions { get; set; } = new();

    public List<RecordingDTO> Recordings { get; set; } = new();
}

