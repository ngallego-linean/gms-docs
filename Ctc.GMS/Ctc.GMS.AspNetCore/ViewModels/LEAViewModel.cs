using System.ComponentModel.DataAnnotations;

namespace Ctc.GMS.AspNetCore.ViewModels;

public class LEADashboardViewModel
{
    public int LEAId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public int TotalApplications { get; set; }
    public int TotalStudents { get; set; }
    public int PendingCompletionCount { get; set; }
    public int SubmittedCount { get; set; }
    public int ApprovedCount { get; set; }
    public List<ApplicationSummaryViewModel> Applications { get; set; } = new();
    public List<ActionItemViewModel> ActionItems { get; set; } = new();
}

public class CompleteApplicationViewModel
{
    public int ApplicationId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public string LEAName { get; set; } = string.Empty;
    public int GrantCycleId { get; set; }

    [Required]
    [Display(Name = "LEA POC First Name")]
    public string POCFirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Last Name")]
    public string POCLastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Email")]
    [EmailAddress]
    public string POCEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "LEA POC Phone")]
    [Phone]
    public string POCPhone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "GAA Signer Name")]
    public string GAASignerName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "GAA Signer Title")]
    public string GAASignerTitle { get; set; } = string.Empty;

    [Required]
    [Display(Name = "GAA Signer Email")]
    [EmailAddress]
    public string GAASignerEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Fiscal Agent Name")]
    public string FiscalAgentName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Fiscal Agent Email")]
    [EmailAddress]
    public string FiscalAgentEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Fiscal Agent Phone")]
    [Phone]
    public string FiscalAgentPhone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Superintendent Name")]
    public string SuperintendentName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Superintendent Email")]
    [EmailAddress]
    public string SuperintendentEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Intent")]
    public string PaymentIntent { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Schedule")]
    public string PaymentSchedule { get; set; } = string.Empty;

    public List<StudentViewModel> Students { get; set; } = new();
}
