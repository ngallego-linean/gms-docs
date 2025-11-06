using System.ComponentModel.DataAnnotations;

namespace Ctc.GMS.AspNetCore.ViewModels;

public class StudentGAAViewModel
{
    public int StudentId { get; set; }

    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Display(Name = "SEID")]
    public string SEID { get; set; } = string.Empty;

    [Display(Name = "IHE Institution")]
    public string IHEName { get; set; } = string.Empty;

    [Display(Name = "LEA District")]
    public string LEAName { get; set; } = string.Empty;

    [Display(Name = "Credential Area")]
    public string CredentialArea { get; set; } = string.Empty;

    [Display(Name = "Award Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal AwardAmount { get; set; }

    [Display(Name = "GAA Status")]
    public string GAAStatus { get; set; } = string.Empty;

    [Display(Name = "Approved Date")]
    public DateTime? ApprovedDate { get; set; }
}

public class GAAListViewModel
{
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public List<StudentGAAViewModel> Students { get; set; } = new();
}

public class PaymentViewModel
{
    public int Id { get; set; }

    [Display(Name = "Payment Date")]
    [DataType(DataType.Date)]
    public DateTime PaymentDate { get; set; }

    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Display(Name = "LEA District")]
    public string LEAName { get; set; } = string.Empty;

    [Display(Name = "Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal Amount { get; set; }

    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Display(Name = "Status")]
    public string Status { get; set; } = string.Empty;
}

public class PaymentListViewModel
{
    public int GrantCycleId { get; set; }
    public string GrantCycleName { get; set; } = string.Empty;
    public List<PaymentViewModel> Payments { get; set; } = new();
}
