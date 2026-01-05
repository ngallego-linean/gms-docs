# CTC Grant Programs Comparison

**California Commission on Teacher Credentialing**
**Grant Management System - Program Analysis**
**December 2025**

---

## Executive Summary

CTC manages 8 grant programs for teacher preparation and development. Version 1 of GMS targets **STSP (Student Teacher Stipend Program)**. This document analyzes what's common vs. unique across all programs to inform system design.

---

## 1. Programs Overview

| Code | Program Name | Primary Org | Secondary Org | Award Model | V1 Ready |
|------|--------------|-------------|---------------|-------------|----------|
| **STSP** | Student Teacher Stipend Program | IHE | LEA (completes) | Fixed $10K | ✅ Yes |
| **TRP** | Teacher Residency Program | IHE | LEA (partner) | Budgeted | ⚠️ V2 |
| **SCRP** | School Counselor Residency Program | IHE | LEA (partner) | Budgeted | ⚠️ V2 |
| **CSEP** | Classified School Employee Program | LEA | None | Budgeted | ⚠️ V2 |
| **CSGF** | Computer Science Supplementary Auth Grant | LEA | None | Capped $2.5K | ✅ Low effort |
| **RLGF** | Reading & Literacy Grant | LEA | None | Capped $2.5K | ✅ Low effort |
| **DELPI** | Diverse Education Leadership Pipeline | LEA | None | Budgeted | ⚠️ V2 |
| **ITP** | Integrated Teacher Preparation | IHE | None | Budgeted | ✅ Low effort |

---

## 2. Common Elements (All Programs)

### 2.1 Workflow Phases
All programs share these high-level phases:
1. **Legislation/Budget** - Grant passes through state legislature
2. **RFA Publication** - CTC creates webpage with application info
3. **Application** - Organizations apply for funding
4. **Review** - CTC staff reviews applications
5. **Award** - System generates Grant Award Agreement (GAA)
6. **Disbursement** - Funds distributed to grantees
7. **Reporting** - Grantees submit required reports
8. **Dashboard/Analytics** - Performance tracking

### 2.2 Common Data Fields

**Organization Info:**
- Organization Name, Type (IHE/LEA/COE)
- CDS Code (County-District-School)
- Contact Information (Email, Phone)
- GAA Signer, Fiscal Agent, Superintendent

**Candidate/Participant Info:**
- Name (First, Last)
- Date of Birth
- Last 4 SSN/ITIN
- SEID (State Educator ID) - matched from ECS
- Demographics (Race, Ethnicity, Gender)
- Credential Area

**Fiscal Info:**
- Appropriated Amount
- Encumbered Amount
- Disbursed Amount
- Remaining Amount
- Encumbrance Date
- Liquidation Date

### 2.3 Common Integrations
- **ECS** - Educator Credentialing System (SEID lookup, credential verification)
- **DocuSign** - GAA signature workflow
- **Fi$Cal** - State fiscal system (PO, Invoice, Warrant)

### 2.4 Common Reports
- Application status
- Funding utilization
- Demographic breakdown
- Completion/outcome metrics

---

## 3. Program-Specific Details

### 3.1 STSP - Student Teacher Stipend Program (V1 Target)

**Purpose:** Provide $10,000 stipends to student teachers completing 540+ hours of clinical practice.

**Workflow:**
```
IHE submits candidate → LEA completes application → CTC reviews →
GAA sent via DocuSign → LEA signs → PO created in Fi$Cal →
Invoice generated → Warrant issued → Payment distributed →
Reports submitted (IHE completion + LEA payment)
```

**Unique Characteristics:**
| Aspect | Description |
|--------|-------------|
| Fixed Award | Exactly $10,000 per candidate |
| Hours Threshold | 540+ hours of clinical practice |
| Credential Hours | 600 hours for credential requirement |
| Split Payments | $5,000 each if candidate in 2 LEAs |
| IHE-LEA Handoff | IHE starts, LEA completes application |
| Batching | Monthly submission batches ("Disbursement Groups") |

**Data Collected:**
- Credential Area (19 options - see Reference Data)
- School CDS Code (placement location)
- LEA POC (Point of Contact)
- How LEA paid student teacher (one payment, two payments, etc.)
- Payment method (independent contractor, etc.)

**Credential Areas (STSP):**
1. Multiple Subject
2. Single Subject - English, Mathematics, Sciences (Bio, Chem, Geo, Physics), Social Science, World Languages (Spanish, French, Other), Art, Music, Physical Education
3. Education Specialist - Mild/Moderate, Moderate/Severe, Deaf/Hard of Hearing, Visual Impairments, Early Childhood Special Education

**Reports Required:**
1. IHE Completion Report - program completion status, credential earned
2. LEA Payment Report - payment distribution confirmation, employment status

---

### 3.2 TRP - Teacher Residency Program

**Purpose:** Multi-year funding for teacher residency programs with IHE-LEA partnerships.

**Workflow:**
```
IHE submits candidates → LEA confirms partnership → CTC reviews →
Multi-year funding allocated → Annual progress tracking →
4-year post-completion employment tracking
```

**Unique Characteristics:**
| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| Dual Funding | Grant funds + LEA matching funds | FundingSource field needed |
| Budget Categories | Tuition, stipends, exam fees, mentor PD | CandidateFunding entity |
| Multi-Year Tracking | Years 1-4 post-completion | CandidateYearOutcome entity |
| Retention Metrics | "Same LEA/School as residency?" | Fields on YearOutcome |
| Mentor Tracking | Mentor start/end dates, feedback | Additional fields |

**Additional Data:**
- Enrolled Date, Expected Completion Date
- Mentor teacher name, start/end dates
- TPA type and attempts
- Per-category funding (Grant + Match):
  - Tuition assistance
  - Stipends
  - Exam fees
  - Mentor professional development
- Year 1-4 Outcomes: Employer, School CDS, Grade Level, Subject Area

**Model Fit:** ⚠️ Needs V2 extensions (CandidateFunding, CandidateYearOutcome)

---

### 3.3 SCRP - School Counselor Residency Program

**Purpose:** Same structure as Teacher Residency but for school counselors.

**Workflow:** Identical to Teacher Residency

**Unique Characteristics:**
| Aspect | Description |
|--------|-------------|
| Credential Type | PPS-SC (Pupil Personnel Services - School Counselor) |
| Placement Tracking | Counseling placements, not teaching |
| 4-Year Tracking | Same as TRP |

**Model Fit:** ⚠️ Same extensions as Teacher Residency
- CandidateYearOutcome.Position = "COUNSELOR"

---

### 3.4 CSEP - Classified School Employee Program

**Purpose:** Help classified school employees become credentialed teachers.

**Workflow:**
```
LEA submits participants → CTC reviews → Multi-year funding →
Annual progress tracking → Credential completion
```

**Unique Characteristics:**
| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| No IHE Workflow | LEA is sole grantee | WorkflowType = "LEA_CTC" |
| Replacements | Exited participant can be replaced | Candidate.ReplacedCandidateId |
| Annual Progress | Degree + Credential progress yearly | CandidateYearProgress entity |
| Multi-Year Funding | Same participant funded across years | FiscalYear on funding |
| Education Level | Track degree at start vs. progress | ExtendedDataJson |

**Additional Data:**
- Employment classification
- Program start/expected completion dates
- Education level at enrollment
- Years of funding expected
- Criminal background check passed
- Annual: Degree progress, Credential progress, Employed next year

**Model Fit:** ⚠️ Needs extensions (Replacements, CandidateYearProgress)

---

### 3.5 CSGF - Computer Science Supplementary Authorization Grant

**Purpose:** Help existing teachers add Computer Science supplementary authorization.

**Workflow:**
```
LEA submits participants → CTC reviews →
Capped funding ($2,500 max) → Authorization tracking
```

**Unique Characteristics:**
| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| Existing Teachers | Already credentialed | Different participant type |
| Capped Awards | Up to $2,500 per participant | GrantCycle.MaxAwardAmount |
| Expense Categories | Tuition, books, release time, filing fees | Optional CandidateFunding |
| School Characteristics | Rural, high-need designation | ExtendedDataJson |

**Additional Data:**
- Current credential type
- Teaching tenure (years)
- Rural school indicator
- High-need school indicator
- Authorization progress (Made progress? Completed?)
- Expense breakdown by category

**Model Fit:** ✅ Mostly works with existing model

---

### 3.6 RLGF - Reading & Literacy Grant

**Purpose:** Help existing teachers add Reading/Literacy supplementary authorization.

**Workflow:** Same as Computer Science

**Unique Characteristics:**
- Authorization type: RLAA/RLLS (Reading/Literacy)
- Same $2,500 cap structure
- Same expense categories
- Eligible school site requirement

**Model Fit:** ✅ Same as Computer Science

---

### 3.7 DELPI - Diverse Education Leadership Pipeline Initiative

**Purpose:** Develop diverse education administrators and leaders.

**Workflow:**
```
LEA submits participants → CTC reviews →
Multi-cohort tracking → Admin outcome tracking
```

**Unique Characteristics:**
| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| Multi-Cohort | 24-25, 25-26, 26-27 cohorts simultaneous | Candidate.CohortYear |
| Two Populations | Candidates + Current Admins | Candidate.ParticipantType |
| Admin Fields | Position, experience, credential at start | ExtendedDataJson |
| Narrative Reports | Qualitative summary narrative | Report.NarrativeText |

**Additional Data:**
- Cohort year
- Participant type (Candidate vs Current Admin)
- Position at program start
- Years of full-time experience
- Credential at program start
- Education level at start

**Model Fit:** ⚠️ Needs extensions (Cohorts, ParticipantType)

---

### 3.8 ITP - Integrated Teacher Preparation Program

**Purpose:** Support integrated BA + credential programs at IHEs.

**Workflow:**
```
IHE submits participants → CTC reviews →
Multi-year funding → Degree + Credential tracking
```

**Unique Characteristics:**
| Aspect | Description | Model Impact |
|--------|-------------|--------------|
| IHE-Only | No LEA in workflow | WorkflowType = "IHE_CTC" |
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

**Model Fit:** ✅ Mostly works with existing model

---

## 4. Workflow Type Comparison

| Workflow Type | Programs | Flow |
|---------------|----------|------|
| IHE → LEA → CTC | STSP, TRP, SCRP | IHE initiates, LEA completes, CTC approves |
| LEA → CTC | CSEP, CSGF, RLGF, DELPI | LEA only, CTC approves |
| IHE → CTC | ITP | IHE only, CTC approves |

---

## 5. Award Model Comparison

| Model | Programs | Description |
|-------|----------|-------------|
| **Fixed** | STSP | Exactly $10,000 per candidate |
| **Capped** | CSGF, RLGF | Up to $2,500 per participant |
| **Budgeted** | TRP, SCRP, CSEP, DELPI, ITP | Variable based on approved budget |

---

## 6. Tracking Requirements

| Tracking Type | Programs | Duration |
|---------------|----------|----------|
| One-time | STSP, CSGF, RLGF | Single payment + report |
| Multi-year funding | TRP, SCRP, CSEP, ITP | 2-5 years |
| Post-completion outcomes | TRP, SCRP | 4 years post-completion |
| Cohort-based | DELPI | Multiple simultaneous cohorts |

---

## 7. V2 Extension Requirements

### 7.1 New Tables Needed

| Table | Purpose | Programs |
|-------|---------|----------|
| CandidateFunding | Per-category funding (Grant + Match) | TRP, SCRP, CSEP, CSGF, RLGF, ITP |
| CandidateYearOutcome | Post-completion employment tracking | TRP, SCRP |
| CandidateYearProgress | Annual degree/credential progress | CSEP, ITP |

### 7.2 New Fields Needed

| Field | Table | Purpose | Programs |
|-------|-------|---------|----------|
| ReplacedCandidateId | Candidate | Replacement participant link | CSEP |
| CohortYear | Candidate | Which cohort (24-25, 25-26, etc.) | DELPI |
| ParticipantType | Candidate | Candidate vs Current Admin | DELPI |
| WorkflowType | GrantProgram | IHE_LEA_CTC, LEA_CTC, IHE_CTC | All |
| HasMatchingFunds | GrantProgram | Requires LEA match | TRP, SCRP |
| TrackingYears | GrantProgram | Post-completion tracking duration | TRP, SCRP |
| HasReplacements | GrantProgram | Allows participant replacement | CSEP |
| HasCohorts | GrantProgram | Multi-cohort tracking | DELPI |
| MaxAwardAmount | GrantCycle | Per-participant cap | CSGF, RLGF |

---

## 8. Implementation Priority

Based on complexity and model fit:

| Priority | Programs | Effort | Rationale |
|----------|----------|--------|-----------|
| 1 (V1) | **STSP** | Done | Core model designed for this |
| 2 | CSGF, RLGF | Low | Capped awards + optional funding detail |
| 3 | ITP | Low | IHE-only workflow + progress tracking |
| 4 | CSEP | Medium | Progress tracking + replacements |
| 5 | DELPI | Medium | Cohort tracking + participant types |
| 6 | TRP, SCRP | High | Dual funding + 4-year outcome tracking |

---

## 9. Source Documents

| Document | Location | Content |
|----------|----------|---------|
| GMS-Data-Model-Report.md | /erd/ | Complete data model analysis |
| GMS-V1-Schema.md | /erd/ | Database schema with seed data |
| Process Flow Template.xlsx | Downloads | STSP and General workflow details |
| CTC_Student_Upload_Template.xlsx | Downloads | STSP data fields and reference data |
| GMS-ERD.mermaid | /erd/ | Entity relationship diagram |
| transcript.txt | /gms-docs/ | Meeting notes on STSP workflow |

---

## 10. Terminology Mapping

| Database Term | UI Term (STSP) | Notes |
|---------------|----------------|-------|
| Batch | Disbursement Group | Monthly grouping of candidates |
| Application | Submission | IHE/LEA submission of candidates |
| Candidate | Student Teacher | Person receiving stipend |
| Initiator | IHE | Organization that starts process |
| Completer | LEA | Organization that completes process |
| GAA | Grant Award Agreement | DocuSign document |
