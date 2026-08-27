using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Interactions;
using CallQuality.Core.Manager.AssessmentsManager.Models;
using System.ComponentModel.DataAnnotations;


namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public sealed class AssessInteractionDTO
{
    public bool IsPsp { get; set; }

    public PSPInteractionsVM? PspInteraction { get; set; }

    public InteractionResult? Interaction { get; set; }

    public List<CallInteractionVM> CallInteraction { get; set; } = new();

    public List<SubGroup> SubGroup { get; set; } = new();

    public string AgentExtension { get; set; } = string.Empty;

    public string AgentName { get; set; } = string.Empty;

    public List<int> SelectedSubGroupId { get; set; } = new();

    public List<QuestionInteraction> SelectedQuestions { get; set; } = new();

    public Dictionary<int, string> Answers { get; set; } = new();

    public int Score { get; set; }

    public int MaxScore { get; set; }

    public double Percentage { get; set; }

    public string AutoFeedback { get; set; } = string.Empty;

    public string AdditionalFeedback { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a call.")]
    public CallInteractionVM? SelectedCall { get; set; }
}

public class SubGroup
{
    public int SubGroupTypeId { get; set; }

    public string SubGroupValue { get; set; } = string.Empty;

    public List<QuestionInteraction> Questions { get; set; } = new();
}

public class QuestionInteraction
{
    public int QuestionId { get; set; }

    public string QuestionValue { get; set; } = string.Empty;

    public string DefaultFeedback { get; set; } = string.Empty;

    public int Score { get; set; }

    public int OrderNumber { get; set; }

    public bool Active { get; set; }
}