namespace GMS.DomainModel;

/// <summary>
/// Represents a reporting period when IHEs must submit completion reports for funded candidates
/// </summary>
public class ReportingPeriod
{
    public int Id { get; set; }

    /// <summary>
    /// The grant cycle this reporting period belongs to
    /// </summary>
    public int GrantCycleId { get; set; }

    /// <summary>
    /// Name of the reporting period (e.g., "Mid-Year Report", "Final Report")
    /// </summary>
    public string PeriodName { get; set; } = string.Empty;

    /// <summary>
    /// When the reporting period opens and IHEs can begin submitting reports
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Deadline for report submission
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Whether this reporting period is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Description or instructions for this reporting period
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of report required (e.g., "Progress", "Completion", "Final")
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    // Navigation properties
    public GrantCycle? GrantCycle { get; set; }
}
