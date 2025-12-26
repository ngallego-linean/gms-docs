# GMS Data Architecture Document

**California Commission on Teacher Credentialing**
**Grant Management System - Technical Architecture**
**December 2025**

---

## Document Purpose

This document is a **technical companion to the Business Requirements Document (BRD)**. It demonstrates how the database schema implements BRD requirements and confirms compliance with CTC's R4 Framework standards.

**Audience:**
- **DTI Technical Lead**: R4 compliance verification (Section 2)
- **DBA**: Entity model review, data dictionary (Sections 3, 5)
- **Project Manager**: Requirements traceability, migration plan (Sections 1, 4)
- **System Architect**: Integration patterns, deployment (Sections 3, 4)

---

## Table of Contents

1. [Requirements Traceability](#1-requirements-traceability)
2. [R4 Framework Compliance](#2-r4-framework-compliance)
3. [Entity Model & Relationships](#3-entity-model--relationships)
4. [Migration & Deployment](#4-migration--deployment)
5. [Data Dictionary](#5-data-dictionary)

---

## 1. Requirements Traceability

This section maps BRD requirements to their schema implementations.

### 1.1 Portal Requirements

| BRD Requirement | Schema Implementation |
|-----------------|----------------------|
| Grant program listings with status | `GrantProgram.IsActive`, `GrantCycle.Status` |
| Application volume metrics and participant counts | `Candidate` table with status aggregations |
| Review IHE-LEA application pairs | `Application` table with `IHEOrgId`, `LEAOrgId` |
| Approve or reject student teachers | `Candidate.StatusId` → `CandidateStatusSVT` |
| Track application status transitions | `CandidateStatusChange` history table |
| GAA generation from approved applications | `Disbursement` table with GAA fields |
| DocuSign integration for multi-party signatures | `Disbursement.GAAEnvelopeId`, `GAAStatus`, `GAASignedOn` |
| Payment initiation and completion tracking | `Disbursement` PO/Invoice/Warrant fields |
| Track fund state transitions | `Batch.TotalAwardAmount` + `DisbursementStatusSVT` |
| View IHE and LEA report submission status | `CandidateReport.Status`, `ReportingPeriod.DueOn` |
| Track overdue reports | `ReportingPeriod.DueOn` vs `CandidateReport.SubmittedOn` |

### 1.2 Data Management Requirements

| BRD Requirement | Schema Implementation |
|-----------------|----------------------|
| Temporal tables for automatic change history | `CandidateStatusChange`, `BatchStatusChange`, `DisbursementStatusChange` |
| Ledger tables for immutable financial records | `Disbursement` table (Azure SQL Ledger configuration) |
| Hybrid schema with JSON extensions | `ExtendedDataJson` columns on `Candidate`, `CandidateReport`, `GrantProgram` |
| Minimum 7-year retention | Archive fields on all tables (`IsArchived`, `ArchivedOn`) - no hard deletes |
| All entities track Created/Updated audit | 5 audit fields per DB-114 on every table |

### 1.3 Security Requirements

| BRD Requirement | Schema Implementation |
|-----------------|----------------------|
| Role-based access control (RBAC) | `UserRoleSVT` with codes: IHE_USER, LEA_USER, CTC_GRANTS, CTC_FISCAL, CTC_ADMIN |
| Portal-level isolation | `Account.OrganizationId` + `Account.RoleId` for authorization |
| EntraID for staff | `Account.EntraId` for SSO mapping |
| OAuth for grantees | `Account.OAuthProvider`, `Account.OAuthId` |
| Magic links with expiration | `Notification` table with tokenized URL support |

### 1.4 Integration Requirements

| BRD Requirement | Schema Implementation |
|-----------------|----------------------|
| ECS Database - SEID lookup | `Candidate.SEID`, `Organization.EcsCdsCode` |
| ECS - Organization CDS code verification | `Organization.EcsCdsCode` with index |
| Geographic reporting | `Organization.County`, `Organization.Region` |
| DocuSign - GAA envelope tracking | `Disbursement.GAAEnvelopeId`, `GAAStatus`, `GAASentOn`, `GAASignedOn` |
| Fiscal System - CSV export (Phase 1) | All financial data in `Disbursement`, `Batch.TotalAwardAmount` |

---

## 2. R4 Framework Compliance

### 2.1 Database Standards Compliance

| R4 Standard | Requirement | Compliance |
|-------------|-------------|------------|
| DB-101 | Table names: PascalCase, singular, no prefixes | ✓ |
| DB-102 | Junction tables: concatenate parent names | ✓ |
| DB-104 | Primary keys named "Id" | ✓ |
| DB-105 | Foreign keys: `{ParentTable}Id` | ✓ |
| DB-108 | Bit fields: "Is/Has/Can" prefix | ✓ |
| DB-110 | DateTime fields: "On" suffix | ✓ |
| DB-113 | Archive fields (4 columns) on archivable tables | ✓ |
| DB-114 | Audit fields (5 columns) on all tables | ✓ |
| DB-201 | SVT tables suffixed with "SVT" | ✓ |
| DB-202 | SVT standard structure | ✓ |

### 2.2 Standard Field Patterns

**Audit Fields (DB-114)** - Present on all 31 tables:

| Field | Type | Default | Required |
|-------|------|---------|----------|
| CreatedOn | DateTime | GETDATE() | Yes |
| CreatedByAccountId | Int | -1 | Yes |
| LastUpdatedOn | DateTime | GETDATE() | Yes |
| LastUpdatedByAccountId | Int | -1 | Yes |
| CanBeDeleted | Bit | 0 | Yes |

**Archive Fields (DB-113)** - Present on archivable tables:

| Field | Type | Default | Required |
|-------|------|---------|----------|
| IsArchived | Bit | 0 | Yes |
| ArchivedOn | DateTime | NULL | No |
| ArchivedByAccountId | Int | NULL | No |
| ArchivedComments | VarChar(MAX) | NULL | No |

### 2.3 SVT (Static Value Table) Inventory

| SVT Table | Purpose | Example Values |
|-----------|---------|----------------|
| OrganizationTypeSVT | Organization categories | IHE, LEA, COE, CTC |
| CandidateStatusSVT | Candidate workflow states | DRAFT, APPROVED, REJECTED |
| BatchStatusSVT | Batch workflow states | PENDING_REVIEW, GAA_SENT, COMPLETE |
| DisbursementStatusSVT | Payment states | PENDING, PROCESSED, CONFIRMED |
| FileTypeSVT | Document categories | GAA_DOCUMENT, INVOICE, PO_DOCUMENT |
| ReportTypeSVT | Report categories | IHE_COMPLETION, LEA_PAYMENT |
| UserRoleSVT | Portal access roles | IHE_USER, LEA_USER, CTC_GRANTS |
| NotificationTypeSVT | Email template types | APPLICATION_SUBMITTED, GAA_READY |
| BatchTimelineEventTypeSVT | Workflow timeline events | BATCH_CREATED, CTC_APPROVED |

---

## 3. Entity Model & Relationships

### 3.1 Entity Relationship Diagram

```
                              ┌──────────────┐
                              │ GrantProgram │ ←── Configuration (V1: STSP only)
                              └──────┬───────┘
                                     │ 1:M
                                     ▼
                              ┌──────────────┐
                              │  GrantCycle  │ ←── Funding period (FY 2024-25)
                              └──────┬───────┘
                                     │
              ┌──────────────────────┼──────────────────────┐
              │ 1:M                  │ 1:M                  │ 1:M
              ▼                      ▼                      ▼
        ┌───────────┐          ┌───────────┐         ┌───────────────┐
        │Application│          │   Batch   │         │ReportingPeriod│
        │ (IHE+LEA) │          │ (Monthly) │         │  (Due Dates)  │
        └─────┬─────┘          └─────┬─────┘         └───────┬───────┘
              │ 1:M                  │ 1:M                   │ 1:M
              ▼                      ▼                       ▼
        ┌───────────┐          ┌───────────┐         ┌───────────────┐
        │ Candidate │◄─────────│BatchCand. │         │CandidateReport│
        │  (Person) │  M:1     │  (Join)   │         │ (IHE/LEA)     │
        └───────────┘          └───────────┘         └───────────────┘
                                     │
                                     │ M:1
                                     ▼
                               ┌───────────┐
                               │Disbursem. │ ←── GAA, PO, Invoice, Warrant
                               └─────┬─────┘
                                     │ 1:M
                                     ▼
                               ┌───────────┐
                               │   File    │ ←── Document storage
                               └───────────┘

 Organization ──┬── 1:M ──► Account (portal users)
                ├── M:1 ──► Application.IHEOrgId
                ├── M:1 ──► Application.LEAOrgId
                └── M:1 ──► Batch.SubmittingOrgId
```

### 3.2 Table Summary

| Category | Tables | Count |
|----------|--------|-------|
| SVT (Lookups) | OrganizationTypeSVT, CandidateStatusSVT, BatchStatusSVT, DisbursementStatusSVT, FileTypeSVT, ReportTypeSVT, UserRoleSVT, NotificationTypeSVT, BatchTimelineEventTypeSVT | 9 |
| Core | GrantProgram, GrantCycle, Organization, Account | 4 |
| Application | Application, Candidate | 2 |
| Batch & Disbursement | Batch, BatchCandidate, Disbursement | 3 |
| Reporting | ReportingPeriod, CandidateReport | 2 |
| File Storage | File, DisbursementFile, CandidateReportFile | 3 |
| Status History | CandidateStatusChange, BatchStatusChange, DisbursementStatusChange | 3 |
| Supporting | Notification, AuditLog | 2 |
| V2 Extensions | CandidateFunding, CandidateYearProgress, CandidateYearOutcome | 3 |
| **Total** | | **31** |

### 3.3 Candidate Status Workflow

```
    ┌─────────┐      ┌─────────────┐      ┌─────────────┐
    │  DRAFT  │─────►│IHE_SUBMITTED│─────►│LEA_REVIEWING│
    └─────────┘      └─────────────┘      └──────┬──────┘
                                                 │
                     ┌───────────────────────────┼───────────────────────────┐
                     ▼                           ▼                           ▼
             ┌───────────────┐            ┌─────────────┐            ┌───────────┐
             │RETURNED_TO_IHE│            │ LEA_APPROVED│            │ REJECTED  │
             └───────┬───────┘            └──────┬──────┘            └───────────┘
                     │                           │                      (Terminal)
                     │                           ▼
                     │                    ┌─────────────┐
                     └───────────────────►│CTC_SUBMITTED│
                                          └──────┬──────┘
                                                 ▼
                                          ┌─────────────┐
                                          │CTC_REVIEWING│
                                          └──────┬──────┘
                                                 │
                     ┌───────────────────────────┼───────────────────────────┐
                     ▼                           ▼                           ▼
             ┌───────────┐               ┌───────────┐               ┌─────────────┐
             │ REJECTED  │               │ APPROVED  │               │REPORTING_DUE│
             └───────────┘               └───────────┘               └──────┬──────┘
               (Terminal)                                                   │
                                                                            ▼
                                                                    ┌───────────────┐
                                                                    │REPORTS_COMPLETE│
                                                                    └───────┬───────┘
                                                                            ▼
                                                                      ┌──────────┐
                                                                      │  CLOSED  │
                                                                      └──────────┘
                                                                        (Terminal)
```

---

## 4. Migration & Deployment

### 4.1 Current State

STSP is currently managed via:
- Excel spreadsheets for candidate tracking
- Email for IHE-LEA-CTC communication
- Paper GAAs with wet signatures
- Manual fund tracking in separate ledgers

### 4.2 Data Migration Plan

**Phase 1: Reference Data (Pre Go-Live)**

| Data | Source | Target Table |
|------|--------|--------------|
| IHE Organizations | ECS Database | Organization |
| LEA Organizations | ECS Database | Organization |
| CTC Staff | EntraID | Account |
| SVT Seed Data | Configuration | All SVT tables |
| STSP Grant Program | Configuration | GrantProgram |
| FY 2024-25 Cycle | Configuration | GrantCycle |

**Phase 2: Historical Data** - Recommended to start fresh for FY 2024-25.

### 4.3 Deployment Sequence

1. Create Database
2. Create Tables
3. Seed SVT Tables
4. Seed GrantProgram (STSP)
5. Create GrantCycle (FY 2024-25)
6. ECS Organization Sync
7. Configure Azure SQL (Temporal + Ledger)
8. Deploy Application
9. Smoke Test
10. Enable User Access

### 4.4 Backup & Recovery

| Requirement | Implementation |
|-------------|----------------|
| Automated daily backups | Azure SQL automated backups |
| Point-in-time restore (7-day window) | Azure SQL PITR |
| Geo-redundant storage | Azure SQL geo-redundant backup storage |
| Recovery Point Objective (RPO) | 1 hour |
| Recovery Time Objective (RTO) | 4 hours |

---

## 5. Data Dictionary

### 5.1 GrantProgram

Configuration for a grant type. V1 contains single row for STSP.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| Code | VarChar(20) | Yes | Unique code (e.g., "STSP") |
| Name | VarChar(200) | Yes | Display name |
| Description | VarChar(2000) | No | Program description |
| WorkflowType | VarChar(20) | Yes | Workflow pattern (default: IHE_LEA_CTC) |
| HasBatching | Bit | Yes | Whether candidates are batched |
| AwardUnit | VarChar(20) | Yes | Award basis (PER_CANDIDATE, LUMP_SUM) |
| DisbursementType | VarChar(20) | Yes | Payment type (ONE_TIME, PHASED) |
| HoursThreshold | Int | No | Minimum hours for eligibility (e.g., 500) |
| IsActive | Bit | Yes | Whether program is active |

### 5.2 GrantCycle

A funding period for a grant program.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| GrantProgramId | Int | Yes | FK to GrantProgram |
| Name | VarChar(200) | Yes | Display name |
| FiscalYear | VarChar(10) | Yes | Fiscal year (e.g., "2024-25") |
| AppropriatedAmount | Decimal(18,2) | Yes | Total appropriation |
| DefaultAwardAmount | Decimal(18,2) | Yes | Award per candidate (e.g., 10000.00) |
| ApplicationOpenOn | DateTime | No | Application period start |
| ApplicationCloseOn | DateTime | No | Application period end |
| Status | VarChar(20) | Yes | Cycle status (DRAFT, OPEN, CLOSED) |

### 5.3 Organization

An IHE, LEA, or other organization. Synced from ECS.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| TypeId | Int | Yes | FK to OrganizationTypeSVT |
| EcsCdsCode | VarChar(14) | No | CDS code from ECS |
| EcsOrganizationId | Int | No | Organization ID from ECS |
| Name | VarChar(200) | Yes | Organization name |
| StreetAddress | NVarChar(80) | No | Street address |
| City | NVarChar(40) | No | City |
| County | NVarChar(50) | No | California county |
| Region | NVarChar(50) | No | CTC region (Northern, Southern, Bay Area) |
| StateProvince | NVarChar(40) | No | State |
| PostalCode | NVarChar(40) | No | ZIP code |
| PrimaryEmailAddress | VarChar(320) | No | Contact email |
| PrimaryPhoneNumber | VarChar(25) | No | Contact phone |
| IsActive | Bit | Yes | Whether organization is active |
| LastEcsSyncOn | DateTime | No | Last ECS sync timestamp |

### 5.4 Account

Portal users (IHE/LEA staff, CTC staff).

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| OrganizationId | Int | No | FK to Organization (NULL for CTC staff) |
| RoleId | Int | Yes | FK to UserRoleSVT |
| EmailAddress | VarChar(320) | Yes | Unique login email |
| FirstName | NVarChar(50) | Yes | First name |
| LastName | NVarChar(50) | Yes | Last name |
| PhoneNumber | VarChar(25) | No | Phone number |
| Title | VarChar(200) | No | Job title |
| EntraId | VarChar(100) | No | EntraID for CTC SSO |
| OAuthProvider | VarChar(50) | No | OAuth provider (google, microsoft) |
| OAuthId | VarChar(200) | No | OAuth identifier |
| IsActive | Bit | Yes | Whether account is active |
| LastLoginOn | DateTime | No | Last login timestamp |

### 5.5 Application

An IHE-LEA partnership for a grant cycle.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| GrantCycleId | Int | Yes | FK to GrantCycle |
| IHEOrgId | Int | No | FK to Organization (IHE) |
| LEAOrgId | Int | No | FK to Organization (LEA) |
| InitiatorContactId | Int | No | FK to Account (IHE contact) |
| CompleterContactId | Int | No | FK to Account (LEA contact) |
| GAASignerName | NVarChar(200) | No | Name of GAA signer |
| GAASignerTitle | VarChar(200) | No | Title of GAA signer |
| GAASignerEmail | VarChar(320) | No | Email for DocuSign |
| Status | VarChar(20) | Yes | Application status |

### 5.6 Candidate

An individual applying for the grant (student teacher).

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| ApplicationId | Int | Yes | FK to Application |
| StatusId | Int | Yes | FK to CandidateStatusSVT |
| FirstName | NVarChar(50) | Yes | First name |
| LastName | NVarChar(50) | Yes | Last name |
| SEID | VarChar(10) | No | State Educator ID |
| DateOfBirth | DateTime | No | Date of birth |
| Last4SSN | VarChar(4) | No | Last 4 digits of SSN |
| EmailAddress | VarChar(320) | No | Email address |
| Race | VarChar(100) | No | Race |
| Ethnicity | VarChar(100) | No | Ethnicity |
| Gender | VarChar(50) | No | Gender |
| CredentialArea | VarChar(100) | No | Credential subject area |
| SchoolCdsCode | VarChar(14) | No | Placement school CDS code |
| SchoolName | VarChar(200) | No | Placement school name |
| RequestedAmount | Decimal(18,2) | No | Requested award amount |
| AwardedAmount | Decimal(18,2) | No | Approved award amount |
| ApprovedOn | DateTime | No | Approval timestamp |
| ApprovedByAccountId | Int | No | FK to Account (approver) |
| RejectedOn | DateTime | No | Rejection timestamp |
| RejectionReason | VarChar(1000) | No | Reason for rejection |
| ExtendedDataJson | NVarChar(MAX) | No | Grant-specific fields (JSON) |

### 5.7 Batch

LEA's monthly submission to CTC. One GAA per batch.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| GrantCycleId | Int | Yes | FK to GrantCycle |
| StatusId | Int | Yes | FK to BatchStatusSVT |
| SubmittingOrgId | Int | Yes | FK to Organization (LEA) |
| BatchNumber | VarChar(20) | No | Batch identifier |
| SubmissionPeriod | VarChar(10) | Yes | Year-Month (e.g., "2025-01") |
| CandidateCount | Int | Yes | Number of candidates |
| TotalAwardAmount | Decimal(18,2) | Yes | Total batch amount |
| SubmittedOn | DateTime | No | Submission timestamp |
| SubmittedByAccountId | Int | No | FK to Account (submitter) |
| ApprovedOn | DateTime | No | Approval timestamp |
| ApprovedByAccountId | Int | No | FK to Account (approver) |

### 5.8 Disbursement

Payment processing record for a batch.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| BatchId | Int | Yes | FK to Batch |
| StatusId | Int | Yes | FK to DisbursementStatusSVT |
| Amount | Decimal(18,2) | Yes | Disbursement amount |
| GAAEnvelopeId | VarChar(100) | No | DocuSign envelope ID |
| GAAStatus | VarChar(30) | No | GAA status |
| GAASentOn | DateTime | No | GAA sent timestamp |
| GAASignedOn | DateTime | No | GAA signed timestamp |
| PONumber | VarChar(50) | No | Purchase order number |
| POAmount | Decimal(18,2) | No | PO amount |
| POIssuedOn | DateTime | No | PO issued date |
| InvoiceNumber | VarChar(50) | No | Invoice number |
| InvoiceAmount | Decimal(18,2) | No | Invoice amount |
| InvoiceSentOn | DateTime | No | Invoice sent date |
| WarrantNumber | VarChar(50) | No | Warrant number |
| WarrantAmount | Decimal(18,2) | No | Warrant amount |
| WarrantConfirmedOn | DateTime | No | Payment confirmed date |

### 5.9 ReportingPeriod

When reports are due for a grant cycle.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| GrantCycleId | Int | Yes | FK to GrantCycle |
| ReportTypeId | Int | Yes | FK to ReportTypeSVT |
| PeriodName | VarChar(100) | Yes | Period name |
| StartOn | DateTime | No | Period start date |
| DueOn | DateTime | Yes | Report due date |
| IsActive | Bit | Yes | Whether period is active |

### 5.10 CandidateReport

Per-candidate report submitted by IHE or LEA.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| CandidateId | Int | Yes | FK to Candidate |
| ReportingPeriodId | Int | Yes | FK to ReportingPeriod |
| ReportingOrgId | Int | Yes | FK to Organization |
| ReportTypeId | Int | Yes | FK to ReportTypeSVT |
| CompletionStatus | VarChar(30) | No | Completion status |
| CompletionOn | DateTime | No | Completion date |
| GrantProgramHours | Int | No | Hours in grant program |
| CredentialProgramHours | Int | No | Hours in credential program |
| HasCredentialEarned | Bit | No | Whether credential earned |
| CredentialEarnedOn | DateTime | No | Credential earned date |
| EmploymentStatus | VarChar(50) | No | Employment outcome |
| IsHiredByPartnerOrg | Bit | No | Hired by partner LEA |
| PaymentCategory | VarChar(50) | No | Payment category |
| PaymentAmount | Decimal(18,2) | No | Payment to candidate |
| PaymentOn | DateTime | No | Payment date |
| Status | VarChar(20) | Yes | Report status (DRAFT, SUBMITTED) |
| SubmittedOn | DateTime | No | Submission timestamp |
| ExtendedDataJson | NVarChar(MAX) | No | Grant-specific fields (JSON) |

### 5.11 File

Document storage.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| TypeId | Int | Yes | FK to FileTypeSVT |
| StorageKey | UniqueIdentifier | Yes | Azure Blob storage key |
| FileName | VarChar(255) | Yes | Original filename |
| FileSizeInKilobytes | Int | Yes | File size |
| Title | VarChar(300) | No | Document title |
| DocumentDate | DateTime | No | Document date |

### 5.12 Notification

Email/system notifications.

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| TypeId | Int | Yes | FK to NotificationTypeSVT |
| RecipientAccountId | Int | No | FK to Account |
| RecipientEmailAddress | VarChar(320) | Yes | Recipient email |
| Subject | NVarChar(500) | Yes | Email subject |
| BodyHtml | NVarChar(MAX) | Yes | Email body |
| Status | VarChar(20) | Yes | Status (PENDING, SENT, FAILED) |
| SentOn | DateTime | No | Sent timestamp |
| ErrorMessage | VarChar(1000) | No | Error if failed |

### 5.13 Status Change Tables

Three tables track status history for audit compliance:

**CandidateStatusChange**

| Column | Type | Required | Description |
|--------|------|----------|-------------|
| Id | Int | Yes | Primary key |
| CandidateId | Int | Yes | FK to Candidate |
| FromStatusId | Int | No | FK to CandidateStatusSVT |
| ToStatusId | Int | Yes | FK to CandidateStatusSVT |
| ChangedOn | DateTime | Yes | Change timestamp |
| ChangedByAccountId | Int | Yes | FK to Account |
| Comments | VarChar(2000) | No | Change notes |

**BatchStatusChange** and **DisbursementStatusChange** follow the same pattern.

### 5.14 Junction Tables

| Table | Purpose | Foreign Keys |
|-------|---------|--------------|
| BatchCandidate | Links Batch to Candidate | BatchId, CandidateId |
| DisbursementFile | Links Disbursement to File | DisbursementId, FileId |
| CandidateReportFile | Links CandidateReport to File | CandidateReportId, FileId |

### 5.15 V2 Extension Tables (Created but not used in V1)

| Table | Purpose |
|-------|---------|
| CandidateFunding | Multiple funding sources per candidate |
| CandidateYearProgress | Annual progress tracking |
| CandidateYearOutcome | Post-program employment tracking |

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | December 2025 | GMS Team | Initial release |
