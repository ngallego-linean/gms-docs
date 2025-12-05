using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;
using GMS.DomainModel;

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
    public IActionResult Dashboard(int leaId = 5, int grantCycleId = 1)
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

            var allStudents = leaApplications.SelectMany(a => a.Students).ToList();
            var totalStudents = allStudents.Count;

            // Count students by mapped LEA statuses (UNDER_REVIEW, APPROVED, PAID)
            var underReviewCount = allStudents.Count(s => MapToLEAStatus(s.Status) == "UNDER_REVIEW");
            var approvedCount = allStudents.Count(s => MapToLEAStatus(s.Status) == "APPROVED");
            var paidCount = allStudents.Count(s => MapToLEAStatus(s.Status) == "PAID");

            // Calculate batch context (month/day information)
            var currentDate = DateTime.Now;
            var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            var daysRemaining = daysInMonth - currentDate.Day;
            var currentBatchMonth = currentDate.ToString("MMMM yyyy");

            // Calculate action item counts (candidates pending LEA review)
            var candidatesReadyForApplication = allStudents.Count(s => s.Status == "PENDING_LEA" || s.Status == "DRAFT");
            var applicationsWithErrors = leaApplications.Count(a =>
                a.Status == "VALIDATION_ERROR" || a.Status == "REJECTED");

            // Get draft applications
            var draftApplications = leaApplications
                .Where(a => a.Status == "DRAFT")
                .Select(a => new DraftApplicationSummaryViewModel
                {
                    ApplicationId = a.Id,
                    IHEName = a.IHE.Name,
                    CandidateCount = a.Students.Count,
                    LastSaved = a.LastModified,
                    DaysRemainingInMonth = daysRemaining
                })
                .ToList();

            // Group IHE submissions by partner
            var iheSubmissions = leaApplications
                .GroupBy(a => a.IHE.Name)
                .Select(g =>
                {
                    var app = g.First();
                    var allStudents = g.SelectMany(a => a.Students).ToList();
                    var hasErrors = g.Any(a => a.Status == "VALIDATION_ERROR" || a.Status == "REJECTED");

                    return new IHESubmissionViewModel
                    {
                        ApplicationId = app.Id,
                        IHEName = app.IHE.Name,
                        SubmissionDate = app.CreatedAt,
                        TotalCandidates = allStudents.Count,
                        Status = app.Status,
                        LastModified = app.LastModified,
                        HasErrors = hasErrors,
                        ValidationErrors = hasErrors ? new List<string> { "Application contains validation errors" } : new List<string>(),
                        Candidates = allStudents.Select(s => new CandidateSummaryViewModel
                        {
                            StudentId = s.Id,
                            FirstName = s.FirstName,
                            LastName = s.LastName,
                            CredentialArea = s.CredentialArea,
                            AwardAmount = s.AwardAmount,
                            Status = s.Status,
                            NeedsCompletion = s.Status == "DRAFT" || s.Status == "PENDING_LEA",
                            SEID = s.SEID
                        }).ToList()
                    };
                })
                .OrderBy(s => s.SubmissionDate)
                .ToList();

            var actionItems = new List<ActionItemViewModel>();

            // Priority order: Errors, Candidates Awaiting Review, Reports Due
            if (applicationsWithErrors > 0)
            {
                actionItems.Add(new ActionItemViewModel
                {
                    Id = 1,
                    Type = "application_errors",
                    Title = "Application Errors",
                    Description = $"You have {applicationsWithErrors} application(s) that require attention",
                    DueDate = DateTime.Now.AddDays(7),
                    Priority = "critical",
                    AssignedTo = "LEA"
                });
            }

            // Create batch submissions grouped by month (LEA batches to CTC)
            var batchSubmissions = leaApplications
                .GroupBy(a => new { Year = a.CreatedAt.Year, Month = a.CreatedAt.Month })
                .Select(g =>
                {
                    var allStudents = g.SelectMany(a => a.Students).ToList();
                    var ihePartners = g.Select(a => a.IHE.Name).Distinct().ToList();
                    var batchDate = new DateTime(g.Key.Year, g.Key.Month, 1);
                    var lastModified = g.Max(a => a.LastModified);

                    // Determine batch status based on application statuses
                    var allSubmitted = g.All(a => a.Status == "DISBURSEMENT" || a.Status == "REPORTING" || a.Status == "COMPLETE");
                    var anyDraft = g.Any(a => a.Status == "DRAFT" || a.Status == "COLLECTING_STUDENTS");
                    var batchStatus = anyDraft ? "DRAFT" : allSubmitted ? "SUBMITTED" : "IN_PROGRESS";

                    return new LEABatchSubmissionViewModel
                    {
                        BatchMonth = batchDate.ToString("MMMM yyyy"),
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        SubmissionDate = allSubmitted ? lastModified : null,
                        TotalCandidates = allStudents.Count,
                        IHEPartnerCount = ihePartners.Count,
                        IHEPartnerNames = ihePartners,
                        Status = batchStatus,
                        LastModified = lastModified,
                        ApplicationIds = g.Select(a => a.Id).ToList()
                    };
                })
                .OrderByDescending(b => b.LastModified)
                .ToList();

            // Get reporting metrics
            var reportingDeadline = grantCycle.EndDate.AddMonths(3); // Example: 3 months after cycle end
            var (totalFunded, reportsSubmitted, reportsPending, reportsOverdue) =
                _grantService.GetReportingMetrics(leaId, grantCycleId, reportingDeadline);

            // Candidates awaiting review (mock count for demonstration)
            var candidatesAwaitingReview = candidatesReadyForApplication > 0 ? candidatesReadyForApplication : 5;

            if (candidatesAwaitingReview > 0)
            {
                actionItems.Add(new ActionItemViewModel
                {
                    Id = 3,
                    Type = "candidates_review",
                    Title = "Candidates Awaiting Review",
                    Description = $"You have {candidatesAwaitingReview} candidate(s) awaiting your review",
                    DueDate = DateTime.Now.AddDays(7),
                    Priority = "medium",
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
                PendingCompletionCount = underReviewCount,
                SubmittedCount = 0, // Not used in dashboard
                ApprovedCount = approvedCount,

                // Enhanced Action Items
                CandidatesReadyForApplication = candidatesReadyForApplication,
                ApplicationsWithErrors = applicationsWithErrors,
                CurrentMonthDay = currentDate.Day,
                DaysRemainingInMonth = daysRemaining,
                CurrentBatchMonth = currentBatchMonth,

                // Draft Applications
                DraftApplicationCount = draftApplications.Count,
                DraftApplications = draftApplications,

                // IHE Submissions
                IHESubmissions = iheSubmissions,

                // LEA Batch Submissions to CTC
                BatchSubmissions = batchSubmissions,

                Applications = leaApplications.Select(a => new ApplicationSummaryViewModel
                {
                    Id = a.Id,
                    IHEName = a.IHE.Name,
                    LEAName = a.LEA.Name,
                    TotalStudents = a.Students.Count,
                    ApprovedCount = a.Students.Count(s => s.Status == "APPROVED"),
                    PendingCount = a.Students.Count(s => s.Status == "SUBMITTED" || s.Status == "PENDING_LEA"),
                    Status = a.Status,
                    SubmissionDate = a.CreatedAt,
                    LastModified = a.LastModified
                }).ToList(),
                ActionItems = actionItems,

                // Reporting metrics (TotalFundedStudents = paid count for LEA status cards)
                TotalFundedStudents = paidCount,
                ReportsSubmitted = reportsSubmitted,
                ReportsPending = reportsPending,
                ReportsOverdue = reportsOverdue,
                ReportingDeadline = reportingDeadline
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
    public IActionResult Applications(int leaId = 5, int grantCycleId = 1)
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
                    SubmissionDate = a.CreatedAt,
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

    // Maps detailed workflow statuses to simplified LEA view statuses
    private string MapToLEAStatus(string detailedStatus)
    {
        return detailedStatus switch
        {
            // Under Review - still being processed
            "DRAFT" or "PENDING_LEA" or "SUBMITTED" or "LEA_REVIEW" or
            "CTC_SUBMITTED" or "CTC_REVIEWING" => "UNDER_REVIEW",

            // Approved - CTC has approved, in GAA/disbursement prep stages
            "CTC_APPROVED" or "APPROVED" or "GAA_PENDING" or "GAA_GENERATED" or "GAA_SIGNED" => "APPROVED",

            // Paid - payment has been processed
            "INVOICE_GENERATED" or "PAYMENT_AUTHORIZED" or "WARRANT_ISSUED" or
            "PAYMENT_COMPLETE" or "REPORTING_PENDING" or "REPORTING_PARTIAL" or
            "REPORTING_COMPLETE" or "REPORTS_APPROVED" or "COMPLETE" => "PAID",

            // Default fallback
            _ => "UNDER_REVIEW"
        };
    }

    [Route("AllCandidates")]
    public IActionResult AllCandidates(int leaId = 5, int grantCycleId = 1, string? status = null)
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

            // Get ALL students across ALL months with mapped statuses
            var allCandidates = leaApplications
                .SelectMany(a => a.Students.Select(s => new LEACandidateViewModel
                {
                    StudentId = s.Id,
                    FullName = $"{s.FirstName} {s.LastName}",
                    SEID = s.SEID,
                    IHEName = a.IHE.Name,
                    CredentialArea = s.CredentialArea,
                    AwardAmount = s.AwardAmount,
                    SubmissionMonth = a.CreatedAt.ToString("MMMM yyyy"),
                    SubmissionDate = a.CreatedAt,
                    Status = MapToLEAStatus(s.Status)
                }))
                .OrderByDescending(c => c.SubmissionDate)
                .ThenBy(c => c.FullName)
                .ToList();

            // Get distinct filter options
            var ihePartners = allCandidates.Select(c => c.IHEName).Distinct().OrderBy(n => n).ToList();
            var credentialAreas = allCandidates.Select(c => c.CredentialArea).Distinct().OrderBy(n => n).ToList();
            var submissionMonths = allCandidates.Select(c => c.SubmissionMonth).Distinct().ToList();

            var model = new LEAAllCandidatesViewModel
            {
                LEAId = leaId,
                LEAName = leaApplications.FirstOrDefault()?.LEA.Name ?? "District",
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                Candidates = allCandidates,
                IHEPartners = ihePartners,
                CredentialAreas = credentialAreas,
                SubmissionMonths = submissionMonths
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading all candidates");
            return View("Error");
        }
    }

    [Route("ReviewCandidates")]
    public IActionResult ReviewCandidates(int leaId = 5, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);
            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            // Get all applications for this LEA
            var allApplications = _grantService.GetApplications();
            var leaApplications = allApplications
                .Where(a => a.LEA.Id == leaId && a.GrantCycleId == grantCycleId)
                .ToList();

            // Get candidates awaiting review (status = PENDING_LEA or similar)
            var candidatesForReview = leaApplications
                .SelectMany(a => a.Students
                    .Where(s => s.Status == "PENDING_LEA" || s.Status == "DRAFT")
                    .Select(s => new CandidateForReviewViewModel
                    {
                        StudentId = s.Id,
                        ApplicationId = a.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        SEID = s.SEID,
                        CredentialArea = s.CredentialArea,
                        AwardAmount = s.AwardAmount,
                        IHEName = a.IHE.Name,
                        SubmittedDate = s.SubmittedAt ?? s.CreatedAt,
                        Status = s.Status,
                        IsSelected = true
                    }))
                .OrderBy(c => c.IHEName)
                .ThenBy(c => c.LastName)
                .ToList();

            // Get existing draft applications
            var draftApplications = leaApplications
                .Where(a => a.Status == "DRAFT")
                .Select(a => new DraftApplicationOptionViewModel
                {
                    ApplicationId = a.Id,
                    IHEName = a.IHE.Name,
                    CandidateCount = a.Students.Count,
                    LastModified = a.LastModified
                })
                .ToList();

            // Batch context
            var currentDate = DateTime.Now;
            var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            var daysRemaining = daysInMonth - currentDate.Day;

            var model = new ReviewCandidatesViewModel
            {
                LEAId = leaId,
                LEAName = leaApplications.FirstOrDefault()?.LEA.Name ?? "District",
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                CurrentBatchMonth = currentDate.ToString("MMMM yyyy"),
                CurrentMonthDay = currentDate.Day,
                DaysInMonth = daysInMonth,
                DaysRemainingInMonth = daysRemaining,
                Candidates = candidatesForReview,
                TotalCandidates = candidatesForReview.Count,
                IHESourceCount = candidatesForReview.Select(c => c.IHEName).Distinct().Count(),
                DraftApplications = draftApplications,

                // Application Information (dummy data - displayed locked by default)
                POCFirstName = "Jane",
                POCLastName = "Doe",
                POCEmail = "jane.doe@lausd.org",
                POCPhone = "(555) 123-4567",

                GAASignerName = "Dr. Robert Johnson",
                GAASignerTitle = "Superintendent",
                GAASignerEmail = "superintendent@lausd.org",

                FiscalAgentName = "Maria Garcia",
                FiscalAgentEmail = "fiscal@lausd.org",
                FiscalAgentPhone = "(555) 987-6543",

                SuperintendentName = "Dr. Robert Johnson",
                SuperintendentEmail = "superintendent@lausd.org",

                PaymentIntent = "Stipend to Student Teacher",
                PaymentSchedule = "Lump Sum"
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading candidate review page");
            return View("Error");
        }
    }

    [HttpPost]
    [Route("ReviewCandidates")]
    public IActionResult ProcessCandidates(ProcessCandidatesViewModel model)
    {
        try
        {
            // In a real application, this would process the selected candidates
            // For now, just redirect back to dashboard with success message

            TempData["Success"] = $"Successfully processed {model.SelectedStudentIds.Count} candidate(s)";

            return RedirectToAction("Dashboard", new { leaId = model.LEAId, grantCycleId = model.GrantCycleId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing candidates");
            TempData["Error"] = $"Error processing candidates: {ex.Message}";
            return RedirectToAction("ReviewCandidates", new { leaId = model.LEAId, grantCycleId = model.GrantCycleId });
        }
    }

    [Route("BatchDetails")]
    public IActionResult BatchDetails(int leaId = 5, int grantCycleId = 1, int year = 2025, int month = 8)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);
            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            // Get all applications for this LEA in the specified month
            var allApplications = _grantService.GetApplications();
            var batchApplications = allApplications
                .Where(a => a.LEA.Id == leaId &&
                           a.GrantCycleId == grantCycleId &&
                           a.CreatedAt.Year == year &&
                           a.CreatedAt.Month == month)
                .ToList();

            if (!batchApplications.Any())
            {
                return NotFound($"No applications found for {new DateTime(year, month, 1):MMMM yyyy}");
            }

            // Aggregate all students from all applications
            var allStudents = batchApplications.SelectMany(a => a.Students).ToList();

            // Get unique IHE partners
            var ihePartners = batchApplications.Select(a => a.IHE.Name).Distinct().ToList();

            // Calculate total award amount
            var totalAwardAmount = allStudents.Sum(s => s.AwardAmount);

            // Determine batch status
            var allSubmitted = batchApplications.All(a => a.Status == "DISBURSEMENT" || a.Status == "REPORTING" || a.Status == "COMPLETE");
            var anyDraft = batchApplications.Any(a => a.Status == "DRAFT" || a.Status == "COLLECTING_STUDENTS");
            var batchStatus = anyDraft ? "DRAFT" : allSubmitted ? "SUBMITTED" : "IN_PROGRESS";

            // Get dates
            var createdDate = batchApplications.Min(a => a.CreatedAt);
            var lastModified = batchApplications.Max(a => a.LastModified);
            var submissionDate = allSubmitted ? lastModified : (DateTime?)null;

            // Build timeline events based on batch workflow progression
            var timelineEvents = new List<BatchTimelineEvent>();

            // Track which workflow stages the batch has reached
            var statusValues = batchApplications.Select(a => a.Status).Distinct().ToList();
            var hasReachedLEAReview = statusValues.Any(s => s == "LEA_REVIEW" || s == "CTC_REVIEW" || s == "DISBURSEMENT" || s == "REPORTING" || s == "COMPLETE");
            var hasReachedCTCReview = statusValues.Any(s => s == "CTC_REVIEW" || s == "DISBURSEMENT" || s == "REPORTING" || s == "COMPLETE");
            var hasReachedDisbursement = statusValues.Any(s => s == "DISBURSEMENT" || s == "REPORTING" || s == "COMPLETE");
            var hasReachedReporting = statusValues.Any(s => s == "REPORTING" || s == "COMPLETE");
            var hasReachedComplete = statusValues.Any(s => s == "COMPLETE");

            // 1. CREATED event - when batch was first created
            timelineEvents.Add(new BatchTimelineEvent
            {
                EventDate = createdDate,
                EventType = "BATCH_CREATED",
                Description = $"Submission with {allStudents.Count} Candidate(s) from {ihePartners.Count} IHE partner(s)",
                Actor = batchApplications.OrderBy(a => a.CreatedAt).First().CreatedBy
            });

            // 2. LEA_REVIEW event - when batch entered LEA review
            if (hasReachedLEAReview)
            {
                var leaReviewApps = batchApplications.Where(a => a.Status == "LEA_REVIEW" || a.Status == "CTC_REVIEW" || a.Status == "DISBURSEMENT" || a.Status == "REPORTING" || a.Status == "COMPLETE");
                var leaReviewDate = leaReviewApps.Min(a => a.LastActionDate ?? a.LastModified);
                timelineEvents.Add(new BatchTimelineEvent
                {
                    EventDate = leaReviewDate,
                    EventType = "LEA_REVIEW",
                    Description = "Submission sent to LEA for review",
                    Actor = leaReviewApps.OrderBy(a => a.LastActionDate ?? a.LastModified).FirstOrDefault()?.LastActionBy
                });
            }

            // 3. CTC_SUBMITTED event - when batch was submitted to CTC
            if (hasReachedCTCReview)
            {
                var ctcReviewApps = batchApplications.Where(a => a.Status == "CTC_REVIEW" || a.Status == "DISBURSEMENT" || a.Status == "REPORTING" || a.Status == "COMPLETE");
                var ctcSubmitDate = ctcReviewApps.Min(a => a.LastActionDate ?? a.LastModified);
                timelineEvents.Add(new BatchTimelineEvent
                {
                    EventDate = ctcSubmitDate,
                    EventType = "CTC_SUBMITTED",
                    Description = $"Submission sent to CTC with {allStudents.Count} Candidate(s)",
                    Actor = ctcReviewApps.OrderBy(a => a.LastActionDate ?? a.LastModified).FirstOrDefault()?.LastActionBy
                });
            }

            // 4. DISBURSEMENT event - when batch moved to disbursement
            if (hasReachedDisbursement)
            {
                var disbursementApps = batchApplications.Where(a => a.Status == "DISBURSEMENT" || a.Status == "REPORTING" || a.Status == "COMPLETE");
                var disbursementDate = disbursementApps.Min(a => a.LastActionDate ?? a.LastModified);
                timelineEvents.Add(new BatchTimelineEvent
                {
                    EventDate = disbursementDate,
                    EventType = "CTC_APPROVED",
                    Description = "Batch approved by CTC and moved to disbursement",
                    Actor = disbursementApps.OrderBy(a => a.LastActionDate ?? a.LastModified).FirstOrDefault()?.LastActionBy
                });
            }

            // 5. REPORTING event - when batch entered reporting phase
            if (hasReachedReporting)
            {
                var reportingApps = batchApplications.Where(a => a.Status == "REPORTING" || a.Status == "COMPLETE");
                var reportingDate = reportingApps.Min(a => a.LastActionDate ?? a.LastModified);
                timelineEvents.Add(new BatchTimelineEvent
                {
                    EventDate = reportingDate,
                    EventType = "REPORTING",
                    Description = "Batch entered reporting phase",
                    Actor = reportingApps.OrderBy(a => a.LastActionDate ?? a.LastModified).FirstOrDefault()?.LastActionBy
                });
            }

            // 6. COMPLETE event - when batch was completed
            if (hasReachedComplete)
            {
                var completeApps = batchApplications.Where(a => a.Status == "COMPLETE");
                var completeDate = completeApps.Min(a => a.LastActionDate ?? a.LastModified);
                timelineEvents.Add(new BatchTimelineEvent
                {
                    EventDate = completeDate,
                    EventType = "COMPLETE",
                    Description = "Batch processing complete",
                    Actor = completeApps.OrderBy(a => a.LastActionDate ?? a.LastModified).FirstOrDefault()?.LastActionBy
                });
            }

            // Sort timeline events by date
            timelineEvents = timelineEvents.OrderBy(e => e.EventDate).ToList();

            var model = new LEABatchDetailsViewModel
            {
                LEAId = leaId,
                LEAName = batchApplications.First().LEA.Name,
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                BatchMonth = new DateTime(year, month, 1).ToString("MMMM yyyy"),
                Year = year,
                Month = month,
                TotalCandidates = allStudents.Count,
                IHEPartnerCount = ihePartners.Count,
                IHEPartnerNames = ihePartners,
                TotalAwardAmount = totalAwardAmount,
                Status = batchStatus,
                SubmissionDate = submissionDate,
                CreatedDate = createdDate,
                LastModified = lastModified,
                Students = allStudents.Select(s => new StudentViewModel
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
                    SubmittedAt = new DateTime(year, month, 1) // All students in batch have same submission month
                }).ToList(),
                ApplicationIds = batchApplications.Select(a => a.Id).ToList(),
                ApplicationCount = batchApplications.Count,
                TimelineEvents = timelineEvents
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading batch details");
            return View("Error");
        }
    }

    // ==================== REPORTING ACTIONS ====================

    [Route("Reports")]
    public IActionResult Reports(int leaId = 5, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);
            if (grantCycle == null)
                return NotFound("Grant cycle not found");

            var reportingDeadline = grantCycle.EndDate.AddMonths(3);
            var (totalFunded, reportsSubmitted, reportsPending, reportsOverdue) =
                _grantService.GetReportingMetrics(leaId, grantCycleId, reportingDeadline);

            var fundedStudents = _grantService.GetFundedStudents(leaId, grantCycleId);
            var reports = _grantService.GetLEAReports(leaId, grantCycleId);
            var applications = _grantService.GetApplications();

            var recentReports = reports
                .OrderByDescending(r => r.SubmittedDate)
                .Take(10)
                .Join(fundedStudents, r => r.StudentId, s => s.Id, (r, s) => new StudentReportSummaryViewModel
                {
                    StudentId = s.Id,
                    ApplicationId = s.ApplicationId,
                    StudentName = $"{s.FirstName} {s.LastName}",
                    SEID = s.SEID,
                    CredentialArea = s.CredentialArea,
                    IHEName = applications.FirstOrDefault(a => a.Id == s.ApplicationId)?.IHE.Name ?? "Unknown",
                    AwardAmount = s.AwardAmount,
                    ApprovedDate = s.ApprovedAt,
                    HasReport = true,
                    ReportStatus = r.Status,
                    ReportSubmittedDate = r.SubmittedDate
                })
                .ToList();

            var model = new LEAReportsViewModel
            {
                LEAId = leaId,
                LEAName = applications.FirstOrDefault(a => a.LEA.Id == leaId)?.LEA.Name ?? "District",
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                TotalFundedStudents = totalFunded,
                ReportsSubmitted = reportsSubmitted,
                ReportsPending = reportsPending,
                ReportsOverdue = reportsOverdue,
                ReportingDeadline = reportingDeadline,
                IsReportingPeriodOpen = reportingDeadline > DateTime.UtcNow,
                RecentReports = recentReports
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reports page");
            return View("Error");
        }
    }

    [Route("FundedCandidates")]
    public IActionResult FundedCandidates(int leaId = 5, int grantCycleId = 1, string? ihePartner = null,
        string? credentialType = null, string? cohort = null, string? reportStatus = null)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);
            if (grantCycle == null)
                return NotFound("Grant cycle not found");

            var fundedStudents = _grantService.GetFundedStudents(leaId, grantCycleId);
            var reports = _grantService.GetLEAReports(leaId, grantCycleId);
            var applications = _grantService.GetApplications();

            var students = fundedStudents.Select(s =>
            {
                var report = reports.FirstOrDefault(r => r.StudentId == s.Id);
                var app = applications.FirstOrDefault(a => a.Id == s.ApplicationId);

                return new StudentReportSummaryViewModel
                {
                    StudentId = s.Id,
                    ApplicationId = s.ApplicationId,
                    StudentName = $"{s.FirstName} {s.LastName}",
                    SEID = s.SEID,
                    CredentialArea = s.CredentialArea,
                    IHEName = app?.IHE.Name ?? "Unknown",
                    AwardAmount = s.AwardAmount,
                    ApprovedDate = s.ApprovedAt,
                    Cohort = s.ApprovedAt?.ToString("MMMM yyyy") ?? "Unknown",
                    HasReport = report != null,
                    ReportStatus = report?.Status ?? "PENDING",
                    ReportSubmittedDate = report?.SubmittedDate
                };
            }).ToList();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(ihePartner))
                students = students.Where(s => s.IHEName == ihePartner).ToList();

            if (!string.IsNullOrWhiteSpace(credentialType))
                students = students.Where(s => s.CredentialArea == credentialType).ToList();

            if (!string.IsNullOrWhiteSpace(cohort))
                students = students.Where(s => s.Cohort == cohort).ToList();

            if (!string.IsNullOrWhiteSpace(reportStatus))
                students = students.Where(s => s.ReportStatus == reportStatus).ToList();

            var model = new FundedCandidatesViewModel
            {
                LEAId = leaId,
                LEAName = applications.FirstOrDefault(a => a.LEA.Id == leaId)?.LEA.Name ?? "District",
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                Students = students,
                TotalCount = fundedStudents.Count,
                IHEPartners = applications.Where(a => a.LEA.Id == leaId && a.GrantCycleId == grantCycleId).Select(a => a.IHE.Name).Distinct().OrderBy(x => x).ToList(),
                CredentialTypes = fundedStudents.Select(s => s.CredentialArea).Distinct().OrderBy(x => x).ToList(),
                Cohorts = _grantService.GetCohorts(leaId, grantCycleId).Select(c => c.Cohort).ToList(),
                SearchCriteria = new ReportSearchCriteria
                {
                    IHEPartner = ihePartner,
                    CredentialType = credentialType,
                    Cohort = cohort,
                    ReportStatus = reportStatus
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading funded candidates");
            return View("Error");
        }
    }

    [Route("SubmitReport/{studentId}")]
    public IActionResult SubmitReport(int studentId)
    {
        try
        {
            var student = _grantService.GetStudent(studentId);
            if (student == null)
                return NotFound("Student not found");

            var application = _grantService.GetApplication(student.ApplicationId);
            if (application == null)
                return NotFound("Application not found");

            var existingReport = _grantService.GetLEAReport(studentId);

            var model = new LEAReportSubmissionViewModel
            {
                StudentId = studentId,
                ApplicationId = student.ApplicationId,
                LEAId = application.LEA.Id,
                GrantCycleId = application.GrantCycleId,
                ReportId = existingReport?.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                SEID = student.SEID,
                CredentialArea = student.CredentialArea,
                AwardAmount = student.AwardAmount,
                IHEName = application.IHE.Name,
                ApprovedDate = student.ApprovedAt,
                ReportStatus = existingReport?.Status ?? "DRAFT",
                IsLocked = existingReport?.IsLocked ?? false,
                CTCFeedback = existingReport?.CTCFeedback
            };

            // Populate from existing report if available
            if (existingReport != null)
            {
                model.PaymentCategory = existingReport.PaymentCategory;
                model.PaymentSchedule = existingReport.PaymentSchedule;
                model.ActualPaymentAmount = existingReport.ActualPaymentAmount;
                model.FirstPaymentDate = existingReport.FirstPaymentDate;
                model.FinalPaymentDate = existingReport.FinalPaymentDate;
                model.ProgramCompletionStatus = existingReport.ProgramCompletionStatus;
                model.ProgramCompletionDate = existingReport.ProgramCompletionDate;
                model.CredentialEarnedStatus = existingReport.CredentialEarnedStatus;
                model.CredentialIssueDate = existingReport.CredentialIssueDate;
                model.HiredInDistrict = existingReport.HiredInDistrict;
                model.EmploymentStatus = existingReport.EmploymentStatus;
                model.EmploymentStartDate = existingReport.EmploymentStartDate;
                model.EmployingLEA = existingReport.EmployingLEA;
                model.SchoolSite = existingReport.SchoolSite;
                model.GradeLevel = existingReport.GradeLevel;
                model.SubjectArea = existingReport.SubjectArea;
                model.JobTitle = existingReport.JobTitle;
                model.PlacementQualityRating = existingReport.PlacementQualityRating;
                model.PlacementQualityNotes = existingReport.PlacementQualityNotes;
                model.MentorTeacherName = existingReport.MentorTeacherName;
                model.MentorTeacherFeedback = existingReport.MentorTeacherFeedback;
                model.AdditionalNotes = existingReport.AdditionalNotes;
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading submit report form");
            return View("Error");
        }
    }

    [HttpPost]
    [Route("SubmitReport")]
    public IActionResult SubmitReport(LEAReportSubmissionViewModel model, string action = "save")
    {
        try
        {
            if (!ModelState.IsValid && action == "submit")
            {
                TempData["Error"] = "Please fill in all required fields before submitting.";
                return View(model);
            }

            var report = new LEAReport
            {
                Id = model.ReportId ?? 0,
                StudentId = model.StudentId,
                ApplicationId = model.ApplicationId,
                PaymentCategory = model.PaymentCategory,
                PaymentSchedule = model.PaymentSchedule,
                ActualPaymentAmount = model.ActualPaymentAmount,
                FirstPaymentDate = model.FirstPaymentDate,
                FinalPaymentDate = model.FinalPaymentDate,
                ProgramCompletionStatus = model.ProgramCompletionStatus,
                ProgramCompletionDate = model.ProgramCompletionDate,
                CredentialEarnedStatus = model.CredentialEarnedStatus,
                CredentialIssueDate = model.CredentialIssueDate,
                HiredInDistrict = model.HiredInDistrict,
                EmploymentStatus = model.EmploymentStatus,
                EmploymentStartDate = model.EmploymentStartDate,
                EmployingLEA = model.EmployingLEA,
                SchoolSite = model.SchoolSite,
                GradeLevel = model.GradeLevel,
                SubjectArea = model.SubjectArea,
                JobTitle = model.JobTitle,
                PlacementQualityRating = model.PlacementQualityRating,
                PlacementQualityNotes = model.PlacementQualityNotes,
                MentorTeacherName = model.MentorTeacherName,
                MentorTeacherFeedback = model.MentorTeacherFeedback,
                AdditionalNotes = model.AdditionalNotes,
                LastModifiedBy = "LEA User"  // In real app, get from auth context
            };

            var savedReport = _grantService.CreateOrUpdateLEAReport(report);

            if (action == "submit")
            {
                _grantService.SubmitLEAReport(savedReport.Id, "LEA User", "lea@example.com");
                _grantService.LockLEAReport(savedReport.Id, "LEA User");
                TempData["Success"] = "Report submitted successfully and locked for review.";
                return RedirectToAction("FundedCandidates", new { leaId = model.LEAId, grantCycleId = model.GrantCycleId });
            }
            else
            {
                TempData["Success"] = "Report saved as draft.";
                return RedirectToAction("SubmitReport", new { studentId = model.StudentId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting report");
            TempData["Error"] = $"Error: {ex.Message}";
            return View(model);
        }
    }

    [Route("BulkUpload")]
    public IActionResult BulkUpload(int leaId = 5, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);
            if (grantCycle == null)
                return NotFound("Grant cycle not found");

            var model = new LEABulkUploadViewModel
            {
                LEAId = leaId,
                LEAName = "Sample LEA",  // Would get from auth context
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bulk upload page");
            return View("Error");
        }
    }

    [HttpPost]
    [Route("BulkUpload")]
    public IActionResult BulkUpload(LEABulkUploadViewModel model)
    {
        try
        {
            if (model.UploadFile == null || model.UploadFile.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return View(model);
            }

            // This is a placeholder - in production would parse CSV/Excel
            // For now, just show success message
            TempData["Success"] = "File uploaded successfully. Processing bulk upload...";
            model.ProcessingComplete = true;
            model.SuccessCount = 0;
            model.ErrorCount = 0;

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing bulk upload");
            TempData["Error"] = $"Error processing file: {ex.Message}";
            return View(model);
        }
    }

    [Route("ReportHistory")]
    public IActionResult ReportHistory(int leaId = 5, int grantCycleId = 1, string? status = null,
        string? ihePartner = null, int? year = null)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);
            if (grantCycle == null)
                return NotFound("Grant cycle not found");

            var fundedStudents = _grantService.GetFundedStudents(leaId, grantCycleId);
            var allReports = _grantService.GetLEAReports(leaId, grantCycleId);
            var applications = _grantService.GetApplications();

            // Filter to only submitted/approved reports
            var submittedReports = allReports
                .Where(r => r.Status == "SUBMITTED" || r.Status == "APPROVED" || r.Status == "REVISION_REQUESTED")
                .ToList();

            var reports = submittedReports
                .Join(fundedStudents, r => r.StudentId, s => s.Id, (r, s) => new
                {
                    Report = r,
                    Student = s,
                    Application = applications.FirstOrDefault(a => a.Id == s.ApplicationId)
                })
                .Select(x => new StudentReportSummaryViewModel
                {
                    StudentId = x.Student.Id,
                    ApplicationId = x.Student.ApplicationId,
                    StudentName = $"{x.Student.FirstName} {x.Student.LastName}",
                    SEID = x.Student.SEID,
                    CredentialArea = x.Student.CredentialArea,
                    IHEName = x.Application?.IHE.Name ?? "Unknown",
                    AwardAmount = x.Student.AwardAmount,
                    ApprovedDate = x.Student.ApprovedAt,
                    HasReport = true,
                    ReportStatus = x.Report.Status,
                    ReportSubmittedDate = x.Report.SubmittedDate
                })
                .ToList();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(status))
                reports = reports.Where(r => r.ReportStatus == status).ToList();

            if (!string.IsNullOrWhiteSpace(ihePartner))
                reports = reports.Where(r => r.IHEName == ihePartner).ToList();

            if (year.HasValue)
                reports = reports.Where(r => r.ReportSubmittedDate?.Year == year.Value).ToList();

            var model = new ReportHistoryViewModel
            {
                LEAId = leaId,
                LEAName = "Sample LEA",
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                Reports = reports.OrderByDescending(r => r.ReportSubmittedDate).ToList(),
                TotalCount = reports.Count,
                IHEPartners = submittedReports
                    .Join(fundedStudents, r => r.StudentId, s => s.Id, (r, s) => applications.FirstOrDefault(a => a.Id == s.ApplicationId)?.IHE.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()!,
                SubmissionYears = submittedReports
                    .Where(r => r.SubmittedDate.HasValue)
                    .Select(r => r.SubmittedDate!.Value.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList(),
                SearchCriteria = new ReportHistorySearchCriteria
                {
                    Status = status,
                    IHEPartner = ihePartner,
                    SubmissionYear = year
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading report history");
            return View("Error");
        }
    }
}
