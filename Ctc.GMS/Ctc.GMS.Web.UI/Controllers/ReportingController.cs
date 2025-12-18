using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("GrantsTeam/Reporting")]
public class ReportingController : Controller
{
    private readonly IReportService _reportService;
    private readonly IGrantService _grantService;
    private readonly ILogger<ReportingController> _logger;

    public ReportingController(
        IReportService reportService,
        IGrantService grantService,
        ILogger<ReportingController> logger)
    {
        _reportService = reportService;
        _grantService = grantService;
        _logger = logger;
    }

    // Main Reporting Dashboard
    [Route("")]
    [Route("Dashboard")]
    public IActionResult Dashboard()
    {
        try
        {
            var metrics = _reportService.GetReportMetrics();
            var deadlines = _reportService.GetReportDeadlines();

            var model = new ReportingDashboardViewModel
            {
                Metrics = new ReportMetricsViewModel
                {
                    ReportsSubmitted = metrics.ReportsSubmitted,
                    ReportsOutstanding = metrics.ReportsOutstanding,
                    ReportsUnderReview = metrics.ReportsUnderReview,
                    ReportsApproved = metrics.ReportsApproved,
                    RevisionsRequested = metrics.RevisionsRequested,
                    IHEReportsSubmitted = metrics.IHEReportsSubmitted,
                    LEAReportsSubmitted = metrics.LEAReportsSubmitted,
                    NextReportingDeadline = metrics.NextReportingDeadline,
                    ComplianceRate = metrics.ComplianceRate
                },
                UpcomingDeadlines = deadlines.Take(5).Select(d => new ReportDeadlineViewModel
                {
                    ApplicationId = d.ApplicationId,
                    LEAName = d.LEAName,
                    IHEName = d.IHEName,
                    ReportingDeadline = d.ReportingDeadline,
                    DaysUntilDue = d.DaysUntilDue,
                    IsOverdue = d.IsOverdue
                }).ToList(),
                CurrentUser = User.Identity?.Name ?? "Grants Team Member"
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Reporting dashboard");
            return View("Error");
        }
    }

    // Submitted Reports List
    [Route("Submitted")]
    public IActionResult Submitted(string? status = null, string? search = null, string? sortBy = "SubmissionDate", string? sortOrder = "desc")
    {
        try
        {
            var iheReports = string.IsNullOrEmpty(status)
                ? _reportService.GetIHEReports()
                : _reportService.GetIHEReportsByStatus(status);

            var leaReports = string.IsNullOrEmpty(status)
                ? _reportService.GetLEAReports()
                : _reportService.GetLEAReportsByStatus(status);

            var iheReportSummaries = iheReports.Select(r => new ReportSummaryViewModel
            {
                Id = r.Id,
                ReportType = "IHE",
                StudentId = r.StudentId,
                StudentName = r.Student != null ? $"{r.Student.FirstName} {r.Student.LastName}" : "Unknown",
                ApplicationId = r.ApplicationId,
                LEAName = r.Application?.LEA.Name ?? "Unknown",
                IHEName = r.Application?.IHE.Name ?? "Unknown",
                SubmittedDate = r.SubmittedDate ?? DateTime.MinValue,
                Status = r.Status,
                StatusBadgeClass = GetStatusBadgeClass(r.Status),
                ReviewedBy = r.ReviewedBy,
                RevisionCount = r.RevisionCount
            }).ToList();

            var leaReportSummaries = leaReports.Select(r => new ReportSummaryViewModel
            {
                Id = r.Id,
                ReportType = "LEA",
                StudentId = r.StudentId,
                StudentName = r.Student != null ? $"{r.Student.FirstName} {r.Student.LastName}" : "Unknown",
                ApplicationId = r.ApplicationId,
                LEAName = r.Application?.LEA.Name ?? "Unknown",
                IHEName = r.Application?.IHE.Name ?? "Unknown",
                SubmittedDate = r.SubmittedDate ?? DateTime.MinValue,
                Status = r.Status,
                StatusBadgeClass = GetStatusBadgeClass(r.Status),
                ReviewedBy = r.ReviewedBy,
                RevisionCount = r.RevisionCount
            }).ToList();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                iheReportSummaries = iheReportSummaries
                    .Where(r => r.StudentName.ToLower().Contains(searchLower))
                    .ToList();
                leaReportSummaries = leaReportSummaries
                    .Where(r => r.StudentName.ToLower().Contains(searchLower))
                    .ToList();
            }

            // Sort reports
            if (sortBy == "SubmissionDate")
            {
                iheReportSummaries = sortOrder == "asc"
                    ? iheReportSummaries.OrderBy(r => r.SubmittedDate).ToList()
                    : iheReportSummaries.OrderByDescending(r => r.SubmittedDate).ToList();

                leaReportSummaries = sortOrder == "asc"
                    ? leaReportSummaries.OrderBy(r => r.SubmittedDate).ToList()
                    : leaReportSummaries.OrderByDescending(r => r.SubmittedDate).ToList();
            }
            else if (sortBy == "LEA")
            {
                iheReportSummaries = sortOrder == "asc"
                    ? iheReportSummaries.OrderBy(r => r.LEAName).ToList()
                    : iheReportSummaries.OrderByDescending(r => r.LEAName).ToList();

                leaReportSummaries = sortOrder == "asc"
                    ? leaReportSummaries.OrderBy(r => r.LEAName).ToList()
                    : leaReportSummaries.OrderByDescending(r => r.LEAName).ToList();
            }
            else if (sortBy == "IHE")
            {
                iheReportSummaries = sortOrder == "asc"
                    ? iheReportSummaries.OrderBy(r => r.IHEName).ToList()
                    : iheReportSummaries.OrderByDescending(r => r.IHEName).ToList();

                leaReportSummaries = sortOrder == "asc"
                    ? leaReportSummaries.OrderBy(r => r.IHEName).ToList()
                    : leaReportSummaries.OrderByDescending(r => r.IHEName).ToList();
            }
            else if (sortBy == "Student")
            {
                iheReportSummaries = sortOrder == "asc"
                    ? iheReportSummaries.OrderBy(r => r.StudentName).ToList()
                    : iheReportSummaries.OrderByDescending(r => r.StudentName).ToList();

                leaReportSummaries = sortOrder == "asc"
                    ? leaReportSummaries.OrderBy(r => r.StudentName).ToList()
                    : leaReportSummaries.OrderByDescending(r => r.StudentName).ToList();
            }

            var model = new SubmittedReportsViewModel
            {
                IHEReports = iheReportSummaries,
                LEAReports = leaReportSummaries,
                SelectedStatus = status ?? string.Empty,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading submitted reports");
            return View("Error");
        }
    }

    // Outstanding Reports
    [Route("Outstanding")]
    public IActionResult Outstanding()
    {
        try
        {
            var outstandingReports = _reportService.GetOutstandingReports();

            var model = new OutstandingReportsViewModel
            {
                OutstandingReports = outstandingReports.Select(o => new OutstandingReportItemViewModel
                {
                    ApplicationId = o.ApplicationId,
                    LEAId = o.LEAId,
                    LEAName = o.LEAName,
                    IHEId = o.IHEId,
                    IHEName = o.IHEName,
                    CandidatesPendingReport = o.CandidatesPendingReport,
                    PaymentDate = o.PaymentDate,
                    ReportingDeadline = o.ReportingDeadline,
                    DaysOverdue = o.DaysOverdue,
                    CriticallyOverdue = o.CriticallyOverdue
                }).ToList(),
                TotalOutstanding = outstandingReports.Sum(o => o.CandidatesPendingReport),
                CriticallyOverdue = outstandingReports.Count(o => o.CriticallyOverdue)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading outstanding reports");
            return View("Error");
        }
    }

    // Individual Report Review
    [Route("Review/{reportType}/{id}")]
    public IActionResult Review(string reportType, int id)
    {
        try
        {
            if (reportType.ToUpper() == "IHE")
            {
                var report = _reportService.GetIHEReport(id);
                if (report == null)
                    return NotFound($"IHE Report {id} not found");

                var application = _grantService.GetApplication(report.ApplicationId);
                var student = application?.Students.FirstOrDefault(s => s.Id == report.StudentId);

                if (application == null || student == null)
                    return NotFound("Related application or student not found");

                var model = new ReportReviewViewModel
                {
                    ReportType = "IHE",
                    ReportId = report.Id,
                    StudentId = report.StudentId,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    SEID = student.SEID,
                    CredentialArea = student.CredentialArea,
                    ApplicationId = report.ApplicationId,
                    LEAName = application.LEA.Name,
                    IHEName = application.IHE.Name,
                    SubmittedDate = report.SubmittedDate ?? DateTime.MinValue,
                    SubmittedBy = report.SubmittedBy,
                    SubmittedByEmail = report.SubmittedByEmail,
                    Status = report.Status,
                    ReviewedBy = report.ReviewedBy,
                    ReviewedDate = report.ReviewedDate,
                    RevisionCount = report.RevisionCount,
                    RevisionNotes = report.RevisionNotes,
                    InternalNotes = report.InternalNotes,
                    IHEDetails = new IHEReportDetailsViewModel
                    {
                        CompletionStatus = report.CompletionStatus,
                        CompletionDate = report.CompletionDate,
                        DenialReason = report.DenialReason,
                        SwitchedToIntern = report.SwitchedToIntern,
                        InternSwitchDate = report.InternSwitchDate,
                        GrantProgramHours = report.GrantProgramHours,
                        Met500Hours = report.Met500Hours,
                        GrantProgramHoursNotes = report.GrantProgramHoursNotes,
                        CredentialProgramHours = report.CredentialProgramHours,
                        Met600Hours = report.Met600Hours,
                        CredentialProgramHoursNotes = report.CredentialProgramHoursNotes,
                        AdditionalNotes = report.AdditionalNotes,
                        DocumentationUrl = report.DocumentationUrl
                    }
                };

                return View(model);
            }
            else if (reportType.ToUpper() == "LEA")
            {
                var report = _reportService.GetLEAReport(id);
                if (report == null)
                    return NotFound($"LEA Report {id} not found");

                var application = _grantService.GetApplication(report.ApplicationId);
                var student = application?.Students.FirstOrDefault(s => s.Id == report.StudentId);

                if (application == null || student == null)
                    return NotFound("Related application or student not found");

                var model = new ReportReviewViewModel
                {
                    ReportType = "LEA",
                    ReportId = report.Id,
                    StudentId = report.StudentId,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    SEID = student.SEID,
                    CredentialArea = student.CredentialArea,
                    ApplicationId = report.ApplicationId,
                    LEAName = application.LEA.Name,
                    IHEName = application.IHE.Name,
                    SubmittedDate = report.SubmittedDate ?? DateTime.MinValue,
                    SubmittedBy = report.SubmittedBy,
                    SubmittedByEmail = report.SubmittedByEmail,
                    Status = report.Status,
                    ReviewedBy = report.ReviewedBy,
                    ReviewedDate = report.ReviewedDate,
                    RevisionCount = report.RevisionCount,
                    RevisionNotes = report.RevisionNotes,
                    InternalNotes = report.InternalNotes,
                    LEADetails = new LEAReportDetailsViewModel
                    {
                        PaymentCategory = report.PaymentCategory,
                        PaymentSchedule = report.PaymentSchedule,
                        ActualPaymentAmount = report.ActualPaymentAmount,
                        PaymentScheduleDetails = report.PaymentScheduleDetails,
                        FirstPaymentDate = report.FirstPaymentDate,
                        FinalPaymentDate = report.FinalPaymentDate,
                        PaymentNotes = report.PaymentNotes,
                        HiredInDistrict = report.HiredInDistrict,
                        EmploymentStatus = report.EmploymentStatus,
                        HireDate = report.HireDate,
                        JobTitle = report.JobTitle,
                        SchoolSite = report.SchoolSite,
                        AdditionalNotes = report.AdditionalNotes,
                        DocumentationUrl = report.DocumentationUrl
                    }
                };

                return View(model);
            }

            return BadRequest("Invalid report type");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading report review for {ReportType} report {Id}", reportType, id);
            return View("Error");
        }
    }

    // Approve Report
    [HttpPost]
    [Route("Approve/{reportType}/{id}")]
    public IActionResult Approve(string reportType, int id)
    {
        try
        {
            var reviewerName = User.Identity?.Name ?? "Grants Team Member";

            if (reportType.ToUpper() == "IHE")
            {
                _reportService.ApproveIHEReport(id, reviewerName);
            }
            else if (reportType.ToUpper() == "LEA")
            {
                _reportService.ApproveLEAReport(id, reviewerName);
            }
            else
            {
                return BadRequest("Invalid report type");
            }

            TempData["SuccessMessage"] = "Report approved successfully.";
            return RedirectToAction("Review", new { reportType, id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving {ReportType} report {Id}", reportType, id);
            TempData["ErrorMessage"] = "Error approving report.";
            return RedirectToAction("Review", new { reportType, id });
        }
    }

    // Request Revisions
    [HttpPost]
    [Route("RequestRevisions/{reportType}/{id}")]
    public IActionResult RequestRevisions(string reportType, int id, string revisionNotes)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(revisionNotes))
            {
                TempData["ErrorMessage"] = "Revision notes are required.";
                return RedirectToAction("Review", new { reportType, id });
            }

            var reviewerName = User.Identity?.Name ?? "Grants Team Member";

            if (reportType.ToUpper() == "IHE")
            {
                _reportService.RequestIHEReportRevisions(id, reviewerName, revisionNotes);
            }
            else if (reportType.ToUpper() == "LEA")
            {
                _reportService.RequestLEAReportRevisions(id, reviewerName, revisionNotes);
            }
            else
            {
                return BadRequest("Invalid report type");
            }

            TempData["SuccessMessage"] = "Revisions requested successfully.";
            return RedirectToAction("Review", new { reportType, id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting revisions for {ReportType} report {Id}", reportType, id);
            TempData["ErrorMessage"] = "Error requesting revisions.";
            return RedirectToAction("Review", new { reportType, id });
        }
    }

    // Analytics Dashboard
    [Route("Analytics")]
    public IActionResult Analytics()
    {
        try
        {
            var analytics = _reportService.GetReportAnalytics();

            var model = new ReportAnalyticsViewModel
            {
                Summary = new ReportAnalyticsSummaryViewModel
                {
                    TotalCandidatesFunded = analytics.TotalCandidatesFunded,
                    TotalCandidatesReported = analytics.TotalCandidatesReported,
                    ReportingComplianceRate = analytics.ReportingComplianceRate,
                    ProgramCompletions = analytics.ProgramCompletions,
                    ProgramCompletionRate = analytics.ProgramCompletionRate,
                    CredentialsEarned = analytics.CredentialsEarned,
                    CredentialEarnRate = analytics.CredentialEarnRate,
                    CandidatesEmployed = analytics.CandidatesEmployed,
                    EmploymentRate = analytics.EmploymentRate,
                    HiredInDistrict = analytics.HiredInDistrict,
                    HiredInDistrictRate = analytics.HiredInDistrictRate,
                    AverageGrantProgramHours = analytics.AverageGrantProgramHours,
                    AverageCredentialProgramHours = analytics.AverageCredentialProgramHours,
                    Met500HoursCount = analytics.Met500HoursCount,
                    Met600HoursCount = analytics.Met600HoursCount,
                    TotalAmountDisbursed = analytics.TotalAmountDisbursed,
                    AveragePaymentAmount = analytics.AveragePaymentAmount,
                    EmploymentByStatus = analytics.EmploymentByStatus
                },
                Filter = new ReportAnalyticsFilterViewModel()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading analytics dashboard");
            return View("Error");
        }
    }

    // Compliance Dashboard
    [Route("Compliance")]
    public IActionResult Compliance()
    {
        try
        {
            var compliance = _reportService.GetComplianceMetrics();

            var model = new ComplianceDashboardViewModel
            {
                Metrics = new ComplianceMetricsViewModel
                {
                    TotalLEAs = compliance.TotalLEAs,
                    LEAsFullCompliance = compliance.LEAsFullCompliance,
                    LEAsPartialCompliance = compliance.LEAsPartialCompliance,
                    LEAsNoCompliance = compliance.LEAsNoCompliance
                },
                LEACompliance = compliance.LEACompliance.Select(l => new LEAComplianceItemViewModel
                {
                    LEAId = l.LEAId,
                    LEAName = l.LEAName,
                    ReportsRequired = l.ReportsRequired,
                    ReportsSubmitted = l.ReportsSubmitted,
                    CompliancePercentage = l.CompliancePercentage,
                    CriticallyOverdue = l.CriticallyOverdue,
                    ComplianceStatus = GetComplianceStatus(l.CompliancePercentage),
                    StatusClass = GetComplianceStatusClass(l.CompliancePercentage)
                }).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading compliance dashboard");
            return View("Error");
        }
    }

    // Helper methods
    private string GetStatusBadgeClass(string status)
    {
        return status switch
        {
            "SUBMITTED" => "badge-secondary",
            "UNDER_REVIEW" => "badge-primary",
            "APPROVED" => "badge-success",
            "REVISIONS_REQUESTED" => "badge-warning",
            _ => "badge-secondary"
        };
    }

    private string GetComplianceStatus(double percentage)
    {
        if (percentage >= 100) return "Full Compliance";
        if (percentage > 0) return "Partial Compliance";
        return "No Compliance";
    }

    private string GetComplianceStatusClass(double percentage)
    {
        if (percentage >= 100) return "success";
        if (percentage >= 50) return "warning";
        return "danger";
    }

    // Quick Access Reports
    [Route("DemographicsReport")]
    public IActionResult DemographicsReport()
    {
        ViewBag.ReportTitle = "Current Demographics Report";
        ViewBag.GrantCycle = "FY 2025-26";
        ViewBag.GeneratedDate = DateTime.Now;
        return View();
    }

    [Route("LEAPerformanceReport")]
    public IActionResult LEAPerformanceReport()
    {
        ViewBag.ReportTitle = "LEA Performance Report";
        ViewBag.GrantCycle = "FY 2025-26";
        ViewBag.GeneratedDate = DateTime.Now;
        return View();
    }

    [Route("FinancialSummaryReport")]
    public IActionResult FinancialSummaryReport()
    {
        ViewBag.ReportTitle = "Financial Summary Report";
        ViewBag.GrantCycle = "FY 2025-26";
        ViewBag.GeneratedDate = DateTime.Now;
        return View();
    }

    [Route("ProgramOutcomesReport")]
    public IActionResult ProgramOutcomesReport()
    {
        ViewBag.ReportTitle = "Program Outcomes Report";
        ViewBag.GrantCycle = "FY 2025-26";
        ViewBag.GeneratedDate = DateTime.Now;
        return View();
    }
}
