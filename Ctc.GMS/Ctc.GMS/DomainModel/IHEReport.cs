namespace GMS.DomainModel;

/// <summary>
/// Represents IHE end-of-cycle reporting for a student
/// From flowchart: "IHE clicks and submits: Completion confirmation (date) or denial, whether switched to intern,
/// have met 500 hours for grant program (Y/N) and 600 hours for credential program (Y/N)"
/// </summary>
public class IHEReport
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int ApplicationId { get; set; }

    // Reporting Period Link
    public int? ReportingPeriodId { get; set; }
    public ReportingPeriod? ReportingPeriod { get; set; }

    // Completion Status (from flowchart: "Completion confirmation (date) or denial")
    public string CompletionStatus { get; set; } = string.Empty;  // COMPLETED, DENIED, IN_PROGRESS
    public DateTime? CompletionDate { get; set; }
    public string DenialReason { get; set; } = string.Empty;

    // Intern Status (from flowchart: "whether switched to intern")
    public bool SwitchedToIntern { get; set; }
    public DateTime? InternSwitchDate { get; set; }

    // Hours Tracking (from flowchart: "have met 500 hours for grant program")
    public int GrantProgramHours { get; set; }
    public bool Met500Hours { get; set; }
    public string GrantProgramHoursNotes { get; set; } = string.Empty;

    // Hours Tracking (from flowchart: "have met 600 hours for credential program")
    public int CredentialProgramHours { get; set; }
    public bool Met600Hours { get; set; }
    public string CredentialProgramHoursNotes { get; set; } = string.Empty;

    // Credential Earned
    public bool CredentialEarned { get; set; }
    public DateTime? CredentialEarnedDate { get; set; }
    public string CredentialType { get; set; } = string.Empty;

    // Employment Status
    public bool EmployedInDistrict { get; set; }
    public bool EmployedInState { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;  // EMPLOYED, NOT_EMPLOYED, SEEKING
    public string EmployerName { get; set; } = string.Empty;
    public DateTime? EmploymentStartDate { get; set; }
    public string SchoolSite { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public string SubjectArea { get; set; } = string.Empty;

    // Additional Information
    public string AdditionalNotes { get; set; } = string.Empty;
    public string DocumentationUrl { get; set; } = string.Empty;

    // Submission and Approval Workflow
    public string Status { get; set; } = "DRAFT";  // DRAFT, SUBMITTED, APPROVED, REJECTED, REVISIONS_REQUESTED
    public DateTime? SubmittedDate { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public string SubmittedByEmail { get; set; } = string.Empty;
    public DateTime? ApprovedDate { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
    public string ConfirmationNumber { get; set; } = string.Empty;

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }

    // Navigation
    public Student? Student { get; set; }
    public Application? Application { get; set; }
}
