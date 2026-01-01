# GMS Analytics & Reporting Redesign Specification

**Created:** December 31, 2024
**Status:** Planning
**Development Start:** Week of 12/22

---

## Stakeholder Feedback (Verbatim)

### I. Reporting
> a. Need ability to generate reports that include both narration and tables, with a focus on ADA compliance (no charts/graphs in formal reports, but visuals are welcome for internal or public dashboards).
>
> b. Option to export reports as PDF or Excel for further manipulation and inclusion in formal documents.
>
> c. Quick access reports are helpful for frequently requested data, such as demographics, credential area breakdowns, and regional distribution.
>
> d. Remove individual student lists from reports when the numbers are large; prefer summary statistics and breakdowns (e.g., number of elementary, middle, high, and charter schools).

### II. Analytics
> a. Emphasis on racial/ethnic breakdowns and other demographic details, as these are common stakeholder questions.
>
> b. Need ability to filter and view data by program (e.g., teacher residency, school counselor residency) and by fiscal year, with toggles for overall vs. year-specific views.
>
> c. Compliance status cards that allow clicking to see which LEAs are partially compliant or not complete, rather than listing all LEAs by default
>    - Choose different language; "compliance" is not the most accurate
>
> d. Financial summary improvements, including monthly disbursement trends and the ability to select programs and fiscal years via dropdowns.
>
> e. Add management analytics, such as average processing times from submission to approval/disbursement, to identify bottlenecks and support staffing/resource requests.
>
> f. Add search functionality to find data by candidate, LEA, IHE, or other filters (e.g., credential area, region), with the ability to generate custom reports for specific queries.
>
> g. Separate search and reporting for candidates, LEAs, and IHEs, as their roles and data needs differ.
>
> h. Create a public-facing analytics/dashboard for high-level, quick-hit statistics to reduce public information requests.

### III. Administrative
> a. The bulk of discovery has concluded and the Linean team will begin development the week of 12/22.
>
> b. Remaining prototype feedback will be a hybrid process, with both links for individual review and group meetings to discuss and refine the prototype.

### IV. Actions
> a. Amarjot to share fields/needs for management analytics
>
> b. Sara Saelee and Cara to review revised Fiscal function of GMS and GAA/Invoice templates
>
> c. Cara to share example of grant dashboard

---

## Reference: WestEd Tableau Dashboard

**URL:** https://public.tableau.com/app/profile/westedimprovementscience/viz/CaliforniaTeacherResidencyGrantProgramDashboard/TeacherResidencyPipeline

### Key Visual Patterns to Incorporate

1. **Pipeline Visualization** - Enrollment → Completion → Hiring flow
2. **Year-over-Year Trends** - Cohort comparisons (2020-21 through 2024-25)
3. **Demographics Breakdowns** - Race/Ethnicity, Gender, Credential Program stacked bars
4. **Non-Completion Analysis** - Reasons for attrition
5. **Interactive Filtering** - Cohort year selection, program filters
6. **Percentage Displays** - Rates shown on visualizations

---

## Module 1: Global Filters

**Priority:** High
**Feedback Reference:** II.b

### Requirements

Add a persistent filter bar at the top of Analytics pages:

```
┌────────────────────────────────────────────────────────────────┐
│  Program: [All Programs ▼]  Fiscal Year: [2024-25 ▼]  [Overall | By Year] │
└────────────────────────────────────────────────────────────────┘
```

### Filter Options

**Program Dropdown:**
- All Programs
- Teacher Residency (TRP)
- School Counselor Residency
- Classified to Certificated
- STSP (if applicable)

**Fiscal Year Dropdown:**
- All Years
- 2024-25
- 2023-24
- 2022-23
- 2021-22
- 2020-21

**View Toggle:**
- Overall (aggregate across selected years)
- By Year (show year-over-year comparison)

### Implementation Notes

- Filters should persist across page navigation within Analytics
- URL parameters for shareable filtered views
- Default: Current fiscal year, All Programs, Overall view

---

## Module 2: Reporting Status Cards (Renamed from Compliance)

**Priority:** High
**Feedback Reference:** II.c
**Status:** IMPLEMENTED (December 31, 2024)

### Implementation Summary

- Created new unified `/Reporting/Status` page with LEA and IHE tabs
- Added clickable status cards (Complete, In Progress, Not Started)
- Implemented JavaScript-powered drill-down tables
- Old `/Reporting/Compliance` route now redirects to `/Reporting/Status?tab=LEA`
- Added fiscal year filter dropdown

### Files Changed

- `Ctc.GMS.AspNetCore/ViewModels/ReportViewModel.cs` - Added new ViewModels
- `Ctc.GMS.Web.UI/Controllers/ReportingController.cs` - Added Status action, redirect for Compliance
- `Ctc.GMS.Web.UI/Views/Reporting/Status.cshtml` - New consolidated view

### Requirements

Replace "Compliance" terminology and change from list view to clickable summary cards.

### Current State

- Called "Compliance Dashboard"
- Lists all LEAs by default
- Language implies regulatory enforcement

### New Design

**Rename to:** "Reporting Status" or "Submission Status"

```
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│    COMPLETE     │  │   IN PROGRESS   │  │   NOT STARTED   │
│                 │  │                 │  │                 │
│       47        │  │       12        │  │        3        │
│      LEAs       │  │      LEAs       │  │      LEAs       │
│                 │  │                 │  │                 │
│  [View List →]  │  │  [View List →]  │  │  [View List →]  │
└─────────────────┘  └─────────────────┘  └─────────────────┘
```

### Interaction

- Cards are clickable
- Click expands to show LEA list for that status only
- Do NOT show all LEAs by default
- Include count and percentage on each card

### Status Definitions

| Status | Definition |
|--------|------------|
| Complete | All required reports submitted and approved |
| In Progress | Some reports submitted, others pending |
| Not Started | No reports submitted for current reporting period |

---

## Module 3: Demographics Section

**Priority:** High
**Feedback Reference:** II.a, Tableau reference

### Requirements

Add comprehensive demographics visualizations as this is a common stakeholder question.

### Components

#### 3A. Race/Ethnicity Breakdown

**Visualization:** Stacked horizontal bar chart (by year if "By Year" toggle active)

**Categories:**
- American Indian/Alaska Native
- Asian
- Black/African American
- Hispanic/Latino
- Native Hawaiian/Pacific Islander
- White
- Two or More Races
- Unknown/Not Reported

**Display:** Count and percentage for each category

#### 3B. Gender Breakdown

**Visualization:** Horizontal bar or donut chart

**Categories:**
- Female
- Male
- Non-Binary
- Not Reported

#### 3C. Credential Area Breakdown

**Visualization:** Horizontal bar chart

**Categories:**
- Multiple Subject
- Single Subject (with subject area sub-breakdown)
- Education Specialist
- School Counselor
- Other

### ADA Compliance Note

- Charts are for internal/public dashboards only
- Formal reports use tables (see Module 7)

---

## Module 4: Financial Analytics Enhancements

**Priority:** High
**Status:** COMPLETED (2024-12-31)
**Feedback Reference:** II.d

### Requirements

Enhance financial summary with trends and filtering.

### Components

#### 4A. Monthly Disbursement Trend

**Visualization:** Line chart or bar chart showing disbursements by month

```
Monthly Disbursements (FY 2024-25)
│
│    ████
│    ████  ████
│    ████  ████  ████
│    ████  ████  ████  ████
└────Jul───Aug───Sep───Oct───Nov───Dec───Jan───Feb───Mar───Apr───May───Jun
```

**Features:**
- Filterable by program (uses global filter)
- Filterable by fiscal year (uses global filter)
- Hover for exact amounts
- Cumulative total line overlay option

#### 4B. Enhanced Summary Cards

**Keep existing:**
- Total Disbursed
- Average Payment

**Add:**
- Disbursements This Month
- Year-over-Year Change (%)
- By Program breakdown table

---

## Module 5: Processing Efficiency Report (Quick Access)

**Priority:** High
**Feedback Reference:** II.e
**Status:** APPROVED (Dec 31, 2024)

### Implementation

Add as a Quick Access Report on the Reporting Dashboard.

**Route:** `/Reporting/ProcessingEfficiencyReport`
**Question:** "How long does it take to get funds to grantees?"

### Report Sections

#### 5A. Report Metadata Bar
- Grant Cycle, Program, As of Date (same as other reports)

#### 5B. Current Pipeline Status
Visual cards showing queue depth at each stage:
- Awaiting GAA (count + amount)
- Awaiting PO (count + amount)
- Awaiting Invoice (count + amount)
- Awaiting Accounting (count + amount)
- Awaiting Warrant (count + amount)
- Total in Pipeline summary

#### 5C. Average Processing Time by Stage
Table with columns: Stage Transition | Target | Avg Time | Variance | Status

| Stage Transition | Target |
|------------------|--------|
| Submission → GAA Signed | 5 days |
| GAA Signed → PO Generated | 3 days |
| PO Generated → PO Uploaded | 3 days |
| PO Uploaded → Invoice Gen | 2 days |
| Invoice → Sent to Acctg | 1 day |
| Sent to Acctg → Warrant | 5 days |
| **End-to-End Total** | **19 days** |

Status indicators: ✓ On Track / ⚠ Over

#### 5D. Bottleneck Analysis
For stages exceeding target, show:
- Average vs Target vs Variance (days and %)
- Trend vs Last Month (↑ Worsening / → Stable / ↓ Improving)
- Items waiting beyond target count
- Root Cause notes field

#### 5E. Throughput Summary
Metric cards:
- Completed This Month (count + amount)
- Completed This FY (count + amount)
- Avg Weekly Throughput

Monthly Completions table: Month | Count | Amount | Avg Days

#### 5F. Year-over-Year Comparison
Table comparing FY 2023-24 vs FY 2024-25:
- Avg End-to-End Time
- Total Disbursements
- Total Amount
- Items Exceeding Target %

#### 5G. Aging Items Requiring Attention
Table sorted by days waiting (descending): LEA | Stage | Days | Amount
- ⚠ indicator for items exceeding SLA

### Files to Create/Modify

1. `Views/Reporting/Dashboard.cshtml` - Add Quick Access card
2. `Views/Reporting/ProcessingEfficiencyReport.cshtml` - New report view
3. `Controllers/ReportingController.cs` - Add action method

### Notes

- **No Executive Summary section** (per stakeholder feedback)
- All sections use tables for ADA compliance
- Export options: Print, PDF, Excel

---

## Module 6: Search Functionality

**Priority:** Medium
**Feedback Reference:** II.f, II.g
**Status:** IMPLEMENTED (December 31, 2024)

### Requirements

Add search functionality with separate pages for different entity types.

### Pages

#### 6A. Candidate Search (`/Search/Candidates`)

**Search Fields:**
- Name
- SEID
- Credential Area
- Region/County
- LEA
- IHE
- Program
- Cohort Year
- Status

**Results Display:**
- Summary count
- Paginated table (no individual PII in large lists)
- Export to Excel option
- "Generate Report" button for filtered results

#### 6B. LEA Search (`/Search/LEAs`)

**Search Fields:**
- LEA Name
- County
- Region
- Program Participation
- Reporting Status
- Candidate Count Range

**Results Display:**
- LEA summary cards or table
- Click to view LEA detail page
- Export option

#### 6C. IHE Search (`/Search/IHEs`)

**Search Fields:**
- IHE Name
- Region
- Program Partnerships
- Candidate Count Range

**Results Display:**
- IHE summary cards or table
- Click to view IHE detail page
- Export option

### Custom Report Generation

- From any search results, allow "Generate Report"
- Report includes only filtered/selected records
- Follows ADA compliance rules (tables, no charts)

---

## Module 7: Report Export & ADA Compliance

**Priority:** High
**Feedback Reference:** I.a, I.b, I.d

### Requirements

Ensure all formal reports are ADA compliant and exportable.

### ADA Compliance Rules

| Element | Formal Reports | Internal Dashboards | Public Dashboards |
|---------|---------------|---------------------|-------------------|
| Charts/Graphs | NO | YES | YES |
| Data Tables | YES | YES | YES |
| Narrative Text | YES | Optional | Optional |
| Individual Student Lists | NO (use summaries) | Limited | NO |

### Export Options

Add to all report pages:

```
┌─────────────────────────────────────┐
│  Export: [PDF ▼] [Excel ▼] [Print]  │
└─────────────────────────────────────┘
```

**PDF Export:**
- Formatted for print
- Includes header with report title, date, filters applied
- Page numbers
- ADA compliant (no charts)

**Excel Export:**
- Raw data tables
- Multiple sheets if multiple tables
- Filterable columns
- For further manipulation

### Summary Statistics Format

Instead of listing individual students, use breakdowns:

```
Candidate Distribution by School Type
┌─────────────────────┬───────┬────────┐
│ School Type         │ Count │ %      │
├─────────────────────┼───────┼────────┤
│ Elementary Schools  │ 127   │ 34.0%  │
│ Middle Schools      │ 89    │ 24.0%  │
│ High Schools        │ 112   │ 30.0%  │
│ Charter Schools     │ 47    │ 12.0%  │
├─────────────────────┼───────┼────────┤
│ Total               │ 375   │ 100.0% │
└─────────────────────┴───────┴────────┘
```

---

## Module 8: Pipeline Funnel Visualization

**Priority:** Medium
**Feedback Reference:** Tableau reference

### Requirements

Add visual pipeline showing candidate progression through stages.

### Design

```
CANDIDATE PIPELINE (2024-25 Cohort)
┌─────────────────────────────────────────────────────────────────┐
│ ████████████████████████████████████████████████████  Enrolled  │
│ 1,110 candidates                                       (100%)   │
├─────────────────────────────────────────────────────────────────┤
│ ████████████████████████████████████████████          Completed │
│ 892 candidates                                         (80.4%)  │
├─────────────────────────────────────────────────────────────────┤
│ ████████████████████████████████████                Credentialed│
│ 847 candidates                                         (76.3%)  │
├─────────────────────────────────────────────────────────────────┤
│ ██████████████████████████████                        Employed  │
│ 756 candidates                                         (68.1%)  │
├─────────────────────────────────────────────────────────────────┤
│ ██████████████████████████                    Retained (1 Year) │
│ 642 candidates                                         (57.8%)  │
└─────────────────────────────────────────────────────────────────┘
```

### Features

- Click each stage to see breakdown
- Shows drop-off between stages
- Filterable by program and fiscal year
- Year-over-year comparison option

---

## Module 9: Public-Facing Dashboard

**Priority:** Low (Scope TBD)
**Feedback Reference:** II.h

### Requirements

Create a public dashboard to reduce public information requests.

### Content (High-Level Only)

- Total candidates funded (by year)
- Credential completion rates (aggregate)
- Employment rates (aggregate)
- Regional distribution (map or table)
- Demographic summary (aggregated, no small cell sizes)
- Program overview

### Restrictions

- NO personally identifiable information (PII)
- NO individual student records
- NO data that could identify individuals (small cell suppression)
- Aggregate statistics only

### Access

- Public URL (no login required)
- Separate from internal GMS system
- Consider: Embedded Tableau or standalone page

### Pending Action

> Cara to share example of grant dashboard

---

## Module 10: Quick Access Reports

**Priority:** Medium
**Feedback Reference:** I.c

### Requirements

Maintain and enhance quick access reports for frequently requested data.

### Current Reports

1. Demographics Report - Candidate Pipeline Analysis
2. Financial Summary Report - Investment Efficiency
3. LEA Performance Report - Partnership Outcomes
4. Program Outcomes Report - Impact metrics

### Enhancements

**Add:**
- Credential Area Breakdown Report
- Regional Distribution Report
- Year-over-Year Comparison Report

**For Each Report:**
- Add PDF/Excel export buttons
- Add filter controls (program, fiscal year)
- Ensure ADA compliance (tables, no charts)
- Include narrative sections where appropriate

---

## Implementation Phases

### Phase 1: High Priority (Start Week of 12/22)

| Module | Description | Effort | Status |
|--------|-------------|--------|--------|
| 1 | Global Filters | Medium | DONE (existing) |
| 2 | Reporting Status Cards (rename from Compliance) | Low | DONE (existing) |
| 3 | Demographics Section | High | DONE (existing) |
| 4 | Financial Analytics Enhancements | High | COMPLETED |
| 7 | Report Export & ADA Compliance | Medium | Pending |

### Phase 2: Medium Priority

| Module | Description | Effort | Status |
|--------|-------------|--------|--------|
| 6 | Search Functionality | High | Pending |
| 8 | Pipeline Funnel Visualization | Medium | Pending |
| 10 | Quick Access Reports Enhancements | Low | Pending |

### Phase 3: Pending Input / Lower Priority

| Module | Description | Blocker |
|--------|-------------|---------|
| 5 | Management Analytics | Waiting on Amarjot |
| 9 | Public-Facing Dashboard | Waiting on Cara's example |

---

## Technical Considerations

### Charting Library

For internal dashboards, consider:
- **Chart.js** - Lightweight, good for basic charts
- **ApexCharts** - More features, good interactivity
- **D3.js** - Maximum flexibility, higher complexity

Recommendation: Chart.js for simplicity and R4 compatibility

### Data Architecture

- Ensure ViewModel supports all required aggregations
- Add caching for expensive queries
- Consider pre-computed summary tables for performance

### R4 Framework Compliance

All implementations must follow R4 guidelines:
- Use R4 component classes
- Follow R4 color palette
- Maintain accessibility standards
- Responsive design

---

## Open Questions

1. What specific fields does Amarjot need for management analytics?
2. What does Cara's example grant dashboard look like?
3. Should public dashboard be embedded Tableau or custom-built?
4. What are the SLA targets for processing time bottleneck indicators?
5. Small cell suppression threshold for public data (typically n<10)?

---

## Appendix: File Locations

### Current Analytics Implementation

- **Controller:** `Ctc.GMS/Ctc.GMS.Web.UI/Controllers/ReportingController.cs`
- **View:** `Ctc.GMS/Ctc.GMS.Web.UI/Views/Reporting/Analytics.cshtml`
- **ViewModel:** `Ctc.GMS/Ctc.GMS.Web.UI/Models/ReportAnalyticsViewModel.cs`

### Current Report Views

- `Views/Reporting/Dashboard.cshtml`
- `Views/Reporting/DemographicsReport.cshtml`
- `Views/Reporting/FinancialSummaryReport.cshtml`
- `Views/Reporting/LEAPerformanceReport.cshtml`
- `Views/Reporting/ProgramOutcomesReport.cshtml`

### Search Implementation (To Be Created)

- `Views/Search/Index.cshtml` (exists - needs enhancement)
- `Views/Search/Candidates.cshtml` (new)
- `Views/Search/LEAs.cshtml` (new)
- `Views/Search/IHEs.cshtml` (new)
