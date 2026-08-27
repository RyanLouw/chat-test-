using CallQuality.Core.DataAccess.CallQualityDataAccess.CQ;
using CallQuality.Core.Manager.ExportManager.Models;
using CallQuality.Core.Manager.TrainingManager.Models;
using CallQuality.Utilities;


namespace CallQuality.Core.Manager.ExportManager
{
    public class ExportManager : IExportService
    {

        private readonly ICallQualityDataAccess _db;

        public ExportManager(ICallQualityDataAccess db)
        {
            _db = db;

        }

        public async Task<byte[]> DownloadAssessorListeningTimeExcelAsync(DateTime startDate, DateTime endDate)
        {
            var ExcelData = new List<ExcelExportDTO>();
            var AssessorsNames = await _db.GetAssessorsAsync(startDate, endDate);

            foreach (string name in AssessorsNames)
            {
                var dt = await _db.AssessorReportMonthAssessmentAsync(startDate, name);

                var assessorChartData = ExcelExportHelper.ToDataTable(dt);
                ExcelData.Add(new ExcelExportDTO
                {
                    AssessorsName = name,
                    AssessorChartData = assessorChartData
                });
            }

            var excelBytes = ExcelExportHelper.ExportDataTablesToExcel(ExcelData);
            return excelBytes;
        }


        public async Task<byte[]> DownloadAccuracyReportExcelAsync(DateTime startDate, DateTime endDate, string assessorOrReAssessor)
        {

            var AssessorsNames = assessorOrReAssessor == "Assessor"
                  ? await _db.GetAssessorsAsync(startDate, endDate)
                  : await _db.GetReassessorsAsync(startDate, endDate);

            var ExcelData = new List<ExcelExportDTO>();


            foreach (string name in AssessorsNames)
            {
                var dt = await _db.GetAccuracyReportAsync(startDate, endDate, name);
                var assessorChartData = ExcelExportHelper.ToDataTable(dt);
                ExcelData.Add(new ExcelExportDTO
                {
                    AssessorsName = name,
                    AssessorChartData = assessorChartData
                });
            }

            var excelBytes = ExcelExportHelper.ExportDataTablesToExcel(ExcelData);
            return excelBytes;
        }


        public async Task<byte[]> DownloadAssessorTrackingReportExcelAsync(DateTime startDate, DateTime endDate)
        {
            var reportTable = await _db.GetAssessorTrackingReportAsync(startDate, endDate);
            var assessorChartData = ExcelExportHelper.ToDataTable(reportTable);
            var excelData = new List<ExcelExportDTO>
             {
                new ExcelExportDTO
                {
                    AssessorsName = "AssessorTrackingReport",
                    AssessorChartData = assessorChartData
                }
             };

            var excelBytes = ExcelExportHelper.ExportDataTablesToExcel(excelData);
            return excelBytes;
        }


        public async Task<byte[]> DownloadAssessorTrainingExcelAsync(TrainingDetailsPageVM modle)
        {

            var TraineesChartData = ExcelExportHelper.ToDataTable(modle.Trainees);
 
            var TimeDurationsChartData = ExcelExportHelper.ToDataTable(modle.TimeDurations);
            var excelData = new List<ExcelExportDTO>
             {
                new ExcelExportDTO
                {
                    AssessorsName = "AssessorTrackingReport",
                    AssessorChartData = TraineesChartData
                }
             };

            var excelBytes = ExcelExportHelper.ExportDataTablesToExcel(excelData);
            return excelBytes;
        }


        public async Task<byte[]> DownloadAssessorBreakdownReportExcelAsync(
            DateTime startDate,
            DateTime endDate)
        {
            var assessors = await _db.GetAssessorsAsync(
                startDate,
                endDate);

            var allDtos = new List<ExcelExportDTO>();

            foreach (var assessor in assessors
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var breakdownResults =
                    await _db.AssessorBreakdown_PercentageAsync(
                        startDate,
                        endDate,
                        assessor);

                var breakdownTable =
                    ExcelExportHelper.ToDataTable(breakdownResults);

                allDtos.Add(new ExcelExportDTO
                {
                    AssessorsName = assessor,
                    AssessorChartData = breakdownTable
                });
            }

            return ExcelExportHelper.ExportDataTablesToExcel(allDtos);
        }






        public async Task<byte[]> DownloadManagerOverviewReportExcelAsync()
        {
            var rawRows = await _db.GetManagerHomeOverviewAsync();

            var allowedDepartments = new List<string>
                {
                    "Dischem PRP",
                    "Dischem SRS",
                    "PRP",
                    "PSP",
                    "CRM"
                };

            var filteredRows = rawRows
                .Where(r => allowedDepartments.Contains(r.Department, StringComparer.OrdinalIgnoreCase))
                .OrderBy(r => r.Department)
                .ThenBy(r => r.DisplayName)
                .ToList();

            var groupedDepartments = filteredRows
                .GroupBy(r => r.Department ?? "Unknown")
                .OrderBy(g => g.Key);

            var excelSheets = new List<ExcelExportDTO>();

            foreach (var departmentGroup in groupedDepartments)
            {
                var departmentRows = departmentGroup
                    .Select(r => new
                    {
                        AgentName = r.DisplayName,
                        TimesAssessed = r.AssessmnetsDone
                    })
                    .ToList();

                var dataTable = ExcelExportHelper.ToDataTable(departmentRows);

                excelSheets.Add(new ExcelExportDTO
                {
                    AssessorsName = departmentGroup.Key,
                    AssessorChartData = dataTable
                });
            }

            return ExcelExportHelper.ExportDataTablesToExcel(excelSheets);
        }







        public async Task<byte[]> DownloadTrainingReportExcelAsync(TrainingVM modle)
        {
            
            var UserTrainings = ExcelExportHelper.ToDataTable(modle.UserTrainings);
            var ExistingRegisters = ExcelExportHelper.ToDataTable(modle.ExistingRegisters);
            var SuggestedTrainings = ExcelExportHelper.ToDataTable(modle.SuggestedTrainings);
            var allDtos = new List<ExcelExportDTO>
            {
                new ExcelExportDTO
                {
                    AssessorsName = "UserTrainings",
                    AssessorChartData = UserTrainings
                },
                new ExcelExportDTO
                {
                    AssessorsName = "ExistingRegisters",
                    AssessorChartData = ExistingRegisters
                },
                new ExcelExportDTO
                {
                    AssessorsName = "SuggestedTrainings",
                    AssessorChartData = SuggestedTrainings
                }
            };

            return ExcelExportHelper.ExportDataTablesToExcel(allDtos);
        }

    }
}
