using GMS.Data.Repositories;
using GMS.DomainModel;
using GMS.Business.DTOs;

namespace GMS.Business.Services;

/// <summary>
/// Grant Service implementation for business operations
/// </summary>
public class GrantService : IGrantService
{
    private readonly MockRepository _repository;

    public GrantService(MockRepository repository)
    {
        _repository = repository;
    }

    public GrantCycle? GetGrantCycle(int id)
    {
        return _repository.GetGrantCycle(id);
    }

    public List<GrantCycle> GetGrantCycles()
    {
        return _repository.GetGrantCycles();
    }

    public Application? GetApplication(int id)
    {
        return _repository.GetApplication(id);
    }

    public List<Application> GetApplications()
    {
        return _repository.GetApplications();
    }

    public GrantCycleMetrics CalculateMetrics(int grantCycleId)
    {
        var cycle = GetGrantCycle(grantCycleId);
        if (cycle == null)
            return new GrantCycleMetrics();

        var applications = _repository.GetApplications()
            .Where(a => a.GrantCycleId == grantCycleId)
            .ToList();

        var allStudents = applications.SelectMany(a => a.Students).ToList();

        var reservedAmount = allStudents
            .Where(s => s.Status == "APPROVED" && string.IsNullOrEmpty(s.GAAStatus))
            .Sum(s => s.AwardAmount);

        var encumberedAmount = allStudents
            .Where(s => !string.IsNullOrEmpty(s.GAAStatus) && s.GAAStatus != "PAYMENT_COMPLETED")
            .Sum(s => s.AwardAmount);

        var disbursedAmount = allStudents
            .Where(s => s.GAAStatus == "PAYMENT_COMPLETED")
            .Sum(s => s.AwardAmount);

        var remainingAmount = cycle.ApproprietedAmount - reservedAmount - encumberedAmount - disbursedAmount;
        var remainingPercent = cycle.ApproprietedAmount > 0
            ? (remainingAmount / cycle.ApproprietedAmount) * 100
            : 0;

        return new GrantCycleMetrics
        {
            ApproprietedAmount = cycle.ApproprietedAmount,
            ReservedAmount = reservedAmount,
            EncumberedAmount = encumberedAmount,
            DisbursedAmount = disbursedAmount,
            RemainingAmount = remainingAmount,
            RemainingPercent = remainingPercent,
            OutstandingBalance = reservedAmount + encumberedAmount,
            TotalStudents = allStudents.Count,
            UniqueIHEs = applications.Select(a => a.IHE.Id).Distinct().Count(),
            UniqueLEAs = applications.Select(a => a.LEA.Id).Distinct().Count(),
            ActivePartnerships = applications.Count,
            StatusCounts = new StatusCounts
            {
                Draft = allStudents.Count(s => s.Status == "DRAFT"),
                PendingLEA = allStudents.Count(s => s.Status == "PENDING_LEA"),
                Submitted = allStudents.Count(s => s.Status == "SUBMITTED"),
                UnderReview = allStudents.Count(s => s.Status == "UNDER_REVIEW"),
                Approved = allStudents.Count(s => s.Status == "APPROVED"),
                Rejected = allStudents.Count(s => s.Status == "REJECTED")
            }
        };
    }

    public List<PaymentWithReportsDTO> GetPaymentsWithReportStatus(int grantCycleId)
    {
        var applications = _repository.GetApplications()
            .Where(a => a.GrantCycleId == grantCycleId)
            .ToList();

        var payments = _repository.GetPayments()
            .Where(p => applications.Any(a => a.Id == p.ApplicationId))
            .ToList();

        var result = new List<PaymentWithReportsDTO>();

        foreach (var payment in payments)
        {
            var student = applications
                .SelectMany(a => a.Students)
                .FirstOrDefault(s => s.Id == payment.StudentId);

            var leaReport = _repository.GetLEAReportByPaymentId(payment.Id);
            var iheReport = _repository.GetIHEReportByPaymentId(payment.Id);

            var dto = new PaymentWithReportsDTO
            {
                PaymentId = payment.Id,
                StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown",
                SEID = student?.SEID ?? "",
                LEAName = payment.LEAName,
                AmountPaid = payment.ActualPaymentAmount ?? payment.AuthorizedAmount,
                PaymentDate = payment.ActualPaymentDate,
                PaymentStatus = payment.Status,
                LEAReportStatus = leaReport != null ? "Submitted" : "Pending",
                IHEReportStatus = iheReport != null ? "Submitted" : "Pending",
                HasOutstandingReports = leaReport == null || iheReport == null,
                ProgramCompletion = iheReport?.CompletionStatus ?? "",
                CredentialEarned = iheReport?.Met600Hours ?? false,
                EmploymentStatus = leaReport?.EmploymentStatus ?? "",
                HiredInDistrict = leaReport?.HiredInDistrict ?? false
            };

            result.Add(dto);
        }

        return result;
    }

    public List<ReportingComplianceDTO> GetReportingComplianceByLEA(int grantCycleId)
    {
        var applications = _repository.GetApplications()
            .Where(a => a.GrantCycleId == grantCycleId)
            .ToList();

        var payments = _repository.GetPayments()
            .Where(p => applications.Any(a => a.Id == p.ApplicationId))
            .ToList();

        var leaGroups = payments.GroupBy(p => p.LEAName);

        var result = new List<ReportingComplianceDTO>();

        foreach (var group in leaGroups)
        {
            var totalPayments = group.Count();
            var reportsSubmitted = 0;

            foreach (var payment in group)
            {
                var leaReport = _repository.GetLEAReportByPaymentId(payment.Id);
                var iheReport = _repository.GetIHEReportByPaymentId(payment.Id);
                if (leaReport != null && iheReport != null)
                {
                    reportsSubmitted++;
                }
            }

            var reportsPending = totalPayments - reportsSubmitted;
            var complianceRate = totalPayments > 0 ? (decimal)reportsSubmitted / totalPayments : 0;
            var complianceStatus = complianceRate >= 0.8m ? "COMPLIANT" :
                                  complianceRate >= 0.5m ? "WARNING" : "NON_COMPLIANT";

            var dto = new ReportingComplianceDTO
            {
                LEAName = group.Key,
                TotalPayments = totalPayments,
                ReportsSubmitted = reportsSubmitted,
                ReportsPending = reportsPending,
                ComplianceRate = complianceRate,
                ComplianceStatus = complianceStatus,
                HasPaymentHoldWarning = complianceRate < 0.5m,
                TotalDisbursed = group.Sum(p => p.ActualPaymentAmount ?? p.AuthorizedAmount)
            };

            result.Add(dto);
        }

        return result.OrderByDescending(r => r.TotalDisbursed).ToList();
    }

    public ReportingDashboardDTO GetReportingDashboardMetrics(int grantCycleId)
    {
        var applications = _repository.GetApplications()
            .Where(a => a.GrantCycleId == grantCycleId)
            .ToList();

        var payments = _repository.GetPayments()
            .Where(p => applications.Any(a => a.Id == p.ApplicationId))
            .ToList();

        var totalPayments = payments.Count;
        var paymentsWithReports = 0;

        foreach (var payment in payments)
        {
            var leaReport = _repository.GetLEAReportByPaymentId(payment.Id);
            var iheReport = _repository.GetIHEReportByPaymentId(payment.Id);
            if (leaReport != null && iheReport != null)
            {
                paymentsWithReports++;
            }
        }

        var outstandingReports = totalPayments - paymentsWithReports;
        var complianceRate = totalPayments > 0 ? (decimal)paymentsWithReports / totalPayments : 0;

        var complianceHealth = complianceRate >= 0.8m ? "GREEN" :
                              complianceRate >= 0.5m ? "YELLOW" : "RED";

        return new ReportingDashboardDTO
        {
            TotalPayments = totalPayments,
            PaymentsWithReports = paymentsWithReports,
            OutstandingReports = outstandingReports,
            ReportingComplianceRate = complianceRate,
            ComplianceHealth = complianceHealth,
            OutcomeMetrics = GetOutcomeMetrics(grantCycleId)
        };
    }

    public OutcomeMetricsDTO GetOutcomeMetrics(int grantCycleId)
    {
        var applications = _repository.GetApplications()
            .Where(a => a.GrantCycleId == grantCycleId)
            .ToList();

        var payments = _repository.GetPayments()
            .Where(p => applications.Any(a => a.Id == p.ApplicationId))
            .ToList();

        var iheReports = _repository.GetIHEReports()
            .Where(r => payments.Any(p => p.Id == r.PaymentId))
            .ToList();

        var leaReports = _repository.GetLEAReports()
            .Where(r => payments.Any(p => p.Id == r.PaymentId))
            .ToList();

        var totalStudentsFunded = payments.Count;
        var programCompletions = iheReports.Count(r => r.CompletionStatus == "COMPLETED");
        var credentialsEarned = iheReports.Count(r => r.Met600Hours);
        var teachersEmployed = leaReports.Count(r => r.HiredInDistrict);

        var completionRate = totalStudentsFunded > 0 ? (decimal)programCompletions / totalStudentsFunded : 0;
        var credentialRate = totalStudentsFunded > 0 ? (decimal)credentialsEarned / totalStudentsFunded : 0;
        var employmentRate = totalStudentsFunded > 0 ? (decimal)teachersEmployed / totalStudentsFunded : 0;

        var totalInvestment = payments.Sum(p => p.ActualPaymentAmount ?? p.AuthorizedAmount);
        var costPerSuccessfulTeacher = teachersEmployed > 0 ? totalInvestment / teachersEmployed : 0;

        return new OutcomeMetricsDTO
        {
            TotalStudentsFunded = totalStudentsFunded,
            ProgramCompletions = programCompletions,
            CompletionRate = completionRate,
            CredentialsEarned = credentialsEarned,
            CredentialRate = credentialRate,
            TeachersEmployed = teachersEmployed,
            EmploymentRate = employmentRate,
            TotalInvestment = totalInvestment,
            CostPerSuccessfulTeacher = costPerSuccessfulTeacher
        };
    }

    public int GetOutstandingReportsCount(int grantCycleId)
    {
        var applications = _repository.GetApplications()
            .Where(a => a.GrantCycleId == grantCycleId)
            .ToList();

        var payments = _repository.GetPayments()
            .Where(p => applications.Any(a => a.Id == p.ApplicationId))
            .ToList();

        var outstandingCount = 0;

        foreach (var payment in payments)
        {
            var leaReport = _repository.GetLEAReportByPaymentId(payment.Id);
            var iheReport = _repository.GetIHEReportByPaymentId(payment.Id);
            if (leaReport == null || iheReport == null)
            {
                outstandingCount++;
            }
        }

        return outstandingCount;
    }

    // Student/Candidate Operations
    public Student? GetStudent(int id)
    {
        return _repository.GetApplications()
            .SelectMany(a => a.Students)
            .FirstOrDefault(s => s.Id == id);
    }

    public List<Student> GetFundedStudents(int leaId, int grantCycleId)
    {
        return _repository.GetApplications()
            .Where(a => a.LEA.Id == leaId && a.GrantCycleId == grantCycleId)
            .SelectMany(a => a.Students)
            .Where(s => s.Status == "APPROVED" || s.GAAStatus == "PAYMENT_COMPLETED")
            .OrderByDescending(s => s.ApprovedAt)
            .ToList();
    }

    public List<Student> GetStudentsByStatus(int leaId, int grantCycleId, string status)
    {
        return _repository.GetApplications()
            .Where(a => a.LEA.Id == leaId && a.GrantCycleId == grantCycleId)
            .SelectMany(a => a.Students)
            .Where(s => s.Status == status)
            .ToList();
    }

    // LEA Report Operations
    private static List<LEAReport> _mockReports = new();  // Mock data store for reports
    private static bool _reportsInitialized = false;

    private void InitializeMockReports()
    {
        if (_reportsInitialized) return;
        _reportsInitialized = true;

        // Add some sample completed reports for demonstration
        _mockReports.AddRange(new[]
        {
            new LEAReport
            {
                Id = 1,
                StudentId = 2, // Maria Garcia
                ApplicationId = 1,
                PaymentCategory = "STIPEND",
                PaymentSchedule = "LUMP_SUM",
                ActualPaymentAmount = 10000,
                FirstPaymentDate = DateTime.Now.AddMonths(-2),
                FinalPaymentDate = DateTime.Now.AddMonths(-2),
                ProgramCompletionStatus = "COMPLETED",
                ProgramCompletionDate = DateTime.Now.AddMonths(-1),
                CredentialEarnedStatus = "EARNED",
                CredentialIssueDate = DateTime.Now.AddDays(-15),
                HiredInDistrict = true,
                EmploymentStatus = "FULL_TIME",
                EmploymentStartDate = DateTime.Now.AddDays(-30),
                EmployingLEA = "Los Angeles Unified School District",
                SchoolSite = "Roosevelt High School",
                GradeLevel = "9-12",
                SubjectArea = "Mathematics",
                JobTitle = "High School Math Teacher",
                PlacementQualityRating = 5,
                PlacementQualityNotes = "Excellent candidate, very well prepared",
                MentorTeacherName = "Dr. Susan Martinez",
                MentorTeacherFeedback = "Outstanding student teacher, highly recommended",
                ReportStatus = "SUBMITTED",
                IsLocked = true,
                SubmittedDate = DateTime.Now.AddDays(-7),
                SubmittedBy = "LEA Coordinator",
                SubmittedByEmail = "coordinator@lausd.net",
                CreatedAt = DateTime.Now.AddDays(-10),
                LastModified = DateTime.Now.AddDays(-7)
            },
            new LEAReport
            {
                Id = 2,
                StudentId = 4, // Sarah Johnson
                ApplicationId = 2,
                PaymentCategory = "SALARY",
                PaymentSchedule = "MONTHLY",
                ActualPaymentAmount = 10000,
                FirstPaymentDate = DateTime.Now.AddMonths(-3),
                FinalPaymentDate = DateTime.Now,
                ProgramCompletionStatus = "COMPLETED",
                ProgramCompletionDate = DateTime.Now.AddMonths(-1),
                CredentialEarnedStatus = "IN_PROGRESS",
                HiredInDistrict = false,
                EmploymentStatus = "FULL_TIME",
                EmploymentStartDate = DateTime.Now.AddDays(-45),
                EmployingLEA = "San Diego Unified School District",
                SchoolSite = "Lincoln Middle School",
                GradeLevel = "6-8",
                SubjectArea = "English Language Arts",
                JobTitle = "Middle School English Teacher",
                PlacementQualityRating = 4,
                MentorTeacherName = "Ms. Jennifer Lee",
                ReportStatus = "APPROVED",
                IsLocked = true,
                SubmittedDate = DateTime.Now.AddDays(-14),
                SubmittedBy = "LEA Administrator",
                SubmittedByEmail = "admin@sdusd.org",
                CTCReviewer = "CTC Staff",
                CTCApprovalDate = DateTime.Now.AddDays(-10),
                CreatedAt = DateTime.Now.AddDays(-20),
                LastModified = DateTime.Now.AddDays(-14)
            }
        });
    }

    public LEAReport? GetLEAReport(int studentId)
    {
        InitializeMockReports();
        return _mockReports.FirstOrDefault(r => r.StudentId == studentId);
    }

    public LEAReport? GetLEAReportById(int reportId)
    {
        InitializeMockReports();
        return _mockReports.FirstOrDefault(r => r.Id == reportId);
    }

    public List<LEAReport> GetLEAReports(int leaId, int grantCycleId)
    {
        InitializeMockReports();
        var applications = _repository.GetApplications()
            .Where(a => a.LEA.Id == leaId && a.GrantCycleId == grantCycleId)
            .ToList();

        var studentIds = applications.SelectMany(a => a.Students).Select(s => s.Id).ToList();

        return _mockReports.Where(r => studentIds.Contains(r.StudentId)).ToList();
    }

    public List<LEAReport> GetLEAReportsByStatus(int leaId, int grantCycleId, string status)
    {
        var reports = GetLEAReports(leaId, grantCycleId);
        return reports.Where(r => r.ReportStatus == status).ToList();
    }

    public LEAReport CreateOrUpdateLEAReport(LEAReport report)
    {
        var existing = report.Id > 0 ? GetLEAReportById(report.Id) : GetLEAReport(report.StudentId);

        if (existing != null)
        {
            // Update existing
            existing.PaymentCategory = report.PaymentCategory;
            existing.PaymentSchedule = report.PaymentSchedule;
            existing.ActualPaymentAmount = report.ActualPaymentAmount;
            existing.FirstPaymentDate = report.FirstPaymentDate;
            existing.FinalPaymentDate = report.FinalPaymentDate;
            existing.HiredInDistrict = report.HiredInDistrict;
            existing.EmploymentStatus = report.EmploymentStatus;
            existing.EmploymentStartDate = report.EmploymentStartDate;
            existing.EmployingLEA = report.EmployingLEA;
            existing.SchoolSite = report.SchoolSite;
            existing.GradeLevel = report.GradeLevel;
            existing.SubjectArea = report.SubjectArea;
            existing.JobTitle = report.JobTitle;
            existing.ProgramCompletionStatus = report.ProgramCompletionStatus;
            existing.ProgramCompletionDate = report.ProgramCompletionDate;
            existing.CredentialEarnedStatus = report.CredentialEarnedStatus;
            existing.CredentialIssueDate = report.CredentialIssueDate;
            existing.PlacementQualityRating = report.PlacementQualityRating;
            existing.PlacementQualityNotes = report.PlacementQualityNotes;
            existing.MentorTeacherName = report.MentorTeacherName;
            existing.MentorTeacherFeedback = report.MentorTeacherFeedback;
            existing.AdditionalNotes = report.AdditionalNotes;
            existing.LastModified = DateTime.UtcNow;
            existing.LastModifiedBy = report.LastModifiedBy;

            return existing;
        }
        else
        {
            // Create new
            report.Id = _mockReports.Any() ? _mockReports.Max(r => r.Id) + 1 : 1;
            report.CreatedAt = DateTime.UtcNow;
            report.ReportStatus = "DRAFT";
            report.IsLocked = false;
            _mockReports.Add(report);
            return report;
        }
    }

    public List<LEAReport> BulkCreateLEAReports(List<LEAReport> reports)
    {
        var created = new List<LEAReport>();
        foreach (var report in reports)
        {
            created.Add(CreateOrUpdateLEAReport(report));
        }
        return created;
    }

    public bool DeleteLEAReport(int reportId)
    {
        var report = GetLEAReportById(reportId);
        if (report != null && !report.IsLocked)
        {
            _mockReports.Remove(report);
            return true;
        }
        return false;
    }

    // Report Validation and Status
    public (bool IsValid, List<string> Errors) ValidateLEAReport(LEAReport report)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(report.PaymentCategory))
            errors.Add("Payment Category is required");

        if (string.IsNullOrWhiteSpace(report.PaymentSchedule))
            errors.Add("Payment Schedule is required");

        if (report.ActualPaymentAmount <= 0)
            errors.Add("Actual Payment Amount must be greater than zero");

        if (string.IsNullOrWhiteSpace(report.ProgramCompletionStatus))
            errors.Add("Program Completion Status is required");

        if (string.IsNullOrWhiteSpace(report.CredentialEarnedStatus))
            errors.Add("Credential Status is required");

        if (report.PlacementQualityRating.HasValue && (report.PlacementQualityRating < 1 || report.PlacementQualityRating > 5))
            errors.Add("Placement Quality Rating must be between 1 and 5");

        return (errors.Count == 0, errors);
    }

    public LEAReport SubmitLEAReport(int reportId, string submittedBy, string submittedByEmail)
    {
        var report = GetLEAReportById(reportId);
        if (report == null)
            throw new ArgumentException($"Report {reportId} not found");

        var (isValid, errors) = ValidateLEAReport(report);
        if (!isValid)
            throw new InvalidOperationException($"Report validation failed: {string.Join(", ", errors)}");

        report.ReportStatus = "SUBMITTED";
        report.SubmittedDate = DateTime.UtcNow;
        report.SubmittedBy = submittedBy;
        report.SubmittedByEmail = submittedByEmail;
        report.LastModified = DateTime.UtcNow;

        return report;
    }

    public LEAReport LockLEAReport(int reportId, string lockedBy)
    {
        var report = GetLEAReportById(reportId);
        if (report == null)
            throw new ArgumentException($"Report {reportId} not found");

        report.IsLocked = true;
        report.LockedDate = DateTime.UtcNow;
        report.LockedBy = lockedBy;
        report.ReportStatus = "LOCKED";
        report.LastModified = DateTime.UtcNow;

        return report;
    }

    public LEAReport UnlockLEAReport(int reportId, string ctcReviewer, string feedback)
    {
        var report = GetLEAReportById(reportId);
        if (report == null)
            throw new ArgumentException($"Report {reportId} not found");

        report.IsLocked = false;
        report.LockedDate = null;
        report.LockedBy = string.Empty;
        report.ReportStatus = "REVISION_REQUESTED";
        report.CTCReviewer = ctcReviewer;
        report.CTCFeedback = feedback;
        report.CTCReviewDate = DateTime.UtcNow;
        report.LastModified = DateTime.UtcNow;

        return report;
    }

    public LEAReport ApproveLEAReport(int reportId, string ctcReviewer)
    {
        var report = GetLEAReportById(reportId);
        if (report == null)
            throw new ArgumentException($"Report {reportId} not found");

        report.ReportStatus = "APPROVED";
        report.CTCReviewer = ctcReviewer;
        report.CTCApprovalDate = DateTime.UtcNow;
        report.CTCReviewDate = DateTime.UtcNow;
        report.IsLocked = true;
        report.LastModified = DateTime.UtcNow;

        return report;
    }

    // Reporting Metrics
    public (int Total, int Submitted, int Pending, int Overdue) GetReportingMetrics(int leaId, int grantCycleId, DateTime? deadline = null)
    {
        var fundedStudents = GetFundedStudents(leaId, grantCycleId);
        var total = fundedStudents.Count;

        var reports = GetLEAReports(leaId, grantCycleId);
        var submitted = reports.Count(r => r.ReportStatus == "SUBMITTED" || r.ReportStatus == "APPROVED");

        var pending = total - submitted;

        var overdue = 0;
        if (deadline.HasValue && deadline.Value < DateTime.UtcNow)
        {
            var reportedStudentIds = reports.Where(r => r.SubmittedDate.HasValue).Select(r => r.StudentId).ToHashSet();
            overdue = fundedStudents.Count(s => !reportedStudentIds.Contains(s.Id));
        }

        return (total, submitted, pending, overdue);
    }

    public List<(string Cohort, int Count)> GetCohorts(int leaId, int grantCycleId)
    {
        var fundedStudents = GetFundedStudents(leaId, grantCycleId);

        return fundedStudents
            .Where(s => s.ApprovedAt.HasValue)
            .GroupBy(s => s.ApprovedAt!.Value.ToString("MMMM yyyy"))
            .Select(g => (Cohort: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Cohort)
            .ToList();
    }
}
