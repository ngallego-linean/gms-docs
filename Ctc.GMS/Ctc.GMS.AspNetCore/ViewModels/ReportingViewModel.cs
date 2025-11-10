using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ctc.GMS.AspNetCore.ViewModels;

/// <summary>
/// ViewModel for the Funded Candidates list view with filtering
/// </summary>
public class FundedCandidatesViewModel
{
    public int IHEId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;

    public CandidateFilterCriteria Filters { get; set; } = new();
    public List<FundedCandidateSummaryViewModel> Candidates { get; set; } = new();
    public int TotalCount { get; set; }

    // Filter options for dropdowns
    public List<string> AvailableCohorts { get; set; } = new();
    public List<OrganizationOption> AvailableLEAs { get; set; } = new();
    public List<string> AvailableCredentialAreas { get; set; } = new();
}

/// <summary>
/// Filter criteria for funded candidates search
/// </summary>
public class CandidateFilterCriteria
{
    public string? CohortYear { get; set; }
    public int? LEAId { get; set; }
    public string? CredentialArea { get; set; }
    public string? PaymentStatus { get; set; }  // PENDING, GAA_SIGNED, PAYMENT_COMPLETED
    public string? ReportingStatus { get; set; }  // NOT_STARTED, IN_PROGRESS, SUBMITTED, APPROVED
}

/// <summary>
/// Summary view of a single funded candidate for the list
/// </summary>
public class FundedCandidateSummaryViewModel
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;
    public string LEAName { get; set; } = string.Empty;
    public string CredentialArea { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string ReportingStatus { get; set; } = string.Empty;
    public DateTime? LastReportDate { get; set; }
    public bool IsReportOverdue { get; set; }
    public bool CanReport { get; set; }  // True if payment completed and reporting period active
}

/// <summary>
/// ViewModel for individual candidate reporting form
/// </summary>
public class CandidateReportingViewModel
{
    public int StudentId { get; set; }
    public StudentSummaryViewModel Student { get; set; } = new();
    public ApplicationSummaryViewModel Application { get; set; } = new();
    public IHEReportFormViewModel ReportForm { get; set; } = new();
    public ReportingPeriodViewModel? CurrentPeriod { get; set; }
    public bool CanEdit { get; set; }  // False if report already submitted
    public bool HasExistingReport { get; set; }
}

/// <summary>
/// Student summary for display on reporting form
/// </summary>
public class StudentSummaryViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;
    public string CredentialArea { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

/// <summary>
/// Reporting period information
/// </summary>
public class ReportingPeriodViewModel
{
    public int Id { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsActive { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DaysUntilDue { get; set; }
}

/// <summary>
/// Form data for IHE Report submission
/// </summary>
public class IHEReportFormViewModel
{
    public int? ReportId { get; set; }
    public int StudentId { get; set; }
    public int ApplicationId { get; set; }
    public int? ReportingPeriodId { get; set; }

    // Program Completion Section
    [Required(ErrorMessage = "Completion status is required")]
    public string CompletionStatus { get; set; } = "IN_PROGRESS";  // COMPLETED, DENIED, IN_PROGRESS

    [DataType(DataType.Date)]
    public DateTime? CompletionDate { get; set; }

    [MaxLength(500)]
    public string DenialReason { get; set; } = string.Empty;

    // Intern Status Section
    public bool SwitchedToIntern { get; set; }

    [DataType(DataType.Date)]
    public DateTime? InternSwitchDate { get; set; }

    // Hours Tracking Section
    [Range(0, 1000, ErrorMessage = "Hours must be between 0 and 1000")]
    public int GrantProgramHours { get; set; }

    public bool Met500Hours { get; set; }

    [MaxLength(500)]
    public string GrantProgramHoursNotes { get; set; } = string.Empty;

    [Range(0, 1000, ErrorMessage = "Hours must be between 0 and 1000")]
    public int CredentialProgramHours { get; set; }

    public bool Met600Hours { get; set; }

    [MaxLength(500)]
    public string CredentialProgramHoursNotes { get; set; } = string.Empty;

    // Credential Earned Section
    public bool CredentialEarned { get; set; }

    [DataType(DataType.Date)]
    public DateTime? CredentialEarnedDate { get; set; }

    [MaxLength(200)]
    public string CredentialType { get; set; } = string.Empty;

    // Employment Section
    public bool EmployedInDistrict { get; set; }
    public bool EmployedInState { get; set; }

    [Required(ErrorMessage = "Employment status is required")]
    public string EmploymentStatus { get; set; } = "NOT_EMPLOYED";  // EMPLOYED, NOT_EMPLOYED, SEEKING

    [MaxLength(200)]
    public string EmployerName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime? EmploymentStartDate { get; set; }

    [MaxLength(200)]
    public string SchoolSite { get; set; } = string.Empty;

    [MaxLength(100)]
    public string GradeLevel { get; set; } = string.Empty;

    [MaxLength(200)]
    public string SubjectArea { get; set; } = string.Empty;

    // Additional Information
    [MaxLength(2000)]
    public string AdditionalNotes { get; set; } = string.Empty;

    public IFormFile? DocumentUpload { get; set; }
    public string DocumentationUrl { get; set; } = string.Empty;

    // Metadata
    public string Status { get; set; } = "DRAFT";
    public DateTime? LastModified { get; set; }
}

/// <summary>
/// ViewModel for bulk upload page
/// </summary>
public class BulkUploadViewModel
{
    public int IHEId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public int ReportingPeriodId { get; set; }
    public ReportingPeriodViewModel? ReportingPeriod { get; set; }

    public IFormFile? UploadedFile { get; set; }

    // Results after validation
    public BulkUploadResultViewModel? UploadResult { get; set; }
}

/// <summary>
/// Results from bulk upload validation
/// </summary>
public class BulkUploadResultViewModel
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<BulkUploadError> Errors { get; set; } = new();
    public List<IHEReportFormViewModel> ValidReports { get; set; } = new();
    public bool HasErrors => ErrorCount > 0;
}

/// <summary>
/// Error details from bulk upload
/// </summary>
public class BulkUploadError
{
    public int RowNumber { get; set; }
    public string Field { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for report submission review
/// </summary>
public class ReportSubmissionViewModel
{
    public int IHEId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public int ReportingPeriodId { get; set; }
    public ReportingPeriodViewModel? ReportingPeriod { get; set; }

    public List<ReportSubmissionItemViewModel> ReportsToSubmit { get; set; } = new();
    public int TotalReports { get; set; }

    [Required(ErrorMessage = "Submitter name is required")]
    public string SubmitterName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string SubmitterEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "You must confirm accuracy")]
    public bool ConfirmAccuracy { get; set; }

    [Required(ErrorMessage = "You must acknowledge the deadline")]
    public bool ConfirmDeadline { get; set; }
}

/// <summary>
/// Individual report in submission batch
/// </summary>
public class ReportSubmissionItemViewModel
{
    public int ReportId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;
    public string CompletionStatus { get; set; } = string.Empty;
    public bool CredentialEarned { get; set; }
    public bool Employed { get; set; }
}

/// <summary>
/// Confirmation after successful submission
/// </summary>
public class ReportSubmissionConfirmationViewModel
{
    public string ConfirmationNumber { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
    public int ReportsSubmitted { get; set; }
    public string SubmitterName { get; set; } = string.Empty;
    public string SubmitterEmail { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for historical reports view
/// </summary>
public class ReportHistoryViewModel
{
    public int IHEId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }

    public ReportHistoryFilterCriteria Filters { get; set; } = new();
    public List<SubmittedReportViewModel> SubmittedReports { get; set; } = new();
    public int TotalCount { get; set; }

    // Filter options
    public List<ReportingPeriodViewModel> AvailablePeriods { get; set; } = new();
    public List<OrganizationOption> AvailableLEAs { get; set; } = new();
}

/// <summary>
/// Filter criteria for report history
/// </summary>
public class ReportHistoryFilterCriteria
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ReportingPeriodId { get; set; }
    public int? LEAId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// Summary of a submitted report
/// </summary>
public class SubmittedReportViewModel
{
    public int ReportId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;
    public string LEAName { get; set; } = string.Empty;
    public DateTime? SubmittedDate { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CompletionStatus { get; set; } = string.Empty;
    public bool CredentialEarned { get; set; }
    public bool EmployedInDistrict { get; set; }
    public string ConfirmationNumber { get; set; } = string.Empty;
}

/// <summary>
/// Detailed view of a single report
/// </summary>
public class ReportDetailViewModel
{
    public int ReportId { get; set; }
    public StudentSummaryViewModel Student { get; set; } = new();
    public ApplicationSummaryViewModel Application { get; set; } = new();
    public ReportingPeriodViewModel ReportingPeriod { get; set; } = new();
    public IHEReportFormViewModel ReportData { get; set; } = new();

    public string Status { get; set; } = string.Empty;
    public DateTime? SubmittedDate { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime? ApprovedDate { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
    public string ConfirmationNumber { get; set; } = string.Empty;
}

/// <summary>
/// Organization option for dropdowns
/// </summary>
public class OrganizationOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
