using System.ComponentModel.DataAnnotations;

namespace Ctc.GMS.AspNetCore.ViewModels;

public class StudentGAAViewModel
{
    public int StudentId { get; set; }

    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Display(Name = "SEID")]
    public string SEID { get; set; } = string.Empty;

    [Display(Name = "IHE Institution")]
    public string IHEName { get; set; } = string.Empty;

    [Display(Name = "LEA District")]
    public string LEAName { get; set; } = string.Empty;

    [Display(Name = "Credential Area")]
    public string CredentialArea { get; set; } = string.Empty;

    [Display(Name = "Award Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal AwardAmount { get; set; }

    [Display(Name = "GAA Status")]
    public string GAAStatus { get; set; } = string.Empty;

    [Display(Name = "Approved Date")]
    public DateTime? ApprovedDate { get; set; }
}

public class GAAListViewModel
{
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public List<StudentGAAViewModel> Students { get; set; } = new();
}

public class PaymentViewModel
{
    public int Id { get; set; }

    [Display(Name = "Payment Date")]
    [DataType(DataType.Date)]
    public DateTime PaymentDate { get; set; }

    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Display(Name = "LEA District")]
    public string LEAName { get; set; } = string.Empty;

    [Display(Name = "Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal Amount { get; set; }

    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Display(Name = "Status")]
    public string Status { get; set; } = string.Empty;
}

public class PaymentListViewModel
{
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public List<PaymentViewModel> Payments { get; set; } = new();
}

public class PaymentWithReportsViewModel
{
    public int PaymentId { get; set; }

    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Display(Name = "SEID")]
    public string SEID { get; set; } = string.Empty;

    [Display(Name = "LEA District")]
    public string LEAName { get; set; } = string.Empty;

    [Display(Name = "Amount Paid")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal AmountPaid { get; set; }

    [Display(Name = "Payment Date")]
    [DataType(DataType.Date)]
    public DateTime? PaymentDate { get; set; }

    [Display(Name = "Payment Status")]
    public string PaymentStatus { get; set; } = string.Empty;

    // Reporting Status
    [Display(Name = "LEA Report Status")]
    public string LEAReportStatus { get; set; } = string.Empty;  // "Submitted", "Pending", "Overdue"

    [Display(Name = "IHE Report Status")]
    public string IHEReportStatus { get; set; } = string.Empty;  // "Submitted", "Pending", "Overdue"

    public bool HasOutstandingReports { get; set; }

    // Outcome Data (from IHE Report)
    [Display(Name = "Program Completion")]
    public string ProgramCompletion { get; set; } = string.Empty;  // "COMPLETED", "DENIED", "IN_PROGRESS"

    [Display(Name = "Credential Earned")]
    public bool CredentialEarned { get; set; }

    // Employment Data (from LEA Report)
    [Display(Name = "Employment Status")]
    public string EmploymentStatus { get; set; } = string.Empty;  // "FULL_TIME", "PART_TIME", "NOT_HIRED"

    [Display(Name = "Hired in District")]
    public bool HiredInDistrict { get; set; }
}

public class ReportingComplianceViewModel
{
    [Display(Name = "LEA Name")]
    public string LEAName { get; set; } = string.Empty;

    [Display(Name = "Total Payments")]
    public int TotalPayments { get; set; }

    [Display(Name = "Reports Submitted")]
    public int ReportsSubmitted { get; set; }

    [Display(Name = "Reports Pending")]
    public int ReportsPending { get; set; }

    [Display(Name = "Compliance Rate")]
    [DisplayFormat(DataFormatString = "{0:P0}")]
    public decimal ComplianceRate { get; set; }

    [Display(Name = "Compliance Status")]
    public string ComplianceStatus { get; set; } = string.Empty;  // "COMPLIANT", "WARNING", "NON_COMPLIANT"

    public bool HasPaymentHoldWarning { get; set; }

    [Display(Name = "Total Disbursed")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal TotalDisbursed { get; set; }
}

public class OutcomeMetricsViewModel
{
    [Display(Name = "Total Students Funded")]
    public int TotalStudentsFunded { get; set; }

    [Display(Name = "Program Completions")]
    public int ProgramCompletions { get; set; }

    [Display(Name = "Completion Rate")]
    [DisplayFormat(DataFormatString = "{0:P0}")]
    public decimal CompletionRate { get; set; }

    [Display(Name = "Credentials Earned")]
    public int CredentialsEarned { get; set; }

    [Display(Name = "Credential Rate")]
    [DisplayFormat(DataFormatString = "{0:P0}")]
    public decimal CredentialRate { get; set; }

    [Display(Name = "Teachers Employed")]
    public int TeachersEmployed { get; set; }

    [Display(Name = "Employment Rate")]
    [DisplayFormat(DataFormatString = "{0:P0}")]
    public decimal EmploymentRate { get; set; }

    [Display(Name = "Total Investment")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal TotalInvestment { get; set; }

    [Display(Name = "Cost Per Successful Teacher")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal CostPerSuccessfulTeacher { get; set; }
}

public class FiscalReportingDashboardViewModel
{
    [Display(Name = "Total Payments")]
    public int TotalPayments { get; set; }

    [Display(Name = "Payments with Reports")]
    public int PaymentsWithReports { get; set; }

    [Display(Name = "Outstanding Reports")]
    public int OutstandingReports { get; set; }

    [Display(Name = "Reporting Compliance Rate")]
    [DisplayFormat(DataFormatString = "{0:P0}")]
    public decimal ReportingComplianceRate { get; set; }

    [Display(Name = "Compliance Health")]
    public string ComplianceHealth { get; set; } = string.Empty;  // "GREEN", "YELLOW", "RED"

    public OutcomeMetricsViewModel OutcomeMetrics { get; set; } = new();
}
