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
    public string EmploymentStatus { get; set; } = string.Empty;  // e.g., "FULL_TIME", "PART_TIME", "NOT_HIRED"
    public DateTime? HireDate { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string SchoolSite { get; set; } = string.Empty;

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
