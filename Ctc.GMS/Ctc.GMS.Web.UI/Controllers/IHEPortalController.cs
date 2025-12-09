using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;
using GMS.Business.Helpers;
using GMS.Data.Repositories;
using GMS.DomainModel;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("IHE")]
public class IHEPortalController : Controller
{
    private readonly IGrantService _grantService;
    private readonly MockRepository _repository;
    private readonly ILogger<IHEPortalController> _logger;
    private readonly IIHETemplateService _templateService;

    public IHEPortalController(IGrantService grantService, MockRepository repository, ILogger<IHEPortalController> logger, IIHETemplateService templateService)
    {
        _grantService = grantService;
        _repository = repository;
        _logger = logger;
        _templateService = templateService;
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

            // Get reporting metrics
            var allStudents = iheApplications.SelectMany(a => a.Students).ToList();
            var fundedStudents = allStudents.Where(s => StatusHelper.IsDisbursedStatus(s.Status)).ToList();
            var activeReportingPeriod = _repository.GetReportingPeriods(grantCycleId)
                .FirstOrDefault(rp => rp.IsActive);

            var reportsNotStarted = fundedStudents.Count(s => s.Status == "PAYMENT_COMPLETE");
            var reportsInProgress = fundedStudents.Count(s => s.Status == "REPORTING_PENDING" || s.Status == "REPORTING_PARTIAL");
            var reportsSubmitted = fundedStudents.Count(s => s.Status == "REPORTING_COMPLETE" || s.Status == "REPORTS_APPROVED");

            var reportingAlerts = new List<ReportingAlertViewModel>();
            if (activeReportingPeriod != null && reportsNotStarted > 0)
            {
                var daysUntilDue = (activeReportingPeriod.DueDate - DateTime.Now).Days;
                if (daysUntilDue <= 7)
                {
                    reportingAlerts.Add(new ReportingAlertViewModel
                    {
                        AlertType = "WARNING",
                        Message = $"Reports due in {daysUntilDue} days! You have {reportsNotStarted} outstanding report(s).",
                        ActionUrl = Url.Action("FundedCandidates", "IHEPortal", new { iheId, grantCycleId }) ?? "",
                        ActionText = "Complete Reports Now"
                    });
                }
            }

            // Add action item for reports if there are funded candidates
            if (fundedStudents.Count() > 0 && (reportsNotStarted > 0 || reportsInProgress > 0))
            {
                actionItems.Add(new ActionItemViewModel
                {
                    Id = 2,
                    Type = "reports_required",
                    Title = "Reports Required",
                    Description = $"{reportsNotStarted + reportsInProgress} report(s) need to be completed",
                    DueDate = activeReportingPeriod?.DueDate,
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

                // Candidate Status Metrics
                TotalCandidates = totalStudents,
                DraftCount = draftStudents,
                UnderReviewCount = submittedStudents,
                ApprovedCount = approvedStudents,

                // Submissions by LEA Partner
                Submissions = iheApplications.Select(a => new ApplicationSummaryViewModel
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

                // Action Items
                ActionItems = actionItems,

                // Post-Payment Reporting Metrics
                FundedCandidatesCount = fundedStudents.Count(),
                ReportsDue = reportsNotStarted + reportsInProgress,
                ReportsInProgress = reportsInProgress,
                ReportsSubmitted = reportsSubmitted,
                NextReportDeadline = activeReportingPeriod?.DueDate,
                NextReportPeriodName = activeReportingPeriod?.PeriodName,
                DaysUntilDeadline = activeReportingPeriod != null ? (activeReportingPeriod.DueDate - DateTime.Now).Days : null,
                HasActiveReportingPeriod = activeReportingPeriod != null,
                ReportingAlerts = reportingAlerts
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

    [Route("BulkUploadCandidates")]
    public IActionResult BulkUploadCandidates(int iheId = 1, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var ihe = _repository.GetOrganizations().FirstOrDefault(o => o.Id == iheId && o.Type == "IHE");

            var model = new BulkUploadCandidatesViewModel
            {
                IHEId = iheId,
                IHEName = ihe?.Name ?? "Institution",
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bulk upload candidates page");
            return View("Error");
        }
    }

    [Route("DownloadCandidateTemplate")]
    [HttpGet]
    public IActionResult DownloadCandidateTemplate()
    {
        try
        {
            var fileBytes = _templateService.GenerateStudentUploadTemplate();
            var fileName = "CTC_Student_Upload_Template.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating student upload template");
            return View("Error");
        }
    }

    [Route("Grants")]
    public IActionResult Grants(int iheId = 1)
    {
        // Mock IHE names
        var iheNames = new Dictionary<int, string>
        {
            { 1, "San Diego State University" },
            { 2, "UC San Diego" },
            { 3, "Cal State San Marcos" },
            { 4, "Point Loma Nazarene University" },
            { 5, "University of San Diego" }
        };

        ViewBag.IHEId = iheId;
        ViewBag.IHEName = iheNames.ContainsKey(iheId) ? iheNames[iheId] : "Unknown Institution";

        return View();
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

    #region Reporting Actions

    [Route("FundedCandidates")]
    public IActionResult FundedCandidates(int iheId = 1, int grantCycleId = 1, CandidateFilterCriteria? filters = null)
    {
        try
        {
            var grantCycle = _repository.GetGrantCycle(grantCycleId);
            var allApplications = _repository.GetApplications()
                .Where(a => a.IHE.Id == iheId && a.GrantCycleId == grantCycleId)
                .ToList();

            var allStudents = allApplications.SelectMany(a => a.Students).ToList();
            var fundedStudents = allStudents.Where(s => StatusHelper.IsPayableStatus(s.Status)).ToList();

            // Apply filters
            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.CredentialArea))
                    fundedStudents = fundedStudents.Where(s => s.CredentialArea == filters.CredentialArea).ToList();

                if (filters.LEAId.HasValue)
                    fundedStudents = fundedStudents.Where(s => allApplications.First(a => a.Id == s.ApplicationId).LEA.Id == filters.LEAId.Value).ToList();

                if (!string.IsNullOrEmpty(filters.ReportingStatus))
                    fundedStudents = fundedStudents.Where(s => s.Status == filters.ReportingStatus).ToList();
            }

            var activeReportingPeriod = _repository.GetReportingPeriods(grantCycleId).FirstOrDefault(rp => rp.IsActive);

            var model = new FundedCandidatesViewModel
            {
                IHEId = iheId,
                IHEName = allApplications.FirstOrDefault()?.IHE.Name ?? "Institution",
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle?.Name ?? "",
                Filters = filters ?? new CandidateFilterCriteria(),
                Candidates = fundedStudents.Select(s =>
                {
                    var app = allApplications.First(a => a.Id == s.ApplicationId);
                    var report = _repository.GetIHEReportsByStudent(s.Id).FirstOrDefault();

                    return new FundedCandidateSummaryViewModel
                    {
                        StudentId = s.Id,
                        FullName = $"{s.FirstName} {s.LastName}",
                        SEID = s.SEID,
                        LEAName = app.LEA.Name,
                        CredentialArea = s.CredentialArea,
                        AwardAmount = s.AwardAmount,
                        PaymentStatus = StatusHelper.GetDisplayText(s.Status),
                        ReportingStatus = s.ApplicationStatus,
                        LastReportDate = report?.LastModified,
                        IsReportOverdue = activeReportingPeriod != null && s.Status == "PAYMENT_COMPLETE" && activeReportingPeriod.DueDate < DateTime.Now,
                        CanReport = activeReportingPeriod != null && s.Status != "REPORTS_APPROVED"
                    };
                }).ToList(),
                TotalCount = fundedStudents.Count(),
                AvailableCohorts = new List<string> { "2025-26", "2024-25" },
                AvailableLEAs = allApplications.Select(a => new OrganizationOption
                {
                    Id = a.LEA.Id,
                    Name = a.LEA.Name,
                    Code = a.LEA.Code
                }).Distinct().ToList(),
                AvailableCredentialAreas = allStudents.Select(s => s.CredentialArea).Distinct().ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading funded candidates");
            return View("Error");
        }
    }

    [Route("CandidateReport/{studentId}")]
    public IActionResult CandidateReport(int studentId)
    {
        try
        {
            var allApplications = _repository.GetApplications();
            var student = allApplications.SelectMany(a => a.Students).FirstOrDefault(s => s.Id == studentId);

            if (student == null)
                return NotFound("Student not found");

            var application = allApplications.First(a => a.Id == student.ApplicationId);
            var existingReport = _repository.GetIHEReportsByStudent(studentId).FirstOrDefault();
            var activeReportingPeriod = _repository.GetReportingPeriods(application.GrantCycleId).FirstOrDefault(rp => rp.IsActive);

            var model = new CandidateReportingViewModel
            {
                StudentId = studentId,
                Student = new StudentSummaryViewModel
                {
                    Id = student.Id,
                    FullName = $"{student.FirstName} {student.LastName}",
                    SEID = student.SEID,
                    CredentialArea = student.CredentialArea,
                    AwardAmount = student.AwardAmount,
                    SubmittedAt = student.SubmittedAt,
                    ApprovedAt = student.ApprovedAt
                },
                Application = new ApplicationSummaryViewModel
                {
                    Id = application.Id,
                    IHEName = application.IHE.Name,
                    LEAName = application.LEA.Name
                },
                CurrentPeriod = activeReportingPeriod != null ? new ReportingPeriodViewModel
                {
                    Id = activeReportingPeriod.Id,
                    PeriodName = activeReportingPeriod.PeriodName,
                    StartDate = activeReportingPeriod.StartDate,
                    DueDate = activeReportingPeriod.DueDate,
                    IsActive = activeReportingPeriod.IsActive,
                    Description = activeReportingPeriod.Description,
                    DaysUntilDue = (activeReportingPeriod.DueDate - DateTime.Now).Days
                } : null,
                ReportForm = existingReport != null ? MapToReportForm(existingReport) : new IHEReportFormViewModel
                {
                    StudentId = studentId,
                    ApplicationId = application.Id,
                    ReportingPeriodId = activeReportingPeriod?.Id
                },
                CanEdit = existingReport == null || existingReport.Status == "DRAFT",
                HasExistingReport = existingReport != null
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading candidate report");
            return View("Error");
        }
    }

    [HttpPost]
    [Route("SaveReportDraft")]
    public IActionResult SaveReportDraft(IHEReportFormViewModel reportForm)
    {
        try
        {
            _logger.LogInformation("Report draft saved for student {StudentId}", reportForm.StudentId);

            TempData["SuccessMessage"] = "Report draft saved successfully. Remember to submit when complete.";
            return RedirectToAction("CandidateReport", new { studentId = reportForm.StudentId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving report draft");
            TempData["ErrorMessage"] = "Error saving report draft";
            return RedirectToAction("CandidateReport", new { studentId = reportForm.StudentId });
        }
    }

    [HttpPost]
    [Route("SubmitReport")]
    public IActionResult SubmitReport(int studentId)
    {
        try
        {
            _logger.LogInformation("Report submitted for student {StudentId}", studentId);

            TempData["SuccessMessage"] = $"Report submitted successfully! Confirmation number: RPT-2026-{DateTime.Now.Ticks.ToString().Substring(0, 8)}";
            return RedirectToAction("FundedCandidates");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting report");
            TempData["ErrorMessage"] = "Error submitting report";
            return RedirectToAction("CandidateReport", new { studentId });
        }
    }

    [Route("ReportHistory")]
    public IActionResult ReportHistory(int iheId = 1, int grantCycleId = 1, ReportHistoryFilterCriteria? filters = null)
    {
        try
        {
            var allApplications = _repository.GetApplications()
                .Where(a => a.IHE.Id == iheId && a.GrantCycleId == grantCycleId)
                .ToList();

            var allReports = _repository.GetIHEReports()
                .Where(r => allApplications.Any(a => a.Id == r.ApplicationId))
                .Where(r => r.Status == "SUBMITTED" || r.Status == "APPROVED")
                .ToList();

            // Apply filters
            if (filters != null)
            {
                if (filters.StartDate.HasValue)
                    allReports = allReports.Where(r => r.SubmittedDate >= filters.StartDate.Value).ToList();

                if (filters.EndDate.HasValue)
                    allReports = allReports.Where(r => r.SubmittedDate <= filters.EndDate.Value).ToList();

                if (filters.ReportingPeriodId.HasValue)
                    allReports = allReports.Where(r => r.ReportingPeriodId == filters.ReportingPeriodId.Value).ToList();

                if (!string.IsNullOrEmpty(filters.Status))
                    allReports = allReports.Where(r => r.Status == filters.Status).ToList();
            }

            var model = new ReportHistoryViewModel
            {
                IHEId = iheId,
                IHEName = allApplications.FirstOrDefault()?.IHE.Name ?? "Institution",
                GrantCycleId = grantCycleId,
                Filters = filters ?? new ReportHistoryFilterCriteria(),
                SubmittedReports = allReports.Select(r =>
                {
                    var student = allApplications.SelectMany(a => a.Students).First(s => s.Id == r.StudentId);
                    var app = allApplications.First(a => a.Id == r.ApplicationId);

                    return new SubmittedReportViewModel
                    {
                        ReportId = r.Id,
                        StudentId = r.StudentId,
                        StudentName = $"{student.FirstName} {student.LastName}",
                        SEID = student.SEID,
                        LEAName = app.LEA.Name,
                        SubmittedDate = r.SubmittedDate,
                        SubmittedBy = r.SubmittedBy,
                        Status = r.Status,
                        CompletionStatus = r.CompletionStatus,
                        CredentialEarned = r.CredentialEarned,
                        EmployedInDistrict = r.EmployedInDistrict,
                        ConfirmationNumber = r.ConfirmationNumber
                    };
                }).ToList(),
                TotalCount = allReports.Count,
                AvailablePeriods = _repository.GetReportingPeriods(grantCycleId).Select(rp => new ReportingPeriodViewModel
                {
                    Id = rp.Id,
                    PeriodName = rp.PeriodName,
                    StartDate = rp.StartDate,
                    DueDate = rp.DueDate
                }).ToList(),
                AvailableLEAs = allApplications.Select(a => new OrganizationOption
                {
                    Id = a.LEA.Id,
                    Name = a.LEA.Name
                }).Distinct().ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading report history");
            return View("Error");
        }
    }

    [Route("BulkUpload")]
    public IActionResult BulkUpload(int iheId = 1, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _repository.GetGrantCycle(grantCycleId);
            var allApplications = _repository.GetApplications()
                .Where(a => a.IHE.Id == iheId && a.GrantCycleId == grantCycleId)
                .ToList();

            var activeReportingPeriod = _repository.GetReportingPeriods(grantCycleId).FirstOrDefault(rp => rp.IsActive);

            var model = new BulkUploadViewModel
            {
                IHEId = iheId,
                IHEName = allApplications.FirstOrDefault()?.IHE.Name ?? "Institution",
                GrantCycleId = grantCycleId,
                ReportingPeriodId = activeReportingPeriod?.Id ?? 0,
                ReportingPeriod = activeReportingPeriod != null ? new ReportingPeriodViewModel
                {
                    Id = activeReportingPeriod.Id,
                    PeriodName = activeReportingPeriod.PeriodName,
                    StartDate = activeReportingPeriod.StartDate,
                    DueDate = activeReportingPeriod.DueDate,
                    Description = activeReportingPeriod.Description
                } : null
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
    [Route("ProcessBulkUpload")]
    public IActionResult ProcessBulkUpload(BulkUploadViewModel model)
    {
        try
        {
            // Mock processing - in production would parse Excel/CSV
            _logger.LogInformation("Processing bulk upload for IHE {IHEId}", model.IHEId);

            TempData["SuccessMessage"] = "Bulk upload processed successfully. Mock: In production, this would validate and import all reports.";
            return RedirectToAction("FundedCandidates", new { iheId = model.IHEId, grantCycleId = model.GrantCycleId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing bulk upload");
            TempData["ErrorMessage"] = "Error processing bulk upload";
            return RedirectToAction("BulkUpload", new { iheId = model.IHEId, grantCycleId = model.GrantCycleId });
        }
    }

    [Route("ViewReport/{reportId}")]
    public IActionResult ViewReport(int reportId)
    {
        try
        {
            var report = _repository.GetIHEReport(reportId);
            if (report == null)
                return NotFound("Report not found");

            // Redirect to CandidateReport view which shows the report in read-only mode
            return RedirectToAction("CandidateReport", new { studentId = report.StudentId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing report");
            return View("Error");
        }
    }

    [Route("StudentDetail/{studentId}")]
    public IActionResult StudentDetail(int studentId)
    {
        try
        {
            var allApplications = _repository.GetApplications();
            var student = allApplications.SelectMany(a => a.Students).FirstOrDefault(s => s.Id == studentId);

            if (student == null)
                return NotFound("Student not found");

            var application = allApplications.First(a => a.Id == student.ApplicationId);
            var grantCycle = _repository.GetGrantCycle(application.GrantCycleId);
            var latestReport = _repository.GetIHEReportsByStudent(studentId).FirstOrDefault();

            var model = new StudentDetailViewModel
            {
                StudentId = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                SEID = student.SEID,
                DateOfBirth = student.DateOfBirth,
                Last4SSN = student.Last4SSN,
                Race = student.Race,
                Ethnicity = student.Ethnicity,
                Gender = student.Gender,
                CredentialArea = student.CredentialArea,
                CountyCDSCode = student.CountyCDSCode,
                SchoolCDSCode = student.SchoolCDSCode,
                ApplicationId = application.Id,
                IHEName = application.IHE.Name,
                LEAName = application.LEA.Name,
                GrantCycleName = grantCycle?.Name ?? "",
                Status = student.ApplicationStatus,
                AwardAmount = student.AwardAmount,
                CreatedAt = student.CreatedAt,
                SubmittedAt = student.SubmittedAt,
                ApprovedAt = student.ApprovedAt,
                GrantProgramHours = student.GrantProgramHours,
                CredentialProgramHours = student.CredentialProgramHours,
                CredentialEarned = student.CredentialEarned,
                CredentialEarnedDate = student.CredentialEarnedDate,
                SwitchedToIntern = student.SwitchedToIntern,
                EmployedInDistrict = student.EmployedInDistrict,
                EmployedInState = student.EmployedInState,
                EmploymentStatus = student.EmploymentStatus,
                LatestReport = latestReport
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading student detail");
            return View("Error");
        }
    }

    #endregion

    #region Helper Methods

    private IHEReportFormViewModel MapToReportForm(IHEReport report)
    {
        return new IHEReportFormViewModel
        {
            ReportId = report.Id,
            StudentId = report.StudentId,
            ApplicationId = report.ApplicationId,
            ReportingPeriodId = report.ReportingPeriodId,
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
            CredentialEarned = report.CredentialEarned,
            CredentialEarnedDate = report.CredentialEarnedDate,
            CredentialType = report.CredentialType,
            EmployedInDistrict = report.EmployedInDistrict,
            EmployedInState = report.EmployedInState,
            EmploymentStatus = report.EmploymentStatus,
            EmployerName = report.EmployerName,
            EmploymentStartDate = report.EmploymentStartDate,
            SchoolSite = report.SchoolSite,
            GradeLevel = report.GradeLevel,
            SubjectArea = report.SubjectArea,
            AdditionalNotes = report.AdditionalNotes,
            DocumentationUrl = report.DocumentationUrl,
            Status = report.Status,
            LastModified = report.LastModified
        };
    }

    #endregion
}
