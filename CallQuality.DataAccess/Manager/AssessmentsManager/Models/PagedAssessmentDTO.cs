using CallQuality.Core.Manager.Models.CallQualityDTOs;

namespace CallQuality.Core.Manager.AssessmentsManager.Models
{
    public class PagedAssessmentVM
    {
        public List<AssessmentDTO> Reassessment { get; set; } = new();

        public List<AssessmentDTO> NotReassessed { get; set; } = new();

        // Possible reassessment pagination
        public int PossibleCurrentPage { get; set; } = 1;

        // Already reassessed pagination
        public int ReassessedCurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 25;

        public int TotalCountReassessment { get; set; }

        public int TotalCountNotReassessment { get; set; }

        public string? SearchTerm { get; set; }

        public string ActiveTab { get; set; } = "possible";

        public int PossibleTotalPages =>
            PageSize <= 0
                ? 0
                : (int)Math.Ceiling(
                    (double)TotalCountNotReassessment / PageSize);

        public int ReassessedTotalPages =>
            PageSize <= 0
                ? 0
                : (int)Math.Ceiling(
                    (double)TotalCountReassessment / PageSize);

        public bool PossibleHasPreviousPage =>
            PossibleCurrentPage > 1;

        public bool PossibleHasNextPage =>
            PossibleCurrentPage < PossibleTotalPages;

        public bool ReassessedHasPreviousPage =>
            ReassessedCurrentPage > 1;

        public bool ReassessedHasNextPage =>
            ReassessedCurrentPage < ReassessedTotalPages;
    }
}