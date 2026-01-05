-- ============================================================================
-- TEACHER RESIDENCY PROGRAM (TRP) - SIMULATED DATA
-- Pressure Test of GMS V1 Schema Against Complex Grant Program
-- ============================================================================
--
-- TRP Characteristics (differs significantly from STSP):
-- - IHE-LEA partnership (both orgs involved throughout)
-- - Budgeted awards (not fixed $10K)
-- - Matching funds required (LEA contributes)
-- - Multiple funding categories (tuition, stipend, exams, mentor PD)
-- - Multi-year program (residents in program 1-2 years)
-- - 4-year post-completion outcome tracking
-- - Mentor teacher assignments
-- - TPA (Teacher Performance Assessment) tracking
--
-- Source: Real Fi$Cal data shows TRP grants range $180K - $1.26M per LEA
-- ============================================================================

-- ============================================================================
-- 1. GRANT PROGRAM CONFIGURATION
-- ============================================================================

INSERT INTO GrantProgram (
    Code, Name, Description,
    WorkflowType, HasInitiatorOrg, HasCompleterOrg, HasBatching,
    EvaluationType, AwardUnit, DisbursementType, HasMatchingFunds,
    TrackingYears, HasCohorts, HasReplacements,
    BatchStrategy, BatchInitiatorOrgType,
    HoursThreshold, CredentialHoursThreshold,
    PaymentPhasesJson, ReportRequirementsJson, ProgramConfigJson,
    IsActive, CreatedByAccountId
) VALUES (
    'TRP', 'Teacher Residency Program',
    'Multi-year funding for teacher residency programs. Residents complete a full year of clinical practice under mentor teacher supervision while earning their credential. Requires IHE-LEA partnership and LEA matching funds. Tracks employment outcomes for 4 years post-completion.',

    -- Workflow: Same as STSP but LEA is true partner, not just completer
    'IHE_LEA_CTC', 1, 1, 1,

    -- Evaluation: Competitive (scored applications, not just eligibility)
    'COMPETITIVE',

    -- Funding: Per-candidate budgeted amounts (not fixed)
    'PER_CANDIDATE',

    -- Disbursement: Multi-phase (initial + completion payments)
    'MULTI_PHASE',

    -- Matching: LEA must provide matching funds
    1,

    -- Tracking: 4 years post-completion employment
    4,

    -- Cohorts: Yes - residents grouped by cohort year
    1,

    -- Replacements: No - can't replace residents mid-program
    0,

    -- Batching: Annual cohort-based, not monthly
    'COHORT', 'IHE',

    -- Hours: Full year residency (not applicable same as STSP)
    NULL, NULL,

    -- Multi-phase payments: 50% on enrollment, 50% on completion
    '[
        {"phase": 1, "percent": 50, "trigger": "ON_ENROLLMENT", "name": "Initial Payment"},
        {"phase": 2, "percent": 50, "trigger": "ON_COMPLETION", "name": "Completion Payment"}
    ]',

    -- Report requirements
    '[
        {"orgType": "IHE", "reportType": "ANNUAL_PROGRESS", "dueMonthDay": "07-31"},
        {"orgType": "IHE", "reportType": "COMPLETION", "dueMonthDay": "08-31"},
        {"orgType": "LEA", "reportType": "EMPLOYMENT_YEAR1", "dueMonthDay": "10-31"},
        {"orgType": "LEA", "reportType": "EMPLOYMENT_YEAR2", "dueMonthDay": "10-31"},
        {"orgType": "LEA", "reportType": "EMPLOYMENT_YEAR3", "dueMonthDay": "10-31"},
        {"orgType": "LEA", "reportType": "EMPLOYMENT_YEAR4", "dueMonthDay": "10-31"}
    ]',

    -- Program-specific config
    '{
        "requiresMentor": true,
        "mentorPDRequired": true,
        "tpaRequired": true,
        "tpaTypes": ["CalTPA", "edTPA"],
        "maxTpaAttempts": 3,
        "fundingCategories": ["TUITION", "STIPEND", "EXAM_FEES", "MENTOR_PD", "ADMIN"],
        "matchRatioMin": 0.25,
        "credentialAreas": ["Multiple Subject", "Single Subject", "Education Specialist"]
    }',

    1, -1
);

-- Get the GrantProgramId (assume it's 2, after STSP which is 1)
DECLARE @TRPProgramId INT = 2;

-- ============================================================================
-- 2. GRANT CYCLE
-- ============================================================================

INSERT INTO GrantCycle (
    GrantProgramId, Name, FiscalYear,
    AppropriatedAmount, DefaultAwardAmount, MinAwardAmount, MaxAwardAmount,
    ApplicationOpenOn, ApplicationCloseOn, CycleStartOn, CycleEndOn,
    EligibilityRulesJson, Status, CreatedByAccountId
) VALUES (
    @TRPProgramId,
    'Teacher Residency Program - Cohort 2024-25',
    '2024-25',

    -- $15M appropriation (from real Fi$Cal data, single grants are $180K-$1.26M)
    15000000.00,

    -- Default $36K per resident (tuition + stipend + fees)
    36000.00,

    -- Min/Max per resident
    25000.00,
    50000.00,

    -- Application window
    '2024-01-15', '2024-03-15',

    -- Cycle runs full academic year
    '2024-07-01', '2025-06-30',

    -- Eligibility rules
    '{
        "iheRequirements": ["accreditedProgram", "partnershipAgreement"],
        "leaRequirements": ["matchingFundsCommitment", "mentorAvailability"],
        "candidateRequirements": ["baDegree", "subjectMatterCompetency", "backgroundCheck"]
    }',

    'ACTIVE', -1
);

DECLARE @TRPCycleId INT = 2;  -- Assume STSP cycle is 1

-- ============================================================================
-- 3. ORGANIZATIONS (IHE + LEA Partners)
-- ============================================================================

-- IHE: CSU Long Beach (from real Fi$Cal data - Long Beach USD is partner)
INSERT INTO Organization (TypeId, EcsCdsCode, Name, City, StateProvince, IsActive)
VALUES (1, '19-10017-0000000', 'California State University, Long Beach', 'Long Beach', 'CA', 1);
DECLARE @CSULBId INT = SCOPE_IDENTITY();

-- LEA: Long Beach USD (real - $1.08M grant in Fi$Cal)
INSERT INTO Organization (TypeId, EcsCdsCode, Name, City, StateProvince, IsActive)
VALUES (2, '19-64733-0000000', 'Long Beach Unified School District', 'Long Beach', 'CA', 1);
DECLARE @LBUSDId INT = SCOPE_IDENTITY();

-- IHE: Sacramento State
INSERT INTO Organization (TypeId, EcsCdsCode, Name, City, StateProvince, IsActive)
VALUES (1, '34-10019-0000000', 'California State University, Sacramento', 'Sacramento', 'CA', 1);
DECLARE @SacStateId INT = SCOPE_IDENTITY();

-- LEA: Sacramento City USD (real - multiple disbursements in Fi$Cal: $900K, $100K, $800K)
INSERT INTO Organization (TypeId, EcsCdsCode, Name, City, StateProvince, IsActive)
VALUES (2, '34-67439-0000000', 'Sacramento City Unified School District', 'Sacramento', 'CA', 1);
DECLARE @SacCityId INT = SCOPE_IDENTITY();

-- ============================================================================
-- 4. CANDIDATES (Residents) - Various Statuses
-- ============================================================================

-- Candidate 1: Completed program, now in Year 2 of employment tracking
INSERT INTO Candidate (
    GrantCycleId, OriginatingOrgId, StatusId,
    SEID, FirstName, LastName, DateOfBirth, Last4SSN, EmailAddress,
    Race, Ethnicity, Gender, CredentialArea,
    SchoolCdsCode, SchoolName,
    RequestedAmount, AwardedAmount,
    CohortYear, ParticipantType,
    StatusChangedOn, SubmittedByInitiatorOn, SubmittedByCompleterOn, ApprovedOn,
    CreatedByAccountId
) VALUES (
    @TRPCycleId, @CSULBId, 12,  -- Status: REPORTING_YEAR2
    'SEID001234567', 'Maria', 'Rodriguez', '1995-03-15', '4567', 'maria.rodriguez@csulb.edu',
    'Hispanic or Latino', 'Hispanic or Latino', 'Female', 'Multiple Subject',
    '19-64733-6019234', 'Washington Elementary',
    36000.00, 36000.00,
    '2023-24', 'RESIDENT',
    GETDATE(), '2023-02-15', '2023-02-20', '2023-03-01',
    -1
);
DECLARE @Candidate1Id INT = SCOPE_IDENTITY();

-- Candidate 2: Currently in program (Year 1 resident)
INSERT INTO Candidate (
    GrantCycleId, OriginatingOrgId, StatusId,
    SEID, FirstName, LastName, DateOfBirth, Last4SSN, EmailAddress,
    Race, Ethnicity, Gender, CredentialArea,
    SchoolCdsCode, SchoolName,
    RequestedAmount, AwardedAmount,
    CohortYear, ParticipantType,
    StatusChangedOn, SubmittedByInitiatorOn, SubmittedByCompleterOn, ApprovedOn,
    CreatedByAccountId
) VALUES (
    @TRPCycleId, @CSULBId, 8,  -- Status: IN_PROGRAM
    'SEID001234568', 'James', 'Chen', '1997-08-22', '7890', 'james.chen@csulb.edu',
    'Asian', 'Not Hispanic or Latino', 'Male', 'Single Subject - Mathematics',
    '19-64733-6019456', 'Wilson High School',
    40000.00, 38000.00,  -- Requested more, got slightly less
    '2024-25', 'RESIDENT',
    GETDATE(), '2024-02-10', '2024-02-18', '2024-03-05',
    -1
);
DECLARE @Candidate2Id INT = SCOPE_IDENTITY();

-- Candidate 3: Completed, Year 1 employment - LEFT TEACHING (attrition case)
INSERT INTO Candidate (
    GrantCycleId, OriginatingOrgId, StatusId,
    SEID, FirstName, LastName, DateOfBirth, Last4SSN, EmailAddress,
    Race, Ethnicity, Gender, CredentialArea,
    SchoolCdsCode, SchoolName,
    RequestedAmount, AwardedAmount,
    CohortYear, ParticipantType,
    StatusChangedOn, SubmittedByInitiatorOn, ApprovedOn,
    CreatedByAccountId
) VALUES (
    @TRPCycleId, @SacStateId, 13,  -- Status: OUTCOME_TRACKING (but left)
    'SEID001234569', 'Ashley', 'Thompson', '1996-11-08', '2345', 'ashley.thompson@csus.edu',
    'White', 'Not Hispanic or Latino', 'Female', 'Education Specialist - Mild/Moderate',
    '34-67439-3430123', 'Sutter Middle School',
    42000.00, 42000.00,
    '2022-23', 'RESIDENT',
    GETDATE(), '2022-02-12', '2022-03-08',
    -1
);
DECLARE @Candidate3Id INT = SCOPE_IDENTITY();

-- Candidate 4: Early exit from program (didn't complete)
INSERT INTO Candidate (
    GrantCycleId, OriginatingOrgId, StatusId,
    SEID, FirstName, LastName, DateOfBirth, Last4SSN, EmailAddress,
    Race, Ethnicity, Gender, CredentialArea,
    SchoolCdsCode, SchoolName,
    RequestedAmount, AwardedAmount,
    CohortYear, ParticipantType,
    StatusChangedOn, SubmittedByInitiatorOn, ApprovedOn, RejectedOn, RejectionReason,
    CreatedByAccountId
) VALUES (
    @TRPCycleId, @SacStateId, 99,  -- Status: EARLY_EXIT
    'SEID001234570', 'Michael', 'Davis', '1994-05-20', '6789', 'michael.davis@csus.edu',
    'Black or African American', 'Not Hispanic or Latino', 'Male', 'Single Subject - Science',
    '34-67439-3430456', 'Kennedy High School',
    36000.00, 36000.00,
    '2023-24', 'RESIDENT',
    GETDATE(), '2023-02-14', '2023-03-02', '2023-11-15',
    'Personal circumstances - relocated out of state',
    -1
);
DECLARE @Candidate4Id INT = SCOPE_IDENTITY();

-- Candidate 5: Sacramento - In program, second year
INSERT INTO Candidate (
    GrantCycleId, OriginatingOrgId, StatusId,
    SEID, FirstName, LastName, DateOfBirth, Last4SSN, EmailAddress,
    Race, Ethnicity, Gender, CredentialArea,
    SchoolCdsCode, SchoolName,
    RequestedAmount, AwardedAmount,
    CohortYear, ParticipantType,
    StatusChangedOn, SubmittedByInitiatorOn, ApprovedOn,
    CreatedByAccountId
) VALUES (
    @TRPCycleId, @SacStateId, 8,  -- Status: IN_PROGRAM
    'SEID001234571', 'Jennifer', 'Nguyen', '1998-01-30', '1234', 'jennifer.nguyen@csus.edu',
    'Asian', 'Not Hispanic or Latino', 'Female', 'Multiple Subject',
    '34-67439-3430789', 'Lincoln Elementary',
    38000.00, 38000.00,
    '2024-25', 'RESIDENT',
    GETDATE(), '2024-02-08', '2024-03-01',
    -1
);
DECLARE @Candidate5Id INT = SCOPE_IDENTITY();

-- ============================================================================
-- 5. CANDIDATE FUNDING (Grant + Match, Multiple Categories)
-- ============================================================================
-- This is the key TRP differentiator - tracking funding by category and source

-- Candidate 1 (Maria) - Completed, Full Funding
-- Grant Funds
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate1Id, '2023-24', 'GRANT', 'TUITION', 15000.00, 15000.00, -1);
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate1Id, '2023-24', 'GRANT', 'STIPEND', 18000.00, 18000.00, -1);
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate1Id, '2023-24', 'GRANT', 'EXAM_FEES', 1500.00, 1200.00, -1);  -- Didn't use full exam budget
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate1Id, '2023-24', 'GRANT', 'MENTOR_PD', 1500.00, 1500.00, -1);

-- Match Funds (LEA contribution)
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate1Id, '2023-24', 'MATCH', 'STIPEND', 6000.00, 6000.00, -1);  -- LEA topped up stipend
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate1Id, '2023-24', 'MATCH', 'MENTOR_PD', 3000.00, 3000.00, -1);  -- LEA funded additional mentor PD

-- Candidate 2 (James) - In Program, Partial Year 1 Spending
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate2Id, '2024-25', 'GRANT', 'TUITION', 16000.00, 8000.00, -1);  -- Half year paid
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate2Id, '2024-25', 'GRANT', 'STIPEND', 19000.00, 9500.00, -1);  -- Half year paid
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate2Id, '2024-25', 'GRANT', 'EXAM_FEES', 1500.00, 0.00, -1);   -- Not yet used
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate2Id, '2024-25', 'GRANT', 'MENTOR_PD', 1500.00, 1500.00, -1);  -- Mentor PD upfront

INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate2Id, '2024-25', 'MATCH', 'STIPEND', 5000.00, 2500.00, -1);
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, CreatedByAccountId)
VALUES (@Candidate2Id, '2024-25', 'MATCH', 'ADMIN', 2000.00, 2000.00, -1);  -- LEA admin costs

-- Candidate 4 (Michael) - Early Exit, Partial Funding Clawback Scenario
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, Notes, CreatedByAccountId)
VALUES (@Candidate4Id, '2023-24', 'GRANT', 'TUITION', 15000.00, 7500.00, 'Prorated - exited Nov 2023', -1);
INSERT INTO CandidateFunding (CandidateId, FiscalYear, FundingSource, Category, BudgetedAmount, ExpendedAmount, Notes, CreatedByAccountId)
VALUES (@Candidate4Id, '2023-24', 'GRANT', 'STIPEND', 18000.00, 6000.00, 'Prorated - exited Nov 2023', -1);

-- ============================================================================
-- 6. CANDIDATE YEAR PROGRESS (Annual Progress During Program)
-- ============================================================================

-- Candidate 1 (Maria) - Completed Year 1 progress
INSERT INTO CandidateYearProgress (
    CandidateId, FiscalYear, YearInProgram,
    HasDegreeProgress, HasDegreeCompleted, HasCredentialProgress, HasCredentialCompleted,
    IsEmployedNextYear, HasEarlyExit,
    ExtendedDataJson, Notes, CreatedByAccountId
) VALUES (
    @Candidate1Id, '2023-24', 1,
    NULL, NULL,  -- Already has BA
    1, 1,        -- Credential completed
    1, 0,        -- Employed next year, no early exit
    '{
        "tpaType": "CalTPA",
        "tpaAttempts": 1,
        "tpaPassed": true,
        "tpaPassedDate": "2024-04-15",
        "mentorName": "Patricia Williams",
        "mentorStartDate": "2023-08-15",
        "mentorEndDate": "2024-05-31",
        "mentorRating": 5,
        "clinicalHours": 1200
    }',
    'Strong candidate. Passed TPA on first attempt. Excellent mentor relationship.',
    -1
);

-- Candidate 2 (James) - Currently in Year 1
INSERT INTO CandidateYearProgress (
    CandidateId, FiscalYear, YearInProgram,
    HasDegreeProgress, HasDegreeCompleted, HasCredentialProgress, HasCredentialCompleted,
    IsEmployedNextYear, HasEarlyExit,
    ExtendedDataJson, Notes, CreatedByAccountId
) VALUES (
    @Candidate2Id, '2024-25', 1,
    NULL, NULL,  -- Already has BA
    1, 0,        -- Credential in progress
    NULL, 0,     -- TBD, not exited
    '{
        "tpaType": "CalTPA",
        "tpaAttempts": 0,
        "tpaPassed": false,
        "mentorName": "Robert Garcia",
        "mentorStartDate": "2024-08-12",
        "mentorEndDate": null,
        "clinicalHours": 450
    }',
    'On track. Taking TPA in Spring 2025.',
    -1
);

-- Candidate 3 (Ashley) - Completed but shows 2 years of progress (2-year program)
INSERT INTO CandidateYearProgress (
    CandidateId, FiscalYear, YearInProgram,
    HasDegreeProgress, HasDegreeCompleted, HasCredentialProgress, HasCredentialCompleted,
    IsEmployedNextYear, HasEarlyExit,
    ExtendedDataJson, CreatedByAccountId
) VALUES (
    @Candidate3Id, '2022-23', 1,
    NULL, NULL, 1, 0,
    1, 0,
    '{"tpaType": "CalTPA", "tpaAttempts": 2, "tpaPassed": false, "mentorName": "Susan Lee"}',
    -1
);

INSERT INTO CandidateYearProgress (
    CandidateId, FiscalYear, YearInProgram,
    HasDegreeProgress, HasDegreeCompleted, HasCredentialProgress, HasCredentialCompleted,
    IsEmployedNextYear, HasEarlyExit,
    ExtendedDataJson, CreatedByAccountId
) VALUES (
    @Candidate3Id, '2023-24', 2,
    NULL, NULL, 1, 1,  -- Completed credential in Year 2
    1, 0,
    '{"tpaType": "CalTPA", "tpaAttempts": 3, "tpaPassed": true, "tpaPassedDate": "2024-03-20"}',
    -1
);

-- Candidate 4 (Michael) - Early Exit
INSERT INTO CandidateYearProgress (
    CandidateId, FiscalYear, YearInProgram,
    HasDegreeProgress, HasDegreeCompleted, HasCredentialProgress, HasCredentialCompleted,
    IsEmployedNextYear, HasEarlyExit, EarlyExitOn, EarlyExitReason,
    ExtendedDataJson, CreatedByAccountId
) VALUES (
    @Candidate4Id, '2023-24', 1,
    NULL, NULL, 1, 0,  -- Did not complete credential
    0, 1, '2023-11-15', 'Relocated out of state - family circumstances',
    '{"tpaType": "CalTPA", "tpaAttempts": 0, "clinicalHours": 280}',
    -1
);

-- ============================================================================
-- 7. CANDIDATE YEAR OUTCOME (4-Year Post-Completion Employment Tracking)
-- ============================================================================
-- This is the signature TRP feature - tracking if residents stay in teaching

-- Candidate 1 (Maria) - Year 1 and Year 2 outcomes
-- Year 1: Teaching at same school as residency
INSERT INTO CandidateYearOutcome (
    CandidateId, EmployerOrgId, OutcomeYear, FiscalYear,
    IsEmployed, EmployerName, SchoolCdsCode, SchoolName,
    Position, GradeLevel, SubjectArea,
    IsSameLEAAsPlacement, IsSameSchoolAsPlacement, HasCompletedYear,
    ExtendedDataJson, CreatedByAccountId
) VALUES (
    @Candidate1Id, @LBUSDId, 1, '2024-25',
    1, 'Long Beach Unified School District', '19-64733-6019234', 'Washington Elementary',
    'TEACHER', 'K-2', 'Multiple Subject',
    1, 1, 1,  -- Same LEA, Same School, Completed Year
    '{"salary": 62000, "contractType": "Probationary", "evaluationRating": "Effective"}',
    -1
);

-- Year 2: Still teaching, same LEA but different school (transferred)
INSERT INTO CandidateYearOutcome (
    CandidateId, EmployerOrgId, OutcomeYear, FiscalYear,
    IsEmployed, EmployerName, SchoolCdsCode, SchoolName,
    Position, GradeLevel, SubjectArea,
    IsSameLEAAsPlacement, IsSameSchoolAsPlacement, HasCompletedYear,
    ExtendedDataJson, CreatedByAccountId
) VALUES (
    @Candidate1Id, @LBUSDId, 2, '2025-26',
    1, 'Long Beach Unified School District', '19-64733-6019567', 'Jefferson Elementary',
    'TEACHER', '3-5', 'Multiple Subject',
    1, 0, NULL,  -- Same LEA, Different School (transferred), Year in progress
    '{"salary": 65000, "contractType": "Probationary Year 2", "transferReason": "Closer to home"}',
    -1
);

-- Candidate 3 (Ashley) - Year 1 outcome: LEFT TEACHING (attrition case)
INSERT INTO CandidateYearOutcome (
    CandidateId, EmployerOrgId, OutcomeYear, FiscalYear,
    IsEmployed, EmployerName, SchoolCdsCode, SchoolName,
    Position, GradeLevel, SubjectArea,
    IsSameLEAAsPlacement, IsSameSchoolAsPlacement, HasCompletedYear,
    ReasonForLeaving, ExtendedDataJson, CreatedByAccountId
) VALUES (
    @Candidate3Id, @SacCityId, 1, '2024-25',
    1, 'Sacramento City Unified School District', '34-67439-3430123', 'Sutter Middle School',
    'TEACHER', '6-8', 'Education Specialist',
    1, 1, 1,
    NULL,  -- Completed Year 1
    '{"salary": 58000, "contractType": "Probationary", "caseload": 18}',
    -1
);

-- Year 2: Left teaching
INSERT INTO CandidateYearOutcome (
    CandidateId, EmployerOrgId, OutcomeYear, FiscalYear,
    IsEmployed, EmployerName, SchoolCdsCode, SchoolName,
    Position, GradeLevel, SubjectArea,
    IsSameLEAAsPlacement, IsSameSchoolAsPlacement, HasCompletedYear,
    ReasonForLeaving, ExtendedDataJson, CreatedByAccountId
) VALUES (
    @Candidate3Id, NULL, 2, '2025-26',
    0, NULL, NULL, NULL,  -- Not employed in education
    NULL, NULL, NULL,
    0, 0, 0,
    'Left teaching - moved to private sector (EdTech company)',
    '{"lastKnownEmployer": "Clever Inc.", "lastKnownRole": "Customer Success Manager"}',
    -1
);

-- ============================================================================
-- 8. DISBURSEMENTS (Multi-Phase Payments)
-- ============================================================================

-- Candidate 1 - Both phases completed
INSERT INTO Disbursement (
    CandidateId, SubmissionId, StatusId, Phase, Sequence,
    Amount, PercentOfTotal,
    PONumber, POAmount, POIssuedOn,
    InvoiceNumber, InvoiceAmount, InvoiceIssuedOn,
    WarrantNumber, WarrantAmount, WarrantIssuedOn, WarrantConfirmedOn,
    CreatedByAccountId
) VALUES (
    @Candidate1Id, NULL, 6, 1, 1,  -- Phase 1, Status: COMPLETE
    18000.00, 50.00,
    '6360-0000005521', 18000.00, '2023-08-15',
    '2021TRIE552-1', 18000.00, '2023-08-20',
    '60582001', 18000.00, '2023-09-05', '2023-09-10',
    -1
);

INSERT INTO Disbursement (
    CandidateId, SubmissionId, StatusId, Phase, Sequence,
    Amount, PercentOfTotal,
    PONumber, POAmount, POIssuedOn,
    InvoiceNumber, InvoiceAmount, InvoiceIssuedOn,
    WarrantNumber, WarrantAmount, WarrantIssuedOn, WarrantConfirmedOn,
    CreatedByAccountId
) VALUES (
    @Candidate1Id, NULL, 6, 2, 2,  -- Phase 2, Status: COMPLETE
    18000.00, 50.00,
    '6360-0000005521', 18000.00, '2024-06-01',
    '2021TRIE552-2', 18000.00, '2024-06-15',
    '60583045', 18000.00, '2024-07-01', '2024-07-10',
    -1
);

-- Candidate 2 - Phase 1 complete, Phase 2 pending
INSERT INTO Disbursement (
    CandidateId, SubmissionId, StatusId, Phase, Sequence,
    Amount, PercentOfTotal,
    PONumber, POAmount, POIssuedOn,
    InvoiceNumber, InvoiceAmount, InvoiceIssuedOn,
    WarrantNumber, WarrantAmount, WarrantIssuedOn, WarrantConfirmedOn,
    CreatedByAccountId
) VALUES (
    @Candidate2Id, NULL, 6, 1, 1,
    19000.00, 50.00,
    '6360-0000005890', 19000.00, '2024-08-20',
    '2024TRIE890-1', 19000.00, '2024-08-25',
    '60589012', 19000.00, '2024-09-10', '2024-09-15',
    -1
);

INSERT INTO Disbursement (
    CandidateId, SubmissionId, StatusId, Phase, Sequence,
    Amount, PercentOfTotal,
    CreatedByAccountId
) VALUES (
    @Candidate2Id, NULL, 1, 2, 2,  -- Phase 2, Status: PENDING (awaiting completion)
    19000.00, 50.00,
    -1
);

-- ============================================================================
-- 9. VALIDATION QUERIES
-- ============================================================================

-- Query 1: TRP Program Summary by Cohort
/*
SELECT
    c.CohortYear,
    COUNT(*) AS TotalResidents,
    SUM(CASE WHEN c.StatusId = 8 THEN 1 ELSE 0 END) AS InProgram,
    SUM(CASE WHEN c.StatusId IN (12, 13) THEN 1 ELSE 0 END) AS Completed,
    SUM(CASE WHEN c.StatusId = 99 THEN 1 ELSE 0 END) AS EarlyExit,
    SUM(c.AwardedAmount) AS TotalAwarded
FROM Candidate c
JOIN GrantCycle gc ON c.GrantCycleId = gc.Id
JOIN GrantProgram gp ON gc.GrantProgramId = gp.Id
WHERE gp.Code = 'TRP'
GROUP BY c.CohortYear;
*/

-- Query 2: Funding by Category and Source
/*
SELECT
    c.FirstName + ' ' + c.LastName AS Resident,
    cf.FiscalYear,
    cf.FundingSource,
    cf.Category,
    cf.BudgetedAmount,
    cf.ExpendedAmount,
    cf.BudgetedAmount - ISNULL(cf.ExpendedAmount, 0) AS Remaining
FROM CandidateFunding cf
JOIN Candidate c ON cf.CandidateId = c.Id
ORDER BY c.LastName, cf.FiscalYear, cf.FundingSource, cf.Category;
*/

-- Query 3: 4-Year Retention Analysis
/*
SELECT
    cyo.OutcomeYear,
    COUNT(*) AS TotalTracked,
    SUM(CASE WHEN cyo.IsEmployed = 1 THEN 1 ELSE 0 END) AS StillTeaching,
    SUM(CASE WHEN cyo.IsSameLEAAsPlacement = 1 THEN 1 ELSE 0 END) AS SameLEA,
    SUM(CASE WHEN cyo.IsSameSchoolAsPlacement = 1 THEN 1 ELSE 0 END) AS SameSchool,
    CAST(SUM(CASE WHEN cyo.IsEmployed = 1 THEN 1.0 ELSE 0 END) / COUNT(*) * 100 AS DECIMAL(5,2)) AS RetentionRate
FROM CandidateYearOutcome cyo
JOIN Candidate c ON cyo.CandidateId = c.Id
JOIN GrantCycle gc ON c.GrantCycleId = gc.Id
JOIN GrantProgram gp ON gc.GrantProgramId = gp.Id
WHERE gp.Code = 'TRP'
GROUP BY cyo.OutcomeYear
ORDER BY cyo.OutcomeYear;
*/

-- Query 4: Grant vs Match Funding Totals
/*
SELECT
    cf.FiscalYear,
    SUM(CASE WHEN cf.FundingSource = 'GRANT' THEN cf.ExpendedAmount ELSE 0 END) AS GrantExpended,
    SUM(CASE WHEN cf.FundingSource = 'MATCH' THEN cf.ExpendedAmount ELSE 0 END) AS MatchExpended,
    SUM(cf.ExpendedAmount) AS TotalExpended,
    CAST(SUM(CASE WHEN cf.FundingSource = 'MATCH' THEN cf.ExpendedAmount ELSE 0 END) /
         NULLIF(SUM(CASE WHEN cf.FundingSource = 'GRANT' THEN cf.ExpendedAmount ELSE 0 END), 0) * 100
         AS DECIMAL(5,2)) AS MatchRatio
FROM CandidateFunding cf
JOIN Candidate c ON cf.CandidateId = c.Id
JOIN GrantCycle gc ON c.GrantCycleId = gc.Id
JOIN GrantProgram gp ON gc.GrantProgramId = gp.Id
WHERE gp.Code = 'TRP'
GROUP BY cf.FiscalYear;
*/

-- ============================================================================
-- END OF TRP SIMULATED DATA
-- ============================================================================
