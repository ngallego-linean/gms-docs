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
    public IActionResult Analytics(string? program = null, string? fiscalYear = null, string? viewMode = "Overall")
    {
        try
        {
            var analytics = _reportService.GetReportAnalytics();

            // Mock financial data for Module 4 enhancements
            // Force mock values for POC consistency
            var budgetAppropriated = 5000000m;
            var encumbered = 2140000m;
            var disbursed = 1650000m; // Use mock value for POC to match program breakdown

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
                    TotalAmountDisbursed = disbursed,
                    AveragePaymentAmount = analytics.AveragePaymentAmount,

                    // Enhanced Financial Analytics (Module 4)
                    TotalBudgetAppropriated = budgetAppropriated,
                    TotalEncumbered = encumbered,
                    TotalRemaining = budgetAppropriated - encumbered,

                    // Monthly disbursement trend (cumulative, current FY)
                    MonthlyDisbursements = new Dictionary<string, decimal>
                    {
                        { "Jul", 0m },
                        { "Aug", 120000m },
                        { "Sep", 400000m },
                        { "Oct", 860000m },
                        { "Nov", 1650000m },
                        { "Dec", 0m },  // Future months
                        { "Jan", 0m },
                        { "Feb", 0m },
                        { "Mar", 0m },
                        { "Apr", 0m },
                        { "May", 0m },
                        { "Jun", 0m }
                    },

                    // Prior year for comparison
                    PriorYearMonthlyDisbursements = new Dictionary<string, decimal>
                    {
                        { "Jul", 0m },
                        { "Aug", 180000m },
                        { "Sep", 520000m },
                        { "Oct", 1100000m },
                        { "Nov", 1850000m },
                        { "Dec", 2400000m },
                        { "Jan", 2900000m },
                        { "Feb", 3300000m },
                        { "Mar", 3650000m },
                        { "Apr", 3900000m },
                        { "May", 4050000m },
                        { "Jun", 4050000m }
                    },

                    // Disbursements by program
                    DisbursementsByProgram = new Dictionary<string, ProgramDisbursementSummary>
                    {
                        { "TRP", new ProgramDisbursementSummary
                            {
                                ProgramName = "Teacher Residency (TRP)",
                                AmountDisbursed = 1200000m,
                                CandidateCount = 120,
                                PercentOfTotal = 72.7
                            }
                        },
                        { "SCR", new ProgramDisbursementSummary
                            {
                                ProgramName = "School Counselor Residency",
                                AmountDisbursed = 280000m,
                                CandidateCount = 28,
                                PercentOfTotal = 17.0
                            }
                        },
                        { "C2C", new ProgramDisbursementSummary
                            {
                                ProgramName = "Classified to Certificated",
                                AmountDisbursed = 170000m,
                                CandidateCount = 17,
                                PercentOfTotal = 10.3
                            }
                        }
                    },

                    EmploymentByStatus = analytics.EmploymentByStatus,

                    // Demographics
                    CandidatesByRaceEthnicity = analytics.CandidatesByRaceEthnicity,
                    CandidatesByGender = analytics.CandidatesByGender,
                    CandidatesByCredentialArea = analytics.CandidatesByCredentialArea,
                    CandidatesByRaceEthnicityByYear = analytics.CandidatesByRaceEthnicityByYear,
                    CandidatesByGenderByYear = analytics.CandidatesByGenderByYear,
                    CandidatesByCredentialAreaByYear = analytics.CandidatesByCredentialAreaByYear
                },
                Filter = new ReportAnalyticsFilterViewModel
                {
                    Program = program,
                    FiscalYear = fiscalYear,
                    ViewMode = viewMode ?? "Overall"
                },
                ProgramOptions = new List<FilterOption>
                {
                    new FilterOption("", "All Programs"),
                    new FilterOption("TRP", "Teacher Residency (TRP)"),
                    new FilterOption("SCR", "School Counselor Residency"),
                    new FilterOption("C2C", "Classified to Certificated")
                },
                FiscalYearOptions = new List<FilterOption>
                {
                    new FilterOption("", "All Years"),
                    new FilterOption("2024-25", "2024-25"),
                    new FilterOption("2023-24", "2023-24"),
                    new FilterOption("2022-23", "2022-23"),
                    new FilterOption("2021-22", "2021-22"),
                    new FilterOption("2020-21", "2020-21")
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading analytics dashboard");
            return View("Error");
        }
    }

    // Compliance Dashboard (Legacy - redirects to Status)
    [Route("Compliance")]
    public IActionResult Compliance()
    {
        return RedirectToAction("Status", new { tab = "LEA" });
    }

    // Unified Reporting Status Dashboard
    [Route("Status")]
    public IActionResult Status(string? tab = "LEA", string? expand = null, string? fiscalYear = "2024-25")
    {
        try
        {
            var model = new ReportingStatusViewModel
            {
                ActiveTab = tab ?? "LEA",
                ExpandedStatus = expand,
                FiscalYear = fiscalYear ?? "2024-25",
                FiscalYearOptions = new List<FilterOption>
                {
                    new FilterOption("2024-25", "2024-25"),
                    new FilterOption("2023-24", "2023-24"),
                    new FilterOption("2022-23", "2022-23"),
                    new FilterOption("2021-22", "2021-22"),
                    new FilterOption("2020-21", "2020-21")
                }
            };

            // Build LEA Status Data
            model.LEA = BuildLEAStatusSection();

            // Build IHE Status Data
            model.IHE = BuildIHEStatusSection();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reporting status dashboard");
            return View("Error");
        }
    }

    private OrganizationStatusSection BuildLEAStatusSection()
    {
        var compliance = _reportService.GetComplianceMetrics();

        var leaSection = new OrganizationStatusSection
        {
            TypeLabel = "LEAs",
            ReportDescription = "Employment outcomes and retention data",
            Metrics = new OrganizationStatusMetrics
            {
                Total = compliance.TotalLEAs,
                Complete = compliance.LEAsFullCompliance,
                InProgress = compliance.LEAsPartialCompliance,
                NotStarted = compliance.LEAsNoCompliance
            },
            Organizations = compliance.LEACompliance.Select(l => new OrganizationStatusItem
            {
                Id = l.LEAId,
                Name = l.LEAName,
                ReportsRequired = l.ReportsRequired,
                ReportsSubmitted = l.ReportsSubmitted,
                ProgressPercent = l.CompliancePercentage,
                Status = GetOrganizationStatus(l.CompliancePercentage),
                OverdueCount = l.CriticallyOverdue,
                HasCriticallyOverdue = l.CriticallyOverdue > 0,
                Candidates = new List<CandidateReportStatus>() // Populated on drill-down
            }).ToList()
        };

        // Calculate candidate totals
        leaSection.Metrics.TotalCandidates = leaSection.Organizations.Sum(o => o.ReportsRequired);
        leaSection.Metrics.CandidatesReported = leaSection.Organizations
            .Where(o => o.Status == "Complete")
            .Sum(o => o.ReportsSubmitted);
        leaSection.Metrics.CandidatesPending = leaSection.Organizations
            .Where(o => o.Status == "InProgress")
            .Sum(o => o.ReportsRequired - o.ReportsSubmitted);
        leaSection.Metrics.CandidatesNotStarted = leaSection.Organizations
            .Where(o => o.Status == "NotStarted")
            .Sum(o => o.ReportsRequired);
        leaSection.CriticallyOverdueCount = leaSection.Organizations.Count(o => o.HasCriticallyOverdue);

        return leaSection;
    }

    private OrganizationStatusSection BuildIHEStatusSection()
    {
        // For IHEs, we'll use mock data similar to LEA structure
        // In production, this would call _reportService.GetIHEComplianceMetrics()
        var iheData = GetMockIHEStatusData();

        var iheSection = new OrganizationStatusSection
        {
            TypeLabel = "IHEs",
            ReportDescription = "Program completion and credentials earned",
            Metrics = new OrganizationStatusMetrics
            {
                Total = iheData.Count,
                Complete = iheData.Count(i => i.Status == "Complete"),
                InProgress = iheData.Count(i => i.Status == "InProgress"),
                NotStarted = iheData.Count(i => i.Status == "NotStarted")
            },
            Organizations = iheData
        };

        // Calculate candidate totals
        iheSection.Metrics.TotalCandidates = iheSection.Organizations.Sum(o => o.ReportsRequired);
        iheSection.Metrics.CandidatesReported = iheSection.Organizations
            .Where(o => o.Status == "Complete")
            .Sum(o => o.ReportsSubmitted);
        iheSection.Metrics.CandidatesPending = iheSection.Organizations
            .Where(o => o.Status == "InProgress")
            .Sum(o => o.ReportsRequired - o.ReportsSubmitted);
        iheSection.Metrics.CandidatesNotStarted = iheSection.Organizations
            .Where(o => o.Status == "NotStarted")
            .Sum(o => o.ReportsRequired);
        iheSection.CriticallyOverdueCount = iheSection.Organizations.Count(o => o.HasCriticallyOverdue);

        return iheSection;
    }

    private List<OrganizationStatusItem> GetMockIHEStatusData()
    {
        return new List<OrganizationStatusItem>
        {
            new OrganizationStatusItem { Id = 1, Name = "California State University, Long Beach", ReportsRequired = 45, ReportsSubmitted = 45, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 2, Name = "University of Southern California", ReportsRequired = 38, ReportsSubmitted = 38, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 3, Name = "Stanford University", ReportsRequired = 22, ReportsSubmitted = 22, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 4, Name = "UCLA", ReportsRequired = 52, ReportsSubmitted = 52, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 5, Name = "UC Berkeley", ReportsRequired = 35, ReportsSubmitted = 35, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 6, Name = "San Diego State University", ReportsRequired = 28, ReportsSubmitted = 28, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 7, Name = "California State University, Fresno", ReportsRequired = 32, ReportsSubmitted = 32, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 8, Name = "University of San Francisco", ReportsRequired = 18, ReportsSubmitted = 18, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 9, Name = "Loyola Marymount University", ReportsRequired = 15, ReportsSubmitted = 15, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 10, Name = "Pepperdine University", ReportsRequired = 12, ReportsSubmitted = 12, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 11, Name = "California State University, Sacramento", ReportsRequired = 25, ReportsSubmitted = 25, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 12, Name = "San Jose State University", ReportsRequired = 30, ReportsSubmitted = 30, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 13, Name = "California State University, Fullerton", ReportsRequired = 20, ReportsSubmitted = 20, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 14, Name = "UC Irvine", ReportsRequired = 24, ReportsSubmitted = 24, ProgressPercent = 100, Status = "Complete", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 15, Name = "California State University, Northridge", ReportsRequired = 28, ReportsSubmitted = 18, ProgressPercent = 64.3, Status = "InProgress", OverdueCount = 2, HasCriticallyOverdue = true },
            new OrganizationStatusItem { Id = 16, Name = "UC Davis", ReportsRequired = 22, ReportsSubmitted = 14, ProgressPercent = 63.6, Status = "InProgress", OverdueCount = 0, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 17, Name = "California State University, Dominguez Hills", ReportsRequired = 15, ReportsSubmitted = 8, ProgressPercent = 53.3, Status = "InProgress", OverdueCount = 1, HasCriticallyOverdue = false },
            new OrganizationStatusItem { Id = 18, Name = "Azusa Pacific University", ReportsRequired = 12, ReportsSubmitted = 0, ProgressPercent = 0, Status = "NotStarted", OverdueCount = 0, HasCriticallyOverdue = false }
        };
    }

    private string GetOrganizationStatus(double percentage)
    {
        if (percentage >= 100) return "Complete";
        if (percentage > 0) return "InProgress";
        return "NotStarted";
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
        ViewBag.ReportTitle = "Candidate Pipeline Analysis";
        ViewBag.GrantCycle = "FY 2024-25";
        ViewBag.GeneratedDate = DateTime.Now;
        return View();
    }

    [Route("LEAPerformanceReport")]
    public IActionResult LEAPerformanceReport()
    {
        ViewBag.ReportTitle = "Partnership Outcomes Report";
        ViewBag.GrantCycle = "FY 2024-25";
        ViewBag.GeneratedDate = DateTime.Now;
        return View();
    }

    [Route("FinancialSummaryReport")]
    public IActionResult FinancialSummaryReport()
    {
        ViewBag.ReportTitle = "Investment Efficiency Report";
        ViewBag.GrantCycle = "FY 2024-25";
        ViewBag.GeneratedDate = DateTime.Now;
        return View();
    }

    [Route("ProgramOutcomesReport")]
    public IActionResult ProgramOutcomesReport()
    {
        ViewBag.ReportTitle = "Program Impact Report";
        ViewBag.GrantCycle = "FY 2024-25";
        ViewBag.GeneratedDate = DateTime.Now;
        return View();
    }

    [Route("ProcessingEfficiencyReport")]
    public IActionResult ProcessingEfficiencyReport()
    {
        ViewBag.ReportTitle = "Processing Efficiency Report";
        ViewBag.GrantCycle = "FY 2024-25";
        ViewBag.GeneratedDate = DateTime.Now;
        return View();
    }

    // ===========================================
    // REPORT EXPORT ACTIONS (Module 7)
    // ===========================================

    /// <summary>
    /// Export a report as PDF (ADA-compliant, tables only)
    /// </summary>
    [Route("ExportPdf")]
    public IActionResult ExportPdf(string reportType, int? id = null)
    {
        try
        {
            // Get report data based on type
            var reportData = GetReportDataForExport(reportType, id);
            if (reportData == null)
            {
                TempData["ErrorMessage"] = $"Report type '{reportType}' not found.";
                return RedirectToAction("Dashboard");
            }

            // Set ViewBag for report metadata
            ViewBag.ReportTitle = reportData.Title;
            ViewBag.ReportType = reportType;
            ViewBag.ReportId = id;
            ViewBag.GrantCycle = "FY 2024-25";
            ViewBag.GeneratedDate = DateTime.Now;
            ViewBag.ExportMode = "PDF";
            ViewBag.IsFormalReport = true; // Force ADA-compliant mode

            // For POC: Return a view that can be printed to PDF
            // In production, use a library like Rotativa, DinkToPdf, or iTextSharp
            // Example with Rotativa: return new ViewAsPdf(reportData.ViewName, reportData.Model);

            // Set content disposition for download prompt
            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{reportData.FileName}.pdf\"");

            // For now, redirect to the report with a flag to indicate formal mode
            TempData["InfoMessage"] = "PDF export: Use browser print (Ctrl+P) and select 'Save as PDF'. The report is displayed in formal/ADA-compliant mode.";

            return RedirectToAction(reportData.ActionName, new { formalMode = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting PDF for report type {ReportType}", reportType);
            TempData["ErrorMessage"] = "Error generating PDF export.";
            return RedirectToAction("Dashboard");
        }
    }

    /// <summary>
    /// Export a report as Excel (raw data tables)
    /// </summary>
    [Route("ExportExcel")]
    public IActionResult ExportExcel(string reportType, int? id = null)
    {
        try
        {
            // Get report data based on type
            var reportData = GetReportDataForExport(reportType, id);
            if (reportData == null)
            {
                TempData["ErrorMessage"] = $"Report type '{reportType}' not found.";
                return RedirectToAction("Dashboard");
            }

            // Build Excel content using CSV format (simple, works everywhere)
            // In production, use EPPlus or ClosedXML for proper .xlsx files
            var csvContent = GenerateExcelContent(reportType, id);

            var fileName = $"{reportData.FileName}_{DateTime.Now:yyyyMMdd}.csv";
            var contentType = "text/csv";

            // Return as file download
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting Excel for report type {ReportType}", reportType);
            TempData["ErrorMessage"] = "Error generating Excel export.";
            return RedirectToAction("Dashboard");
        }
    }

    /// <summary>
    /// Get report metadata for export
    /// </summary>
    private ReportExportData? GetReportDataForExport(string reportType, int? id)
    {
        return reportType?.ToLower() switch
        {
            "demographics" => new ReportExportData
            {
                Title = "Candidate Pipeline Analysis",
                ActionName = "DemographicsReport",
                ViewName = "DemographicsReport",
                FileName = "CandidatePipelineAnalysis"
            },
            "financial" => new ReportExportData
            {
                Title = "Investment Efficiency Report",
                ActionName = "FinancialSummaryReport",
                ViewName = "FinancialSummaryReport",
                FileName = "InvestmentEfficiencyReport"
            },
            "lea" or "leaperformance" => new ReportExportData
            {
                Title = "Partnership Outcomes Report",
                ActionName = "LEAPerformanceReport",
                ViewName = "LEAPerformanceReport",
                FileName = "PartnershipOutcomesReport"
            },
            "outcomes" or "programoutcomes" => new ReportExportData
            {
                Title = "Program Impact Report",
                ActionName = "ProgramOutcomesReport",
                ViewName = "ProgramOutcomesReport",
                FileName = "ProgramImpactReport"
            },
            _ => null
        };
    }

    /// <summary>
    /// Generate CSV/Excel content for a report
    /// </summary>
    private string GenerateExcelContent(string reportType, int? id)
    {
        var sb = new System.Text.StringBuilder();

        // Add metadata header
        sb.AppendLine("California Commission on Teacher Credentialing");
        sb.AppendLine($"Report Type,{reportType}");
        sb.AppendLine($"Generated,{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Grant Cycle,FY 2024-25");
        sb.AppendLine();

        // Generate report-specific data
        switch (reportType?.ToLower())
        {
            case "demographics":
                GenerateDemographicsExcelContent(sb);
                break;
            case "financial":
                GenerateFinancialExcelContent(sb);
                break;
            case "lea":
            case "leaperformance":
                GenerateLEAPerformanceExcelContent(sb);
                break;
            case "outcomes":
            case "programoutcomes":
                GenerateProgramOutcomesExcelContent(sb);
                break;
            default:
                sb.AppendLine("No data available for this report type.");
                break;
        }

        return sb.ToString();
    }

    private void GenerateDemographicsExcelContent(System.Text.StringBuilder sb)
    {
        // Summary Statistics
        sb.AppendLine("SUMMARY STATISTICS");
        sb.AppendLine("Metric,Value");
        sb.AppendLine("Total Candidates,214");
        sb.AppendLine("Unique LEAs,48");
        sb.AppendLine("Unique IHEs,12");
        sb.AppendLine("Hispanic/Latino %,42.1%");
        sb.AppendLine("Male %,26.2%");
        sb.AppendLine("Underrepresented %,51.4%");
        sb.AppendLine();

        // Race/Ethnicity Distribution
        sb.AppendLine("RACE/ETHNICITY DISTRIBUTION");
        sb.AppendLine("Category,Program %,Prior Year %,CA Teachers %,CA Students %");
        sb.AppendLine("Hispanic/Latino,42.1%,39.4%,24.0%,55.0%");
        sb.AppendLine("White,32.2%,34.8%,54.0%,22.0%");
        sb.AppendLine("Asian,12.1%,11.9%,9.0%,12.0%");
        sb.AppendLine("Black/African American,6.5%,6.8%,4.0%,5.0%");
        sb.AppendLine("Two or More Races,4.7%,4.5%,3.0%,4.0%");
        sb.AppendLine("Other,2.4%,2.6%,6.0%,2.0%");
        sb.AppendLine();

        // Gender Distribution
        sb.AppendLine("GENDER DISTRIBUTION");
        sb.AppendLine("Category,Program %,Prior Year %,CA Teachers %");
        sb.AppendLine("Female,71.5%,69.8%,72.0%");
        sb.AppendLine("Male,26.2%,28.1%,28.0%");
        sb.AppendLine("Non-binary/Other,2.3%,2.1%,0.0%");
        sb.AppendLine();

        // Demographics by Credential Area
        sb.AppendLine("DEMOGRAPHICS BY CREDENTIAL AREA");
        sb.AppendLine("Credential Area,Hispanic/Latino %,Male %,Underrepresented %");
        sb.AppendLine("Multiple Subject,45.6%,18.2%,52.1%");
        sb.AppendLine("Single Subject - Math,38.1%,42.9%,47.6%");
        sb.AppendLine("Single Subject - English,39.3%,21.4%,46.4%");
        sb.AppendLine("Single Subject - Science,41.7%,37.5%,50.0%");
        sb.AppendLine("Education Specialist,44.2%,15.4%,51.9%");
    }

    private void GenerateFinancialExcelContent(System.Text.StringBuilder sb)
    {
        // Budget Status
        sb.AppendLine("BUDGET STATUS");
        sb.AppendLine("Category,FY 2024-25,FY 2023-24");
        sb.AppendLine("Appropriated,$5000000,$4500000");
        sb.AppendLine("Encumbered,$2140000,$4200000");
        sb.AppendLine("Disbursed,$1650000,$4050000");
        sb.AppendLine("Remaining,$2860000,$300000");
        sb.AppendLine();

        // Investment Efficiency
        sb.AppendLine("INVESTMENT EFFICIENCY");
        sb.AppendLine("Metric,Target,FY 2023-24,FY 2024-25");
        sb.AppendLine("Cost per Student Funded,$10000,$20455,$7710");
        sb.AppendLine("Cost per Credential Earned,$12000,$25633,$10185");
        sb.AppendLine("Cost per Teacher Employed,$13000,$27365,$10443");
        sb.AppendLine("Cost per Teacher in District,$15000,$32143,$11620");
        sb.AppendLine();

        // Monthly Disbursements
        sb.AppendLine("MONTHLY DISBURSEMENTS (CUMULATIVE)");
        sb.AppendLine("Month,FY 2024-25,FY 2023-24");
        sb.AppendLine("Jul,$0,$0");
        sb.AppendLine("Aug,$120000,$180000");
        sb.AppendLine("Sep,$400000,$520000");
        sb.AppendLine("Oct,$860000,$1100000");
        sb.AppendLine("Nov,$1650000,$1850000");
        sb.AppendLine("Dec,,$2400000");
        sb.AppendLine("Jan,,$2900000");
        sb.AppendLine("Feb,,$3300000");
        sb.AppendLine("Mar,,$3650000");
        sb.AppendLine("Apr,,$3900000");
        sb.AppendLine("May,,$4050000");
        sb.AppendLine("Jun,,$4050000");
    }

    private void GenerateLEAPerformanceExcelContent(System.Text.StringBuilder sb)
    {
        // Partnership Summary
        sb.AppendLine("PARTNERSHIP SUMMARY");
        sb.AppendLine("Metric,FY 2024-25,FY 2023-24");
        sb.AppendLine("Active Partnerships,10,8");
        sb.AppendLine("Candidates Funded,214,178");
        sb.AppendLine("Avg Completion Rate,87.0%,82.4%");
        sb.AppendLine("Avg Credential Rate,90.0%,85.2%");
        sb.AppendLine("Avg Employment Rate,84.4%,79.8%");
        sb.AppendLine("Avg Retention Rate,89.3%,86.5%");
        sb.AppendLine();

        // Partnership Details
        sb.AppendLine("PARTNERSHIP DETAILS");
        sb.AppendLine("IHE,LEA,Funded,Completed,Credentialed,Employed,In District,Completion %,Credential %,Employment %,Retention %");
        sb.AppendLine("UCLA,Los Angeles USD,32,29,27,26,24,90.6%,93.1%,89.7%,92.3%");
        sb.AppendLine("SDSU,San Diego USD,28,24,22,21,19,85.7%,91.7%,87.5%,90.5%");
        sb.AppendLine("Fresno State,Fresno USD,26,23,20,19,16,88.5%,87.0%,82.6%,84.2%");
        sb.AppendLine("Sac State,Sacramento City USD,24,21,19,18,17,87.5%,90.5%,85.7%,94.4%");
        sb.AppendLine("Cal State East Bay,Oakland USD,22,19,17,16,14,86.4%,89.5%,84.2%,87.5%");
        sb.AppendLine("CSU Long Beach,Long Beach USD,20,18,17,16,15,90.0%,94.4%,88.9%,93.8%");
        sb.AppendLine("SF State,San Francisco USD,18,16,15,14,13,88.9%,93.8%,87.5%,92.9%");
        sb.AppendLine("San Jose State,San Jose USD,16,14,12,11,9,87.5%,85.7%,78.6%,81.8%");
        sb.AppendLine("CSU Fullerton,Santa Ana USD,14,12,11,10,9,85.7%,91.7%,83.3%,90.0%");
        sb.AppendLine("CSU Bakersfield,Bakersfield City SD,14,11,9,7,6,78.6%,81.8%,63.6%,85.7%");
    }

    private void GenerateProgramOutcomesExcelContent(System.Text.StringBuilder sb)
    {
        // Program Goals vs Actuals
        sb.AppendLine("PROGRAM GOALS VS ACTUALS");
        sb.AppendLine("Metric,Target,FY 2023-24,FY 2024-25");
        sb.AppendLine("Completion Rate,85.0%,82.3%,87.4%");
        sb.AppendLine("Credential Rate,80.0%,80.1%,86.6%");
        sb.AppendLine("Employment Rate,80.0%,79.8%,84.5%");
        sb.AppendLine("Hired in District,75.0%,85.2%,89.9%");
        sb.AppendLine();

        // Outcomes by Credential Area
        sb.AppendLine("OUTCOMES BY CREDENTIAL AREA");
        sb.AppendLine("Credential Area,Funded,Completed,Completion %,Credentialed,Credential %,Employed,Employment %");
        sb.AppendLine("Multiple Subject,68,62,91.2%,58,93.5%,54,93.1%");
        sb.AppendLine("Single Subject - Math,42,35,83.3%,33,94.3%,31,93.9%");
        sb.AppendLine("Single Subject - English,28,24,85.7%,22,91.7%,20,90.9%");
        sb.AppendLine("Single Subject - Science,24,20,83.3%,19,95.0%,18,94.7%");
        sb.AppendLine("Education Specialist,52,46,88.5%,30,65.2%,35,76.1%");
        sb.AppendLine();

        // Program Impact
        sb.AppendLine("PROGRAM IMPACT");
        sb.AppendLine("Metric,Value");
        sb.AppendLine("Total Candidates Funded,214");
        sb.AppendLine("Credentials Earned,162");
        sb.AppendLine("Hired in Sponsoring District,142");
        sb.AppendLine("Total Investment,$2140000");
        sb.AppendLine("Cost per Credential,$13210");
    }

    /// <summary>
    /// Report export metadata helper class
    /// </summary>
    private class ReportExportData
    {
        public string Title { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string ViewName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public object? Model { get; set; }
    }
}
