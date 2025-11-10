namespace GMS.DomainModel;

/// <summary>
/// Represents payment/warrant tracking for approved students
/// From flowchart: "Add process step to key in PO number" and "Fiscal team alerts system that payment has been distributed"
/// </summary>
public class Payment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int ApplicationId { get; set; }

    // Payment Authorization (from flowchart: "add process step to key in PO number")
    public string PONumber { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal AuthorizedAmount { get; set; }
    public DateTime? AuthorizationDate { get; set; }

    // LEA Information (from flowchart requirements)
    public string LEAName { get; set; } = string.Empty;
    public string LEAAddress { get; set; } = string.Empty;

    // Warrant/Payment Distribution (from flowchart: "Fiscal team alerts system that payment has been distributed")
    public string WarrantNumber { get; set; } = string.Empty;
    public DateTime? WarrantDate { get; set; }
    public decimal? ActualPaymentAmount { get; set; }
    public DateTime? ActualPaymentDate { get; set; }

    // Status Tracking
    public string Status { get; set; } = string.Empty;  // AUTHORIZED, DISTRIBUTED, COMPLETED

    // Notes
    public string Notes { get; set; } = string.Empty;

    // Audit
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;

    // Navigation
    public Student? Student { get; set; }
    public Application? Application { get; set; }
    public LEAReport? LEAReport { get; set; }
    public IHEReport? IHEReport { get; set; }

    // Computed Properties
    public bool HasLEAReport => LEAReport != null && LEAReport.SubmittedDate != default;
    public bool HasIHEReport => IHEReport != null && IHEReport.SubmittedDate != default;
    public bool HasOutstandingReports => !HasLEAReport || !HasIHEReport;
}
