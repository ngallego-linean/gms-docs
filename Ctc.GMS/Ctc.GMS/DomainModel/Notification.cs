namespace GMS.DomainModel;

/// <summary>
/// Represents email notifications sent throughout the workflow
/// From flowchart: Multiple notification points throughout both CTC and IHE/LEA workflows
/// </summary>
public class Notification
{
    public int Id { get; set; }

    // Recipient Information
    public string RecipientEmail { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientType { get; set; } = string.Empty;  // IHE, LEA, GRANTS_TEAM, FISCAL_TEAM

    // Notification Content
    public string NotificationType { get; set; } = string.Empty;  // APPLICATION_SUBMITTED, REVIEW_NEEDED, GAA_NEEDED, REPORT_DUE, etc.
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;

    // Related Entities
    public int? ApplicationId { get; set; }
    public int? StudentId { get; set; }
    public int? GAADocumentId { get; set; }

    // Delivery Status
    public string Status { get; set; } = string.Empty;  // PENDING, SENT, FAILED, BOUNCED
    public DateTime? SentDate { get; set; }
    public DateTime? OpenedDate { get; set; }
    public DateTime? ClickedDate { get; set; }

    // Error Tracking
    public string ErrorMessage { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public DateTime? LastRetryDate { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }

    // Navigation
    public Application? Application { get; set; }
    public Student? Student { get; set; }
}
