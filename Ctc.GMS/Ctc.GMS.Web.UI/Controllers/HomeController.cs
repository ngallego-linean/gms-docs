using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("")]
[Route("Home")]
public class HomeController : Controller
{
    private readonly IGrantService _grantService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IGrantService grantService, ILogger<HomeController> logger)
    {
        _grantService = grantService;
        _logger = logger;
    }

    [Route("")]
    [Route("Index")]
    public IActionResult Index(int grantCycleId = 1)
    {
        try
        {
            var metrics = _grantService.CalculateMetrics(grantCycleId);
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            // Get applications with pending students
            var allApplications = _grantService.GetApplications();
            var applicationsWithPending = allApplications
                .Where(a => a.GrantCycleId == grantCycleId)
                .Where(a => a.Students.Any(s => s.Status == "SUBMITTED"))
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

            var model = new DashboardViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                PendingReviewCount = allApplications
                    .SelectMany(a => a.Students)
                    .Count(s => s.Status == "SUBMITTED"),
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
                ApplicationsWithPendingStudents = applicationsWithPending,
                CurrentUser = User.Identity?.Name ?? "Guest"
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            return View("Error");
        }
    }

    [Route("Error")]
    public IActionResult Error()
    {
        return View();
    }
}
