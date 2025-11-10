using GMS.DomainModel;

namespace GMS.Data.Repositories;

/// <summary>
/// Mock repository providing sample data for the GMS application
/// This will be replaced with Entity Framework repositories once database is configured
/// </summary>
public class MockRepository
{
    private readonly List<GrantCycle> _grantCycles;
    private readonly List<Organization> _organizations;
    private readonly List<Application> _applications;
    private readonly List<ReportingPeriod> _reportingPeriods;
    private readonly List<IHEReport> _iheReports;

    public MockRepository()
    {
        _grantCycles = InitializeGrantCycles();
        _organizations = InitializeOrganizations();
        _reportingPeriods = InitializeReportingPeriods();
        _applications = InitializeApplications();
        _iheReports = InitializeIHEReports();
    }

    private List<GrantCycle> InitializeGrantCycles()
    {
        return new List<GrantCycle>
        {
            new GrantCycle
            {
                Id = 1,
                Name = "Teacher Recruitment Incentive Program (STIPEND) - FY 2025-26",
                ProgramType = "STIPEND",
                ApproprietedAmount = 25000000,
                StartDate = new DateTime(2025, 7, 1),
                EndDate = new DateTime(2026, 6, 30),
                ApplicationOpen = true
            }
        };
    }

    private List<Organization> InitializeOrganizations()
    {
        return new List<Organization>
        {
            new Organization { Id = 1, Name = "Cal State University, Fullerton", Type = "IHE", Code = "CSUF" },
            new Organization { Id = 2, Name = "UCLA", Type = "IHE", Code = "UCLA" },
            new Organization { Id = 3, Name = "San Diego State University", Type = "IHE", Code = "SDSU" },
            new Organization { Id = 4, Name = "University of California, Irvine", Type = "IHE", Code = "UCI" },
            new Organization { Id = 5, Name = "Los Angeles Unified School District", Type = "LEA", Code = "19-64733-0000000" },
            new Organization { Id = 6, Name = "San Diego Unified School District", Type = "LEA", Code = "37-68338-0000000" },
            new Organization { Id = 7, Name = "Fresno Unified School District", Type = "LEA", Code = "10-62281-0000000" },
            new Organization { Id = 8, Name = "Long Beach Unified School District", Type = "LEA", Code = "19-64733-1993647" }
        };
    }

    private List<ReportingPeriod> InitializeReportingPeriods()
    {
        return new List<ReportingPeriod>
        {
            new ReportingPeriod
            {
                Id = 1,
                GrantCycleId = 1,
                PeriodName = "Mid-Year Progress Report",
                StartDate = new DateTime(2025, 12, 1),
                DueDate = new DateTime(2026, 1, 31),
                IsActive = true,
                Description = "Report on candidates who have completed at least 500 hours by mid-year",
                ReportType = "Progress"
            },
            new ReportingPeriod
            {
                Id = 2,
                GrantCycleId = 1,
                PeriodName = "Final Completion Report",
                StartDate = new DateTime(2026, 6, 1),
                DueDate = new DateTime(2026, 7, 31),
                IsActive = false,
                Description = "Final report on program completion, credential earned, and employment outcomes",
                ReportType = "Completion"
            },
            new ReportingPeriod
            {
                Id = 3,
                GrantCycleId = 1,
                PeriodName = "End-of-Year Employment Report",
                StartDate = new DateTime(2026, 8, 1),
                DueDate = new DateTime(2026, 9, 15),
                IsActive = false,
                Description = "Report on employment status one year after program completion",
                ReportType = "Final"
            }
        };
    }

    private List<Application> InitializeApplications()
    {
        var applications = new List<Application>
        {
            new Application
            {
                Id = 1,
                GrantCycleId = 1,
                IHE = _organizations.First(o => o.Id == 1),
                LEA = _organizations.First(o => o.Id == 5),
                Status = "ACTIVE",
                CreatedAt = new DateTime(2025, 8, 15, 10, 0, 0),
                CreatedBy = "coordinator@fullerton.edu",
                LastModified = new DateTime(2025, 11, 3, 14, 30, 0),
                Students = new List<Student>()
            },
            new Application
            {
                Id = 2,
                GrantCycleId = 1,
                IHE = _organizations.First(o => o.Id == 2),
                LEA = _organizations.First(o => o.Id == 6),
                Status = "ACTIVE",
                CreatedAt = new DateTime(2025, 8, 20, 9, 0, 0),
                CreatedBy = "grants@ucla.edu",
                LastModified = new DateTime(2025, 11, 2, 16, 45, 0),
                Students = new List<Student>()
            },
            new Application
            {
                Id = 3,
                GrantCycleId = 1,
                IHE = _organizations.First(o => o.Id == 3),
                LEA = _organizations.First(o => o.Id == 7),
                Status = "ACTIVE",
                CreatedAt = new DateTime(2025, 9, 1, 11, 0, 0),
                CreatedBy = "teacher.ed@sdsu.edu",
                LastModified = new DateTime(2025, 10, 28, 10, 15, 0),
                Students = new List<Student>()
            }
        };

        // Add sample students to applications
        applications[0].Students.AddRange(new[]
        {
            new Student
            {
                Id = 1,
                ApplicationId = 1,
                FirstName = "John",
                LastName = "Smith",
                SEID = "12345678",
                CredentialArea = "Multiple Subject",
                Status = "SUBMITTED",
                AwardAmount = 20000,
                CreatedAt = new DateTime(2025, 8, 16),
                SubmittedAt = new DateTime(2025, 9, 1)
            },
            new Student
            {
                Id = 2,
                ApplicationId = 1,
                FirstName = "Maria",
                LastName = "Garcia",
                SEID = "23456789",
                CredentialArea = "Single Subject - Mathematics",
                Status = "APPROVED",
                AwardAmount = 20000,
                CreatedAt = new DateTime(2025, 8, 17),
                SubmittedAt = new DateTime(2025, 9, 2)
            },
            new Student
            {
                Id = 3,
                ApplicationId = 1,
                FirstName = "David",
                LastName = "Chen",
                SEID = "34567890",
                CredentialArea = "Education Specialist",
                Status = "SUBMITTED",
                AwardAmount = 20000,
                CreatedAt = new DateTime(2025, 8, 18),
                SubmittedAt = new DateTime(2025, 9, 3)
            }
        });

        applications[1].Students.AddRange(new[]
        {
            new Student
            {
                Id = 4,
                ApplicationId = 2,
                FirstName = "Sarah",
                LastName = "Johnson",
                SEID = "45678901",
                CredentialArea = "Single Subject - English",
                Status = "APPROVED",
                AwardAmount = 20000,
                CreatedAt = new DateTime(2025, 8, 21),
                SubmittedAt = new DateTime(2025, 9, 4),
                GAAStatus = "GAA_SIGNED"
            },
            new Student
            {
                Id = 5,
                ApplicationId = 2,
                FirstName = "Michael",
                LastName = "Brown",
                SEID = "56789012",
                CredentialArea = "Multiple Subject",
                Status = "SUBMITTED",
                AwardAmount = 20000,
                CreatedAt = new DateTime(2025, 8, 22),
                SubmittedAt = new DateTime(2025, 9, 5)
            }
        });

        // Add more students to show budget utilization (94% used)
        // Total budget: $25M, want to show ~$23.5M used
        // Add 1170 more students at $20k each = $23.4M
        var additionalStudentId = 6;
        for (int i = 0; i < 390; i++)
        {
            applications[0].Students.Add(new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 1,
                FirstName = $"Student{i}",
                LastName = $"Test{i}",
                SEID = $"{10000000 + i}",
                CredentialArea = i % 3 == 0 ? "Multiple Subject" : i % 3 == 1 ? "Single Subject" : "Education Specialist",
                Status = "APPROVED",
                AwardAmount = 20000,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i % 30),
                SubmittedAt = new DateTime(2025, 9, 15).AddDays(i % 30),
                GAAStatus = i % 3 == 0 ? "PAYMENT_COMPLETED" : i % 3 == 1 ? "GAA_SIGNED" : null
            });
        }

        for (int i = 0; i < 390; i++)
        {
            applications[1].Students.Add(new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 2,
                FirstName = $"Student{i + 390}",
                LastName = $"Test{i + 390}",
                SEID = $"{10000390 + i}",
                CredentialArea = i % 3 == 0 ? "Multiple Subject" : i % 3 == 1 ? "Single Subject" : "Education Specialist",
                Status = "APPROVED",
                AwardAmount = 20000,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i % 30),
                SubmittedAt = new DateTime(2025, 9, 15).AddDays(i % 30),
                GAAStatus = i % 3 == 0 ? "PAYMENT_COMPLETED" : i % 3 == 1 ? "GAA_SIGNED" : null
            });
        }

        for (int i = 0; i < 390; i++)
        {
            applications[2].Students.Add(new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 3,
                FirstName = $"Student{i + 780}",
                LastName = $"Test{i + 780}",
                SEID = $"{10000780 + i}",
                CredentialArea = i % 3 == 0 ? "Multiple Subject" : i % 3 == 1 ? "Single Subject" : "Education Specialist",
                Status = "APPROVED",
                AwardAmount = 20000,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i % 30),
                SubmittedAt = new DateTime(2025, 9, 15).AddDays(i % 30),
                GAAStatus = i % 3 == 0 ? "PAYMENT_COMPLETED" : i % 3 == 1 ? "GAA_SIGNED" : null
            });
        }

        return applications;
    }

    private List<IHEReport> InitializeIHEReports()
    {
        var reports = new List<IHEReport>();

        // Get funded students (those with PAYMENT_COMPLETED status)
        var fundedStudents = _applications.SelectMany(a => a.Students)
            .Where(s => s.GAAStatus == "PAYMENT_COMPLETED")
            .Take(30)
            .ToList();

        // Update some funded students with reporting status
        var reportId = 1;
        foreach (var student in fundedStudents.Take(20))
        {
            student.ReportingStatus = reportId <= 12 ? "SUBMITTED" : reportId <= 17 ? "IN_PROGRESS" : "NOT_STARTED";
            student.CurrentReportingPeriodId = 1;
        }

        // Create sample IHE reports for the first 12 funded students (submitted reports)
        for (int i = 0; i < 12 && i < fundedStudents.Count; i++)
        {
            var student = fundedStudents[i];
            var isCompleted = i % 3 != 2;  // 2 out of 3 completed, 1 out of 3 in progress
            var credentialEarned = i % 4 != 3;  // 3 out of 4 earned credential
            var employed = i % 3 != 2;  // 2 out of 3 employed

            reports.Add(new IHEReport
            {
                Id = reportId++,
                StudentId = student.Id,
                ApplicationId = student.ApplicationId,
                ReportingPeriodId = 1,

                // Completion Status
                CompletionStatus = isCompleted ? "COMPLETED" : (i % 2 == 0 ? "IN_PROGRESS" : "DENIED"),
                CompletionDate = isCompleted ? new DateTime(2025, 12, 15).AddDays(i) : null,
                DenialReason = isCompleted ? string.Empty : "Did not meet attendance requirements",

                // Intern Status
                SwitchedToIntern = i % 5 == 0,
                InternSwitchDate = i % 5 == 0 ? new DateTime(2026, 1, 15) : null,

                // Hours
                GrantProgramHours = isCompleted ? 500 + (i * 10) : 450 + (i * 10),
                Met500Hours = isCompleted,
                GrantProgramHoursNotes = isCompleted ? "Completed all required hours" : "In progress",
                CredentialProgramHours = isCompleted ? 600 + (i * 15) : 550 + (i * 10),
                Met600Hours = isCompleted && credentialEarned,
                CredentialProgramHoursNotes = credentialEarned ? "Exceeded minimum requirements" : "Still completing",

                // Credential
                CredentialEarned = credentialEarned,
                CredentialEarnedDate = credentialEarned ? new DateTime(2026, 1, 30).AddDays(i) : null,
                CredentialType = credentialEarned ? student.CredentialArea : string.Empty,

                // Employment
                EmployedInDistrict = employed && i % 2 == 0,
                EmployedInState = employed,
                EmploymentStatus = employed ? "EMPLOYED" : (i % 2 == 0 ? "SEEKING" : "NOT_EMPLOYED"),
                EmployerName = employed ? (i % 2 == 0 ? "Los Angeles Unified" : "Pasadena Unified") : string.Empty,
                EmploymentStartDate = employed ? new DateTime(2026, 2, 1).AddDays(i) : null,
                SchoolSite = employed ? $"Elementary School #{i + 1}" : string.Empty,
                GradeLevel = employed ? (i % 3 == 0 ? "K-2" : i % 3 == 1 ? "3-5" : "6-8") : string.Empty,
                SubjectArea = employed ? student.CredentialArea : string.Empty,

                // Notes
                AdditionalNotes = isCompleted ? "Excellent progress throughout the program" : "Needs additional support",

                // Submission
                Status = "SUBMITTED",
                SubmittedDate = new DateTime(2025, 12, 20).AddDays(i),
                SubmittedBy = "Jane Coordinator",
                SubmittedByEmail = "coordinator@csuf.edu",
                ApprovedDate = i < 8 ? new DateTime(2025, 12, 25).AddDays(i) : null,
                ApprovedBy = i < 8 ? "CTC Staff" : string.Empty,
                ConfirmationNumber = $"RPT-2026-{1000 + i}",

                // Audit
                CreatedAt = new DateTime(2025, 12, 15).AddDays(i),
                LastModified = new DateTime(2025, 12, 20).AddDays(i)
            });
        }

        // Create 5 draft reports (in progress, not submitted yet)
        for (int i = 12; i < 17 && i < fundedStudents.Count; i++)
        {
            var student = fundedStudents[i];

            reports.Add(new IHEReport
            {
                Id = reportId++,
                StudentId = student.Id,
                ApplicationId = student.ApplicationId,
                ReportingPeriodId = 1,

                // Partial data - draft report
                CompletionStatus = "IN_PROGRESS",
                GrantProgramHours = 480 + (i * 10),
                Met500Hours = false,
                CredentialProgramHours = 570 + (i * 10),
                Met600Hours = false,

                // Status
                Status = "DRAFT",

                // Audit
                CreatedAt = new DateTime(2025, 12, 18).AddDays(i - 12),
                LastModified = new DateTime(2025, 12, 19).AddDays(i - 12)
            });
        }

        return reports;
    }

    // Public methods to access mock data
    public GrantCycle? GetGrantCycle(int id)
    {
        return _grantCycles.FirstOrDefault(gc => gc.Id == id);
    }

    public List<GrantCycle> GetGrantCycles()
    {
        return _grantCycles;
    }

    public Application? GetApplication(int id)
    {
        return _applications.FirstOrDefault(a => a.Id == id);
    }

    public List<Application> GetApplications()
    {
        return _applications;
    }

    public List<Organization> GetOrganizations(string? type = null)
    {
        if (string.IsNullOrEmpty(type))
            return _organizations;

        return _organizations.Where(o => o.Type == type).ToList();
    }

    public List<ReportingPeriod> GetReportingPeriods(int? grantCycleId = null)
    {
        if (grantCycleId.HasValue)
            return _reportingPeriods.Where(rp => rp.GrantCycleId == grantCycleId.Value).ToList();

        return _reportingPeriods;
    }

    public ReportingPeriod? GetReportingPeriod(int id)
    {
        return _reportingPeriods.FirstOrDefault(rp => rp.Id == id);
    }

    public List<IHEReport> GetIHEReports(int? applicationId = null, int? studentId = null, int? reportingPeriodId = null)
    {
        var query = _iheReports.AsQueryable();

        if (applicationId.HasValue)
            query = query.Where(r => r.ApplicationId == applicationId.Value);

        if (studentId.HasValue)
            query = query.Where(r => r.StudentId == studentId.Value);

        if (reportingPeriodId.HasValue)
            query = query.Where(r => r.ReportingPeriodId == reportingPeriodId.Value);

        return query.ToList();
    }

    public IHEReport? GetIHEReport(int id)
    {
        return _iheReports.FirstOrDefault(r => r.Id == id);
    }

    public void AddOrUpdateIHEReport(IHEReport report)
    {
        var existing = _iheReports.FirstOrDefault(r => r.Id == report.Id);
        if (existing != null)
        {
            _iheReports.Remove(existing);
        }
        _iheReports.Add(report);
    }
}
