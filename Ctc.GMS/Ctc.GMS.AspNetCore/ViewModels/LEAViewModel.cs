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

    // Action Items (Enhanced)
    public int CandidatesReadyForApplication { get; set; }
    public int ApplicationsWithErrors { get; set; }
    public int CurrentMonthDay { get; set; }
    public int DaysRemainingInMonth { get; set; }
    public string CurrentBatchMonth { get; set; } = string.Empty;

    // Draft Applications
    public int DraftApplicationCount { get; set; }
    public List<DraftApplicationSummaryViewModel> DraftApplications { get; set; } = new();

    // IHE Submissions (replaces Applications in some views)
    public List<IHESubmissionViewModel> IHESubmissions { get; set; } = new();

    // LEA Batch Submissions to CTC (monthly batches)
    public List<LEABatchSubmissionViewModel> BatchSubmissions { get; set; } = new();

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
/// Bulk upload interface for LEA portal
/// </summary>
public class LEABulkUploadViewModel
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
/// IHE submission grouped by partner for LEA dashboard
/// </summary>
public class IHESubmissionViewModel
{
    public int ApplicationId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
    public List<CandidateSummaryViewModel> Candidates { get; set; } = new();
    public int TotalCandidates { get; set; }
    public string Status { get; set; } = string.Empty; // DRAFT, PENDING_COMPLETION, SUBMITTED, APPROVED
    public DateTime LastModified { get; set; }
    public bool HasErrors { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}

/// <summary>
/// Summary of a candidate/student for LEA view
/// </summary>
public class CandidateSummaryViewModel
{
    public int StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string CredentialArea { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool NeedsCompletion { get; set; }
    public string? SEID { get; set; }
}

/// <summary>
/// Draft application summary for LEA dashboard
/// </summary>
public class DraftApplicationSummaryViewModel
{
    public int ApplicationId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int CandidateCount { get; set; }
    public DateTime LastSaved { get; set; }
    public int DaysRemainingInMonth { get; set; }
    public string FormattedLastSaved => LastSaved.ToString("MMM dd, yyyy h:mm tt");
}

/// <summary>
/// LEA batch submission to CTC (monthly batches containing students from multiple IHEs)
/// </summary>
public class LEABatchSubmissionViewModel
{
    public string BatchMonth { get; set; } = string.Empty; // "October 2025"
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public int TotalCandidates { get; set; }
    public int IHEPartnerCount { get; set; }
    public List<string> IHEPartnerNames { get; set; } = new();
    public string Status { get; set; } = string.Empty; // "DRAFT", "SUBMITTED", "APPROVED"
    public DateTime LastModified { get; set; }
    public List<int> ApplicationIds { get; set; } = new(); // IDs of applications included in this batch
}

/// <summary>
/// Detailed view of a monthly LEA batch submission (aggregated from multiple applications)
/// </summary>
public class LEABatchDetailsViewModel
{
    // Basic Information
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public string BatchMonth { get; set; } = string.Empty; // "October 2025"
    public int Year { get; set; }
    public int Month { get; set; }

    // Aggregated Metrics
    public int TotalCandidates { get; set; }
    public int IHEPartnerCount { get; set; }
    public List<string> IHEPartnerNames { get; set; } = new();
    public decimal TotalAwardAmount { get; set; }

    // Status and Timeline
    public string Status { get; set; } = string.Empty; // "DRAFT", "IN_PROGRESS", "SUBMITTED", "APPROVED"
    public DateTime? SubmissionDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModified { get; set; }

    // Students (all students from all applications in this batch)
    public List<StudentViewModel> Students { get; set; } = new();

    // Source Applications
    public List<int> ApplicationIds { get; set; } = new();
    public int ApplicationCount { get; set; }

    // Timeline Events
    public List<BatchTimelineEvent> TimelineEvents { get; set; } = new();
}

/// <summary>
/// Timeline event for batch history
/// </summary>
public class BatchTimelineEvent
{
    public DateTime EventDate { get; set; }
    public string EventType { get; set; } = string.Empty; // "CREATED", "STUDENT_ADDED", "SUBMITTED", "APPROVED"
    public string Description { get; set; } = string.Empty;
    public string? Actor { get; set; }
    public string? IHESource { get; set; } // For student additions
}

/// <summary>
/// View model for reviewing and batching candidates from multiple IHE sources
/// </summary>
public class ReviewCandidatesViewModel
{
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;

    // Batch context
    public string CurrentBatchMonth { get; set; } = string.Empty;
    public int CurrentMonthDay { get; set; }
    public int DaysInMonth { get; set; }
    public int DaysRemainingInMonth { get; set; }

    // Candidates awaiting review
    public List<CandidateForReviewViewModel> Candidates { get; set; } = new();
    public int TotalCandidates { get; set; }
    public int IHESourceCount { get; set; }

    // Existing draft applications to add candidates to
    public List<DraftApplicationOptionViewModel> DraftApplications { get; set; } = new();

    // Application Information (POC/Contact fields - displayed locked by default)
    public string POCFirstName { get; set; } = string.Empty;
    public string POCLastName { get; set; } = string.Empty;
    public string POCEmail { get; set; } = string.Empty;
    public string POCPhone { get; set; } = string.Empty;

    public string GAASignerName { get; set; } = string.Empty;
    public string GAASignerTitle { get; set; } = string.Empty;
    public string GAASignerEmail { get; set; } = string.Empty;

    public string FiscalAgentName { get; set; } = string.Empty;
    public string FiscalAgentEmail { get; set; } = string.Empty;
    public string FiscalAgentPhone { get; set; } = string.Empty;

    public string SuperintendentName { get; set; } = string.Empty;
    public string SuperintendentEmail { get; set; } = string.Empty;

    public string PaymentIntent { get; set; } = string.Empty;
    public string PaymentSchedule { get; set; } = string.Empty;
}

/// <summary>
/// Candidate awaiting LEA review
/// </summary>
public class CandidateForReviewViewModel
{
    public int StudentId { get; set; }
    public int ApplicationId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string SEID { get; set; } = string.Empty;
    public string CredentialArea { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public DateTime SubmittedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsSelected { get; set; } = true; // Default to selected
}

/// <summary>
/// Draft application option for adding candidates
/// </summary>
public class DraftApplicationOptionViewModel
{
    public int ApplicationId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int CandidateCount { get; set; }
    public DateTime LastModified { get; set; }
    public string DisplayText => $"{IHEName} ({CandidateCount} candidates) - Last modified {LastModified:MMM dd, yyyy}";
}

/// <summary>
/// Form submission for processing reviewed candidates
/// </summary>
public class ProcessCandidatesViewModel
{
    public int LEAId { get; set; }
    public int GrantCycleId { get; set; }
    public List<int> SelectedStudentIds { get; set; } = new();
    public string Action { get; set; } = string.Empty; // "create_new" or "add_to_existing"
    public int? ExistingApplicationId { get; set; } // For adding to existing application
}
