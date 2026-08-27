using CallQuality.Core.DataAccess.Context.Entities;


namespace CallQuality.Core.Manager.ReportManager.Models
{
    public sealed class ViewAssessmentPageVM
    {
        public ViewAssessmentPageVM()
        {
            Assessment = new AssessmentHeaderDTO();
        }

        public ViewAssessmentPageVM(
            Assessment assessment,
            IEnumerable<Feedback> feedbackItems)
        {
            Assessment = new AssessmentHeaderDTO(assessment);

            Questions = assessment.AssessmentDetails
                .Select(d => new QuestionAnswerDTO(d))
                .ToList();

            Recordings = assessment.CallDetails
                .Select(c => new RecordingDTO(c, assessment.Extension))
                .ToList();

            Feedback = feedbackItems
                .Select(f => new FeedbackDTO(f))
                .ToList();
        }

        public AssessmentHeaderDTO Assessment { get; set; }

        public List<QuestionAnswerDTO> Questions { get; set; } = new();

        public List<FeedbackDTO> Feedback { get; set; } = new();

        public List<RecordingDTO> Recordings { get; set; } = new();
    }



    public class AssessmentHeaderDTO
    {
        public AssessmentHeaderDTO()
        {
        }

        public AssessmentHeaderDTO(Assessment assessment)
        {
            AssessmentID = assessment.AssessmentId;
            ContactID = assessment.ContactId;
            FamilyIdentifier = assessment.FamilyIdentifier ?? string.Empty;
            PatientID = assessment.PatientId;
            ScriptID = assessment.ScriptId;
            SystemName = assessment.SystemName ?? string.Empty;
            InteractionID = assessment.InteractionId;
            Extension = assessment.Extension ?? string.Empty;

            PharmacyGroup = assessment.PharmacyGroup ?? string.Empty;
            PharmacyName = assessment.PharmacyName ?? string.Empty;
            OrderPharmacyName = assessment.OrderPharmacyName ?? string.Empty;

            AssessmentScore = assessment.AssessmentScore;
            AssessedOn = assessment.AssessedOn;
            AssessedBy = assessment.AssessedBy ?? string.Empty;

            IsReassessed = assessment.IsReassessed ?? false;
            ReassessmentScore = assessment.ReassessmentScore;
            ReassessedOn = assessment.ReassessedOn;
            ReassessedBy = assessment.ReassessedBy ?? string.Empty;

            ContactPerson = assessment.ContactPerson ?? string.Empty;

            ProfileNumber = int.TryParse(assessment.ProfileNumber, out var profileNumber)
                ? profileNumber
                : null;

            IsManualAssessment = assessment.IsManualAssessment;

            PossibleScore = assessment.AssessmentDetails
                .Where(x => x.IsNa == false)
                .Sum(x => x.Score ?? 0);
        }

        public long? AssessmentID { get; set; }

        public long? ContactID { get; set; }

        public string FamilyIdentifier { get; set; } = string.Empty;

        public long? PatientID { get; set; }

        public long? ScriptID { get; set; }

        public string SystemName { get; set; } = string.Empty;

        public long? InteractionID { get; set; }

        public string Extension { get; set; } = string.Empty;

        public string PharmacyGroup { get; set; } = string.Empty;

        public string PharmacyName { get; set; } = string.Empty;

        public string OrderPharmacyName { get; set; } = string.Empty;

        public int? AssessmentScore { get; set; }

        public DateTime? AssessedOn { get; set; }

        public string AssessedBy { get; set; } = string.Empty;

        public bool? IsReassessed { get; set; }

        public int? ReassessmentScore { get; set; }

        public DateTime? ReassessedOn { get; set; }

        public string ReassessedBy { get; set; } = string.Empty;

        public string ContactPerson { get; set; } = string.Empty;

        public int? ProfileNumber { get; set; }

        public bool? IsManualAssessment { get; set; }

        public int PossibleScore { get; set; }
    }

    public class QuestionAnswerDTO
    {
        public QuestionAnswerDTO()
        {
        }

        public QuestionAnswerDTO(AssessmentDetail detail)
        {
            QuestionID = detail.QuestionId ?? 0;
            Score = detail.Score ?? 0;

            AssessorAnswer = detail.AssessorAnswer == null
            ? "N/A"
            : detail.AssessorAnswer == true
                ? "Yes"
                : "No";

            ReassessorAnswer = detail.ReassessorAnswer?.ToString() ?? string.Empty;
            ReassessorNote = detail.ReassessorNote ?? string.Empty;
            IsNA = detail.IsNa;

            QuestionValue = detail.Question?.QuestionValue ?? "QUESTION NOT FOUND";
        }

        public int QuestionID { get; set; }

        public string QuestionValue { get; set; } = string.Empty;

        public int Score { get; set; }

        public string AssessorAnswer { get; set; } = string.Empty;

        public string ReassessorAnswer { get; set; } = string.Empty;

        public string ReassessorNote { get; set; } = string.Empty;

        public bool IsNA { get; set; }
    }



    public class RecordingDTO
    {
        public RecordingDTO()
        {
        }

        public RecordingDTO(CallDetail callDetail, string? assessmentExtension)
        {
            RecordingID = (int)callDetail.RecordingId;
            NumberAssessedOn = callDetail.NumberAssessedOn ?? string.Empty;
            RecordingLength = callDetail.RecordingLength ?? string.Empty;
            RecordingURL = callDetail.RecordingUrl ?? string.Empty;
            RecordingTime = callDetail.RecordingTime;
            CallDate = callDetail.CallDate;
            Extension = assessmentExtension ?? string.Empty;
        }

        public long RecordingID { get; set; }

        public string NumberAssessedOn { get; set; } = string.Empty;

        public string RecordingLength { get; set; } = string.Empty;

        public string RecordingURL { get; set; } = string.Empty;

        public TimeOnly? RecordingTime { get; set; }

        public DateTime? DidAssessmentOn { get; set; }

        public DateTime? CallDate { get; set; }

        public string Extension { get; set; } = string.Empty;
    }


    public class FeedbackDTO
    {
        public FeedbackDTO()
        {
        }

        public FeedbackDTO(Feedback feedback)
        {
            FeedbackText = feedback.FeedbackText ?? string.Empty;
            FeedbackSendBy = feedback.FeedbackSendBy ?? string.Empty;
            FeedbackSendOn = feedback.FeedbackSendOn;
        }

        public string FeedbackText { get; set; } = string.Empty;

        public string FeedbackSendBy { get; set; } = string.Empty;

        public DateTime? FeedbackSendOn { get; set; }
    }

}
