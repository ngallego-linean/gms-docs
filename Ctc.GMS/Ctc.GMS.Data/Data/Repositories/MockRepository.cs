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

    public MockRepository()
    {
        _grantCycles = InitializeGrantCycles();
        _organizations = InitializeOrganizations();
        _applications = InitializeApplications();
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
}
