
using CallQuality.Core.DataAccess.Context.Entities;

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public partial class QuestionsDTO
{
    public QuestionsDTO()
    {
    }

    public QuestionsDTO(Questions entity)
    {
        QuestionId = entity.QuestionId;
        QuestionValue = entity.QuestionValue;
        DefaultFeedback = entity.DefaultFeedback;
    }

    public int QuestionId { get; set; }

    public string? QuestionValue { get; set; }

    public string? DefaultFeedback { get; set; }

    public int Score { get; set; }

    public int OrderNumber { get; set; }

    public bool Active { get; set; }

    public List<AssessmentDetailsDTO> AssessmentDetails { get; set; } = [];

    public List<QuestionInTypeDTO> QuestionInType { get; set; } = [];
}