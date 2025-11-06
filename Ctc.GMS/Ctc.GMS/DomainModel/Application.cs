namespace GMS.DomainModel;

/// <summary>
/// Represents an IHE-LEA application for a grant cycle
/// </summary>
public class Application
{
    public int Id { get; set; }
    public int GrantCycleId { get; set; }
    public Organization IHE { get; set; } = new();
    public Organization LEA { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public List<Student> Students { get; set; } = new();
}
