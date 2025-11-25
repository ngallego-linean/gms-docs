namespace GMS.Business.Helpers;

/// <summary>
/// Utility class for mapping status codes to display-friendly text and UI properties
/// </summary>
public static class StatusHelper
{
    // Workflow Stage Groupings
    public enum WorkflowStage
    {
        Submission,
        Review,
        Disbursement,
        Reporting,
        Complete,
        Rejected
    }

    // CTC Status Constants (public-facing statuses)
    public static class CTCStatus
    {
        public const string Submitted = "SUBMITTED";
        public const string Pending = "PENDING";
        public const string UnderReview = "UNDER_REVIEW";
        public const string Waitlist = "WAITLIST";
        public const string Approved = "APPROVED";
        public const string PaymentInProcess = "PAYMENT_IN_PROCESS";
        public const string Paid = "PAID";
    }

    /// <summary>
    /// Get the workflow stage for a given student status
    /// </summary>
    public static WorkflowStage GetWorkflowStage(string status)
    {
        return status switch
        {
            // CTC Public-Facing Statuses
            "SUBMITTED" or "PENDING" => WorkflowStage.Submission,
            "UNDER_REVIEW" or "WAITLIST" => WorkflowStage.Review,
            "APPROVED" or "PAYMENT_IN_PROCESS" or "PAID" => WorkflowStage.Disbursement,

            // Legacy/Internal Statuses
            "DRAFT" or "IHE_SUBMITTED" or "LEA_REVIEWING" or "LEA_APPROVED" or "CTC_SUBMITTED" => WorkflowStage.Submission,
            "CTC_REVIEWING" => WorkflowStage.Review,
            "CTC_APPROVED" or "GAA_PENDING" or "GAA_GENERATED" or "GAA_SIGNED" or
            "INVOICE_GENERATED" or "PAYMENT_AUTHORIZED" or "WARRANT_ISSUED" or "PAYMENT_COMPLETE" => WorkflowStage.Disbursement,
            "REPORTING_PENDING" or "REPORTING_PARTIAL" or "REPORTING_COMPLETE" or "REPORTS_APPROVED" => WorkflowStage.Reporting,
            "CTC_REJECTED" or "REVISION_REQUESTED" => WorkflowStage.Rejected,
            _ => WorkflowStage.Submission
        };
    }

    /// <summary>
    /// Get the badge CSS class for a given status
    /// </summary>
    public static string GetBadgeClass(string status)
    {
        return status switch
        {
            // CTC Public-Facing Statuses
            "SUBMITTED" => "badge badge-info",                    // Info Blue - submitted by IHE
            "PENDING" => "badge badge-secondary",                 // Gray - LEA reviewing/processing
            "UNDER_REVIEW" => "badge badge-warning",              // Warning Yellow - CTC reviewing
            "WAITLIST" => "badge badge-waitlist",                 // CTC Gold - funding low, in queue
            "APPROVED" => "badge badge-success",                  // Success Green - eligible for payment
            "PAYMENT_IN_PROCESS" => "badge badge-primary",        // Primary Blue - disbursement happening
            "PAID" => "badge badge-paid",                         // Dark Green - funds disbursed

            // Reporting Statuses
            "IN_PROGRESS" => "badge badge-warning",               // Warning Yellow - report in progress
            "NOT_STARTED" => "badge badge-secondary",             // Gray - not started

            // Legacy/Internal Statuses
            // Success statuses (green)
            "CTC_APPROVED" or "LEA_APPROVED" or "PAYMENT_COMPLETE" or "PAYMENT_COMPLETED" or "REPORTS_APPROVED" => "badge badge-success",

            // Warning statuses (yellow/orange)
            "REVISION_REQUESTED" or "LEA_REVIEWING" or "CTC_REVIEWING" or "REPORTING_PARTIAL" => "badge badge-warning",

            // Danger statuses (red)
            "CTC_REJECTED" => "badge badge-danger",

            // Info statuses (blue)
            "CTC_SUBMITTED" or "IHE_SUBMITTED" or "WARRANT_ISSUED" or "INVOICE_GENERATED" or "PAYMENT_AUTHORIZED" => "badge badge-info",

            // Processing statuses (light blue)
            "GAA_PENDING" or "GAA_GENERATED" or "GAA_SIGNED" or "REPORTING_PENDING" => "badge badge-primary",

            // Draft/default (gray)
            "DRAFT" or _ => "badge badge-secondary"
        };
    }

    /// <summary>
    /// Get display-friendly text for a status code (without date)
    /// </summary>
    public static string GetDisplayText(string status)
    {
        return status switch
        {
            // CTC Public-Facing Statuses
            "SUBMITTED" => "Submitted",
            "PENDING" => "Pending",
            "UNDER_REVIEW" => "Under Review",
            "WAITLIST" => "Waitlist",
            "APPROVED" => "Approved",
            "PAYMENT_IN_PROCESS" => "Payment in Process",
            "PAID" => "Paid",

            // Reporting Statuses
            "IN_PROGRESS" => "In Progress",
            "NOT_STARTED" => "Not Started",
            "PAYMENT_COMPLETED" => "Paid",

            // Legacy/Internal Statuses
            "DRAFT" => "Draft",
            "IHE_SUBMITTED" => "Submitted to LEA",
            "LEA_REVIEWING" => "LEA Reviewing",
            "LEA_APPROVED" => "LEA Approved",
            "CTC_SUBMITTED" => "Submitted to CTC",
            "CTC_REVIEWING" => "CTC Reviewing",
            "CTC_APPROVED" => "Approved by CTC",
            "CTC_REJECTED" => "Rejected",
            "REVISION_REQUESTED" => "Revision Requested",
            "GAA_PENDING" => "Awaiting GAA Generation",
            "GAA_GENERATED" => "GAA Generated",
            "GAA_SIGNED" => "GAA Signed",
            "INVOICE_GENERATED" => "Invoice Generated",
            "PAYMENT_AUTHORIZED" => "Payment Authorized",
            "WARRANT_ISSUED" => "Warrant Issued",
            "PAYMENT_COMPLETE" => "Payment Complete",
            "REPORTING_PENDING" => "Reports Pending",
            "REPORTING_PARTIAL" => "Partial Reports Submitted",
            "REPORTING_COMPLETE" => "Reports Submitted",
            "REPORTS_APPROVED" => "Reports Approved",
            _ => status
        };
    }

    /// <summary>
    /// Get display-friendly text with last action date
    /// </summary>
    public static string GetApplicationStatus(string status, DateTime? lastActionDate)
    {
        var displayText = GetDisplayText(status);
        if (lastActionDate.HasValue)
        {
            return $"{displayText}: {lastActionDate.Value:MM/dd/yyyy}";
        }
        return displayText;
    }

    /// <summary>
    /// Determine if a student is in a payable status (approved and in disbursement process)
    /// </summary>
    public static bool IsPayableStatus(string status)
    {
        return status switch
        {
            "CTC_APPROVED" or "GAA_PENDING" or "GAA_GENERATED" or "GAA_SIGNED" or
            "INVOICE_GENERATED" or "PAYMENT_AUTHORIZED" or "WARRANT_ISSUED" or "PAYMENT_COMPLETE" => true,
            _ => false
        };
    }

    /// <summary>
    /// Determine if a student should be included in budget reserved calculations
    /// </summary>
    public static bool IsReservedStatus(string status)
    {
        return status == "CTC_APPROVED";
    }

    /// <summary>
    /// Determine if a student should be included in budget encumbered calculations
    /// </summary>
    public static bool IsEncumberedStatus(string status)
    {
        return status switch
        {
            "GAA_PENDING" or "GAA_GENERATED" or "GAA_SIGNED" or "INVOICE_GENERATED" or
            "PAYMENT_AUTHORIZED" or "WARRANT_ISSUED" => true,
            _ => false
        };
    }

    /// <summary>
    /// Determine if a student should be included in budget disbursed calculations
    /// </summary>
    public static bool IsDisbursedStatus(string status)
    {
        return status == "PAYMENT_COMPLETE";
    }

    /// <summary>
    /// Get the next logical status(es) for a given current status
    /// Used for workflow buttons/actions
    /// </summary>
    public static List<string> GetNextStatuses(string currentStatus)
    {
        return currentStatus switch
        {
            "DRAFT" => new List<string> { "IHE_SUBMITTED" },
            "IHE_SUBMITTED" => new List<string> { "LEA_REVIEWING" },
            "LEA_REVIEWING" => new List<string> { "LEA_APPROVED", "REVISION_REQUESTED" },
            "LEA_APPROVED" => new List<string> { "CTC_SUBMITTED" },
            "CTC_SUBMITTED" => new List<string> { "CTC_REVIEWING" },
            "CTC_REVIEWING" => new List<string> { "CTC_APPROVED", "CTC_REJECTED", "REVISION_REQUESTED" },
            "CTC_APPROVED" => new List<string> { "GAA_PENDING" },
            "GAA_PENDING" => new List<string> { "GAA_GENERATED" },
            "GAA_GENERATED" => new List<string> { "GAA_SIGNED" },
            "GAA_SIGNED" => new List<string> { "INVOICE_GENERATED" },
            "INVOICE_GENERATED" => new List<string> { "PAYMENT_AUTHORIZED" },
            "PAYMENT_AUTHORIZED" => new List<string> { "WARRANT_ISSUED" },
            "WARRANT_ISSUED" => new List<string> { "PAYMENT_COMPLETE" },
            "PAYMENT_COMPLETE" => new List<string> { "REPORTING_PENDING" },
            "REPORTING_PENDING" => new List<string> { "REPORTING_PARTIAL", "REPORTING_COMPLETE" },
            "REPORTING_PARTIAL" => new List<string> { "REPORTING_COMPLETE" },
            "REPORTING_COMPLETE" => new List<string> { "REPORTS_APPROVED" },
            "REVISION_REQUESTED" => new List<string> { "IHE_SUBMITTED", "CTC_SUBMITTED" },
            _ => new List<string>()
        };
    }
}
