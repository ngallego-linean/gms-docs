# GMS Prototype Implementation Plan

**Date:** December 6, 2025
**Based on:** Discovery notes (11.18, 11.21), GAA/Invoice templates

---

## Summary of Required Changes

Based on review of discovery notes and current prototype state, the following changes are needed:

| Feature | Priority | Effort | Current State |
|---------|----------|--------|---------------|
| GAA Document Generation | High | Medium | Has UI, needs actual template |
| Invoice Generation | High | Medium | Has UI, needs actual template |
| Waitlist Status | High | Medium | Not implemented |
| Archive View (LEA/IHE) | Medium | Low | Not implemented |
| Status Terminology Updates | Medium | Low | Partially done |
| Minor UI Fixes | Low | Low | Various |

---

## 1. GAA Document Generation

**Source:** UX Discovery 4 (11.18), GAA Template (11.25)

### Current State
- `Views/FiscalTeam/GAA.cshtml` exists with DocuSign integration mock
- Clicking "Generate GAA" calls `/FiscalTeam/SendGAA/{groupId}`
- Opens DocuSign in new tab (mock)

### Required Changes

**1.1 Create GAA Preview Page** (`Views/FiscalTeam/GAAPreview.cshtml`)

Before sending to DocuSign, show a preview of the generated GAA with:
- Grantee Name (LEA name)
- Grant Number (auto-generated format: `STSP-{FY}-{LEA_ID}-{SEQ}`)
- Agreement Term (fiscal year)
- Student count and total amount
- Attachment A reference (student list)
- Signer information fields

**1.2 GAA Template Fields** (from template)
```
- Grantee Name: [LEA Name]
- Grant Number: [Auto-generated]
- Agreement Term: July 1, 202X – June 30, 202X
- Student Teacher Count: [##]
- Original Amount: $##,000.00
- Signers:
  - Grantee Authorized Representative
  - Commission on Teacher Credentialing
  - Accounting Officer
```

**1.3 Files to Modify**
- `Views/FiscalTeam/GAA.cshtml` - Add "Preview" button before DocuSign
- `Controllers/FiscalTeamController.cs` - Add `GAAPreview` action
- Create `Views/FiscalTeam/GAAPreview.cshtml` - Show populated template
- Create `ViewModels/GAAPreviewViewModel.cs`

---

## 2. Invoice Generation

**Source:** UX Discovery 4 (11.18), Invoice Template (11.25)

### Current State
- `Views/FiscalTeam/GenerateInvoice.cshtml` exists
- Has form fields but doesn't match actual template format

### Required Changes

**2.1 Update Invoice Form** to match template:
```
- Grantee Name
- Grantee Address (City, State, Zip)
- Invoice Number: [Format: {GRANT_NUM}-{SEQ}]
- Date
- TO: Commission on Teacher Credentialing
      651 Bannon Street, Suite 600
      Sacramento, CA 95811
- FOR: Grant Program (Student Teacher Stipend Program)
- Description/Amount table
- ENY / Account Code fields
- Total
- Payment terms: "Payment is due within 45 days"
```

**2.2 Add Invoice Preview** (`Views/FiscalTeam/InvoicePreview.cshtml`)
- Show formatted invoice matching CTC template
- Print/Download button
- "Send to Accounting" button (triggers status change to "Payment in Process")

**2.3 Files to Modify**
- `Views/FiscalTeam/GenerateInvoice.cshtml` - Update form fields
- `Controllers/FiscalTeamController.cs` - Add preview action
- Create `Views/FiscalTeam/InvoicePreview.cshtml`

---

## 3. Waitlist Status

**Source:** Detail Discovery (11.21)

### Requirements
- When funding gets low, batches cease and students become singular submissions
- Add "WAITLIST" status so students can be paid based on place in line
- System sends notification to LEA/IHE about waitlist status
- Show on all portals (IHE, LEA, CTC)

### Implementation

**3.1 Add Waitlist Status to All Portals**

Update status dropdowns/displays in:
- `Views/IHEPortal/Applications.cshtml`
- `Views/IHEPortal/Dashboard.cshtml`
- `Views/LEAPortal/AllCandidates.cshtml`
- `Views/LEAPortal/Dashboard.cshtml`
- `Views/GrantsTeam/Dashboard.cshtml`
- `Views/GrantsTeam/Review.cshtml`

**3.2 Status Values (Updated)**
```
1. Submitted (submitted by IHE)
2. Pending (LEA is reviewing/processing)
3. Under Review (LEA sent to CTC; CTC is reviewing)
4. Waitlist (funding low, place in line)
5. Approved (Eligible for Payment)
6. Payment in Process (disbursement happening)
7. Paid (funds disbursed)
```

**3.3 Add Waitlist Mock Data**
- Add 2-3 candidates with WAITLIST status to mock data
- Show waitlist position/queue number

**3.4 CTC Dashboard: Add Funding Alert**
When funding reaches threshold (e.g., <20% remaining):
- Show alert banner
- "Waitlist Mode Active" indicator
- Link to manage waitlist

---

## 4. Archive View

**Source:** Detail Discovery (11.21)

### Requirements
- LEA/IHE need archive of previous applications over 3-year fiscal term
- Currently "All Applications" only shows pending

### Implementation

**4.1 LEA Portal - Add Archive Tab/Section**

Update `Views/LEAPortal/AllCandidates.cshtml`:
- Add tabs: "Current Year" | "Archived"
- Or add status filter: "Include Archived"
- Show past fiscal years' applications

**4.2 IHE Portal - Add Archive Tab/Section**

Update `Views/IHEPortal/Applications.cshtml`:
- Same pattern as LEA
- Filter by fiscal year

**4.3 Archive Mock Data**
Add candidates from previous fiscal years:
- FY 2023-24 (archived)
- FY 2024-25 (archived)
- FY 2025-26 (current)

---

## 5. Status Terminology Updates

**Source:** Detail Discovery (11.21)

### Changes Required

| Location | Current | Change To |
|----------|---------|-----------|
| LEA Dashboard | "Your Submissions to CTC" | Keep, but remove "batch" references |
| LEA Dashboard | "2 source applications" | Remove entirely |
| CTC Dashboard | "Applications with Pending Students" | "Submissions with Pending Students" |
| All portals | Various status labels | Align to 7-status system above |

### Files to Update
- `Views/LEAPortal/Dashboard.cshtml`
- `Views/LEAPortal/BatchDetails.cshtml` - Remove "batch" terminology
- `Views/GrantsTeam/Dashboard.cshtml`

---

## 6. Minor UI Fixes

**Source:** Detail Discovery (11.21)

### IHE Portal
- [ ] Remove "Last 4 of Social" column from submissions
- [ ] Remove "Last Modified" column
- [ ] Bulk uploads at TOP of SubmitCandidates page
- [ ] Add "Clinical Placement Information" section after Credential Area:
  - County CDS Code
  - LEA CDS Code
  - School CDS Code
  - Checkbox: "More than one placement" (shows additional CDS fields)

### LEA Portal
- [ ] LEA should be able to edit candidates awaiting review
- [ ] Show LEA name under "LEA point of contact" section
- [ ] Remove demographic checkbox (keep fields)

### Public Portal
- [ ] Change "Grant Applications" disclaimer to "Important Note"
- [ ] Fix "Higher Education Institutions" → "Institutions of Higher Education"

---

## Implementation Order

### Phase 1: Core Features (Recommended First)
1. **Waitlist Status** - Affects all portals, foundational change
2. **Status Terminology** - Quick wins, consistency

### Phase 2: Document Generation
3. **GAA Preview/Template** - Key fiscal workflow
4. **Invoice Preview/Template** - Key fiscal workflow

### Phase 3: Archive & Polish
5. **Archive Views** - LEA/IHE portals
6. **Minor UI Fixes** - Polish items

---

## Files Summary

### New Files to Create
- `Views/FiscalTeam/GAAPreview.cshtml`
- `Views/FiscalTeam/InvoicePreview.cshtml`
- `ViewModels/GAAPreviewViewModel.cs` (if needed)

### Files to Modify
| File | Changes |
|------|---------|
| `Views/FiscalTeam/GAA.cshtml` | Add preview flow |
| `Views/FiscalTeam/GenerateInvoice.cshtml` | Match template format |
| `Views/IHEPortal/Applications.cshtml` | Archive tab, waitlist status, remove columns |
| `Views/IHEPortal/SubmitCandidates.cshtml` | Bulk upload position, clinical placement section |
| `Views/LEAPortal/AllCandidates.cshtml` | Archive tab, waitlist status |
| `Views/LEAPortal/Dashboard.cshtml` | Terminology, waitlist display |
| `Views/LEAPortal/ReviewCandidates.cshtml` | Edit capability for candidates |
| `Views/GrantsTeam/Dashboard.cshtml` | Terminology, waitlist alert |
| `Views/GrantsTeam/Review.cshtml` | Waitlist status |
| `Controllers/FiscalTeamController.cs` | Preview actions |
| `Business/Helpers/StatusHelper.cs` | Add WAITLIST status |

---

## Questions to Clarify

1. **Waitlist threshold** - At what % of funding remaining does waitlist mode activate?
2. **Waitlist ordering** - FIFO based on submission date? Or other criteria?
3. **Archive retention** - Show all 3 years or just current + 1 previous?
4. **GAA Grant Number format** - Confirm: `STSP-{FY}-{LEA_ID}-{SEQ}`?
5. **Invoice number format** - Confirm: `{GRANT_NUM}-{SEQ}` per LEA per month?
