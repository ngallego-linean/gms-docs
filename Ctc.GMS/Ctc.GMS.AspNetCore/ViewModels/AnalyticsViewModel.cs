using System.ComponentModel.DataAnnotations;

namespace Ctc.GMS.AspNetCore.ViewModels;

/// <summary>
/// Comprehensive analytics view model for CTC Staff
/// Contains all detailed metrics and visualizations moved from the Dashboard
/// </summary>
public class AnalyticsViewModel
{
    [Display(Name = "Grant Cycle")]
    public int GrantCycleId { get; set; }

    public string GrantCycleName { get; set; } = string.Empty;

    // Budget Overview (Detailed)
    public BudgetOverviewViewModel BudgetOverview { get; set; } = new();

    // Grant Submissions (Student Statistics)
    public GrantSubmissionsViewModel GrantSubmissions { get; set; } = new();

    // Program Outcomes
    public ProgramOutcomesViewModel ProgramOutcomes { get; set; } = new();

    // Grantee Reporting Metrics
    public GranteeReportingMetricsViewModel GranteeReportingMetrics { get; set; } = new();

    // Financial Summary
    public FinancialSummaryViewModel FinancialSummary { get; set; } = new();

    // Partnership Statistics
    public PartnershipStatisticsViewModel PartnershipStatistics { get; set; } = new();
}

/// <summary>
/// Detailed budget overview with all financial metrics
/// </summary>
public class BudgetOverviewViewModel
{
    [Display(Name = "Appropriated Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal ApproprietedAmount { get; set; }

    [Display(Name = "Reserved Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal ReservedAmount { get; set; }

    [Display(Name = "Encumbered Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal EncumberedAmount { get; set; }

    [Display(Name = "Disbursed Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal DisbursedAmount { get; set; }

    [Display(Name = "Remaining Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal RemainingAmount { get; set; }

    [Display(Name = "Outstanding Balance")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal OutstandingBalance { get; set; }

    public decimal RemainingPercent { get; set; }

    public decimal EncumberedPercent { get; set; }

    public decimal DisbursedPercent { get; set; }
}

/// <summary>
/// Grant submissions and student status breakdown
/// </summary>
public class GrantSubmissionsViewModel
{
    [Display(Name = "Total Students")]
    public int TotalStudents { get; set; }

    [Display(Name = "Students Approved")]
    public int StudentsApproved { get; set; }

    [Display(Name = "Students Under Review")]
    public int StudentsUnderReview { get; set; }

    [Display(Name = "Students Pending LEA")]
    public int StudentsPendingLEA { get; set; }

    [Display(Name = "Students Drafted")]
    public int StudentsDraft { get; set; }

    [Display(Name = "Students Rejected")]
    public int StudentsRejected { get; set; }

    public StatusCountsViewModel StatusCounts { get; set; } = new();
}

/// <summary>
/// Program outcomes and completion metrics
/// </summary>
public class ProgramOutcomesViewModel
{
    [Display(Name = "Total Hours Completed")]
    public int TotalHoursCompleted { get; set; }

    [Display(Name = "Average Hours Per Student")]
    public decimal AverageHoursPerStudent { get; set; }

    [Display(Name = "Completion Rate")]
    [DisplayFormat(DataFormatString = "{0:P0}")]
    public decimal CompletionRate { get; set; }

    [Display(Name = "Students Completing Program")]
    public int StudentsCompleting { get; set; }

    [Display(Name = "Credential Areas Served")]
    public int CredentialAreasServed { get; set; }

    public List<CredentialAreaBreakdown> CredentialBreakdown { get; set; } = new();
}

public class CredentialAreaBreakdown
{
    public string CredentialArea { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// Grantee (LEA) reporting metrics
/// </summary>
public class GranteeReportingMetricsViewModel
{
    [Display(Name = "Total Reports Required")]
    public int TotalReportsRequired { get; set; }

    [Display(Name = "Reports Submitted")]
    public int ReportsSubmitted { get; set; }

    [Display(Name = "Reports Outstanding")]
    public int ReportsOutstanding { get; set; }

    [Display(Name = "Reports Under Review")]
    public int ReportsUnderReview { get; set; }

    [Display(Name = "Reports Approved")]
    public int ReportsApproved { get; set; }

    [Display(Name = "IHE Reports Submitted")]
    public int IHEReportsSubmitted { get; set; }

    [Display(Name = "LEA Reports Submitted")]
    public int LEAReportsSubmitted { get; set; }

    [Display(Name = "Submission Rate")]
    [DisplayFormat(DataFormatString = "{0:P0}")]
    public decimal SubmissionRate { get; set; }
}

/// <summary>
/// Financial summary and payment tracking
/// </summary>
public class FinancialSummaryViewModel
{
    [Display(Name = "Total Disbursements")]
    public int TotalDisbursements { get; set; }

    [Display(Name = "Pending GAAs")]
    public int PendingGAAs { get; set; }

    [Display(Name = "Active GAAs")]
    public int ActiveGAAs { get; set; }

    [Display(Name = "Completed Payments")]
    public int CompletedPayments { get; set; }

    [Display(Name = "Total Amount Paid")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal TotalAmountPaid { get; set; }

    [Display(Name = "Average Payment Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal AveragePaymentAmount { get; set; }
}

/// <summary>
/// Partnership and organization statistics
/// </summary>
public class PartnershipStatisticsViewModel
{
    [Display(Name = "Total IHEs")]
    public int TotalIHEs { get; set; }

    [Display(Name = "Total LEAs")]
    public int TotalLEAs { get; set; }

    [Display(Name = "Active Partnerships")]
    public int ActivePartnerships { get; set; }

    [Display(Name = "IHEs with Submissions")]
    public int IHEsWithSubmissions { get; set; }

    [Display(Name = "LEAs with Approved Students")]
    public int LEAsWithApprovedStudents { get; set; }
}
