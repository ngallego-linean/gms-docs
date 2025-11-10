namespace GMS.Business.DTOs;

public class PaymentWithReportsDTO
{
    public int PaymentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;
    public string LEAName { get; set; } = string.Empty;
    public decimal AmountPaid { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string LEAReportStatus { get; set; } = string.Empty;
    public string IHEReportStatus { get; set; } = string.Empty;
    public bool HasOutstandingReports { get; set; }
    public string ProgramCompletion { get; set; } = string.Empty;
    public bool CredentialEarned { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;
    public bool HiredInDistrict { get; set; }
}

public class ReportingComplianceDTO
{
    public string LEAName { get; set; } = string.Empty;
    public int TotalPayments { get; set; }
    public int ReportsSubmitted { get; set; }
    public int ReportsPending { get; set; }
    public decimal ComplianceRate { get; set; }
    public string ComplianceStatus { get; set; } = string.Empty;
    public bool HasPaymentHoldWarning { get; set; }
    public decimal TotalDisbursed { get; set; }
}

public class OutcomeMetricsDTO
{
    public int TotalStudentsFunded { get; set; }
    public int ProgramCompletions { get; set; }
    public decimal CompletionRate { get; set; }
    public int CredentialsEarned { get; set; }
    public decimal CredentialRate { get; set; }
    public int TeachersEmployed { get; set; }
    public decimal EmploymentRate { get; set; }
    public decimal TotalInvestment { get; set; }
    public decimal CostPerSuccessfulTeacher { get; set; }
}

public class ReportingDashboardDTO
{
    public int TotalPayments { get; set; }
    public int PaymentsWithReports { get; set; }
    public int OutstandingReports { get; set; }
    public decimal ReportingComplianceRate { get; set; }
    public string ComplianceHealth { get; set; } = string.Empty;
    public OutcomeMetricsDTO OutcomeMetrics { get; set; } = new();
}
