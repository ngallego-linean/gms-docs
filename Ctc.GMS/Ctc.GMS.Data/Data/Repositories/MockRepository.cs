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
    private readonly List<IHEReport> _iheReports;
    private readonly List<LEAReport> _leaReports;

    public MockRepository()
    {
        _grantCycles = InitializeGrantCycles();
        _organizations = InitializeOrganizations();
        _applications = InitializeApplications();
        _iheReports = InitializeIHEReports();
        _leaReports = InitializeLEAReports();
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

    private List<IHEReport> InitializeIHEReports()
    {
        // Create sample IHE reports for students who have completed their programs
        // Mix of different statuses: Submitted, Under Review, Approved, Revisions Requested
        var reports = new List<IHEReport>
        {
            // Submitted reports awaiting review
            new IHEReport
            {
                Id = 1,
                StudentId = 1,
                ApplicationId = 1,
                CompletionStatus = "COMPLETED",
                CompletionDate = new DateTime(2025, 10, 15),
                GrantProgramHours = 520,
                Met500Hours = true,
                CredentialProgramHours = 650,
                Met600Hours = true,
                SwitchedToIntern = false,
                SubmittedDate = new DateTime(2025, 11, 5),
                SubmittedBy = "Jane Coordinator",
                SubmittedByEmail = "coordinator@fullerton.edu",
                CreatedAt = new DateTime(2025, 11, 5),
                Status = "SUBMITTED"
            },
            new IHEReport
            {
                Id = 2,
                StudentId = 2,
                ApplicationId = 1,
                CompletionStatus = "COMPLETED",
                CompletionDate = new DateTime(2025, 10, 20),
                GrantProgramHours = 500,
                Met500Hours = true,
                CredentialProgramHours = 600,
                Met600Hours = true,
                SwitchedToIntern = false,
                SubmittedDate = new DateTime(2025, 11, 6),
                SubmittedBy = "Jane Coordinator",
                SubmittedByEmail = "coordinator@fullerton.edu",
                CreatedAt = new DateTime(2025, 11, 6),
                Status = "SUBMITTED"
            },
            // Under review
            new IHEReport
            {
                Id = 3,
                StudentId = 4,
                ApplicationId = 2,
                CompletionStatus = "COMPLETED",
                CompletionDate = new DateTime(2025, 10, 10),
                GrantProgramHours = 550,
                Met500Hours = true,
                CredentialProgramHours = 620,
                Met600Hours = true,
                SwitchedToIntern = false,
                SubmittedDate = new DateTime(2025, 11, 1),
                SubmittedBy = "Bob Grant",
                SubmittedByEmail = "grants@ucla.edu",
                CreatedAt = new DateTime(2025, 11, 1),
                Status = "UNDER_REVIEW",
                ReviewedBy = "Sarah CTC",
                ReviewedDate = new DateTime(2025, 11, 3)
            },
            // Approved reports
            new IHEReport
            {
                Id = 4,
                StudentId = 6,
                ApplicationId = 1,
                CompletionStatus = "COMPLETED",
                CompletionDate = new DateTime(2025, 9, 30),
                GrantProgramHours = 510,
                Met500Hours = true,
                CredentialProgramHours = 605,
                Met600Hours = true,
                SwitchedToIntern = false,
                SubmittedDate = new DateTime(2025, 10, 15),
                SubmittedBy = "Jane Coordinator",
                SubmittedByEmail = "coordinator@fullerton.edu",
                CreatedAt = new DateTime(2025, 10, 15),
                Status = "APPROVED",
                ReviewedBy = "Sarah CTC",
                ReviewedDate = new DateTime(2025, 10, 20),
                ApprovedDate = new DateTime(2025, 10, 20)
            },
            new IHEReport
            {
                Id = 5,
                StudentId = 7,
                ApplicationId = 1,
                CompletionStatus = "COMPLETED",
                CompletionDate = new DateTime(2025, 9, 28),
                GrantProgramHours = 530,
                Met500Hours = true,
                CredentialProgramHours = 615,
                Met600Hours = true,
                SwitchedToIntern = false,
                SubmittedDate = new DateTime(2025, 10, 16),
                SubmittedBy = "Jane Coordinator",
                SubmittedByEmail = "coordinator@fullerton.edu",
                CreatedAt = new DateTime(2025, 10, 16),
                Status = "APPROVED",
                ReviewedBy = "Mike CTC",
                ReviewedDate = new DateTime(2025, 10, 21),
                ApprovedDate = new DateTime(2025, 10, 21)
            },
            // Revisions requested
            new IHEReport
            {
                Id = 6,
                StudentId = 3,
                ApplicationId = 1,
                CompletionStatus = "COMPLETED",
                CompletionDate = new DateTime(2025, 10, 25),
                GrantProgramHours = 480,
                Met500Hours = false,
                GrantProgramHoursNotes = "Please clarify the hours breakdown",
                CredentialProgramHours = 600,
                Met600Hours = true,
                SwitchedToIntern = true,
                InternSwitchDate = new DateTime(2025, 9, 1),
                SubmittedDate = new DateTime(2025, 11, 2),
                SubmittedBy = "Jane Coordinator",
                SubmittedByEmail = "coordinator@fullerton.edu",
                CreatedAt = new DateTime(2025, 11, 2),
                Status = "REVISIONS_REQUESTED",
                ReviewedBy = "Sarah CTC",
                ReviewedDate = new DateTime(2025, 11, 4),
                RevisionNotes = "Grant program hours appear to be under 500. Please verify the total hours and provide documentation.",
                RevisionCount = 1
            },
            // Denied completion
            new IHEReport
            {
                Id = 7,
                StudentId = 5,
                ApplicationId = 2,
                CompletionStatus = "DENIED",
                DenialReason = "Did not complete clinical practice requirements",
                GrantProgramHours = 350,
                Met500Hours = false,
                CredentialProgramHours = 450,
                Met600Hours = false,
                SwitchedToIntern = false,
                AdditionalNotes = "Student withdrew from program in October 2025",
                SubmittedDate = new DateTime(2025, 10, 30),
                SubmittedBy = "Bob Grant",
                SubmittedByEmail = "grants@ucla.edu",
                CreatedAt = new DateTime(2025, 10, 30),
                Status = "APPROVED",
                ReviewedBy = "Mike CTC",
                ReviewedDate = new DateTime(2025, 11, 1),
                ApprovedDate = new DateTime(2025, 11, 1)
            }
        };

        return reports;
    }

    private List<LEAReport> InitializeLEAReports()
    {
        // Create sample LEA reports for students who have been paid
        // Mix of different statuses: Submitted, Under Review, Approved, Revisions Requested
        var reports = new List<LEAReport>
        {
            // Submitted reports awaiting review
            new LEAReport
            {
                Id = 1,
                StudentId = 1,
                ApplicationId = 1,
                PaymentCategory = "STIPEND",
                PaymentSchedule = "LUMP_SUM",
                ActualPaymentAmount = 20000,
                FirstPaymentDate = new DateTime(2025, 11, 1),
                FinalPaymentDate = new DateTime(2025, 11, 1),
                HiredInDistrict = true,
                EmploymentStatus = "FULL_TIME",
                HireDate = new DateTime(2025, 11, 15),
                JobTitle = "Elementary Teacher",
                SchoolSite = "Washington Elementary School",
                SubmittedDate = new DateTime(2025, 11, 7),
                SubmittedBy = "Lisa LEA",
                SubmittedByEmail = "fiscal@lausd.net",
                CreatedAt = new DateTime(2025, 11, 7),
                Status = "SUBMITTED"
            },
            new LEAReport
            {
                Id = 2,
                StudentId = 2,
                ApplicationId = 1,
                PaymentCategory = "STIPEND",
                PaymentSchedule = "MONTHLY",
                PaymentScheduleDetails = "Paid over 10 months, September through June",
                ActualPaymentAmount = 20000,
                FirstPaymentDate = new DateTime(2025, 9, 30),
                FinalPaymentDate = new DateTime(2026, 6, 30),
                HiredInDistrict = true,
                EmploymentStatus = "FULL_TIME",
                HireDate = new DateTime(2025, 8, 15),
                JobTitle = "Math Teacher",
                SchoolSite = "Lincoln High School",
                SubmittedDate = new DateTime(2025, 11, 7),
                SubmittedBy = "Lisa LEA",
                SubmittedByEmail = "fiscal@lausd.net",
                CreatedAt = new DateTime(2025, 11, 7),
                Status = "SUBMITTED"
            },
            // Under review
            new LEAReport
            {
                Id = 3,
                StudentId = 4,
                ApplicationId = 2,
                PaymentCategory = "STIPEND",
                PaymentSchedule = "LUMP_SUM",
                ActualPaymentAmount = 20000,
                FirstPaymentDate = new DateTime(2025, 10, 15),
                FinalPaymentDate = new DateTime(2025, 10, 15),
                HiredInDistrict = true,
                EmploymentStatus = "FULL_TIME",
                HireDate = new DateTime(2025, 11, 1),
                JobTitle = "English Teacher",
                SchoolSite = "Roosevelt Middle School",
                SubmittedDate = new DateTime(2025, 11, 2),
                SubmittedBy = "Tom District",
                SubmittedByEmail = "grants@sandi.net",
                CreatedAt = new DateTime(2025, 11, 2),
                Status = "UNDER_REVIEW",
                ReviewedBy = "Sarah CTC",
                ReviewedDate = new DateTime(2025, 11, 3)
            },
            // Approved reports
            new LEAReport
            {
                Id = 4,
                StudentId = 6,
                ApplicationId = 1,
                PaymentCategory = "STIPEND",
                PaymentSchedule = "LUMP_SUM",
                ActualPaymentAmount = 20000,
                FirstPaymentDate = new DateTime(2025, 10, 1),
                FinalPaymentDate = new DateTime(2025, 10, 1),
                HiredInDistrict = true,
                EmploymentStatus = "FULL_TIME",
                HireDate = new DateTime(2025, 10, 15),
                JobTitle = "Special Education Teacher",
                SchoolSite = "Jefferson Elementary",
                SubmittedDate = new DateTime(2025, 10, 20),
                SubmittedBy = "Lisa LEA",
                SubmittedByEmail = "fiscal@lausd.net",
                CreatedAt = new DateTime(2025, 10, 20),
                Status = "APPROVED",
                ReviewedBy = "Sarah CTC",
                ReviewedDate = new DateTime(2025, 10, 22),
                ApprovedDate = new DateTime(2025, 10, 22)
            },
            new LEAReport
            {
                Id = 5,
                StudentId = 7,
                ApplicationId = 1,
                PaymentCategory = "STIPEND",
                PaymentSchedule = "LUMP_SUM",
                ActualPaymentAmount = 20000,
                FirstPaymentDate = new DateTime(2025, 10, 1),
                FinalPaymentDate = new DateTime(2025, 10, 1),
                HiredInDistrict = false,
                EmploymentStatus = "FULL_TIME",
                HireDate = new DateTime(2025, 10, 1),
                JobTitle = "Science Teacher",
                SchoolSite = "Central High School",
                PaymentNotes = "Candidate hired by neighboring district but received stipend from partner LEA",
                SubmittedDate = new DateTime(2025, 10, 21),
                SubmittedBy = "Lisa LEA",
                SubmittedByEmail = "fiscal@lausd.net",
                CreatedAt = new DateTime(2025, 10, 21),
                Status = "APPROVED",
                ReviewedBy = "Mike CTC",
                ReviewedDate = new DateTime(2025, 10, 23),
                ApprovedDate = new DateTime(2025, 10, 23)
            },
            // Revisions requested
            new LEAReport
            {
                Id = 6,
                StudentId = 3,
                ApplicationId = 1,
                PaymentCategory = "STIPEND",
                PaymentSchedule = "LUMP_SUM",
                ActualPaymentAmount = 18000,
                FirstPaymentDate = new DateTime(2025, 11, 1),
                FinalPaymentDate = new DateTime(2025, 11, 1),
                HiredInDistrict = true,
                EmploymentStatus = "FULL_TIME",
                HireDate = new DateTime(2025, 11, 10),
                JobTitle = "Special Education Teacher",
                SchoolSite = "Adams Elementary",
                SubmittedDate = new DateTime(2025, 11, 5),
                SubmittedBy = "Lisa LEA",
                SubmittedByEmail = "fiscal@lausd.net",
                CreatedAt = new DateTime(2025, 11, 5),
                Status = "REVISIONS_REQUESTED",
                ReviewedBy = "Sarah CTC",
                ReviewedDate = new DateTime(2025, 11, 6),
                RevisionNotes = "Payment amount ($18,000) does not match approved award amount ($20,000). Please clarify or provide explanation.",
                RevisionCount = 1
            },
            // Not hired scenario
            new LEAReport
            {
                Id = 7,
                StudentId = 5,
                ApplicationId = 2,
                PaymentCategory = "STIPEND",
                PaymentSchedule = "LUMP_SUM",
                ActualPaymentAmount = 20000,
                FirstPaymentDate = new DateTime(2025, 10, 20),
                FinalPaymentDate = new DateTime(2025, 10, 20),
                HiredInDistrict = false,
                EmploymentStatus = "NOT_HIRED",
                PaymentNotes = "Candidate completed program and received stipend but was not hired by district. Currently seeking employment.",
                SubmittedDate = new DateTime(2025, 11, 1),
                SubmittedBy = "Tom District",
                SubmittedByEmail = "grants@sandi.net",
                CreatedAt = new DateTime(2025, 11, 1),
                Status = "APPROVED",
                ReviewedBy = "Mike CTC",
                ReviewedDate = new DateTime(2025, 11, 2),
                ApprovedDate = new DateTime(2025, 11, 2)
            }
        };

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

    public IHEReport? GetIHEReport(int id)
    {
        return _iheReports.FirstOrDefault(r => r.Id == id);
    }

    public List<IHEReport> GetIHEReports()
    {
        return _iheReports;
    }

    public List<IHEReport> GetIHEReportsByStatus(string status)
    {
        return _iheReports.Where(r => r.Status == status).ToList();
    }

    public List<IHEReport> GetIHEReportsByStudent(int studentId)
    {
        return _iheReports.Where(r => r.StudentId == studentId).ToList();
    }

    public List<IHEReport> GetIHEReportsByApplication(int applicationId)
    {
        return _iheReports.Where(r => r.ApplicationId == applicationId).ToList();
    }

    public LEAReport? GetLEAReport(int id)
    {
        return _leaReports.FirstOrDefault(r => r.Id == id);
    }

    public List<LEAReport> GetLEAReports()
    {
        return _leaReports;
    }

    public List<LEAReport> GetLEAReportsByStatus(string status)
    {
        return _leaReports.Where(r => r.Status == status).ToList();
    }

    public List<LEAReport> GetLEAReportsByStudent(int studentId)
    {
        return _leaReports.Where(r => r.StudentId == studentId).ToList();
    }

    public List<LEAReport> GetLEAReportsByApplication(int applicationId)
    {
        return _leaReports.Where(r => r.ApplicationId == applicationId).ToList();
    }
}
