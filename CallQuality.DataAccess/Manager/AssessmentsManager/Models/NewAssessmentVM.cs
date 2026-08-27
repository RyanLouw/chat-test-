
using CallQuality.Core.DataAccess.ThreeCXDataAccess.Models;


namespace CallQuality.Core.Manager.AssessmentsManager.Models;

public sealed class NewAssessmentVM
{
    public List<AgentAssignedVM> Agents { get; set; }
    public List<InteractionResultVM> Interactions { get; set; }
    public List<InteractionResultVM> RandomInteractions { get; set; }
    public List<PSPInteractionsVM> PSPInteractions { get; set; }
    public List<PSPInteractionsVM> RandomPSPInteractions { get; set; }
    public List<CallInteractionVM> CallInteractions { get; set; }
    public bool IsManulaAssessment { get; set; }
    public string? SelectedAgentId { get; set; }
}
