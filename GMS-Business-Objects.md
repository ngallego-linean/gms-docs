# GMS Business Objects Reference

This document defines the high-level business objects for the Grant Management System (GMS), derived from UI mockup analysis.

---

## Core Entities

### 1. Student (Candidate)

The central entity representing a student teacher applying for grant funding.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| StudentId | int | Yes | Primary key |
| SEID | string | Yes | Statewide Educator Identifier (from ECS) |
| FirstName | string | Yes | Legal first name |
| LastName | string | Yes | Legal last name |
| DateOfBirth | date | Yes | Date of birth |
| Last4SSN | string | No | Last 4 digits of SSN/ITIN (masked display) |
| Race | enum | No | See Demographics enum |
| Ethnicity | enum | No | Hispanic or Latino / Not Hispanic or Latino |
| Gender | enum | No | Male / Female / Non-binary / Prefer not to say |
| CredentialArea | enum | Yes | See Credential Area enum |
| Status | enum | Yes | PENDING, UNDER_REVIEW, APPROVED, WAITLIST, PAID |
| AwardAmount | decimal | Yes | Stipend amount (typically $8,000) |
| CreatedAt | datetime | Yes | Record creation timestamp |
| SubmittedAt | datetime | No | When submitted to CTC |
| ApprovedAt | datetime | No | When approved by CTC |

**Relationships:**
- Belongs to one IHE (originating institution)
- Belongs to one or more LEAs (placement districts)
- Has one or more Placements
- Belongs to one Application
- Has zero or more Reports

---

### 2. Placement

Clinical placement information for a student. A student may have up to two placements.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| PlacementId | int | Yes | Primary key |
| StudentId | int | Yes | Foreign key to Student |
| PlacementOrder | int | Yes | 1 = Primary, 2 = Secondary |
| CountyCDSCode | string | Yes | 2-digit California county code |
| CountyName | string | Yes | County name (derived from ECS) |
| LEACDSCode | string | Yes | 7-digit LEA/district code |
| LEAName | string | Yes | District name (derived from ECS) |
| SchoolCDSCode | string | Yes | 14-digit school code |
| SchoolName | string | Yes | School name (derived from ECS) |

**Relationships:**
- Belongs to one Student
- Has one LEA Point of Contact

---

### 3. Point of Contact (POC)

Contact information for organizational representatives. Reused across multiple contexts.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| POCId | int | Yes | Primary key |
| FirstName | string | Yes | First name |
| LastName | string | Yes | Last name |
| Title | string | No | Job title |
| Email | string | Yes | Email address |
| Phone | string | Yes | Phone number (format: (XXX) XXX-XXXX) |
| POCType | enum | Yes | IHE_CONTACT, LEA_CONTACT, GAA_SIGNER, FISCAL_AGENT, SUPERINTENDENT |

**Usage Contexts:**
- IHE submission contact
- LEA placement contact (per placement)
- GAA authorized signer
- Fiscal agent
- Superintendent

---

### 4. Application

Groups students submitted by an IHE to an LEA partner for a grant cycle.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| ApplicationId | int | Yes | Primary key |
| IHEId | int | Yes | Foreign key to IHE |
| LEAId | int | Yes | Foreign key to LEA |
| GrantCycleId | int | Yes | Foreign key to Grant Cycle |
| Status | enum | Yes | DRAFT, SUBMITTED, APPROVED |
| SubmissionDate | datetime | No | When submitted to LEA/CTC |
| LastModifiedDate | datetime | Yes | Last update timestamp |
| CandidateCount | int | Yes | Number of students in application |
| PendingCount | int | Yes | Students awaiting review |

**Relationships:**
- Belongs to one IHE
- Belongs to one LEA
- Belongs to one Grant Cycle
- Contains one or more Students
- Has one IHE POC
- Has one LEA POC

---

### 5. IHE (Institution of Higher Education)

California postsecondary institution with accredited educator preparation program.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| IHEId | int | Yes | Primary key |
| IHEName | string | Yes | Institution name |
| CDSCode | string | No | CDS code if applicable |

**Relationships:**
- Has many Applications
- Partners with many LEAs
- Has many Students (originated from)

---

### 6. LEA (Local Education Agency)

School district or county office of education that hosts student teacher placements.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| LEAId | int | Yes | Primary key |
| LEAName | string | Yes | District/agency name |
| CDSCode | string | Yes | 7-digit CDS code |
| CountyCode | string | Yes | 2-digit county code |

**Relationships:**
- Has many Applications
- Partners with many IHEs
- Has many Placements
- Has many Batch Submissions
- Has many Disbursement Groups

---

### 7. Grant Program

Top-level program definition (e.g., Student Teacher Stipend Program).

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| ProgramId | int | Yes | Primary key |
| ProgramName | string | Yes | Program name |
| ProgramCode | string | Yes | Short code (e.g., STSP) |
| Description | string | No | Program description |
| LegislativeReference | string | No | Education Code reference |

---

### 8. Grant Cycle

Annual funding cycle for a grant program.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| GrantCycleId | int | Yes | Primary key |
| ProgramId | int | Yes | Foreign key to Grant Program |
| FiscalYear | string | Yes | Fiscal year (e.g., "2025-26") |
| CycleName | string | Yes | Display name |
| StartDate | date | Yes | Cycle start date |
| EndDate | date | Yes | Cycle end date |
| Status | enum | Yes | ACTIVE, CLOSED, ARCHIVED |
| ApproprietedAmount | decimal | Yes | Total budget appropriation |
| EncumberedAmount | decimal | Yes | Funds reserved/committed |
| DisbursedAmount | decimal | Yes | Funds paid out |

**Relationships:**
- Belongs to one Grant Program
- Has many Applications
- Has many Reporting Periods
- Has many Batch Submissions

---

### 9. Batch Submission

Monthly submission of candidates from an LEA to CTC.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| BatchId | int | Yes | Primary key |
| LEAId | int | Yes | Foreign key to LEA |
| GrantCycleId | int | Yes | Foreign key to Grant Cycle |
| SubmissionMonth | string | Yes | Month/year (e.g., "October 2025") |
| SubmissionDate | datetime | Yes | When submitted |
| Status | enum | Yes | DRAFT, SUBMITTED, APPROVED |
| CandidateCount | int | Yes | Number of candidates |
| IHEPartnerCount | int | Yes | Number of IHE partners included |

**Relationships:**
- Belongs to one LEA
- Belongs to one Grant Cycle
- Contains many Students

---

### 10. Reporting Period

Defined period when IHE/LEA reports are due.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| ReportingPeriodId | int | Yes | Primary key |
| GrantCycleId | int | Yes | Foreign key to Grant Cycle |
| PeriodName | string | Yes | Display name (e.g., "Annual Report 2025") |
| DueDate | date | Yes | Report due date |
| OpenDate | date | Yes | When reporting opens |

**Computed Fields:**
- DaysUntilDue: int
- IsOverdue: bool

---

## Reporting Entities

### 11. IHE Report (Completion Report)

Report submitted by IHE on student outcomes.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| ReportId | int | Yes | Primary key |
| StudentId | int | Yes | Foreign key to Student |
| ApplicationId | int | Yes | Foreign key to Application |
| ReportingPeriodId | int | Yes | Foreign key to Reporting Period |
| Status | enum | Yes | DRAFT, SUBMITTED, APPROVED, REVISION_REQUESTED |
| CompletionStatus | enum | Yes | IN_PROGRESS, COMPLETED, DENIED |
| CompletionDate | date | No | Program completion date |
| DenialReason | string | No | Reason if denied/withdrawn |
| GrantProgramHours | int | Yes | Hours in grant program |
| Met500Hours | bool | Yes | Met 500 hour requirement |
| CredentialProgramHours | int | Yes | Hours in credential program |
| Met600Hours | bool | Yes | Met 600 hour requirement |
| HoursNotes | string | No | Additional notes on hours |
| CredentialEarned | bool | No | Whether credential was earned |
| CredentialEarnedDate | date | No | Date credential issued |
| CredentialType | string | No | Type of credential earned |
| SwitchedToIntern | bool | No | Switched to intern credential |
| EmploymentStatus | enum | No | NOT_EMPLOYED, SEEKING, EMPLOYED |
| EmployedInDistrict | bool | No | Employed in partner district |
| EmployedInState | bool | No | Employed in California |
| EmployerName | string | No | Employer name |
| EmploymentStartDate | date | No | Employment start date |
| SchoolSite | string | No | School where employed |
| GradeLevel | string | No | Grade level teaching |
| SubjectArea | string | No | Subject area teaching |
| AdditionalNotes | string | No | Any additional notes |
| DocumentationUrl | string | No | Supporting document URL |
| SubmittedDate | datetime | No | When submitted |
| ConfirmationNumber | string | No | Confirmation number |

---

### 12. LEA Report (Payment Report)

Report submitted by LEA on payment distribution and outcomes.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| ReportId | int | Yes | Primary key |
| StudentId | int | Yes | Foreign key to Student |
| ApplicationId | int | Yes | Foreign key to Application |
| LEAId | int | Yes | Foreign key to LEA |
| GrantCycleId | int | Yes | Foreign key to Grant Cycle |
| Status | enum | Yes | DRAFT, SUBMITTED, APPROVED, REVISION_REQUESTED |
| IsLocked | bool | Yes | Whether report is locked |
| CTCFeedback | string | No | Feedback from CTC reviewer |
| PaymentCategory | enum | Yes | STIPEND, SALARY, TUITION, OTHER |
| PaymentSchedule | enum | Yes | LUMP_SUM, MONTHLY, QUARTERLY, SEMESTER |
| ActualPaymentAmount | decimal | Yes | Amount actually paid |
| FirstPaymentDate | date | No | First payment date |
| FinalPaymentDate | date | No | Final payment date |
| ProgramCompletionStatus | enum | Yes | COMPLETED, IN_PROGRESS, NOT_COMPLETED |
| ProgramCompletionDate | date | No | Completion date |
| CredentialEarnedStatus | enum | Yes | EARNED, IN_PROGRESS, NOT_EARNED |
| CredentialIssueDate | date | No | Credential issue date |
| HiredInDistrict | bool | No | Hired in partner district |
| EmploymentStatus | enum | No | FULL_TIME, PART_TIME, SEEKING, NOT_HIRED |
| EmploymentStartDate | date | No | Employment start date |
| EmployingLEA | string | No | Which LEA hired candidate |
| SchoolSite | string | No | School where employed |
| GradeLevel | string | No | Grade level |
| SubjectArea | string | No | Subject area |
| JobTitle | string | No | Job title |
| PlacementQualityRating | int | No | 1-5 rating |
| PlacementQualityNotes | string | No | Quality notes |
| MentorTeacherName | string | No | Mentor teacher name |
| MentorTeacherFeedback | string | No | Mentor feedback |
| AdditionalNotes | string | No | Additional notes |

---

## Fiscal Entities

### 13. Disbursement Group

Groups approved students for GAA and payment processing.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| GroupId | int | Yes | Primary key |
| LEAId | int | Yes | Foreign key to LEA |
| GrantCycleId | int | Yes | Foreign key to Grant Cycle |
| SubmissionMonth | string | Yes | Submission month |
| SubmissionDate | date | Yes | Batch submission date |
| TotalAmount | decimal | Yes | Total award amount |
| StudentCount | int | Yes | Number of students |
| WorkflowState | enum | Yes | See Disbursement State enum |
| Step1_GAASignedDate | datetime | No | GAA fully signed |
| Step2_POGeneratedDate | datetime | No | PO generated |
| Step3_POUploadedDate | datetime | No | PO uploaded |
| Step4_InvoiceGeneratedDate | datetime | No | Invoice generated |
| Step5_InvoiceSentDate | datetime | No | Invoice sent to accounting |
| Step6_WarrantConfirmedDate | datetime | No | Warrant confirmed |

**Relationships:**
- Belongs to one LEA
- Belongs to one Grant Cycle
- Contains many Students
- Has one GAA
- Has one Invoice

---

### 14. GAA (Grant Award Agreement)

Legal agreement between CTC and LEA for grant funding.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| GAAId | int | Yes | Primary key |
| GroupId | int | Yes | Foreign key to Disbursement Group |
| GrantNumber | string | Yes | Unique grant number |
| GranteeName | string | Yes | LEA name |
| AgreementTerm | string | Yes | Agreement term description |
| TotalAmount | decimal | Yes | Total award amount |
| StudentTeacherCount | int | Yes | Number of students |
| SubmissionMonth | string | Yes | Submission month |
| GranteeSignerName | string | Yes | LEA signer name |
| GranteeSignerEmail | string | Yes | LEA signer email |
| GranteeSignedDate | datetime | No | When LEA signed |
| CTCSignerName | string | No | CTC signer name |
| CTCSignedDate | datetime | No | When CTC signed |
| AccountingSignerName | string | No | Accounting officer name |
| AccountingSignedDate | datetime | No | When accounting signed |
| DocuSignEnvelopeId | string | No | DocuSign envelope ID |
| Status | enum | Yes | DRAFT, SENT, PARTIALLY_SIGNED, FULLY_EXECUTED |

---

### 15. Invoice

Invoice for grant payment processing.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| InvoiceId | int | Yes | Primary key |
| GroupId | int | Yes | Foreign key to Disbursement Group |
| PONumber | string | No | Purchase order number |
| InvoiceAmount | decimal | Yes | Invoice amount |
| GeneratedDate | datetime | Yes | When generated |
| SentToFiSCalDate | datetime | No | When sent to accounting |
| Status | enum | Yes | DRAFT, GENERATED, SENT, PAID |

---

### 16. Document

Uploaded or generated documents.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| DocumentId | int | Yes | Primary key |
| GroupId | int | No | Foreign key to Disbursement Group |
| DocumentType | enum | Yes | GAA, PO, INVOICE, WARRANT, SUPPORTING |
| FileName | string | Yes | Original file name |
| FileUrl | string | Yes | Storage URL |
| UploadedDate | datetime | Yes | Upload timestamp |
| UploadedBy | string | Yes | User who uploaded |

---

## Supporting Entities

### 17. Action Item

Dashboard action items for users.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| ActionItemId | int | Yes | Primary key |
| UserId | int | No | Target user (null = org-wide) |
| OrganizationId | int | No | Target organization |
| Title | string | Yes | Action item title |
| Description | string | No | Detailed description |
| ActionType | enum | Yes | SUBMIT_CANDIDATES, REPORTS_DUE, REVIEW_PENDING, etc. |
| DueDate | date | No | Due date if applicable |
| Priority | enum | Yes | CRITICAL, HIGH, NORMAL |
| LinkUrl | string | No | Action URL |

---

### 18. Validation Error

Bulk upload validation errors.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| RowNumber | int | Yes | Excel row number |
| Field | string | Yes | Field with error |
| ErrorMessage | string | Yes | Error description |

---

### 19. Upload Result

Bulk upload processing result.

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| TotalRows | int | Yes | Total rows processed |
| SuccessCount | int | Yes | Valid rows |
| ErrorCount | int | Yes | Invalid rows |
| HasErrors | bool | Yes | Any errors present |
| Errors | ValidationError[] | No | List of errors |

---

## Enumerations

### Credential Area
- Multiple Subject
- Single Subject (Math)
- Single Subject (Science)
- Single Subject (English)
- Single Subject (History)
- Education Specialist
- Bilingual Authorization
- Administrative Services
- PK-3 Early Childhood Education Specialist

### Student Status
- PENDING
- UNDER_REVIEW
- APPROVED
- WAITLIST
- PAID

### Application Status
- DRAFT
- SUBMITTED
- APPROVED

### Report Status
- NOT_STARTED
- DRAFT
- IN_PROGRESS
- SUBMITTED
- APPROVED
- REVISION_REQUESTED

### Disbursement State
- NEEDS_GAA
- NEEDS_PO
- NEEDS_INVOICE
- NEEDS_WARRANT
- COMPLETE

### Completion Status
- IN_PROGRESS
- COMPLETED
- DENIED
- NOT_COMPLETED

### Employment Status
- NOT_EMPLOYED
- SEEKING
- EMPLOYED
- FULL_TIME
- PART_TIME
- NOT_HIRED

### Payment Category
- STIPEND
- SALARY
- TUITION
- OTHER

### Payment Schedule
- LUMP_SUM
- MONTHLY
- QUARTERLY
- SEMESTER

### Race
- American Indian or Alaska Native
- Asian
- Black or African American
- Native Hawaiian or Pacific Islander
- White
- Two or More Races

### POC Type
- IHE_CONTACT
- LEA_CONTACT
- GAA_SIGNER
- FISCAL_AGENT
- SUPERINTENDENT

---

## Entity Relationships Diagram

```
GrantProgram (1) ──────< GrantCycle (N)
                              │
                              ├──< ReportingPeriod (N)
                              │
                              ├──< Application (N)
                              │         │
                              │         ├── IHE (1)
                              │         ├── LEA (1)
                              │         └──< Student (N)
                              │                  │
                              │                  ├──< Placement (1-2)
                              │                  │         └── POC (1)
                              │                  │
                              │                  ├──< IHEReport (N)
                              │                  └──< LEAReport (N)
                              │
                              ├──< BatchSubmission (N)
                              │         │
                              │         └── LEA (1)
                              │
                              └──< DisbursementGroup (N)
                                        │
                                        ├── LEA (1)
                                        ├── GAA (1)
                                        ├── Invoice (1)
                                        ├──< Document (N)
                                        └──< Student (N)
```

---

*Document generated from UI mockup analysis - December 2025*
