using GMS.DomainModel;

namespace GMS.Business.Services;

/// <summary>
/// Report Service interface for business operations related to IHE and LEA reporting
/// </summary>
public interface IReportService
{
    // IHE Report Operations
    IHEReport? GetIHEReport(int id);
    List<IHEReport> GetIHEReports();
    List<IHEReport> GetIHEReportsByStatus(string status);
    List<IHEReport> GetIHEReportsByStudent(int studentId);
    List<IHEReport> GetIHEReportsByApplication(int applicationId);

    // LEA Report Operations
    LEAReport? GetLEAReport(int id);
    List<LEAReport> GetLEAReports();
    List<LEAReport> GetLEAReportsByStatus(string status);
    List<LEAReport> GetLEAReportsByStudent(int studentId);
    List<LEAReport> GetLEAReportsByApplication(int applicationId);

    // Report Workflow Operations
    void ApproveIHEReport(int id, string reviewerName);
    void ApproveLEAReport(int id, string reviewerName);
    void RequestIHEReportRevisions(int id, string reviewerName, string revisionNotes);
    void RequestLEAReportRevisions(int id, string reviewerName, string revisionNotes);
    void SetReportUnderReview(int id, string reportType, string reviewerName);

    // Report Analytics and Metrics
    ReportMetrics GetReportMetrics();
    ReportComplianceMetrics GetComplianceMetrics();
    ReportAnalytics GetReportAnalytics(ReportAnalyticsFilter? filter = null);

    // Outstanding Reports Tracking
    List<OutstandingReportInfo> GetOutstandingReports();
    List<ReportDeadlineInfo> GetReportDeadlines();
}

/// <summary>
/// Metrics for the reporting dashboard
/// </summary>
public class ReportMetrics
{
    public int ReportsSubmitted { get; set; }
    public int ReportsOutstanding { get; set; }
    public int ReportsUnderReview { get; set; }
    public int ReportsApproved { get; set; }
    public int RevisionsRequested { get; set; }
    public int IHEReportsSubmitted { get; set; }
    public int LEAReportsSubmitted { get; set; }
    public DateTime? NextReportingDeadline { get; set; }
    public double ComplianceRate { get; set; }
}

/// <summary>
/// Compliance tracking metrics
/// </summary>
public class ReportComplianceMetrics
{
    public int TotalLEAs { get; set; }
    public int LEAsFullCompliance { get; set; }
    public int LEAsPartialCompliance { get; set; }
    public int LEAsNoCompliance { get; set; }
    public List<LEAComplianceInfo> LEACompliance { get; set; } = new();
}

/// <summary>
/// Individual LEA compliance information
/// </summary>
public class LEAComplianceInfo
{
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int ReportsRequired { get; set; }
    public int ReportsSubmitted { get; set; }
    public double CompliancePercentage { get; set; }
    public int CriticallyOverdue { get; set; }
}

/// <summary>
/// Outstanding report information
/// </summary>
public class OutstandingReportInfo
{
    public int ApplicationId { get; set; }
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int IHEId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int CandidatesPendingReport { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? ReportingDeadline { get; set; }
    public int DaysOverdue { get; set; }
    public bool CriticallyOverdue { get; set; }
}

/// <summary>
/// Report deadline information
/// </summary>
public class ReportDeadlineInfo
{
    public int ApplicationId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public string IHEName { get; set; } = string.Empty;
    public DateTime ReportingDeadline { get; set; }
    public int DaysUntilDue { get; set; }
    public bool IsOverdue { get; set; }
}

/// <summary>
/// Analytics data for aggregate reporting
/// </summary>
public class ReportAnalytics
{
    public int TotalCandidatesFunded { get; set; }
    public int TotalCandidatesReported { get; set; }
    public double ReportingComplianceRate { get; set; }

    // Program Outcomes
    public int ProgramCompletions { get; set; }
    public double ProgramCompletionRate { get; set; }
    public int CredentialsEarned { get; set; }
    public double CredentialEarnRate { get; set; }
    public int CandidatesEmployed { get; set; }
    public double EmploymentRate { get; set; }
    public int HiredInDistrict { get; set; }
    public double HiredInDistrictRate { get; set; }

    // Hours Tracking
    public double AverageGrantProgramHours { get; set; }
    public double AverageCredentialProgramHours { get; set; }
    public int Met500HoursCount { get; set; }
    public int Met600HoursCount { get; set; }

    // Payment Information
    public decimal TotalAmountDisbursed { get; set; }
    public decimal AveragePaymentAmount { get; set; }

    // Demographics and Breakdowns
    public Dictionary<string, int> EmploymentByStatus { get; set; } = new();
    public Dictionary<string, int> OutcomesByCredentialType { get; set; } = new();
    public Dictionary<string, int> OutcomesByLEA { get; set; } = new();
    public Dictionary<string, int> OutcomesByIHE { get; set; } = new();

    // Candidate Demographics
    public Dictionary<string, int> CandidatesByRaceEthnicity { get; set; } = new();
    public Dictionary<string, int> CandidatesByGender { get; set; } = new();
    public Dictionary<string, int> CandidatesByCredentialArea { get; set; } = new();

    // Year-over-year demographics
    public Dictionary<string, Dictionary<string, int>> CandidatesByRaceEthnicityByYear { get; set; } = new();
    public Dictionary<string, Dictionary<string, int>> CandidatesByGenderByYear { get; set; } = new();
    public Dictionary<string, Dictionary<string, int>> CandidatesByCredentialAreaByYear { get; set; } = new();
}

/// <summary>
/// Filter for report analytics
/// </summary>
public class ReportAnalyticsFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? LEAId { get; set; }
    public int? IHEId { get; set; }
    public string? CredentialType { get; set; }
    public string? Status { get; set; }
}
