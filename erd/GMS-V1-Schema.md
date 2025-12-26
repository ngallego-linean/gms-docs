# GMS V1 Database Schema

**California Commission on Teacher Credentialing**
**Grant Management System - Version 1.0**
**December 2025**

---

## Overview

This schema is designed for V1 (STSP grant) while supporting future configurability (V2+). All entities follow R4 framework standards as implemented in CRISP.

### R4 Compliance Standards Applied

| Standard | Requirement | Applied |
|----------|-------------|---------|
| DB-114 | 5 standard audit fields on ALL tables | Yes |
| DB-113 | Archive fields on archivable entities | Yes |
| DB-110 | DateTime fields suffixed with "On" | Yes |
| DB-108 | Bit fields prefixed with "Is/Has/Can" | Yes |
| DB-201 | SVT tables suffixed with "SVT" | Yes |
| DB-202 | SVT standard structure | Yes |
| SA-111 | Domain models suffixed with "Model" | Yes |

---

## Table of Contents

1. [Static Value Tables (SVTs)](#1-static-value-tables-svts)
2. [Core Entities](#2-core-entities)
3. [Application Entities](#3-application-entities)
4. [Batch & Disbursement](#4-batch--disbursement)
5. [Reporting Entities](#5-reporting-entities)
6. [File & Document Storage](#6-file--document-storage)
7. [Status Change History](#7-status-change-history)
8. [Supporting Entities](#8-supporting-entities)
9. [V2 Extension Tables](#9-v2-extension-tables)
10. [Seed Data](#10-seed-data)

---

## 1. Static Value Tables (SVTs)

All SVT tables follow the R4 standard structure from DB-202.

### 1.1 OrganizationTypeSVT

```sql
CREATE TABLE OrganizationTypeSVT (
    -- Primary Key
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    -- Standard SVT Fields
    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    -- Archive Fields (DB-113)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByAccountId     INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,

    -- Standard Audit Fields (DB-114)
    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

**Seed Values:**
| Id | Name | Description |
|----|------|-------------|
| 1 | IHE | Institution of Higher Education |
| 2 | LEA | Local Education Agency |
| 3 | COE | County Office of Education |
| 4 | CTC | Commission on Teacher Credentialing |

---

### 1.2 CandidateStatusSVT

```sql
CREATE TABLE CandidateStatusSVT (
    -- Primary Key
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    -- Standard SVT Fields
    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    -- Extended Status Fields
    Code                    VARCHAR(30) NOT NULL UNIQUE,
    Category                VARCHAR(20) NOT NULL,  -- SUBMISSION, REVIEW, DISBURSEMENT, REPORTING, TERMINAL
    LEADisplayCategory      VARCHAR(20) NULL,      -- Simplified view for LEA portal: UNDER_REVIEW, APPROVED, PAID
    IsTerminal              BIT NOT NULL DEFAULT 0,
    AllowedTransitionsJson  NVARCHAR(MAX) NULL,    -- JSON array of allowed next status codes

    -- Archive Fields
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByAccountId     INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

**Seed Values (STSP):**
| Id | Code | Name | Category | LEADisplayCategory | DisplayOrder |
|----|------|------|----------|-------------------|--------------|
| 1 | DRAFT | Draft | SUBMISSION | UNDER_REVIEW | 1 |
| 2 | IHE_SUBMITTED | Submitted to LEA | SUBMISSION | UNDER_REVIEW | 2 |
| 3 | LEA_REVIEWING | LEA Reviewing | SUBMISSION | UNDER_REVIEW | 3 |
| 4 | RETURNED_TO_IHE | Returned to IHE | SUBMISSION | UNDER_REVIEW | 4 |
| 5 | LEA_APPROVED | LEA Approved | SUBMISSION | UNDER_REVIEW | 5 |
| 6 | CTC_SUBMITTED | Submitted to CTC | REVIEW | UNDER_REVIEW | 6 |
| 7 | CTC_REVIEWING | Under Review | REVIEW | UNDER_REVIEW | 7 |
| 8 | APPROVED | Approved | REVIEW | APPROVED | 8 |
| 9 | REJECTED | Rejected | TERMINAL | NULL | 99 |
| 10 | REPORTING_DUE | Reports Due | REPORTING | PAID | 10 |
| 11 | REPORTS_COMPLETE | Reports Complete | REPORTING | PAID | 11 |
| 12 | CLOSED | Closed | TERMINAL | PAID | 100 |

---

### 1.3 BatchStatusSVT

```sql
CREATE TABLE BatchStatusSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,
    Category                VARCHAR(20) NOT NULL,
    IsTerminal              BIT NOT NULL DEFAULT 0,
    AllowedTransitionsJson  NVARCHAR(MAX) NULL,

    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByAccountId     INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

**Seed Values:**
| Id | Code | Name | Category |
|----|------|------|----------|
| 1 | DRAFT | Draft | SUBMISSION |
| 2 | PENDING_REVIEW | Pending Review | REVIEW |
| 3 | APPROVED | Approved | REVIEW |
| 4 | REJECTED | Rejected | TERMINAL |
| 5 | GAA_SENT | GAA Sent | DISBURSEMENT |
| 6 | GAA_SIGNED | GAA Signed | DISBURSEMENT |
| 7 | PO_PENDING | PO Pending | DISBURSEMENT |
| 8 | PO_UPLOADED | PO Uploaded | DISBURSEMENT |
| 9 | INVOICE_GENERATED | Invoice Generated | DISBURSEMENT |
| 10 | INVOICE_SENT | Invoice Sent | DISBURSEMENT |
| 11 | WARRANT_PENDING | Warrant Pending | DISBURSEMENT |
| 12 | COMPLETE | Complete | TERMINAL |

---

### 1.4 DisbursementStatusSVT

```sql
CREATE TABLE DisbursementStatusSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,

    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByAccountId     INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

---

### 1.5 FileTypeSVT

```sql
CREATE TABLE FileTypeSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByAccountId     INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

**Seed Values:**
| Id | Name | Description |
|----|------|-------------|
| 1 | GAA_DOCUMENT | Grant Award Agreement |
| 2 | GAA_SIGNED | Signed Grant Award Agreement |
| 3 | PO_DOCUMENT | Purchase Order |
| 4 | INVOICE | Invoice Document |
| 5 | BATCH_EXPORT | Batch Export CSV |
| 6 | REPORT_ATTACHMENT | Report Supporting Document |

---

### 1.6 ReportTypeSVT

```sql
CREATE TABLE ReportTypeSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,

    -- Report Configuration
    ReportingOrgTypeId      INT NULL REFERENCES OrganizationTypeSVT(Id),  -- Which org type submits this report (IHE=1, LEA=2)
    FormSchemaJson          NVARCHAR(MAX) NULL,  -- JSON Schema defining required fields for this report type

    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByAccountId     INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

**Seed Values:**
| Id | Code | Name | ReportingOrgTypeId |
|----|------|------|--------------------|
| 1 | IHE_COMPLETION | IHE Completion Report | 1 (IHE) |
| 2 | LEA_PAYMENT | LEA Payment Report | 2 (LEA) |

---

### 1.7 UserRoleSVT

```sql
CREATE TABLE UserRoleSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,

    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByAccountId     INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

**Seed Values:**
| Id | Code | Name |
|----|------|------|
| 1 | IHE_USER | IHE User |
| 2 | LEA_USER | LEA User |
| 3 | CTC_GRANTS | CTC Grants Team |
| 4 | CTC_FISCAL | CTC Fiscal Team |
| 5 | CTC_ADMIN | CTC Administrator |

---

### 1.8 NotificationTypeSVT

```sql
CREATE TABLE NotificationTypeSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(50) NOT NULL UNIQUE,
    SubjectTemplate         NVARCHAR(500) NULL,
    BodyTemplate            NVARCHAR(MAX) NULL,

    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByAccountId     INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

---

### 1.9 BatchTimelineEventTypeSVT

Tracks workflow timeline events for batch processing (from prototype).

```sql
CREATE TABLE BatchTimelineEventTypeSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,

    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByAccountId     INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

**Seed Values:**
| Id | Code | Name | DisplayOrder |
|----|------|------|--------------|
| 1 | BATCH_CREATED | Batch Created | 1 |
| 2 | LEA_REVIEW | Sent to LEA | 2 |
| 3 | CTC_SUBMITTED | Submitted to CTC | 3 |
| 4 | CTC_APPROVED | Approved by CTC | 4 |
| 5 | REPORTING | Entered Reporting | 5 |
| 6 | COMPLETE | Processing Complete | 6 |

---

## 2. Core Entities

### 2.1 GrantProgram

Represents a grant type (STSP, Teacher Residency, etc.). V1 has single row for STSP.

```sql
CREATE TABLE GrantProgram (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Identification
    Code                        VARCHAR(20) NOT NULL UNIQUE,
    Name                        VARCHAR(200) NOT NULL,
    Description                 VARCHAR(2000) NULL,

    -- Workflow Configuration
    WorkflowType                VARCHAR(20) NOT NULL DEFAULT 'IHE_LEA_CTC',
    HasInitiatorOrg             BIT NOT NULL DEFAULT 1,
    HasCompleterOrg             BIT NOT NULL DEFAULT 1,
    HasBatching                 BIT NOT NULL DEFAULT 1,

    -- Evaluation Configuration
    EvaluationType              VARCHAR(20) NOT NULL DEFAULT 'ELIGIBILITY',

    -- Funding Configuration
    AwardUnit                   VARCHAR(20) NOT NULL DEFAULT 'PER_CANDIDATE',
    DisbursementType            VARCHAR(20) NOT NULL DEFAULT 'ONE_TIME',
    HasMatchingFunds            BIT NOT NULL DEFAULT 0,

    -- Tracking Configuration
    TrackingYears               INT NOT NULL DEFAULT 0,
    HasCohorts                  BIT NOT NULL DEFAULT 0,
    HasReplacements             BIT NOT NULL DEFAULT 0,

    -- Batching Configuration
    BatchStrategy               VARCHAR(30) NOT NULL DEFAULT 'MONTHLY',  -- NONE, MONTHLY, QUARTERLY, ON_DEMAND, COHORT
    BatchInitiatorOrgType       VARCHAR(10) NULL,                        -- Which org type creates batches: IHE, LEA, CTC

    -- Program-Specific Thresholds (queryable)
    HoursThreshold              INT NULL,                                -- e.g., 500 for STSP grant hours
    CredentialHoursThreshold    INT NULL,                                -- e.g., 600 for STSP credential hours

    -- Complex Configuration (JSON)
    PaymentPhasesJson           NVARCHAR(MAX) NULL,  -- For multi-phase: [{"phase":1,"percent":25,"trigger":"ON_APPROVAL"}, ...]
    ReportRequirementsJson      NVARCHAR(MAX) NULL,  -- Report due dates/requirements per org type
    ProgramConfigJson           NVARCHAR(MAX) NULL,  -- Additional program-specific config values

    -- Form Configuration (V2)
    CandidateFormSchemaJson     NVARCHAR(MAX) NULL,
    ReportFormSchemaJson        NVARCHAR(MAX) NULL,

    -- Status
    IsActive                    BIT NOT NULL DEFAULT 1,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);
```

---

### 2.2 GrantCycle

A specific funding period for a grant program.

```sql
CREATE TABLE GrantCycle (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    GrantProgramId              INT NOT NULL REFERENCES GrantProgram(Id),

    -- Identification
    Name                        VARCHAR(200) NOT NULL,
    FiscalYear                  VARCHAR(10) NOT NULL,

    -- Funding
    AppropriatedAmount          DECIMAL(18,2) NOT NULL,
    DefaultAwardAmount          DECIMAL(18,2) NOT NULL,
    MinAwardAmount              DECIMAL(18,2) NULL,
    MaxAwardAmount              DECIMAL(18,2) NULL,

    -- Eligibility (V2: RulesDsl)
    EligibilityRulesJson        NVARCHAR(MAX) NULL,

    -- Dates
    ApplicationOpenOn           DATETIME NULL,
    ApplicationCloseOn          DATETIME NULL,
    CycleStartOn                DATETIME NULL,
    CycleEndOn                  DATETIME NULL,

    -- Status
    Status                      VARCHAR(20) NOT NULL DEFAULT 'DRAFT',

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_GrantCycle_GrantProgramId ON GrantCycle(GrantProgramId);
CREATE INDEX IX_GrantCycle_FiscalYear ON GrantCycle(FiscalYear);
```

---

### 2.3 Organization

An IHE or LEA organization. Synced from ECS.

```sql
CREATE TABLE Organization (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    TypeId                      INT NOT NULL REFERENCES OrganizationTypeSVT(Id),

    -- ECS Integration
    EcsCdsCode                  VARCHAR(14) NULL,
    EcsOrganizationId           INT NULL,

    -- Identification
    Name                        VARCHAR(200) NOT NULL,

    -- Address (per Common Data Fields Guidelines)
    StreetAddress               NVARCHAR(80) NULL,
    City                        NVARCHAR(40) NULL,
    County                      NVARCHAR(50) NULL,     -- California county (for geographic reporting)
    Region                      NVARCHAR(50) NULL,     -- CTC region designation (e.g., "Northern", "Southern", "Bay Area")
    StateProvince               NVARCHAR(40) NULL,
    PostalCode                  NVARCHAR(40) NULL,
    CountryId                   INT NULL DEFAULT 226,  -- USA

    -- Contact
    PrimaryEmailAddress         VARCHAR(320) NULL,
    PrimaryPhoneNumber          VARCHAR(25) NULL,
    EmailDomain                 VARCHAR(100) NULL,

    -- Status
    IsActive                    BIT NOT NULL DEFAULT 1,
    LastEcsSyncOn               DATETIME NULL,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_Organization_TypeId ON Organization(TypeId);
CREATE INDEX IX_Organization_EcsCdsCode ON Organization(EcsCdsCode);
CREATE INDEX IX_Organization_Name ON Organization(Name);
CREATE INDEX IX_Organization_County ON Organization(County) WHERE County IS NOT NULL;
CREATE INDEX IX_Organization_Region ON Organization(Region) WHERE Region IS NOT NULL;
```

---

### 2.4 Account

Portal users (IHE/LEA staff, CTC staff). Named "Account" per CRISP pattern.

```sql
CREATE TABLE Account (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    OrganizationId              INT NULL REFERENCES Organization(Id),
    RoleId                      INT NOT NULL REFERENCES UserRoleSVT(Id),

    -- Identity
    EmailAddress                VARCHAR(320) NOT NULL UNIQUE,
    FirstName                   NVARCHAR(50) NOT NULL,
    LastName                    NVARCHAR(50) NOT NULL,
    PhoneNumber                 VARCHAR(25) NULL,
    Title                       VARCHAR(200) NULL,

    -- Authentication
    EntraId                     VARCHAR(100) NULL,  -- For CTC staff SSO
    OAuthProvider               VARCHAR(50) NULL,   -- google, microsoft
    OAuthId                     VARCHAR(200) NULL,

    -- Status
    IsActive                    BIT NOT NULL DEFAULT 1,
    LastLoginOn                 DATETIME NULL,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_Account_OrganizationId ON Account(OrganizationId);
CREATE INDEX IX_Account_RoleId ON Account(RoleId);
CREATE INDEX IX_Account_EmailAddress ON Account(EmailAddress);
```

---

## 3. Application Entities

### 3.1 Application

An IHE-LEA pair for a grant cycle.

```sql
CREATE TABLE Application (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    GrantCycleId                INT NOT NULL REFERENCES GrantCycle(Id),

    -- Abstracted Org Roles (for configurability)
    InitiatorOrgId              INT NULL REFERENCES Organization(Id),
    CompleterOrgId              INT NULL REFERENCES Organization(Id),

    -- Convenience References (for STSP queries/reports)
    IHEOrgId                    INT NULL REFERENCES Organization(Id),
    LEAOrgId                    INT NULL REFERENCES Organization(Id),

    -- Primary Contacts (system users who log in)
    InitiatorContactId          INT NULL REFERENCES Account(Id),
    CompleterContactId          INT NULL REFERENCES Account(Id),

    -- GAA Signer (critical for DocuSign - may not be a system user)
    GAASignerName               NVARCHAR(200) NULL,
    GAASignerTitle              VARCHAR(200) NULL,
    GAASignerEmail              VARCHAR(320) NULL,

    -- Extended Contacts (JSON for Fiscal Agent, Superintendent, etc.)
    ExtendedContactsJson        NVARCHAR(MAX) NULL,  -- {"fiscalAgent": {"name":"...", "email":"...", "phone":"..."}, "superintendent": {...}}

    -- Status
    Status                      VARCHAR(20) NOT NULL DEFAULT 'DRAFT',

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_Application_GrantCycleId ON Application(GrantCycleId);
CREATE INDEX IX_Application_IHEOrgId ON Application(IHEOrgId);
CREATE INDEX IX_Application_LEAOrgId ON Application(LEAOrgId);
CREATE INDEX IX_Application_GAASignerEmail ON Application(GAASignerEmail) WHERE GAASignerEmail IS NOT NULL;
CREATE UNIQUE INDEX IX_Application_Unique ON Application(GrantCycleId, IHEOrgId, LEAOrgId)
    WHERE IsArchived = 0;
```

---

### 3.2 Candidate

An individual applying for the grant (student teacher, resident, participant).

```sql
CREATE TABLE Candidate (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    ApplicationId               INT NOT NULL REFERENCES Application(Id),
    StatusId                    INT NOT NULL REFERENCES CandidateStatusSVT(Id),

    -- Core Identity
    FirstName                   NVARCHAR(50) NOT NULL,
    LastName                    NVARCHAR(50) NOT NULL,
    SEID                        VARCHAR(10) NULL,
    DateOfBirth                 DATETIME NULL,
    Last4SSN                    VARCHAR(4) NULL,
    EmailAddress                VARCHAR(320) NULL,

    -- Standard Demographics
    Race                        VARCHAR(100) NULL,
    Ethnicity                   VARCHAR(100) NULL,
    Gender                      VARCHAR(50) NULL,

    -- STSP-Specific (V1)
    CredentialArea              VARCHAR(100) NULL,
    SchoolCdsCode               VARCHAR(14) NULL,
    SchoolName                  VARCHAR(200) NULL,

    -- Scoring (V2 - for competitive grants)
    Score                       DECIMAL(10,2) NULL,
    ScoreDetailsJson            NVARCHAR(MAX) NULL,
    Rank                        INT NULL,

    -- Multi-Grant Support (V2)
    CohortYear                  VARCHAR(10) NULL,
    ParticipantType             VARCHAR(20) NULL DEFAULT 'CANDIDATE',
    ReplacedCandidateId         INT NULL REFERENCES Candidate(Id),
    ReplacementReason           VARCHAR(500) NULL,

    -- Financial
    RequestedAmount             DECIMAL(18,2) NULL,
    AwardedAmount               DECIMAL(18,2) NULL,

    -- Extension Point (V2)
    ExtendedDataJson            NVARCHAR(MAX) NULL,

    -- Workflow Timestamps
    StatusChangedOn             DATETIME NULL,
    StatusChangedByAccountId    INT NULL REFERENCES Account(Id),
    SubmittedByInitiatorOn      DATETIME NULL,
    SubmittedByCompleterOn      DATETIME NULL,
    SubmittedToCTCOn            DATETIME NULL,
    ApprovedOn                  DATETIME NULL,
    ApprovedByAccountId         INT NULL REFERENCES Account(Id),
    RejectedOn                  DATETIME NULL,
    RejectedByAccountId         INT NULL REFERENCES Account(Id),
    RejectionReason             VARCHAR(1000) NULL,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_Candidate_ApplicationId ON Candidate(ApplicationId);
CREATE INDEX IX_Candidate_StatusId ON Candidate(StatusId);
CREATE INDEX IX_Candidate_SEID ON Candidate(SEID);
CREATE INDEX IX_Candidate_SchoolCdsCode ON Candidate(SchoolCdsCode);
```

---

## 4. Batch & Disbursement

### 4.1 Batch

LEA's monthly submission to CTC. One GAA per batch.

```sql
CREATE TABLE Batch (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    GrantCycleId                INT NOT NULL REFERENCES GrantCycle(Id),
    StatusId                    INT NOT NULL REFERENCES BatchStatusSVT(Id),
    SubmittingOrgId             INT NOT NULL REFERENCES Organization(Id),
    LEAOrgId                    INT NULL REFERENCES Organization(Id),

    -- Identification
    BatchNumber                 VARCHAR(20) NULL,
    SubmissionPeriod            VARCHAR(10) NOT NULL,  -- "2025-11" (Year-Month)
    BatchType                   VARCHAR(20) NOT NULL DEFAULT 'MONTHLY',

    -- Aggregates (denormalized for performance)
    CandidateCount              INT NOT NULL DEFAULT 0,
    TotalAwardAmount            DECIMAL(18,2) NOT NULL DEFAULT 0,

    -- Workflow Timestamps
    StatusChangedOn             DATETIME NULL,
    StatusChangedByAccountId    INT NULL REFERENCES Account(Id),
    SubmittedOn                 DATETIME NULL,
    SubmittedByAccountId        INT NULL REFERENCES Account(Id),
    ApprovedOn                  DATETIME NULL,
    ApprovedByAccountId         INT NULL REFERENCES Account(Id),
    RejectedOn                  DATETIME NULL,
    RejectedByAccountId         INT NULL REFERENCES Account(Id),
    RejectionReason             VARCHAR(1000) NULL,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_Batch_GrantCycleId ON Batch(GrantCycleId);
CREATE INDEX IX_Batch_StatusId ON Batch(StatusId);
CREATE INDEX IX_Batch_SubmittingOrgId ON Batch(SubmittingOrgId);
CREATE INDEX IX_Batch_SubmissionPeriod ON Batch(SubmissionPeriod);
```

---

### 4.2 BatchCandidate

Join table linking Batch to Candidate.

```sql
CREATE TABLE BatchCandidate (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    BatchId                     INT NOT NULL REFERENCES Batch(Id),
    CandidateId                 INT NOT NULL REFERENCES Candidate(Id),

    -- Tracking
    AddedOn                     DATETIME NOT NULL DEFAULT GETDATE(),
    AddedByAccountId            INT NOT NULL REFERENCES Account(Id),

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_BatchCandidate_BatchId ON BatchCandidate(BatchId);
CREATE UNIQUE INDEX IX_BatchCandidate_CandidateId ON BatchCandidate(CandidateId)
    WHERE IsArchived = 0;
```

---

### 4.3 Disbursement

Payment processing record. V1: One per batch. V2: Multiple for phased.

```sql
CREATE TABLE Disbursement (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    BatchId                     INT NOT NULL REFERENCES Batch(Id),
    StatusId                    INT NOT NULL REFERENCES DisbursementStatusSVT(Id),

    -- Phase/Sequence (V2 support)
    Phase                       VARCHAR(20) NOT NULL DEFAULT 'FULL',
    Sequence                    INT NOT NULL DEFAULT 1,

    -- Amounts
    Amount                      DECIMAL(18,2) NOT NULL,
    PercentOfTotal              DECIMAL(5,2) NULL,

    -- GAA
    GAADocumentStorageKey       UNIQUEIDENTIFIER NULL,
    GAAEnvelopeId               VARCHAR(100) NULL,
    GAAStatus                   VARCHAR(30) NULL,
    GAASentOn                   DATETIME NULL,
    GAASignedOn                 DATETIME NULL,
    GAACompletedOn              DATETIME NULL,

    -- PO
    PONumber                    VARCHAR(50) NULL,
    POAmount                    DECIMAL(18,2) NULL,
    POIssuedOn                  DATETIME NULL,
    PODocumentStorageKey        UNIQUEIDENTIFIER NULL,
    POUploadedOn                DATETIME NULL,
    POUploadedByAccountId       INT NULL REFERENCES Account(Id),

    -- Invoice
    InvoiceNumber               VARCHAR(50) NULL,
    InvoiceAmount               DECIMAL(18,2) NULL,
    InvoiceIssuedOn             DATETIME NULL,
    InvoiceDocumentStorageKey   UNIQUEIDENTIFIER NULL,
    InvoiceSentOn               DATETIME NULL,

    -- Warrant
    WarrantNumber               VARCHAR(50) NULL,
    WarrantAmount               DECIMAL(18,2) NULL,
    WarrantIssuedOn             DATETIME NULL,
    WarrantConfirmedOn          DATETIME NULL,
    WarrantConfirmedByAccountId INT NULL REFERENCES Account(Id),

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_Disbursement_BatchId ON Disbursement(BatchId);
CREATE INDEX IX_Disbursement_StatusId ON Disbursement(StatusId);
CREATE INDEX IX_Disbursement_GAAEnvelopeId ON Disbursement(GAAEnvelopeId);
```

---

## 5. Reporting Entities

### 5.1 ReportingPeriod

When reports are due.

```sql
CREATE TABLE ReportingPeriod (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    GrantCycleId                INT NOT NULL REFERENCES GrantCycle(Id),
    ReportTypeId                INT NOT NULL REFERENCES ReportTypeSVT(Id),

    -- Identification
    PeriodName                  VARCHAR(100) NOT NULL,

    -- Dates
    StartOn                     DATETIME NULL,
    DueOn                       DATETIME NOT NULL,

    -- Status
    IsActive                    BIT NOT NULL DEFAULT 1,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_ReportingPeriod_GrantCycleId ON ReportingPeriod(GrantCycleId);
CREATE INDEX IX_ReportingPeriod_ReportTypeId ON ReportingPeriod(ReportTypeId);
```

---

### 5.2 CandidateReport

Per-candidate report (IHE completion or LEA payment).

```sql
CREATE TABLE CandidateReport (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    CandidateId                 INT NOT NULL REFERENCES Candidate(Id),
    ReportingPeriodId           INT NOT NULL REFERENCES ReportingPeriod(Id),
    ReportingOrgId              INT NOT NULL REFERENCES Organization(Id),
    ReportTypeId                INT NOT NULL REFERENCES ReportTypeSVT(Id),
    ReportingOrgTypeId          INT NOT NULL REFERENCES OrganizationTypeSVT(Id),  -- IHE=1, LEA=2 (denormalized for query performance)

    -- Standard Fields (STSP)
    CompletionStatus            VARCHAR(30) NULL,
    CompletionOn                DATETIME NULL,
    GrantProgramHours           INT NULL,
    CredentialProgramHours      INT NULL,
    HasCredentialEarned         BIT NULL,
    CredentialEarnedOn          DATETIME NULL,
    EmploymentStatus            VARCHAR(50) NULL,
    IsHiredByPartnerOrg         BIT NULL,
    PaymentCategory             VARCHAR(50) NULL,
    PaymentAmount               DECIMAL(18,2) NULL,
    PaymentOn                   DATETIME NULL,

    -- Extension Point (V2)
    ExtendedDataJson            NVARCHAR(MAX) NULL,

    -- Workflow
    Status                      VARCHAR(20) NOT NULL DEFAULT 'DRAFT',
    SubmittedOn                 DATETIME NULL,
    SubmittedByAccountId        INT NULL REFERENCES Account(Id),
    ApprovedOn                  DATETIME NULL,
    ApprovedByAccountId         INT NULL REFERENCES Account(Id),
    RevisionNotes               VARCHAR(2000) NULL,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_CandidateReport_CandidateId ON CandidateReport(CandidateId);
CREATE INDEX IX_CandidateReport_ReportingPeriodId ON CandidateReport(ReportingPeriodId);
CREATE INDEX IX_CandidateReport_ReportTypeId ON CandidateReport(ReportTypeId);
CREATE INDEX IX_CandidateReport_ReportingOrgTypeId ON CandidateReport(ReportingOrgTypeId);
```

---

## 6. File & Document Storage

### 6.1 File

Central document storage following R4 Common Data Fields Guidelines.

```sql
CREATE TABLE [File] (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    TypeId                      INT NOT NULL REFERENCES FileTypeSVT(Id),

    -- Document Metadata (per Common Data Fields Guidelines)
    DocumentDate                DATETIME NULL,
    Title                       VARCHAR(300) NULL,
    StorageKey                  UNIQUEIDENTIFIER NOT NULL,
    [FileName]                  VARCHAR(255) NOT NULL,
    FileSizeInKilobytes         INT NOT NULL,
    FormatDescription           VARCHAR(100) NULL,
    RelativePath                VARCHAR(500) NULL,

    -- Content Hash (for deduplication)
    ContentHash                 VARCHAR(64) NULL,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_File_TypeId ON [File](TypeId);
CREATE INDEX IX_File_StorageKey ON [File](StorageKey);
```

---

### 6.2 DisbursementFile

Junction table linking Disbursement to Files.

```sql
CREATE TABLE DisbursementFile (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    DisbursementId              INT NOT NULL REFERENCES Disbursement(Id),
    FileId                      INT NOT NULL REFERENCES [File](Id),

    -- Purpose
    Purpose                     VARCHAR(50) NOT NULL,  -- GAA, GAA_SIGNED, PO, INVOICE

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_DisbursementFile_DisbursementId ON DisbursementFile(DisbursementId);
CREATE INDEX IX_DisbursementFile_FileId ON DisbursementFile(FileId);
```

---

### 6.3 CandidateReportFile

Junction table linking CandidateReport to Files.

```sql
CREATE TABLE CandidateReportFile (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    CandidateReportId           INT NOT NULL REFERENCES CandidateReport(Id),
    FileId                      INT NOT NULL REFERENCES [File](Id),

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_CandidateReportFile_CandidateReportId ON CandidateReportFile(CandidateReportId);
CREATE INDEX IX_CandidateReportFile_FileId ON CandidateReportFile(FileId);
```

---

## 7. Status Change History

Following CRISP's `HelpRequestStatusChange` pattern for audit compliance.

### 7.1 CandidateStatusChange

```sql
CREATE TABLE CandidateStatusChange (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    CandidateId                 INT NOT NULL REFERENCES Candidate(Id),
    FromStatusId                INT NULL REFERENCES CandidateStatusSVT(Id),
    ToStatusId                  INT NOT NULL REFERENCES CandidateStatusSVT(Id),

    -- Change Details
    ChangedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    ChangedByAccountId          INT NOT NULL REFERENCES Account(Id),
    Comments                    VARCHAR(2000) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_CandidateStatusChange_CandidateId ON CandidateStatusChange(CandidateId);
CREATE INDEX IX_CandidateStatusChange_ChangedOn ON CandidateStatusChange(ChangedOn);
```

---

### 7.2 BatchStatusChange

```sql
CREATE TABLE BatchStatusChange (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    BatchId                     INT NOT NULL REFERENCES Batch(Id),
    FromStatusId                INT NULL REFERENCES BatchStatusSVT(Id),
    ToStatusId                  INT NOT NULL REFERENCES BatchStatusSVT(Id),

    -- Change Details
    ChangedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    ChangedByAccountId          INT NOT NULL REFERENCES Account(Id),
    Comments                    VARCHAR(2000) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_BatchStatusChange_BatchId ON BatchStatusChange(BatchId);
CREATE INDEX IX_BatchStatusChange_ChangedOn ON BatchStatusChange(ChangedOn);
```

---

### 7.3 DisbursementStatusChange

```sql
CREATE TABLE DisbursementStatusChange (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    DisbursementId              INT NOT NULL REFERENCES Disbursement(Id),
    FromStatusId                INT NULL REFERENCES DisbursementStatusSVT(Id),
    ToStatusId                  INT NOT NULL REFERENCES DisbursementStatusSVT(Id),

    -- Change Details
    ChangedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    ChangedByAccountId          INT NOT NULL REFERENCES Account(Id),
    Comments                    VARCHAR(2000) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_DisbursementStatusChange_DisbursementId ON DisbursementStatusChange(DisbursementId);
```

---

## 8. Supporting Entities

### 8.1 Notification

Email/system notifications.

```sql
CREATE TABLE Notification (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    TypeId                      INT NOT NULL REFERENCES NotificationTypeSVT(Id),
    RecipientAccountId          INT NULL REFERENCES Account(Id),

    -- Related Entities (nullable - at least one should be set)
    RelatedBatchId              INT NULL REFERENCES Batch(Id),
    RelatedCandidateId          INT NULL REFERENCES Candidate(Id),
    RelatedApplicationId        INT NULL REFERENCES Application(Id),

    -- Email Details
    RecipientEmailAddress       VARCHAR(320) NOT NULL,
    Subject                     NVARCHAR(500) NOT NULL,
    BodyHtml                    NVARCHAR(MAX) NOT NULL,

    -- Status
    Status                      VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    SentOn                      DATETIME NULL,
    ErrorMessage                VARCHAR(1000) NULL,
    RetryCount                  INT NOT NULL DEFAULT 0,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_Notification_TypeId ON Notification(TypeId);
CREATE INDEX IX_Notification_Status ON Notification(Status);
CREATE INDEX IX_Notification_RecipientAccountId ON Notification(RecipientAccountId);
```

---

### 8.2 AuditLog

Action history for compliance. Supplements status change tables with general auditing.

```sql
CREATE TABLE AuditLog (
    -- Primary Key
    Id                          BIGINT IDENTITY(1,1) PRIMARY KEY,

    -- What Changed
    EntityType                  VARCHAR(50) NOT NULL,
    EntityId                    INT NOT NULL,
    Action                      VARCHAR(50) NOT NULL,

    -- Change Details
    OldValueJson                NVARCHAR(MAX) NULL,
    NewValueJson                NVARCHAR(MAX) NULL,

    -- Who/When
    AccountId                   INT NULL REFERENCES Account(Id),
    IpAddress                   VARCHAR(50) NULL,
    UserAgent                   VARCHAR(500) NULL,

    -- Standard Audit Fields (simplified for log table)
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE INDEX IX_AuditLog_EntityType_EntityId ON AuditLog(EntityType, EntityId);
CREATE INDEX IX_AuditLog_CreatedOn ON AuditLog(CreatedOn);
CREATE INDEX IX_AuditLog_AccountId ON AuditLog(AccountId);
```

---

## 9. V2 Extension Tables

These tables are created in V1 but not populated until V2.

### 9.1 CandidateFunding

For grants with multiple funding categories/sources.

```sql
CREATE TABLE CandidateFunding (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    CandidateId                 INT NOT NULL REFERENCES Candidate(Id),

    -- Tracking
    FiscalYear                  VARCHAR(10) NOT NULL,
    FundingSource               VARCHAR(20) NOT NULL,  -- GRANT, MATCH, OTHER
    Category                    VARCHAR(50) NULL,

    -- Amounts
    BudgetedAmount              DECIMAL(18,2) NULL,
    ExpendedAmount              DECIMAL(18,2) NULL,

    -- Notes
    Notes                       VARCHAR(1000) NULL,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_CandidateFunding_CandidateId ON CandidateFunding(CandidateId);
```

---

### 9.2 CandidateYearProgress

For grants with annual progress tracking.

```sql
CREATE TABLE CandidateYearProgress (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    CandidateId                 INT NOT NULL REFERENCES Candidate(Id),

    -- Tracking
    FiscalYear                  VARCHAR(10) NOT NULL,
    YearInProgram               INT NOT NULL,

    -- Standard Progress
    HasDegreeProgress           BIT NULL,
    HasDegreeCompleted          BIT NULL,
    HasCredentialProgress       BIT NULL,
    HasCredentialCompleted      BIT NULL,
    HasAuthorizationProgress    BIT NULL,
    HasAuthorizationCompleted   BIT NULL,

    -- Employment/Continuation
    IsEmployedNextYear          BIT NULL,
    HasEarlyExit                BIT NULL,
    EarlyExitOn                 DATETIME NULL,
    EarlyExitReason             VARCHAR(500) NULL,

    -- Extension
    ExtendedDataJson            NVARCHAR(MAX) NULL,
    Notes                       VARCHAR(2000) NULL,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_CandidateYearProgress_CandidateId ON CandidateYearProgress(CandidateId);
CREATE INDEX IX_CandidateYearProgress_FiscalYear ON CandidateYearProgress(FiscalYear);
```

---

### 9.3 CandidateYearOutcome

For grants that track post-program employment.

```sql
CREATE TABLE CandidateYearOutcome (
    -- Primary Key
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    -- Foreign Keys
    CandidateId                 INT NOT NULL REFERENCES Candidate(Id),
    EmployerOrgId               INT NULL REFERENCES Organization(Id),

    -- Tracking
    OutcomeYear                 INT NOT NULL,
    FiscalYear                  VARCHAR(10) NOT NULL,

    -- Employment
    IsEmployed                  BIT NULL,
    EmployerName                VARCHAR(200) NULL,
    SchoolCdsCode               VARCHAR(14) NULL,
    SchoolName                  VARCHAR(200) NULL,
    Position                    VARCHAR(50) NULL,
    GradeLevel                  VARCHAR(50) NULL,
    SubjectArea                 VARCHAR(100) NULL,

    -- Retention Metrics
    IsSameLEAAsPlacement        BIT NULL,
    IsSameSchoolAsPlacement     BIT NULL,
    HasCompletedYear            BIT NULL,
    ReasonForLeaving            VARCHAR(500) NULL,

    -- Extension
    ExtendedDataJson            NVARCHAR(MAX) NULL,
    Notes                       VARCHAR(2000) NULL,

    -- Archive Fields
    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    -- Standard Audit Fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_CandidateYearOutcome_CandidateId ON CandidateYearOutcome(CandidateId);
CREATE INDEX IX_CandidateYearOutcome_OutcomeYear ON CandidateYearOutcome(OutcomeYear);
```

---

## 10. Seed Data

### 10.1 STSP GrantProgram

```sql
INSERT INTO GrantProgram (
    Code, Name, Description,
    WorkflowType, HasInitiatorOrg, HasCompleterOrg, HasBatching,
    EvaluationType, AwardUnit, DisbursementType, HasMatchingFunds,
    TrackingYears, HasCohorts, HasReplacements,
    BatchStrategy, BatchInitiatorOrgType,
    HoursThreshold, CredentialHoursThreshold,
    IsActive, CreatedByAccountId
) VALUES (
    'STSP', 'Student Teacher Stipend Program',
    'Provides $10,000 stipends to student teachers completing 540+ hours of clinical practice.',
    'IHE_LEA_CTC', 1, 1, 1,
    'ELIGIBILITY', 'PER_CANDIDATE', 'ONE_TIME', 0,
    0, 0, 0,
    'MONTHLY', 'LEA',
    500, 600,  -- Grant program hours threshold, Credential program hours threshold
    1, -1
);
```

---

## Entity Relationship Summary

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           GMS V1 ENTITY RELATIONSHIPS                        │
└─────────────────────────────────────────────────────────────────────────────┘

                              ┌──────────────┐
                              │ GrantProgram │
                              │   (Config)   │
                              └──────┬───────┘
                                     │ 1:M
                                     ▼
                              ┌──────────────┐
                              │  GrantCycle  │
                              └──────┬───────┘
                                     │
              ┌──────────────────────┼──────────────────────┐
              │ 1:M                  │ 1:M                  │ 1:M
              ▼                      ▼                      ▼
        ┌───────────┐          ┌───────────┐         ┌───────────────┐
        │Application│          │   Batch   │         │ReportingPeriod│
        └─────┬─────┘          └─────┬─────┘         └───────────────┘
              │ 1:M                  │ 1:M
              ▼                      ▼
        ┌───────────┐          ┌───────────┐
        │ Candidate │◄─────────│BatchCand. │
        └─────┬─────┘          └───────────┘
              │                      │
              │ 1:M                  │ M:1
              ▼                      ▼
        ┌───────────┐          ┌───────────┐
        │ Candidate │          │Disbursem. │
        │  Report   │          └─────┬─────┘
        └───────────┘                │ 1:M
                                     ▼
                               ┌───────────┐
                               │   File    │
                               └───────────┘

 Organization ──┬── 1:M ──▶ Account
                │
                ├── M:1 ──▶ Application.InitiatorOrgId
                ├── M:1 ──▶ Application.CompleterOrgId
                └── M:1 ──▶ Batch.SubmittingOrgId

 SVT Tables (Lookups):
   • OrganizationTypeSVT
   • CandidateStatusSVT      ──▶ CandidateStatusChange
   • BatchStatusSVT          ──▶ BatchStatusChange
   • DisbursementStatusSVT   ──▶ DisbursementStatusChange
   • FileTypeSVT
   • ReportTypeSVT
   • UserRoleSVT
   • NotificationTypeSVT
```

---

## Table Count Summary

| Category | Tables | V1 Used |
|----------|--------|---------|
| SVT (Lookups) | 9 | Yes |
| Core | 4 | Yes |
| Application | 2 | Yes |
| Batch & Disbursement | 3 | Yes |
| Reporting | 2 | Yes |
| File Storage | 3 | Yes |
| Status History | 3 | Yes |
| Supporting | 2 | Yes |
| V2 Extensions | 3 | Created, Not Used |
| **Total** | **31** | |

---

## Domain Model Naming (SA-111)

Per R4 standard SA-111, C# domain model classes should be suffixed with "Model":

| Table | Domain Model Class |
|-------|-------------------|
| GrantProgram | GrantProgramModel |
| GrantCycle | GrantCycleModel |
| Organization | OrganizationModel |
| Account | AccountModel |
| Application | ApplicationModel |
| Candidate | CandidateModel |
| Batch | BatchModel |
| BatchCandidate | BatchCandidateModel |
| Disbursement | DisbursementModel |
| CandidateReport | CandidateReportModel |
| File | FileModel |
| CandidateStatusSVT | CandidateStatusSvtModel |
| (etc.) | |

---

*Document Version: 1.0*
*Last Updated: December 2025*
*R4 Framework Compliance: Full*
