using Microsoft.AspNetCore.Mvc;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("Search")]
public class SearchController : Controller
{
    [HttpGet("")]
    public IActionResult Index(string tab = "candidates")
    {
        ViewData["Title"] = "Search";
        ViewData["ActiveTab"] = tab;
        return View();
    }

    [HttpGet("Candidates")]
    public IActionResult Candidates(
        string? search = null,
        string? status = null,
        string? credentialArea = null,
        int? iheId = null,
        int? leaId = null,
        string? grantCycle = null,
        bool? met500Hours = null,
        bool? met600Hours = null,
        decimal? minAward = null,
        decimal? maxAward = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 25,
        string sortBy = "lastName",
        string sortDir = "asc")
    {
        // Return JSON for AJAX requests
        var candidates = GetMockCandidates();

        // Apply filters
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            candidates = candidates.Where(c =>
                c.SEID.ToLower().Contains(search) ||
                c.FirstName.ToLower().Contains(search) ||
                c.LastName.ToLower().Contains(search)).ToList();
        }

        if (!string.IsNullOrEmpty(status))
        {
            var statuses = status.Split(',');
            candidates = candidates.Where(c => statuses.Contains(c.Status)).ToList();
        }

        if (!string.IsNullOrEmpty(credentialArea))
        {
            var areas = credentialArea.Split(',');
            candidates = candidates.Where(c => areas.Contains(c.CredentialArea)).ToList();
        }

        if (iheId.HasValue)
        {
            candidates = candidates.Where(c => c.IHEId == iheId.Value).ToList();
        }

        if (leaId.HasValue)
        {
            candidates = candidates.Where(c => c.LEAId == leaId.Value).ToList();
        }

        if (met500Hours == true)
        {
            candidates = candidates.Where(c => c.GrantProgramHours >= 500).ToList();
        }

        if (met600Hours == true)
        {
            candidates = candidates.Where(c => c.CredentialProgramHours >= 600).ToList();
        }

        // Pagination
        var totalCount = candidates.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pagedCandidates = candidates
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Json(new
        {
            data = pagedCandidates,
            totalCount,
            totalPages,
            currentPage = page,
            pageSize
        });
    }

    [HttpGet("LEAs")]
    public IActionResult LEAs(
        string? search = null,
        string? grantCycle = null,
        string? county = null,
        string? complianceStatus = null,
        bool? hasActiveCandidates = null,
        int page = 1,
        int pageSize = 25)
    {
        var leas = GetMockLEAs();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            leas = leas.Where(l =>
                l.Name.ToLower().Contains(search) ||
                l.CDSCode.ToLower().Contains(search)).ToList();
        }

        if (hasActiveCandidates == true)
        {
            leas = leas.Where(l => l.TotalCandidates > 0).ToList();
        }

        if (!string.IsNullOrEmpty(complianceStatus))
        {
            leas = leas.Where(l =>
                (complianceStatus == "full" && l.CompliancePercent == 100) ||
                (complianceStatus == "partial" && l.CompliancePercent >= 50 && l.CompliancePercent < 100) ||
                (complianceStatus == "non-compliant" && l.CompliancePercent < 50)).ToList();
        }

        var totalCount = leas.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pagedLEAs = leas.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Json(new
        {
            data = pagedLEAs,
            totalCount,
            totalPages,
            currentPage = page,
            pageSize
        });
    }

    [HttpGet("IHEs")]
    public IActionResult IHEs(
        string? search = null,
        string? grantCycle = null,
        bool? hasActiveCandidates = null,
        int page = 1,
        int pageSize = 25)
    {
        var ihes = GetMockIHEs();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            ihes = ihes.Where(i =>
                i.Name.ToLower().Contains(search) ||
                i.Code.ToLower().Contains(search)).ToList();
        }

        if (hasActiveCandidates == true)
        {
            ihes = ihes.Where(i => i.TotalCandidates > 0).ToList();
        }

        var totalCount = ihes.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pagedIHEs = ihes.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Json(new
        {
            data = pagedIHEs,
            totalCount,
            totalPages,
            currentPage = page,
            pageSize
        });
    }

    [HttpGet("Export")]
    public IActionResult Export(string type, string format, string? columns = null)
    {
        // In production, this would generate actual export files
        // For POC, return a message
        return Json(new { message = $"Export {type} as {format} - columns: {columns ?? "all"}" });
    }

    #region Mock Data

    private List<CandidateSearchResult> GetMockCandidates()
    {
        var statuses = new[] { "DRAFT", "IHE_SUBMITTED", "LEA_REVIEWING", "LEA_APPROVED", "CTC_SUBMITTED",
            "CTC_REVIEWING", "CTC_APPROVED", "GAA_PENDING", "GAA_SIGNED", "INVOICE_GENERATED",
            "PAYMENT_COMPLETE", "REPORTING_PENDING", "REPORTING_COMPLETE", "REPORTS_APPROVED" };
        var credentials = new[] { "Multiple Subject", "Single Subject - Mathematics", "Single Subject - English",
            "Single Subject - Science", "Education Specialist", "PK-3 Early Childhood" };
        var ihes = new[] {
            (1, "CSU Fullerton", "CSUF"), (2, "UCLA", "UCLA"), (3, "San Diego State", "SDSU"),
            (4, "UC Irvine", "UCI"), (5, "CSU Long Beach", "CSULB"), (6, "USC", "USC"),
            (7, "CSU Northridge", "CSUN"), (8, "UC Berkeley", "UCB") };
        var leas = new[] {
            (1, "Los Angeles USD", "19-64733"), (2, "San Diego USD", "37-68338"),
            (3, "Fresno USD", "10-62166"), (4, "Long Beach USD", "19-64733"),
            (5, "Sacramento City USD", "34-67439"), (6, "Oakland USD", "01-61259"),
            (7, "San Francisco USD", "38-68478"), (8, "Santa Ana USD", "30-66621") };
        var firstNames = new[] { "Maria", "James", "Ashley", "Michael", "Sarah", "David", "Emily", "Robert",
            "Jessica", "William", "Jennifer", "Christopher", "Amanda", "Daniel", "Stephanie", "Matthew",
            "Nicole", "Andrew", "Elizabeth", "Joshua", "Lauren", "Ryan", "Megan", "Brandon", "Rachel" };
        var lastNames = new[] { "Garcia", "Wilson", "Chen", "Johnson", "Brown", "Martinez", "Anderson", "Taylor",
            "Thomas", "Jackson", "White", "Harris", "Clark", "Lewis", "Robinson", "Walker", "Young", "King",
            "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker" };

        var candidates = new List<CandidateSearchResult>();
        var random = new Random(42); // Fixed seed for consistent mock data

        for (int i = 1; i <= 250; i++)
        {
            var ihe = ihes[random.Next(ihes.Length)];
            var lea = leas[random.Next(leas.Length)];
            var status = statuses[random.Next(statuses.Length)];
            var grantHours = random.Next(350, 650);
            var credHours = random.Next(450, 750);
            var submittedDate = DateTime.Now.AddDays(-random.Next(10, 180));

            candidates.Add(new CandidateSearchResult
            {
                Id = i,
                SEID = $"SE{100000 + i}",
                FirstName = firstNames[random.Next(firstNames.Length)],
                LastName = lastNames[random.Next(lastNames.Length)],
                Status = status,
                StatusDisplay = GetStatusDisplay(status),
                StatusCategory = GetStatusCategory(status),
                CredentialArea = credentials[random.Next(credentials.Length)],
                IHEId = ihe.Item1,
                IHEName = ihe.Item2,
                IHECode = ihe.Item3,
                LEAId = lea.Item1,
                LEAName = lea.Item2,
                CDSCode = lea.Item3,
                AwardAmount = 10000m,
                GrantProgramHours = grantHours,
                CredentialProgramHours = credHours,
                Met500Hours = grantHours >= 500,
                Met600Hours = credHours >= 600,
                CredentialEarned = status == "REPORTS_APPROVED" ? random.Next(100) > 20 : (bool?)null,
                EmployedInDistrict = status == "REPORTS_APPROVED" ? random.Next(100) > 30 : (bool?)null,
                EmployedInState = status == "REPORTS_APPROVED" ? random.Next(100) > 15 : (bool?)null,
                EmploymentStatus = status == "REPORTS_APPROVED" ? (random.Next(100) > 50 ? "Full-time" : "Part-time") : null,
                SubmittedAt = submittedDate,
                ApprovedAt = status.StartsWith("CTC_APPROVED") || status.StartsWith("GAA") || status.StartsWith("INVOICE") ||
                    status.StartsWith("PAYMENT") || status.StartsWith("REPORT") ? submittedDate.AddDays(random.Next(5, 30)) : (DateTime?)null,
                CreatedAt = submittedDate.AddDays(-random.Next(1, 10)),
                LastActionDate = DateTime.Now.AddDays(-random.Next(1, 30)),
                LastActionBy = random.Next(100) > 50 ? "CTC Grants Team" : "LEA Coordinator",
                GrandCycle = "2024-25",
                ApplicationId = (i % 20) + 1
            });
        }

        return candidates;
    }

    private List<LEASearchResult> GetMockLEAs()
    {
        return new List<LEASearchResult>
        {
            new() { Id = 1, Name = "Los Angeles Unified School District", CDSCode = "19-64733-0000000", County = "Los Angeles",
                IHEPartners = 6, TotalCandidates = 85, Approved = 72, Paid = 65, TotalAwards = 850000m, CompliancePercent = 92,
                PendingReview = 8, InDisbursement = 12, InReporting = 45, ReportsSubmitted = 58, ReportsOverdue = 3 },
            new() { Id = 2, Name = "San Diego Unified School District", CDSCode = "37-68338-0000000", County = "San Diego",
                IHEPartners = 4, TotalCandidates = 52, Approved = 48, Paid = 42, TotalAwards = 520000m, CompliancePercent = 88,
                PendingReview = 4, InDisbursement = 8, InReporting = 32, ReportsSubmitted = 40, ReportsOverdue = 2 },
            new() { Id = 3, Name = "Fresno Unified School District", CDSCode = "10-62166-0000000", County = "Fresno",
                IHEPartners = 3, TotalCandidates = 38, Approved = 35, Paid = 30, TotalAwards = 380000m, CompliancePercent = 95,
                PendingReview = 3, InDisbursement = 5, InReporting = 22, ReportsSubmitted = 28, ReportsOverdue = 0 },
            new() { Id = 4, Name = "Long Beach Unified School District", CDSCode = "19-64733-1930000", County = "Los Angeles",
                IHEPartners = 3, TotalCandidates = 42, Approved = 38, Paid = 35, TotalAwards = 420000m, CompliancePercent = 100,
                PendingReview = 4, InDisbursement = 6, InReporting = 26, ReportsSubmitted = 32, ReportsOverdue = 0 },
            new() { Id = 5, Name = "Sacramento City Unified School District", CDSCode = "34-67439-0000000", County = "Sacramento",
                IHEPartners = 4, TotalCandidates = 35, Approved = 30, Paid = 28, TotalAwards = 350000m, CompliancePercent = 85,
                PendingReview = 5, InDisbursement = 4, InReporting = 19, ReportsSubmitted = 22, ReportsOverdue = 4 },
            new() { Id = 6, Name = "Oakland Unified School District", CDSCode = "01-61259-0000000", County = "Alameda",
                IHEPartners = 3, TotalCandidates = 28, Approved = 25, Paid = 22, TotalAwards = 280000m, CompliancePercent = 78,
                PendingReview = 3, InDisbursement = 5, InReporting = 14, ReportsSubmitted = 15, ReportsOverdue = 5 },
            new() { Id = 7, Name = "San Francisco Unified School District", CDSCode = "38-68478-0000000", County = "San Francisco",
                IHEPartners = 5, TotalCandidates = 45, Approved = 42, Paid = 38, TotalAwards = 450000m, CompliancePercent = 91,
                PendingReview = 3, InDisbursement = 7, InReporting = 28, ReportsSubmitted = 35, ReportsOverdue = 2 },
            new() { Id = 8, Name = "Santa Ana Unified School District", CDSCode = "30-66621-0000000", County = "Orange",
                IHEPartners = 2, TotalCandidates = 22, Approved = 20, Paid = 18, TotalAwards = 220000m, CompliancePercent = 100,
                PendingReview = 2, InDisbursement = 3, InReporting = 13, ReportsSubmitted = 16, ReportsOverdue = 0 },
            new() { Id = 9, Name = "San Bernardino City USD", CDSCode = "36-67827-0000000", County = "San Bernardino",
                IHEPartners = 3, TotalCandidates = 30, Approved = 26, Paid = 24, TotalAwards = 300000m, CompliancePercent = 82,
                PendingReview = 4, InDisbursement = 4, InReporting = 16, ReportsSubmitted = 18, ReportsOverdue = 4 },
            new() { Id = 10, Name = "Riverside Unified School District", CDSCode = "33-67124-0000000", County = "Riverside",
                IHEPartners = 2, TotalCandidates = 18, Approved = 16, Paid = 14, TotalAwards = 180000m, CompliancePercent = 75,
                PendingReview = 2, InDisbursement = 3, InReporting = 9, ReportsSubmitted = 10, ReportsOverdue = 3 }
        };
    }

    private List<IHESearchResult> GetMockIHEs()
    {
        return new List<IHESearchResult>
        {
            new() { Id = 1, Name = "California State University, Fullerton", Code = "CSUF",
                LEAPartners = 8, TotalCandidates = 65, Submitted = 62, Approved = 58, CompletionRate = 89, CredentialRate = 82,
                PendingLEAReview = 3, PendingCTCReview = 4, InDisbursement = 15, InReporting = 36, Met500Count = 58, Met600Count = 53,
                EmploymentRate = 78, InDistrictRate = 65, ReportsSubmitted = 48, ReportsOverdue = 2 },
            new() { Id = 2, Name = "University of California, Los Angeles", Code = "UCLA",
                LEAPartners = 6, TotalCandidates = 48, Submitted = 46, Approved = 44, CompletionRate = 92, CredentialRate = 88,
                PendingLEAReview = 2, PendingCTCReview = 2, InDisbursement = 12, InReporting = 28, Met500Count = 44, Met600Count = 42,
                EmploymentRate = 85, InDistrictRate = 72, ReportsSubmitted = 38, ReportsOverdue = 1 },
            new() { Id = 3, Name = "San Diego State University", Code = "SDSU",
                LEAPartners = 5, TotalCandidates = 42, Submitted = 40, Approved = 38, CompletionRate = 90, CredentialRate = 85,
                PendingLEAReview = 2, PendingCTCReview = 2, InDisbursement = 10, InReporting = 24, Met500Count = 38, Met600Count = 36,
                EmploymentRate = 80, InDistrictRate = 68, ReportsSubmitted = 32, ReportsOverdue = 1 },
            new() { Id = 4, Name = "University of California, Irvine", Code = "UCI",
                LEAPartners = 4, TotalCandidates = 35, Submitted = 34, Approved = 32, CompletionRate = 91, CredentialRate = 86,
                PendingLEAReview = 1, PendingCTCReview = 2, InDisbursement = 8, InReporting = 20, Met500Count = 32, Met600Count = 30,
                EmploymentRate = 82, InDistrictRate = 70, ReportsSubmitted = 26, ReportsOverdue = 0 },
            new() { Id = 5, Name = "California State University, Long Beach", Code = "CSULB",
                LEAPartners = 6, TotalCandidates = 55, Submitted = 52, Approved = 50, CompletionRate = 91, CredentialRate = 84,
                PendingLEAReview = 3, PendingCTCReview = 2, InDisbursement = 14, InReporting = 31, Met500Count = 50, Met600Count = 46,
                EmploymentRate = 79, InDistrictRate = 66, ReportsSubmitted = 40, ReportsOverdue = 2 },
            new() { Id = 6, Name = "University of Southern California", Code = "USC",
                LEAPartners = 5, TotalCandidates = 38, Submitted = 36, Approved = 35, CompletionRate = 95, CredentialRate = 92,
                PendingLEAReview = 2, PendingCTCReview = 1, InDisbursement = 9, InReporting = 23, Met500Count = 35, Met600Count = 34,
                EmploymentRate = 88, InDistrictRate = 75, ReportsSubmitted = 30, ReportsOverdue = 0 },
            new() { Id = 7, Name = "California State University, Northridge", Code = "CSUN",
                LEAPartners = 5, TotalCandidates = 40, Submitted = 38, Approved = 36, CompletionRate = 88, CredentialRate = 80,
                PendingLEAReview = 2, PendingCTCReview = 2, InDisbursement = 10, InReporting = 22, Met500Count = 35, Met600Count = 32,
                EmploymentRate = 75, InDistrictRate = 62, ReportsSubmitted = 28, ReportsOverdue = 3 },
            new() { Id = 8, Name = "University of California, Berkeley", Code = "UCB",
                LEAPartners = 4, TotalCandidates = 32, Submitted = 31, Approved = 30, CompletionRate = 94, CredentialRate = 90,
                PendingLEAReview = 1, PendingCTCReview = 1, InDisbursement = 7, InReporting = 20, Met500Count = 30, Met600Count = 29,
                EmploymentRate = 86, InDistrictRate = 74, ReportsSubmitted = 25, ReportsOverdue = 0 }
        };
    }

    private string GetStatusDisplay(string status) => status switch
    {
        "DRAFT" => "Draft",
        "IHE_SUBMITTED" => "Submitted to LEA",
        "LEA_REVIEWING" => "LEA Reviewing",
        "LEA_APPROVED" => "LEA Approved",
        "CTC_SUBMITTED" => "Submitted to CTC",
        "CTC_REVIEWING" => "CTC Reviewing",
        "CTC_APPROVED" => "CTC Approved",
        "CTC_REJECTED" => "Rejected",
        "REVISION_REQUESTED" => "Revision Requested",
        "GAA_PENDING" => "Awaiting GAA",
        "GAA_GENERATED" => "GAA Generated",
        "GAA_SIGNED" => "GAA Signed",
        "INVOICE_GENERATED" => "Invoice Generated",
        "PAYMENT_AUTHORIZED" => "Payment Authorized",
        "WARRANT_ISSUED" => "Warrant Issued",
        "PAYMENT_COMPLETE" => "Payment Complete",
        "REPORTING_PENDING" => "Reports Pending",
        "REPORTING_PARTIAL" => "Partial Reports",
        "REPORTING_COMPLETE" => "Reports Submitted",
        "REPORTS_APPROVED" => "Complete",
        _ => status
    };

    private string GetStatusCategory(string status) => status switch
    {
        "DRAFT" or "IHE_SUBMITTED" or "LEA_REVIEWING" or "LEA_APPROVED" or "CTC_SUBMITTED" => "submission",
        "CTC_REVIEWING" or "REVISION_REQUESTED" => "review",
        "CTC_REJECTED" => "rejected",
        "CTC_APPROVED" or "GAA_PENDING" or "GAA_GENERATED" or "GAA_SIGNED" or "INVOICE_GENERATED" or
            "PAYMENT_AUTHORIZED" or "WARRANT_ISSUED" or "PAYMENT_COMPLETE" => "disbursement",
        "REPORTING_PENDING" or "REPORTING_PARTIAL" or "REPORTING_COMPLETE" => "reporting",
        "REPORTS_APPROVED" => "complete",
        _ => "unknown"
    };

    #endregion

    #region DTOs

    public class CandidateSearchResult
    {
        public int Id { get; set; }
        public string SEID { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Status { get; set; } = "";
        public string StatusDisplay { get; set; } = "";
        public string StatusCategory { get; set; } = "";
        public string CredentialArea { get; set; } = "";
        public int IHEId { get; set; }
        public string IHEName { get; set; } = "";
        public string IHECode { get; set; } = "";
        public int LEAId { get; set; }
        public string LEAName { get; set; } = "";
        public string CDSCode { get; set; } = "";
        public decimal AwardAmount { get; set; }
        public int GrantProgramHours { get; set; }
        public int CredentialProgramHours { get; set; }
        public bool Met500Hours { get; set; }
        public bool Met600Hours { get; set; }
        public bool? CredentialEarned { get; set; }
        public bool? EmployedInDistrict { get; set; }
        public bool? EmployedInState { get; set; }
        public string? EmploymentStatus { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActionDate { get; set; }
        public string? LastActionBy { get; set; }
        public string GrandCycle { get; set; } = "";
        public int ApplicationId { get; set; }
    }

    public class LEASearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string CDSCode { get; set; } = "";
        public string County { get; set; } = "";
        public int IHEPartners { get; set; }
        public int TotalCandidates { get; set; }
        public int Approved { get; set; }
        public int Paid { get; set; }
        public decimal TotalAwards { get; set; }
        public int CompliancePercent { get; set; }
        public int PendingReview { get; set; }
        public int InDisbursement { get; set; }
        public int InReporting { get; set; }
        public int ReportsSubmitted { get; set; }
        public int ReportsOverdue { get; set; }
    }

    public class IHESearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public int LEAPartners { get; set; }
        public int TotalCandidates { get; set; }
        public int Submitted { get; set; }
        public int Approved { get; set; }
        public int CompletionRate { get; set; }
        public int CredentialRate { get; set; }
        public int PendingLEAReview { get; set; }
        public int PendingCTCReview { get; set; }
        public int InDisbursement { get; set; }
        public int InReporting { get; set; }
        public int Met500Count { get; set; }
        public int Met600Count { get; set; }
        public int EmploymentRate { get; set; }
        public int InDistrictRate { get; set; }
        public int ReportsSubmitted { get; set; }
        public int ReportsOverdue { get; set; }
    }

    #endregion
}
