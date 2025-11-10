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
}
