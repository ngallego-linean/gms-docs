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

    // Application-Level Status Tracking
    // Status Values: DRAFT, COLLECTING_STUDENTS, LEA_REVIEW, CTC_REVIEW,
    //                DISBURSEMENT, REPORTING, COMPLETE, CLOSED
    public string Status { get; set; } = "DRAFT";

    // Last Action Tracking
    public DateTime? LastActionDate { get; set; }
    public string LastActionBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public List<Student> Students { get; set; } = new();

    // Computed property for display purposes
    public string ApplicationStatus
    {
        get
        {
            var dateStr = LastActionDate?.ToString("MM/dd/yyyy") ?? "";
            return Status switch
            {
                "DRAFT" => "Draft",
                "COLLECTING_STUDENTS" => "Collecting Candidates",
                "LEA_REVIEW" => $"LEA Review{(dateStr != "" ? $": {dateStr}" : "")}",
                "CTC_REVIEW" => $"CTC Review{(dateStr != "" ? $": {dateStr}" : "")}",
                "DISBURSEMENT" => $"Disbursement Processing{(dateStr != "" ? $": {dateStr}" : "")}",
                "REPORTING" => $"Reporting Phase{(dateStr != "" ? $": {dateStr}" : "")}",
                "COMPLETE" => $"Complete{(dateStr != "" ? $": {dateStr}" : "")}",
                "CLOSED" => $"Closed{(dateStr != "" ? $": {dateStr}" : "")}",
                _ => Status
            };
        }
    }
}
