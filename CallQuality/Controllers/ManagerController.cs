using CallQuality.Core.Manager.AssessmentsManager;
using CallQuality.Core.Manager.AssessmentsManager.Models;
using CallQuality.Core.Manager.ExportManager;
using CallQuality.Core.Manager.QuestionsManager;
using CallQuality.Core.Manager.QuestionsManager.Models;
using CallQuality.Core.Manager.ReportManager;
using CallQuality.Core.Manager.ReportManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CallQuality.Controllers;

[RequireHttps]
[Route("Manager")]
public class ManagerController : Controller
{
    private readonly IReportManager _service;
    private readonly IExportService _excel;
    private readonly IQuestionsManager _questions;
    private readonly ICallAssessmentManager _assessments;
    private readonly IConfiguration _config;

    public ManagerController(
        IConfiguration config,
        IReportManager service,
        IExportService excel,
        IQuestionsManager questions,
        ICallAssessmentManager assessments)
    {
        _config = config;
        _service = service;
        _excel = excel;
        _questions = questions;
        _assessments = assessments;
    }

    [Authorize(Policy = "Manager")]
    [HttpGet("")]
    [HttpGet(nameof(Index))]
    public async Task<IActionResult> Index()
    {
        var data = await _service.GetManagerHomeOverviewAsync();
        return View(data);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(ReportHome))]
    public IActionResult ReportHome()
    {
        return View();
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(Report_AssessorListeningTime))]
    public async Task<IActionResult> Report_AssessorListeningTime()
    {
        var now = DateTime.Now;
        var start = new DateTime(now.Year, now.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        var model = await _service.GetAssessorListeningTimeAsync(start, end);
        return View(model);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(GetAssessorListeningTimeData))]
    public async Task<IActionResult> GetAssessorListeningTimeData([FromQuery] DateTime? startDate)
    {
        var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(1);
        var end = start.AddMonths(1).AddDays(-1);

        var model = await _service.GetAssessorListeningTimeAsync(start, end);
        return Json(model);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(AssessmentAccuracy))]
    public async Task<IActionResult> AssessmentAccuracy()
    {
        var now = DateTime.Now;
        var start = new DateTime(now.Year, now.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        var model = await _service.GetAccuracyReportAsync(start, end, "Assessor");
        return View(model);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(GetAccuracyReportData))]
    public async Task<IActionResult> GetAccuracyReportData(
        [FromQuery] DateTime startDate,
        [FromQuery] string role)
    {
        var endDate = new DateTime(
            startDate.Year,
            startDate.Month,
            DateTime.DaysInMonth(startDate.Year, startDate.Month));

        var model = await _service.GetAccuracyReportAsync(startDate, endDate, role);
        return Json(model);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(Report_AssessorTracking))]
    public IActionResult Report_AssessorTracking()
    {
        var model = new AssessorTracking_Report();
        return View(model);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(GetAssessorTrackingReportAsync))]
    public async Task<IActionResult> GetAssessorTrackingReportAsync(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var result = await _service.GetAssessorTrackingReportAsync(from, to);
        return Json(result);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(Report_AssessorBreakdown))]
    public async Task<IActionResult> Report_AssessorBreakdown()
    {
        var now = DateTime.Now;
        var start = new DateTime(now.Year, now.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        var model = await _service.GetAssessorBreakdown_PercentageAsync(start, end);
        return View(model);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(GetAssessorBreakdownReport))]
    public async Task<IActionResult> GetAssessorBreakdownReport(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var data = await _service.GetAssessorBreakdown_PercentageAsync(from, to);
        return Json(data);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(QuestionWrongStatsReport))]
    public async Task<IActionResult> QuestionWrongStatsReport([FromQuery] string? type)
    {
        var data = await _service.GetQuestionWrongStatsReportAsync(type);
        return View(data);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(ManageQuestions))]
    public async Task<IActionResult> ManageQuestions()
    {
        var model = await _questions.GetMasterQuestionsAsync();
        return View(model);
    }

    [Authorize(Policy = "Manager")]
    [HttpPost(nameof(UpdateQuestion))]
    public async Task<IActionResult> UpdateQuestion([FromBody] QuestionWithTypesDTO updated)
    {
        bool result = await _questions.UpdateQuestionAsync(updated);

        if (!result)
        {
            return NotFound(new
            {
                success = false,
                message = "Question not found or update failed."
            });
        }

        return Ok(new
        {
            success = true,
            message = "Question updated successfully."
        });
    }

    [Authorize(Policy = "Manager")]
    [HttpPost(nameof(CreateQuestion))]
    public async Task<IActionResult> CreateQuestion([FromBody] QuestionWithTypesDTO dto)
    {
        var result = await _questions.CreateQuestionAsync(dto);

        return Json(new
        {
            success = result,
            message = result
                ? "Question created successfully."
                : "Failed to create question."
        });
    }

    [Authorize(Policy = "Manager")]
    [HttpPost(nameof(SaveOrder))]
    public async Task<IActionResult> SaveOrder([FromBody] SubGroupTypeWithQuestionsDTO dto)
    {
        var ok = await _questions.UpdateQuestionOrderAsync(dto);

        return Ok(new
        {
            success = ok
        });
    }

    [Authorize(Policy = "Manager")]
    [HttpPost(nameof(SaveNewSubgroupOrder))]
    public async Task<IActionResult> SaveNewSubgroupOrder([FromBody] CreateSubGroupDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

            return BadRequest(new
            {
                success = false,
                message = "Model binding failed",
                errors
            });
        }

        var ok = await _questions.CreateSubGroupWithQuestionsAsync(dto);

        return Ok(new
        {
            success = ok
        });
    }


    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(Assessments))]
    public async Task<IActionResult> Assessments(
     [FromQuery] int possiblePage = 1,
     [FromQuery] int reassessedPage = 1,
     [FromQuery] int pageSize = 25,
     [FromQuery] string? search = null,
     [FromQuery] string activeTab = "possible")
    {
        var startDate = DateTime.Now.AddMonths(-1);

        var model =
            await _assessments.GetPagedAssessmentsAsync(
                startDate,
                possiblePage,
                reassessedPage,
                pageSize,
                search,
                activeTab);

        return View(model);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(Reassess))]
    [HttpGet(nameof(Reassess) + "/{id:int}")]
    public async Task<IActionResult> Reassess([FromRoute] int id)
    {
        var vm = await _assessments.GetReassessAsync(id);
        return View(vm);
    }

    [Authorize(Policy = "Manager")]
    [HttpPost(nameof(SaveReassessment))]
    public async Task<IActionResult> SaveReassessment([FromBody] ReassessmentSaveRequest? request)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid reassessment request."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid reassessment request.",
                errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray())
            });
        }

        var success = await _assessments.SaveReassessmentAsync(request);

        if (!success)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to save reassessment."
            });
        }

        return Ok(new
        {
            success = true
        });
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(DownloadAssessorListeningTimeExcel))]
    public async Task<IActionResult> DownloadAssessorListeningTimeExcel([FromQuery] DateTime? startDate)
    {
        var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        var bytes = await _excel.DownloadAssessorListeningTimeExcelAsync(start, end);
        var fileName = $"AssessorListeningTime_{start:yyyyMM}.xlsx";

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(DownloadAccuracyExcel))]
    public async Task<IActionResult> DownloadAccuracyExcel(
        [FromQuery] DateTime startDate,
        [FromQuery] string role)
    {
        var endDate = new DateTime(
            startDate.Year,
            startDate.Month,
            DateTime.DaysInMonth(startDate.Year, startDate.Month));

        var bytes = await _excel.DownloadAccuracyReportExcelAsync(startDate, endDate, role);
        var fileName = $"AssessorAccuracy_{startDate:yyyyMM}.xlsx";

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(DownloadAssessorTrackingReportExcel))]
    public async Task<IActionResult> DownloadAssessorTrackingReportExcel(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var bytes = await _excel.DownloadAssessorTrackingReportExcelAsync(from, to);
        var fileName = $"AssessorTrackingReport_{from:yyyyMM}.xlsx";

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(DownloadAssessorBreakdownReportExcel))]
    public async Task<IActionResult> DownloadAssessorBreakdownReportExcel(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var bytes = await _excel.DownloadAssessorBreakdownReportExcelAsync(from, to);
        var fileName = $"AssessorBreakdownReport_{from:yyyyMM}.xlsx";

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [Authorize(Policy = "Manager")]
    [HttpGet(nameof(Export))]
    public async Task<IActionResult> Export()
    {
        var bytes = await _excel.DownloadManagerOverviewReportExcelAsync();
        var fileName = $"ManagerOverviewReport_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
