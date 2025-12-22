# GMS Entity Relationship Diagram

**California Commission on Teacher Credentialing**
**Grant Management System - Version 1.0**
**December 2025**

## How to View

Open `GMS-ERD.mermaid` in [mermaid.live](https://mermaid.live) to render the diagram.

---

## System Users

**Important:** Candidates (students) are NOT system users. They are the subject of grants but never log in.

| User Role | Organization Type | What They Do |
|-----------|-------------------|--------------|
| **IHE Staff** | University/College | Enter candidates (originating org), submit IHE reports |
| **LEA Staff** | School District | Review candidates, create monthly submissions to CTC, submit LEA reports |
| **CTC Grants Team** | CTC | Review/approve submissions, manage waitlist, approve reports |
| **CTC Fiscal Team** | CTC | Generate GAAs, process payments, track warrants |

### Key Relationship Clarifications
- **ORGANIZATION enters CANDIDATE** — IHE staff enter candidates into the system
- **ORGANIZATION submits SUBMISSION** — LEA staff create monthly submissions to CTC
- **SUBMISSION contains CANDIDATE** — Candidates are linked to monthly submissions (many-to-many via SUBMISSION_CANDIDATE)
- **CANDIDATE has CANDIDATE_REPORT** — Reports are *about* candidates
- **ORGANIZATION submits CANDIDATE_REPORT** — IHE/LEA staff *submit* reports

---

## R4 Compliance Legend

### Standard Fields (DB-114)
All tables include 5 audit fields (not shown in diagram for brevity):
- `CreatedOn` (datetime)
- `CreatedByAccountId` (int)
- `LastUpdatedOn` (datetime)
- `LastUpdatedByAccountId` (int)
- `CanBeDeleted` (bit)

### Archivable Tables (DB-113)
Some tables include archive fields:
- `IsArchived` (bit)
- `ArchivedOn` (datetime)
- `ArchivedByAccountId` (int)
- `ArchivedComments` (varchar)

### SVT - Static Value Tables (DB-201)
Lookup tables where code references specific IDs in business logic.

**Warning:** Changes to SVT data or structure may impact the application due to code being written against specific values.

Standard SVT fields:
- `Id` (PK)
- `Code` (UK)
- `Name`
- `Description`
- `DisplayOrder`

SVT tables also include archive fields (DB-113) for soft-delete support.

### Expected SVT Seed Values

#### CANDIDATE_STATUS_SVT
| Code | Category | Description |
|------|----------|-------------|
| `DRAFT` | Submission | IHE creating candidate record |
| `IHE_SUBMITTED` | Submission | Submitted by IHE to LEA |
| `LEA_REVIEWING` | Submission | LEA reviewing candidate |
| `LEA_APPROVED` | Submission | LEA approved, ready for batch |
| `CTC_SUBMITTED` | Submission | Submitted to CTC in monthly batch |
| `CTC_REVIEWING` | Review | CTC grants team reviewing |
| `CTC_APPROVED` | Review | Approved for funding |
| `CTC_REJECTED` | Review | Rejected (terminal) |
| `REVISION_REQUESTED` | Review | Returned for corrections |
| `WAITLIST` | Special | Approved but awaiting funding |
| `GAA_PENDING` | Disbursement | Awaiting GAA generation |
| `GAA_GENERATED` | Disbursement | GAA created |
| `GAA_SIGNED` | Disbursement | GAA signed by LEA |
| `INVOICE_GENERATED` | Disbursement | Invoice created |
| `PAYMENT_AUTHORIZED` | Disbursement | Payment approved |
| `WARRANT_ISSUED` | Disbursement | Warrant issued by SCO |
| `PAYMENT_COMPLETE` | Disbursement | Payment confirmed |
| `REPORTING_PENDING` | Reporting | Awaiting reports |
| `REPORTING_PARTIAL` | Reporting | One report submitted |
| `REPORTING_COMPLETE` | Reporting | Both reports submitted |
| `REPORTS_APPROVED` | Reporting | Reports approved (terminal) |

#### SUBMISSION_STATUS_SVT
| Code | Description |
|------|-------------|
| `DRAFT` | LEA preparing submission |
| `IN_PROGRESS` | Candidates being added |
| `SUBMITTED` | Sent to CTC |
| `UNDER_REVIEW` | CTC reviewing |
| `APPROVED` | All candidates processed |
| `CLOSED` | Cycle complete (terminal) |

#### REPORT_TYPE_SVT
| Code | ReportingOrgType | Description |
|------|------------------|-------------|
| `IHE_COMPLETION` | IHE | Program completion report |
| `LEA_EMPLOYMENT` | LEA | Employment outcomes report |
| `IHE_MIDYEAR` | IHE | Mid-year progress report |
| `LEA_PAYMENT` | LEA | Payment categorization report |

#### USER_ROLE_SVT
| Code | Description |
|------|-------------|
| `IHE_STAFF` | IHE portal user |
| `LEA_STAFF` | LEA portal user |
| `CTC_GRANTS` | CTC grants team member |
| `CTC_FISCAL` | CTC fiscal team member |
| `CTC_ADMIN` | CTC administrator |

### Naming Conventions
- **DB-110**: DateTime fields end with "On" (e.g., `CreatedOn`, `ApprovedOn`)
- **DB-108**: Bit fields start with `Is`, `Has`, or `Can` (e.g., `IsActive`, `HasCredentialEarned`)

---

## Terminology Mapping

| Database Term | UI Term | Notes |
|---------------|---------|-------|
| SUBMISSION | Submission | Monthly candidate submissions from LEA to CTC |
| CANDIDATE | Candidate | Student receiving the grant award |

---

## External Integrations

### ECS (Educator Credential System)
CTC's external system used for:
- **SEID Lookup** - Validating candidate credentials
- **Organization Sync** - IHE/LEA data (names, CDS codes, addresses)
- **School/District Lookup** - CDS code resolution

Fields on ORGANIZATION table:
- `EcsCdsCode` - County-District-School code from ECS
- `EcsOrganizationId` - Organization ID in ECS
- `LastEcsSyncOn` - Last sync timestamp

### DocuSign (GAA Signing)
Used for Grant Award Agreement electronic signatures.
Fields on DISBURSEMENT table:
- `GAAEnvelopeId` - DocuSign envelope ID
- `GAAStatus` - Envelope status (sent, signed, completed)
- `GAASentOn`, `GAASignedOn`, `GAACompletedOn` - Timestamps

### Fi$Cal (State Financial System)
California's statewide financial system for invoice and warrant processing.

---

## Funding & Disbursement

### Budget Tracking (GRANT_CYCLE)

| Field | Description |
|-------|-------------|
| `AppropriatedAmount` | Total budget allocated for the grant cycle |
| `EncumberedAmount` | Funds committed/locked (PO issued) |
| `DisbursedAmount` | Funds actually paid out (warrant confirmed) |

**Remaining** = Appropriated - Encumbered (calculated)

### Disbursement Workflow States (DISBURSEMENT_STATUS_SVT)

| Status | Description | Next Action |
|--------|-------------|-------------|
| `NEEDS_GAA` | Submission approved, awaiting GAA | Generate & send GAA for signature |
| `GAA_SENT` | GAA sent to LEA for signature | Awaiting signature |
| `GAA_SIGNED` | GAA signed by LEA | Generate PO |
| `NEEDS_PO` | Awaiting Purchase Order | Upload PO from Fi$Cal |
| `PO_UPLOADED` | PO received and uploaded | Generate invoice |
| `NEEDS_INVOICE` | Awaiting invoice generation | Generate invoice |
| `INVOICE_SENT` | Invoice sent to Fi$Cal/Accounting | Awaiting warrant |
| `NEEDS_WARRANT` | Awaiting warrant from State Controller | Confirm warrant receipt |
| `COMPLETE` | Warrant confirmed, payment complete | Terminal state |

### Disbursement Document Flow
```
GAA Signed → PO Generated → PO Uploaded → Invoice Generated → Invoice Sent → Warrant Confirmed
```

### Submission Timeline Events (SUBMISSION_TIMELINE_EVENT)

Tracks significant activities during a submission's lifecycle.

| Event Type | Description |
|------------|-------------|
| `CANDIDATE_ADDED` | Candidate added to submission |
| `CANDIDATE_REMOVED` | Candidate removed from submission |
| `IHE_DATA_RECEIVED` | Data received from IHE partner |
| `DOCUMENT_UPLOADED` | Supporting document attached |
| `COMMENT_ADDED` | Internal comment recorded |
| `STATUS_CHANGED` | Submission status transition |
| `VERIFICATION_COMPLETE` | Credential/employment verified |

The `IHESource` field identifies which IHE partner originated the event (for submissions with multiple IHE sources).

---

## Table Count Summary

| Section | Tables |
|---------|--------|
| Core Configuration | 2 |
| Organizations & Accounts | 4 |
| Candidates | 3 |
| Submissions | 6 |
| Disbursements | 4 |
| Reporting | 4 |
| Files | 2 |
| Notifications | 2 |
| Audit | 1 |
| **Total** | **28** |
