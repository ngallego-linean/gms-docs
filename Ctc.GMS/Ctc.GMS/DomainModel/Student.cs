namespace GMS.DomainModel;

/// <summary>
/// Represents a student application within an IHE-LEA application
/// </summary>
public class Student
{
    // Core Identity
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;

    // Demographics (from flowchart: IHE submits demographic info)
    public DateTime? DateOfBirth { get; set; }
    public string Last4SSN { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string Ethnicity { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;

    // Credential and Program Info
    public string CredentialArea { get; set; } = string.Empty;
    public string CountyCDSCode { get; set; } = string.Empty;
    public string SchoolCDSCode { get; set; } = string.Empty;

    // Comprehensive Status Tracking
    // Replaces fragmented Status, GAAStatus, and ReportingStatus fields with unified lifecycle tracking
    // Status Values:
    //   Submission Phase: DRAFT, IHE_SUBMITTED, LEA_REVIEWING, LEA_APPROVED, CTC_SUBMITTED
    //   Review Phase: CTC_REVIEWING, CTC_APPROVED, CTC_REJECTED, REVISION_REQUESTED
    //   Disbursement Phase: GAA_PENDING, GAA_GENERATED, GAA_SIGNED, INVOICE_GENERATED,
    //                       PAYMENT_AUTHORIZED, WARRANT_ISSUED, PAYMENT_COMPLETE
    //   Reporting Phase: REPORTING_PENDING, REPORTING_PARTIAL, REPORTING_COMPLETE, REPORTS_APPROVED
    public string Status { get; set; } = "DRAFT";
    public decimal AwardAmount { get; set; }

    // Last Action Tracking (for display and audit)
    public DateTime? LastActionDate { get; set; }
    public string LastActionBy { get; set; } = string.Empty;

    // Reporting Tracking
    public int? CurrentReportingPeriodId { get; set; }
    public ReportingPeriod? CurrentReportingPeriod { get; set; }

    // Computed property for display purposes - combines status with last action date
    public string ApplicationStatus
    {
        get
        {
            var dateStr = LastActionDate?.ToString("MM/dd/yyyy") ?? "";
            return Status switch
            {
                "DRAFT" => "Draft",
                "IHE_SUBMITTED" => $"Submitted to LEA{(dateStr != "" ? $": {dateStr}" : "")}",
                "LEA_REVIEWING" => $"LEA Reviewing{(dateStr != "" ? $": {dateStr}" : "")}",
                "LEA_APPROVED" => $"LEA Approved{(dateStr != "" ? $": {dateStr}" : "")}",
                "CTC_SUBMITTED" => $"Submitted to CTC{(dateStr != "" ? $": {dateStr}" : "")}",
                "CTC_REVIEWING" => $"CTC Reviewing{(dateStr != "" ? $": {dateStr}" : "")}",
                "CTC_APPROVED" => $"Approved by CTC{(dateStr != "" ? $": {dateStr}" : "")}",
                "CTC_REJECTED" => $"Rejected{(dateStr != "" ? $": {dateStr}" : "")}",
                "REVISION_REQUESTED" => $"Revision Requested{(dateStr != "" ? $": {dateStr}" : "")}",
                "GAA_PENDING" => "Awaiting GAA Generation",
                "GAA_GENERATED" => $"GAA Generated{(dateStr != "" ? $": {dateStr}" : "")}",
                "GAA_SIGNED" => $"GAA Signed{(dateStr != "" ? $": {dateStr}" : "")}",
                "INVOICE_GENERATED" => $"Invoice Generated{(dateStr != "" ? $": {dateStr}" : "")}",
                "PAYMENT_AUTHORIZED" => $"Payment Authorized{(dateStr != "" ? $": {dateStr}" : "")}",
                "WARRANT_ISSUED" => $"Warrant Issued{(dateStr != "" ? $": {dateStr}" : "")}",
                "PAYMENT_COMPLETE" => $"Payment Complete{(dateStr != "" ? $": {dateStr}" : "")}",
                "REPORTING_PENDING" => "Reports Pending",
                "REPORTING_PARTIAL" => "Partial Reports Submitted",
                "REPORTING_COMPLETE" => $"Reports Submitted{(dateStr != "" ? $": {dateStr}" : "")}",
                "REPORTS_APPROVED" => $"Reports Approved{(dateStr != "" ? $": {dateStr}" : "")}",
                _ => Status
            };
        }
    }

    // Hours Tracking (from IHE Report requirements)
    public int? GrantProgramHours { get; set; }  // 500 required
    public int? CredentialProgramHours { get; set; }  // 600 required

    // Outcome Tracking (from Dashboard requirements and IHE Report)
    public bool? CredentialEarned { get; set; }
    public DateTime? CredentialEarnedDate { get; set; }
    public bool? SwitchedToIntern { get; set; }

    // Employment Tracking (from LEA Report and Dashboard requirements)
    public bool? EmployedInDistrict { get; set; }
    public bool? EmployedInState { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
}
