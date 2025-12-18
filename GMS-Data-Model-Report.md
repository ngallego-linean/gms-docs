# GMS Data Model & Architecture Report

**California Commission on Teacher Credentialing**
**Grant Management System (GMS)**
**Version 1.0 - December 2025**

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [System Overview](#2-system-overview)
3. [Business Requirements Summary](#3-business-requirements-summary)
4. [V1 Data Model](#4-v1-data-model)
5. [Configurability Strategy](#5-configurability-strategy)
6. [Grant Program Analysis](#6-grant-program-analysis)
7. [Entity Specifications](#7-entity-specifications)
8. [Status & Workflow Design](#8-status--workflow-design)
9. [Integration Points](#9-integration-points)
10. [Recommendations](#10-recommendations)
11. [Appendices](#appendices)

---

## 1. Executive Summary

### 1.1 What is GMS?

The Grant Management System (GMS) is a web-based platform to manage California's teacher preparation grants. The initial deployment (V1) focuses on the **Student Teacher Stipend Program (STSP)**, which provides $10,000 stipends to student teachers completing 540+ hours of clinical practice.

### 1.2 The Problem

CTC's Professional Services Division currently manages grants manually via:
- Spreadsheets for tracking candidates and payments
- Email for communication and document exchange
- Paper forms for signatures and approvals

This results in:
- Duplicate data entry and data quality issues
- Staff time consumed by transactional work
- Difficulty meeting legislative reporting requirements
- No real-time visibility into fund status
- Slow turnaround times for grantees

### 1.3 The Solution

GMS automates the entire grant lifecycle:
- **Application intake** from IHEs and LEAs
- **Review and approval** by CTC staff
- **Disbursement processing** including DocuSign GAAs
- **Payment tracking** through warrant confirmation
- **Post-award reporting** for outcomes tracking
- **Fund management** with real-time budget visibility

### 1.4 Key Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| V1 Scope | STSP only | Ship focused product by April 2026 |
| Architecture | Design for configurability | Support future grants without rewrite |
| Org Roles | Abstract (Initiator/Completer) | Different grants have different workflows |
| Disbursement | 1:M from Batch | Support future multi-phase disbursements |
| Data Extension | JSON columns | Grant-specific fields without schema changes |

---

## 2. System Overview

### 2.1 Grant Lifecycle (STSP)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           STSP GRANT LIFECYCLE                               │
└─────────────────────────────────────────────────────────────────────────────┘

    IHE identifies          LEA completes           CTC reviews
    student teacher         fiscal info &           and approves
    candidates              confirms hours          candidates
         │                       │                       │
         ▼                       ▼                       ▼
    ┌─────────┐             ┌─────────┐             ┌─────────┐
    │   IHE   │────────────▶│   LEA   │────────────▶│   CTC   │
    │ Submit  │  Notify     │Complete │   Submit    │ Review  │
    └─────────┘             └─────────┘   Batch     └─────────┘
                                                         │
                                                         │ Approve
                                                         ▼
    ┌─────────┐             ┌─────────┐             ┌─────────┐
    │ Reports │◀────────────│ Payment │◀────────────│  Fiscal │
    │   Due   │  Warrant    │Processed│   GAA/PO    │  Team   │
    └─────────┘  Confirmed  └─────────┘   Invoice   └─────────┘
         │
         ▼
    ┌─────────┐
    │ Closed  │
    └─────────┘
```

### 2.2 Three Portals

| Portal | Users | Primary Functions |
|--------|-------|-------------------|
| **Public Portal** | Anyone | Grant listings, eligibility info, RFAs |
| **Grantee Portal** | IHE/LEA Staff | Submit candidates, complete applications, reports |
| **CTC Staff Portal** | Grants/Fiscal Teams | Review queue, approvals, GAA generation, payments |

### 2.3 Technology Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core MVC (.NET 9) |
| ORM | Entity Framework Core |
| Database | Azure SQL (temporal + ledger tables) |
| UI Components | Telerik UI, Kendo, Bootstrap |
| Authentication | EntraID (staff), OAuth (grantees) |
| Signatures | DocuSign API |
| External Data | ECS Database (read-only) |
| Hosting | Azure App Services |

---

## 3. Business Requirements Summary

### 3.1 Stakeholders

**IHE (Institutions of Higher Education)**
- Universities with credential programs
- Submit student teacher candidates
- Report on completion and credential earned

**LEA (Local Education Agencies)**
- School districts where student teachers are placed
- Confirm student eligibility (540+ hours)
- Complete fiscal information
- Receive the $10,000 payment
- Report on payment distribution to student

**CTC Grants Team**
- Review applications
- Approve/reject candidates
- Monitor reporting compliance

**CTC Fiscal Team**
- Generate Grant Award Agreements (GAAs)
- Process purchase orders and invoices
- Track payments through disbursement

### 3.2 V1 Features (April 2026 Go-Live)

#### Application Management
- IHE submits student info (name, SEID, DOB, credential area)
- One application per IHE-LEA pair per grant cycle
- Students added incrementally throughout cycle
- Draft saving, status tracking
- LEA completes fiscal details, confirms practicum hours
- Manual entry and bulk CSV upload

#### Review & Approval
- Staff review queue with pending candidates
- Approve/reject individual candidates
- Validation against available funds
- Status transitions tracked with timestamps

#### Disbursement Workflow
- LEA submissions batched monthly
- GAA generation from approved candidates
- DocuSign 3-party routing (LEA → CTC Staff 1 → CTC Staff 2)
- PO upload from Fi$CAL
- Invoice generation
- Warrant confirmation (manual logging)

#### Fund Tracking
- Real-time dashboard showing:
  - Appropriated amount
  - Reserved (pending approval)
  - Encumbered (approved, awaiting payment)
  - Disbursed (paid)
  - Remaining balance
- Color-coded alerts for fund depletion

#### Post-Award Reporting
- **IHE Completion Report**: Program completion, credential earned, employment
- **LEA Payment Report**: Payment category, schedule, hours completed
- Draft saving, due date tracking, overdue reminders

#### Notifications
- Email triggers for key events
- Template-based with merge fields

### 3.3 V2/Aspirational Features

| Feature | Description |
|---------|-------------|
| Grant Configurator | Self-service tool to create new grant programs |
| Fi$CAL Integration | Real-time payment tracking |
| Magic Link Data Collection | Tokenized links to students for demographics |
| ECS Outcomes Dashboard | Credential attainment, retention rates |
| AI Reporting | Natural language query interface |
| Bulk CSV Upload | High-volume IHE student uploads |

### 3.4 Key Numbers

| Metric | Value |
|--------|-------|
| Award per student | $10,000 |
| Minimum practicum hours | 540 |
| Example appropriation | $25,000,000 |
| Organizations (IHE + LEA) | 500+ |
| Applications per cycle | 5,000+ |
| Peak concurrent users | 200+ |
| Data retention | 7 years |

---

## 4. V1 Data Model

### 4.1 Entity Relationship Overview

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         ENTITY RELATIONSHIP DIAGRAM                          │
└─────────────────────────────────────────────────────────────────────────────┘

                              ┌──────────────┐
                              │ GrantProgram │
                              │  (Config)    │
                              └──────┬───────┘
                                     │ 1:M
                                     ▼
┌──────────────┐              ┌──────────────┐              ┌──────────────┐
│ Organization │              │  GrantCycle  │              │    User      │
│  (IHE/LEA)   │              │              │              │              │
└──────┬───────┘              └──────┬───────┘              └──────┬───────┘
       │                             │                             │
       │ M:1                         │ 1:M                         │ M:1
       │        ┌────────────────────┼────────────────────┐        │
       │        │                    │                    │        │
       │        ▼                    ▼                    ▼        │
       │  ┌───────────┐        ┌───────────┐        ┌───────────┐  │
       └─▶│Application│        │   Batch   │        │  Report   │◀─┘
          │ (IHE-LEA) │        │  (LEA     │        │ Period    │
          └─────┬─────┘        │  Monthly) │        └───────────┘
                │              └─────┬─────┘
                │ 1:M                │ 1:M
                ▼                    ▼
          ┌───────────┐        ┌───────────┐
          │ Candidate │◀──────▶│  Batch    │
          │           │  M:1   │ Candidate │
          └─────┬─────┘        └───────────┘
                │                    │
                │                    │ M:1
                │                    ▼
                │              ┌───────────┐
                │              │Disbursement│
                │              │(GAA/PO/INV)│
                │              └───────────┘
                │ 1:M
                ▼
          ┌───────────┐
          │ Candidate │
          │  Report   │
          └───────────┘
```

### 4.2 Cardinality Summary

| Relationship | Cardinality | Description |
|--------------|-------------|-------------|
| GrantProgram → GrantCycle | 1:M | Multiple cycles per program |
| GrantCycle → Application | 1:M | Multiple IHE-LEA pairs per cycle |
| GrantCycle → Batch | 1:M | Multiple monthly batches per cycle |
| GrantCycle → ReportingPeriod | 1:M | Multiple report due dates |
| Organization → User | 1:M | Multiple staff per organization |
| Organization (IHE) → Application | 1:M | IHE partners with many LEAs |
| Organization (LEA) → Application | 1:M | LEA partners with many IHEs |
| Application → Candidate | 1:M | Multiple students per IHE-LEA pair |
| Batch → Candidate | 1:M | Multiple candidates per batch |
| Candidate → Batch | M:1 | Each candidate in exactly one batch |
| Batch → Disbursement | 1:M | V1: one; V2: multiple phases |
| Candidate → CandidateReport | 1:M | Multiple reports per candidate |
| ReportingPeriod → CandidateReport | 1:M | Reports tied to periods |

### 4.3 Key Design Decisions

#### 4.3.1 Batch-Based Disbursement

**Decision:** GAA, PO, Invoice, and Warrant are per-batch, not per-candidate.

**Rationale:**
- Matches real-world CTC process (one GAA per LEA monthly submission)
- Reduces paperwork and DocuSign transactions
- Simplifies payment tracking

**Impact:**
- One Disbursement record per batch (V1)
- Can add multiple for phased disbursement (V2)

#### 4.3.2 Abstracted Organization Roles

**Decision:** Use `InitiatorOrgId` and `CompleterOrgId` instead of hardcoded `IHEOrgId`/`LEAOrgId`.

**Rationale:**
- Different grants have different org involvement
- Some grants are LEA-only, some IHE-only
- Workflow order can vary

**Impact:**
- Keep `IHEOrgId`/`LEAOrgId` for convenience and reporting
- Workflow logic uses abstracted roles
- V2 can configure which org plays which role

#### 4.3.3 JSON Extension Columns

**Decision:** Add `ExtendedDataJson` columns to Candidate, CandidateReport, etc.

**Rationale:**
- Different grants collect different data
- Avoids schema changes for each new grant
- Can validate against JSON Schema in V2

**Impact:**
- V1: Column exists but is empty/null
- V2: Populated per GrantProgram.FormSchemaJson

#### 4.3.4 Status as Data

**Decision:** Store workflow states in `WorkflowState` table, not hardcoded in application.

**Rationale:**
- Different grants have different status progressions
- Enables validation of allowed transitions
- UI can display dynamic status names

**Impact:**
- V1: Seed WorkflowState with STSP states
- V2: Add states for new grant programs
- Code validates transitions against data

---

## 5. Configurability Strategy

### 5.1 The Challenge

CTC manages multiple grant programs with varying:
- Organization involvement (IHE+LEA, LEA-only, IHE-only)
- Workflow order (who submits first, who completes)
- Evaluation type (eligibility vs. scored/competitive)
- Disbursement pattern (one-time vs. phased)
- Data requirements (different fields per grant)
- Tracking duration (single cycle vs. multi-year)

### 5.2 Design Principles

1. **Avoid hardcoding assumptions** in the schema
2. **Use nullable FKs** where relationships may not exist
3. **Add JSON columns** for grant-specific extensions
4. **Design status as data** (workflow states stored, not hardcoded)
5. **Make disbursement a collection** (1:M), not 1:1

### 5.3 Variability Matrix

| What Varies | STSP (V1) | Other Grants | Design Solution |
|-------------|-----------|--------------|-----------------|
| Org roles | IHE initiates, LEA completes | Reversed, or single-org | Abstract roles |
| Workflow order | IHE → LEA → CTC | LEA → IHE → CTC, etc. | WorkflowState table |
| Batch strategy | Monthly by LEA | Could be IHE, quarterly, cohort | BatchStrategy, BatchInitiatorOrgType |
| Evaluation | Valid/Invalid | Scored, ranked | Candidate.Score (nullable) |
| Disbursement | One-time per batch | Advance/Progress/Final | PaymentPhasesJson + Disbursement 1:M |
| Data collected | Fixed candidate fields | Different per grant | ExtendedDataJson |
| Funding model | Fixed $10K | Capped or budgeted | GrantCycle.MaxAwardAmount |
| Hours thresholds | 500/600 hours | Different per program | HoursThreshold, CredentialHoursThreshold |
| Tracking | Single cycle | Multi-year | CandidateYearProgress table |
| Report fields | Fixed IHE/LEA fields | Program-specific | ReportTypeSVT.FormSchemaJson |
| Contacts | POC, GAA Signer, Fiscal Agent | May vary by program | GAASignerEmail + ExtendedContactsJson |

### 5.4 GrantProgram Configuration

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        GrantProgram Configuration                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  WORKFLOW CONFIGURATION                                                     │
│  ─────────────────────                                                      │
│  WorkflowType:          IHE_LEA_CTC | LEA_IHE_CTC | LEA_CTC | IHE_CTC      │
│  RequiresInitiatorOrg:  true/false                                         │
│  RequiresCompleterOrg:  true/false                                         │
│  RequiresBatching:      true/false                                         │
│                                                                             │
│  EVALUATION CONFIGURATION                                                   │
│  ────────────────────────                                                   │
│  EvaluationType:        ELIGIBILITY | SCORED | LOTTERY                     │
│  RubricSchemaJson:      NULL (V1) | JSON Schema (V2)                       │
│                                                                             │
│  FUNDING CONFIGURATION                                                      │
│  ─────────────────────                                                      │
│  AwardUnit:             PER_CANDIDATE | PER_ORG | LUMP_SUM                 │
│  DisbursementType:      ONE_TIME | MULTI_PHASE | REIMBURSEMENT             │
│  HasMatchingFunds:      true/false                                         │
│                                                                             │
│  TRACKING CONFIGURATION                                                     │
│  ──────────────────────                                                     │
│  TrackingYears:         0 | 1 | 2 | 3 | 4                                  │
│  HasCohorts:            true/false                                         │
│  HasReplacements:       true/false                                         │
│                                                                             │
│  BATCHING CONFIGURATION                                                     │
│  ──────────────────────                                                     │
│  BatchStrategy:         NONE | MONTHLY | QUARTERLY | ON_DEMAND | COHORT    │
│  BatchInitiatorOrgType: IHE | LEA | CTC                                    │
│                                                                             │
│  PROGRAM THRESHOLDS (queryable)                                             │
│  ──────────────────────────────                                             │
│  HoursThreshold:        e.g., 500 for STSP grant hours requirement         │
│  CredentialHoursThreshold: e.g., 600 for STSP credential hours             │
│                                                                             │
│  COMPLEX CONFIGURATION (JSON)                                               │
│  ────────────────────────────                                               │
│  PaymentPhasesJson:        Multi-phase disbursement config                 │
│  ReportRequirementsJson:   Report due dates by org type                    │
│  ProgramConfigJson:        Additional program-specific values              │
│                                                                             │
│  FORM CONFIGURATION (V2)                                                    │
│  ───────────────────────                                                    │
│  CandidateFormSchemaJson:  NULL (V1) | JSON Schema (V2)                    │
│  ReportFormSchemaJson:     NULL (V1) | JSON Schema (V2)                    │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 5.5 V1 → V2 Migration Path

| Entity | V1 State | V2 Migration |
|--------|----------|--------------|
| GrantProgram | 1 row (STSP), defaults | Add rows for new programs |
| GrantProgram.PaymentPhasesJson | NULL | Populated for multi-phase programs |
| GrantProgram.ReportRequirementsJson | NULL | Populated for programs with custom report cadence |
| GrantCycle | Links to STSP | Links to any program |
| Application | InitiatorOrg=IHE, CompleterOrg=LEA | Set by WorkflowType |
| Application.GAASignerEmail | Populated for STSP | Used for DocuSign routing |
| Application.ExtendedContactsJson | Contains Fiscal Agent, Superintendent | Program-specific contacts |
| Candidate.ExtendedDataJson | Empty/NULL | Populated per FormSchema |
| Candidate.Score | NULL | Populated for scored grants |
| Batch.SubmittingOrgId | Always LEA | Per BatchInitiatorOrgType config |
| Disbursement | 1 row, Phase=FULL | Multiple rows per PaymentPhasesJson |
| WorkflowState | STSP states seeded | New states for new programs |
| CandidateReport.ReportingOrgTypeId | IHE=1 or LEA=2 | Enables efficient queries |
| ReportTypeSVT.FormSchemaJson | NULL | JSON Schema for report validation |

---

## 6. Grant Program Analysis

### 6.1 Programs Analyzed

| Grant | Code | Primary Org | Secondary Org | Award Model |
|-------|------|-------------|---------------|-------------|
| Student Teacher Stipend | STSP | IHE | LEA (completes) | Fixed $10K |
| Teacher Residency | TRP | IHE | LEA (partner) | Budgeted |
| School Counselor | SCRP | IHE | LEA (partner) | Budgeted |
| Classified | CSEP | LEA | None | Budgeted |
| Computer Science | CSGF | LEA | None | Capped $2.5K |
| DELPI | DELPI | LEA | None | Budgeted |
| Reading & Literacy | RLGF | LEA | None | Capped $2.5K |
| Integrated Teacher Prep | ITP | IHE | None | Budgeted |

### 6.2 Detailed Program Analysis

#### 6.2.1 STSP (Student Teacher Stipend Program) - V1 Target

**Flow:** IHE → LEA → CTC → Payment → Reports

**Characteristics:**
- IHE identifies and submits candidates
- LEA completes fiscal info, confirms 540+ hours
- CTC reviews and approves
- One-time $10,000 per candidate
- Two reports: IHE completion, LEA payment

**Data Collected:**
- Candidate: Name, SEID, DOB, Last4SSN, Credential Area
- Demographics: Race, Ethnicity, Gender
- Placement: School CDS Code, LEA
- Reports: Completion status, credential earned, employment, payment distribution

**Model Fit:** ✅ Perfect - this is what V1 is designed for

---

#### 6.2.2 Teacher Residency Program

**Flow:** IHE submits → LEA partner confirms → CTC reviews → Multi-year funding → 4-year teaching tracking

**Unique Characteristics:**

| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| Dual Funding | Grant funds + Matching funds | Need FundingSource field |
| Budget Categories | Tuition, stipends, exam fees, mentor PD | Need CandidateFunding entity |
| Multi-Year Tracking | Years 1-4 post-completion employment | Need CandidateYearOutcome |
| Retention Metrics | "Same LEA/School as residency?" | Fields on YearOutcome |
| Program Completion | TPA passed, Basic Skills, Subject Matter | Separate completion tracking |

**Additional Data:**
- Enrolled Date, Expected Completion Date
- Mentor start/end dates
- Credential requirements (TPA type, attempts)
- Per-category funding amounts (Grant + Match)
- Year 1-4: Employer, School CDS, Grade Level, Subject Area, Completed Year

**Model Fit:** ⚠️ Needs extensions
- CandidateFunding table
- CandidateYearOutcome table
- GrantProgram.HasMatchingFunds = true
- GrantProgram.TrackingYears = 4

---

#### 6.2.3 School Counselor Program

**Flow:** Same as Teacher Residency

**Unique Characteristics:**
- PPS-SC (Pupil Personnel Services - School Counselor) credential
- 4-year counseling placement tracking (not teaching)
- Otherwise identical structure to Teacher Residency

**Model Fit:** ⚠️ Same extensions as Teacher Residency
- CandidateYearOutcome.Position = "COUNSELOR"

---

#### 6.2.4 Classified School Employee Program

**Flow:** LEA only → CTC reviews → Multi-year funding → Credential tracking

**Unique Characteristics:**

| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| No IHE Workflow | LEA is sole grantee | InitiatorOrg=LEA, CompleterOrg=NULL |
| Replacement Participants | Exited participant can be replaced | Candidate.ReplacedCandidateId |
| Annual Progress | Degree + Credential progress yearly | CandidateYearProgress |
| Multi-Year Funding | Same participant funded across years | CandidateFunding with FiscalYear |
| Education Level | Degree at start vs. progress | ExtendedDataJson |

**Additional Data:**
- Employment classification
- Program start/expected completion dates
- Education level at enrollment
- Years of funding expected
- Criminal background check passed
- Annual: Degree progress, Credential progress, Employed next year

**Model Fit:** ⚠️ Needs extensions
- Candidate.ReplacedCandidateId
- CandidateYearProgress table
- GrantProgram.WorkflowType = "LEA_CTC"
- GrantProgram.HasReplacements = true

---

#### 6.2.5 Computer Science Supplementary Authorization Grant

**Flow:** LEA only → CTC → Per-participant capped funding → Authorization tracking

**Unique Characteristics:**

| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| Existing Teachers | Already credentialed, adding supplementary auth | Different participant type |
| Capped Awards | Up to $2,500 per participant | GrantCycle.MaxAwardAmount |
| Expense Categories | Tuition, books, release time, filing fees | CandidateFunding |
| Authorization Tracking | Progress toward supplementary auth | CandidateYearProgress |
| School Characteristics | Rural, high-need designation | ExtendedDataJson |

**Additional Data:**
- Current credential type
- Teaching tenure (years)
- Rural school indicator
- High-need school indicator
- Authorization progress (Made progress? Completed?)
- Expense breakdown by category

**Model Fit:** ✅ Mostly works
- GrantCycle.MaxAwardAmount = 2500
- CandidateFunding for expense tracking (optional)
- ExtendedDataJson for school characteristics

---

#### 6.2.6 DELPI (Diverse Education Leadership Pipeline Initiative)

**Flow:** LEA only → CTC → Multi-cohort tracking → Admin outcome tracking

**Unique Characteristics:**

| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| Multi-Cohort | 24-25, 25-26, 26-27 cohorts simultaneous | Candidate.CohortYear |
| Two Populations | Candidates + Current Admins | Candidate.ParticipantType |
| Admin Fields | Position, experience, credential at start | ExtendedDataJson |
| Narrative Reports | Qualitative summary narrative | Report.NarrativeText |

**Additional Data:**
- Cohort year (which cohort they belong to)
- Participant type (Candidate vs Current Admin)
- Position at program start
- Years of full-time experience
- Credential at program start
- Education level at start

**Model Fit:** ⚠️ Needs extensions
- Candidate.CohortYear
- Candidate.ParticipantType
- GrantProgram.HasCohorts = true

---

#### 6.2.7 Reading & Literacy Grant

**Flow:** Same as Computer Science

**Unique Characteristics:**
- Different supplementary authorization type (RLAA/RLLS)
- Same $2,500 cap structure
- Same expense categories
- Eligible school site requirement

**Model Fit:** ✅ Same as Computer Science

---

#### 6.2.8 Integrated Teacher Preparation Program

**Flow:** IHE only → CTC → Multi-year funding → Degree + Credential tracking

**Unique Characteristics:**

| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| IHE-Only | No LEA in workflow | InitiatorOrg=IHE, CompleterOrg=NULL |
| Dual Progress | BA degree + Teaching credential | CandidateYearProgress |
| Budget Categories | Faculty costs, admin, CCC coordination | CandidateFunding |
| Shortage Areas | Teacher shortage area tracking | ExtendedDataJson |

**Additional Data:**
- Expected completion date
- Teacher shortage area
- Credential pursuing
- BA major pursuing
- Academic status at start
- Annual: Degree progress/completed, Credential progress/completed, Early exit

**Model Fit:** ✅ Mostly works
- GrantProgram.WorkflowType = "IHE_CTC"
- CandidateYearProgress for dual tracking

---

### 6.3 Compatibility Summary

| Grant | Core Model | Extensions Needed | V2 Complexity |
|-------|------------|-------------------|---------------|
| **STSP** | ✅ Full | None | Low (V1) |
| **Teacher Residency** | ⚠️ Partial | CandidateFunding, CandidateYearOutcome | High |
| **School Counselor** | ⚠️ Partial | Same as Teacher Residency | High |
| **Classified** | ⚠️ Partial | CandidateYearProgress, Replacements | Medium |
| **Computer Science** | ✅ Mostly | Capped awards, optional funding detail | Low |
| **Reading & Literacy** | ✅ Mostly | Same as Computer Science | Low |
| **DELPI** | ⚠️ Partial | Cohorts, ParticipantType | Medium |
| **Integrated** | ✅ Mostly | CandidateYearProgress | Low |

### 6.4 V2 Implementation Priority

Based on complexity and model fit:

1. **Computer Science / Reading & Literacy** - Easiest
   - Just needs capped awards + optional funding categories
   - Minimal new entities

2. **Integrated Teacher Prep** - Low-Medium
   - Needs IHE-only workflow (config change)
   - CandidateYearProgress for dual tracking

3. **Classified** - Medium
   - Needs progress tracking + replacement participant support
   - LEA-only workflow

4. **DELPI** - Medium
   - Needs cohort tracking + participant types
   - LEA-only workflow

5. **Teacher Residency / School Counselor** - Highest
   - Full funding tracking (Grant + Match)
   - 4-year outcome tracking
   - Most complex data model

---

## 7. Entity Specifications

### 7.1 Core Entities

#### 7.1.1 GrantProgram

Represents a grant type (STSP, Teacher Residency, etc.). V1 has single row for STSP.

```
GrantProgram
├── Id                          INT PK
├── Code                        VARCHAR(20) UNIQUE     "STSP"
├── Name                        VARCHAR(200)           "Student Teacher Stipend Program"
├── Description                 TEXT
│
├── -- Workflow Configuration --
├── WorkflowType                VARCHAR(20)            "IHE_LEA_CTC"
├── RequiresInitiatorOrg        BIT                    1
├── RequiresCompleterOrg        BIT                    1
├── RequiresBatching            BIT                    1
│
├── -- Evaluation Configuration --
├── EvaluationType              VARCHAR(20)            "ELIGIBILITY"
│
├── -- Funding Configuration --
├── AwardUnit                   VARCHAR(20)            "PER_CANDIDATE"
├── DisbursementType            VARCHAR(20)            "ONE_TIME"
├── HasMatchingFunds            BIT                    0
│
├── -- Tracking Configuration --
├── TrackingYears               INT                    0
├── HasCohorts                  BIT                    0
├── HasReplacements             BIT                    0
│
├── -- Batching Configuration --
├── BatchStrategy               VARCHAR(30)            "MONTHLY"
├── BatchInitiatorOrgType       VARCHAR(10)            "LEA"
│
├── -- Program Thresholds (queryable) --
├── HoursThreshold              INT                    500 (STSP)
├── CredentialHoursThreshold    INT                    600 (STSP)
│
├── -- Complex Configuration (JSON) --
├── PaymentPhasesJson           NVARCHAR(MAX)          NULL (V1)
├── ReportRequirementsJson      NVARCHAR(MAX)          NULL (V1)
├── ProgramConfigJson           NVARCHAR(MAX)          NULL (V1)
│
├── -- Form Configuration (V2) --
├── CandidateFormSchemaJson     NVARCHAR(MAX)          NULL
├── ReportFormSchemaJson        NVARCHAR(MAX)          NULL
│
├── IsActive                    BIT
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

#### 7.1.2 GrantCycle

A specific funding period for a grant program.

```
GrantCycle
├── Id                          INT PK
├── GrantProgramId              INT FK → GrantProgram
├── Name                        VARCHAR(200)           "STSP FY 2025-26"
├── FiscalYear                  VARCHAR(10)            "2025-26"
│
├── -- Funding --
├── ApproprietedAmount          DECIMAL(18,2)          25000000.00
├── DefaultAwardAmount          DECIMAL(18,2)          10000.00
├── MinAwardAmount              DECIMAL(18,2)          NULL
├── MaxAwardAmount              DECIMAL(18,2)          NULL
│
├── -- Eligibility (V2: RulesDsl) --
├── EligibilityRulesJson        NVARCHAR(MAX)          '{"minPracticumHours": 540}'
│
├── -- Dates --
├── ApplicationOpenDate         DATE
├── ApplicationCloseDate        DATE
├── CycleStartDate              DATE
├── CycleEndDate                DATE
│
├── Status                      VARCHAR(20)            "OPEN"
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

#### 7.1.3 Organization

An IHE or LEA organization. Synced from ECS.

```
Organization
├── Id                          INT PK
├── EcsCdsCode                  VARCHAR(14)            CDS code (nullable for IHEs)
├── Name                        VARCHAR(200)
├── OrgType                     VARCHAR(20)            "IHE" | "LEA" | "COE"
│
├── -- Contact/Address --
├── Address                     VARCHAR(500)
├── City                        VARCHAR(100)
├── State                       VARCHAR(2)             "CA"
├── Zip                         VARCHAR(10)
├── PrimaryEmail                VARCHAR(200)
├── PrimaryPhone                VARCHAR(20)
├── EmailDomain                 VARCHAR(100)           For signup flow
│
├── IsActive                    BIT
├── LastEcsSyncAt               DATETIME2
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

#### 7.1.4 User

Portal users (IHE/LEA staff, CTC staff).

```
User
├── Id                          INT PK
├── OrganizationId              INT FK → Organization  (nullable for CTC staff)
├── Email                       VARCHAR(200) UNIQUE
├── FirstName                   VARCHAR(100)
├── LastName                    VARCHAR(100)
├── Phone                       VARCHAR(20)
├── Title                       VARCHAR(200)
├── Role                        VARCHAR(20)            "IHE_USER" | "LEA_USER" |
│                                                      "CTC_GRANTS" | "CTC_FISCAL"
├── EntraId                     VARCHAR(100)           For CTC staff SSO
├── IsActive                    BIT
├── LastLoginAt                 DATETIME2
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

### 7.2 Application Entities

#### 7.2.1 Application

An IHE-LEA pair for a grant cycle.

```
Application
├── Id                          INT PK
├── GrantCycleId                INT FK → GrantCycle
│
├── -- Abstracted Org Roles --
├── InitiatorOrgId              INT FK → Organization  (nullable)
├── CompleterOrgId              INT FK → Organization  (nullable)
│
├── -- Convenience References --
├── IHEOrgId                    INT FK → Organization  (nullable)
├── LEAOrgId                    INT FK → Organization  (nullable)
│
├── -- Contacts (system users) --
├── InitiatorContactId          INT FK → User
├── CompleterContactId          INT FK → User          (nullable)
│
├── -- GAA Signer (for DocuSign - may not be system user) --
├── GAASignerName               NVARCHAR(200)          Name of authorized signer
├── GAASignerTitle              VARCHAR(200)           Title (e.g., "Superintendent")
├── GAASignerEmail              VARCHAR(320)           Email for DocuSign routing
│
├── -- Extended Contacts (JSON) --
├── ExtendedContactsJson        NVARCHAR(MAX)          Fiscal Agent, Superintendent, etc.
│
├── Status                      VARCHAR(20)            "DRAFT" | "ACTIVE" | "CLOSED"
│
├── CreatedAt                   DATETIME2
├── CreatedById                 INT FK → User
└── LastModifiedAt              DATETIME2
```

#### 7.2.2 Candidate

An individual applying for the grant (student teacher, resident, participant).

```
Candidate
├── Id                          INT PK
├── ApplicationId               INT FK → Application
│
├── -- Core Identity --
├── FirstName                   VARCHAR(100)
├── LastName                    VARCHAR(100)
├── SEID                        VARCHAR(10)            ECS verified (nullable until verified)
├── DateOfBirth                 DATE
├── Last4SSN                    VARCHAR(4)             For ECS matching only
├── Email                       VARCHAR(200)           For magic link / direct contact
│
├── -- Standard Demographics --
├── Race                        VARCHAR(100)
├── Ethnicity                   VARCHAR(100)
├── Gender                      VARCHAR(50)
│
├── -- STSP-Specific (V1) --
├── CredentialArea              VARCHAR(100)
├── SchoolCdsCode               VARCHAR(14)
├── SchoolName                  VARCHAR(200)
│
├── -- Scoring (V2) --
├── Score                       DECIMAL(10,2)          NULL for eligibility-only
├── ScoreDetailsJson            NVARCHAR(MAX)          NULL
├── Rank                        INT                    NULL for non-competitive
│
├── -- Multi-Grant Support --
├── CohortYear                  VARCHAR(10)            NULL, "2024-25" for DELPI
├── ParticipantType             VARCHAR(20)            "CANDIDATE" | "CURRENT_ADMIN"
├── ReplacedCandidateId         INT FK → Candidate     NULL, for replacement tracking
├── ReplacementReason           VARCHAR(500)
│
├── -- Financial --
├── RequestedAmount             DECIMAL(18,2)          For variable awards
├── AwardedAmount               DECIMAL(18,2)          V1: always 10000
│
├── -- Extension Point --
├── ExtendedDataJson            NVARCHAR(MAX)          Grant-specific fields
│
├── -- Status & Workflow --
├── Status                      VARCHAR(30)            From WorkflowState
├── StatusChangedAt             DATETIME2
├── StatusChangedById           INT FK → User
│
├── -- Key Dates --
├── SubmittedByInitiatorAt      DATETIME2
├── SubmittedByCompleterAt      DATETIME2              (nullable)
├── SubmittedToCTCAt            DATETIME2
├── ApprovedAt                  DATETIME2
├── RejectedAt                  DATETIME2
├── RejectionReason             VARCHAR(1000)
│
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

### 7.3 Batch & Disbursement Entities

#### 7.3.1 Batch

LEA's monthly submission to CTC. One GAA per batch.

```
Batch
├── Id                          INT PK
├── GrantCycleId                INT FK → GrantCycle
│
├── -- Ownership --
├── SubmittingOrgId             INT FK → Organization
├── LEAOrgId                    INT FK → Organization  (nullable, convenience)
│
├── SubmissionPeriod            VARCHAR(10)            "2025-11" (Year-Month)
├── BatchType                   VARCHAR(20)            "MONTHLY" | "QUARTERLY" | "AD_HOC"
│
├── -- Aggregates --
├── CandidateCount              INT
├── TotalAwardAmount            DECIMAL(18,2)
│
├── -- Status --
├── Status                      VARCHAR(30)
├── StatusChangedAt             DATETIME2
├── StatusChangedById           INT FK → User
│
├── SubmittedAt                 DATETIME2
├── SubmittedById               INT FK → User
├── ApprovedAt                  DATETIME2
├── ApprovedById                INT FK → User
│
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

#### 7.3.2 BatchCandidate

Join table linking Batch to Candidate.

```
BatchCandidate
├── Id                          INT PK
├── BatchId                     INT FK → Batch
├── CandidateId                 INT FK → Candidate     UNIQUE (one batch per candidate)
├── AddedAt                     DATETIME2
└── AddedById                   INT FK → User
```

#### 7.3.3 Disbursement

Payment processing record. V1: One per batch. V2: Multiple for phased.

```
Disbursement
├── Id                          INT PK
├── BatchId                     INT FK → Batch
│
├── -- Phase/Sequence --
├── Phase                       VARCHAR(20)            "FULL" | "ADVANCE" | "FINAL"
├── Sequence                    INT                    1, 2, 3...
│
├── -- Amounts --
├── Amount                      DECIMAL(18,2)
├── PercentOfTotal              DECIMAL(5,2)           NULL or 100 for V1
│
├── -- Status --
├── Status                      VARCHAR(30)            "PENDING" | "GAA_SENT" | etc.
│
├── -- GAA --
├── GAADocumentUrl              VARCHAR(500)
├── GAAEnvelopeId               VARCHAR(100)           DocuSign
├── GAAStatus                   VARCHAR(30)            DocuSign status
├── GAASentAt                   DATETIME2
├── GAASignedAt                 DATETIME2
├── GAACompletedAt              DATETIME2
│
├── -- PO --
├── PONumber                    VARCHAR(50)
├── POAmount                    DECIMAL(18,2)
├── PODate                      DATE
├── PODocumentUrl               VARCHAR(500)
├── POUploadedAt                DATETIME2
│
├── -- Invoice --
├── InvoiceNumber               VARCHAR(50)
├── InvoiceAmount               DECIMAL(18,2)
├── InvoiceDate                 DATE
├── InvoiceDocumentUrl          VARCHAR(500)
├── InvoiceSentAt               DATETIME2
│
├── -- Warrant --
├── WarrantNumber               VARCHAR(50)
├── WarrantAmount               DECIMAL(18,2)
├── WarrantDate                 DATE
├── WarrantConfirmedAt          DATETIME2
│
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

### 7.4 Reporting Entities

#### 7.4.1 ReportingPeriod

When reports are due.

```
ReportingPeriod
├── Id                          INT PK
├── GrantCycleId                INT FK → GrantCycle
├── PeriodName                  VARCHAR(100)           "Final Completion Report"
├── ReportType                  VARCHAR(30)            "IHE_COMPLETION" | "LEA_PAYMENT"
├── StartDate                   DATE
├── DueDate                     DATE
├── IsActive                    BIT
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

#### 7.4.2 CandidateReport

Per-candidate report (IHE completion or LEA payment).

```
CandidateReport
├── Id                          INT PK
├── CandidateId                 INT FK → Candidate
├── ReportingPeriodId           INT FK → ReportingPeriod
│
├── -- Who Is Reporting --
├── ReportingOrgId              INT FK → Organization
├── ReportingOrgTypeId          INT FK → OrgTypeSVT    IHE=1, LEA=2 (denorm for queries)
├── ReportType                  VARCHAR(30)            "IHE_COMPLETION" | "LEA_PAYMENT"
│
├── -- Standard Fields (STSP) --
├── CompletionStatus            VARCHAR(30)            "COMPLETED" | "DENIED" | "IN_PROGRESS"
├── CompletionDate              DATE
├── GrantProgramHours           INT
├── CredentialProgramHours      INT
├── CredentialEarned            BIT
├── CredentialEarnedDate        DATE
├── EmploymentStatus            VARCHAR(50)
├── HiredByPartnerOrg           BIT
├── PaymentCategory             VARCHAR(50)            "CLASSIFIED" | "CERTIFICATED" | "STIPEND"
├── PaymentAmount               DECIMAL(18,2)
├── PaymentDate                 DATE
│
├── -- Extension Point --
├── ExtendedDataJson            NVARCHAR(MAX)
│
├── -- Workflow --
├── Status                      VARCHAR(20)            "DRAFT" | "SUBMITTED" | "APPROVED"
├── SubmittedAt                 DATETIME2
├── SubmittedById               INT FK → User
├── ApprovedAt                  DATETIME2
├── ApprovedById                INT FK → User
├── RevisionNotes               VARCHAR(2000)
│
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

### 7.5 V2 Extension Entities

These tables should be created in V1 but not used until V2.

#### 7.5.1 CandidateFunding

For grants with multiple funding categories/sources.

```
CandidateFunding
├── Id                          INT PK
├── CandidateId                 INT FK → Candidate
├── FiscalYear                  VARCHAR(10)            "2025-26"
├── FundingSource               VARCHAR(20)            "GRANT" | "MATCH" | "OTHER"
├── Category                    VARCHAR(50)            From lookup
├── BudgetedAmount              DECIMAL(18,2)
├── ExpendedAmount              DECIMAL(18,2)
├── Notes                       VARCHAR(1000)
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

#### 7.5.2 CandidateYearProgress

For grants with annual progress tracking.

```
CandidateYearProgress
├── Id                          INT PK
├── CandidateId                 INT FK → Candidate
├── FiscalYear                  VARCHAR(10)            "2025-26"
├── YearInProgram               INT                    1, 2, 3, 4, 5
│
├── -- Standard Progress --
├── DegreeProgress              BIT
├── DegreeCompleted             BIT
├── CredentialProgress          BIT
├── CredentialCompleted         BIT
├── AuthorizationProgress       BIT                    For supplementary auth
├── AuthorizationCompleted      BIT
│
├── -- Employment/Continuation --
├── EmployedNextYear            BIT
├── EarlyExit                   BIT
├── EarlyExitDate               DATE
├── EarlyExitReason             VARCHAR(500)
│
├── -- Extension --
├── ExtendedDataJson            NVARCHAR(MAX)
│
├── Notes                       VARCHAR(2000)
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

#### 7.5.3 CandidateYearOutcome

For grants that track post-program employment.

```
CandidateYearOutcome
├── Id                          INT PK
├── CandidateId                 INT FK → Candidate
├── OutcomeYear                 INT                    1, 2, 3, 4 (post-completion)
├── FiscalYear                  VARCHAR(10)            "2025-26"
│
├── -- Employment --
├── IsEmployed                  BIT
├── EmployerOrgId               INT FK → Organization  (nullable)
├── EmployerName                VARCHAR(200)           If not in system
├── SchoolCdsCode               VARCHAR(14)
├── SchoolName                  VARCHAR(200)
├── Position                    VARCHAR(50)            "TEACHER" | "COUNSELOR" | "ADMIN"
├── GradeLevel                  VARCHAR(50)
├── SubjectArea                 VARCHAR(100)
│
├── -- Retention Metrics --
├── SameLEAAsPlacement          BIT
├── SameSchoolAsPlacement       BIT
├── CompletedYear               BIT
├── ReasonForLeaving            VARCHAR(500)
│
├── -- Extension --
├── ExtendedDataJson            NVARCHAR(MAX)
│
├── Notes                       VARCHAR(2000)
├── CreatedAt                   DATETIME2
└── LastModifiedAt              DATETIME2
```

### 7.6 Supporting Entities

#### 7.6.1 WorkflowState

Status definitions per grant program.

```
WorkflowState
├── Id                          INT PK
├── GrantProgramId              INT FK → GrantProgram
├── EntityType                  VARCHAR(20)            "CANDIDATE" | "BATCH" | "DISBURSEMENT"
├── Code                        VARCHAR(30)            "IHE_SUBMITTED"
├── DisplayName                 VARCHAR(100)           "Submitted to LEA"
├── Sequence                    INT                    1, 2, 3...
├── Category                    VARCHAR(20)            "SUBMISSION" | "REVIEW" | "DISBURSEMENT"
├── IsTerminal                  BIT
├── AllowedTransitionsJson      NVARCHAR(MAX)          '["LEA_REVIEWING", "RETURNED_TO_IHE"]'
├── TransitionConditionsJson    NVARCHAR(MAX)          NULL for V1
├── OnEnterActionsJson          NVARCHAR(MAX)          NULL for V1
└── CreatedAt                   DATETIME2
```

#### 7.6.2 Notification

Email/system notifications.

```
Notification
├── Id                          INT PK
├── Type                        VARCHAR(50)            "CANDIDATES_PENDING_LEA" | etc.
├── RecipientUserId             INT FK → User
├── RecipientEmail              VARCHAR(200)
├── Subject                     VARCHAR(500)
├── BodyHtml                    NVARCHAR(MAX)
├── RelatedBatchId              INT FK → Batch         (nullable)
├── RelatedCandidateId          INT FK → Candidate     (nullable)
├── Status                      VARCHAR(20)            "PENDING" | "SENT" | "FAILED"
├── SentAt                      DATETIME2
├── ErrorMessage                VARCHAR(1000)
└── CreatedAt                   DATETIME2
```

#### 7.6.3 AuditLog

Action history for compliance.

```
AuditLog
├── Id                          BIGINT PK
├── EntityType                  VARCHAR(50)            "Candidate" | "Batch" | etc.
├── EntityId                    INT
├── Action                      VARCHAR(50)            "CREATED" | "STATUS_CHANGED" | etc.
├── OldValueJson                NVARCHAR(MAX)
├── NewValueJson                NVARCHAR(MAX)
├── UserId                      INT FK → User
├── IpAddress                   VARCHAR(50)
└── CreatedAt                   DATETIME2
```

---

## 8. Status & Workflow Design

### 8.1 Candidate Status State Machine

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        CANDIDATE STATUS FLOW (STSP)                          │
└─────────────────────────────────────────────────────────────────────────────┘

    ┌─────────┐
    │  DRAFT  │ ─────────────────────────────────────────────────────────┐
    └────┬────┘                                                          │
         │ IHE submits                                                   │
         ▼                                                               │
    ┌─────────────────┐                                                  │
    │  IHE_SUBMITTED  │ ──────────────────────────────────────────────┐  │
    └────────┬────────┘                                               │  │
             │ LEA receives notification                              │  │
             ▼                                                        │  │
    ┌─────────────────┐          ┌─────────────────────┐              │  │
    │  LEA_REVIEWING  │ ────────▶│  RETURNED_TO_IHE    │──────────────┘  │
    └────────┬────────┘  Return  └─────────────────────┘                 │
             │ LEA approves                                              │
             ▼                                                           │
    ┌─────────────────┐                                                  │
    │  LEA_APPROVED   │                                                  │
    └────────┬────────┘                                                  │
             │ LEA submits batch                                         │
             ▼                                                           │
    ┌─────────────────┐                                                  │
    │  CTC_SUBMITTED  │                                                  │
    └────────┬────────┘                                                  │
             │ CTC begins review                                         │
             ▼                                                           │
    ┌─────────────────┐          ┌─────────────────────┐                 │
    │  CTC_REVIEWING  │ ────────▶│     REJECTED        │ ◀───────────────┘
    └────────┬────────┘  Reject  └─────────────────────┘   (Terminal)
             │ CTC approves
             ▼
    ┌─────────────────┐
    │    APPROVED     │ ───────────────────────────────────────────────┐
    └────────┬────────┘                                                │
             │ Batch disbursed, reports due                            │
             ▼                                                         │
    ┌─────────────────┐                                                │
    │  REPORTING_DUE  │                                                │
    └────────┬────────┘                                                │
             │ IHE submits report                                      │
             ▼                                                         │
    ┌─────────────────────────┐                                        │
    │  IHE_REPORT_SUBMITTED   │                                        │
    └────────────┬────────────┘                                        │
                 │ LEA submits report                                  │
                 ▼                                                     │
    ┌─────────────────────────┐                                        │
    │  LEA_REPORT_SUBMITTED   │                                        │
    └────────────┬────────────┘                                        │
                 │ Both reports approved                               │
                 ▼                                                     │
    ┌─────────────────────────┐                                        │
    │    REPORTS_COMPLETE     │                                        │
    └────────────┬────────────┘                                        │
                 │                                                     │
                 ▼                                                     │
    ┌─────────────────────────┐                                        │
    │        CLOSED           │ ◀──────────────────────────────────────┘
    └─────────────────────────┘   (Terminal)
```

### 8.2 Batch Status State Machine

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          BATCH STATUS FLOW (STSP)                            │
└─────────────────────────────────────────────────────────────────────────────┘

    ┌─────────────────┐
    │ PENDING_REVIEW  │
    └────────┬────────┘
             │
         ┌───┴───┐
         │       │
         ▼       ▼
    ┌─────────┐  ┌─────────┐
    │APPROVED │  │REJECTED │ (Terminal)
    └────┬────┘  └─────────┘
         │
         ▼
    ┌─────────────────┐
    │    GAA_SENT     │ ← DocuSign sent
    └────────┬────────┘
             │
             ▼
    ┌─────────────────┐
    │   GAA_SIGNED    │ ← All parties signed
    └────────┬────────┘
             │
             ▼
    ┌─────────────────┐
    │   PO_PENDING    │ ← Waiting for PO from Fi$CAL
    └────────┬────────┘
             │
             ▼
    ┌─────────────────┐
    │   PO_UPLOADED   │ ← PO document uploaded
    └────────┬────────┘
             │
             ▼
    ┌─────────────────────┐
    │ INVOICE_GENERATED   │ ← Invoice PDF created
    └──────────┬──────────┘
               │
               ▼
    ┌─────────────────────┐
    │   INVOICE_SENT      │ ← Sent to CTC Accounting
    └──────────┬──────────┘
               │
               ▼
    ┌─────────────────────┐
    │  WARRANT_PENDING    │ ← Awaiting payment confirmation
    └──────────┬──────────┘
               │
               ▼
    ┌─────────────────────┐
    │      COMPLETE       │ (Terminal)
    └─────────────────────┘
```

### 8.3 WorkflowState Seed Data (STSP)

```sql
-- Candidate States
INSERT INTO WorkflowState (GrantProgramId, EntityType, Code, DisplayName, Sequence, Category, IsTerminal, AllowedTransitionsJson)
VALUES
(1, 'CANDIDATE', 'DRAFT', 'Draft', 1, 'SUBMISSION', 0, '["IHE_SUBMITTED"]'),
(1, 'CANDIDATE', 'IHE_SUBMITTED', 'Submitted to LEA', 2, 'SUBMISSION', 0, '["LEA_REVIEWING"]'),
(1, 'CANDIDATE', 'LEA_REVIEWING', 'LEA Reviewing', 3, 'SUBMISSION', 0, '["LEA_APPROVED", "RETURNED_TO_IHE"]'),
(1, 'CANDIDATE', 'RETURNED_TO_IHE', 'Returned to IHE', 4, 'SUBMISSION', 0, '["IHE_SUBMITTED"]'),
(1, 'CANDIDATE', 'LEA_APPROVED', 'LEA Approved', 5, 'SUBMISSION', 0, '["CTC_SUBMITTED"]'),
(1, 'CANDIDATE', 'CTC_SUBMITTED', 'Submitted to CTC', 6, 'REVIEW', 0, '["CTC_REVIEWING"]'),
(1, 'CANDIDATE', 'CTC_REVIEWING', 'Under Review', 7, 'REVIEW', 0, '["APPROVED", "REJECTED"]'),
(1, 'CANDIDATE', 'APPROVED', 'Approved', 8, 'REVIEW', 0, '["REPORTING_DUE"]'),
(1, 'CANDIDATE', 'REJECTED', 'Rejected', 99, 'TERMINAL', 1, '[]'),
(1, 'CANDIDATE', 'REPORTING_DUE', 'Reports Due', 9, 'REPORTING', 0, '["IHE_REPORT_SUBMITTED"]'),
(1, 'CANDIDATE', 'IHE_REPORT_SUBMITTED', 'IHE Report Submitted', 10, 'REPORTING', 0, '["LEA_REPORT_SUBMITTED"]'),
(1, 'CANDIDATE', 'LEA_REPORT_SUBMITTED', 'LEA Report Submitted', 11, 'REPORTING', 0, '["REPORTS_COMPLETE"]'),
(1, 'CANDIDATE', 'REPORTS_COMPLETE', 'Reports Complete', 12, 'REPORTING', 0, '["CLOSED"]'),
(1, 'CANDIDATE', 'CLOSED', 'Closed', 100, 'TERMINAL', 1, '[]');

-- Batch States
INSERT INTO WorkflowState (GrantProgramId, EntityType, Code, DisplayName, Sequence, Category, IsTerminal, AllowedTransitionsJson)
VALUES
(1, 'BATCH', 'PENDING_REVIEW', 'Pending Review', 1, 'REVIEW', 0, '["APPROVED", "REJECTED"]'),
(1, 'BATCH', 'APPROVED', 'Approved', 2, 'REVIEW', 0, '["GAA_SENT"]'),
(1, 'BATCH', 'REJECTED', 'Rejected', 99, 'TERMINAL', 1, '[]'),
(1, 'BATCH', 'GAA_SENT', 'GAA Sent', 3, 'DISBURSEMENT', 0, '["GAA_SIGNED"]'),
(1, 'BATCH', 'GAA_SIGNED', 'GAA Signed', 4, 'DISBURSEMENT', 0, '["PO_PENDING"]'),
(1, 'BATCH', 'PO_PENDING', 'PO Pending', 5, 'DISBURSEMENT', 0, '["PO_UPLOADED"]'),
(1, 'BATCH', 'PO_UPLOADED', 'PO Uploaded', 6, 'DISBURSEMENT', 0, '["INVOICE_GENERATED"]'),
(1, 'BATCH', 'INVOICE_GENERATED', 'Invoice Generated', 7, 'DISBURSEMENT', 0, '["INVOICE_SENT"]'),
(1, 'BATCH', 'INVOICE_SENT', 'Invoice Sent', 8, 'DISBURSEMENT', 0, '["WARRANT_PENDING"]'),
(1, 'BATCH', 'WARRANT_PENDING', 'Warrant Pending', 9, 'DISBURSEMENT', 0, '["COMPLETE"]'),
(1, 'BATCH', 'COMPLETE', 'Complete', 100, 'TERMINAL', 1, '[]');
```

### 8.4 LEA Portal Status Display

The prototype implements role-based status views. LEAs see a simplified 3-tier status while the database maintains detailed workflow states:

| LEA Display | Maps From (Detailed Status) |
|-------------|----------------------------|
| **UNDER_REVIEW** | DRAFT, PENDING_LEA, SUBMITTED, LEA_REVIEW, CTC_SUBMITTED, CTC_REVIEWING |
| **APPROVED** | CTC_APPROVED, APPROVED, GAA_PENDING, GAA_GENERATED, GAA_SIGNED |
| **PAID** | INVOICE_*, PAYMENT_*, WARRANT_*, REPORTING_*, COMPLETE |

This mapping is stored in `CandidateStatusSVT.LEADisplayCategory` and applied in the LEA portal controller via `MapToLEAStatus()`.

---

## 9. Integration Points

### 9.1 ECS (Educator Credentialing System)

| GMS Entity | ECS Data Source | Sync Strategy |
|------------|-----------------|---------------|
| Organization (LEA) | ECS Districts | Nightly batch sync |
| Organization.EmailDomain | ECS Contact emails | Initial setup |
| Candidate.SEID | ECS SEID Lookup API | Real-time on entry |
| Candidate.SchoolCdsCode | ECS Schools | Cascading dropdown |
| User suggestions | ECS Contacts | Autocomplete lookup |

**ECS Data Used:**
- Counties (58 California counties)
- Districts (1,000+ LEAs)
- Schools (10,000+ schools)
- Contacts (GMS_CONTACT, FISCAL roles)
- SEID Records (for candidate verification)

### 9.2 DocuSign

**Integration Points:**
- GAA document generation (PDF)
- Envelope creation with 3 signers
- Signing ceremony URL generation
- Webhook for completion notification
- Signed document retrieval

**Signing Flow:**
1. LEA Authorized Signatory
2. CTC Staff Signatory #1
3. CTC Staff Signatory #2

### 9.3 Fi$CAL (Future)

**V1:** Manual PO/Warrant entry
**V2:** API integration for:
- PO creation/lookup
- Invoice submission
- Voucher status
- Warrant confirmation

### 9.4 EntraID

**CTC Staff Authentication:**
- SSO via Microsoft EntraID
- Role mapping from AD groups
- Session management

**Grantee Authentication (V1):**
- OAuth with Google/Microsoft
- Email domain validation against ECS

---

## 10. Recommendations

### 10.1 V1 Implementation Order

1. **Database Schema**
   - Create all tables including V2 extension tables
   - Add nullable columns for future use
   - Seed WorkflowState with STSP states
   - Seed GrantProgram with STSP configuration

2. **Core Entities**
   - GrantProgram, GrantCycle, Organization, User
   - Application, Candidate
   - Keep IHEOrgId/LEAOrgId convenience columns

3. **Batch & Disbursement**
   - Batch, BatchCandidate
   - Disbursement (one per batch, Phase=FULL)

4. **Reporting**
   - ReportingPeriod, CandidateReport
   - IHE_COMPLETION and LEA_PAYMENT types

5. **Supporting**
   - Notification, AuditLog
   - WorkflowState validation

### 10.2 V2 Priority Roadmap

| Priority | Grant(s) | New Entities | Config Changes |
|----------|----------|--------------|----------------|
| 1 | Computer Science, Reading & Literacy | Optional: CandidateFunding | MaxAwardAmount, LEA-only workflow |
| 2 | Integrated Teacher Prep | CandidateYearProgress | IHE-only workflow |
| 3 | Classified | CandidateYearProgress | Replacements, LEA-only |
| 4 | DELPI | (Uses ExtendedDataJson) | Cohorts, ParticipantType |
| 5 | Teacher Residency, School Counselor | CandidateFunding, CandidateYearOutcome | MatchingFunds, 4-year tracking |

### 10.3 Technical Recommendations

1. **Use JSON columns wisely**
   - ExtendedDataJson for grant-specific fields
   - Validate against schema in application code
   - Index commonly queried JSON paths

2. **Implement proper audit logging**
   - All status changes logged
   - User, timestamp, old/new values
   - 7-year retention per CTC requirements

3. **Design for reporting**
   - Denormalize aggregates on Batch (CandidateCount, TotalAmount)
   - Consider materialized views for dashboards
   - Plan for legislative reporting requirements

4. **Plan for scale**
   - 5,000+ candidates per cycle
   - 200+ concurrent users
   - Batch operations for bulk uploads

### 10.4 Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Schema changes for new grants | JSON extension columns, V2 tables pre-created |
| Workflow complexity | WorkflowState table, transition validation |
| ECS sync failures | Graceful degradation, manual entry fallback |
| DocuSign failures | Retry logic, manual GAA upload option |
| Performance at scale | Indexed queries, pagination, caching |

---

## Appendices

### Appendix A: Glossary

| Term | Definition |
|------|------------|
| **STSP** | Student Teacher Stipend Program |
| **IHE** | Institution of Higher Education (university) |
| **LEA** | Local Education Agency (school district) |
| **CTC** | California Commission on Teacher Credentialing |
| **ECS** | Educator Credentialing System |
| **GAA** | Grant Award Agreement |
| **PO** | Purchase Order |
| **SEID** | Statewide Educator Identifier |
| **CDS** | County-District-School code |
| **Fi$CAL** | California's financial management system |

### Appendix B: Status Code Reference

**Candidate Statuses:**
| Code | Display | Category |
|------|---------|----------|
| DRAFT | Draft | SUBMISSION |
| IHE_SUBMITTED | Submitted to LEA | SUBMISSION |
| LEA_REVIEWING | LEA Reviewing | SUBMISSION |
| RETURNED_TO_IHE | Returned to IHE | SUBMISSION |
| LEA_APPROVED | LEA Approved | SUBMISSION |
| CTC_SUBMITTED | Submitted to CTC | REVIEW |
| CTC_REVIEWING | Under Review | REVIEW |
| APPROVED | Approved | REVIEW |
| REJECTED | Rejected | TERMINAL |
| REPORTING_DUE | Reports Due | REPORTING |
| IHE_REPORT_SUBMITTED | IHE Report Submitted | REPORTING |
| LEA_REPORT_SUBMITTED | LEA Report Submitted | REPORTING |
| REPORTS_COMPLETE | Reports Complete | REPORTING |
| CLOSED | Closed | TERMINAL |

**Batch Statuses:**
| Code | Display | Category |
|------|---------|----------|
| PENDING_REVIEW | Pending Review | REVIEW |
| APPROVED | Approved | REVIEW |
| REJECTED | Rejected | TERMINAL |
| GAA_SENT | GAA Sent | DISBURSEMENT |
| GAA_SIGNED | GAA Signed | DISBURSEMENT |
| PO_PENDING | PO Pending | DISBURSEMENT |
| PO_UPLOADED | PO Uploaded | DISBURSEMENT |
| INVOICE_GENERATED | Invoice Generated | DISBURSEMENT |
| INVOICE_SENT | Invoice Sent | DISBURSEMENT |
| WARRANT_PENDING | Warrant Pending | DISBURSEMENT |
| COMPLETE | Complete | TERMINAL |

### Appendix C: Data Field Mapping by Grant

**Common Fields (All Grants):**
- Candidate Name (First, Last)
- SEID
- Demographics (Race, Ethnicity, Gender)

**STSP-Specific:**
- Date of Birth
- Last 4 SSN
- Credential Area
- School CDS Code
- Practicum Hours

**Teacher Residency / School Counselor:**
- Enrolled Date
- Expected Completion Date
- Mentor Start/End Dates
- TPA Type, Attempts, Passed
- Year 1-4 Employment (LEA, School, Grade, Subject)
- Funding by Category (Grant + Match)

**Classified:**
- Employment Classification
- Education Level at Start
- Program Start/Expected Completion
- Annual Progress (Degree, Credential)
- Replacement Participant Info

**Computer Science / Reading & Literacy:**
- Current Credential
- Teaching Tenure
- School Characteristics (Rural, High-Need)
- Authorization Progress
- Expense Categories

**DELPI:**
- Cohort Year
- Position at Start
- Years of Experience
- Participant Type (Candidate vs Admin)

**Integrated:**
- Teacher Shortage Area
- BA Major
- Credential Pursuing
- Academic Status
- Dual Progress (Degree + Credential)

---

*Document Version: 1.0*
*Last Updated: December 2025*
*Author: GMS Technical Team*
