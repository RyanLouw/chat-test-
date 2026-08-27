using CallQuality.Core.Manager.TrainingManager;
using CallQuality.Core.Manager.TrainingManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace CallQuality.Controllers;

[Route("TrainingRegister")]
public class TrainingRegisterController : Controller
{
    private readonly ITrainingManager _trainingManager;

    public TrainingRegisterController(ITrainingManager trainingManager)
    {
        _trainingManager = trainingManager;
    }

    [HttpGet("")]
    [HttpGet("[action]")]
    public async Task<IActionResult> Index()
    {
        var model = await _trainingManager.GetTrainingRegisterDataAsync();
        return View(model);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> TrainingDetails([FromQuery] Guid? userId)
    {
        var model = await _trainingManager.GetTrainingDetailsAsync(userId);
        return View(model);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ExportTraining()
    {
        var bytes = await _trainingManager.ExportTraining();

        var now = DateTime.Now;
        var fileName = $"TrainingRegister_{now:yyyyMMdd_HHmmss}.xlsx";

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }

    [HttpGet("~/Training/GetUsersByGroup/{id}")]
    public async Task<IActionResult> GetUsersByGroup([FromRoute] string id)
    {
        var traineeIds = await _trainingManager.GetTraineesByGroupIdAsync(id);
        return Json(traineeIds);
    }

    [HttpGet("~/Training/GetUsersByLeader/{id}")]
    public async Task<IActionResult> GetUsersByLeader([FromRoute] string id)
    {
        var traineeIds = await _trainingManager.GetTraineesByLeaderIdAsync(id);
        return Json(traineeIds);
    }

    [HttpPost("Save")] 
    public async Task<IActionResult> Save(TrainingDetailsPageVM model) 
    { 
        var files = Request.Form.Files; 
        var success = await _trainingManager.SaveTrainingRegisterAsync(model, files); 
        if (!success) 
        { 
            TempData["Error"] = "An error occurred. Please try again."; 
            return BadRequest(); 
        } 
        TempData["Success"] = "Training register added successfully!"; 
        return RedirectToAction("Index"); 
    }

}