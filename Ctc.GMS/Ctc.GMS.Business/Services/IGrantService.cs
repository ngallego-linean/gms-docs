using GMS.DomainModel;
using GMS.Business.DTOs;

namespace GMS.Business.Services;

/// <summary>
/// Grant Service interface for business operations related to grant cycles and applications
/// </summary>
public interface IGrantService
{
    GrantCycle? GetGrantCycle(int id);
    List<GrantCycle> GetGrantCycles();
    Application? GetApplication(int id);
    List<Application> GetApplications();
    GrantCycleMetrics CalculateMetrics(int grantCycleId);

    // Reporting methods
    List<PaymentWithReportsDTO> GetPaymentsWithReportStatus(int grantCycleId);
    List<ReportingComplianceDTO> GetReportingComplianceByLEA(int grantCycleId);
    ReportingDashboardDTO GetReportingDashboardMetrics(int grantCycleId);
    OutcomeMetricsDTO GetOutcomeMetrics(int grantCycleId);
    int GetOutstandingReportsCount(int grantCycleId);
}
