using System.ComponentModel.DataAnnotations;

namespace Ctc.GMS.AspNetCore.ViewModels;

public class ApplicationViewModel
{
    public int Id { get; set; }

    [Display(Name = "Grant Cycle")]
    public int GrantCycleId { get; set; }

    public GrantCycleViewModel GrantCycle { get; set; } = new();

    [Display(Name = "Institution of Higher Education")]
    public OrganizationViewModel IHE { get; set; } = new();

    [Display(Name = "Local Education Agency")]
    public OrganizationViewModel LEA { get; set; } = new();

    public string Status { get; set; } = string.Empty;

    [Display(Name = "Students")]
    public List<StudentViewModel> Students { get; set; } = new();

    [Display(Name = "Created Date")]
    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Last Modified")]
    [DataType(DataType.Date)]
    public DateTime LastModified { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    // Computed properties
    public int ApprovedCount => Students.Count(s => s.Status == "APPROVED");
    public int PendingCount => Students.Count(s => s.Status == "SUBMITTED");
    public int TotalCount => Students.Count;
}

public class ApplicationListViewModel
{
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public ApplicationSearchCriteria SearchCriteria { get; set; } = new();
    public List<ApplicationSummaryViewModel> Applications { get; set; } = new();
    public List<ApplicationSummaryViewModel> Results { get; set; } = new();
}

public class ApplicationSearchCriteria
{
    [Display(Name = "Grant Cycle")]
    public int? GrantCycleId { get; set; }

    [Display(Name = "IHE Institution")]
    public int? IHEId { get; set; }

    [Display(Name = "LEA District")]
    public int? LEAId { get; set; }

    [Display(Name = "Status")]
    public string? Status { get; set; }
}

public class ApplicationSummaryViewModel
{
    public int Id { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public string LEAName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int ApprovedCount { get; set; }
    public int PendingCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
}

public class GrantCycleViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal ApproprietedAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class OrganizationViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class StudentViewModel
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }

    [Display(Name = "SEID")]
    [Required]
    public string SEID { get; set; } = string.Empty;

    [Display(Name = "First Name")]
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Last Name")]
    [Required]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Credential Area")]
    [Required]
    public string CredentialArea { get; set; } = string.Empty;

    [Display(Name = "Status")]
    public string Status { get; set; } = string.Empty;

    [Display(Name = "Award Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal AwardAmount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class StudentReviewViewModel
{
    public int ApplicationId { get; set; }
    public string IHEName { get; set; } = string.Empty;
    public string LEAName { get; set; } = string.Empty;
    public StudentViewModel Student { get; set; } = new();
}
