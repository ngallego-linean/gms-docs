# Updated Figma Flow Analysis - Version 2

**Date:** November 3, 2025 (4:22 PM)
**Source:** Data Flow Diagram (Community).png (updated)

---

## 🆕 What's New in This Version

### **1. Dashboard Specifications (Major Addition)**

A large yellow/tan note box has been added detailing **Dashboard metrics and KPIs**. This is critical new information that wasn't in the PDF version or our original analysis.

#### **Dashboard Info Requirements:**

```
Dashboard (info.)
- # of ST (Student Teachers)
- # of apps (various statuses)
- # of IHEs
- # of LEAs
- # of SEIDs
- Appropriated (money)
- Amount of money distributed
- Amount of money remaining
- Outstanding balances (balance?)
- If this weren't credential
- If they didn't earn credential
- If they didn't remain in credential area
- How many proceeded in the district (those LEA)
- Early withdrawal
- (data post submission)
```

### **Analysis of Dashboard Requirements:**

| Metric Category | Specific Metrics | Data Source | Implementation Notes |
|----------------|------------------|-------------|---------------------|
| **Volume Metrics** | # of Student Teachers, # of Apps (by status), # of IHEs, # of LEAs, # of SEIDs | Application data | Real-time counts, filterable by grant cycle |
| **Financial Tracking** | Appropriated amount, Distributed, Remaining, Outstanding balances | GrantCycle + Award entities | This confirms our fund tracking requirement. "Outstanding balances" may mean encumbered but not disbursed |
| **Completion Tracking** | If didn't earn credential, If didn't remain in credential area, Early withdrawal | Report data + ECS integration | Post-completion tracking. Requires IHE reports + potential ECS data warehouse queries |
| **Retention Metrics** | How many proceeded in the district (those LEA) | Report data or Data Warehouse | Employment retention in same LEA that hosted student teaching. Requires post-submission data tracking |

---

## 🔍 Key Observations

### **1. "Outstanding Balances" - New Term**
**Question:** What does this mean?
- Awards encumbered but not yet disbursed (GAA signed, payment not yet completed)?
- Or LEA hasn't paid student teacher yet?
- Or fiscal tracking of POs in FI$Cal pipeline?

**Recommendation:** Clarify with fiscal team. Likely means: `ReservedAmount + EncumberedAmount` (committed but not yet distributed)

---

### **2. Post-Credential Tracking Requirements**

Several metrics track **post-program outcomes:**
- "If didn't earn credential"
- "If didn't remain in credential area"
- "How many proceeded in the district"

**Implications:**
- This requires **ongoing tracking beyond award closure**
- Data may come from:
  - IHE reports (completion, credential earned)
  - **ECS Data Warehouse** (credential issuance, employment)
  - LEA self-reporting (hired student teacher after completion)

**Phase 2 Requirement:** Full integration with ECS data warehouse for employment outcomes

---

### **3. "Data Post Submission" Note**

The parenthetical "(data post submission)" at the bottom of the dashboard note suggests:
- Some metrics are only available **after reports are submitted**
- Dashboard may have two views:
  - **Real-time operational metrics** (apps, awards, funds)
  - **Outcome metrics** (completion rates, employment, retention) - updated after reporting periods

---

## 🆚 Changes from PDF Version

### **Additions:**
1. ✅ **Dashboard specifications** (entirely new)
2. Additional annotations/clarifications on existing flow elements (harder to read in PNG, would need higher resolution to confirm specifics)

### **Similarities:**
- Core flow structure unchanged (IHE → LEA → Grants Team → Fiscal Team)
- Decision points remain the same
- Swim lanes for different actors still present

### **What We Still Need:**
- Higher resolution image or Figma export to read smaller annotations
- Clarification on "outstanding balances" definition
- Confirmation on which dashboard metrics are V1 vs Phase 2

---

## 📊 Dashboard Implementation Plan

### **Phase 1 (V1 - April 2026) - Operational Dashboard**

**Staff Portal Dashboard:**

```
GRANT CYCLE: Teacher Recruitment Incentive Program - FY 2025-26

┌─────────────────────────────────────────────────────────┐
│ FINANCIAL TRACKING                                      │
├─────────────────────────────────────────────────────────┤
│ Appropriated:        $25,000,000                        │
│ Reserved:            $2,150,000  (86 pending GAAs)      │
│ Encumbered:          $8,500,000  (850 signed GAAs)      │
│ Disbursed:           $10,200,000 (1,020 payments sent)  │
│ Remaining:           $4,150,000  (16.6%)               │
│                                                          │
│ [Chart: Fund Depletion Over Time]                       │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ APPLICATION METRICS                                     │
├─────────────────────────────────────────────────────────┤
│ Total Applications:  2,156                              │
│  - Draft:              45                               │
│  - Submitted by IHE:   12                               │
│  - Submitted:          89 (awaiting review)             │
│  - Under Review:       15                               │
│  - Conditionally Approved: 8                            │
│  - Approved:          1,987                             │
│  - Rejected:           0                                │
│  - No Funding:         0                                │
│                                                          │
│ [Chart: Applications by Status]                         │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ PARTICIPANT METRICS                                     │
├─────────────────────────────────────────────────────────┤
│ Student Teachers (SEIDs):    2,156                      │
│ IHEs Participating:          87                         │
│ LEAs Participating:          456                        │
│                                                          │
│ Top IHEs by Application Volume:                         │
│  1. Cal State University, Fullerton: 156               │
│  2. UCLA: 142                                           │
│  3. San Diego State: 128                                │
│  ... [see all]                                          │
└─────────────────────────────────────────────────────────┘
```

**Implementation:**
- SQL aggregation queries on Application, Award, GrantCycle tables
- Real-time updates via SignalR when status changes
- Telerik Charts for visualizations
- Export to Excel for reporting

**Complexity:** MD (1-2 weeks) - queries are straightforward, chart integration with Telerik is well-documented

---

### **Phase 2 (Post-April 2026) - Outcomes Dashboard**

**Public Portal Dashboard (Accountability Metrics):**

```
TEACHER RECRUITMENT INCENTIVE PROGRAM - OUTCOMES

┌─────────────────────────────────────────────────────────┐
│ PROGRAM COMPLETION (FY 2024-25)                        │
├─────────────────────────────────────────────────────────┤
│ Total Participants:       1,850                         │
│ Completed Program:        1,702 (92%)                   │
│ Early Withdrawal:         148 (8%)                      │
│  - Changed to intern:       45                          │
│  - Withdrew from program:   78                          │
│  - Other:                   25                          │
│                                                          │
│ [Chart: Completion Rate Trend Over Years]              │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ CREDENTIAL OUTCOMES                                     │
├─────────────────────────────────────────────────────────┤
│ Earned Credential:        1,598 (94% of completers)    │
│ Did Not Earn Credential:   104 (6%)                     │
│                                                          │
│ Remained in Credential Area: 1,487 (93%)               │
│ Changed Credential Area:      111 (7%)                  │
│                                                          │
│ [Chart: Credential Area Distribution]                  │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ EMPLOYMENT & RETENTION                                  │
├─────────────────────────────────────────────────────────┤
│ Employed in Education:    1,523 (95% of credentialed)  │
│ Employed at Host LEA:      987 (65%)                    │
│ Employed at Different LEA: 536 (35%)                    │
│                                                          │
│ Retention Rate (1 year):  89%                           │
│ Retention Rate (2 years): 78%                           │
│                                                          │
│ [Chart: Employment by County]                           │
└─────────────────────────────────────────────────────────┘
```

**Implementation:**
- Requires **IHE/LEA report data** (completion, early exits)
- Requires **ECS Data Warehouse integration** (credential issuance, employment by SEID)
- Scheduled ETL jobs to pull employment data from ECS
- Anonymized, aggregated data only (no PII on public dashboard)

**Complexity:** LG (2-4 weeks) - requires external API integration, data warehouse queries, complex aggregation

---

## 🔧 Technical Implications

### **GrantCycle Entity Updates**

Our existing `GrantCycle` entity needs these additional tracked fields:

```csharp
public class GrantCycle
{
    // Existing fields...
    public decimal AppropriationAmount { get; set; }

    // NEW: Add these calculated/tracked properties
    public decimal ReservedAmount { get; set; }      // Awards approved, GAA not signed
    public decimal EncumberedAmount { get; set; }    // GAA signed, payment not completed
    public decimal DisbursedAmount { get; set; }     // Payment completed

    // Calculated property
    public decimal RemainingAmount => AppropriationAmount - (ReservedAmount + EncumberedAmount + DisbursedAmount);

    // NEW: For dashboard
    public decimal OutstandingBalance => ReservedAmount + EncumberedAmount; // Awaiting disbursement
}
```

### **Dashboard Controller / Service**

```csharp
public class DashboardService
{
    public async Task<GrantCycleDashboard> GetDashboardAsync(int grantCycleId)
    {
        var cycle = await _context.GrantCycles
            .Include(gc => gc.Applications)
            .Include(gc => gc.Awards)
            .FirstOrDefaultAsync(gc => gc.Id == grantCycleId);

        return new GrantCycleDashboard
        {
            // Financial
            Appropriated = cycle.AppropriationAmount,
            Reserved = cycle.ReservedAmount,
            Encumbered = cycle.EncumberedAmount,
            Disbursed = cycle.DisbursedAmount,
            Remaining = cycle.RemainingAmount,
            OutstandingBalance = cycle.OutstandingBalance,

            // Application counts
            TotalApplications = cycle.Applications.Count,
            ApplicationsByStatus = cycle.Applications.GroupBy(a => a.Status)
                .ToDictionary(g => g.Key, g => g.Count()),

            // Participant counts
            TotalStudentTeachers = cycle.Applications.Select(a => a.Seid).Distinct().Count(),
            TotalIHEs = cycle.Applications.Select(a => a.IheOrganizationId).Distinct().Count(),
            TotalLEAs = cycle.Applications.Select(a => a.LeaOrganizationId).Distinct().Count(),

            // Top IHEs by volume
            TopIHEs = cycle.Applications
                .GroupBy(a => a.IheOrganization)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new { Organization = g.Key.Name, Count = g.Count() })
                .ToList()
        };
    }

    // Phase 2: Outcomes dashboard
    public async Task<OutcomesDashboard> GetOutcomesDashboardAsync(int grantCycleId)
    {
        // Query reports + ECS data warehouse
        // Calculate completion rates, employment outcomes, retention
        // This is Phase 2 - requires report data and ECS integration
    }
}
```

### **Real-Time Updates**

Dashboard should update automatically when:
- Application status changes
- Award approved (Reserved amount increases)
- GAA signed (Reserved → Encumbered)
- Payment completed (Encumbered → Disbursed)

**Implementation:** SignalR hub that broadcasts dashboard updates to all connected staff users

```csharp
public class DashboardHub : Hub
{
    public async Task BroadcastDashboardUpdate(int grantCycleId)
    {
        var dashboard = await _dashboardService.GetDashboardAsync(grantCycleId);
        await Clients.Group($"GrantCycle_{grantCycleId}").SendAsync("DashboardUpdated", dashboard);
    }
}
```

---

## ✅ Updated V1 Requirements

### **Dashboard Features - V1 (REQUIRED)**

| Feature | Complexity | Priority | Notes |
|---------|-----------|----------|-------|
| **Financial Tracking** | SM | REQUIRED | Appropriated, Reserved, Encumbered, Disbursed, Remaining, Outstanding Balance |
| **Application Counts** | XS | REQUIRED | Count by status, total apps |
| **Participant Counts** | XS | REQUIRED | # of STs, IHEs, LEAs |
| **Real-Time Updates** | MD | OPTIONAL | SignalR for live updates. Can refresh on page load for V1 |
| **Top IHEs List** | XS | OPTIONAL | Nice-to-have, not critical |
| **Charts/Visualizations** | SM | REQUIRED | Telerik Charts for fund depletion, app status pie chart |
| **Export to Excel** | XS | OPTIONAL | Allow staff to download dashboard data |

### **Dashboard Features - Phase 2**

| Feature | Complexity | Priority | Notes |
|---------|-----------|----------|-------|
| **Completion Rate Tracking** | MD | Phase 2 | Requires IHE reports |
| **Credential Outcomes** | LG | Phase 2 | Requires ECS data warehouse integration |
| **Employment Tracking** | LG | Phase 2 | Requires ECS employment data |
| **Retention Metrics** | LG | Phase 2 | Multi-year tracking |
| **Public Dashboard** | MD | Phase 2 | Anonymized, aggregated outcomes for public transparency |

---

## 🎯 Key Questions for CTC

### **1. Outstanding Balance Definition**
**Q:** Does "Outstanding balance" mean:
- A) Reserved + Encumbered (awards committed but not yet disbursed)?
- B) Only Encumbered (GAA signed, awaiting payment)?
- C) Something else?

**Recommendation:** Use definition A (Reserved + Encumbered) to show total committed funds in pipeline.

---

### **2. Post-Completion Tracking Timeline**
**Q:** When are outcome metrics (credential earned, employment, retention) measured?
- A) 1 year after program completion?
- B) 2 years?
- C) Ongoing (updated annually)?

**Recommendation:** Define reporting periods in grant program configuration. Example: "Annual employment check 12 months post-completion."

---

### **3. Data Source for Employment Outcomes**
**Q:** Where does "employed at host LEA" data come from?
- A) LEA self-reports in final report
- B) ECS data warehouse (employment records by SEID)
- C) CDE data (teacher assignment data)

**Recommendation:** Phase 1: LEA self-report. Phase 2: Validate against ECS/CDE data warehouse.

---

### **4. Public Dashboard Scope**
**Q:** Which metrics should be public vs. staff-only?
- Public: Aggregate outcomes (completion rates, employment trends) - no PII
- Staff-only: Financial tracking, individual application details, specific organizations

**Recommendation:** Separate dashboards. Staff Portal has operational + outcomes. Public Portal has outcomes only (anonymized, aggregated).

---

## 📋 Next Steps

1. ✅ **Confirm dashboard requirements** with Cara/Sara/Eddie
2. ✅ **Clarify "outstanding balance"** definition
3. ✅ **Add GrantCycle fund tracking fields** to domain model
4. ✅ **Design dashboard UI mockups** with Telerik components
5. ✅ **Implement V1 operational dashboard** (financial + application metrics)
6. ⏳ **Defer outcomes dashboard to Phase 2** (post-reporting, requires ECS integration)
7. ⏳ **Schedule ECS data warehouse integration discovery** for Phase 2 planning

---

## 📄 Document Summary

**What Changed:**
- Dashboard specifications added (major new requirement)
- Financial tracking clarified with "outstanding balance" term
- Post-completion outcome metrics identified (Phase 2)

**What Stayed the Same:**
- Core workflow (IHE → LEA → Review → Award → Payment → Reports)
- State machines remain valid
- V1 scope mostly unchanged (dashboard is addition, not replacement)

**Impact on Timeline:**
- Dashboard implementation adds **1-2 weeks** to V1 (medium complexity)
- Adjusted V1 timeline: **19-24 weeks** (was 18-22 weeks)
- Phase 2 outcomes dashboard requires ECS integration (separately scoped)

---

**Version:** 2.1 (Post-Updated Figma)
**Date:** November 3, 2025
**Next Review:** Dashboard mockup review with stakeholders
