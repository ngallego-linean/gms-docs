using System.ComponentModel.DataAnnotations;

namespace Ctc.GMS.AspNetCore.ViewModels;

/// <summary>
/// View model for the Reporting Dashboard
/// </summary>
public class ReportingDashboardViewModel
{
    public ReportMetricsViewModel Metrics { get; set; } = new();
    public List<ReportDeadlineViewModel> UpcomingDeadlines { get; set; } = new();
    public string CurrentUser { get; set; } = string.Empty;
}

/// <summary>
/// View model for report metrics
/// </summary>
public class ReportMetricsViewModel
{
    [Display(Name = "Reports Submitted")]
    public int ReportsSubmitted { get; set; }

    [Display(Name = "Reports Outstanding")]
    public int ReportsOutstanding { get; set; }

    [Display(Name = "Reports Under Review")]
    public int ReportsUnderReview { get; set; }

    [Display(Name = "Reports Approved")]
    public int ReportsApproved { get; set; }

    [Display(Name = "Revisions Requested")]
    public int RevisionsRequested { get; set; }

    [Display(Name = "IHE Reports Submitted")]
    public int IHEReportsSubmitted { get; set; }

    [Display(Name = "LEA Reports Submitted")]
    public int LEAReportsSubmitted { get; set; }

    [Display(Name = "Next Reporting Deadline")]
    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime? NextReportingDeadline { get; set; }

    [Display(Name = "Compliance Rate")]
    [DisplayFormat(DataFormatString = "{0:F1}%")]
    public double ComplianceRate { get; set; }
}

/// <summary>
/// View model for report deadline information
/// </summary>
public class ReportDeadlineViewModel
{
    public int ApplicationId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public string IHEName { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime ReportingDeadline { get; set; }

    public int DaysUntilDue { get; set; }
    public bool IsOverdue { get; set; }
}

/// <summary>
/// View model for submitted reports list
/// </summary>
public class SubmittedReportsViewModel
{
    public List<ReportSummaryViewModel> IHEReports { get; set; } = new();
    public List<ReportSummaryViewModel> LEAReports { get; set; } = new();
    public string SelectedStatus { get; set; } = string.Empty;
    public string SelectedLEA { get; set; } = string.Empty;
    public string SelectedIHE { get; set; } = string.Empty;
    public string SortBy { get; set; } = "SubmissionDate";
    public string SortOrder { get; set; } = "desc";
}

/// <summary>
/// View model for individual report summary (list view)
/// </summary>
public class ReportSummaryViewModel
{
    public int Id { get; set; }
    public string ReportType { get; set; } = string.Empty; // IHE or LEA
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public string IHEName { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime SubmittedDate { get; set; }

    public string Status { get; set; } = string.Empty;
    public string StatusBadgeClass { get; set; } = string.Empty;
    public string ReviewedBy { get; set; } = string.Empty;
    public int RevisionCount { get; set; }
}

/// <summary>
/// View model for outstanding reports
/// </summary>
public class OutstandingReportsViewModel
{
    public List<OutstandingReportItemViewModel> OutstandingReports { get; set; } = new();
    public int TotalOutstanding { get; set; }
    public int CriticallyOverdue { get; set; }
}

/// <summary>
/// View model for individual outstanding report item
/// </summary>
public class OutstandingReportItemViewModel
{
    public int ApplicationId { get; set; }
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int IHEId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int CandidatesPendingReport { get; set; }

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime? PaymentDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime? ReportingDeadline { get; set; }

    public int DaysOverdue { get; set; }
    public bool CriticallyOverdue { get; set; }
}

/// <summary>
/// View model for individual report review page
/// </summary>
public class ReportReviewViewModel
{
    public string ReportType { get; set; } = string.Empty; // IHE or LEA
    public int ReportId { get; set; }

    // Student information
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;
    public string CredentialArea { get; set; } = string.Empty;

    // Application information
    public int ApplicationId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public string IHEName { get; set; } = string.Empty;

    // Report metadata
    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime SubmittedDate { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public string SubmittedByEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ReviewedBy { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime? ReviewedDate { get; set; }
    public int RevisionCount { get; set; }
    public string RevisionNotes { get; set; } = string.Empty;
    public string InternalNotes { get; set; } = string.Empty;

    // IHE Report specific fields
    public IHEReportDetailsViewModel? IHEDetails { get; set; }

    // LEA Report specific fields
    public LEAReportDetailsViewModel? LEADetails { get; set; }
}

/// <summary>
/// IHE Report details
/// </summary>
public class IHEReportDetailsViewModel
{
    public string CompletionStatus { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime? CompletionDate { get; set; }
    public string DenialReason { get; set; } = string.Empty;

    public bool SwitchedToIntern { get; set; }

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime? InternSwitchDate { get; set; }

    public int GrantProgramHours { get; set; }
    public bool Met500Hours { get; set; }
    public string GrantProgramHoursNotes { get; set; } = string.Empty;

    public int CredentialProgramHours { get; set; }
    public bool Met600Hours { get; set; }
    public string CredentialProgramHoursNotes { get; set; } = string.Empty;

    public string AdditionalNotes { get; set; } = string.Empty;
    public string DocumentationUrl { get; set; } = string.Empty;
}

/// <summary>
/// LEA Report details
/// </summary>
public class LEAReportDetailsViewModel
{
    public string PaymentCategory { get; set; } = string.Empty;
    public string PaymentSchedule { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal ActualPaymentAmount { get; set; }
    public string PaymentScheduleDetails { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime? FirstPaymentDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime? FinalPaymentDate { get; set; }
    public string PaymentNotes { get; set; } = string.Empty;

    public bool HiredInDistrict { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime? HireDate { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string SchoolSite { get; set; } = string.Empty;

    public string AdditionalNotes { get; set; } = string.Empty;
    public string DocumentationUrl { get; set; } = string.Empty;
}

/// <summary>
/// View model for analytics dashboard
/// </summary>
public class ReportAnalyticsViewModel
{
    public ReportAnalyticsSummaryViewModel Summary { get; set; } = new();
    public ReportAnalyticsFilterViewModel Filter { get; set; } = new();
}

/// <summary>
/// Analytics summary data
/// </summary>
public class ReportAnalyticsSummaryViewModel
{
    [Display(Name = "Total Candidates Funded")]
    public int TotalCandidatesFunded { get; set; }

    [Display(Name = "Total Candidates Reported")]
    public int TotalCandidatesReported { get; set; }

    [Display(Name = "Reporting Compliance Rate")]
    [DisplayFormat(DataFormatString = "{0:F1}%")]
    public double ReportingComplianceRate { get; set; }

    [Display(Name = "Program Completions")]
    public int ProgramCompletions { get; set; }

    [Display(Name = "Program Completion Rate")]
    [DisplayFormat(DataFormatString = "{0:F1}%")]
    public double ProgramCompletionRate { get; set; }

    [Display(Name = "Credentials Earned")]
    public int CredentialsEarned { get; set; }

    [Display(Name = "Credential Earn Rate")]
    [DisplayFormat(DataFormatString = "{0:F1}%")]
    public double CredentialEarnRate { get; set; }

    [Display(Name = "Candidates Employed")]
    public int CandidatesEmployed { get; set; }

    [Display(Name = "Employment Rate")]
    [DisplayFormat(DataFormatString = "{0:F1}%")]
    public double EmploymentRate { get; set; }

    [Display(Name = "Hired in District")]
    public int HiredInDistrict { get; set; }

    [Display(Name = "Hired in District Rate")]
    [DisplayFormat(DataFormatString = "{0:F1}%")]
    public double HiredInDistrictRate { get; set; }

    [Display(Name = "Average Grant Program Hours")]
    [DisplayFormat(DataFormatString = "{0:F0}")]
    public double AverageGrantProgramHours { get; set; }

    [Display(Name = "Average Credential Program Hours")]
    [DisplayFormat(DataFormatString = "{0:F0}")]
    public double AverageCredentialProgramHours { get; set; }

    [Display(Name = "Met 500 Hours")]
    public int Met500HoursCount { get; set; }

    [Display(Name = "Met 600 Hours")]
    public int Met600HoursCount { get; set; }

    [Display(Name = "Total Amount Disbursed")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal TotalAmountDisbursed { get; set; }

    [Display(Name = "Average Payment Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal AveragePaymentAmount { get; set; }

    public Dictionary<string, int> EmploymentByStatus { get; set; } = new();
}

/// <summary>
/// Filter for analytics
/// </summary>
public class ReportAnalyticsFilterViewModel
{
    [Display(Name = "Start Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime? EndDate { get; set; }

    [Display(Name = "LEA")]
    public int? LEAId { get; set; }

    [Display(Name = "IHE")]
    public int? IHEId { get; set; }

    [Display(Name = "Credential Type")]
    public string? CredentialType { get; set; }

    [Display(Name = "Status")]
    public string? Status { get; set; }
}

/// <summary>
/// View model for compliance dashboard
/// </summary>
public class ComplianceDashboardViewModel
{
    public ComplianceMetricsViewModel Metrics { get; set; } = new();
    public List<LEAComplianceItemViewModel> LEACompliance { get; set; } = new();
}

/// <summary>
/// Compliance metrics summary
/// </summary>
public class ComplianceMetricsViewModel
{
    [Display(Name = "Total LEAs")]
    public int TotalLEAs { get; set; }

    [Display(Name = "Full Compliance")]
    public int LEAsFullCompliance { get; set; }

    [Display(Name = "Partial Compliance")]
    public int LEAsPartialCompliance { get; set; }

    [Display(Name = "No Compliance")]
    public int LEAsNoCompliance { get; set; }
}

/// <summary>
/// Individual LEA compliance item
/// </summary>
public class LEAComplianceItemViewModel
{
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int ReportsRequired { get; set; }
    public int ReportsSubmitted { get; set; }

    [DisplayFormat(DataFormatString = "{0:F1}%")]
    public double CompliancePercentage { get; set; }
    public int CriticallyOverdue { get; set; }
    public string ComplianceStatus { get; set; } = string.Empty;
    public string StatusClass { get; set; } = string.Empty;
}
