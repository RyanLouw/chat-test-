using CallQuality.Core.Helpers;
using CallQuality.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CallQuality.Controllers;

[Route("")]
public class NavController : Controller
{
    private readonly IUserSession _userSession;
    private readonly ILogger<NavController> _logger;

    public NavController(
        ILogger<NavController> logger,
        IUserSession userSession)
    {
        _logger = logger;
        _userSession = userSession;
    }

    [HttpGet("")]
    [HttpGet("Nav")]
    [HttpGet("Nav/Index")]
    public async Task<IActionResult> Index()
    {
        var roles = await _userSession.GetUserRolesAsync();

        ViewBag.Roles = roles;
        ViewBag.UserName = User.Identity?.Name;

        if (roles.Contains("Manager"))
        {
            return RedirectToAction("Index", "Manager");
        }

        if (roles.Contains("Assessor"))
        {
            return RedirectToAction("Report", "Assessor");
        }

        if (roles.Contains("IT"))
        {
            return RedirectToAction("Index", "IT");
        }

        return View();
    }


    [HttpGet("Switch")]
    [HttpGet("Nav/Switch")]
    public async Task<IActionResult> Switch([FromQuery] string? mode)
    {
        var roles = await _userSession.GetUserRolesAsync();

        var canUseManagerView = roles.Contains("Manager");
        var canUseAssessorView = roles.Contains("Assessor");

        var selectedMode = mode?.Trim();

        if (string.Equals(selectedMode, "Assessor", StringComparison.OrdinalIgnoreCase) &&
            canUseAssessorView)
        {
            HttpContext.Session.SetString("ActiveViewMode", "Assessor");
            return RedirectToAction("Report", "Assessor");
        }

        if (string.Equals(selectedMode, "Manager", StringComparison.OrdinalIgnoreCase) &&
            canUseManagerView)
        {
            HttpContext.Session.SetString("ActiveViewMode", "Manager");
            return RedirectToAction("Assessments", "Manager");
        }

        if (canUseManagerView)
        {
            HttpContext.Session.SetString("ActiveViewMode", "Manager");
            return RedirectToAction("Assessments", "Manager");
        }

        if (canUseAssessorView)
        {
            HttpContext.Session.SetString("ActiveViewMode", "Assessor");
            return RedirectToAction("Report", "Assessor");
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpGet("Privacy")]
    public IActionResult Privacy()
    {
        return View();
    }


}