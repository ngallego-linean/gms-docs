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

    // Additional Information
    public string AdditionalNotes { get; set; } = string.Empty;
    public string DocumentationUrl { get; set; } = string.Empty;

    // Submission Tracking
    public DateTime SubmittedDate { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public string SubmittedByEmail { get; set; } = string.Empty;

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }

    // Navigation
    public Student? Student { get; set; }
    public Application? Application { get; set; }
}
