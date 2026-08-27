using System.Data;

namespace CallQuality.Core.Manager.ExportManager.Models;

public sealed class ExcelExportDTO
{
    public string AssessorsName { get; set; } = string.Empty;
    public DataTable AssessorChartData { get; set; } = new();
}
