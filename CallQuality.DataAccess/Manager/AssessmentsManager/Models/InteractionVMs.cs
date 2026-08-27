using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Interactions;
using CallQuality.Core.DataAccess.PSPDataAccess.Models;
using CallQuality.Core.DataAccess.ThreeCXDataAccess.Models;
using CallQuality.Core.Manager.Models.CallQualityDTOs;
using System.ComponentModel.DataAnnotations;

namespace CallQuality.Core.Manager.AssessmentsManager.Models;

public class InteractionResultVM
{
    public InteractionResultVM()
    {
    }

    public InteractionResultVM(InteractionResult result)
    {
        ContactID = result.ContactID;
        FamilyIdentifier = result.FamilyIdentifier;
        OrderID = result.OrderID;
        Extension = result.Extension;
        PharmacyGroup = result.PharmacyGroup;
        PharmacyName = result.PharmacyName;
        Profile = result.Profile;
        AgentName = result.AgentName;
        CellNumber = result.CellNumber;
        HomeNumber = result.HomeNumber;
        WorkNumber = result.WorkNumber;
    }

    public long? ContactID { get; set; }
    public string? FamilyIdentifier { get; set; }
    public long? OrderID { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string? PharmacyGroup { get; set; }
    public string PharmacyName { get; set; } = string.Empty;
    public string? Profile { get; set; }
    public string AgentName { get; set; } = string.Empty;

    public string? CellNumber { get; set; }
    public string? HomeNumber { get; set; }
    public string? WorkNumber { get; set; }
}

public sealed class PSPInteractionsVM
{

    public PSPInteractionsVM()
    {
    }

    public PSPInteractionsVM(PSPInteractionsDTO dto)
    {
        PspName = dto.PspName ?? string.Empty;
        PatientID = dto.PatientID;
        Extension = dto.Extension ?? string.Empty;
        ContactPerson = dto.ContactPerson;
        HWNumber = dto.HWNumber;
        AgentName = dto.AgentName ?? string.Empty;
        CellNumber = dto.CellNumber;
    }

    public string PspName { get; set; } = string.Empty;
    public long? PatientID { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? HWNumber { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string? CellNumber { get; set; }
}

public sealed class CallInteractionVM
{
    public CallInteractionVM()
    {
    }

    public CallInteractionVM(CallInteraction entity)
    {
        Extension = entity.Extension;
        CallDateTime = entity.CallDateTime;
        DurationMinutes = entity.DurationMinutes;
        IsAnswered = entity.IsAnswered;
        Incoming = entity.Incoming;
        RecordingURL = entity.RecordingURL;
        RecordingID = entity.RecordingID;
        CallerNumber = entity.CallerNumber;
        CallerFullName = entity.CallerFullName;
    }

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




public sealed class AssessInteractionVM
{
    public bool IsPsp { get; set; }

    public PSPInteractionsVM PspInteraction { get; set; }
    public InteractionResultVM Interaction { get; set; }

    public List<CallInteractionVM> CallInteraction { get; set; } = new();
    public List<SubGroupVM> SubGroup { get; set; } = new();
    public string AgentExtension { get; set; }
    public string AgentName { get; set; }

    // Question group
    public List<int> SelectedSubGroupId { get; set; } = [];
    public List<QuestionInteractionVM> SelectedQuestions { get; set; } = new();

    // Answers keyed by QuestionId
    public Dictionary<int, string> Answers { get; set; } = new();

    // ======= Scoring =======
    public int Score { get; set; } = 0;
    public int MaxScore { get; set; } = 0;
    public double Percentage { get; set; } = 0;

    // ======= Feedback =======
    public string AutoFeedback { get; set; } = "";
    public string AdditionalFeedback { get; set; } = "";

    [Required(ErrorMessage = "Please select a call.")]
    public CallInteraction? SelectedCall { get; set; }
}

public class SubGroupVM
{
    public SubGroupVM()
    {
    }

    public SubGroupVM(SubGroupType entity)
    {
        SubGroupTypeId = entity.SubGroupTypeId;
        SubGroupValue = entity.SubGroupValue ?? string.Empty;

        Questions = entity.QuestionInType
            .Where(qit => qit.Question != null)
            .OrderBy(qit => qit.OrderNumber)
            .Select(qit => new QuestionInteractionVM(qit))
            .ToList();
    }

    public int SubGroupTypeId { get; set; }

    public string SubGroupValue { get; set; } = string.Empty;

    public List<QuestionInteractionVM> Questions { get; set; } = new();
}

public class QuestionInteractionVM
{
    public QuestionInteractionVM()
    {
    }

    public QuestionInteractionVM(QuestionInType entity)
    {
        QuestionId = entity.QuestionId ?? 0;
        QuestionValue = entity.Question?.QuestionValue ?? string.Empty;
        DefaultFeedback = entity.Question?.DefaultFeedback ?? string.Empty;
        Score = entity.Score ?? 0;
        OrderNumber = entity.OrderNumber;
        Active = entity.Active ?? false;
    }
    public int QuestionId { get; set; }
    public string QuestionValue { get; set; }
    public string DefaultFeedback { get; set; }

    public int Score { get; set; }
    public int OrderNumber { get; set; }

    public bool Active { get; set; }
}