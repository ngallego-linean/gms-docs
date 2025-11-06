namespace GMS.DomainModel;

/// <summary>
/// Represents a grant cycle for the Teacher Recruitment Incentive Program
/// </summary>
public class GrantCycle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProgramType { get; set; } = string.Empty;
    public decimal ApproprietedAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool ApplicationOpen { get; set; }
}
