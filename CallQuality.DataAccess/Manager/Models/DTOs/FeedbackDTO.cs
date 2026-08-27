

namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

    public partial class FeedbackDTO
    {
        public int FeedbackId { get; set; }

        public string? FeedbackText { get; set; }

        public string? FeedbackSendBy { get; set; }

        public DateTime? FeedbackSendOn { get; set; }

        public string? AssessmentsIncluded { get; set; }

        public string? FeedbackSendTo { get; set; }

        public virtual ICollection<AssessmentsDTO> Assessment { get; set; } = new List<AssessmentsDTO>();
    }
