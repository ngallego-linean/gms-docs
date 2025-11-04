# CTC Grant Templates: Universal vs Grant-Specific Patterns

**Analysis Date:** October 24, 2025
**Templates Analyzed:** 8 grant programs
**Purpose:** Identify what belongs in core platform vs grant pack configuration

---

## Executive Summary

After analyzing 8 CTC grant templates, we've identified strong universal patterns that suggest a highly reusable core platform with grant-specific customization through configuration packs.

**Key Findings:**
- **70%+ templates share:** Instructions sheet, Program Enrollment sheet
- **50%+ templates share:** Demographics fields (ethnicity, gender, sexual orientation), funding tracking, summary narratives
- **All templates use:** Data validations (lists, custom, text length, dates)
- **Named ranges are universal:** CredentialProgram, Ethnicity, GenderIdentification, SexualOrientation appear in 7/8 templates

**Recommendation:** Build a "Program Lifecycle Platform" with universal entities and workflows, configurable per grant via packs.

---

## Universal Patterns (Core Platform Features)

These appear in 70%+ of templates and should be **built into the core system**.

### 1. Universal Sheet Types (Workflow Stages)

| Sheet Type | Occurrences | Purpose | Core Entity |
|------------|-------------|---------|-------------|
| **Instructions** | 7/8 (88%) | Grant-specific guidance | `GrantPage` (CMS) |
| **Program Enrollment** | 6/8 (75%) | Initial participant registration | `Participant` + `Application` |

**Implication:** Every grant has an enrollment phase and needs instructions. Build:
- Generic "Enrollment" workflow stage
- CMS-driven instructions per grant pack
- Participant entity with extensible JSON for grant-specific fields

### 2. Universal Demographic Fields

| Field | Occurrences | Type | Implementation |
|-------|-------------|------|----------------|
| **Ethnicity/Race** | 7/8 templates | Dropdown (named range) | Lookup table + named range in pack |
| **Gender Identification** | 7/8 templates | Dropdown (self-identified) | Lookup table + named range in pack |
| **Sexual Orientation** | 7/8 templates | Dropdown (self-identified) | Lookup table + named range in pack |
| **Academic Year** | 5/8 templates | Text/Year | Computed or entered |

**Implication:** Demographics are collected via participant "magic link" forms. Core platform provides:
- Standard `Participant` demographic fields (nullable)
- Configurable visibility per grant (some grants don't collect all demographics)
- Data suppression rules built-in (n < 11)

### 3. Universal Validation Types

| Validation Type | Occurrences | Implementation Strategy |
|-----------------|-------------|-------------------------|
| **List** (dropdowns) | 7/8 templates | JsonSchema enum + named ranges in pack |
| **Custom** (formulas) | 6/8 templates | JsonLogic/CEL rules in pack |
| **Text Length** | 5/8 templates | JsonSchema minLength/maxLength |
| **Date** | 4/8 templates | JsonSchema format: date + min/max |

**Implication:** All templates use data validation extensively. Pack configuration must support:
- Named ranges → enum lists
- Custom formula validation → expression engine
- Standard types (text length, dates) → JSON Schema

### 4. Universal Named Ranges

| Named Range | Occurrences | Purpose | Implementation |
|-------------|-------------|---------|----------------|
| **CredentialProgram** | 7/8 | Credential type lookup | Pack-specific enum |
| **Ethnicity** | 7/8 | Race/ethnicity dropdown | Universal lookup (CDE standard) |
| **GenderIdentification** | 7/8 | Gender dropdown | Universal lookup (CDE standard) |
| **SexualOrientation** | 7/8 | Sexual orientation dropdown | Universal lookup |

**Implication:** Lookups are foundational. Core system needs:
- Universal lookups (demographics) in database
- Grant-specific lookups (credential programs) in pack config
- Named range resolver that maps pack references to actual data

### 5. Universal Workflow Pattern

**All templates follow this lifecycle:**

```
Instructions → Enrollment → Progress Tracking → Funding → Completion → Summary/Narrative
```

**Core workflow states (applies to all grants):**
1. **Setup:** LEA reads instructions, determines eligibility
2. **Enrollment:** Submit participant information
3. **Progress:** Track participant progress over multiple years
4. **Funding:** Record grant and match amounts
5. **Completion:** Record credential attainment and placement
6. **Summary:** Annual narrative + quantitative summary

**Implication:** Build a generic "Program Lifecycle" state machine with these stages; grant packs configure:
- Which stages are required
- What data is collected at each stage
- Validation rules per stage

### 6. Universal Summary/Reporting Pattern

**Every template has:**
- **Summary Data** sheet: Quantitative rollups (counts, totals, averages)
- **Summary Narrative** sheet: Qualitative reflections (annual + final)
- Auto-population from other tabs (formulas)

**Common narrative prompts:**
- How is this grant helping you meet objectives?
- What challenges did you encounter?
- What are the outcomes/impacts?

**Implication:**
- Annual survey functionality is universal (gates final 10% disbursement)
- Reports auto-aggregate from participant data
- Pack defines survey questions + quantitative metrics

---

## Grant-Specific Patterns (Pack Configuration)

These vary significantly by grant and should be **defined in grant packs**.

### 1. Grant-Specific Sheets

Different grants have unique tabs based on their requirements:

| Template | Unique Sheets | Purpose |
|----------|---------------|---------|
| **School Counselor** | Year 1-4 School Counseling | Multi-year service tracking |
| **Teacher Residency** | Year 1-4 Teaching | Multi-year service tracking |
| **DELPI** | 24-25/25-26/26-27 Cohorts, Current Admins, Planning Space | Cohort-based admin pipeline |
| **Classified/Integrated/CS/R&L** | Budget, Data Fields, Funding, Program Progress | Standard grant tracking |

**Patterns:**
- **Residency grants** (SC, TR): Multi-year service requirement tabs (4 years post-completion)
- **Pipeline grants** (DELPI): Cohort-based tracking across multiple entry years
- **Implementation grants** (others): Budget, funding, progress tracking

**Implication:** Pack configuration defines:
- Number of years to track
- Cohort structure (single vs multi-cohort)
- Service requirements (yes/no, how many years)

### 2. Grant-Specific Columns

Each grant collects different participant data:

| Grant Type | Unique Columns | Reasoning |
|------------|----------------|-----------|
| **Teacher Residency** | Credential Pathway, Teacher Shortage Area, Placement School, Subject Taught | Teaching-specific |
| **School Counselor** | PPS Credential Status, School Counseling Placement | Counseling-specific |
| **DELPI** | Admin Credential Type, Service Hours, Support Received | Admin pipeline-specific |
| **Integrated** | CCC Partner, Planning Grant Type, BA Major | Higher ed integration |
| **Classified** | Education Level at Start, CCC/IHE Info | Para-educator to teacher pipeline |

**Implication:** Pack schema defines:
- Participant-level fields (JSON extensions on Participant table)
- Funding-level fields (JSON extensions on FundingLine table)
- Completion-level fields (credential, placement info)

### 3. Grant-Specific Funding Models

| Template | Funding Model | Max Per Participant | Match Required |
|----------|---------------|---------------------|----------------|
| **Teacher Residency** | Multi-year, Grant + Match tabs | Varies | Yes |
| **School Counselor** | Multi-year, Grant + Match tabs | Varies | Yes |
| **Computer Science** | Annual allocations | $2,500 | No |
| **Reading & Literacy** | Annual allocations | $2,500 | No |
| **Classified** | Multi-year grid | Varies | Sometimes |
| **Integrated** | Total grant amount | Varies | Varies |

**Implication:** Pack configuration defines:
- Per-participant caps (if any)
- Match requirements (yes/no, ratio)
- Multi-year vs annual funding
- Grant + match separate or combined

### 4. Grant-Specific Formulas

Each template has unique calculations:

**Examples:**
- **Budget totals:** `=SUM(B4:B20)` (universal pattern, different ranges)
- **Per-participant caps:** `=B5*2500` (CS/R&L: $2,500 max)
- **Match ratios:** `=GrantAmount * 0.5` (50% match required)
- **Auto-population:** `=IF(ISBLANK('Program Enrollment'!A3), "", 'Program Enrollment'!A3)` (copy name forward)

**Common formula types:**
- SUM, COUNT, AVERAGE (aggregates)
- IF, ISBLANK (conditionals)
- Cross-tab references (pull data from enrollment to funding)
- Caps and limits (max per participant, total budget)

**Implication:** Pack formulas.json defines:
- Computed fields (totals, ratios)
- Cross-field references
- Caps/limits as validation rules
- Auto-population via DAG

---

## Recommended Core Platform Entities

Based on universal patterns, these entities belong in the **normalized database schema**:

### Core Entities

```
Application
  ├─ LEA (Organization)
  ├─ GrantCycle
  ├─ Pack (id + version)
  ├─ Data (JSON: grant-specific fields)
  └─ Status (Draft, Submitted, InReview, etc.)

Participant
  ├─ Application (FK)
  ├─ FirstName, LastName, Email
  ├─ Ethnicity (FK to Lookup or JSON)
  ├─ GenderIdentification (FK to Lookup or JSON)
  ├─ SexualOrientation (FK to Lookup or JSON)
  ├─ Data (JSON: grant-specific fields like credential type, pathway, etc.)
  └─ ParticipantLinks (magic link tokens)

FundingLine
  ├─ Participant (FK)
  ├─ Year
  ├─ GrantAmount
  ├─ MatchAmount (nullable)
  ├─ Data (JSON: grant-specific funding details)

ProgressRecord
  ├─ Participant (FK)
  ├─ Year
  ├─ Status (Enrolled, Completed, Withdrew, etc.)
  ├─ Data (JSON: year-specific progress info)

CompletionRecord
  ├─ Participant (FK)
  ├─ CredentialAwarded (text or FK to Lookup)
  ├─ CompletionDate
  ├─ PlacementInfo (JSON: LEA, school, role)
  ├─ Data (JSON: grant-specific completion fields)

ServiceRecord (for residency grants)
  ├─ Participant (FK)
  ├─ Year (1-4)
  ├─ SchoolPlacement (text or FK to Organization)
  ├─ RoleType (Teacher, Counselor, Admin)
  ├─ Data (JSON: grant-specific service info)

SurveyResponse
  ├─ Application (FK)
  ├─ Year
  ├─ Answers (JSON: survey responses)
  ├─ CompletedAt

Lookup (universal dropdowns)
  ├─ Category (Ethnicity, GenderIdentification, SexualOrientation, States, etc.)
  ├─ Code
  ├─ DisplayValue
  ├─ SortOrder
```

### Grant Pack Configuration

```
Pack JSON Structure:

{
  "id": "teacher-residency",
  "version": "1.0.0",

  "lifecycle": {
    "stages": ["enrollment", "progress", "funding", "completion", "service"],
    "years_to_track": 4,
    "service_requirement": {
      "enabled": true,
      "years": 4,
      "type": "Teaching"
    }
  },

  "schema": {
    "participant": {
      "properties": {
        "credential_pathway": { "type": "string", "enum": [".."] },
        "teacher_shortage_area": { "type": "string" },
        "placement_school": { "type": "string" }
      }
    },
    "funding": {
      "max_per_participant": null,
      "match_required": true,
      "match_ratio": 1.0
    }
  },

  "ui": {
    "enrollment_form": [...],
    "progress_form": [...],
    "funding_grid": [...],
    "completion_form": [...],
    "service_form": [...]
  },

  "formulas": {
    "total_grant_funding": { "reduce": [...] },
    "total_match_funding": { "reduce": [...] },
    "budget_remaining": { "-": [...] }
  },

  "rules": {
    "enrollment": [...],
    "funding": [
      {
        "error": "Match must equal or exceed grant amount",
        "expr": { ">=": [{"var": "match_total"}, {"var": "grant_total"}] }
      }
    ]
  },

  "surveys": {
    "annual": {
      "questions": [...],
      "schedule": "yearly",
      "required_for_final_disbursement": true,
      "withheld_percent": 10
    }
  },

  "lookups": {
    "CredentialProgram": ["Multiple Subject", "Single Subject", "Education Specialist"],
    "PathwayType": ["Traditional", "Intern", "Residency"]
  },

  "reports": {
    "summary_data": {...},
    "legislative_report": {...},
    "fiscal_export": {...}
  }
}
```

---

## Implementation Recommendations

### Phase 1: Core Platform (April 2026 - STSP)

**Hard-code first pack, validate platform design:**

1. **Entities:** Application, Participant, FundingLine, ProgressRecord, CompletionRecord, ServiceRecord, SurveyResponse
2. **Workflows:** Generic lifecycle state machine (configurable stages)
3. **Forms:** Telerik UI renderer that consumes ui.json
4. **Validations:** JsonLogic/CEL engine for rules.json
5. **Formulas:** DAG evaluator for formulas.json
6. **Demographics:** Standard fields with configurable visibility
7. **Surveys:** Annual survey with gating (configurable questions)
8. **Reports:** Template-driven (SQL + Jinja)

**STSP pack v1.0 hard-coded initially, then extracted to JSON for v1.1**

### Phase 2: Pack Builder (April 2027)

**Enable self-service grant configuration:**

1. **Pack Builder UI:**
   - Field library (drag-drop)
   - Validation/formula editor
   - Preview with real Telerik controls
   - Linter (cycles, broken refs)

2. **Excel Importer:**
   - ClosedXML-based inference
   - Mapping DSL for curation
   - Round-trip validation

3. **Multi-Grant Runtime:**
   - Pack registry
   - Side-by-side packs
   - Pack versioning and migrations

### Phase 3: Advanced Features (Beyond 2027)

1. **Cohort Support:** Multi-cohort tracking (like DELPI)
2. **Multi-Year Service:** Automated service requirement tracking
3. **Budget Management:** Real-time budget/expenditure tracking across all grants
4. **Advanced Reporting:** Cross-grant analytics, dashboards, legislative reports

---

## Key Insights for Architecture

### 1. Hybrid Data Model is Essential

**Normalized tables** (Application, Participant, Funding, etc.) enable:
- Cross-grant queries ("Show all participants across all grants")
- Foreign key integrity
- Standard indexes and performance

**JSON extensions** enable:
- Grant-specific fields without schema migrations
- Flexibility to add new grants quickly
- Backward compatibility (old packs still work)

### 2. Named Ranges Are Foundational

**Every template uses named ranges for dropdowns.** Implement:
- Universal lookups in `Lookup` table (Ethnicity, Gender, etc.)
- Grant-specific lookups in pack JSON
- Named range resolver: `@CredentialProgram` → pack.lookups.CredentialProgram

### 3. Formulas Must Run Client + Server

**Templates have extensive formulas (SUM, IF, etc.).** Pattern:
- Define in formulas.json as JsonLogic DAG
- Evaluate client-side (instant feedback via JavaScript)
- Re-evaluate server-side (authoritative on save)
- Cache computed values in database for reporting

### 4. Multi-Year Tracking is Common

**Many grants track participants over 4-5 years:**
- Residency grants: 4-year service requirement
- Implementation grants: 4-year funding cycles
- Pipeline grants: Multiple cohorts entering over years

**Solution:** `ProgressRecord` and `ServiceRecord` with `Year` field; pack defines how many years.

### 5. Surveys Gate Final Disbursement

**Universal pattern: "Final 10% withheld until survey complete"**

Implement:
- Survey scheduling (annual reminders)
- Completion tracking
- Disbursement gate (check survey status before final payment)
- Pack defines survey questions + gate percent

---

## Summary: Universal vs Unique

| Aspect | Universal (Core Platform) | Unique (Grant Pack Config) |
|--------|---------------------------|----------------------------|
| **Workflow** | Enrollment → Progress → Funding → Completion → Survey | Number of years, service requirement, cohorts |
| **Demographics** | Ethnicity, Gender, Sexual Orientation | Visibility (some grants skip), additional fields |
| **Validations** | List, Custom, TextLength, Date | Specific rules, dropdown options, formulas |
| **Funding** | Grant amount, match amount (nullable) | Per-participant caps, match ratio, multi-year structure |
| **Surveys** | Annual survey with gating | Questions, schedule, withheld % |
| **Reporting** | Summary data, summary narrative | Metrics to aggregate, narrative prompts |
| **Lookups** | Universal demographics | Credential programs, pathways, grant-specific enums |
| **Formulas** | SUM, IF, cross-tab refs | Specific calculations, caps, ratios |

---

## Appendix: Template Comparison Matrix

### Enrollment Pattern

| Template | Enrollment Sheet | Key Fields | Special Notes |
|----------|------------------|------------|---------------|
| Teacher Residency | Program Enrollment | Name, Credential, Pathway, Demographics | 250+ row capacity |
| School Counselor | Program Enrollment | Name, PPS Credential, Demographics | 250+ row capacity |
| Classified | Program Enrollment | Name, Education Level, Credential Pursuing | Plus Data Fields tab |
| Integrated | Data Fields | Name, Planning Type, CCC Partner, Credential | Includes planning grants |
| Computer Science | Program Enrollment | Name, Credential, Completion Status | Teacher-focused |
| Reading & Literacy | Program Enrollment | Name, Credential, Completion Status | Teacher-focused |
| DELPI | Data Entry (3 cohorts) | Name, Admin Credential, IHE Partner | Cohort-based |

**Commonality:** All have participant name + credential type + demographics. Implement as universal `Participant` entity.

### Funding Pattern

| Template | Funding Model | Tabs | Match Required | Cap |
|----------|---------------|------|----------------|-----|
| Teacher Residency | Multi-year Grant + Match | 2 tabs (Grant, Match) | Yes, 1:1 ratio | Varies |
| School Counselor | Multi-year Grant + Match | 2 tabs (Grant, Match) | Yes, 1:1 ratio | Varies |
| Classified | Multi-year grid | 1 tab (Funding) | Sometimes | Varies |
| Integrated | Total grant lump sum | 1 tab (Budget) | Varies | Varies |
| Computer Science | Annual allocations | 1 tab (Funding) | No | $2,500 |
| Reading & Literacy | Annual allocations | 1 tab (Funding) | No | $2,500 |
| DELPI | Per-cohort, multi-year | Embedded in Data Entry | Sometimes | Varies |

**Commonality:** All track money by participant by year. Implement as `FundingLine` entity with grant/match fields.

### Progress/Service Tracking

| Template | Progress Tracking | Service Requirement | Years |
|----------|-------------------|---------------------|-------|
| Teacher Residency | Year 1-4 Teaching tabs | Yes | 4 years |
| School Counselor | Year 1-4 School Counseling tabs | Yes | 4 years |
| Classified | Program Progress | No (just completion) | 4 years |
| Integrated | Implicit (via Data Fields) | No | 5 years |
| Computer Science | Program Progress | No | 4 years |
| Reading & Literacy | Program Progress | No | 4 years |
| DELPI | Data Entry per cohort | No (admin pipeline) | 3 cohorts |

**Commonality:** Most track over 4 years. Residency grants explicitly require service. Implement as `ProgressRecord` + `ServiceRecord`.

---

## Conclusion

**The templates reveal a highly consistent "Program Lifecycle" pattern** with grant-specific variations in:
- Fields collected (JSON extensions)
- Funding structure (pack config)
- Service requirements (pack config)
- Survey questions (pack config)
- Reports/metrics (pack config)

**Recommendation:** Build a generic "Program Lifecycle Platform" with:
- Universal entities (Application, Participant, Funding, Progress, Completion, Service, Survey)
- Grant packs (schema.json, ui.json, rules.json, formulas.json, surveys.json, reports.json)
- Expression engine (JsonLogic/CEL) for validations and formulas
- UI renderer (Telerik) driven by pack ui.json
- Reporting engine (SQL/Jinja) driven by pack reports.json

This architecture enables CTC to add new grants in **days/weeks (configuration)** instead of **months (development)**.

---

**Next Steps:**
1. Lock STSP pack spec (schema, validations, formulas, surveys)
2. Build core platform entities and workflows
3. Implement JsonLogic/CEL engine with client/server parity
4. Build Telerik UI renderer
5. Hard-code STSP pack → extract to JSON → validate with second grant (April 2027)
