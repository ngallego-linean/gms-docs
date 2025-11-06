using System.ComponentModel.DataAnnotations;

namespace Ctc.GMS.AspNetCore.ViewModels;

public class DashboardViewModel
{
    [Display(Name = "Grant Cycle")]
    public int GrantCycleId { get; set; }

    public string GrantCycleName { get; set; } = string.Empty;

    public int PendingReviewCount { get; set; }

    public GrantCycleMetricsViewModel Metrics { get; set; } = new();

    public List<ApplicationSummaryViewModel> ApplicationsWithPendingStudents { get; set; } = new();

    public string CurrentUser { get; set; } = string.Empty;

    public List<ActionItemViewModel> ActionItems { get; set; } = new();
}

public class GrantCycleMetricsViewModel
{
    [Display(Name = "Appropriated Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal ApproprietedAmount { get; set; }

    [Display(Name = "Reserved Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal ReservedAmount { get; set; }

    [Display(Name = "Encumbered Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal EncumberedAmount { get; set; }

    [Display(Name = "Disbursed Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal DisbursedAmount { get; set; }

    [Display(Name = "Remaining Amount")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal RemainingAmount { get; set; }

    public decimal RemainingPercent { get; set; }

    [Display(Name = "Outstanding Balance")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal OutstandingBalance { get; set; }

    [Display(Name = "Total Students")]
    public int TotalStudents { get; set; }

    public int UniqueIHEs { get; set; }

    public int UniqueLEAs { get; set; }

    public int ActivePartnerships { get; set; }

    public StatusCountsViewModel StatusCounts { get; set; } = new();
}

public class StatusCountsViewModel
{
    public int Draft { get; set; }
    public int PendingLEA { get; set; }
    public int Submitted { get; set; }
    public int UnderReview { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
}

public class ActionItemViewModel
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
}
