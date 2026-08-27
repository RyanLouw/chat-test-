using CallQuality.Core.Manager.TrainingManager.Models;
using System.Data;

namespace CallQuality.Core.Manager.ExportManager;

public interface IExportService
{
    Task<byte[]> DownloadAssessorListeningTimeExcelAsync(DateTime startDate, DateTime endDate);
    Task<byte[]> DownloadAccuracyReportExcelAsync(DateTime startDate, DateTime endDate, string assessorOrReAssessor);
    Task<byte[]> DownloadAssessorTrackingReportExcelAsync(DateTime startDate, DateTime endDate);
    Task<byte[]> DownloadAssessorBreakdownReportExcelAsync(DateTime startDate, DateTime endDate);
    Task<byte[]> DownloadTrainingReportExcelAsync(TrainingVM modle);
    Task<byte[]> DownloadManagerOverviewReportExcelAsync();

}
