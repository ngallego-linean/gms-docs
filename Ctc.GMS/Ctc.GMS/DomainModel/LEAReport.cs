namespace GMS.DomainModel;

/// <summary>
/// Represents LEA end-of-cycle reporting for a student
/// From flowchart: "LEA clicks and submits: How they categorized ST for payment, Payment schedule and amount,
/// Payment date, Employment-- whether hired in the district"
/// </summary>
public class LEAReport
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int ApplicationId { get; set; }
    public int? PaymentId { get; set; }  // Link to Payment record

    // Payment Categorization (from flowchart: "How they categorized ST for payment")
    public string PaymentCategorization { get; set; } = string.Empty;
    public string PaymentCategory { get; set; } = string.Empty;  // e.g., "SALARY", "STIPEND", "OTHER"

    // Payment Details (from flowchart: "Payment schedule and amount")
    public string PaymentSchedule { get; set; } = string.Empty;  // e.g., "MONTHLY", "QUARTERLY", "LUMP_SUM"
    public decimal ActualPaymentAmount { get; set; }
    public string PaymentScheduleDetails { get; set; } = string.Empty;

    // Payment Date (from flowchart: "Payment date")
    public DateTime? FirstPaymentDate { get; set; }
    public DateTime? FinalPaymentDate { get; set; }
    public string PaymentNotes { get; set; } = string.Empty;

    // Employment Status (from flowchart: "Employment-- whether hired in the district")
    public bool HiredInDistrict { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;  // e.g., "FULL_TIME", "PART_TIME", "NOT_HIRED", "SEEKING"
    public DateTime? HireDate { get; set; }
    public DateTime? EmploymentStartDate { get; set; }
    public string EmployingLEA { get; set; } = string.Empty;  // Which LEA hired them (may differ from submitting LEA)
    public string JobTitle { get; set; } = string.Empty;
    public string SchoolSite { get; set; } = string.Empty;

    // Teaching Assignment
    public string GradeLevel { get; set; } = string.Empty;  // e.g., "K-2", "3-5", "6-8", "9-12"
    public string SubjectArea { get; set; } = string.Empty;  // e.g., "Mathematics", "English", "Science"

    // Program Completion
    public string ProgramCompletionStatus { get; set; } = string.Empty;  // "COMPLETED", "IN_PROGRESS", "NOT_COMPLETED"
    public DateTime? ProgramCompletionDate { get; set; }

    // Credential Information
    public string CredentialEarnedStatus { get; set; } = string.Empty;  // "EARNED", "IN_PROGRESS", "NOT_EARNED"
    public DateTime? CredentialIssueDate { get; set; }

    // Quality Metrics
    public int? PlacementQualityRating { get; set; }  // 1-5 scale
    public string PlacementQualityNotes { get; set; } = string.Empty;
    public string MentorTeacherName { get; set; } = string.Empty;
    public string MentorTeacherFeedback { get; set; } = string.Empty;

    // Additional Information
    public string AdditionalNotes { get; set; } = string.Empty;
    public string DocumentationUrl { get; set; } = string.Empty;

    // Report Status and Workflow
    // Status Values: DRAFT, SUBMITTED, UNDER_REVIEW, APPROVED, REVISION_REQUESTED, REJECTED
    // (Consolidated from separate ReportStatus and Status fields)
    public bool IsLocked { get; set; }  // Lock after submission, unlock only if CTC requests revisions
    public DateTime? LockedDate { get; set; }
    public string LockedBy { get; set; } = string.Empty;

    // CTC Review
    public string CTCReviewer { get; set; } = string.Empty;
    public DateTime? CTCReviewDate { get; set; }
    public DateTime? CTCApprovalDate { get; set; }
    public string CTCFeedback { get; set; } = string.Empty;  // Feedback if revisions requested

    // Submission Tracking
    public DateTime? SubmittedDate { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public string SubmittedByEmail { get; set; } = string.Empty;

    // Review and Approval Tracking
    public string Status { get; set; } = "DRAFT";
    public string ReviewedBy { get; set; } = string.Empty;
    public DateTime? ReviewedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string RevisionNotes { get; set; } = string.Empty;
    public int RevisionCount { get; set; } = 0;
    public string InternalNotes { get; set; } = string.Empty;  // CTC staff notes

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public string LastModifiedBy { get; set; } = string.Empty;

    // Navigation
    public Student? Student { get; set; }
    public Application? Application { get; set; }
    public Payment? Payment { get; set; }
}
