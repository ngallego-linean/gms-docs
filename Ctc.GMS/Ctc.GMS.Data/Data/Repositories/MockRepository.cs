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
                Name = "Student Teacher Stipend Program - FY 2025-26",
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
                Status = "DISBURSEMENT",  // Most students in disbursement/reporting phase
                LastActionDate = new DateTime(2025, 10, 15),
                LastActionBy = "CTC Grants Team",
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
                Status = "DISBURSEMENT",  // Mixed disbursement and reporting
                LastActionDate = new DateTime(2025, 10, 20),
                LastActionBy = "CTC Grants Team",
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
                LEA = _organizations.First(o => o.Id == 8),  // Long Beach Unified - diversify GAA page display
                Status = "CTC_REVIEW",  // Some students still under review
                LastActionDate = new DateTime(2025, 10, 28),
                LastActionBy = "CTC Grants Team",
                CreatedAt = new DateTime(2025, 9, 1, 11, 0, 0),
                CreatedBy = "teacher.ed@sdsu.edu",
                LastModified = new DateTime(2025, 10, 28, 10, 15, 0),
                Students = new List<Student>()
            },
            new Application
            {
                Id = 4,
                GrantCycleId = 1,
                IHE = _organizations.First(o => o.Id == 2), // UCLA
                LEA = _organizations.First(o => o.Id == 5), // LAUSD
                Status = "LEA_REVIEW",  // LEA reviewing IHE submission
                LastActionDate = new DateTime(2025, 11, 5),
                LastActionBy = "grants@ucla.edu",
                CreatedAt = new DateTime(2025, 11, 1, 9, 0, 0),
                CreatedBy = "grants@ucla.edu",
                LastModified = new DateTime(2025, 11, 5, 14, 20, 0),
                Students = new List<Student>()
            },
            new Application
            {
                Id = 5,
                GrantCycleId = 1,
                IHE = _organizations.First(o => o.Id == 4), // UCI
                LEA = _organizations.First(o => o.Id == 5), // LAUSD
                Status = "LEA_REVIEW",  // LEA reviewing IHE submission
                LastActionDate = new DateTime(2025, 11, 6),
                LastActionBy = "credentialing@uci.edu",
                CreatedAt = new DateTime(2025, 11, 3, 14, 0, 0),
                CreatedBy = "credentialing@uci.edu",
                LastModified = new DateTime(2025, 11, 6, 9, 10, 0),
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
                Status = "CTC_REVIEWING",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 10, 15),
                LastActionBy = "CTC Grants Team",
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
                Status = "PAYMENT_COMPLETE",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 10, 28),
                LastActionBy = "CTC Fiscal Team",
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
                Status = "GAA_SIGNED",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 10, 20),
                LastActionBy = "LEA Coordinator",
                CreatedAt = new DateTime(2025, 8, 18),
                SubmittedAt = new DateTime(2025, 9, 3),
                ApprovedAt = new DateTime(2025, 9, 18, 14, 15, 0)
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
                Status = "REPORTING_PENDING",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 1),
                LastActionBy = "CTC Fiscal Team",
                CreatedAt = new DateTime(2025, 8, 21),
                SubmittedAt = new DateTime(2025, 9, 4),
                ApprovedAt = new DateTime(2025, 9, 20, 14, 15, 0)
            },
            new Student
            {
                Id = 5,
                ApplicationId = 2,
                FirstName = "Michael",
                LastName = "Brown",
                SEID = "56789012",
                CredentialArea = "Multiple Subject",
                Status = "INVOICE_GENERATED",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 10, 25),
                LastActionBy = "CTC Fiscal Team",
                CreatedAt = new DateTime(2025, 8, 22),
                SubmittedAt = new DateTime(2025, 9, 5),
                ApprovedAt = new DateTime(2025, 9, 22, 11, 30, 0)
            }
        });

        // Add more students to show budget utilization and workflow distribution
        // Total budget: $25M, distribute students across lifecycle stages
        var additionalStudentId = 6;
        for (int i = 0; i < 390; i++)
        {
            // Distribute students across workflow stages:
            // 5% CTC_REVIEWING, 10% CTC_APPROVED/GAA stages, 40% disbursement stages, 35% reporting, 10% complete
            string status;
            DateTime? lastActionDate;
            string lastActionBy;

            if (i % 20 == 0)  // 5% - CTC Review
            {
                status = "CTC_REVIEWING";
                lastActionDate = new DateTime(2025, 10, 1).AddDays(i % 30);
                lastActionBy = "CTC Grants Team";
            }
            else if (i % 10 < 2)  // 10% - GAA stages
            {
                status = i % 2 == 0 ? "GAA_PENDING" : "GAA_GENERATED";
                lastActionDate = new DateTime(2025, 10, 5).AddDays(i % 30);
                lastActionBy = "CTC Grants Team";
            }
            else if (i % 10 < 6)  // 40% - Disbursement stages
            {
                status = (i % 4) switch {
                    0 => "GAA_SIGNED",
                    1 => "INVOICE_GENERATED",
                    2 => "PAYMENT_AUTHORIZED",
                    _ => "WARRANT_ISSUED"
                };
                lastActionDate = new DateTime(2025, 10, 10).AddDays(i % 30);
                lastActionBy = i % 2 == 0 ? "CTC Fiscal Team" : "LEA Coordinator";
            }
            else if (i % 10 < 9)  // 30% - Payment complete / Reporting
            {
                status = i % 3 == 0 ? "PAYMENT_COMPLETE" : "REPORTING_PENDING";
                lastActionDate = new DateTime(2025, 10, 20).AddDays(i % 30);
                lastActionBy = "CTC Fiscal Team";
            }
            else  // 10% - Reports submitted/approved
            {
                status = i % 2 == 0 ? "REPORTING_COMPLETE" : "REPORTS_APPROVED";
                lastActionDate = new DateTime(2025, 11, 1).AddDays(i % 10);
                lastActionBy = i % 2 == 0 ? "IHE Coordinator" : "CTC Grants Team";
            }

            applications[0].Students.Add(new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 1,
                FirstName = $"Student{i}",
                LastName = $"Test{i}",
                SEID = $"{10000000 + i}",
                CredentialArea = i % 3 == 0 ? "Multiple Subject" : i % 3 == 1 ? "Single Subject" : "Education Specialist",
                Status = status,
                AwardAmount = 10000,
                LastActionDate = lastActionDate,
                LastActionBy = lastActionBy,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i % 30),
                SubmittedAt = new DateTime(2025, 9, 15).AddDays(i % 30),
                ApprovedAt = status != "CTC_REVIEWING" ? new DateTime(2025, 10, 1).AddDays(i % 30).AddHours(i % 24) : null
            });
        }

        for (int i = 0; i < 390; i++)
        {
            // Similar distribution for application 2
            string status;
            DateTime? lastActionDate;
            string lastActionBy;

            if (i % 20 == 0)  // 5% - CTC Review
            {
                status = "CTC_REVIEWING";
                lastActionDate = new DateTime(2025, 10, 2).AddDays(i % 30);
                lastActionBy = "CTC Grants Team";
            }
            else if (i % 10 < 2)  // 10% - GAA stages
            {
                status = i % 2 == 0 ? "GAA_PENDING" : "GAA_SIGNED";
                lastActionDate = new DateTime(2025, 10, 8).AddDays(i % 30);
                lastActionBy = i % 2 == 0 ? "CTC Grants Team" : "LEA Coordinator";
            }
            else if (i % 10 < 6)  // 40% - Disbursement stages
            {
                status = (i % 4) switch {
                    0 => "GAA_SIGNED",
                    1 => "INVOICE_GENERATED",
                    2 => "WARRANT_ISSUED",
                    _ => "PAYMENT_AUTHORIZED"
                };
                lastActionDate = new DateTime(2025, 10, 15).AddDays(i % 30);
                lastActionBy = "CTC Fiscal Team";
            }
            else if (i % 10 < 9)  // 30% - Payment complete / Reporting
            {
                status = i % 3 == 0 ? "PAYMENT_COMPLETE" : "REPORTING_PENDING";
                lastActionDate = new DateTime(2025, 10, 25).AddDays(i % 30);
                lastActionBy = "CTC Fiscal Team";
            }
            else  // 10% - Reports submitted/approved
            {
                status = i % 2 == 0 ? "REPORTING_PARTIAL" : "REPORTING_COMPLETE";
                lastActionDate = new DateTime(2025, 11, 2).AddDays(i % 10);
                lastActionBy = "LEA Coordinator";
            }

            applications[1].Students.Add(new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 2,
                FirstName = $"Student{i + 390}",
                LastName = $"Test{i + 390}",
                SEID = $"{10000390 + i}",
                CredentialArea = i % 3 == 0 ? "Multiple Subject" : i % 3 == 1 ? "Single Subject" : "Education Specialist",
                Status = status,
                AwardAmount = 10000,
                LastActionDate = lastActionDate,
                LastActionBy = lastActionBy,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i % 30),
                SubmittedAt = new DateTime(2025, 9, 15).AddDays(i % 30),
                ApprovedAt = status != "CTC_REVIEWING" ? new DateTime(2025, 10, 1).AddDays(i % 30).AddHours((i + 8) % 24) : null
            });
        }

        for (int i = 0; i < 390; i++)
        {
            // Application 3 has more students in earlier stages (CTC_REVIEW application status)
            string status;
            DateTime? lastActionDate;
            string lastActionBy;

            if (i % 5 == 0)  // 20% - CTC Review
            {
                status = i % 2 == 0 ? "CTC_REVIEWING" : "CTC_SUBMITTED";
                lastActionDate = new DateTime(2025, 10, 20).AddDays(i % 30);
                lastActionBy = i % 2 == 0 ? "CTC Grants Team" : "LEA Coordinator";
            }
            else if (i % 10 < 3)  // 10% - Just approved / GAA stages
            {
                status = (i % 3) switch {
                    0 => "CTC_APPROVED",
                    1 => "GAA_PENDING",
                    _ => "GAA_GENERATED"
                };
                lastActionDate = new DateTime(2025, 10, 25).AddDays(i % 30);
                lastActionBy = "CTC Grants Team";
            }
            else if (i % 10 < 7)  // 40% - Disbursement stages
            {
                status = (i % 4) switch {
                    0 => "GAA_SIGNED",
                    1 => "INVOICE_GENERATED",
                    2 => "PAYMENT_AUTHORIZED",
                    _ => "WARRANT_ISSUED"
                };
                lastActionDate = new DateTime(2025, 10, 28).AddDays(i % 30);
                lastActionBy = i % 2 == 0 ? "CTC Fiscal Team" : "LEA Coordinator";
            }
            else  // 30% - Payment complete / Early reporting
            {
                status = (i % 3) switch {
                    0 => "PAYMENT_COMPLETE",
                    1 => "REPORTING_PENDING",
                    _ => "REPORTING_PARTIAL"
                };
                lastActionDate = new DateTime(2025, 11, 5).AddDays(i % 10);
                lastActionBy = "CTC Fiscal Team";
            }

            applications[2].Students.Add(new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 3,
                FirstName = $"Student{i + 780}",
                LastName = $"Test{i + 780}",
                SEID = $"{10000780 + i}",
                CredentialArea = i % 3 == 0 ? "Multiple Subject" : i % 3 == 1 ? "Single Subject" : "Education Specialist",
                Status = status,
                AwardAmount = 10000,
                LastActionDate = lastActionDate,
                LastActionBy = lastActionBy,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i % 30),
                SubmittedAt = new DateTime(2025, 9, 15).AddDays(i % 30),
                ApprovedAt = (status != "CTC_REVIEWING" && status != "CTC_SUBMITTED") ?
                    new DateTime(2025, 10, 1).AddDays(i % 30).AddHours((i + 16) % 24) : null
            });
        }

        // Add PENDING_LEA students to Application 4 (UCLA -> LAUSD) - Candidates awaiting review
        applications[3].Students.AddRange(new[]
        {
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 4,
                FirstName = "Jennifer",
                LastName = "Martinez",
                SEID = "87654321",
                CredentialArea = "Multiple Subject",
                Status = "PENDING_LEA",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 5),
                LastActionBy = "grants@ucla.edu",
                CreatedAt = new DateTime(2025, 11, 1, 9, 15, 0),
                SubmittedAt = new DateTime(2025, 11, 5, 14, 20, 0)
            },
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 4,
                FirstName = "Robert",
                LastName = "Thompson",
                SEID = "87654322",
                CredentialArea = "Single Subject - Science",
                Status = "PENDING_LEA",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 5),
                LastActionBy = "grants@ucla.edu",
                CreatedAt = new DateTime(2025, 11, 1, 10, 30, 0),
                SubmittedAt = new DateTime(2025, 11, 5, 14, 20, 0)
            },
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 4,
                FirstName = "Amanda",
                LastName = "Lee",
                SEID = "87654323",
                CredentialArea = "Education Specialist - Mild/Moderate",
                Status = "PENDING_LEA",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 5),
                LastActionBy = "grants@ucla.edu",
                CreatedAt = new DateTime(2025, 11, 2, 8, 45, 0),
                SubmittedAt = new DateTime(2025, 11, 5, 14, 20, 0)
            },
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 4,
                FirstName = "Christopher",
                LastName = "Williams",
                SEID = "87654324",
                CredentialArea = "Single Subject - English",
                Status = "PENDING_LEA",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 5),
                LastActionBy = "grants@ucla.edu",
                CreatedAt = new DateTime(2025, 11, 2, 13, 20, 0),
                SubmittedAt = new DateTime(2025, 11, 5, 14, 20, 0)
            },
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 4,
                FirstName = "Michelle",
                LastName = "Rodriguez",
                SEID = "87654325",
                CredentialArea = "Multiple Subject",
                Status = "PENDING_LEA",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 5),
                LastActionBy = "grants@ucla.edu",
                CreatedAt = new DateTime(2025, 11, 3, 11, 0, 0),
                SubmittedAt = new DateTime(2025, 11, 5, 14, 20, 0)
            }
        });

        // Add PENDING_LEA students to Application 5 (UCI -> LAUSD) - Candidates awaiting review
        applications[4].Students.AddRange(new[]
        {
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 5,
                FirstName = "Kevin",
                LastName = "Nguyen",
                SEID = "76543210",
                CredentialArea = "Single Subject - Mathematics",
                Status = "PENDING_LEA",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 6),
                LastActionBy = "credentialing@uci.edu",
                CreatedAt = new DateTime(2025, 11, 3, 14, 30, 0),
                SubmittedAt = new DateTime(2025, 11, 6, 9, 10, 0)
            },
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 5,
                FirstName = "Patricia",
                LastName = "Anderson",
                SEID = "76543211",
                CredentialArea = "Multiple Subject",
                Status = "PENDING_LEA",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 6),
                LastActionBy = "credentialing@uci.edu",
                CreatedAt = new DateTime(2025, 11, 4, 9, 15, 0),
                SubmittedAt = new DateTime(2025, 11, 6, 9, 10, 0)
            },
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 5,
                FirstName = "Brian",
                LastName = "Kim",
                SEID = "76543212",
                CredentialArea = "Single Subject - Social Studies",
                Status = "PENDING_LEA",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 6),
                LastActionBy = "credentialing@uci.edu",
                CreatedAt = new DateTime(2025, 11, 5, 10, 45, 0),
                SubmittedAt = new DateTime(2025, 11, 6, 9, 10, 0)
            }
        });

        // Add WAITLIST students - Approved but awaiting funding
        // These are candidates who have been approved but the program is at/near capacity
        applications[0].Students.AddRange(new[]
        {
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 1,
                FirstName = "Emma",
                LastName = "Richardson",
                SEID = "98765432",
                CredentialArea = "Single Subject - Science",
                Status = "WAITLIST",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 10),
                LastActionBy = "CTC Grants Team",
                CreatedAt = new DateTime(2025, 10, 15, 9, 0, 0),
                SubmittedAt = new DateTime(2025, 10, 20, 14, 30, 0),
                ApprovedAt = new DateTime(2025, 11, 1, 10, 0, 0),
                WaitlistDate = new DateTime(2025, 11, 10),
                WaitlistPosition = 1
            },
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 1,
                FirstName = "Carlos",
                LastName = "Mendez",
                SEID = "98765433",
                CredentialArea = "Multiple Subject",
                Status = "WAITLIST",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 10),
                LastActionBy = "CTC Grants Team",
                CreatedAt = new DateTime(2025, 10, 16, 11, 30, 0),
                SubmittedAt = new DateTime(2025, 10, 21, 9, 15, 0),
                ApprovedAt = new DateTime(2025, 11, 2, 14, 45, 0),
                WaitlistDate = new DateTime(2025, 11, 10),
                WaitlistPosition = 2
            },
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 1,
                FirstName = "Ashley",
                LastName = "Patel",
                SEID = "98765434",
                CredentialArea = "Education Specialist - Moderate/Severe",
                Status = "WAITLIST",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 11),
                LastActionBy = "CTC Grants Team",
                CreatedAt = new DateTime(2025, 10, 18, 8, 45, 0),
                SubmittedAt = new DateTime(2025, 10, 23, 16, 0, 0),
                ApprovedAt = new DateTime(2025, 11, 5, 11, 30, 0),
                WaitlistDate = new DateTime(2025, 11, 11),
                WaitlistPosition = 3
            }
        });

        applications[1].Students.AddRange(new[]
        {
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 2,
                FirstName = "Nathan",
                LastName = "Foster",
                SEID = "98765435",
                CredentialArea = "Single Subject - Mathematics",
                Status = "WAITLIST",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 12),
                LastActionBy = "CTC Grants Team",
                CreatedAt = new DateTime(2025, 10, 20, 10, 0, 0),
                SubmittedAt = new DateTime(2025, 10, 25, 13, 45, 0),
                ApprovedAt = new DateTime(2025, 11, 8, 9, 15, 0),
                WaitlistDate = new DateTime(2025, 11, 12),
                WaitlistPosition = 4
            },
            new Student
            {
                Id = additionalStudentId++,
                ApplicationId = 2,
                FirstName = "Grace",
                LastName = "Wong",
                SEID = "98765436",
                CredentialArea = "Single Subject - English",
                Status = "WAITLIST",
                AwardAmount = 10000,
                LastActionDate = new DateTime(2025, 11, 12),
                LastActionBy = "CTC Grants Team",
                CreatedAt = new DateTime(2025, 10, 22, 14, 30, 0),
                SubmittedAt = new DateTime(2025, 10, 27, 11, 0, 0),
                ApprovedAt = new DateTime(2025, 11, 9, 15, 30, 0),
                WaitlistDate = new DateTime(2025, 11, 12),
                WaitlistPosition = 5
            }
        });

        return applications;
    }

    private List<Payment> InitializePayments()
    {
        var payments = new List<Payment>();

        // Create payments for approved students (showing various payment lifecycle stages)
        // Student 2 (Maria Garcia) - Payment complete, has both reports (COMPLIANT)
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

        // Student 4 (Sarah Johnson) - Warrant issued, awaiting distribution (Has IHE report but missing LEA report - WARNING)
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
            ActualPaymentAmount = null,
            ActualPaymentDate = null,
            Status = "WARRANT_ISSUED",
            CreatedAt = new DateTime(2025, 9, 15),
            CreatedBy = "fiscal@ctc.ca.gov",
            LastModified = new DateTime(2025, 10, 1),
            ModifiedBy = "fiscal@ctc.ca.gov"
        });

        // Add 20 more payments with varying payment lifecycle stages
        // Distribution: 15% AUTHORIZED, 20% INVOICE_GENERATED, 25% WARRANT_ISSUED, 40% COMPLETED
        for (int i = 0; i < 20; i++)
        {
            string status;
            string warrantNumber = "";
            DateTime? warrantDate = null;
            decimal? actualPaymentAmount = null;
            DateTime? actualPaymentDate = null;
            DateTime? lastModified;
            string invoiceNumber = "";

            // Determine status and associated fields based on distribution
            if (i < 3)  // 15% - AUTHORIZED (no invoice yet)
            {
                status = "AUTHORIZED";
                invoiceNumber = "";
                lastModified = new DateTime(2025, 9, 1).AddDays(i);
            }
            else if (i < 7)  // 20% - INVOICE_GENERATED (invoice created, no warrant)
            {
                status = "INVOICE_GENERATED";
                invoiceNumber = $"INV-{(003 + i):D3}";
                lastModified = new DateTime(2025, 9, 5).AddDays(i);
            }
            else if (i < 12)  // 25% - WARRANT_ISSUED (warrant issued, not distributed)
            {
                status = "WARRANT_ISSUED";
                invoiceNumber = $"INV-{(003 + i):D3}";
                warrantNumber = $"W-2025-{(003 + i):D3}";
                warrantDate = new DateTime(2025, 9, 10).AddDays(i);
                lastModified = new DateTime(2025, 9, 10).AddDays(i);
            }
            else  // 40% - COMPLETED (payment distributed)
            {
                status = "COMPLETED";
                invoiceNumber = $"INV-{(003 + i):D3}";
                warrantNumber = $"W-2025-{(003 + i):D3}";
                warrantDate = new DateTime(2025, 9, 15).AddDays(i);
                actualPaymentAmount = 10000;
                actualPaymentDate = new DateTime(2025, 9, 15).AddDays(i);
                lastModified = new DateTime(2025, 9, 15).AddDays(i);
            }

            payments.Add(new Payment
            {
                Id = 3 + i,
                StudentId = 100 + i,
                ApplicationId = (i % 3) + 1,
                PONumber = $"PO-2025-{(003 + i):D3}",
                InvoiceNumber = invoiceNumber,
                AuthorizedAmount = 10000,
                AuthorizationDate = new DateTime(2025, 9, 1).AddDays(i),
                LEAName = i % 3 == 0 ? "Los Angeles Unified School District" :
                         i % 3 == 1 ? "San Diego Unified School District" :
                         "Fresno Unified School District",
                LEAAddress = i % 3 == 0 ? "333 S. Beaudry Ave, Los Angeles, CA 90017" :
                            i % 3 == 1 ? "4100 Normal St, San Diego, CA 92103" :
                            "2309 Tulare St, Fresno, CA 93721",
                WarrantNumber = warrantNumber,
                WarrantDate = warrantDate,
                ActualPaymentAmount = actualPaymentAmount,
                ActualPaymentDate = actualPaymentDate,
                Status = status,
                CreatedAt = new DateTime(2025, 9, 1).AddDays(i),
                CreatedBy = "fiscal@ctc.ca.gov",
                LastModified = lastModified,
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
            Status = "SUBMITTED",
            SubmittedDate = new DateTime(2025, 11, 1),
            SubmittedBy = "Jane Smith",
            SubmittedByEmail = "jsmith@lausd.net",
            CreatedAt = new DateTime(2025, 11, 1),
            LastModified = new DateTime(2025, 11, 1)
        });

        // Add reports for ~60% of other payments (showing partial compliance)
        // Status distribution: 70% SUBMITTED, 30% APPROVED
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
                Status = i % 10 < 7 ? "SUBMITTED" : "APPROVED",
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
            Status = "SUBMITTED",
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
            Status = "SUBMITTED",
            SubmittedDate = new DateTime(2025, 10, 28),
            SubmittedBy = "Dr. Sarah Williams",
            SubmittedByEmail = "swilliams@ucla.edu",
            CreatedAt = new DateTime(2025, 10, 28),
            LastModified = new DateTime(2025, 10, 28)
        });

        // Add reports for ~70% of other payments (showing higher IHE compliance than LEA)
        // Status distribution: 60% SUBMITTED, 40% APPROVED
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
                Status = i % 10 < 6 ? "SUBMITTED" : "APPROVED",
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
