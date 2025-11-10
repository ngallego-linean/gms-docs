using GMS.DomainModel;

namespace GMS.Business.Services;

/// <summary>
/// Grant Service interface for business operations related to grant cycles and applications
/// </summary>
public interface IGrantService
{
    // Grant Cycle Operations
    GrantCycle? GetGrantCycle(int id);
    List<GrantCycle> GetGrantCycles();
    Application? GetApplication(int id);
    List<Application> GetApplications();
    GrantCycleMetrics CalculateMetrics(int grantCycleId);

    // Student/Candidate Operations
    Student? GetStudent(int id);
    List<Student> GetFundedStudents(int leaId, int grantCycleId);
    List<Student> GetStudentsByStatus(int leaId, int grantCycleId, string status);

    // LEA Report Operations
    LEAReport? GetLEAReport(int studentId);
    LEAReport? GetLEAReportById(int reportId);
    List<LEAReport> GetLEAReports(int leaId, int grantCycleId);
    List<LEAReport> GetLEAReportsByStatus(int leaId, int grantCycleId, string status);
    LEAReport CreateOrUpdateLEAReport(LEAReport report);
    List<LEAReport> BulkCreateLEAReports(List<LEAReport> reports);
    bool DeleteLEAReport(int reportId);

    // Report Validation and Status
    (bool IsValid, List<string> Errors) ValidateLEAReport(LEAReport report);
    LEAReport SubmitLEAReport(int reportId, string submittedBy, string submittedByEmail);
    LEAReport LockLEAReport(int reportId, string lockedBy);
    LEAReport UnlockLEAReport(int reportId, string ctcReviewer, string feedback);
    LEAReport ApproveLEAReport(int reportId, string ctcReviewer);

    // Reporting Metrics
    (int Total, int Submitted, int Pending, int Overdue) GetReportingMetrics(int leaId, int grantCycleId, DateTime? deadline = null);
    List<(string Cohort, int Count)> GetCohorts(int leaId, int grantCycleId);
}
