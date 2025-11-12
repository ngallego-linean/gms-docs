using System.ComponentModel.DataAnnotations;
using GMS.DomainModel;

namespace Ctc.GMS.AspNetCore.ViewModels;

public class IHEDashboardViewModel
{
    public int IHEId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;

    // Candidate Status Metrics (renamed from Application)
    public int TotalCandidates { get; set; }
    public int DraftCount { get; set; }
    public int UnderReviewCount { get; set; }
    public int ApprovedCount { get; set; }

    // Submissions by LEA Partner
    public List<ApplicationSummaryViewModel> Submissions { get; set; } = new();

    // Action Items
    public List<ActionItemViewModel> ActionItems { get; set; } = new();

    // Post-Payment Reporting Section Metrics
    public int FundedCandidatesCount { get; set; }
    public int ReportsDue { get; set; }
    public int ReportsInProgress { get; set; }
    public int ReportsSubmitted { get; set; }
    public DateTime? NextReportDeadline { get; set; }
    public string? NextReportPeriodName { get; set; }
    public int? DaysUntilDeadline { get; set; }
    public bool HasActiveReportingPeriod { get; set; }
    public List<ReportingAlertViewModel> ReportingAlerts { get; set; } = new();
}

/// <summary>
/// Alert notification for reporting actions needed
/// </summary>
public class ReportingAlertViewModel
{
    public string AlertType { get; set; } = string.Empty;  // INFO, WARNING, DANGER
    public string Message { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string ActionText { get; set; } = string.Empty;
}

public class SubmitCandidatesViewModel
{
    public int IHEId { get; set; }
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "POC First Name")]
    public string POCFirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "POC Last Name")]
    public string POCLastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "POC Email")]
    [EmailAddress]
    public string POCEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "POC Phone")]
    [Phone]
    public string POCPhone { get; set; } = string.Empty;

    public List<CandidateViewModel> Candidates { get; set; } = new();
}

/// <summary>
/// ViewModel for bulk uploading candidates via Excel/CSV
/// </summary>
public class BulkUploadCandidatesViewModel
{
    public int IHEId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public int UploadedCount { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<BulkUploadError> Errors { get; set; } = new();
}

public class CandidateViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [Display(Name = "Last 4 of SSN/ITIN")]
    [StringLength(4, MinimumLength = 4)]
    public string Last4SSN { get; set; } = string.Empty;

    // Demographic Information
    [Display(Name = "Race")]
    public string Race { get; set; } = string.Empty;

    [Display(Name = "Ethnicity")]
    public string Ethnicity { get; set; } = string.Empty;

    [Display(Name = "Gender")]
    public string Gender { get; set; } = string.Empty;

    [Display(Name = "Has Multiple Demographics")]
    public bool HasMultipleDemographics { get; set; }

    [Required]
    [Display(Name = "Credential Area")]
    public string CredentialArea { get; set; } = string.Empty;

    [Required]
    [Display(Name = "County CDS Code")]
    public string CountyCDSCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA CDS Code")]
    public string LEACDSCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "School CDS Code")]
    public string SchoolCDSCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC First Name")]
    public string LEAPOCFirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Last Name")]
    public string LEAPOCLastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Email")]
    [EmailAddress]
    public string LEAPOCEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Phone")]
    [Phone]
    public string LEAPOCPhone { get; set; } = string.Empty;
}

/// <summary>
/// Detailed view of a student with all their information
/// </summary>
public class StudentDetailViewModel
{
    // Student Information
    public int StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string SEID { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Last4SSN { get; set; } = string.Empty;

    // Demographics
    public string Race { get; set; } = string.Empty;
    public string Ethnicity { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;

    // Credential Information
    public string CredentialArea { get; set; } = string.Empty;
    public string CountyCDSCode { get; set; } = string.Empty;
    public string SchoolCDSCode { get; set; } = string.Empty;

    // Application Context
    public int ApplicationId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public string LEAName { get; set; } = string.Empty;
    public string GrantCycleName { get; set; } = string.Empty;

    // Status Information
    public string Status { get; set; } = string.Empty;
    public string GAAStatus { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }

    // Timeline
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Outcomes (if available)
    public int? GrantProgramHours { get; set; }
    public int? CredentialProgramHours { get; set; }
    public bool? CredentialEarned { get; set; }
    public DateTime? CredentialEarnedDate { get; set; }
    public bool? SwitchedToIntern { get; set; }
    public bool? EmployedInDistrict { get; set; }
    public bool? EmployedInState { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;

    // Reporting
    public string ReportingStatus { get; set; } = string.Empty;
    public IHEReport? LatestReport { get; set; }
}
