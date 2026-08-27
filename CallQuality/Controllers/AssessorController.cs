using CallQuality.Core.Manager.AssessmentsManager;
using CallQuality.Core.Manager.AssessmentsManager.Models;
using CallQuality.Core.Manager.Models.CallQualityDTOs;
using CallQuality.Core.Manager.ReportManager;
using CallQuality.Core.Manager.ReportManager.Models;
using CallQuality.Models;
using CallQuality.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CallQuality.Controllers;

[Route("Assessor")]
public class AssessorController : Controller
{
    private readonly ICallAssessmentManager _assessments;
    private readonly IReportManager _service;


    public AssessorController(
        ICallAssessmentManager assessments,
        IReportManager service)
    {
        _assessments = assessments;
        _service = service;
    }

    [Authorize(Policy = "Assessor")]
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Report), "Assessor");
    }

    [Authorize(Policy = "AssessorOrManager")]
    [HttpGet("OperatorAssessment")]
    public async Task<IActionResult> OperatorAssessment(
        [FromQuery] string? ext,
        [FromQuery] string? department,
        [FromQuery] DateOnly? start,
        [FromQuery] DateOnly? end)
    {
        var assessments = await _assessments.GetOperatorAssessmentsAsync(ext, department, start, end);
        return View(assessments);
    }

    [Authorize(Policy = "AssessorOrManager")]
    [HttpGet("ViewOperatorAssessment/{assessmentId:int}")]
    public async Task<IActionResult> ViewOperatorAssessment([FromRoute] int assessmentId)
    {
        var model = await _service.GetOperatorAssessmentAsync(assessmentId);
        return View(model);
    }

    [Authorize(Policy = "Assessor")]
    [HttpGet("NewAssessment")]
    public async Task<IActionResult> NewAssessment()
    {
        var model = await _assessments.NewAssessment(null, null);
        return View(model);
    }

    [Authorize(Policy = "Assessor")]
    [HttpPost("DownloadCall")]
    public IActionResult DownloadCall([Required][FromBody] RecordingDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid recording data."
            });
        }

        string secureUrl = "";

        return Ok(new
        {
            success = true,
            url = secureUrl
        });
    }

    [Authorize(Policy = "Assessor")]
    [HttpPost("SearchInteractions")]
    public async Task<IActionResult> SearchInteractions(
        [FromBody] AgentAssignedVM? agent,
        [FromQuery] bool? isManual)
    {

        var model = await _assessments.NewAssessment(agent, isManual);
        return Ok(model);
    }

    [Authorize(Policy = "Assessor")]
    [HttpPost("SelectInteraction")]
    public async Task<IActionResult> SelectInteraction([FromBody] InteractionResultVM dto)
    {
        var model = await _assessments.BuildAssessInteractionAsync(dto, null);
        HttpContext.Items["AssessModel"] = model;

        return View("AssessInteraction", model);
    }

    [Authorize(Policy = "Assessor")]
    [HttpPost("SelectPspInteraction")]
    public async Task<IActionResult> SelectPspInteraction([FromBody] PSPInteractionsVM dto)
    {
        var model = await _assessments.BuildAssessInteractionAsync(null, dto);
        HttpContext.Items["AssessModel"] = model;

        return View("AssessInteraction", model);
    }

    [Authorize(Policy = "Assessor")]
    [HttpPost("SelectCallInteraction")]
    public async Task<IActionResult> SelectCallInteraction([FromBody] CallInteractionVM dto)
    {
        var model = await _assessments.BuildcallInteractionAsync(dto);
        HttpContext.Items["AssessModel"] = model;

        return View("AssessInteraction", model);
    }


    [Authorize(Policy = "Assessor")]
    [HttpPost("SelectSubGroup")]
    public async Task<IActionResult> SelectSubGroupAsync(
     [FromBody] SelectGroupRequest request)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid request."
            });
        }

        if (string.IsNullOrWhiteSpace(request.FullModel))
        {
            return BadRequest(new
            {
                success = false,
                message = "The assessment model was not supplied."
            });
        }

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var fullModel = JsonSerializer.Deserialize<AssessInteractionVM>(
            request.FullModel,
            serializerOptions);

        if (fullModel is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "The assessment model could not be loaded."
            });
        }

        var selectedSubGroupIds = request.SelectedSubGroupId?
            .Where(id => id > 0)
            .Distinct()
            .ToList() ?? [];

        if (selectedSubGroupIds.Count == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Please select at least one question group."
            });
        }

        var selectedSubGroups =
            await _assessments.GetSubGroupsWithQuestionsByIdsAsync(
                selectedSubGroupIds);

        if (selectedSubGroups.Count == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "None of the selected question groups were found."
            });
        }

        fullModel.SelectedSubGroupId = selectedSubGroupIds;

        fullModel.SelectedQuestions = selectedSubGroups
            .SelectMany(subGroup => subGroup.Questions)
            .Select(question => new
            {
                Question = question,

                Link = question.QuestionInTypes
                    .FirstOrDefault(link => link.Active == true)
            })
            .Where(x => x.Link != null)
            .GroupBy(x => x.Question.QuestionId)
            .Select(group => group.First())
            .Select(x => new QuestionInteractionVM
            {
                QuestionId = x.Question.QuestionId,
                QuestionValue =
                    x.Question.QuestionValue ?? string.Empty,

                DefaultFeedback =
                    x.Question.DefaultFeedback ?? string.Empty,

                Score = x.Link!.Score ?? 0,
                OrderNumber = x.Link.OrderNumber,
                Active = x.Link.Active ?? false
            })
            .ToList();

        var updatedModel = JsonSerializer.Serialize(fullModel);

        var html = this.RenderPartialViewToString(
            "_QuestionsPartial",
            fullModel);

        return Ok(new
        {
            success = true,
            html,
            updatedModel
        });
    }

    [Authorize(Policy = "Assessor")]
    [HttpPost("CalculateScore")]
    public IActionResult CalculateScore([FromBody] AssessInteractionVM model)
    {
        int score = 0;
        int maxScore = 0;

        foreach (QuestionInteractionVM q in model.SelectedQuestions)
        {
            string answer = model.Answers.ContainsKey(q.QuestionId)
                ? model.Answers[q.QuestionId]
                : "N/A";

            if (answer is not "N/A" && answer is not "na")
            {
              maxScore += q.Score;
            }

            if (answer == "yes")
            {
                score += q.Score;
            }
        }

        model.Score = score;
        model.MaxScore = maxScore;
        model.Percentage = model.MaxScore == 0
            ? 0
            : ((double)score / model.MaxScore * 100).Truncate2();


        model.AutoFeedback = _assessments.ScoreFeedback(model);

        string updatedJson = System.Text.Json.JsonSerializer.Serialize(model);

        return Ok(new
        {
            success = true,
            updatedModel = updatedJson,
            scoreHtml = $"<strong>Score:</strong> {model.Score} / {model.MaxScore} ({model.Percentage}%)",
            autoFeedbackHtml = model.AutoFeedback
        });
    }

    [Authorize(Policy = "AssessorOrManager")]
    [HttpPost("GetRecordingUrl")]
    public async Task<IActionResult> GetRecordingUrl([FromBody] RecordingRequest req)
    {
        string? url = await _assessments.GetDownloadUrlAsync(req.RecordingId);

        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest(new
            {
                success = false
            });
        }

        return Ok(new
        {
            success = true,
            url
        });
    }
    [Authorize(Policy = "Assessor")]
    [HttpPost("SubmitFinal")]
    public async Task<IActionResult> SubmitFinal(
        [FromBody] AssessInteractionDTO? model)
    {
        if (model is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "The assessment request could not be deserialized."
            });
        }

        var id = await _assessments.SaveAssessmentAsync(model, User);

        return Ok(new
        {
            success = true,
            id
        });
    }

    [Authorize(Policy = "Assessor")]
    [HttpGet("Report")]
    public async Task<IActionResult> Report([FromQuery] DateTime? month)
    {
        if (!month.HasValue)
        {
            return View(new List<OperatorNumberOfAssessmentReportDTO>());
        }

        var rows = await _service.GetOperator_NumberOfAssessment_Reports(month);
        return View(rows);
    }
}