// Mock Data Structure for GMS

const mockData = {
    grantCycles: [
        {
            id: 1,
            name: "Teacher Recruitment Incentive Program (STIPEND) - FY 2025-26",
            programType: "STIPEND",
            appropriatedAmount: 25000000,
            reservedAmount: 2150000,
            encumberedAmount: 8500000,
            disbursedAmount: 10200000,
            startDate: "2025-07-01",
            endDate: "2026-06-30",
            applicationOpen: true
        }
    ],

    organizations: [
        { id: 1, name: "Cal State University, Fullerton", type: "IHE", cdsCode: null },
        { id: 2, name: "UCLA", type: "IHE", cdsCode: null },
        { id: 3, name: "San Diego State University", type: "IHE", cdsCode: null },
        { id: 4, name: "University of California, Irvine", type: "IHE", cdsCode: null },
        { id: 5, name: "Los Angeles Unified School District", type: "LEA", cdsCode: "19-64733-0000000" },
        { id: 6, name: "San Diego Unified School District", type: "LEA", cdsCode: "37-68338-0000000" },
        { id: 7, name: "Fresno Unified School District", type: "LEA", cdsCode: "10-62281-0000000" },
        { id: 8, name: "Long Beach Unified School District", type: "LEA", cdsCode: "19-64733-1993647" }
    ],

    // Applications are now batch containers - one per IHE-LEA pair per grant cycle
    applications: [
        {
            id: 1,
            grantCycleId: 1,
            iheId: 1, // Cal State Fullerton
            leaId: 5, // LAUSD
            status: "ACTIVE", // ACTIVE = application is open for adding students throughout the year
            createdAt: "2025-08-15T10:00:00Z",
            createdBy: "coordinator@fullerton.edu",
            lastModified: "2025-11-03T14:30:00Z",
            students: [] // Will be populated separately
        },
        {
            id: 2,
            grantCycleId: 1,
            iheId: 2, // UCLA
            leaId: 6, // San Diego USD
            status: "ACTIVE",
            createdAt: "2025-08-20T09:00:00Z",
            createdBy: "grants@ucla.edu",
            lastModified: "2025-11-02T16:45:00Z",
            students: []
        },
        {
            id: 3,
            grantCycleId: 1,
            iheId: 3, // San Diego State
            leaId: 7, // Fresno USD
            status: "ACTIVE",
            createdAt: "2025-09-01T11:00:00Z",
            createdBy: "teacher.ed@sdsu.edu",
            lastModified: "2025-10-28T10:15:00Z",
            students: []
        },
        {
            id: 4,
            grantCycleId: 1,
            iheId: 1, // Cal State Fullerton
            leaId: 8, // Long Beach USD
            status: "ACTIVE",
            createdAt: "2025-08-18T14:00:00Z",
            createdBy: "coordinator@fullerton.edu",
            lastModified: "2025-10-15T09:30:00Z",
            students: []
        },
        {
            id: 5,
            grantCycleId: 1,
            iheId: 2, // UCLA
            leaId: 5, // LAUSD
            status: "ACTIVE",
            createdAt: "2025-08-22T11:00:00Z",
            createdBy: "grants@ucla.edu",
            lastModified: "2025-11-01T16:00:00Z",
            students: []
        }
    ],

    // Students within applications
    students: [
        // Cal State Fullerton → LAUSD students
        {
            id: 1,
            applicationId: 1,
            firstName: "John",
            lastName: "Smith",
            dob: "1998-01-15",
            seid: "12345678",
            credentialArea: "Multiple Subject",
            expectedCompletion: "2026-05-31",
            status: "APPROVED", // DRAFT | PENDING_LEA | PENDING_SUBMISSION | SUBMITTED | UNDER_REVIEW | APPROVED | REJECTED
            awardAmount: 10000,

            // IHE-entered data
            iheSubmittedAt: "2025-09-15T14:00:00Z",
            iheSubmittedBy: "coordinator@fullerton.edu",

            // LEA-entered data
            leaStartDate: "2025-09-01",
            leaEndDate: "2026-05-31",
            leaTotalDays: 180,
            leaContactEmail: "fiscal@lausd.net",
            leaContactPhone: "(213) 555-0100",
            leaAuthorizedSignatory: "Maria Rodriguez, Chief Business Officer",
            leaConfirmedEligibility: true,
            leaSubmittedAt: "2025-09-20T11:30:00Z",
            leaSubmittedBy: "fiscal@lausd.net",

            // CTC review data
            reviewedAt: "2025-09-25T09:00:00Z",
            reviewedBy: "Cara Mendoza",
            reviewNotes: "Approved - all eligibility criteria met",

            createdAt: "2025-09-15T14:00:00Z",
            lastModified: "2025-09-25T09:00:00Z"
        },
        {
            id: 2,
            applicationId: 1,
            firstName: "Maria",
            lastName: "Garcia",
            dob: "1999-03-22",
            seid: "23456789",
            credentialArea: "Single Subject - Mathematics",
            expectedCompletion: "2026-05-31",
            status: "APPROVED",
            awardAmount: 10000,

            iheSubmittedAt: "2025-09-15T14:05:00Z",
            iheSubmittedBy: "coordinator@fullerton.edu",

            leaStartDate: "2025-09-01",
            leaEndDate: "2026-05-31",
            leaTotalDays: 180,
            leaContactEmail: "fiscal@lausd.net",
            leaContactPhone: "(213) 555-0100",
            leaAuthorizedSignatory: "Maria Rodriguez, Chief Business Officer",
            leaConfirmedEligibility: true,
            leaSubmittedAt: "2025-09-20T11:30:00Z",
            leaSubmittedBy: "fiscal@lausd.net",

            reviewedAt: "2025-09-25T09:05:00Z",
            reviewedBy: "Cara Mendoza",
            reviewNotes: "Approved",

            createdAt: "2025-09-15T14:05:00Z",
            lastModified: "2025-09-25T09:05:00Z"
        },
        {
            id: 3,
            applicationId: 1,
            firstName: "David",
            lastName: "Chen",
            dob: "1997-11-08",
            seid: "34567890",
            credentialArea: "Education Specialist",
            expectedCompletion: "2026-05-31",
            status: "PENDING_LEA",
            awardAmount: null,

            iheSubmittedAt: "2025-11-01T10:00:00Z",
            iheSubmittedBy: "coordinator@fullerton.edu",

            leaStartDate: null,
            leaEndDate: null,
            leaTotalDays: null,
            leaContactEmail: null,
            leaContactPhone: null,
            leaAuthorizedSignatory: null,
            leaConfirmedEligibility: false,
            leaSubmittedAt: null,
            leaSubmittedBy: null,

            reviewedAt: null,
            reviewedBy: null,
            reviewNotes: null,

            createdAt: "2025-11-01T10:00:00Z",
            lastModified: "2025-11-01T10:00:00Z"
        },
        {
            id: 4,
            applicationId: 1,
            firstName: "Sarah",
            lastName: "Johnson",
            dob: "1998-07-14",
            seid: "45678901",
            credentialArea: "Multiple Subject",
            expectedCompletion: "2026-05-31",
            status: "DRAFT",
            awardAmount: null,

            iheSubmittedAt: null,
            iheSubmittedBy: null,

            leaStartDate: null,
            leaEndDate: null,
            leaTotalDays: null,
            leaContactEmail: null,
            leaContactPhone: null,
            leaAuthorizedSignatory: null,
            leaConfirmedEligibility: false,
            leaSubmittedAt: null,
            leaSubmittedBy: null,

            reviewedAt: null,
            reviewedBy: null,
            reviewNotes: null,

            createdAt: "2025-11-03T09:30:00Z",
            lastModified: "2025-11-03T14:30:00Z"
        },

        // UCLA → San Diego USD students
        {
            id: 5,
            applicationId: 2,
            firstName: "Jennifer",
            lastName: "Lee",
            dob: "1999-05-20",
            seid: "56789012",
            credentialArea: "Single Subject - English",
            expectedCompletion: "2026-06-15",
            status: "SUBMITTED",
            awardAmount: null,

            iheSubmittedAt: "2025-10-15T11:00:00Z",
            iheSubmittedBy: "grants@ucla.edu",

            leaStartDate: "2025-09-15",
            leaEndDate: "2026-06-15",
            leaTotalDays: 185,
            leaContactEmail: "payroll@sdusd.net",
            leaContactPhone: "(619) 555-0200",
            leaAuthorizedSignatory: "Robert Kim, CFO",
            leaConfirmedEligibility: true,
            leaSubmittedAt: "2025-10-20T14:00:00Z",
            leaSubmittedBy: "payroll@sdusd.net",

            reviewedAt: null,
            reviewedBy: null,
            reviewNotes: null,

            createdAt: "2025-10-15T11:00:00Z",
            lastModified: "2025-10-20T14:00:00Z"
        },
        {
            id: 6,
            applicationId: 2,
            firstName: "Michael",
            lastName: "Brown",
            dob: "1998-12-03",
            seid: "67890123",
            credentialArea: "Single Subject - Science",
            expectedCompletion: "2026-06-15",
            status: "SUBMITTED",
            awardAmount: null,

            iheSubmittedAt: "2025-10-15T11:05:00Z",
            iheSubmittedBy: "grants@ucla.edu",

            leaStartDate: "2025-09-15",
            leaEndDate: "2026-06-15",
            leaTotalDays: 185,
            leaContactEmail: "payroll@sdusd.net",
            leaContactPhone: "(619) 555-0200",
            leaAuthorizedSignatory: "Robert Kim, CFO",
            leaConfirmedEligibility: true,
            leaSubmittedAt: "2025-10-20T14:05:00Z",
            leaSubmittedBy: "payroll@sdusd.net",

            reviewedAt: null,
            reviewedBy: null,
            reviewNotes: null,

            createdAt: "2025-10-15T11:05:00Z",
            lastModified: "2025-10-20T14:05:00Z"
        }
    ],

    // Awards are created per student once approved
    awards: [
        {
            id: 1,
            studentId: 1,
            grantCycleId: 1,
            amount: 10000,
            status: "PAYMENT_COMPLETED",

            gaaGeneratedAt: "2025-09-26T10:00:00Z",
            gaaSentAt: "2025-09-26T10:15:00Z",

            gaaSignatures: [
                {
                    party: "LEA",
                    name: "Maria Rodriguez, Chief Business Officer",
                    status: "SIGNED",
                    sentAt: "2025-09-26T10:15:00Z",
                    signedAt: "2025-09-27T14:30:00Z"
                },
                {
                    party: "GRANTS_MANAGER",
                    name: "Cara Mendoza",
                    status: "SIGNED",
                    sentAt: "2025-09-27T15:00:00Z",
                    signedAt: "2025-09-28T09:15:00Z"
                },
                {
                    party: "FISCAL_OFFICER",
                    name: "Sara Saelee",
                    status: "SIGNED",
                    sentAt: "2025-09-28T09:30:00Z",
                    signedAt: "2025-09-29T11:00:00Z"
                }
            ],

            gaaFullySignedAt: "2025-09-29T11:00:00Z",
            paymentInitiatedAt: "2025-09-30T10:00:00Z",
            paymentCompletedAt: "2025-10-05T14:00:00Z",

            fiscalReferenceNumber: "FY26-STIPEND-0001",

            createdAt: "2025-09-26T10:00:00Z",
            lastModified: "2025-10-05T14:00:00Z"
        },
        {
            id: 2,
            studentId: 2,
            grantCycleId: 1,
            amount: 10000,
            status: "GAA_SIGNED",

            gaaGeneratedAt: "2025-09-26T10:05:00Z",
            gaaSentAt: "2025-09-26T10:20:00Z",

            gaaSignatures: [
                {
                    party: "LEA",
                    name: "Maria Rodriguez, Chief Business Officer",
                    status: "SIGNED",
                    sentAt: "2025-09-26T10:20:00Z",
                    signedAt: "2025-09-27T14:35:00Z"
                },
                {
                    party: "GRANTS_MANAGER",
                    name: "Cara Mendoza",
                    status: "SIGNED",
                    sentAt: "2025-09-27T15:05:00Z",
                    signedAt: "2025-09-28T09:20:00Z"
                },
                {
                    party: "FISCAL_OFFICER",
                    name: "Sara Saelee",
                    status: "SIGNED",
                    sentAt: "2025-09-28T09:35:00Z",
                    signedAt: "2025-09-29T11:05:00Z"
                }
            ],

            gaaFullySignedAt: "2025-09-29T11:05:00Z",
            paymentInitiatedAt: null,
            paymentCompletedAt: null,

            fiscalReferenceNumber: "FY26-STIPEND-0002",

            createdAt: "2025-09-26T10:05:00Z",
            lastModified: "2025-09-29T11:05:00Z"
        }
    ],

    // Current user context
    currentUser: {
        name: "Cara Mendoza",
        email: "cara.mendoza@ctc.ca.gov",
        role: "STAFF_ADMIN",
        organizationId: null
    },

    // Portal-specific users for switching
    portalUsers: {
        staff: {
            name: "Cara Mendoza",
            email: "cara.mendoza@ctc.ca.gov",
            role: "STAFF_ADMIN",
            organizationId: null
        },
        ihe: {
            name: "Dr. Emily Richardson",
            email: "coordinator@fullerton.edu",
            role: "IHE_COORDINATOR",
            organizationId: 1 // Cal State Fullerton
        },
        lea: {
            name: "Maria Rodriguez",
            email: "fiscal@lausd.net",
            role: "LEA_FISCAL",
            organizationId: 5 // LAUSD
        }
    },

    // Fund depletion timeline for charting
    fundDepletionTimeline: [
        {
            date: "2025-07-01",
            remainingFunds: 25000000,
            disbursed: 0,
            encumbered: 0,
            reserved: 0,
            label: "Grant Cycle Start"
        },
        {
            date: "2025-07-15",
            remainingFunds: 25000000,
            disbursed: 0,
            encumbered: 0,
            reserved: 0,
            label: "Applications Open"
        },
        {
            date: "2025-08-01",
            remainingFunds: 24500000,
            disbursed: 0,
            encumbered: 0,
            reserved: 500000,
            label: "First Approvals"
        },
        {
            date: "2025-08-15",
            remainingFunds: 23500000,
            disbursed: 0,
            encumbered: 500000,
            reserved: 1000000,
            label: "GAAs Begin Signing"
        },
        {
            date: "2025-09-01",
            remainingFunds: 21200000,
            disbursed: 1800000,
            encumbered: 1200000,
            reserved: 800000,
            label: "First Payments"
        },
        {
            date: "2025-09-15",
            remainingFunds: 18400000,
            disbursed: 3500000,
            encumbered: 1800000,
            reserved: 1300000,
            label: "Payment Processing"
        },
        {
            date: "2025-10-01",
            remainingFunds: 14600000,
            disbursed: 6200000,
            encumbered: 2900000,
            reserved: 1300000,
            label: "Q1 Peak"
        },
        {
            date: "2025-10-15",
            remainingFunds: 11100000,
            disbursed: 8400000,
            encumbered: 4200000,
            reserved: 1300000,
            label: "High Volume Period"
        },
        {
            date: "2025-11-01",
            remainingFunds: 5450000,
            disbursed: 9800000,
            encumbered: 7600000,
            reserved: 2150000,
            label: "Continued Processing"
        },
        {
            date: "2025-11-04",
            remainingFunds: 4150000,
            disbursed: 10200000,
            encumbered: 8500000,
            reserved: 2150000,
            label: "Current (Today)"
        }
    ],

    // Notifications
    notifications: [
        {
            id: 1,
            type: "action_required",
            icon: "⚡",
            title: "Student needs LEA information",
            body: "David Chen - Cal State Fullerton → LAUSD",
            time: "2 hours ago",
            read: false,
            portal: "lea",
            action: () => Applications.viewStudent(3)
        },
        {
            id: 2,
            type: "action_required",
            icon: "📋",
            title: "89 students awaiting review",
            body: "New applications submitted for your review",
            time: "3 hours ago",
            read: false,
            portal: "staff",
            action: () => { App.currentView = 'applications'; App.renderCurrentView(); }
        },
        {
            id: 3,
            type: "info",
            icon: "✓",
            title: "Student approved",
            body: "John Smith approved for $10,000 award",
            time: "Yesterday",
            read: false,
            portal: "ihe",
            action: () => Applications.viewStudent(1)
        },
        {
            id: 4,
            type: "deadline",
            icon: "⏰",
            title: "Annual report due in 3 days",
            body: "STIPEND FY 2024-25 report due Nov 7",
            time: "Yesterday",
            read: true,
            portal: "lea",
            action: () => Utils.showToast('Reports feature coming soon', 'info')
        },
        {
            id: 5,
            type: "completed",
            icon: "📝",
            title: "GAA signed by all parties",
            body: "Maria Garcia award fully executed",
            time: "3 days ago",
            read: true,
            portal: "staff",
            action: () => Awards.viewAwardDetails(2)
        }
    ]
};

// Helper functions for data access
const dataHelpers = {
    getGrantCycle(id) {
        return mockData.grantCycles.find(gc => gc.id === id);
    },

    getOrganization(id) {
        return mockData.organizations.find(org => org.id === id);
    },

    getApplication(id) {
        const app = mockData.applications.find(a => a.id === id);
        if (app) {
            app.ihe = this.getOrganization(app.iheId);
            app.lea = this.getOrganization(app.leaId);
            app.students = this.getStudentsByApplication(app.id);
        }
        return app;
    },

    getApplicationsByPortal(portal) {
        const currentUser = mockData.portalUsers[portal];
        if (portal === 'staff') {
            return mockData.applications.map(app => this.getApplication(app.id));
        } else if (portal === 'ihe') {
            return mockData.applications
                .filter(app => app.iheId === currentUser.organizationId)
                .map(app => this.getApplication(app.id));
        } else if (portal === 'lea') {
            return mockData.applications
                .filter(app => app.leaId === currentUser.organizationId)
                .map(app => this.getApplication(app.id));
        }
        return [];
    },

    getStudentsByApplication(applicationId) {
        return mockData.students.filter(s => s.applicationId === applicationId);
    },

    getStudent(id) {
        return mockData.students.find(s => s.id === id);
    },

    getAwardByStudent(studentId) {
        return mockData.awards.find(a => a.studentId === studentId);
    },

    calculateGrantCycleMetrics(grantCycleId) {
        const cycle = this.getGrantCycle(grantCycleId);
        if (!cycle) return null;

        const remainingAmount = cycle.appropriatedAmount -
            (cycle.reservedAmount + cycle.encumberedAmount + cycle.disbursedAmount);
        const outstandingBalance = cycle.reservedAmount + cycle.encumberedAmount;
        const remainingPercent = (remainingAmount / cycle.appropriatedAmount) * 100;

        // Count students by status across all applications in this cycle
        const allStudents = mockData.students.filter(s => {
            const app = mockData.applications.find(a => a.id === s.applicationId);
            return app && app.grantCycleId === grantCycleId;
        });

        const statusCounts = {
            draft: allStudents.filter(s => s.status === 'DRAFT').length,
            pending_lea: allStudents.filter(s => s.status === 'PENDING_LEA').length,
            submitted: allStudents.filter(s => s.status === 'SUBMITTED').length,
            under_review: allStudents.filter(s => s.status === 'UNDER_REVIEW').length,
            approved: allStudents.filter(s => s.status === 'APPROVED').length,
            rejected: allStudents.filter(s => s.status === 'REJECTED').length
        };

        // Count unique organizations
        const appsInCycle = mockData.applications.filter(a => a.grantCycleId === grantCycleId);
        const uniqueIHEs = new Set(appsInCycle.map(a => a.iheId)).size;
        const uniqueLEAs = new Set(appsInCycle.map(a => a.leaId)).size;

        return {
            ...cycle,
            remainingAmount,
            outstandingBalance,
            remainingPercent,
            totalStudents: allStudents.length,
            uniqueIHEs,
            uniqueLEAs,
            statusCounts
        };
    }
};
