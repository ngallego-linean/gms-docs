using GMS.Data.Repositories;
using GMS.DomainModel;

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
}
