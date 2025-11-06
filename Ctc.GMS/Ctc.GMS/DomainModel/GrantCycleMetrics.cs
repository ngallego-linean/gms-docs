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
/// Student status counts for reporting
/// </summary>
public class StatusCounts
{
    public int Draft { get; set; }
    public int PendingLEA { get; set; }
    public int Submitted { get; set; }
    public int UnderReview { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
}
