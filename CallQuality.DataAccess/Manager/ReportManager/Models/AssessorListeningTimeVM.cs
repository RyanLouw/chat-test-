

namespace CallQuality.Core.Manager.ReportManager.Models
{
    public sealed class AssessorListeningTimeVM
    {
        public string SelectedMonth { get; set; } = string.Empty;
        public List<AssessorChartData> Assessors { get; set; } = new();
    }

    public sealed class AssessorChartData
    {
        public string AssessorName { get; set; } = string.Empty;
        public List<int> Days { get; set; } = new();
        public List<DepartmentSeries> Departments { get; set; } = new();
    }

    public sealed class DepartmentSeries
    {
        public string DepartmentName { get; set; } = string.Empty;
        public List<double> HoursListened { get; set; } = new();
        public string Color { get; set; } = string.Empty;
    }
}
