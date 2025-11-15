using GMS.Data.Repositories;
using GMS.DomainModel;

namespace GMS.Business.Services;

/// <summary>
/// Report Service implementation for business operations related to reporting
/// </summary>
public class ReportService : IReportService
{
    private readonly MockRepository _repository;

    public ReportService(MockRepository repository)
    {
        _repository = repository;
    }

    // IHE Report Operations
    public IHEReport? GetIHEReport(int id)
    {
        return _repository.GetIHEReport(id);
    }

    public List<IHEReport> GetIHEReports()
    {
        var reports = _repository.GetIHEReports();
        // Populate navigation properties
        foreach (var report in reports)
        {
            report.Student = _repository.GetApplications()
                .SelectMany(a => a.Students)
                .FirstOrDefault(s => s.Id == report.StudentId);
            report.Application = _repository.GetApplication(report.ApplicationId);
        }
        return reports;
    }

    public List<IHEReport> GetIHEReportsByStatus(string status)
    {
        var reports = _repository.GetIHEReportsByStatus(status);
        // Populate navigation properties
        foreach (var report in reports)
        {
            report.Student = _repository.GetApplications()
                .SelectMany(a => a.Students)
                .FirstOrDefault(s => s.Id == report.StudentId);
            report.Application = _repository.GetApplication(report.ApplicationId);
        }
        return reports;
    }

    public List<IHEReport> GetIHEReportsByStudent(int studentId)
    {
        return _repository.GetIHEReportsByStudent(studentId);
    }

    public List<IHEReport> GetIHEReportsByApplication(int applicationId)
    {
        return _repository.GetIHEReportsByApplication(applicationId);
    }

    // LEA Report Operations
    public LEAReport? GetLEAReport(int id)
    {
        return _repository.GetLEAReport(id);
    }

    public List<LEAReport> GetLEAReports()
    {
        var reports = _repository.GetLEAReports();
        // Populate navigation properties
        foreach (var report in reports)
        {
            report.Student = _repository.GetApplications()
                .SelectMany(a => a.Students)
                .FirstOrDefault(s => s.Id == report.StudentId);
            report.Application = _repository.GetApplication(report.ApplicationId);
        }
        return reports;
    }

    public List<LEAReport> GetLEAReportsByStatus(string status)
    {
        var reports = _repository.GetLEAReportsByStatus(status);
        // Populate navigation properties
        foreach (var report in reports)
        {
            report.Student = _repository.GetApplications()
                .SelectMany(a => a.Students)
                .FirstOrDefault(s => s.Id == report.StudentId);
            report.Application = _repository.GetApplication(report.ApplicationId);
        }
        return reports;
    }

    public List<LEAReport> GetLEAReportsByStudent(int studentId)
    {
        return _repository.GetLEAReportsByStudent(studentId);
    }

    public List<LEAReport> GetLEAReportsByApplication(int applicationId)
    {
        return _repository.GetLEAReportsByApplication(applicationId);
    }

    // Report Workflow Operations
    public void ApproveIHEReport(int id, string reviewerName)
    {
        var report = GetIHEReport(id);
        if (report != null)
        {
            report.Status = "APPROVED";
            report.ReviewedBy = reviewerName;
            report.ReviewedDate = DateTime.Now;
            report.ApprovedDate = DateTime.Now;
            report.LastModified = DateTime.Now;
        }
    }

    public void ApproveLEAReport(int id, string reviewerName)
    {
        var report = GetLEAReport(id);
        if (report != null)
        {
            report.Status = "APPROVED";
            report.ReviewedBy = reviewerName;
            report.ReviewedDate = DateTime.Now;
            report.ApprovedDate = DateTime.Now;
            report.LastModified = DateTime.Now;
        }
    }

    public void RequestIHEReportRevisions(int id, string reviewerName, string revisionNotes)
    {
        var report = GetIHEReport(id);
        if (report != null)
        {
            report.Status = "REVISIONS_REQUESTED";
            report.ReviewedBy = reviewerName;
            report.ReviewedDate = DateTime.Now;
            report.RevisionNotes = revisionNotes;
            report.RevisionCount++;
            report.LastModified = DateTime.Now;
        }
    }

    public void RequestLEAReportRevisions(int id, string reviewerName, string revisionNotes)
    {
        var report = GetLEAReport(id);
        if (report != null)
        {
            report.Status = "REVISIONS_REQUESTED";
            report.ReviewedBy = reviewerName;
            report.ReviewedDate = DateTime.Now;
            report.RevisionNotes = revisionNotes;
            report.RevisionCount++;
            report.LastModified = DateTime.Now;
        }
    }

    public void SetReportUnderReview(int id, string reportType, string reviewerName)
    {
        if (reportType == "IHE")
        {
            var report = GetIHEReport(id);
            if (report != null)
            {
                report.Status = "UNDER_REVIEW";
                report.ReviewedBy = reviewerName;
                report.ReviewedDate = DateTime.Now;
                report.LastModified = DateTime.Now;
            }
        }
        else if (reportType == "LEA")
        {
            var report = GetLEAReport(id);
            if (report != null)
            {
                report.Status = "UNDER_REVIEW";
                report.ReviewedBy = reviewerName;
                report.ReviewedDate = DateTime.Now;
                report.LastModified = DateTime.Now;
            }
        }
    }

    // Report Analytics and Metrics
    public ReportMetrics GetReportMetrics()
    {
        var iheReports = GetIHEReports();
        var leaReports = GetLEAReports();

        var allStudents = _repository.GetApplications().SelectMany(a => a.Students).ToList();
        var paidStudents = allStudents.Where(s => GMS.Business.Helpers.StatusHelper.IsDisbursedStatus(s.Status)).ToList();

        var submitted = iheReports.Count(r => r.Status == "SUBMITTED") + leaReports.Count(r => r.Status == "SUBMITTED");
        var underReview = iheReports.Count(r => r.Status == "UNDER_REVIEW") + leaReports.Count(r => r.Status == "UNDER_REVIEW");
        var approved = iheReports.Count(r => r.Status == "APPROVED") + leaReports.Count(r => r.Status == "APPROVED");
        var revisionsRequested = iheReports.Count(r => r.Status == "REVISIONS_REQUESTED") + leaReports.Count(r => r.Status == "REVISIONS_REQUESTED");

        // Calculate outstanding reports (paid students without reports)
        var reportedStudentIds = new HashSet<int>(
            iheReports.Select(r => r.StudentId).Union(leaReports.Select(r => r.StudentId))
        );
        var outstanding = paidStudents.Count(s => !reportedStudentIds.Contains(s.Id));

        var totalReportsExpected = paidStudents.Count() * 2; // IHE + LEA per student
        var totalReportsSubmitted = iheReports.Count() + leaReports.Count();
        var complianceRate = totalReportsExpected > 0 ? (double)totalReportsSubmitted / totalReportsExpected * 100 : 0;

        return new ReportMetrics
        {
            ReportsSubmitted = submitted,
            ReportsOutstanding = outstanding,
            ReportsUnderReview = underReview,
            ReportsApproved = approved,
            RevisionsRequested = revisionsRequested,
            IHEReportsSubmitted = iheReports.Count,
            LEAReportsSubmitted = leaReports.Count,
            NextReportingDeadline = new DateTime(2026, 1, 15), // Example deadline
            ComplianceRate = complianceRate
        };
    }

    public ReportComplianceMetrics GetComplianceMetrics()
    {
        var applications = _repository.GetApplications();
        var leas = _repository.GetOrganizations("LEA");
        var leaReports = GetLEAReports();

        var leaCompliance = new List<LEAComplianceInfo>();

        foreach (var lea in leas)
        {
            var leaApplications = applications.Where(a => a.LEA.Id == lea.Id).ToList();
            var paidStudents = leaApplications
                .SelectMany(a => a.Students)
                .Where(s => GMS.Business.Helpers.StatusHelper.IsDisbursedStatus(s.Status))
                .ToList();

            var reportsRequired = paidStudents.Count();
            var reportsSubmitted = leaReports.Count(r =>
                paidStudents.Any(s => s.Id == r.StudentId));

            var compliancePercentage = reportsRequired > 0
                ? (double)reportsSubmitted / reportsRequired * 100
                : 0;

            // Calculate critically overdue (more than 30 days past deadline)
            var deadline = new DateTime(2026, 1, 15);
            var daysOverdue = (DateTime.Now - deadline).Days;
            var criticallyOverdue = daysOverdue > 30 && reportsSubmitted < reportsRequired
                ? reportsRequired - reportsSubmitted
                : 0;

            leaCompliance.Add(new LEAComplianceInfo
            {
                LEAId = lea.Id,
                LEAName = lea.Name,
                ReportsRequired = reportsRequired,
                ReportsSubmitted = reportsSubmitted,
                CompliancePercentage = compliancePercentage,
                CriticallyOverdue = criticallyOverdue
            });
        }

        var fullCompliance = leaCompliance.Count(l => l.CompliancePercentage >= 100);
        var partialCompliance = leaCompliance.Count(l => l.CompliancePercentage > 0 && l.CompliancePercentage < 100);
        var noCompliance = leaCompliance.Count(l => l.CompliancePercentage == 0);

        return new ReportComplianceMetrics
        {
            TotalLEAs = leas.Count,
            LEAsFullCompliance = fullCompliance,
            LEAsPartialCompliance = partialCompliance,
            LEAsNoCompliance = noCompliance,
            LEACompliance = leaCompliance
        };
    }

    public ReportAnalytics GetReportAnalytics(ReportAnalyticsFilter? filter = null)
    {
        var iheReports = GetIHEReports();
        var leaReports = GetLEAReports();
        var allStudents = _repository.GetApplications().SelectMany(a => a.Students).ToList();

        // Apply filters if provided
        if (filter != null)
        {
            if (filter.StartDate.HasValue)
            {
                iheReports = iheReports.Where(r => r.SubmittedDate >= filter.StartDate.Value).ToList();
                leaReports = leaReports.Where(r => r.SubmittedDate >= filter.StartDate.Value).ToList();
            }
            if (filter.EndDate.HasValue)
            {
                iheReports = iheReports.Where(r => r.SubmittedDate <= filter.EndDate.Value).ToList();
                leaReports = leaReports.Where(r => r.SubmittedDate <= filter.EndDate.Value).ToList();
            }
            if (filter.LEAId.HasValue)
            {
                var studentIds = _repository.GetApplications()
                    .Where(a => a.LEA.Id == filter.LEAId.Value)
                    .SelectMany(a => a.Students.Select(s => s.Id))
                    .ToList();
                iheReports = iheReports.Where(r => studentIds.Contains(r.StudentId)).ToList();
                leaReports = leaReports.Where(r => studentIds.Contains(r.StudentId)).ToList();
            }
            if (filter.IHEId.HasValue)
            {
                var studentIds = _repository.GetApplications()
                    .Where(a => a.IHE.Id == filter.IHEId.Value)
                    .SelectMany(a => a.Students.Select(s => s.Id))
                    .ToList();
                iheReports = iheReports.Where(r => studentIds.Contains(r.StudentId)).ToList();
                leaReports = leaReports.Where(r => studentIds.Contains(r.StudentId)).ToList();
            }
        }

        var reportedStudentIds = new HashSet<int>(
            iheReports.Select(r => r.StudentId).Union(leaReports.Select(r => r.StudentId))
        );

        var paidStudents = allStudents.Where(s => GMS.Business.Helpers.StatusHelper.IsDisbursedStatus(s.Status)).ToList();
        var reportedStudents = paidStudents.Where(s => reportedStudentIds.Contains(s.Id)).ToList();

        // Calculate program outcomes
        var completedPrograms = iheReports.Count(r => r.CompletionStatus == "COMPLETED");
        var completionRate = iheReports.Count() > 0 ? (double)completedPrograms / iheReports.Count() * 100 : 0;

        // Employment statistics
        var employed = leaReports.Count(r => r.EmploymentStatus == "FULL_TIME" || r.EmploymentStatus == "PART_TIME");
        var employmentRate = leaReports.Count() > 0 ? (double)employed / leaReports.Count() * 100 : 0;
        var hiredInDistrict = leaReports.Count(r => r.HiredInDistrict);
        var hiredInDistrictRate = employed > 0 ? (double)hiredInDistrict / employed * 100 : 0;

        // Hours tracking
        var met500Hours = iheReports.Count(r => r.Met500Hours);
        var met600Hours = iheReports.Count(r => r.Met600Hours);
        var avgGrantHours = iheReports.Any() ? iheReports.Average(r => r.GrantProgramHours) : 0;
        var avgCredentialHours = iheReports.Any() ? iheReports.Average(r => r.CredentialProgramHours) : 0;

        // Payment information
        var totalDisbursed = leaReports.Sum(r => r.ActualPaymentAmount);
        var avgPayment = leaReports.Any() ? leaReports.Average(r => r.ActualPaymentAmount) : 0;

        // Employment breakdown
        var employmentByStatus = leaReports
            .GroupBy(r => r.EmploymentStatus)
            .ToDictionary(g => g.Key, g => g.Count());

        var analytics = new ReportAnalytics
        {
            TotalCandidatesFunded = paidStudents.Count(),
            TotalCandidatesReported = reportedStudents.Count(),
            ReportingComplianceRate = paidStudents.Count() > 0
                ? (double)reportedStudents.Count() / paidStudents.Count() * 100
                : 0,

            ProgramCompletions = completedPrograms,
            ProgramCompletionRate = completionRate,
            CredentialsEarned = completedPrograms, // Simplified - completion = credential
            CredentialEarnRate = completionRate,
            CandidatesEmployed = employed,
            EmploymentRate = employmentRate,
            HiredInDistrict = hiredInDistrict,
            HiredInDistrictRate = hiredInDistrictRate,

            AverageGrantProgramHours = avgGrantHours,
            AverageCredentialProgramHours = avgCredentialHours,
            Met500HoursCount = met500Hours,
            Met600HoursCount = met600Hours,

            TotalAmountDisbursed = totalDisbursed,
            AveragePaymentAmount = avgPayment,

            EmploymentByStatus = employmentByStatus
        };

        return analytics;
    }

    public List<OutstandingReportInfo> GetOutstandingReports()
    {
        var applications = _repository.GetApplications();
        var iheReports = GetIHEReports();
        var leaReports = GetLEAReports();

        var reportedStudentIds = new HashSet<int>(
            iheReports.Select(r => r.StudentId).Union(leaReports.Select(r => r.StudentId))
        );

        var deadline = new DateTime(2026, 1, 15);
        var outstanding = new List<OutstandingReportInfo>();

        foreach (var app in applications)
        {
            var paidStudents = app.Students.Where(s => GMS.Business.Helpers.StatusHelper.IsDisbursedStatus(s.Status)).ToList();
            var unreportedStudents = paidStudents.Where(s => !reportedStudentIds.Contains(s.Id)).ToList();

            if (unreportedStudents.Any())
            {
                var mostRecentPayment = paidStudents
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefault();

                var paymentDate = mostRecentPayment?.CreatedAt ?? DateTime.Now.AddMonths(-2);
                var daysOverdue = (DateTime.Now - deadline).Days;

                outstanding.Add(new OutstandingReportInfo
                {
                    ApplicationId = app.Id,
                    LEAId = app.LEA.Id,
                    LEAName = app.LEA.Name,
                    IHEId = app.IHE.Id,
                    IHEName = app.IHE.Name,
                    CandidatesPendingReport = unreportedStudents.Count(),
                    PaymentDate = paymentDate,
                    ReportingDeadline = deadline,
                    DaysOverdue = Math.Max(0, daysOverdue),
                    CriticallyOverdue = daysOverdue > 30
                });
            }
        }

        return outstanding.OrderByDescending(o => o.DaysOverdue).ToList();
    }

    public List<ReportDeadlineInfo> GetReportDeadlines()
    {
        var applications = _repository.GetApplications();
        var deadline = new DateTime(2026, 1, 15);
        var daysUntil = (deadline - DateTime.Now).Days;

        return applications.Select(app => new ReportDeadlineInfo
        {
            ApplicationId = app.Id,
            LEAName = app.LEA.Name,
            IHEName = app.IHE.Name,
            ReportingDeadline = deadline,
            DaysUntilDue = daysUntil,
            IsOverdue = daysUntil < 0
        }).ToList();
    }
}
