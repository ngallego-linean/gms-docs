using Microsoft.AspNetCore.Mvc;
using Ctc.GMS.AspNetCore.ViewModels;
using GMS.Business.Services;
using GMS.Business.Helpers;

namespace Ctc.GMS.Web.UI.Controllers;

[Route("Application")]
public class ApplicationController : Controller
{
    private readonly IGrantService _grantService;
    private readonly ILogger<ApplicationController> _logger;

    public ApplicationController(IGrantService grantService, ILogger<ApplicationController> logger)
    {
        _grantService = grantService;
        _logger = logger;
    }

    [Route("")]
    [Route("Index")]
    public IActionResult Index(ApplicationSearchCriteria? criteria)
    {
        var applications = _grantService.GetApplications();

        var model = new ApplicationListViewModel
        {
            SearchCriteria = criteria ?? new ApplicationSearchCriteria(),
            Applications = applications.Select(a => new ApplicationSummaryViewModel
            {
                Id = a.Id,
                IHEName = a.IHE?.Name ?? "Unknown",
                LEAName = a.LEA?.Name ?? "Unknown",
                TotalStudents = a.Students?.Count ?? 0,
                ApprovedCount = a.Students?.Count(s =>
                    StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Disbursement ||
                    StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Reporting ||
                    StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Complete) ?? 0,
                PendingCount = a.Students?.Count(s =>
                    StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Review) ?? 0,
                Status = a.Status,
                LastModified = a.LastModified
            }).ToList()
        };

        return View(model);
    }

    [Route("Details/{id}")]
    public IActionResult Details(int id)
    {
        var application = _grantService.GetApplication(id);

        if (application == null)
        {
            return NotFound($"Application {id} not found");
        }

        var grantCycle = _grantService.GetGrantCycle(application.GrantCycleId);

        var model = new ApplicationViewModel
        {
            Id = application.Id,
            GrantCycleId = application.GrantCycleId,
            GrantCycle = new GrantCycleViewModel
            {
                Id = grantCycle.Id,
                Name = grantCycle.Name,
                ApproprietedAmount = grantCycle.ApproprietedAmount,
                StartDate = grantCycle.StartDate,
                EndDate = grantCycle.EndDate
            },
            IHE = new OrganizationViewModel
            {
                Id = application.IHE.Id,
                Name = application.IHE.Name,
                Code = application.IHE.Code,
                Type = application.IHE.Type
            },
            LEA = new OrganizationViewModel
            {
                Id = application.LEA.Id,
                Name = application.LEA.Name,
                Code = application.LEA.Code,
                Type = application.LEA.Type
            },
            Status = application.Status,
            Students = application.Students.Select(s => new StudentViewModel
            {
                Id = s.Id,
                ApplicationId = s.ApplicationId,
                SEID = s.SEID,
                FirstName = s.FirstName,
                LastName = s.LastName,
                CredentialArea = s.CredentialArea,
                Status = s.Status,
                AwardAmount = s.AwardAmount,
                CreatedAt = s.CreatedAt,
                SubmittedAt = s.SubmittedAt
            }).ToList(),
            CreatedAt = application.CreatedAt,
            LastModified = application.LastModified,
            CreatedBy = application.CreatedBy
        };

        return View(model);
    }

    [Route("GetApplications")]
    public IActionResult GetApplications()
    {
        // This would be used by Kendo Grid for AJAX data loading
        var applications = _grantService.GetApplications()
            .Select(a => new ApplicationSummaryViewModel
            {
                Id = a.Id,
                IHEName = a.IHE.Name,
                LEAName = a.LEA.Name,
                TotalStudents = a.Students.Count,
                ApprovedCount = a.Students.Count(s =>
                    StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Disbursement ||
                    StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Reporting ||
                    StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Complete),
                PendingCount = a.Students.Count(s =>
                    StatusHelper.GetWorkflowStage(s.Status) == StatusHelper.WorkflowStage.Review),
                Status = a.Status,
                LastModified = a.LastModified
            })
            .ToList();

        return Json(applications);
    }

    [Route("GetStudents")]
    public IActionResult GetStudents(int id)
    {
        // This would be used by Kendo Grid for AJAX data loading
        var application = _grantService.GetApplication(id);

        if (application == null)
        {
            return NotFound();
        }

        var students = application.Students.Select(s => new StudentViewModel
        {
            Id = s.Id,
            ApplicationId = s.ApplicationId,
            SEID = s.SEID,
            FirstName = s.FirstName,
            LastName = s.LastName,
            CredentialArea = s.CredentialArea,
            Status = s.Status,
            AwardAmount = s.AwardAmount,
            CreatedAt = s.CreatedAt,
            SubmittedAt = s.SubmittedAt
        }).ToList();

        return Json(students);
    }
}
