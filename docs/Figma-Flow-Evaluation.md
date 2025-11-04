# Figma Flow Evaluation vs. GMS Requirements Analysis

**Date:** November 3, 2025
**Purpose:** Evaluate Figma Data Flow Diagram against documented requirements, SOW, and Process Flow Template

---

## Executive Summary

The Figma flow diagram **largely aligns** with our documented analysis but reveals several **gaps, ambiguities, and opportunities for refinement**. This evaluation identifies:
- ✅ **Confirmed features** matching our V1 requirements
- ⚠️ **Gaps** between Figma and documented requirements
- 🔧 **Technical clarifications** needed for implementation
- 📋 **Process improvements** to incorporate

---

## Flow-by-Flow Comparison

### 1️⃣ IHE Application Flow

#### Figma Shows:
```
Marketing Campaign → IHE clicks link → Signs in to portal → Starts application

IHE fills in:
- Their own POC
- Candidate's name
- DOB
- Last 4 of SSN/ITIN
- Credential area
- County assignment (CDS)
- LEA POC
- Checkbox for more than one LEA placement

→ IHE submits application
```

#### ✅ Matches Our Analysis:
- Two-party workflow starts with IHE
- Marketing campaign awareness (Public Portal)
- Portal login required (OAuth authentication)
- Collects candidate demographics
- Multi-LEA placement handling

#### ⚠️ Gaps & Questions:

| Issue | Figma Flow | Our Requirements | Resolution Needed |
|-------|-----------|------------------|-------------------|
| **SEID Matching** | Not shown | "Back-end matches candidate to SEID from ECS" (Process Flow Template) | **When does SEID matching occur?** After IHE enters name/DOB? Or only when LEA confirms? |
| **Multiple Candidates** | Not shown | IHE may apply for 10-50 student teachers | **Does IHE submit one application with multiple candidates, or multiple applications?** Critical for UX design |
| **Credential Area** | Single field | Process template shows it's critical for SEID match | **Is this a dropdown (pre-defined list) or free text?** Need CDE credential area taxonomy |
| **County Assignment (CDS)** | Listed as single field | CDS format: `00-00000-0000000` (County-District-School) | **Does IHE enter full CDS or just county? Does system validate format?** |
| **Draft State** | Not shown | Applications can be saved as drafts | **Can IHE save draft and return later? Auto-save?** |

#### 🔧 Technical Implications:
- **SEID lookup timing**: If SEID matching happens at IHE submission, need to handle "SEID not found" scenarios (candidate hasn't applied for COC yet)
- **Multiple candidates**: If one application = many candidates, need repeating form sections or bulk upload (CSV)
- **CDS validation**: Need real-time validation against ECS organization directory

---

### 2️⃣ LEA Completion Flow

#### Figma Shows:
```
LEA receives email → Clicks link, logs in → Views application

LEA fills in:
- Their own POC
- Confirms SEID
- Confirms GAA Signer
- Fiscal agent
- Superintendent
- How they intend to pay ST
- Payment schedule

→ LEA submits application
```

#### ✅ Matches Our Analysis:
- Email notification triggers LEA action
- Separate login/view for LEA (role-based access)
- Collects fiscal/payment info
- SEID confirmation step

#### ⚠️ Gaps & Questions:

| Issue | Figma Flow | Our Requirements | Resolution Needed |
|-------|-----------|------------------|-------------------|
| **"Confirms SEID"** | Single action | SEID is auto-matched from ECS | **What does LEA "confirm"? Just verify it's correct, or enter it if IHE didn't provide?** |
| **Superintendent** | Required field | Not mentioned in Process Flow Template | **Is superintendent always required? Same as GAA signer in some cases?** |
| **Payment Schedule** | Free-form? | Process template: "one payment," "two payments," etc. | **Dropdown or free text? Need predefined options** |
| **Multiple Candidates** | Not addressed | If IHE applied for 10 STs, does LEA confirm all at once? | **Bulk confirmation UI needed** |
| **Magic Links** | Not shown | Our requirement: LEA sends magic links to collect grantee demographics | **Figma doesn't show this step. Where does it fit?** |

#### 🔧 Technical Implications:
- **SEID confirmation UI**: Show SEID + candidate name/DOB, allow LEA to flag if incorrect
- **Payment schedule**: Need dropdown with options like "Single $10k payment," "Two $5k payments (split between LEAs)," etc.
- **Magic link integration**: After LEA submits, should system prompt "Send data collection links to candidates now?"

---

### 3️⃣ Application Review & Approval

#### Figma Shows:
```
Grants team receives notification → COMPLETED applications in queue
→ Grants Team reviews application
→ Decision: Approved / Not Approved / Not Approved; No Funding

If "Not Approved; Bad Info":
→ LEA fixes application issues (loop back)

If "Not Approved; No Funding":
→ LEA receives notification → End Process

If "Approved; CTC processes payment":
→ Continue to Fiscal Team
```

#### ✅ Matches Our Analysis:
- Queue-based review workflow
- Three outcome states: Approve, Reject (fixable), Reject (out of funds)
- Notification to LEA for corrections
- Conditional funding revision loop

#### ⚠️ Gaps & Questions:

| Issue | Figma Flow | Our Requirements | Resolution Needed |
|-------|-----------|------------------|-------------------|
| **Review Criteria** | Not specified | STIPEND is non-competitive: eligibility check only | **What makes an app "Not Approved"? Incomplete? Ineligible org? Duplicate candidate?** |
| **Revision Process** | "LEA fixes application issues" | Our analysis: In-app inline comments (Phase 2) vs. email (V1) | **V1 approach: Email with list of issues, or unlock app for editing with highlighted fields?** |
| **Fund Depletion Logic** | "No Funding" state shown | Our requirement: Real-time fund tracking, soft-block at <10% | **When is "No Funding" determined? At submission (block) or at review (reject)?** Figma shows it as a review decision, but we want to prevent submission if funds exhausted |
| **Approval = Award?** | Implies instant | Our analysis: Approval → Award generation → GAA signing → Payment | **Figma conflates "Approved" with "CTC processes payment." Need clearer state transitions** |
| **Who reviews?** | "Grants team" | Process template: "Cara Mendoza, New staff to review, Sara Saelee or Eddie contribute/have access" | **Assignment logic? First available? Round-robin? Manual assignment by admin?** |

#### 🔧 Technical Implications:
- **Eligibility rules engine**: Define rejection criteria (e.g., duplicate SEID in same grant cycle, ineligible credential area, etc.)
- **Fund reservation**: When LEA submits, reserve funds? Or only reserve on approval?
- **Revision workflow**: V1 = unlock application + email notification. Phase 2 = inline comments with version history

---

### 4️⃣ GAA & DocuSign Workflow

#### Figma Shows:
```
Fiscal team receives notification → GAA is needed
→ Fiscal team creates GAA [in system? or uploads to system?]
→ Fiscal team sends GAA in system via DocuSign
→ LEA Signs DocuSign
→ Fiscal team receives notification → DocuSign completed
```

#### ✅ Matches Our Analysis:
- Fiscal team triggers GAA creation
- DocuSign integration shown
- Notification loop back to fiscal team

#### ⚠️ Gaps & Questions:

| Issue | Figma Flow | Our Requirements | Resolution Needed |
|-------|-----------|------------------|-------------------|
| **"Creates GAA [in system? or uploads to system?]"** | Uncertainty noted in diagram! | Our requirement: GMS auto-populates GAA template from application data | **V1 approach: System generates GAA content (pre-filled Word/PDF), fiscal reviews/triggers DocuSign. System doesn't create DocuSign envelope?** Or does it? Need clarity |
| **Manual Creation Option** | Implied by "or uploads" | Not in our requirements | **Should fiscal have option to manually create GAA outside system and upload? Or always system-generated?** Recommend: Always system-generated, but allow manual override for exceptions |
| **DocuSign Routing** | Not specified | Process template: "LEA signer → Cara → Saelee" (three-party signing) | **Figma only shows LEA signing. Missing CTC signature steps** |
| **GAA Template** | Not shown | Different grant programs may have different GAA templates | **Is GAA template configured per grant program? Or global with merge fields?** |
| **Notification Recipient** | "Fiscal team" | Process template pain point: "DocuSign only emails signer, not day-to-day contact" | **Should system email both GAA signer AND day-to-day LEA contact?** Yes per requirements |

#### 🔧 Technical Implications:
- **DocuSign API**: Must create envelope with routing order (LEA → Cara → Saelee), track signature status via webhooks
- **GAA template engine**: Razor or Word template with merge fields from `Application` + `Award` entities
- **Manual override**: Allow fiscal staff to upload custom GAA if needed (edge case)

---

### 5️⃣ Payment Process

#### Figma Shows:
```
Payment Process [OUTSIDE SYSTEM]
→ Fiscal team alerts system that payment has been distributed
→ LEA receives notice that payment is coming
→ LEA pays ST [OUTSIDE SYSTEM]
```

#### ✅ Matches Our Analysis:
- Payment process marked as external (FI$Cal)
- Manual status update by fiscal team
- LEA notified when payment coming

#### ⚠️ Gaps & Questions:

| Issue | Figma Flow | Our Requirements | Resolution Needed |
|-------|-----------|------------------|-------------------|
| **Payment Status Tracking** | Single "distributed" flag | Our Phase 1: Export to FI$Cal. Phase 2: Track PO → Invoice → Voucher → Warrant | **V1: Binary flag "Payment Sent: Yes/No"? Or track "Payment Initiated Date" + "Payment Completed Date"?** |
| **Notification Timing** | "LEA receives notice that payment is coming" | When? | **Trigger: When fiscal marks "payment distributed"? Or earlier when PO created?** |
| **LEA Payment Confirmation** | Not shown | Process template: LEA needs space to mark award distributed to ST | **After LEA pays ST, do they confirm in system? Or only in final report?** |
| **Multiple Payments** | Not addressed | Process template: LEA can pay "one payment," "two payments," etc. | **How does system track payment schedule LEA indicated? Is it just informational, or does system remind LEA of next payment due?** |

#### 🔧 Technical Implications:
- **V1 Payment Tracking**: Simple fields on `Award` entity: `PaymentInitiatedDate`, `PaymentCompletedDate`, `FiscalNotes` (text field)
- **Phase 2**: Add `PaymentStatus` enum: `NotStarted → POCreated → InvoiceGenerated → VoucherCreated → WarrantIssued → Completed`
- **LEA Payment**: Add `Award.LeaPaidStudentTeacherDate` for tracking downstream

---

### 6️⃣ Reporting Flow

#### Figma Shows:
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

#### ✅ Matches Our Analysis:
- Separate IHE and LEA reports
- Email notifications to trigger reporting
- Grants team reviews submitted reports

#### ⚠️ Gaps & Questions:

| Issue | Figma Flow | Our Requirements | Resolution Needed |
|-------|-----------|------------------|-------------------|
| **Report Types** | Only two shown | Process template: Annual program reports (July-Aug) + expenditure reports (Nov) | **Are these the only two report types? Or example subset?** STIPEND may be simpler than other grants |
| **Report Timing** | Not specified | Process template: Annual reports due July-Aug, expenditure reports Nov | **Are these one-time reports per award, or recurring annually?** If multi-year award, annual reports each year? |
| **Data Validation** | Not shown | Process template pain point: "Tedious data validation, time suck" | **What validation rules? Math errors? Required fields? Cross-checks against application data?** |
| **Revision Loop** | Not shown | Process template: "Grantee edits, staff reviews again, spans many weeks/months" | **If report has errors, can grantee edit inline? Or email back-and-forth?** Same as application revision question |
| **Early Exits** | Not shown | Process template: "IHE responsible for noting early exiter, if changed to intern" | **Where does early exit tracking fit? Is this part of the "Completion confirmation" report? Or separate flag during grant period?** |
| **Magic Link Data** | Not mentioned | Our requirement: LEA collected demographics via magic links | **Is that data surfaced in reports? Or only for internal CTC use?** |

#### 🔧 Technical Implications:
- **Report Schema**: Define JSON schema per report type (like application forms)
- **Validation**: Server-side validation rules, client-side feedback
- **Phase 2**: Consider in-app revision workflow (same as application conditional funding)

---

## Missing Elements Not Shown in Figma

### 1. **Magic Link Grantee Data Collection**
**Our Requirement:** LEA sends tokenized links to individual student teachers to collect demographics/grant-specific data.

**Figma:** Not shown. IHE enters candidate info, LEA confirms, but no step for LEA → Grantee direct data collection.

**Impact:** This is a **REQUIRED V1 feature** per our analysis. Major efficiency gain (LEA doesn't manually gather data from 10-50 candidates).

**Where it fits:** After LEA submits application (or after approval?), LEA clicks "Send Data Collection Links" → System generates tokens → Emails sent to candidates → LEA sees completion dashboard → Once all complete, application marked "Fully Ready for Award."

### 2. **Real-Time Fund Tracking Dashboard**
**Our Requirement:** Staff see remaining appropriation amount in real-time, alerts when <10% remaining.

**Figma:** Shows "Not Approved; No Funding" decision, but no dashboard or proactive fund tracking.

**Impact:** **REQUIRED V1 feature**. Critical for preventing over-awarding.

**Where it fits:** Staff Portal dashboard widget: "Grant Cycle XYZ: $2.5M appropriated, $1.8M awarded, $700K remaining (28%)."

### 3. **Intent to Apply**
**Process Flow Template mentions:** "Intent to Apply" submission before full application.

**Figma:** Not shown. Goes straight from "Marketing Campaign" → "IHE clicks link, signs in, starts application."

**Impact:** **OPTIONAL for V1**. Helps CTC forecast application volume, but not critical.

### 4. **Application Draft State**
**Our Requirement:** Applicants can save draft and return later.

**Figma:** Only shows "IHE submits application" (implies one-shot completion).

**Impact:** **REQUIRED V1 feature**. Applications are complex, users need to save progress.

**Where it fits:** "Save Draft" button visible throughout application form. Applications in "Draft" state don't notify Grants Team.

### 5. **Award Entity Separation**
**Figma:** "Approved; CTC processes payment" implies instant payment.

**Our Domain Model:** `Application (approved)` → `Award (generated)` → `GAA (signed)` → `Payment (initiated)` are separate states.

**Impact:** Figma oversimplifies. Need clearer state machine.

**Correct flow:**
1. Grants Team approves application → `Application.Status = Approved`
2. System auto-generates `Award` record → `Award.Status = Pending GAA`
3. Fiscal triggers DocuSign → `Award.Status = GAA Sent`
4. All parties sign → `Award.Status = GAA Signed, Award.GaaSignedDate = [timestamp]`
5. Fiscal initiates payment → `Award.Status = Payment Initiated`
6. Fiscal confirms payment → `Award.Status = Payment Completed`

### 6. **Public Portal / Marketing Campaign Details**
**Figma:** Shows "Marketing Campaign" as starting point, but no detail.

**Our Requirement:** Public Portal with grant listings, RFA info, eligibility criteria.

**Impact:** **REQUIRED V1**. Public awareness is critical for grant uptake.

**What's needed:** Public-facing pages (no login) with:
- Available grants list
- Grant details (eligibility, amount, deadlines)
- Link to Grantee Portal login ("Apply Now" button)
- Downloadable RFA PDF (or dynamically generated page)

### 7. **Organization Management**
**Figma:** Assumes IHE/LEA identities are known/validated.

**Our Domain Model:** `Organization` entity with `IsVerified` flag.

**Impact:** How do new IHEs/LEAs register? Or is org list pre-seeded from ECS?

**V1 Approach:** Pre-seed `Organization` table from ECS export. If user's email domain doesn't match known org, require manual verification by admin before application submission allowed.

### 8. **User Roles & Permissions**
**Figma:** Shows three swim lanes (IHE/LEA, Grants Team, Fiscal Team) but no detail on role-based access.

**Our Requirement:** Admin, Grants Team, Fiscal, Reviewer, IHE User, LEA User, Public.

**Impact:** Need authorization policies for each role. Example:
- IHE User: Can create/edit applications for their IHE, view own applications
- LEA User: Can complete/edit LEA portion of applications for their LEA, view own applications
- Grants Team: Can view all applications, approve/reject, assign reviewers
- Fiscal Team: Can view approved applications, trigger GAA, update payment status
- Admin: Can configure grant programs, manage users, override any action

---

## Process Improvements & Clarifications Needed

### 1. **Application Submission Lock**
**Question:** When IHE submits, is application locked? Or can IHE edit until LEA also submits?

**Recommendation:**
- IHE submits → `Application.Status = Submitted by IHE` (IHE portion locked, LEA can still edit)
- LEA submits → `Application.Status = Submitted` (both portions locked, ready for review)
- If revision needed, Grants Team "unlocks" for LEA to edit

### 2. **Fund Reservation Timing**
**Question:** When are funds reserved? At submission? At approval? At GAA signing?

**Recommendation:**
- Submission: No reservation (prevents gaming the system)
- Approval: Reserve funds (`GrantCycle.ReservedAmount += award_amount`)
- GAA Signing: Convert reserved to encumbered (`GrantCycle.EncumberedAmount`, `ReservedAmount` decreases)
- Payment: Convert encumbered to disbursed (`GrantCycle.DisbursedAmount`)

### 3. **Duplicate Candidate Prevention**
**Question:** Can same candidate (SEID) receive STIPEND from multiple IHE/LEA pairs?

**Assumption:** No. One STIPEND award per candidate per grant cycle.

**Implementation:** Validate at LEA submission: Check if `SEID` already exists in approved/pending applications for this grant cycle. If yes, reject with error "Candidate already has application in progress."

### 4. **Multi-LEA Split Payment**
**Figma shows:** Checkbox for "more than one LEA placement."

**Question:** How does this work?
- One application with two LEA portions (IHE → LEA1 completes, LEA2 completes)?
- Or two separate applications (IHE applies twice, once per LEA)?

**Recommendation:** One application, but after IHE submits, system allows multiple LEA completions. Each LEA gets $5k (if two), $3.33k (if three), etc. Award calculation: `$10,000 / number_of_LEAs`.

### 5. **Email Notification Cadence**
**Figma shows many notifications.** Need to define:
- **Immediate:** Application submitted, GAA signed, payment distributed
- **Daily Digest:** New applications in queue for review (batch notifications to avoid spam)
- **Reminders:** Application incomplete after 7 days, report due in 14 days (scheduled jobs)

### 6. **Report Due Date Calculation**
**Question:** How is report due date determined?
- Fixed date per grant cycle (e.g., all annual reports due August 1)?
- Or relative to award date (e.g., 12 months after GAA signing)?

**Recommendation:** Configure per grant program: `ReportingRules.DueDateType = FixedDate | RelativeToAwardDate`.

---

## Technical Architecture Gaps

### 1. **State Machine Definition**
Figma shows decision diamonds but not explicit state transitions. Need to formalize:

**Application States:**
```
Draft → Submitted by IHE → Submitted (by LEA) → Under Review →
(Rejected | Conditionally Approved | Approved) →
(if Conditionally Approved) → Draft (unlocked for revision) → ...
→ (if Approved) → Award Generated
```

**Award States:**
```
Pending GAA → GAA Sent → GAA Signed →
Payment Initiated → Payment Completed →
(if reports required) → Awaiting Reports → Reports Submitted → Closed
```

### 2. **Webhook Integration Points**
- **DocuSign:** Signature completed webhook → Update `Award.GaaSignedDate`, `Award.Status`
- **Email Service:** Delivery/open/click tracking (optional)
- **FI$Cal (Phase 2):** Payment status updates

### 3. **Background Jobs / Scheduled Tasks**
- **Nightly:** Check for overdue reports, send reminders
- **Hourly:** Check for grant cycle close dates approaching, send alerts to staff
- **On-Demand:** Export data to FI$Cal (triggered by fiscal staff)
- **Phase 2:** Poll FI$Cal for payment status updates

---

## V1 Scope Validation

### ✅ **Figma Flow Supports V1 Requirements**
- Two-party application workflow (IHE → LEA)
- Review & approval by Grants Team
- DocuSign GAA integration
- Payment status tracking (manual for V1)
- Basic reporting (structure defined)

### ⚠️ **Figma Gaps That Are V1 Requirements**
- Magic link grantee data collection (**CRITICAL - not shown**)
- Real-time fund tracking dashboard (**CRITICAL - not shown**)
- Draft/auto-save application state (**Important UX**)
- Public Portal for marketing/awareness (**Required**)
- Clear Award entity and state transitions (**Architecture**)

### ✅ **Figma Correctly Defers to Phase 2**
- Competitive grant scoring (not shown, not needed for STIPEND)
- In-app revision workflows (shown as "LEA fixes issues" but not detailed)
- Advanced reporting features

---

## Recommendations for CTC Stakeholder Review

### Questions to Ask:
1. **Magic Links:** "We don't see the step where LEAs collect data from individual student teachers. Is this a required feature, or does LEA manually gather and enter all candidate info?"
   - **Our recommendation:** Magic links are critical for V1 efficiency.

2. **SEID Matching:** "When does the system match candidates to their SEID? When IHE enters name, or when LEA confirms?"
   - **Our recommendation:** Attempt match when IHE enters name/DOB, allow LEA to correct if wrong.

3. **Multiple Candidates:** "Does one application cover one candidate, or can IHE submit a single application for 50 student teachers?"
   - **Our recommendation:** Depends on volume. If typical IHE has 5-10, repeating form sections OK. If 50+, need bulk CSV upload.

4. **Fund Depletion:** "How should the system prevent over-awarding? Block new applications when funds low, or allow submission but reject at review?"
   - **Our recommendation:** Show warning at submission if <10% funds remaining, but allow. Hard block at 0%.

5. **GAA Creation:** "The Figma notes uncertainty: 'Fiscal team creates GAA [in system? or uploads to system?]' — what's the desired V1 behavior?"
   - **Our recommendation:** System auto-generates GAA from template, fiscal reviews and triggers DocuSign. No manual upload needed.

6. **DocuSign Routing:** "Figma only shows LEA signing. Who else signs the GAA?"
   - **Clarify:** Process template says Cara and Saelee also sign. Confirm routing order.

7. **Reporting Timing:** "Are the IHE/LEA reports one-time (at program end) or annual recurring reports?"
   - **Need answer** to design report schema and due date logic.

---

## Conclusion

**Overall Assessment:** The Figma flow is a **solid starting point** that captures the core STIPEND workflow. It aligns well with our documented requirements (~80% match).

**Critical Gaps for V1:**
1. Magic link grantee data collection workflow (**must add**)
2. Real-time fund tracking dashboard (**must add**)
3. Draft application state (**must add**)
4. Public Portal details (**must add**)
5. Clear Award state machine (**architectural clarity needed**)

**Next Steps:**
1. ✅ Share this evaluation with CTC stakeholders
2. ✅ Hold review meeting to address open questions
3. ✅ Update Figma with missing flows (magic links, fund tracking, draft states)
4. ✅ Formalize state machines (Application, Award) in technical documentation
5. ✅ Confirm V1 scope and defer Phase 2 features explicitly

**Confidence Level:** With clarifications on the 7 key questions above, this flow can proceed to **Business Requirements Document** and **ERD development**.
