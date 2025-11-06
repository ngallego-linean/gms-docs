using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("IHE")]
public class IHEPortalController : Controller
{
    private readonly IGrantService _grantService;
    private readonly ILogger<IHEPortalController> _logger;

    public IHEPortalController(IGrantService grantService, ILogger<IHEPortalController> logger)
    {
        _grantService = grantService;
        _logger = logger;
    }

    [Route("")]
    [Route("Dashboard")]
    public IActionResult Dashboard(int iheId = 1, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var allApplications = _grantService.GetApplications();
            var iheApplications = allApplications
                .Where(a => a.IHE.Id == iheId && a.GrantCycleId == grantCycleId)
                .ToList();

            var totalStudents = iheApplications.SelectMany(a => a.Students).Count();
            var draftStudents = iheApplications.SelectMany(a => a.Students).Count(s => s.Status == "DRAFT");
            var submittedStudents = iheApplications.SelectMany(a => a.Students).Count(s => s.Status == "SUBMITTED");
            var approvedStudents = iheApplications.SelectMany(a => a.Students).Count(s => s.Status == "APPROVED");

            var actionItems = new List<ActionItemViewModel>();

            if (draftStudents > 0)
            {
                actionItems.Add(new ActionItemViewModel
                {
                    Id = 1,
                    Type = "submit_candidates",
                    Title = "Submit Candidate List",
                    Description = $"{draftStudents} draft candidate(s) ready to submit to LEA",
                    DueDate = DateTime.Now.AddDays(7),
                    Priority = "high",
                    AssignedTo = "IHE"
                });
            }

            var model = new IHEDashboardViewModel
            {
                IHEId = iheId,
                IHEName = iheApplications.FirstOrDefault()?.IHE.Name ?? "Institution",
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                TotalApplications = iheApplications.Count,
                TotalStudents = totalStudents,
                DraftCount = draftStudents,
                SubmittedCount = submittedStudents,
                ApprovedCount = approvedStudents,
                Applications = iheApplications.Select(a => new ApplicationSummaryViewModel
                {
                    Id = a.Id,
                    IHEName = a.IHE.Name,
                    LEAName = a.LEA.Name,
                    TotalStudents = a.Students.Count,
                    ApprovedCount = a.Students.Count(s => s.Status == "APPROVED"),
                    PendingCount = a.Students.Count(s => s.Status == "SUBMITTED"),
                    Status = a.Status,
                    LastModified = a.LastModified
                }).ToList(),
                ActionItems = actionItems
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading IHE dashboard");
            return View("Error");
        }
    }

    [Route("SubmitCandidates")]
    public IActionResult SubmitCandidates(int iheId = 1, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var model = new SubmitCandidatesViewModel
            {
                IHEId = iheId,
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading submit candidates page");
            return View("Error");
        }
    }

    [Route("Applications")]
    public IActionResult Applications(int iheId = 1, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var allApplications = _grantService.GetApplications();
            var iheApplications = allApplications
                .Where(a => a.IHE.Id == iheId && a.GrantCycleId == grantCycleId)
                .Select(a => new ApplicationSummaryViewModel
                {
                    Id = a.Id,
                    IHEName = a.IHE.Name,
                    LEAName = a.LEA.Name,
                    TotalStudents = a.Students.Count,
                    ApprovedCount = a.Students.Count(s => s.Status == "APPROVED"),
                    PendingCount = a.Students.Count(s => s.Status == "SUBMITTED"),
                    Status = a.Status,
                    LastModified = a.LastModified
                })
                .ToList();

            var model = new ApplicationListViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                Applications = iheApplications,
                SearchCriteria = new ApplicationSearchCriteria()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading IHE applications");
            return View("Error");
        }
    }
}
