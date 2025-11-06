namespace GMS.DomainModel;

/// <summary>
/// Represents an organization (IHE or LEA)
/// </summary>
public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // IHE or LEA
    public string Code { get; set; } = string.Empty;
}
