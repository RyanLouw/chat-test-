

using CallQuality.Core.Manager.AssessmentsManager.Models;
using System.ComponentModel.DataAnnotations;

namespace CallQuality.Models;

public class SelectGroupRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Selected subgroup is required.")]
    public List<int> SelectedSubGroupId { get; set; }

    [Required(ErrorMessage = "Full model is required.")]
    public string FullModel { get; set; } = string.Empty;
}

public class RecordingRequest
{
    public string RecordingId { get; set; }
}

public sealed class AssessInteraction
{
    public bool IsPsp { get; set; }

    public PSPInteractionsVM PspInteraction { get; set; }
    public InteractionResultVM Interaction { get; set; }

    public List<CallInteractionVM> CallInteraction { get; set; } = new();
    public List<SubGroupVM> SubGroup { get; set; } = new();
    public string AgentExtension { get; set; }
    public string AgentName { get; set; }

    // Question group
    public List<int> SelectedSubGroupIds { get; set; } = [];
    public List<QuestionInteractionVM> SelectedQuestions { get; set; } = new();

    // Answers keyed by QuestionId
    public Dictionary<int, string> Answers { get; set; } = new();

    // ======= Scoring =======
    public int Score { get; set; } = 0;
    public int MaxScore { get; set; } = 0;
    public int Percentage { get; set; } = 0;

    // ======= Feedback =======
    public string AutoFeedback { get; set; } = "";
    public string AdditionalFeedback { get; set; } = "";

    [Required(ErrorMessage = "Please select a call.")]
    public CallInteractionVM? SelectedCall { get; set; }

}



