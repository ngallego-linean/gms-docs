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

            // Get reporting metrics
            var reportingDeadline = grantCycle.EndDate.AddMonths(3); // Example: 3 months after cycle end
            var (totalFunded, reportsSubmitted, reportsPending, reportsOverdue) =
                _grantService.GetReportingMetrics(leaId, grantCycleId, reportingDeadline);

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
                ActionItems = actionItems,
                // Reporting metrics
                TotalFundedStudents = totalFunded,
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
                    ReportStatus = r.ReportStatus,
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
                    ReportStatus = report?.ReportStatus ?? "PENDING",
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
                ReportStatus = existingReport?.ReportStatus ?? "DRAFT",
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
                .Where(r => r.ReportStatus == "SUBMITTED" || r.ReportStatus == "APPROVED" || r.ReportStatus == "REVISION_REQUESTED")
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
                    ReportStatus = x.Report.ReportStatus,
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
