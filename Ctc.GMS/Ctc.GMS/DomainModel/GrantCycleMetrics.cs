namespace GMS.DomainModel;

/// <summary>
/// Contains calculated metrics for a grant cycle
/// </summary>
public class GrantCycleMetrics
{
    public decimal ApproprietedAmount { get; set; }
    public decimal ReservedAmount { get; set; }
    public decimal EncumberedAmount { get; set; }
    public decimal DisbursedAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal RemainingPercent { get; set; }
    public decimal OutstandingBalance { get; set; }
    public int TotalStudents { get; set; }
    public int UniqueIHEs { get; set; }
    public int UniqueLEAs { get; set; }
    public int ActivePartnerships { get; set; }
    public StatusCounts StatusCounts { get; set; } = new();
}

/// <summary>
/// Student status counts for reporting - grouped by workflow stage
/// </summary>
public class StatusCounts
{
    public int Submission { get; set; }     // DRAFT through CTC_SUBMITTED
    public int Review { get; set; }         // CTC_REVIEWING
    public int Disbursement { get; set; }   // CTC_APPROVED through PAYMENT_COMPLETE
    public int Reporting { get; set; }      // REPORTING_PENDING through REPORTS_APPROVED
    public int Rejected { get; set; }       // CTC_REJECTED, REVISION_REQUESTED
    public int Complete { get; set; }       // REPORTS_APPROVED (final state)
}
