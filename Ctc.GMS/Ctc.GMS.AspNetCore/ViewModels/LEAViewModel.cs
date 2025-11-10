using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ctc.GMS.AspNetCore.ViewModels;

public class LEADashboardViewModel
{
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public int TotalApplications { get; set; }
    public int TotalStudents { get; set; }
    public int PendingCompletionCount { get; set; }
    public int SubmittedCount { get; set; }
    public int ApprovedCount { get; set; }
    public List<ApplicationSummaryViewModel> Applications { get; set; } = new();
    public List<ActionItemViewModel> ActionItems { get; set; } = new();

    // Reporting Metrics
    public int TotalFundedStudents { get; set; }
    public int ReportsSubmitted { get; set; }
    public int ReportsPending { get; set; }
    public int ReportsOverdue { get; set; }
    public DateTime? ReportingDeadline { get; set; }
    public bool HasReportsDue => ReportsPending > 0 || ReportsOverdue > 0;
}

public class CompleteApplicationViewModel
{
    public int ApplicationId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }

    [Required]
    [Display(Name = "LEA POC First Name")]
    public string POCFirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Last Name")]
    public string POCLastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Email")]
    [EmailAddress]
    public string POCEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Phone")]
    [Phone]
    public string POCPhone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "GAA Signer Name")]
    public string GAASignerName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "GAA Signer Title")]
    public string GAASignerTitle { get; set; } = string.Empty;

    [Required]
    [Display(Name = "GAA Signer Email")]
    [EmailAddress]
    public string GAASignerEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Fiscal Agent Name")]
    public string FiscalAgentName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Fiscal Agent Email")]
    [EmailAddress]
    public string FiscalAgentEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Fiscal Agent Phone")]
    [Phone]
    public string FiscalAgentPhone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Superintendent Name")]
    public string SuperintendentName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Superintendent Email")]
    [EmailAddress]
    public string SuperintendentEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Intent")]
    public string PaymentIntent { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Schedule")]
    public string PaymentSchedule { get; set; } = string.Empty;

    public List<StudentViewModel> Students { get; set; } = new();
}

/// <summary>
/// Main reporting dashboard for LEA
/// </summary>
public class LEAReportsViewModel
{
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;

    // Metrics
    public int TotalFundedStudents { get; set; }
    public int ReportsSubmitted { get; set; }
    public int ReportsPending { get; set; }
    public int ReportsOverdue { get; set; }
    public DateTime? ReportingDeadline { get; set; }
    public bool IsReportingPeriodOpen { get; set; }

    // Collections
    public List<ReportDeadlineViewModel> UpcomingDeadlines { get; set; } = new();
    public List<StudentReportSummaryViewModel> RecentReports { get; set; } = new();
}

/// <summary>
/// Funded candidates list view with filters
/// </summary>
public class FundedCandidatesViewModel
{
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;

    // Filters
    public ReportSearchCriteria SearchCriteria { get; set; } = new();

    // Results
    public List<StudentReportSummaryViewModel> Students { get; set; } = new();
    public int TotalCount { get; set; }
    public int FilteredCount => Students.Count;

    // Filter options
    public List<string> IHEPartners { get; set; } = new();
    public List<string> CredentialTypes { get; set; } = new();
    public List<string> Cohorts { get; set; } = new();
}

/// <summary>
/// Individual candidate reporting form
/// </summary>
public class LEAReportSubmissionViewModel
{
    public int StudentId { get; set; }
    public int ApplicationId { get; set; }
    public int LEAId { get; set; }
    public int GrantCycleId { get; set; }
    public int? ReportId { get; set; }  // Null if new report, has value if editing

    // Student Context (Read-only, for display)
    public string StudentName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;
    public string CredentialArea { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public DateTime? ApprovedDate { get; set; }

    // Payment Information
    [Required]
    [Display(Name = "Payment Category")]
    public string PaymentCategory { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Schedule")]
    public string PaymentSchedule { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Actual Payment Amount")]
    [Range(0, 100000)]
    public decimal ActualPaymentAmount { get; set; }

    [Display(Name = "First Payment Date")]
    public DateTime? FirstPaymentDate { get; set; }

    [Display(Name = "Final Payment Date")]
    public DateTime? FinalPaymentDate { get; set; }

    // Program Completion
    [Required]
    [Display(Name = "Program Completion Status")]
    public string ProgramCompletionStatus { get; set; } = string.Empty;

    [Display(Name = "Program Completion Date")]
    public DateTime? ProgramCompletionDate { get; set; }

    // Credential Information
    [Required]
    [Display(Name = "Credential Status")]
    public string CredentialEarnedStatus { get; set; } = string.Empty;

    [Display(Name = "Credential Issue Date")]
    public DateTime? CredentialIssueDate { get; set; }

    // Employment Information
    [Required]
    [Display(Name = "Hired in Your District?")]
    public bool HiredInDistrict { get; set; }

    [Display(Name = "Employment Status")]
    public string EmploymentStatus { get; set; } = string.Empty;

    [Display(Name = "Employment Start Date")]
    public DateTime? EmploymentStartDate { get; set; }

    [Display(Name = "Employing LEA/District")]
    public string EmployingLEA { get; set; } = string.Empty;

    [Display(Name = "School Site")]
    public string SchoolSite { get; set; } = string.Empty;

    // Teaching Assignment
    [Display(Name = "Grade Level")]
    public string GradeLevel { get; set; } = string.Empty;

    [Display(Name = "Subject Area")]
    public string SubjectArea { get; set; } = string.Empty;

    [Display(Name = "Job Title")]
    public string JobTitle { get; set; } = string.Empty;

    // Quality Metrics
    [Display(Name = "Placement Quality Rating (1-5)")]
    [Range(1, 5)]
    public int? PlacementQualityRating { get; set; }

    [Display(Name = "Placement Quality Notes")]
    [DataType(DataType.MultilineText)]
    public string PlacementQualityNotes { get; set; } = string.Empty;

    [Display(Name = "Mentor Teacher Name")]
    public string MentorTeacherName { get; set; } = string.Empty;

    [Display(Name = "Mentor Teacher Feedback")]
    [DataType(DataType.MultilineText)]
    public string MentorTeacherFeedback { get; set; } = string.Empty;

    // Additional Information
    [Display(Name = "Additional Notes")]
    [DataType(DataType.MultilineText)]
    public string AdditionalNotes { get; set; } = string.Empty;

    // Report Status
    public string ReportStatus { get; set; } = "DRAFT";
    public bool IsLocked { get; set; }
    public string? CTCFeedback { get; set; }  // Shown if revision requested
}

/// <summary>
/// Summary info for a student requiring/having a report
/// </summary>
public class StudentReportSummaryViewModel
{
    public int StudentId { get; set; }
    public int ApplicationId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;
    public string CredentialArea { get; set; } = string.Empty;
    public string IHEName { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string Cohort { get; set; } = string.Empty;  // e.g., "October 2024"

    // Report Status
    public bool HasReport { get; set; }
    public string ReportStatus { get; set; } = string.Empty;
    public DateTime? ReportSubmittedDate { get; set; }
    public bool IsOverdue { get; set; }
    public string ReportStatusBadgeClass => ReportStatus switch
    {
        "APPROVED" => "badge-success",
        "SUBMITTED" => "badge-primary",
        "REVISION_REQUESTED" => "badge-warning",
        "DRAFT" => "badge-secondary",
        _ => "badge-secondary"
    };
}

/// <summary>
/// Bulk upload interface
/// </summary>
public class BulkUploadViewModel
{
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;

    [Display(Name = "Upload File")]
    public IFormFile? UploadFile { get; set; }

    // Results after processing
    public List<BulkUploadResultViewModel> Results { get; set; } = new();
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public bool HasErrors => ErrorCount > 0;
    public bool ProcessingComplete { get; set; }
}

/// <summary>
/// Result of processing a single row in bulk upload
/// </summary>
public class BulkUploadResultViewModel
{
    public int RowNumber { get; set; }
    public string SEID { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public string ErrorSummary => string.Join("; ", Errors);
}

/// <summary>
/// Historical reports view
/// </summary>
public class ReportHistoryViewModel
{
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;

    // Filters
    public ReportHistorySearchCriteria SearchCriteria { get; set; } = new();

    // Results
    public List<StudentReportSummaryViewModel> Reports { get; set; } = new();
    public int TotalCount { get; set; }

    // Filter options
    public List<string> IHEPartners { get; set; } = new();
    public List<string> StatusOptions { get; set; } = new() { "SUBMITTED", "APPROVED", "REVISION_REQUESTED" };
    public List<int> SubmissionYears { get; set; } = new();
}

/// <summary>
/// Search/filter criteria for funded candidates
/// </summary>
public class ReportSearchCriteria
{
    public string? IHEPartner { get; set; }
    public string? CredentialType { get; set; }
    public string? Cohort { get; set; }
    public string? ReportStatus { get; set; }
    public bool? ShowOnlyOverdue { get; set; }
}

/// <summary>
/// Search criteria for historical reports
/// </summary>
public class ReportHistorySearchCriteria
{
    public string? IHEPartner { get; set; }
    public string? Status { get; set; }
    public int? SubmissionYear { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// Upcoming deadline information
/// </summary>
public class ReportDeadlineViewModel
{
    public string Description { get; set; } = string.Empty;
    public DateTime DeadlineDate { get; set; }
    public int DaysRemaining { get; set; }
    public int StudentsAffected { get; set; }
    public string UrgencyLevel => DaysRemaining switch
    {
        <= 0 => "overdue",
        <= 3 => "critical",
        <= 7 => "warning",
        _ => "normal"
    };
    public string BadgeClass => UrgencyLevel switch
    {
        "overdue" => "badge-danger",
        "critical" => "badge-danger",
        "warning" => "badge-warning",
        _ => "badge-info"
    };
}
