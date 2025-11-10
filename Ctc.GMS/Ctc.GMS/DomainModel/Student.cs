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

    // Status and Award
    public string Status { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }
    public string GAAStatus { get; set; } = string.Empty;

    // Reporting Status
    public string ReportingStatus { get; set; } = "NOT_STARTED";  // NOT_STARTED, IN_PROGRESS, SUBMITTED, APPROVED
    public int? CurrentReportingPeriodId { get; set; }
    public ReportingPeriod? CurrentReportingPeriod { get; set; }

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
