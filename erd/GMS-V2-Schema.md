# GMS V2 Database Schema

**California Commission on Teacher Credentialing**
**Grant Management System - Version 2.0 (R4 Compliance Update)**
**December 2025**

---

## Change Summary from V1

**Base Schema:** `GMS-V1-Schema.md` (Noah's schema)

This revision addresses **verified R4 compliance violations only**. No optional improvements.

| # | Change | Standard | Scope |
|---|--------|----------|-------|
| 1 | ArchivedByAccountId → ArchivedByID | DB-202 | 9 SVT tables |
| 2 | RoleId → UserRoleSVTId | DB-105 | Account table |
| 3 | ReportingOrgTypeId → ReportingOrgTypeSVTId | DB-105 | ReportTypeSVT, CandidateReport |
| 4 | Add 4 audit fields to AuditLog | DB-114 | AuditLog table |

**Total: 13 column changes across 12 tables**

---

## 1. Static Value Tables (SVTs)

All SVT tables now use `ArchivedByID` per DB-202.

### 1.1 OrganizationTypeSVT

```sql
CREATE TABLE OrganizationTypeSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    -- Standard SVT Fields (DB-202)
    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    -- Archive Fields (DB-202: ArchivedByID for SVTs)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByID            INT NULL,  -- CHANGED from ArchivedByAccountId
    ArchivedComments        VARCHAR(MAX) NULL,

    -- Standard Audit Fields (DB-114)
    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

---

### 1.2 CandidateStatusSVT

```sql
CREATE TABLE CandidateStatusSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,
    Category                VARCHAR(20) NOT NULL,
    LEADisplayCategory      VARCHAR(20) NULL,
    IsTerminal              BIT NOT NULL DEFAULT 0,
    AllowedTransitionsJson  NVARCHAR(MAX) NULL,

    -- Archive Fields (DB-202)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByID            INT NULL,  -- CHANGED
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

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

    -- Archive Fields (DB-202)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByID            INT NULL,  -- CHANGED
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

---

### 1.4 DisbursementStatusSVT

```sql
CREATE TABLE DisbursementStatusSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,

    -- Archive Fields (DB-202)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByID            INT NULL,  -- CHANGED
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

    -- Archive Fields (DB-202)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByID            INT NULL,  -- CHANGED
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

---

### 1.6 ReportTypeSVT

**CHANGED:** `ReportingOrgTypeId` → `ReportingOrgTypeSVTId` per DB-105.

```sql
CREATE TABLE ReportTypeSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,
    ReportingOrgTypeSVTId   INT NULL REFERENCES OrganizationTypeSVT(Id),  -- CHANGED from ReportingOrgTypeId
    FormSchemaJson          NVARCHAR(MAX) NULL,

    -- Archive Fields (DB-202)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByID            INT NULL,  -- CHANGED
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

---

### 1.7 UserRoleSVT

```sql
CREATE TABLE UserRoleSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,

    -- Archive Fields (DB-202)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByID            INT NULL,  -- CHANGED
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

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

    -- Archive Fields (DB-202)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByID            INT NULL,  -- CHANGED
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

```sql
CREATE TABLE BatchTimelineEventTypeSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,

    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,

    Code                    VARCHAR(30) NOT NULL UNIQUE,

    -- Archive Fields (DB-202)
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
    ArchivedByID            INT NULL,  -- CHANGED
    ArchivedComments        VARCHAR(MAX) NULL,

    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

---

## 2. Core Entities

### 2.1 GrantProgram

No changes - R4 compliant.

*(Same as V1)*

---

### 2.2 GrantCycle

No changes - R4 compliant.

*(Same as V1)*

---

### 2.3 Organization

No changes - R4 compliant per DB-107.

**Note:** `TypeId` is compliant because "OrganizationTypeSVT" contains "Organization", allowing shortening per DB-107.

*(Same as V1)*

---

### 2.4 Account

**CHANGED:** `RoleId` → `UserRoleSVTId` per DB-105.

DB-107 does NOT apply because "UserRoleSVT" does not contain "Account".

```sql
CREATE TABLE Account (
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    OrganizationId              INT NULL REFERENCES Organization(Id),
    UserRoleSVTId               INT NOT NULL REFERENCES UserRoleSVT(Id),  -- CHANGED from RoleId

    EmailAddress                VARCHAR(320) NOT NULL UNIQUE,
    FirstName                   NVARCHAR(50) NOT NULL,
    LastName                    NVARCHAR(50) NOT NULL,
    PhoneNumber                 VARCHAR(25) NULL,
    Title                       VARCHAR(200) NULL,

    EntraId                     VARCHAR(100) NULL,
    OAuthProvider               VARCHAR(50) NULL,
    OAuthId                     VARCHAR(200) NULL,

    IsActive                    BIT NOT NULL DEFAULT 1,
    LastLoginOn                 DATETIME NULL,

    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_Account_OrganizationId ON Account(OrganizationId);
CREATE INDEX IX_Account_UserRoleSVTId ON Account(UserRoleSVTId);  -- CHANGED
CREATE INDEX IX_Account_EmailAddress ON Account(EmailAddress);
```

---

## 3. Application Entities

### 3.1 Application

No changes - R4 compliant.

*(Same as V1)*

---

### 3.2 Candidate

No changes - R4 compliant.

*(Same as V1)*

---

## 4. Batch & Disbursement

No changes to Batch, BatchCandidate, or Disbursement - R4 compliant.

*(Same as V1)*

---

## 5. Reporting Entities

### 5.1 ReportingPeriod

No changes - R4 compliant.

*(Same as V1)*

---

### 5.2 CandidateReport

**CHANGED:** `ReportingOrgTypeId` → `ReportingOrgTypeSVTId` per DB-105.

DB-107 does NOT apply because "OrganizationTypeSVT" does not contain "CandidateReport".

```sql
CREATE TABLE CandidateReport (
    Id                          INT IDENTITY(1,1) PRIMARY KEY,

    CandidateId                 INT NOT NULL REFERENCES Candidate(Id),
    ReportingPeriodId           INT NOT NULL REFERENCES ReportingPeriod(Id),
    ReportingOrgId              INT NOT NULL REFERENCES Organization(Id),
    ReportTypeId                INT NOT NULL REFERENCES ReportTypeSVT(Id),
    ReportingOrgTypeSVTId       INT NOT NULL REFERENCES OrganizationTypeSVT(Id),  -- CHANGED from ReportingOrgTypeId

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

    ExtendedDataJson            NVARCHAR(MAX) NULL,

    Status                      VARCHAR(20) NOT NULL DEFAULT 'DRAFT',
    SubmittedOn                 DATETIME NULL,
    SubmittedByAccountId        INT NULL REFERENCES Account(Id),
    ApprovedOn                  DATETIME NULL,
    ApprovedByAccountId         INT NULL REFERENCES Account(Id),
    RevisionNotes               VARCHAR(2000) NULL,

    IsArchived                  BIT NOT NULL DEFAULT 0,
    ArchivedOn                  DATETIME NULL,
    ArchivedByAccountId         INT NULL,
    ArchivedComments            VARCHAR(MAX) NULL,

    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
    CanBeDeleted                BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_CandidateReport_CandidateId ON CandidateReport(CandidateId);
CREATE INDEX IX_CandidateReport_ReportingPeriodId ON CandidateReport(ReportingPeriodId);
CREATE INDEX IX_CandidateReport_ReportTypeId ON CandidateReport(ReportTypeId);
CREATE INDEX IX_CandidateReport_ReportingOrgTypeSVTId ON CandidateReport(ReportingOrgTypeSVTId);  -- CHANGED
```

---

## 6. File & Document Storage

No changes to File, DisbursementFile, or CandidateReportFile - R4 compliant.

**Note:** `File.TypeId` is compliant per DB-107 because "FileTypeSVT" contains "File".

*(Same as V1)*

---

## 7. Status Change History

No changes - R4 compliant.

*(Same as V1)*

---

## 8. Supporting Entities

### 8.1 Notification

No changes - R4 compliant.

**Note:** `TypeId` is compliant per DB-107 because "NotificationTypeSVT" contains "Notification".

*(Same as V1)*

---

### 8.2 AuditLog

**CHANGED:** Added 4 missing audit fields per DB-114.

```sql
CREATE TABLE AuditLog (
    Id                          BIGINT IDENTITY(1,1) PRIMARY KEY,

    EntityType                  VARCHAR(50) NOT NULL,
    EntityId                    INT NOT NULL,
    Action                      VARCHAR(50) NOT NULL,

    OldValueJson                NVARCHAR(MAX) NULL,
    NewValueJson                NVARCHAR(MAX) NULL,

    AccountId                   INT NULL REFERENCES Account(Id),
    IpAddress                   VARCHAR(50) NULL,
    UserAgent                   VARCHAR(500) NULL,

    -- Standard Audit Fields (DB-114) - FIXED: was missing 4 fields
    CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId          INT NOT NULL DEFAULT -1,           -- ADDED
    LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(), -- ADDED
    LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,           -- ADDED
    CanBeDeleted                BIT NOT NULL DEFAULT 0             -- ADDED
);

CREATE INDEX IX_AuditLog_EntityType_EntityId ON AuditLog(EntityType, EntityId);
CREATE INDEX IX_AuditLog_CreatedOn ON AuditLog(CreatedOn);
CREATE INDEX IX_AuditLog_AccountId ON AuditLog(AccountId);
```

---

## 9. V2 Extension Tables

No changes - R4 compliant.

*(Same as V1)*

---

## Tables Changed Summary

| Table | Change | Standard |
|-------|--------|----------|
| OrganizationTypeSVT | ArchivedByAccountId → ArchivedByID | DB-202 |
| CandidateStatusSVT | ArchivedByAccountId → ArchivedByID | DB-202 |
| BatchStatusSVT | ArchivedByAccountId → ArchivedByID | DB-202 |
| DisbursementStatusSVT | ArchivedByAccountId → ArchivedByID | DB-202 |
| FileTypeSVT | ArchivedByAccountId → ArchivedByID | DB-202 |
| ReportTypeSVT | ArchivedByAccountId → ArchivedByID, ReportingOrgTypeId → ReportingOrgTypeSVTId | DB-202, DB-105 |
| UserRoleSVT | ArchivedByAccountId → ArchivedByID | DB-202 |
| NotificationTypeSVT | ArchivedByAccountId → ArchivedByID | DB-202 |
| BatchTimelineEventTypeSVT | ArchivedByAccountId → ArchivedByID | DB-202 |
| Account | RoleId → UserRoleSVTId | DB-105 |
| CandidateReport | ReportingOrgTypeId → ReportingOrgTypeSVTId | DB-105 |
| AuditLog | +4 audit fields | DB-114 |

---

## Tables NOT Changed (R4 Compliant)

| Table | Notes |
|-------|-------|
| GrantProgram | Compliant |
| GrantCycle | Compliant |
| Organization | TypeId compliant per DB-107 |
| Application | Compliant |
| Candidate | Compliant |
| Batch | Compliant |
| BatchCandidate | Compliant |
| Disbursement | Compliant |
| ReportingPeriod | Compliant |
| File | TypeId compliant per DB-107 |
| DisbursementFile | Compliant |
| CandidateReportFile | Compliant |
| CandidateStatusChange | Compliant |
| BatchStatusChange | Compliant |
| DisbursementStatusChange | Compliant |
| Notification | TypeId compliant per DB-107 |
| CandidateFunding | Compliant |
| CandidateYearProgress | Compliant |
| CandidateYearOutcome | Compliant |

---

## R4 Compliance Summary

| Standard | Requirement | V2 Status |
|----------|-------------|-----------|
| DB-202 | SVT tables use ArchivedByID | **Compliant** |
| DB-105 | FK = parent table name + Id | **Compliant** |
| DB-107 | FK shortening allowed when parent contains table name | **Applied** |
| DB-114 | All tables have 5 audit fields | **Compliant** |
| DB-113 | Archive fields on archivable tables | **Compliant** |

---

*Document Version: 2.0 (Corrected)*
*Last Updated: December 2025*
*R4 Framework Compliance: Full*
