# GMS Interactive Mockup

High-level interactive mockup demonstrating the Grant Management System workflow.

## What This Demonstrates

### Key Concept: Batch Student Management
- **One application per IHE-LEA pair per grant cycle**
- IHEs and LEAs add multiple students to their application throughout the year
- Students can be added incrementally as they begin student teaching
- Each student has their own status within the application

### Three Portals

#### 1. Staff Portal (CTC)
- **Dashboard**: Real-time financial tracking and metrics
  - Appropriated, Reserved, Encumbered, Disbursed, Remaining funds
  - Outstanding balance (committed but not yet paid)
  - Student counts by status
  - Participant metrics (IHEs, LEAs, SEIDs)
  - Fund depletion warnings
- **Applications**: Review all IHE-LEA applications and their students
  - Approve/reject individual students
  - Fund availability checking before approval
- **Awards & GAAs**: Track DocuSign GAA routing and payment status
  - Simulate 3-party signature workflow (LEA → Cara → Sara)
  - Initiate payments and mark as completed
  - Watch funds move: Reserved → Encumbered → Disbursed

#### 2. IHE Portal
- **Your Grant Applications**: List of all IHE-LEA applications
  - One application card per LEA partner
  - Shows total students, approved count, pending count
  - Click application to manage students for that IHE-LEA pair
- **Manage Students** (within application):
  - Add new students to specific IHE-LEA application
  - Save as draft or submit directly to LEA
  - Edit drafts before submitting
  - Track student status (Draft, Pending LEA, Submitted, Approved)
- **Quick Stats**: See total, approved, and pending students across all applications

#### 3. LEA Portal
- **Your Grant Applications**: List of all IHE-LEA applications
  - One application card per IHE partner
  - Shows total students, approved count, pending LEA count
  - Click application to complete student information for that IHE-LEA pair
- **Complete Student Information** (within application):
  - View student info from IHE (read-only)
  - Add LEA fiscal data (dates, contact, signatory)
  - Confirm eligibility (540+ hours)
  - Submit to CTC for review
- **Quick Stats**: See students needing LEA action across all applications

## Interactive Features

### Complete Workflows You Can Test

1. **IHE adds student → LEA completes info → Staff approves → Award created**
   - Switch to **IHE Portal**
   - Go to **Applications** (shows list of IHE-LEA application cards)
   - Click **"View Students"** on Cal State Fullerton → LAUSD application
   - Click **"+ Add Student"**
   - Fill out form (or save as draft)
   - Click **"Save & Submit to LEA"**
   - Switch to **LEA Portal**
   - Go to **Applications** (shows list of IHE-LEA application cards)
   - Click **"View Students"** on Cal State Fullerton → LAUSD application
   - Click **"Complete LEA Info"** on pending student
   - Fill out fiscal information
   - Click **"Submit to CTC"**
   - Switch to **Staff Portal**
   - Go to **Applications** (shows all IHE-LEA application cards)
   - Click **"View Students"** on any application
   - Click **"Approve"** on submitted student (watch funds move from Remaining → Reserved)

2. **Staff generates GAA → Simulate DocuSign routing → Initiate payment**
   - From Staff Portal → Applications
   - Approved student shows "Generate GAA" button
   - Click to generate GAA
   - Simulate each signature (LEA, Cara, Sara)
   - Watch award status change: GAA Sent → GAA Signed
   - Watch funds move: Reserved → Encumbered
   - Go to Awards view
   - Click "Initiate Payment" → "Mark as Paid"
   - Watch funds move: Encumbered → Disbursed

3. **Fund depletion scenario**
   - Approve multiple students
   - Watch dashboard metrics update in real-time
   - See warning alerts when funds drop below 10%
   - Try to approve when insufficient funds (rejection modal)

### Real-Time Dashboard Updates
- Approving students updates dashboard metrics immediately
- Fund status colors change based on remaining percentage:
  - Green: >25% remaining
  - Orange: 10-25% remaining
  - Red: <10% remaining
- Progress bars animate when values change
- Metric cards pulse briefly on update

### State Visualization
- Student status badges color-coded by state
- Award GAA progress bars show signature completion
- Signature tracking with timeline showing who signed when
- Modal workflows guide user through multi-step processes

## Technical Structure

```
mockup/
├── index.html                    # Main HTML structure
├── css/
│   ├── styles.css                # Global styles, components, utilities
│   └── dashboard.css             # Dashboard-specific styles
├── js/
│   ├── mockData.js               # Mock data (grant cycles, applications, students, awards)
│   ├── app.js                    # Main app controller, portal switching, routing
│   └── components/
│       ├── dashboard.js          # Dashboard rendering and metrics
│       ├── applications.js       # Student batch management (IHE/LEA/Staff views)
│       └── awards.js             # GAA generation, DocuSign tracking, payments
└── README.md                     # This file
```

## Data Model

### Grant Cycle
- Appropriation amount
- Reserved, Encumbered, Disbursed amounts
- Calculated: Remaining, Outstanding Balance

### Application (Batch Container)
- One per IHE-LEA pair per grant cycle
- Status: ACTIVE (open for adding students throughout year)
- Contains multiple students

### Student (within Application)
- Student info (name, DOB, SEID, credential area)
- Status: DRAFT → PENDING_LEA → SUBMITTED → APPROVED/REJECTED
- IHE-entered data + LEA-entered data
- Award amount once approved

### Award (per Student)
- Created when student approved
- Status: PENDING_GAA → GAA_SENT → GAA_SIGNED → PAYMENT_INITIATED → PAYMENT_COMPLETED
- GAA signatures (3-party: LEA, Cara, Sara)
- Fiscal reference number

## What's NOT Implemented

This is a high-level mockup focusing on workflow and UX. The following are intentionally simplified or mocked:

- **No persistence**: Refreshing the page resets all data
- **No backend API**: All data manipulation happens in-memory
- **Simplified forms**: Real forms would have more validation and fields
- **No JSON Schema**: Forms are hardcoded for STIPEND grant (not dynamic)
- **No actual DocuSign API**: Signatures are simulated with buttons
- **No actual ECS SEID matching**: Random SEID generation on student add
- **No charts**: Placeholders shown instead of Chart.js visualizations
- **No file uploads**: Document attachment not implemented
- **No search/filtering**: Application lists show all records
- **No pagination**: All data shown at once
- **No authentication**: Portal switching simulates different users
- **No reports**: Report submission workflow not included

## How to Use

1. Open `index.html` in a web browser
2. Start in **Staff Portal** to see the dashboard
3. Switch to **IHE Portal** to add students
4. Switch to **LEA Portal** to complete fiscal info
5. Return to **Staff Portal** to approve and generate GAAs
6. Click around, explore workflows, test interactions
7. Watch toast notifications for feedback
8. Check browser console for any errors (should be none)

## Key Insights from Mockup

### For Stakeholders:
- Visualizes the batch student model (not one-app-per-student)
- Demonstrates real-time dashboard with fund tracking
- Shows complete workflow from IHE entry → LEA info → CTC approval → GAA → Payment
- Illustrates fund state transitions (Remaining → Reserved → Encumbered → Disbursed)
- Demonstrates multi-party GAA routing

### For Developers:
- Component structure and separation of concerns
- Data model relationships (Grant Cycle → Application → Student → Award)
- State management approach (in-memory for mockup, would use API + state library in production)
- UI patterns for forms, tables, modals, toasts
- Event handling and portal-specific views
- Calculated properties (remaining funds, outstanding balance)

## Next Steps

After stakeholder review:
1. Refine workflows based on feedback
2. Identify missing features or edge cases
3. Confirm data model matches business requirements
4. Use mockup as blueprint for actual implementation
5. Expand to include Phase 2 features (reports, public portal)

---

**Version:** 1.0
**Date:** November 4, 2025
**Note:** This is a mockup for demonstration only. No data is saved or transmitted.
