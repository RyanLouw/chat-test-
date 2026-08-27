using CallQuality.Core.Manager.OperatorAssignmentManager;
using CallQuality.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CallQuality.Controllers;

[Authorize(Policy = "It")]
[Route("IT")]
public class ITController : Controller
{
    private readonly IOperatorManager _operatorAssignmentManager;

    public ITController(IOperatorManager operatorAssignmentManager)
    {
        _operatorAssignmentManager = operatorAssignmentManager;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index([FromQuery] string? managerId = null)
    {
        var model = await _operatorAssignmentManager.GetAssignmentAsync(managerId);
        return View(model);
    }

    [HttpPost("BulkAssign")]
    public async Task<IActionResult> BulkAssign([FromBody] SaveAssignRequestDTO? req)
    {
        if (req is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Missing request body."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid bulk assignment request.",
                errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
            });
        }

        var inserted = await _operatorAssignmentManager.SaveAssignOperatorsAsync(
            req.AssessorId,
            req.OperatorIds);

        return Ok(new
        {
            success = true,
            inserted,
            message = inserted == 0
                ? "Nothing to add (all selected operators were already assigned)."
                : $"Added {inserted} assignment(s)."
        });
    }

    [HttpPost("DeleteAssignment")]
    public async Task<IActionResult> DeleteAssignment([FromBody] DeleteAssignmentRequest? req)
    {
        if (req is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid request."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid delete assignment request.",
                errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
            });
        }

        await _operatorAssignmentManager.DeleteAssignmentAsync(req.RowKey);

        return Ok(new
        {
            success = true,
            message = "Deleted."
        });
    }

    [HttpPost("UpdateSecondaryAssignment")]
    public async Task<IActionResult> UpdateSecondaryAssignment(
        [FromBody] UpdateSecondaryAssignmentRequest? req)
    {
        if (req is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid request."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid secondary assignment request.",
                errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
            });
        }

        await _operatorAssignmentManager.UpdateSecondaryAssignmentAsync(
            req.RowKey,
            req.AssessorIdSecondary,
            req.SecondaryStartDate,
            req.SecondaryEndDate);

        return Ok(new
        {
            success = true,
            message = "Updated."
        });
    }

    [HttpPost("UpdateAssignment")]
    public async Task<IActionResult> UpdateAssignment([FromBody] UpdateAssignmentRequest? request)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid request."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid assignment request.",
                errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
            });
        }

        await _operatorAssignmentManager.UpdateAssignmentAsync(
            request.RowKey,
            request.AssessorId,
            request.AssessorIdSecondary,
            request.SecondaryStartDate,
            request.SecondaryEndDate);

        return Ok(new
        {
            success = true,
            message = "Assignment updated successfully."
        });
    }
}