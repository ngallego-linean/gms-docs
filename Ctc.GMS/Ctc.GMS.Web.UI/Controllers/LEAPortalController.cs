using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("LEA")]
public class LEAPortalController : Controller
{
    private readonly IGrantService _grantService;
    private readonly ILogger<LEAPortalController> _logger;

    public LEAPortalController(IGrantService grantService, ILogger<LEAPortalController> logger)
    {
        _grantService = grantService;
        _logger = logger;
    }

    [Route("")]
    [Route("Dashboard")]
    public IActionResult Dashboard(int leaId = 2, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var allApplications = _grantService.GetApplications();
            var leaApplications = allApplications
                .Where(a => a.LEA.Id == leaId && a.GrantCycleId == grantCycleId)
                .ToList();

            var totalStudents = leaApplications.SelectMany(a => a.Students).Count();
            var draftStudents = leaApplications.SelectMany(a => a.Students).Count(s => s.Status == "DRAFT" || s.Status == "PENDING_LEA");
            var submittedStudents = leaApplications.SelectMany(a => a.Students).Count(s => s.Status == "SUBMITTED");
            var approvedStudents = leaApplications.SelectMany(a => a.Students).Count(s => s.Status == "APPROVED");

            var actionItems = new List<ActionItemViewModel>();

            if (draftStudents > 0)
            {
                actionItems.Add(new ActionItemViewModel
                {
                    Id = 1,
                    Type = "complete_application",
                    Title = "Complete Application",
                    Description = $"{draftStudents} candidate(s) need application completion",
                    DueDate = DateTime.Now.AddDays(14),
                    Priority = "high",
                    AssignedTo = "LEA"
                });
            }

            var model = new LEADashboardViewModel
            {
                LEAId = leaId,
                LEAName = leaApplications.FirstOrDefault()?.LEA.Name ?? "District",
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                TotalApplications = leaApplications.Count,
                TotalStudents = totalStudents,
                PendingCompletionCount = draftStudents,
                SubmittedCount = submittedStudents,
                ApprovedCount = approvedStudents,
                Applications = leaApplications.Select(a => new ApplicationSummaryViewModel
                {
                    Id = a.Id,
                    IHEName = a.IHE.Name,
                    LEAName = a.LEA.Name,
                    TotalStudents = a.Students.Count,
                    ApprovedCount = a.Students.Count(s => s.Status == "APPROVED"),
                    PendingCount = a.Students.Count(s => s.Status == "SUBMITTED" || s.Status == "PENDING_LEA"),
                    Status = a.Status,
                    LastModified = a.LastModified
                }).ToList(),
                ActionItems = actionItems
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading LEA dashboard");
            return View("Error");
        }
    }

    [Route("CompleteApplication/{id}")]
    public IActionResult CompleteApplication(int id)
    {
        try
        {
            var application = _grantService.GetApplication(id);

            if (application == null)
            {
                return NotFound($"Application {id} not found");
            }

            var model = new CompleteApplicationViewModel
            {
                ApplicationId = id,
                IHEName = application.IHE.Name,
                LEAName = application.LEA.Name,
                GrantCycleId = application.GrantCycleId,
                Students = application.Students
                    .Where(s => s.Status == "DRAFT" || s.Status == "PENDING_LEA")
                    .Select(s => new StudentViewModel
                    {
                        Id = s.Id,
                        ApplicationId = s.ApplicationId,
                        SEID = s.SEID,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        CredentialArea = s.CredentialArea,
                        Status = s.Status,
                        AwardAmount = s.AwardAmount,
                        CreatedAt = s.CreatedAt,
                        SubmittedAt = s.SubmittedAt
                    })
                    .ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading complete application page");
            return View("Error");
        }
    }

    [Route("Applications")]
    public IActionResult Applications(int leaId = 2, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var allApplications = _grantService.GetApplications();
            var leaApplications = allApplications
                .Where(a => a.LEA.Id == leaId && a.GrantCycleId == grantCycleId)
                .Select(a => new ApplicationSummaryViewModel
                {
                    Id = a.Id,
                    IHEName = a.IHE.Name,
                    LEAName = a.LEA.Name,
                    TotalStudents = a.Students.Count,
                    ApprovedCount = a.Students.Count(s => s.Status == "APPROVED"),
                    PendingCount = a.Students.Count(s => s.Status == "SUBMITTED" || s.Status == "PENDING_LEA"),
                    Status = a.Status,
                    LastModified = a.LastModified
                })
                .ToList();

            var model = new ApplicationListViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                Applications = leaApplications,
                SearchCriteria = new ApplicationSearchCriteria()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading LEA applications");
            return View("Error");
        }
    }
}
