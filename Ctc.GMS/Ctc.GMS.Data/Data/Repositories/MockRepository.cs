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
    private readonly List<Payment> _payments;
    private readonly List<LEAReport> _leaReports;
    private readonly List<IHEReport> _iheReports;
    private readonly List<ReportingPeriod> _reportingPeriods;

    public MockRepository()
    {
        _grantCycles = InitializeGrantCycles();
        _organizations = InitializeOrganizations();
        _applications = InitializeApplications();
        _payments = InitializePayments();
        _leaReports = InitializeLEAReports();
        _iheReports = InitializeIHEReports();
        _reportingPeriods = InitializeReportingPeriods();
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
                AwardAmount = 10000,
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
                AwardAmount = 10000,
                GAAStatus = "PAYMENT_COMPLETED",
                CreatedAt = new DateTime(2025, 8, 17),
                SubmittedAt = new DateTime(2025, 9, 2),
                ApprovedAt = new DateTime(2025, 9, 15, 10, 30, 0)
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
                AwardAmount = 10000,
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
                AwardAmount = 10000,
                CreatedAt = new DateTime(2025, 8, 21),
                SubmittedAt = new DateTime(2025, 9, 4),
                ApprovedAt = new DateTime(2025, 9, 20, 14, 15, 0),
                GAAStatus = "PAYMENT_COMPLETED"
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
                AwardAmount = 10000,
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
                AwardAmount = 10000,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i % 30),
                SubmittedAt = new DateTime(2025, 9, 15).AddDays(i % 30),
                ApprovedAt = new DateTime(2025, 10, 1).AddDays(i % 30).AddHours(i % 24),
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
                AwardAmount = 10000,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i % 30),
                SubmittedAt = new DateTime(2025, 9, 15).AddDays(i % 30),
                ApprovedAt = new DateTime(2025, 10, 1).AddDays(i % 30).AddHours((i + 8) % 24),
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
                AwardAmount = 10000,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i % 30),
                SubmittedAt = new DateTime(2025, 9, 15).AddDays(i % 30),
                ApprovedAt = new DateTime(2025, 10, 1).AddDays(i % 30).AddHours((i + 16) % 24),
                GAAStatus = i % 3 == 0 ? "PAYMENT_COMPLETED" : i % 3 == 1 ? "GAA_SIGNED" : null
            });
        }

        return applications;
    }

    private List<Payment> InitializePayments()
    {
        var payments = new List<Payment>();

        // Create payments for approved students (showing various scenarios)
        // Student 2 (Maria Garcia) - Has both reports (COMPLIANT)
        payments.Add(new Payment
        {
            Id = 1,
            StudentId = 2,
            ApplicationId = 1,
            PONumber = "PO-2025-001",
            InvoiceNumber = "INV-001",
            AuthorizedAmount = 10000,
            AuthorizationDate = new DateTime(2025, 10, 1),
            LEAName = "Los Angeles Unified School District",
            LEAAddress = "333 S. Beaudry Ave, Los Angeles, CA 90017",
            WarrantNumber = "W-2025-001",
            WarrantDate = new DateTime(2025, 10, 15),
            ActualPaymentAmount = 10000,
            ActualPaymentDate = new DateTime(2025, 10, 15),
            Status = "COMPLETED",
            CreatedAt = new DateTime(2025, 10, 1),
            CreatedBy = "fiscal@ctc.ca.gov",
            LastModified = new DateTime(2025, 10, 15),
            ModifiedBy = "fiscal@ctc.ca.gov"
        });

        // Student 4 (Sarah Johnson) - Has IHE report but missing LEA report (WARNING)
        payments.Add(new Payment
        {
            Id = 2,
            StudentId = 4,
            ApplicationId = 2,
            PONumber = "PO-2025-002",
            InvoiceNumber = "INV-002",
            AuthorizedAmount = 10000,
            AuthorizationDate = new DateTime(2025, 9, 15),
            LEAName = "San Diego Unified School District",
            LEAAddress = "4100 Normal St, San Diego, CA 92103",
            WarrantNumber = "W-2025-002",
            WarrantDate = new DateTime(2025, 10, 1),
            ActualPaymentAmount = 10000,
            ActualPaymentDate = new DateTime(2025, 10, 1),
            Status = "COMPLETED",
            CreatedAt = new DateTime(2025, 9, 15),
            CreatedBy = "fiscal@ctc.ca.gov",
            LastModified = new DateTime(2025, 10, 1),
            ModifiedBy = "fiscal@ctc.ca.gov"
        });

        // Add 20 more payments with varying report compliance
        for (int i = 0; i < 20; i++)
        {
            payments.Add(new Payment
            {
                Id = 3 + i,
                StudentId = 100 + i,
                ApplicationId = (i % 3) + 1,
                PONumber = $"PO-2025-{(003 + i):D3}",
                InvoiceNumber = $"INV-{(003 + i):D3}",
                AuthorizedAmount = 10000,
                AuthorizationDate = new DateTime(2025, 9, 1).AddDays(i),
                LEAName = i % 3 == 0 ? "Los Angeles Unified School District" :
                         i % 3 == 1 ? "San Diego Unified School District" :
                         "Fresno Unified School District",
                LEAAddress = i % 3 == 0 ? "333 S. Beaudry Ave, Los Angeles, CA 90017" :
                            i % 3 == 1 ? "4100 Normal St, San Diego, CA 92103" :
                            "2309 Tulare St, Fresno, CA 93721",
                WarrantNumber = $"W-2025-{(003 + i):D3}",
                WarrantDate = new DateTime(2025, 9, 15).AddDays(i),
                ActualPaymentAmount = 10000,
                ActualPaymentDate = new DateTime(2025, 9, 15).AddDays(i),
                Status = "COMPLETED",
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i),
                CreatedBy = "fiscal@ctc.ca.gov",
                LastModified = new DateTime(2025, 9, 15).AddDays(i),
                ModifiedBy = "fiscal@ctc.ca.gov"
            });
        }

        return payments;
    }

    private List<LEAReport> InitializeLEAReports()
    {
        var reports = new List<LEAReport>();

        // Report for Student 2 (Maria Garcia) - Payment 1 - COMPLIANT
        reports.Add(new LEAReport
        {
            Id = 1,
            StudentId = 2,
            ApplicationId = 1,
            PaymentId = 1,
            PaymentCategorization = "Categorized as student teacher stipend",
            PaymentCategory = "STIPEND",
            PaymentSchedule = "LUMP_SUM",
            ActualPaymentAmount = 10000,
            FirstPaymentDate = new DateTime(2025, 10, 15),
            FinalPaymentDate = new DateTime(2025, 10, 15),
            PaymentNotes = "Full payment disbursed as lump sum",
            HiredInDistrict = true,
            EmploymentStatus = "FULL_TIME",
            HireDate = new DateTime(2025, 8, 1),
            JobTitle = "Elementary School Teacher",
            SchoolSite = "Washington Elementary School",
            SubmittedDate = new DateTime(2025, 11, 1),
            SubmittedBy = "Jane Smith",
            SubmittedByEmail = "jsmith@lausd.net",
            CreatedAt = new DateTime(2025, 11, 1),
            LastModified = new DateTime(2025, 11, 1)
        });

        // Add reports for ~60% of other payments (showing partial compliance)
        for (int i = 0; i < 12; i++)
        {
            reports.Add(new LEAReport
            {
                Id = 2 + i,
                StudentId = 100 + i,
                ApplicationId = (i % 3) + 1,
                PaymentId = 3 + i,
                PaymentCategorization = "Categorized as student teacher stipend",
                PaymentCategory = "STIPEND",
                PaymentSchedule = i % 2 == 0 ? "LUMP_SUM" : "MONTHLY",
                ActualPaymentAmount = 10000,
                FirstPaymentDate = new DateTime(2025, 9, 15).AddDays(i),
                FinalPaymentDate = new DateTime(2025, 10, 15).AddDays(i),
                HiredInDistrict = i % 3 != 0,
                EmploymentStatus = i % 3 == 0 ? "NOT_HIRED" : i % 3 == 1 ? "FULL_TIME" : "PART_TIME",
                HireDate = i % 3 != 0 ? new DateTime(2025, 8, 1).AddDays(i) : null,
                JobTitle = i % 3 == 1 ? "Middle School Teacher" : i % 3 == 2 ? "High School Teacher" : "",
                SchoolSite = i % 3 != 0 ? $"School Site {i}" : "",
                SubmittedDate = new DateTime(2025, 10, 20).AddDays(i),
                SubmittedBy = "LEA Coordinator",
                SubmittedByEmail = $"coordinator{i}@district.edu",
                CreatedAt = new DateTime(2025, 10, 20).AddDays(i),
                LastModified = new DateTime(2025, 10, 20).AddDays(i)
            });
        }

        return reports;
    }

    private List<IHEReport> InitializeIHEReports()
    {
        var reports = new List<IHEReport>();

        // Report for Student 2 (Maria Garcia) - Payment 1 - COMPLIANT
        reports.Add(new IHEReport
        {
            Id = 1,
            StudentId = 2,
            ApplicationId = 1,
            PaymentId = 1,
            CompletionStatus = "COMPLETED",
            CompletionDate = new DateTime(2025, 6, 15),
            SwitchedToIntern = false,
            GrantProgramHours = 550,
            Met500Hours = true,
            CredentialProgramHours = 650,
            Met600Hours = true,
            SubmittedDate = new DateTime(2025, 10, 25),
            SubmittedBy = "Dr. John Davis",
            SubmittedByEmail = "jdavis@fullerton.edu",
            CreatedAt = new DateTime(2025, 10, 25),
            LastModified = new DateTime(2025, 10, 25)
        });

        // Report for Student 4 (Sarah Johnson) - Payment 2 - PARTIAL COMPLIANCE
        reports.Add(new IHEReport
        {
            Id = 2,
            StudentId = 4,
            ApplicationId = 2,
            PaymentId = 2,
            CompletionStatus = "COMPLETED",
            CompletionDate = new DateTime(2025, 6, 20),
            SwitchedToIntern = false,
            GrantProgramHours = 520,
            Met500Hours = true,
            CredentialProgramHours = 630,
            Met600Hours = true,
            SubmittedDate = new DateTime(2025, 10, 28),
            SubmittedBy = "Dr. Sarah Williams",
            SubmittedByEmail = "swilliams@ucla.edu",
            CreatedAt = new DateTime(2025, 10, 28),
            LastModified = new DateTime(2025, 10, 28)
        });

        // Add reports for ~70% of other payments (showing higher IHE compliance than LEA)
        for (int i = 0; i < 15; i++)
        {
            reports.Add(new IHEReport
            {
                Id = 3 + i,
                StudentId = 100 + i,
                ApplicationId = (i % 3) + 1,
                PaymentId = 3 + i,
                CompletionStatus = i % 5 == 0 ? "IN_PROGRESS" : i % 7 == 0 ? "DENIED" : "COMPLETED",
                CompletionDate = i % 5 != 0 ? new DateTime(2025, 6, 15).AddDays(i) : null,
                DenialReason = i % 7 == 0 ? "Did not complete program requirements" : "",
                SwitchedToIntern = i % 4 == 0,
                InternSwitchDate = i % 4 == 0 ? new DateTime(2025, 3, 1).AddDays(i) : null,
                GrantProgramHours = 480 + (i * 10),
                Met500Hours = (480 + (i * 10)) >= 500,
                CredentialProgramHours = 580 + (i * 10),
                Met600Hours = (580 + (i * 10)) >= 600,
                SubmittedDate = new DateTime(2025, 10, 15).AddDays(i),
                SubmittedBy = "IHE Coordinator",
                SubmittedByEmail = $"coordinator{i}@university.edu",
                CreatedAt = new DateTime(2025, 10, 15).AddDays(i),
                LastModified = new DateTime(2025, 10, 15).AddDays(i)
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

    public List<Payment> GetPayments()
    {
        return _payments;
    }

    public Payment? GetPayment(int id)
    {
        return _payments.FirstOrDefault(p => p.Id == id);
    }

    public List<LEAReport> GetLEAReports()
    {
        return _leaReports;
    }

    public LEAReport? GetLEAReport(int id)
    {
        return _leaReports.FirstOrDefault(r => r.Id == id);
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

    public LEAReport? GetLEAReportByPaymentId(int paymentId)
    {
        return _leaReports.FirstOrDefault(r => r.PaymentId == paymentId);
    }

    public List<IHEReport> GetIHEReports()
    {
        return _iheReports;
    }

    public IHEReport? GetIHEReport(int id)
    {
        return _iheReports.FirstOrDefault(r => r.Id == id);
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

    public IHEReport? GetIHEReportByPaymentId(int paymentId)
    {
        return _iheReports.FirstOrDefault(r => r.PaymentId == paymentId);
    }

    // Reporting Periods
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
}
