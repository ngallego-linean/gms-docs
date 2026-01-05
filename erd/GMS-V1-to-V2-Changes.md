# GMS Schema V1 → V2 Change Log

**R4 Compliance Fixes**
**December 30, 2025**

**V1 Base:** `GMS-V1-Schema.md` (Noah's schema)

---

## Summary

| Category | Changes | Standard |
|----------|---------|----------|
| SVT Archive Field Rename | 9 tables, 1 column each | DB-202 |
| FK Naming Corrections | 3 columns | DB-105 |
| Missing Audit Fields | 1 table, 4 columns | DB-114 |
| **Total Required** | **13 column changes across 12 tables** | |

---

## Required R4 Fixes

### Change 1: SVT ArchivedByAccountId → ArchivedByID

#### R4 Justification

**Standard:** DB-202 (Database Static Value Tables)

**Exact Text from R4:**
> "All SVT tables must have the fields listed in the table below..."

| Field | Data Type | Default Value | Required? |
|-------|-----------|---------------|-----------|
| ... | ... | ... | ... |
| **ArchivedByID** | int | - | No |
| ... | ... | ... | ... |

**Important Distinction:**
- DB-113 specifies `ArchivedByAccountId` for **regular tables**
- DB-202 specifies `ArchivedByID` for **SVT tables**

These are different standards. Noah's V1 schema uses `ArchivedByAccountId` in SVT tables, which violates DB-202.

#### Tables Affected

| # | Table | V1 Column | V2 Column |
|---|-------|-----------|-----------|
| 1 | OrganizationTypeSVT | ArchivedByAccountId | ArchivedByID |
| 2 | CandidateStatusSVT | ArchivedByAccountId | ArchivedByID |
| 3 | BatchStatusSVT | ArchivedByAccountId | ArchivedByID |
| 4 | DisbursementStatusSVT | ArchivedByAccountId | ArchivedByID |
| 5 | FileTypeSVT | ArchivedByAccountId | ArchivedByID |
| 6 | ReportTypeSVT | ArchivedByAccountId | ArchivedByID |
| 7 | UserRoleSVT | ArchivedByAccountId | ArchivedByID |
| 8 | NotificationTypeSVT | ArchivedByAccountId | ArchivedByID |
| 9 | BatchTimelineEventTypeSVT | ArchivedByAccountId | ArchivedByID |

#### Diff (Example: OrganizationTypeSVT)

```diff
CREATE TABLE OrganizationTypeSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,
    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,
    IsArchived              BIT NOT NULL DEFAULT 0,
    ArchivedOn              DATETIME NULL,
-   ArchivedByAccountId     INT NULL,
+   ArchivedByID            INT NULL,
    ArchivedComments        VARCHAR(MAX) NULL,
    CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByAccountId      INT NOT NULL DEFAULT -1,
    LastUpdatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
    LastUpdatedByAccountId  INT NOT NULL DEFAULT -1,
    CanBeDeleted            BIT NOT NULL DEFAULT 0
);
```

---

### Change 2: Foreign Key Naming Corrections

#### R4 Justification

**Standard:** DB-105 (Foreign Key Naming)

**Exact Text from R4:**
> "Foreign keys are named 'Id' prefixed with the name of the foreign key's parent table."
>
> Example:
> - PK: Id (in table named Company)
> - FK Parent Table Name: Contact
> - FK: ContactId

**Standard:** DB-107 (FK Shortening Exception)

**Exact Text from R4:**
> "When a foreign key's parent table name includes the primary table's name, the foreign key name can be shortened to avoid redundancy, assuming it does not create ambiguity about the foreign key's parent table."
>
> Example:
> - PK: Id (in table named Application)
> - FK Parent Table Name: ApplicationStatusSVT
> - FK1: StatusId (instead of ApplicationStatusId)

#### Analysis of V1 FK Names

| Column | Table | Parent SVT | Parent contains table name? | DB-107 Applies? | Violation? |
|--------|-------|------------|----------------------------|-----------------|------------|
| TypeId | Organization | OrganizationTypeSVT | Yes ("Organization") | Yes | **NO** |
| RoleId | Account | UserRoleSVT | No ("Account" ∉ "UserRoleSVT") | No | **YES** |
| TypeId | File | FileTypeSVT | Yes ("File") | Yes | **NO** |
| TypeId | Notification | NotificationTypeSVT | Yes ("Notification") | Yes | **NO** |
| ReportingOrgTypeId | ReportTypeSVT | OrganizationTypeSVT | No ("ReportTypeSVT" ∉ "OrganizationTypeSVT") | No | **YES** |
| ReportingOrgTypeId | CandidateReport | OrganizationTypeSVT | No ("CandidateReport" ∉ "OrganizationTypeSVT") | No | **YES** |

**Result:** Only 3 columns are actual DB-105 violations.

#### Columns Requiring Change

| # | Table | V1 Column | V2 Column | Parent Table |
|---|-------|-----------|-----------|--------------|
| 1 | Account | RoleId | UserRoleSVTId | UserRoleSVT |
| 2 | ReportTypeSVT | ReportingOrgTypeId | ReportingOrgTypeSVTId | OrganizationTypeSVT |
| 3 | CandidateReport | ReportingOrgTypeId | ReportingOrgTypeSVTId | OrganizationTypeSVT |

#### Diffs

**Account:**
```diff
CREATE TABLE Account (
    Id                          INT IDENTITY(1,1) PRIMARY KEY,
    OrganizationId              INT NULL REFERENCES Organization(Id),
-   RoleId                      INT NOT NULL REFERENCES UserRoleSVT(Id),
+   UserRoleSVTId               INT NOT NULL REFERENCES UserRoleSVT(Id),
    EmailAddress                VARCHAR(320) NOT NULL UNIQUE,
    ...
);

-CREATE INDEX IX_Account_RoleId ON Account(RoleId);
+CREATE INDEX IX_Account_UserRoleSVTId ON Account(UserRoleSVTId);
```

**ReportTypeSVT:**
```diff
CREATE TABLE ReportTypeSVT (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,
    Name                    VARCHAR(50) NOT NULL,
    Description             VARCHAR(500) NOT NULL,
    DisplayOrder            INT NOT NULL,
    Code                    VARCHAR(30) NOT NULL UNIQUE,
-   ReportingOrgTypeId      INT NULL REFERENCES OrganizationTypeSVT(Id),
+   ReportingOrgTypeSVTId   INT NULL REFERENCES OrganizationTypeSVT(Id),
    FormSchemaJson          NVARCHAR(MAX) NULL,
    ...
);
```

**CandidateReport:**
```diff
CREATE TABLE CandidateReport (
    Id                          INT IDENTITY(1,1) PRIMARY KEY,
    CandidateId                 INT NOT NULL REFERENCES Candidate(Id),
    ReportingPeriodId           INT NOT NULL REFERENCES ReportingPeriod(Id),
    ReportingOrgId              INT NOT NULL REFERENCES Organization(Id),
    ReportTypeId                INT NOT NULL REFERENCES ReportTypeSVT(Id),
-   ReportingOrgTypeId          INT NOT NULL REFERENCES OrganizationTypeSVT(Id),
+   ReportingOrgTypeSVTId       INT NOT NULL REFERENCES OrganizationTypeSVT(Id),
    ...
);

-CREATE INDEX IX_CandidateReport_ReportingOrgTypeId ON CandidateReport(ReportingOrgTypeId);
+CREATE INDEX IX_CandidateReport_ReportingOrgTypeSVTId ON CandidateReport(ReportingOrgTypeSVTId);
```

---

### Change 3: AuditLog Missing Audit Fields

#### R4 Justification

**Standard:** DB-114 (Audit Fields)

**Exact Text from R4:**
> "All tables must include five (5) metadata fields to keep track of who created and updated the record and when it was created and updated. The fields must be required and must be the last fields in the table in the order shown below."

| Field | Data Type | Default Value | Required? |
|-------|-----------|---------------|-----------|
| CreatedOn | datetime | GETDATE() | Yes |
| CreatedByAccountId | int | -1 | Yes |
| LastUpdatedOn | datetime | GETDATE() | Yes |
| LastUpdatedByAccountId | int | -1 | Yes |
| CanBeDeleted | bit | 0 | Yes |

**Violation:** V1 `AuditLog` table only has `CreatedOn`. Missing 4 of 5 required fields.

#### Diff

```diff
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
-   CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE()
+   CreatedOn                   DATETIME NOT NULL DEFAULT GETDATE(),
+   CreatedByAccountId          INT NOT NULL DEFAULT -1,
+   LastUpdatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
+   LastUpdatedByAccountId      INT NOT NULL DEFAULT -1,
+   CanBeDeleted                BIT NOT NULL DEFAULT 0
);
```

---

## What Was NOT Changed (Compliant Per DB-107)

The following columns were reviewed but found compliant with DB-107's shortening rule:

| Column | Table | Parent SVT | Why Compliant |
|--------|-------|------------|---------------|
| TypeId | Organization | OrganizationTypeSVT | "OrganizationTypeSVT" contains "Organization" |
| TypeId | File | FileTypeSVT | "FileTypeSVT" contains "File" |
| TypeId | Notification | NotificationTypeSVT | "NotificationTypeSVT" contains "Notification" |

Per DB-107: "When a foreign key's parent table name includes the primary table's name, the foreign key name can be shortened."

---

## Optional Improvements (Not R4 Violations)

The following changes were considered but are NOT required by R4:

| Change | Why Not Required |
|--------|------------------|
| Status → StatusCode | R4 has no rule about VARCHAR status field naming. DB-105/DB-107 only apply to FK columns. |
| Add archive fields to DisbursementFile | DB-113 says "any table **that requires** the archived fields" - conditional, not mandatory. |
| Add archive fields to CandidateReportFile | Same as above - design decision, not R4 requirement. |
| Add archive fields to Notification | Same as above - design decision, not R4 requirement. |
| Organization.TypeId → OrganizationTypeSVTId | Compliant per DB-107 (optional clarity improvement). |
| File.TypeId → FileTypeSVTId | Compliant per DB-107 (optional clarity improvement). |
| Notification.TypeId → NotificationTypeSVTId | Compliant per DB-107 (optional clarity improvement). |

---

## R4 Compliance Summary

| Standard | Requirement | V1 Status | V2 Status |
|----------|-------------|-----------|-----------|
| DB-202 | SVT tables use ArchivedByID | **Violation** (used ArchivedByAccountId) | **Fixed** |
| DB-105 | FK = parent table name + Id | **Violation** (3 columns) | **Fixed** |
| DB-114 | All tables have 5 audit fields | **Violation** (AuditLog missing 4) | **Fixed** |
| DB-107 | FK shortening when parent contains table name | Compliant | Compliant |
| DB-113 | Archive fields on archivable tables | Compliant (optional) | Compliant |

---

*Document Version: 2.0 (Corrected)*
*Generated: December 30, 2025*
