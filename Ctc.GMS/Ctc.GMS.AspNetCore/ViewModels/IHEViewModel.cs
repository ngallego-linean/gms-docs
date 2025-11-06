using System.ComponentModel.DataAnnotations;

namespace Ctc.GMS.AspNetCore.ViewModels;

public class IHEDashboardViewModel
{
    public int IHEId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public int TotalApplications { get; set; }
    public int TotalStudents { get; set; }
    public int DraftCount { get; set; }
    public int SubmittedCount { get; set; }
    public int ApprovedCount { get; set; }
    public List<ApplicationSummaryViewModel> Applications { get; set; } = new();
    public List<ActionItemViewModel> ActionItems { get; set; } = new();
}

public class SubmitCandidatesViewModel
{
    public int IHEId { get; set; }
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "POC First Name")]
    public string POCFirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "POC Last Name")]
    public string POCLastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "POC Email")]
    [EmailAddress]
    public string POCEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "POC Phone")]
    [Phone]
    public string POCPhone { get; set; } = string.Empty;

    public List<CandidateViewModel> Candidates { get; set; } = new();
}

public class CandidateViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [Display(Name = "Last 4 of SSN/ITIN")]
    [StringLength(4, MinimumLength = 4)]
    public string Last4SSN { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Credential Area")]
    public string CredentialArea { get; set; } = string.Empty;

    [Required]
    [Display(Name = "County CDS Code")]
    public string CountyCDSCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA CDS Code")]
    public string LEACDSCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "School CDS Code")]
    public string SchoolCDSCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC First Name")]
    public string LEAPOCFirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Last Name")]
    public string LEAPOCLastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Email")]
    [EmailAddress]
    public string LEAPOCEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Phone")]
    [Phone]
    public string LEAPOCPhone { get; set; } = string.Empty;
}
