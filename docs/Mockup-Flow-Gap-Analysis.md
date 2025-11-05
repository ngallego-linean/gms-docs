# Mockup vs. Figma Flow Gap Analysis

**Date:** November 5, 2025
**Purpose:** Evaluate current mockup against documented Figma flows to identify gaps and prioritize improvements

---

## Executive Summary

The current mockup **successfully demonstrates** the core batch student management model and financial tracking dashboard. However, it is **missing 3 critical workflows** from the Figma flows:

🔴 **CRITICAL GAPS:**
1. **Magic Link Grantee Data Collection** - LEA collecting demographics directly from students
2. **Reporting Flow** - IHE/LEA post-award reporting
3. **Public Portal** - Grant listings and awareness (marketing campaign entry point)

🟡 **MODERATE GAPS:**
4. **Application Revision Workflow** - Staff rejecting and LEA fixing issues
5. **Intent to Apply** - Optional pre-application step
6. **Multi-LEA Split Payment** - Student teaching at multiple LEAs

🟢 **WELL REPRESENTED:**
- ✅ Batch student management (one app per IHE-LEA pair)
- ✅ Real-time dashboard with fund tracking
- ✅ Draft states for applications
- ✅ Three-portal architecture (Staff, IHE, LEA)
- ✅ GAA & DocuSign workflow simulation
- ✅ Payment lifecycle (Reserved → Encumbered → Disbursed)
- ✅ Fund depletion warnings

---

## Detailed Gap Analysis

### 🔴 CRITICAL GAP #1: Magic Link Grantee Data Collection

#### What Figma Shows:
**Documented requirement:** After LEA completes their portion, LEA sends tokenized links to individual student teachers to collect demographics/grant-specific data directly from grantees.

**Where it fits in flow:**
```
IHE submits student → LEA completes fiscal info →
[MISSING: LEA sends magic links to students] →
[MISSING: Students fill out demographics form] →
[MISSING: LEA sees completion dashboard] →
Application marked "Fully Ready" → Submitted to CTC
```

#### What Mockup Shows:
- IHE enters basic student info (name, DOB, SEID, credential area)
- LEA enters fiscal info
- **No mechanism for LEA to send links to students**
- **No student-facing form/portal**
- **No completion tracking dashboard**

#### Impact:
**HIGH** - This is a V1 REQUIRED feature per Figma analysis. Major efficiency gain (LEA doesn't manually gather data from 10-50 candidates).

#### Recommended Mockup Additions:

**1. LEA Applications View - Add "Send Data Collection Links" button**
```
┌──────────────────────────────────────────────────┐
│ Student: Maria Garcia                             │
│ Status: LEA Info Completed                        │
│                                                    │
│ [Complete LEA Info] [Send Data Collection Link]  │
└──────────────────────────────────────────────────┘
```

**2. New View: LEA Data Collection Dashboard**
```
┌──────────────────────────────────────────────────┐
│ Data Collection Status: 8/10 students completed  │
├──────────────────────────────────────────────────┤
│ ✅ Maria Garcia - Completed Nov 3, 2025          │
│ ✅ James Chen - Completed Nov 2, 2025            │
│ ⏳ Sarah Johnson - Link sent, not completed      │
│ ⏳ Michael Brown - Link not sent yet             │
│                                                    │
│ [Send All Remaining Links]  [Submit to CTC]      │
└──────────────────────────────────────────────────┘
```

**3. New Portal: Student Data Collection (Magic Link)**
```
┌──────────────────────────────────────────────────┐
│ STIPEND Grant - Student Teacher Information      │
├──────────────────────────────────────────────────┤
│ Hello Maria Garcia,                               │
│ Your LEA has requested additional information    │
│                                                    │
│ Ethnicity: [Dropdown]                             │
│ Gender Identity: [Dropdown]                       │
│ Sexual Orientation: [Optional dropdown]           │
│ Credential Program: [Pre-filled: Multiple Subject]│
│ Expected Completion Date: [Date picker]          │
│                                                    │
│ [Submit Information]                              │
└──────────────────────────────────────────────────┘
```

---

### 🔴 CRITICAL GAP #2: Reporting Flow

#### What Figma Shows:
```
IHE/LEA are alerted to fill out reports

IHE submits:
- Completion confirmation
- Employment area

LEA submits:
- How they categorized ST for payment
- Payment schedule and amount
- Payment date

→ Grants team is pinged → Reports available
→ Grants Team Receives Reports
```

#### What Mockup Shows:
**Nothing.** No reporting workflow at all.

#### Impact:
**HIGH** - Reporting is the closure step for awards. Without it, the lifecycle is incomplete.

#### Recommended Mockup Additions:

**1. Staff Dashboard - Add "Reports Due" Action Card**
```
┌──────────────────────────────────────────────────┐
│ 🔔 REPORTS DUE                                   │
├──────────────────────────────────────────────────┤
│ • 45 IHE completion reports due by July 31       │
│ • 38 LEA payment reports due by August 15        │
│                                                    │
│ [View Reports]                                    │
└──────────────────────────────────────────────────┘
```

**2. IHE Portal - Add "Reports" Tab**
```
┌──────────────────────────────────────────────────┐
│ Reports for FY 2024-25 Cohort                    │
├──────────────────────────────────────────────────┤
│ Student: Maria Garcia                             │
│ Status: Report Due (by July 31, 2025)           │
│                                                    │
│ Did student complete program? [Yes/No]           │
│ If yes, credential area: [Multiple Subject]      │
│ If no, reason: [Dropdown: Early exit, etc.]     │
│ Employment status: [Dropdown: Employed in edu]   │
│ Employment LEA: [Dropdown: LAUSD]                │
│                                                    │
│ [Save Draft]  [Submit Report]                    │
└──────────────────────────────────────────────────┘
```

**3. LEA Portal - Add "Reports" Tab**
```
┌──────────────────────────────────────────────────┐
│ Payment Reports for FY 2024-25 Cohort            │
├──────────────────────────────────────────────────┤
│ Student: Maria Garcia                             │
│ Award Amount: $10,000                             │
│                                                    │
│ How was ST categorized? [Dropdown: Classified]   │
│ Payment schedule: [One payment]                   │
│ Payment amount: [$10,000]                         │
│ Payment date: [Date picker]                       │
│ Did ST complete 540+ hours? [Yes/No]             │
│                                                    │
│ [Save Draft]  [Submit Report]                    │
└──────────────────────────────────────────────────┘
```

**4. Staff Portal - Add "Reports" View**
```
┌──────────────────────────────────────────────────┐
│ Reports Overview                                  │
├──────────────────────────────────────────────────┤
│ IHE Reports: 142/187 submitted (76%)             │
│ LEA Reports: 158/187 submitted (85%)             │
│                                                    │
│ [Filter: Overdue] [Export to Excel]             │
│                                                    │
│ Student        IHE Report  LEA Report  Status    │
│ ─────────────────────────────────────────────────│
│ Maria Garcia   ✅ Nov 3    ✅ Nov 5     Complete │
│ James Chen     ✅ Oct 28   ⏳ Pending  Incomplete│
│ Sarah Johnson  ⏳ Overdue  ⏳ Pending  Overdue   │
└──────────────────────────────────────────────────┘
```

---

### 🔴 CRITICAL GAP #3: Public Portal

#### What Figma Shows:
```
Marketing Campaign → IHE clicks link → Signs in to portal → Starts application
```

The "Marketing Campaign" is the entry point, implying a **public-facing portal** with grant information.

#### What Mockup Shows:
- Starts directly at IHE Portal
- No public portal or marketing materials
- No grant listing/search
- No RFA information

#### Impact:
**HIGH** - Public awareness is critical for grant uptake. Without it, potential applicants don't know about the grant.

#### Recommended Mockup Additions:

**1. New Portal: Public Portal (No Login Required)**
```
┌──────────────────────────────────────────────────┐
│ California Commission on Teacher Credentialing    │
│ Grant Management System                           │
├──────────────────────────────────────────────────┤
│ AVAILABLE GRANTS                                  │
│                                                    │
│ ┌────────────────────────────────────────────┐  │
│ │ 🎓 Teacher Recruitment Incentive (STIPEND) │  │
│ │ Award: $10,000 per student teacher         │  │
│ │ Status: Applications Open                   │  │
│ │ Deadline: Rolling (funds available)         │  │
│ │                                             │  │
│ │ [View Details]  [Apply Now]                │  │
│ └────────────────────────────────────────────┘  │
│                                                    │
│ ┌────────────────────────────────────────────┐  │
│ │ 🏫 California Teacher Residency Grant      │  │
│ │ Award: $20,000 per resident                │  │
│ │ Status: Applications Closed                │  │
│ │ Next Cycle: March 2026                     │  │
│ │                                             │  │
│ │ [View Details]                             │  │
│ └────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────┘
```

**2. Grant Detail Page**
```
┌──────────────────────────────────────────────────┐
│ Teacher Recruitment Incentive Grant (STIPEND)    │
├──────────────────────────────────────────────────┤
│ OVERVIEW                                          │
│ The STIPEND grant provides $10,000 to student    │
│ teachers completing 540+ hours of student teaching│
│                                                    │
│ ELIGIBILITY                                       │
│ • Student teacher enrolled in credential program │
│ • Completing student teaching at qualifying LEA  │
│ • 540+ hours of student teaching required        │
│                                                    │
│ FUNDING AVAILABLE                                 │
│ $4.15M remaining (16.6% of $25M appropriation)   │
│                                                    │
│ APPLICATION PROCESS                               │
│ 1. IHE submits student information               │
│ 2. LEA completes fiscal details                  │
│ 3. CTC reviews and approves                      │
│ 4. GAA signed by all parties                     │
│ 5. Payment processed via FI$Cal                  │
│                                                    │
│ [Download RFA] [Apply Now - IHE Login]           │
└──────────────────────────────────────────────────┘
```

---

## 🟡 MODERATE GAPS

### 4. Application Revision Workflow

**Figma shows:**
```
If "Not Approved; Bad Info":
→ LEA fixes application issues (loop back)
```

**Mockup shows:**
- Staff can approve/reject students
- But no mechanism for LEA to edit after rejection
- No comments/feedback from staff to LEA

**Recommended Addition:**
- When staff rejects, show modal with rejection reason
- Change student status to "NEEDS_REVISION"
- LEA portal shows students needing revision with staff comments
- LEA can edit and re-submit

---

### 5. Intent to Apply (Optional)

**Figma mentions** but marks as optional. Not critical for V1.

**Recommended:** Defer to Phase 2. Focus on core application workflow first.

---

### 6. Multi-LEA Split Payment

**Figma shows:** Checkbox for "more than one LEA placement"

**Mockup shows:** Single LEA per application

**Recommended Addition:**
- When IHE adds student, allow checkbox "Student teaching at multiple LEAs"
- If checked, show "+ Add Another LEA" button
- Award amount automatically splits: $10k / # of LEAs
- Each LEA completes their portion separately

---

## 🟢 What the Mockup Does Well

### Excellent Representations of Figma Flows:

1. **Batch Student Management Model** ✅
   - One application per IHE-LEA pair (matches Figma requirement)
   - Students can be added incrementally throughout year
   - Each student has individual status tracking

2. **Real-Time Financial Dashboard** ✅
   - Appropriated, Reserved, Encumbered, Disbursed, Remaining
   - Outstanding Balance calculated correctly
   - Fund depletion warnings at <10%
   - Color-coded status (green/orange/red)
   - Progress bars for remaining funds

3. **Three-Portal Architecture** ✅
   - Staff Portal: Review, approve, GAA, payments
   - IHE Portal: Add students, manage applications
   - LEA Portal: Complete fiscal info, submit to CTC

4. **Application Lifecycle** ✅
   - Draft → Pending LEA → Submitted → Under Review → Approved/Rejected
   - Clear status badges and state transitions
   - Toast notifications for actions

5. **GAA & DocuSign Workflow** ✅
   - 3-party signature simulation (LEA → Cara → Sara)
   - Signature tracking with timestamps
   - Award status updates: GAA Sent → GAA Signed

6. **Payment Lifecycle** ✅
   - Fund state transitions: Remaining → Reserved → Encumbered → Disbursed
   - Visual representation of fund movement
   - Dashboard metrics update in real-time

7. **Notification System** ✅
   - Portal-specific notifications
   - Badge counts
   - Click-through to relevant items

---

## Priority Improvements

### 🎯 Phase 1 - Critical for Client Demo (1-2 weeks)

Focus on showing the **complete user journey** from the Figma flows:

**PRIORITY 1: Reporting Flow (3-4 days)**
- Add "Reports" tab to IHE/LEA/Staff portals
- Simple forms for IHE completion and LEA payment reports
- Staff view of report submission status
- **Rationale:** This completes the end-to-end workflow. Without it, the lifecycle stops at payment.

**PRIORITY 2: Magic Link Workflow (3-4 days)**
- Add "Send Data Collection Link" button in LEA portal
- Create simple "Magic Link" form (no login, tokenized URL)
- Add data collection status dashboard for LEA
- **Rationale:** This is a V1 REQUIRED feature per Figma analysis. Shows key efficiency gain.

**PRIORITY 3: Public Portal (2-3 days)**
- Create public home page with grant listings
- Grant detail page with eligibility, process, funding available
- "Apply Now" button that goes to IHE login
- **Rationale:** Shows the marketing campaign entry point from Figma flows.

### 🎯 Phase 2 - Enhancements (1 week)

**PRIORITY 4: Application Revision Workflow (2-3 days)**
- Add rejection reason modal
- Add "NEEDS_REVISION" status
- Allow LEA to edit and re-submit

**PRIORITY 5: Multi-LEA Split Payment (2-3 days)**
- Add "Multiple LEAs" checkbox
- Split award amount automatically
- Show multiple LEA portions in student detail

**PRIORITY 6: Intent to Apply (1-2 days)**
- Simple form before full application
- Optional feature for volume forecasting

---

## Questions for You

Before I start implementing improvements, I need your input on priorities:

### Question 1: Which flows are most important to demonstrate?
The Figma docs identify 6 main workflows. Which are **MUST HAVE** for the client demo?
- [ ] IHE Application Flow (already in mockup)
- [ ] LEA Completion Flow (already in mockup)
- [ ] Application Review & Approval (already in mockup)
- [ ] GAA & DocuSign Workflow (already in mockup)
- [ ] Payment Process (already in mockup)
- [ ] **Reporting Flow (MISSING)**
- [ ] **Magic Link Data Collection (MISSING)**
- [ ] **Public Portal / Marketing Campaign (MISSING)**

### Question 2: Level of fidelity needed?
For the missing flows, what level of detail do you want?
- **A) High-level representation** - Show the screens/tabs exist, basic forms, minimal interaction
- **B) Full workflow** - Complete interactive flow with validation, state changes, etc.
- **C) Somewhere in between** - Key screens with realistic forms, but simplified logic

### Question 3: Focus on breadth vs. depth?
Would you rather I:
- **A) Add all 3 missing critical flows (Reporting, Magic Link, Public Portal) at basic level** - Shows complete end-to-end journey
- **B) Fully implement 1-2 of the missing flows** - Deep dive on specific workflows

### Question 4: Are there specific client questions to address?
Is there a particular workflow or feature the client is uncertain about? I can prioritize demonstrating that.

### Question 5: Timeline?
How much time do we have before the client demo?
- **A) 2-3 days** - Focus on highest priority flow only
- **B) 1 week** - Can add all 3 critical flows at basic level
- **C) 2+ weeks** - Can do comprehensive implementation

---

## Recommended Approach

Given your goal of **"representing the Figma user journeys well"**, here's my recommendation:

### Recommended Plan: "Complete Journey, Basic Fidelity"

**Goal:** Show every major workflow from Figma, even if simplified

**Approach:**
1. **Add Reporting Flow** (basic forms, submission workflow)
2. **Add Magic Link Workflow** (link generation, student form, completion tracking)
3. **Add Public Portal** (grant listings, detail page, "Apply Now" entry point)
4. **Enhance existing flows** (add revision workflow, multi-LEA)

**Timeline:** 1-2 weeks

**Result:** Client can see the complete end-to-end journey:
- Public Portal → IHE applies → LEA completes → Magic Links sent → Students fill out →
- Staff reviews → Approves → GAA sent → Signed → Payment → Reports submitted → Closed

This gives you a **discussion tool** to iterate on each workflow with the client.

---

**Next Steps:**
1. Let me know your answers to the questions above
2. I'll prioritize the improvements based on your input
3. I'll start implementing the highest-priority flows
4. We'll iterate based on your feedback

What do you think? Which approach makes sense for your client demo goals?
