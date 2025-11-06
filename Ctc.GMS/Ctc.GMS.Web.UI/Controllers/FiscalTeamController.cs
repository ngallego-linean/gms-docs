using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("FiscalTeam")]
public class FiscalTeamController : Controller
{
    private readonly IGrantService _grantService;
    private readonly ILogger<FiscalTeamController> _logger;

    public FiscalTeamController(IGrantService grantService, ILogger<FiscalTeamController> logger)
    {
        _grantService = grantService;
        _logger = logger;
    }

    [Route("")]
    [Route("Dashboard")]
    public IActionResult Dashboard(int grantCycleId = 1)
    {
        try
        {
            var metrics = _grantService.CalculateMetrics(grantCycleId);
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var allApplications = _grantService.GetApplications();
            var cycleApplications = allApplications
                .Where(a => a.GrantCycleId == grantCycleId)
                .ToList();

            var approvedStudents = cycleApplications
                .SelectMany(a => a.Students)
                .Where(s => s.Status == "APPROVED")
                .Count();

            var studentsNeedingGAA = cycleApplications
                .SelectMany(a => a.Students)
                .Where(s => s.Status == "APPROVED" && string.IsNullOrEmpty(s.GAAStatus))
                .Count();

            var actionItems = new List<ActionItemViewModel>();

            if (studentsNeedingGAA > 0)
            {
                actionItems.Add(new ActionItemViewModel
                {
                    Id = 1,
                    Type = "gaa_generation",
                    Title = "Generate GAA Documents",
                    Description = $"{studentsNeedingGAA} approved student(s) need Grant Award Agreement",
                    DueDate = DateTime.Now.AddDays(3),
                    Priority = "high",
                    AssignedTo = "Fiscal Team"
                });
            }

            var model = new DashboardViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                PendingReviewCount = studentsNeedingGAA,
                Metrics = new GrantCycleMetricsViewModel
                {
                    ApproprietedAmount = metrics.ApproprietedAmount,
                    ReservedAmount = metrics.ReservedAmount,
                    EncumberedAmount = metrics.EncumberedAmount,
                    DisbursedAmount = metrics.DisbursedAmount,
                    RemainingAmount = metrics.RemainingAmount,
                    RemainingPercent = metrics.RemainingPercent,
                    OutstandingBalance = metrics.OutstandingBalance,
                    TotalStudents = metrics.TotalStudents,
                    UniqueIHEs = metrics.UniqueIHEs,
                    UniqueLEAs = metrics.UniqueLEAs,
                    ActivePartnerships = metrics.ActivePartnerships,
                    StatusCounts = new StatusCountsViewModel
                    {
                        Draft = metrics.StatusCounts.Draft,
                        PendingLEA = metrics.StatusCounts.PendingLEA,
                        Submitted = metrics.StatusCounts.Submitted,
                        UnderReview = metrics.StatusCounts.UnderReview,
                        Approved = metrics.StatusCounts.Approved,
                        Rejected = metrics.StatusCounts.Rejected
                    }
                },
                ApplicationsWithPendingStudents = new List<ApplicationSummaryViewModel>(),
                CurrentUser = User.Identity?.Name ?? "Fiscal Team Member",
                ActionItems = actionItems
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Fiscal Team dashboard");
            return View("Error");
        }
    }

    [Route("GAA")]
    public IActionResult GAA(int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var allApplications = _grantService.GetApplications();
            var studentsNeedingGAA = allApplications
                .Where(a => a.GrantCycleId == grantCycleId)
                .SelectMany(a => a.Students.Select(s => new
                {
                    Application = a,
                    Student = s
                }))
                .Where(x => x.Student.Status == "APPROVED")
                .Select(x => new StudentGAAViewModel
                {
                    StudentId = x.Student.Id,
                    StudentName = $"{x.Student.FirstName} {x.Student.LastName}",
                    SEID = x.Student.SEID,
                    IHEName = x.Application.IHE.Name,
                    LEAName = x.Application.LEA.Name,
                    CredentialArea = x.Student.CredentialArea,
                    AwardAmount = x.Student.AwardAmount,
                    GAAStatus = x.Student.GAAStatus,
                    ApprovedDate = x.Student.SubmittedAt
                })
                .ToList();

            var model = new GAAListViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                Students = studentsNeedingGAA
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading GAA generation page");
            return View("Error");
        }
    }

    [Route("Payments")]
    public IActionResult Payments(int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var model = new PaymentListViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                Payments = new List<PaymentViewModel>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading payments page");
            return View("Error");
        }
    }
}
