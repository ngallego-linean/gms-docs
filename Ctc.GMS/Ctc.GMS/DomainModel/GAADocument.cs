namespace GMS.DomainModel;

/// <summary>
/// Represents Grant Award Agreement (GAA) documents and DocuSign tracking
/// From flowchart: "Fiscal team creates GAA in system" and "Fiscal team sends GAA in system via DocuSign"
/// </summary>
public class GAADocument
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int ApplicationId { get; set; }

    // Document Information
    public string DocumentUrl { get; set; } = string.Empty;
    public string DocumentFileName { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }
    public string AwardDescription { get; set; } = string.Empty;

    // DocuSign Integration (from flowchart: "Fiscal team sends GAA via DocuSign")
    public string DocuSignEnvelopeId { get; set; } = string.Empty;
    public string DocuSignStatus { get; set; } = string.Empty;  // CREATED, SENT, DELIVERED, SIGNED, COMPLETED, DECLINED, VOIDED
    public DateTime? DocuSignSentDate { get; set; }
    public DateTime? DocuSignViewedDate { get; set; }
    public DateTime? DocuSignSignedDate { get; set; }
    public DateTime? DocuSignCompletedDate { get; set; }

    // Signers
    public string LEASignerName { get; set; } = string.Empty;
    public string LEASignerEmail { get; set; } = string.Empty;
    public string LEASignerTitle { get; set; } = string.Empty;
    public DateTime? LEASignedDate { get; set; }

    // Payment Information (linked from flowchart: "add process step to key in PO number")
    public string PONumber { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string LEAAddress { get; set; } = string.Empty;

    // Status Tracking
    public string Status { get; set; } = string.Empty;  // DRAFT, PENDING_DOCUSIGN, SENT, SIGNED, PROCESSING, COMPLETED

    // Notes
    public string Notes { get; set; } = string.Empty;
    public string InternalNotes { get; set; } = string.Empty;

    // Audit
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;

    // Navigation
    public Student? Student { get; set; }
    public Application? Application { get; set; }
}
