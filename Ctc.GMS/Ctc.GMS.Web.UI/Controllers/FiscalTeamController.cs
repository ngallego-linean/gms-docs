using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;
using GMS.Business.DTOs;
using Ctc.GMS.Web.UI.Services;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("FiscalTeam")]
public class FiscalTeamController : Controller
{
    private readonly IGrantService _grantService;
    private readonly IDocuSignService _docuSignService;
    private readonly ILogger<FiscalTeamController> _logger;
    private readonly IWebHostEnvironment _environment;

    public FiscalTeamController(
        IGrantService grantService,
        IDocuSignService docuSignService,
        ILogger<FiscalTeamController> logger,
        IWebHostEnvironment environment)
    {
        _grantService = grantService;
        _docuSignService = docuSignService;
        _logger = logger;
        _environment = environment;
    }

    [Route("")]
    [Route("Dashboard")]
    public IActionResult Dashboard(int grantCycleId = 1)
    {
        // Redirect to the unified Grants/Fiscal Team Dashboard
        return RedirectToAction("Dashboard", "GrantsTeam", new { grantCycleId });
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

            // Group students by LEA and submission month (disbursement groups)
            var disbursementGroups = allApplications
                .Where(a => a.GrantCycleId == grantCycleId)
                .SelectMany(a => a.Students.Select(s => new
                {
                    Application = a,
                    Student = s
                }))
                .Where(x => x.Student.Status == "CTC_APPROVED")
                .GroupBy(x => new
                {
                    LEA = x.Application.LEA.Name,
                    Month = x.Student.SubmittedAt?.ToString("yyyy-MM") ?? DateTime.Now.ToString("yyyy-MM")
                })
                .Select((g, index) => new DisbursementGroupViewModel
                {
                    Id = index + 1,
                    LEAName = g.Key.LEA,
                    SubmissionMonth = g.Key.Month,
                    StudentCount = g.Count(),
                    TotalAmount = g.Sum(x => x.Student.AwardAmount),
                    GAAStatus = "GAA_PENDING",
                    Students = g.Select(x => new StudentGAAViewModel
                    {
                        StudentId = x.Student.Id,
                        StudentName = $"{x.Student.FirstName} {x.Student.LastName}",
                        SEID = x.Student.SEID,
                        IHEName = x.Application.IHE.Name,
                        LEAName = x.Application.LEA.Name,
                        CredentialArea = x.Student.CredentialArea,
                        AwardAmount = x.Student.AwardAmount,
                        GAAStatus = x.Student.Status,
                        ApprovedDate = x.Student.SubmittedAt
                    }).ToList()
                })
                .OrderBy(g => g.LEAName)
                .ThenBy(g => g.SubmissionMonth)
                .ToList();

            var model = new GAAListViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                DisbursementGroups = disbursementGroups
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading GAA generation page");
            return View("Error");
        }
    }

    [Route("GAAPreview/{groupId}")]
    public IActionResult GAAPreview(int groupId, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);
            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            // Mock LEA names matching Dashboard mock data
            var mockLEAs = new Dictionary<int, (string Name, string Address, string Contact, string Email)>
            {
                { 1, ("Fresno Unified School District", "2309 Tulare Street, Fresno, CA 93721", "Maria Santos", "msantos@fresnounified.org") },
                { 2, ("Sacramento City Unified School District", "5735 47th Avenue, Sacramento, CA 95824", "James Wilson", "jwilson@scusd.edu") },
                { 3, ("Oakland Unified School District", "1000 Broadway, Suite 150, Oakland, CA 94607", "Lisa Chen", "lchen@ousd.org") },
                { 4, ("Los Angeles Unified School District", "333 South Beaudry Ave, Los Angeles, CA 90017", "Robert Martinez", "rmartinez@lausd.net") },
                { 5, ("San Diego Unified School District", "4100 Normal Street, San Diego, CA 92103", "Patricia Johnson", "pjohnson@sandi.net") },
                { 6, ("Long Beach Unified School District", "1515 Hughes Way, Long Beach, CA 90810", "Michael Brown", "mbrown@lbusd.k12.ca.us") }
            };

            var leaInfo = mockLEAs.GetValueOrDefault(groupId, ($"District {groupId}", "123 Main Street, Sacramento, CA 95814", "Contact Name", "contact@district.org"));

            // Mock IHE diversification
            var mockIHEs = new[] {
                "San Diego State University",
                "UC San Diego",
                "Cal State San Marcos",
                "Point Loma Nazarene University",
                "University of San Diego",
                "CSU Fullerton"
            };

            // Generate mock students for this group
            var mockStudents = Enumerable.Range(1, 5 + (groupId % 3)).Select(i => new StudentGAAViewModel
            {
                StudentId = i,
                StudentName = $"Student Teacher {i}",
                SEID = $"SEID{groupId:D2}{i:D3}",
                IHEName = mockIHEs[(i - 1) % mockIHEs.Length],
                LEAName = leaInfo.Item1,
                CredentialArea = i % 3 == 0 ? "Education Specialist" : (i % 2 == 0 ? "Multiple Subject" : "Single Subject"),
                AwardAmount = 10000m,
                ApprovedDate = DateTime.Now.AddDays(-7 - i)
            }).ToList();

            var fiscalYear = DateTime.Now.Month >= 7 ? DateTime.Now.Year : DateTime.Now.Year - 1;

            var model = new GAAPreviewViewModel
            {
                GroupId = groupId,
                GrantCycleId = grantCycleId,
                GranteeName = leaInfo.Item1,
                GrantNumber = $"STSP-{fiscalYear}-{fiscalYear + 1}-{groupId:D4}",
                AgreementTerm = $"July 1, {fiscalYear} – June 30, {fiscalYear + 1}",
                AgreementTermStart = $"July 1, {fiscalYear}",
                AgreementTermEnd = $"June 30, {fiscalYear + 1}",
                StudentTeacherCount = mockStudents.Count(),
                OriginalAmount = mockStudents.Sum(s => s.AwardAmount),
                SubmissionMonth = DateTime.Now.ToString("MMMM yyyy"),
                Students = mockStudents,
                GranteeSignerName = leaInfo.Item3,
                GranteeSignerEmail = leaInfo.Item4
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading GAA preview for group {GroupId}", groupId);
            return View("Error");
        }
    }

    [HttpPost]
    [Route("SendGAA/{groupId}")]
    public async Task<IActionResult> SendGAA(int groupId, int grantCycleId = 1)
    {
        try
        {
            // Mock LEA names matching Dashboard mock data
            var mockLEAs = new Dictionary<int, string>
            {
                { 1, "Fresno Unified School District" },
                { 2, "Sacramento City Unified School District" },
                { 3, "Oakland Unified School District" },
                { 4, "Los Angeles Unified School District" },
                { 5, "San Diego Unified School District" },
                { 6, "Long Beach Unified School District" }
            };

            var leaName = mockLEAs.GetValueOrDefault(groupId, $"District {groupId}");

            // Mock IHE diversification
            var mockIHEs = new[] {
                "San Diego State University",
                "UC San Diego",
                "Cal State San Marcos",
                "Point Loma Nazarene University",
                "University of San Diego",
                "CSU Fullerton"
            };

            // Generate mock students for this group
            var mockStudents = Enumerable.Range(1, 5).Select(i => new GAAStudentInfo
            {
                StudentName = $"Student {i}",
                SEID = $"SEID{groupId:D2}{i:D3}",
                IHEName = mockIHEs[(i - 1) % mockIHEs.Length],
                CredentialArea = i % 2 == 0 ? "Multiple Subject" : "Single Subject",
                AwardAmount = 10000m
            }).ToList();

            // Build return URL
            var returnUrl = Url.Action("GAACallback", "FiscalTeam", new { groupId }, Request.Scheme);

            var request = new GAAEnvelopeRequest
            {
                GroupId = groupId,
                LEAName = leaName,
                GrantNumber = $"STS-{grantCycleId}-{groupId:D4}", // Mock grant number
                AgreementTermStart = "July 1, 2024",
                AgreementTermEnd = "June 30, 2025",
                SubmissionMonth = DateTime.Now.ToString("MMMM yyyy"),
                TotalAmount = mockStudents.Sum(s => s.AwardAmount),
                StudentCount = mockStudents.Count,
                Students = mockStudents,
                // For demo, use a test signer - in production, this would be the LEA representative
                SignerEmail = "noah.gallego@ctc.ca.gov", // Replace with actual signer
                SignerName = "GAA Signer",
                ReturnUrl = returnUrl ?? $"{Request.Scheme}://{Request.Host}/FiscalTeam/GAA"
            };

            var signingUrl = await _docuSignService.SendGAAForSigningAsync(request);

            _logger.LogInformation("DocuSign envelope created for group {GroupId}, redirecting to signing", groupId);

            return Json(new { success = true, signingUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending GAA to DocuSign for group {GroupId}", groupId);
            return Json(new { success = false, error = ex.Message });
        }
    }

    [Route("GAACallback")]
    public IActionResult GAACallback(int groupId, string @event = "")
    {
        // Handle DocuSign callback after signing
        _logger.LogInformation("DocuSign callback received for group {GroupId} with event: {Event}", groupId, @event);

        if (@event == "signing_complete")
        {
            TempData["SuccessMessage"] = $"GAA for group {groupId} has been signed successfully!";
        }
        else if (@event == "decline")
        {
            TempData["ErrorMessage"] = $"GAA for group {groupId} was declined.";
        }
        else if (@event == "cancel")
        {
            TempData["WarningMessage"] = $"GAA signing for group {groupId} was cancelled.";
        }

        return RedirectToAction("GAA");
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

    [Route("Invoices")]
    public IActionResult Invoices(int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var allApplications = _grantService.GetApplications();

            // Mock: Group students by LEA who have GAA signed (DocuSign completed)
            // In production, this would query actual disbursement groups with signed GAAs
            var disbursementGroups = allApplications
                .Where(a => a.GrantCycleId == grantCycleId)
                .GroupBy(a => a.LEA.Name)
                .Select((group, index) => new DisbursementGroupInvoiceViewModel
                {
                    DisbursementGroupId = index + 1,
                    LEAName = group.Key,
                    LEAAddress = $"{index + 100} Main Street, Sacramento, CA 95814", // Mock address
                    StudentCount = group.SelectMany(a => a.Students).Count(s => s.Status == "APPROVED"),
                    TotalAmount = group.SelectMany(a => a.Students).Where(s => s.Status == "APPROVED").Sum(s => s.AwardAmount),
                    GAASignedDate = DateTime.Now.AddDays(-7), // Mock: signed 7 days ago
                    DaysSinceSigned = 7,
                    InvoiceGenerated = false
                })
                .Where(dg => dg.StudentCount > 0)
                .ToList();

            var model = new InvoiceListViewModel
            {
                GrantCycleId = grantCycleId,
                GrantCycleName = grantCycle.Name,
                DisbursementGroups = disbursementGroups
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading invoice generation page");
            return View("Error");
        }
    }

    [Route("GenerateInvoice")]
    public IActionResult GenerateInvoice(int disbursementGroupId, int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var allApplications = _grantService.GetApplications();

            // Mock: Get disbursement group details
            // In production, this would query actual disbursement group by ID
            var leaGroups = allApplications
                .Where(a => a.GrantCycleId == grantCycleId)
                .GroupBy(a => a.LEA.Name)
                .ToList();

            if (disbursementGroupId < 1 || disbursementGroupId > leaGroups.Count)
            {
                return NotFound("Disbursement group not found");
            }

            var group = leaGroups[disbursementGroupId - 1];
            var students = group.SelectMany(a => a.Students.Select(s => new
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
                GAAStatus = "GAA_SIGNED"
            })
            .ToList();

            var model = new InvoiceGenerationViewModel
            {
                DisbursementGroupId = disbursementGroupId,
                LEAName = group.Key,
                LEAAddress = $"{disbursementGroupId + 99} Main Street, Sacramento, CA 95814", // Mock
                InvoiceNumber = $"INV-{grantCycleId}-{disbursementGroupId:D4}",
                Amount = students.Sum(s => s.AwardAmount),
                StudentCount = students.Count,
                Students = students,
                InvoiceDate = DateTime.Now,
                PONumber = "" // To be filled by user
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading invoice generation form");
            return View("Error");
        }
    }

    [HttpPost]
    [Route("GenerateInvoice")]
    public IActionResult GenerateInvoice(InvoiceGenerationViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                // Repopulate the Students list before returning to view
                RepopulateInvoiceModel(model, 1);
                return View(model);
            }

            // Mock: In production, this would save the invoice to database
            // and trigger payment processing workflow
            _logger.LogInformation($"Invoice generated: {model.InvoiceNumber} for {model.LEAName}, PO: {model.PONumber}, Amount: {model.Amount:C0}");

            TempData["SuccessMessage"] = $"Invoice {model.InvoiceNumber} generated successfully for {model.LEAName}";

            return RedirectToAction("Invoices", new { grantCycleId = 1 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving invoice");
            RepopulateInvoiceModel(model, 1);
            ModelState.AddModelError("", "An error occurred while generating the invoice");
            return View(model);
        }
    }

    [Route("InvoicePreview/{groupId}")]
    public IActionResult InvoicePreview(int groupId, int grantCycleId = 1)
    {
        // Mock LEA data for invoice preview
        var mockLEAs = new Dictionary<int, (string Name, string Address, string City, string Zip)>
        {
            { 1, ("Fresno Unified School District", "2309 Tulare Street", "Fresno", "93721") },
            { 2, ("Sacramento City Unified School District", "5735 47th Avenue", "Sacramento", "95824") },
            { 3, ("Los Angeles Unified School District", "333 S Beaudry Avenue", "Los Angeles", "90017") },
            { 4, ("San Diego Unified School District", "4100 Normal Street", "San Diego", "92103") }
        };

        var leaInfo = mockLEAs.GetValueOrDefault(groupId, ("Unknown LEA", "123 Main Street", "Sacramento", "95814"));

        // Mock student data
        var mockStudents = new List<StudentGAAViewModel>();
        var studentNames = new[] { "Emily Rodriguez", "James Chen", "Sarah Williams", "Michael Brown", "Jessica Lee", "David Martinez" };
        var seids = new[] { "ST1234567", "ST2345678", "ST3456789", "ST4567890", "ST5678901", "ST6789012" };
        var credentials = new[] { "Multiple Subject", "Single Subject - Math", "Education Specialist", "Single Subject - English", "Multiple Subject", "Single Subject - Science" };

        var studentCount = groupId switch { 1 => 6, 2 => 4, 3 => 8, _ => 3 };
        for (int i = 0; i < studentCount; i++)
        {
            mockStudents.Add(new StudentGAAViewModel
            {
                StudentId = i + 1,
                StudentName = studentNames[i % studentNames.Length],
                SEID = seids[i % seids.Length],
                CredentialArea = credentials[i % credentials.Length],
                AwardAmount = 10000m
            });
        }

        var model = new InvoicePreviewViewModel
        {
            GroupId = groupId,
            GrantCycleId = grantCycleId,
            GranteeName = leaInfo.Item1,
            GranteeAddress = leaInfo.Item2,
            GranteeCity = leaInfo.Item3,
            GranteeState = "CA",
            GranteeZip = leaInfo.Item4,
            InvoiceNumber = $"INV-STS-2025-{groupId:D3}",
            InvoiceDate = DateTime.Now,
            PONumber = $"PO-{DateTime.Now:yyyyMM}-{groupId:D3}",
            GrantNumber = $"STS-2025-{groupId:D3}",
            TotalAmount = mockStudents.Sum(s => s.AwardAmount),
            StudentCount = mockStudents.Count,
            Students = mockStudents
        };

        return View(model);
    }

    private void RepopulateInvoiceModel(InvoiceGenerationViewModel model, int grantCycleId)
    {
        try
        {
            var allApplications = _grantService.GetApplications();
            var leaGroups = allApplications
                .Where(a => a.GrantCycleId == grantCycleId)
                .GroupBy(a => a.LEA.Name)
                .ToList();

            if (model.DisbursementGroupId > 0 && model.DisbursementGroupId <= leaGroups.Count)
            {
                var group = leaGroups[model.DisbursementGroupId - 1];
                model.Students = group.SelectMany(a => a.Students.Select(s => new
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
                    GAAStatus = "GAA_SIGNED"
                })
                .ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error repopulating invoice model");
            model.Students = new List<StudentGAAViewModel>();
        }
    }

    [Route("Reports")]
    public IActionResult Reports(int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var paymentsWithReportsDTO = _grantService.GetPaymentsWithReportStatus(grantCycleId);
            var paymentsWithReports = paymentsWithReportsDTO.Select(MapToPaymentWithReportsViewModel).ToList();

            var reportingMetricsDTO = _grantService.GetReportingDashboardMetrics(grantCycleId);
            var reportingMetrics = MapToReportingDashboardViewModel(reportingMetricsDTO);

            ViewBag.GrantCycleId = grantCycleId;
            ViewBag.GrantCycleName = grantCycle.Name;
            ViewBag.ReportingMetrics = reportingMetrics;

            return View(paymentsWithReports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reports page");
            return View("Error");
        }
    }

    [Route("LEACompliance")]
    public IActionResult LEACompliance(int grantCycleId = 1)
    {
        try
        {
            var grantCycle = _grantService.GetGrantCycle(grantCycleId);

            if (grantCycle == null)
            {
                return NotFound("Grant cycle not found");
            }

            var complianceDataDTO = _grantService.GetReportingComplianceByLEA(grantCycleId);
            var complianceData = complianceDataDTO.Select(MapToReportingComplianceViewModel).ToList();

            var reportingMetricsDTO = _grantService.GetReportingDashboardMetrics(grantCycleId);
            var reportingMetrics = MapToReportingDashboardViewModel(reportingMetricsDTO);

            ViewBag.GrantCycleId = grantCycleId;
            ViewBag.GrantCycleName = grantCycle.Name;
            ViewBag.ReportingMetrics = reportingMetrics;

            return View(complianceData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading LEA compliance page");
            return View("Error");
        }
    }

    // Mapping methods
    private PaymentWithReportsViewModel MapToPaymentWithReportsViewModel(PaymentWithReportsDTO dto)
    {
        return new PaymentWithReportsViewModel
        {
            PaymentId = dto.PaymentId,
            StudentName = dto.StudentName,
            SEID = dto.SEID,
            LEAName = dto.LEAName,
            AmountPaid = dto.AmountPaid,
            PaymentDate = dto.PaymentDate,
            PaymentStatus = dto.PaymentStatus,
            LEAReportStatus = dto.LEAReportStatus,
            IHEReportStatus = dto.IHEReportStatus,
            HasOutstandingReports = dto.HasOutstandingReports,
            ProgramCompletion = dto.ProgramCompletion,
            CredentialEarned = dto.CredentialEarned,
            EmploymentStatus = dto.EmploymentStatus,
            HiredInDistrict = dto.HiredInDistrict
        };
    }

    private ReportingComplianceViewModel MapToReportingComplianceViewModel(ReportingComplianceDTO dto)
    {
        return new ReportingComplianceViewModel
        {
            LEAName = dto.LEAName,
            TotalPayments = dto.TotalPayments,
            ReportsSubmitted = dto.ReportsSubmitted,
            ReportsPending = dto.ReportsPending,
            ComplianceRate = dto.ComplianceRate,
            ComplianceStatus = dto.ComplianceStatus,
            HasPaymentHoldWarning = dto.HasPaymentHoldWarning,
            TotalDisbursed = dto.TotalDisbursed
        };
    }

    private FiscalReportingDashboardViewModel MapToReportingDashboardViewModel(ReportingDashboardDTO dto)
    {
        return new FiscalReportingDashboardViewModel
        {
            TotalPayments = dto.TotalPayments,
            PaymentsWithReports = dto.PaymentsWithReports,
            OutstandingReports = dto.OutstandingReports,
            ReportingComplianceRate = dto.ReportingComplianceRate,
            ComplianceHealth = dto.ComplianceHealth,
            OutcomeMetrics = MapToOutcomeMetricsViewModel(dto.OutcomeMetrics)
        };
    }

    private OutcomeMetricsViewModel MapToOutcomeMetricsViewModel(OutcomeMetricsDTO dto)
    {
        return new OutcomeMetricsViewModel
        {
            TotalStudentsFunded = dto.TotalStudentsFunded,
            ProgramCompletions = dto.ProgramCompletions,
            CompletionRate = dto.CompletionRate,
            CredentialsEarned = dto.CredentialsEarned,
            CredentialRate = dto.CredentialRate,
            TeachersEmployed = dto.TeachersEmployed,
            EmploymentRate = dto.EmploymentRate,
            TotalInvestment = dto.TotalInvestment,
            CostPerSuccessfulTeacher = dto.CostPerSuccessfulTeacher
        };
    }

    [Route("ViewDocuments/{id}")]
    public IActionResult ViewDocuments(int id)
    {
        // Mock data for POC - maps to disbursement groups from DisbursementQueue.cshtml
        var mockGroups = new Dictionary<int, dynamic>
        {
            { 1, new { LEA = "Fresno Unified School District", Amount = 67500m, StudentCount = 9,
                Step1Date = (DateTime?)null, Step2Date = (DateTime?)null, Step3Date = (DateTime?)null,
                Step4Date = (DateTime?)null, Step5Date = (DateTime?)null, Step6Date = (DateTime?)null }},
            { 2, new { LEA = "Sacramento City Unified School District", Amount = 48000m, StudentCount = 6,
                Step1Date = (DateTime?)DateTime.Now.AddDays(-2), Step2Date = (DateTime?)null, Step3Date = (DateTime?)null,
                Step4Date = (DateTime?)null, Step5Date = (DateTime?)null, Step6Date = (DateTime?)null }},
            { 3, new { LEA = "Oakland Unified School District", Amount = 42000m, StudentCount = 5,
                Step1Date = (DateTime?)DateTime.Now.AddDays(-8), Step2Date = (DateTime?)DateTime.Now.AddDays(-3), Step3Date = (DateTime?)null,
                Step4Date = (DateTime?)null, Step5Date = (DateTime?)null, Step6Date = (DateTime?)null }},
            { 4, new { LEA = "Los Angeles Unified School District", Amount = 125000m, StudentCount = 15,
                Step1Date = (DateTime?)DateTime.Now.AddDays(-12), Step2Date = (DateTime?)DateTime.Now.AddDays(-8), Step3Date = (DateTime?)DateTime.Now.AddDays(-5),
                Step4Date = (DateTime?)DateTime.Now.AddDays(-2), Step5Date = (DateTime?)null, Step6Date = (DateTime?)null }},
            { 5, new { LEA = "San Diego Unified School District", Amount = 87500m, StudentCount = 10,
                Step1Date = (DateTime?)DateTime.Now.AddDays(-18), Step2Date = (DateTime?)DateTime.Now.AddDays(-14), Step3Date = (DateTime?)DateTime.Now.AddDays(-10),
                Step4Date = (DateTime?)DateTime.Now.AddDays(-5), Step5Date = (DateTime?)DateTime.Now.AddDays(-2), Step6Date = (DateTime?)DateTime.Now.AddDays(-1) }},
            { 6, new { LEA = "Long Beach Unified School District", Amount = 52500m, StudentCount = 7,
                Step1Date = (DateTime?)null, Step2Date = (DateTime?)null, Step3Date = (DateTime?)null,
                Step4Date = (DateTime?)null, Step5Date = (DateTime?)null, Step6Date = (DateTime?)null }},
            { 7, new { LEA = "Riverside Unified School District", Amount = 95000m, StudentCount = 12,
                Step1Date = (DateTime?)DateTime.Now.AddDays(-28), Step2Date = (DateTime?)DateTime.Now.AddDays(-24), Step3Date = (DateTime?)DateTime.Now.AddDays(-20),
                Step4Date = (DateTime?)DateTime.Now.AddDays(-15), Step5Date = (DateTime?)DateTime.Now.AddDays(-10), Step6Date = (DateTime?)DateTime.Now.AddDays(-5) }}
        };

        if (!mockGroups.ContainsKey(id))
        {
            return NotFound("Disbursement group not found");
        }

        var group = mockGroups[id];

        ViewBag.GroupId = id;
        ViewBag.LEAName = group.LEA;
        ViewBag.Amount = group.Amount;
        ViewBag.StudentCount = group.StudentCount;
        ViewBag.Step1Date = group.Step1Date;
        ViewBag.Step2Date = group.Step2Date;
        ViewBag.Step3Date = group.Step3Date;
        ViewBag.Step4Date = group.Step4Date;
        ViewBag.Step5Date = group.Step5Date;
        ViewBag.Step6Date = group.Step6Date;

        return View();
    }

    [Route("UploadPO/{id}")]
    public IActionResult UploadPO(int id)
    {
        // Mock LEA names for POC
        var leaNames = new Dictionary<int, string>
        {
            { 1, "Fresno Unified School District" },
            { 2, "Sacramento City Unified School District" },
            { 3, "Oakland Unified School District" },
            { 4, "Los Angeles Unified School District" },
            { 5, "San Diego Unified School District" },
            { 6, "Long Beach Unified School District" }
        };

        ViewBag.GroupId = id;
        ViewBag.LEAName = leaNames.ContainsKey(id) ? leaNames[id] : "Unknown District";

        return View();
    }

    [HttpPost]
    [Route("UploadPO/{id}")]
    public IActionResult UploadPO(int id, IFormFile poFile)
    {
        // Mock upload processing for POC
        TempData["SuccessMessage"] = $"Purchase Order uploaded successfully for group {id}.";
        return RedirectToAction("Dashboard", "GrantsTeam");
    }

    [Route("DownloadInvoice/{id}")]
    public IActionResult DownloadInvoice(int id)
    {
        // Try to find and download the invoice template
        var templatePath = Path.Combine(_environment.ContentRootPath, "docs", "Grant_Invoice_Template.docx");
        var parentTemplatePath = Path.Combine(_environment.ContentRootPath, "..", "docs", "Grant_Invoice_Template.docx");

        if (System.IO.File.Exists(templatePath))
        {
            var fileBytes = System.IO.File.ReadAllBytes(templatePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Grant_Invoice_Template.docx");
        }
        else if (System.IO.File.Exists(parentTemplatePath))
        {
            var fileBytes = System.IO.File.ReadAllBytes(parentTemplatePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Grant_Invoice_Template.docx");
        }

        // If template not found, redirect with message
        TempData["WarningMessage"] = "Invoice template not found. In production, this would download a pre-filled invoice.";
        return RedirectToAction("Dashboard", "GrantsTeam");
    }

    [Route("UploadWarrant/{id}")]
    public IActionResult UploadWarrant(int id)
    {
        // Mock LEA names for POC
        var leaNames = new Dictionary<int, string>
        {
            { 1, "Fresno Unified School District" },
            { 2, "Sacramento City Unified School District" },
            { 3, "Oakland Unified School District" },
            { 4, "Los Angeles Unified School District" },
            { 5, "San Diego Unified School District" },
            { 6, "Long Beach Unified School District" }
        };

        ViewBag.GroupId = id;
        ViewBag.LEAName = leaNames.ContainsKey(id) ? leaNames[id] : "Unknown District";

        return View();
    }

    [HttpPost]
    [Route("UploadWarrant/{id}")]
    public IActionResult UploadWarrant(int id, string warrantNumber, DateTime warrantDate)
    {
        // Mock warrant processing for POC
        TempData["SuccessMessage"] = $"Warrant #{warrantNumber} confirmed for group {id} (Date: {warrantDate:MM/dd/yyyy}).";
        return RedirectToAction("Dashboard", "GrantsTeam");
    }

    [Route("DisbursementQueue")]
    public IActionResult DisbursementQueue()
    {
        return View();
    }
}
