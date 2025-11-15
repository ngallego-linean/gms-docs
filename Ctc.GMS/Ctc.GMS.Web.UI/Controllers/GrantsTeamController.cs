using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;
using GMS.Business.Helpers;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("GrantsTeam")]
public class GrantsTeamController : Controller
{
    private readonly IGrantService _grantService;
    private readonly ILogger<GrantsTeamController> _logger;

    public GrantsTeamController(IGrantService grantService, ILogger<GrantsTeamController> logger)
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

            var applicationsWithPending = cycleApplications
                .Where(a => a.Students.Any(s => StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Review))
                .Select(a => new ApplicationSummaryViewModel
                {
                    Id = a.Id,
                    IHEName = a.IHE.Name,
                    LEAName = a.LEA.Name,
                    TotalStudents = a.Students.Count,
                    ApprovedCount = a.Students.Count(s => StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Disbursement || StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Reporting || StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Complete),
                    PendingCount = a.Students.Count(s => StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Review),
                    Status = a.Status,
                    LastModified = a.LastModified
                })
                .OrderByDescending(a => a.PendingCount)
                .ToList();

            var recentActivity = cycleApplications
                .SelectMany(a => a.Students.Select(s => new
                {
                    Application = a,
                    Student = s
                }))
                .Where(x => x.Student.Status == "APPROVED" || x.Student.Status == "REJECTED")
                .OrderByDescending(x => x.Student.SubmittedAt ?? DateTime.MinValue)
                .Take(10)
                .Select(x => new ActionItemViewModel
                {
                    Id = x.Student.Id,
                    Type = x.Student.Status == "APPROVED" ? "approval" : "rejection",
                    Title = $"{x.Student.FirstName} {x.Student.LastName}",
                    Description = $"{x.Application.IHE.Name} - {x.Application.LEA.Name}",
                    DueDate = x.Student.SubmittedAt,
                    Priority = "completed",
                    AssignedTo = "Grants Team"
                })
                .ToList();

            var actionItems = applicationsWithPending
                .SelectMany(a => Enumerable.Range(0, a.PendingCount).Select(_ => new ActionItemViewModel
                {
                    Id = a.Id,
                    Type = "review",
                    Title = "Review Student Application",
                    Description = $"{a.IHEName} - {a.LEAName}",
                    DueDate = a.LastModified.AddDays(7),
                    Priority = a.LastModified < DateTime.Now.AddDays(-5) ? "high" : "normal",
                    AssignedTo = "Grants Team"
                }))
                .ToList();

            var model = new DashboardViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                PendingReviewCount = metrics.StatusCounts.Review,
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
                        Submission = metrics.StatusCounts.Submission,
                        Review = metrics.StatusCounts.Review,
                        Disbursement = metrics.StatusCounts.Disbursement,
                        Reporting = metrics.StatusCounts.Reporting,
                        Rejected = metrics.StatusCounts.Rejected,
                        Complete = metrics.StatusCounts.Complete
                    }
                },
                ApplicationsWithPendingStudents = applicationsWithPending,
                CurrentUser = User.Identity?.Name ?? "Grants Team Member",
                ActionItems = actionItems
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Grants Team dashboard");
            return View("Error");
        }
    }

    [Route("Review")]
    public IActionResult Review(int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var allApplications = _grantService.GetApplications();
            var applicationsWithPending = allApplications
                .Where(a => a.GrantCycleId == grantCycleId)
                .Where(a => a.Students.Any(s => StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Review))
                .Select(a => new ApplicationSummaryViewModel
                {
                    Id = a.Id,
                    IHEName = a.IHE.Name,
                    LEAName = a.LEA.Name,
                    TotalStudents = a.Students.Count,
                    ApprovedCount = a.Students.Count(s => StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Disbursement || StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Reporting || StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Complete),
                    PendingCount = a.Students.Count(s => StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Review),
                    Status = a.Status,
                    LastModified = a.LastModified
                })
                .OrderBy(a => a.LastModified)
                .ToList();

            var model = new ApplicationListViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                Applications = applicationsWithPending,
                SearchCriteria = new ApplicationSearchCriteria()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Grants Team review queue");
            return View("Error");
        }
    }

    [Route("ReviewStudent/{id}")]
    public IActionResult ReviewStudent(int id)
    {
        var allApplications = _grantService.GetApplications();
        var studentWithApp = allApplications
            .SelectMany(a => a.Students.Select(s => new { Application = a, Student = s }))
            .FirstOrDefault(x => x.Student.Id == id);

        if (studentWithApp == null)
        {
            return NotFound($"Student {id} not found");
        }

        var application = studentWithApp.Application;
        var student = studentWithApp.Student;

        var model = new StudentReviewViewModel
        {
            ApplicationId = application.Id,
            IHEName = application.IHE.Name,
            LEAName = application.LEA.Name,
            Student = new StudentViewModel
            {
                Id = student.Id,
                ApplicationId = student.ApplicationId,
                SEID = student.SEID,
                FirstName = student.FirstName,
                LastName = student.LastName,
                CredentialArea = student.CredentialArea,
                Status = student.Status,
                AwardAmount = student.AwardAmount,
                CreatedAt = student.CreatedAt,
                SubmittedAt = student.SubmittedAt
            }
        };

        return View(model);
    }

    [Route("Analytics")]
    public IActionResult Analytics(int grantCycleId = 1)
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

            // Calculate comprehensive analytics
            var totalStudents = cycleApplications.SelectMany(a => a.Students).ToList();
            var credentialBreakdown = totalStudents
                .GroupBy(s => s.CredentialArea)
                .Select(g => new CredentialAreaBreakdown
                {
                    CredentialArea = g.Key,
                    StudentCount = g.Count(),
                    Percentage = (decimal)g.Count() / totalStudents.Count * 100
                })
                .OrderByDescending(x => x.StudentCount)
                .ToList();

            var model = new AnalyticsViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                BudgetOverview = new BudgetOverviewViewModel
                {
                    ApproprietedAmount = metrics.ApproprietedAmount,
                    ReservedAmount = metrics.ReservedAmount,
                    EncumberedAmount = metrics.EncumberedAmount,
                    DisbursedAmount = metrics.DisbursedAmount,
                    RemainingAmount = metrics.RemainingAmount,
                    OutstandingBalance = metrics.OutstandingBalance,
                    RemainingPercent = metrics.RemainingPercent,
                    EncumberedPercent = metrics.ApproprietedAmount > 0
                        ? (metrics.EncumberedAmount / metrics.ApproprietedAmount) * 100
                        : 0,
                    DisbursedPercent = metrics.ApproprietedAmount > 0
                        ? (metrics.DisbursedAmount / metrics.ApproprietedAmount) * 100
                        : 0
                },
                GrantSubmissions = new GrantSubmissionsViewModel
                {
                    TotalStudents = metrics.TotalStudents,
                    StudentsApproved = metrics.StatusCounts.Disbursement + metrics.StatusCounts.Reporting + metrics.StatusCounts.Complete,
                    StudentsUnderReview = metrics.StatusCounts.Submission + metrics.StatusCounts.Review,
                    StudentsPendingLEA = 0, // No longer tracked separately in workflow stages
                    StudentsDraft = 0, // No longer tracked separately in workflow stages
                    StudentsRejected = metrics.StatusCounts.Rejected,
                    StatusCounts = new StatusCountsViewModel
                    {
                        Submission = metrics.StatusCounts.Submission,
                        Review = metrics.StatusCounts.Review,
                        Disbursement = metrics.StatusCounts.Disbursement,
                        Reporting = metrics.StatusCounts.Reporting,
                        Rejected = metrics.StatusCounts.Rejected,
                        Complete = metrics.StatusCounts.Complete
                    }
                },
                ProgramOutcomes = new ProgramOutcomesViewModel
                {
                    TotalHoursCompleted = 0, // TODO: Implement when hours tracking is added
                    AverageHoursPerStudent = 0,
                    CompletionRate = metrics.StatusCounts.Complete > 0 && metrics.TotalStudents > 0
                        ? (decimal)metrics.StatusCounts.Complete / metrics.TotalStudents
                        : 0,
                    StudentsCompleting = metrics.StatusCounts.Complete,
                    CredentialAreasServed = credentialBreakdown.Count,
                    CredentialBreakdown = credentialBreakdown
                },
                GranteeReportingMetrics = new GranteeReportingMetricsViewModel
                {
                    TotalReportsRequired = 14, // Mock data - will be calculated from actual reports
                    ReportsSubmitted = 7,
                    ReportsOutstanding = 7,
                    ReportsUnderReview = 3,
                    ReportsApproved = 4,
                    IHEReportsSubmitted = 4,
                    LEAReportsSubmitted = 3,
                    SubmissionRate = 0.50m
                },
                FinancialSummary = new FinancialSummaryViewModel
                {
                    TotalDisbursements = cycleApplications.Count,
                    PendingGAAs = cycleApplications.Count(a => a.Status == "SUBMITTED"),
                    ActiveGAAs = cycleApplications.Count(a => a.Status == "APPROVED"),
                    CompletedPayments = 0, // TODO: Calculate from payment records
                    TotalAmountPaid = metrics.DisbursedAmount,
                    AveragePaymentAmount = metrics.StatusCounts.Disbursement > 0
                        ? metrics.DisbursedAmount / metrics.StatusCounts.Disbursement
                        : 0
                },
                PartnershipStatistics = new PartnershipStatisticsViewModel
                {
                    TotalIHEs = metrics.UniqueIHEs,
                    TotalLEAs = metrics.UniqueLEAs,
                    ActivePartnerships = metrics.ActivePartnerships,
                    IHEsWithSubmissions = cycleApplications.Select(a => a.IHE.Id).Distinct().Count(),
                    LEAsWithApprovedStudents = cycleApplications
                        .Where(a => a.Students.Any(s => s.Status == "APPROVED"))
                        .Select(a => a.LEA.Id)
                        .Distinct()
                        .Count()
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Analytics page");
            return View("Error");
        }
    }
}
