# GMS Entity Relationship Diagrams

Use a Mermaid renderer (VS Code extension, GitHub, or https://mermaid.live) to view these diagrams.

---

## 1. High-Level System ERD

```mermaid
erDiagram
    %% Core Configuration
    GRANT_PROGRAM ||--o{ GRANT_CYCLE : "has many"

    %% Grant Cycle Relationships
    GRANT_CYCLE ||--o{ APPLICATION : "contains"
    GRANT_CYCLE ||--o{ SUBMISSION : "receives"
    GRANT_CYCLE ||--o{ REPORTING_PERIOD : "defines"

    %% Organization Relationships
    ORGANIZATION ||--o{ APPLICATION : "IHE initiates"
    ORGANIZATION ||--o{ APPLICATION : "LEA completes"
    ORGANIZATION ||--o{ ACCOUNT : "employs"
    ORGANIZATION ||--o{ SUBMISSION : "submits"

    %% Application & Candidate
    APPLICATION ||--o{ CANDIDATE : "includes"

    %% Submission Processing
    SUBMISSION ||--o{ SUBMISSION_CANDIDATE : "contains"
    SUBMISSION_CANDIDATE }o--|| CANDIDATE : "references"
    SUBMISSION ||--|| DISBURSEMENT : "triggers"

    %% Disbursement & Files
    DISBURSEMENT ||--o{ FILE : "has documents"

    %% Reporting
    CANDIDATE ||--o{ CANDIDATE_REPORT : "submits"
    REPORTING_PERIOD ||--o{ CANDIDATE_REPORT : "collects"
    CANDIDATE_REPORT ||--o{ FILE : "attaches"

    %% Audit Trail
    CANDIDATE ||--o{ CANDIDATE_STATUS_CHANGE : "tracks"
    SUBMISSION ||--o{ SUBMISSION_STATUS_CHANGE : "tracks"

    %% Notifications
    ACCOUNT ||--o{ NOTIFICATION : "receives"
```

---

## 2. Core Configuration Domain

```mermaid
erDiagram
    GRANT_PROGRAM {
        int Id PK
        string Code UK "e.g., STSP"
        string Name
        string Description
        boolean IsActive
        json EligibilityRulesSchemaJson "V2: DSL"
        datetime CreatedOn
    }

    GRANT_CYCLE {
        int Id PK
        int GrantProgramId FK
        string Name
        string FiscalYear
        decimal AppropriatedAmount
        decimal DefaultAwardAmount
        decimal MinAwardAmount
        decimal MaxAwardAmount
        datetime ApplicationOpenOn
        datetime ApplicationCloseOn
        datetime CycleStartOn
        datetime CycleEndOn
        string Status "DRAFT, OPEN, CLOSED"
        datetime CreatedOn
    }

    REPORTING_PERIOD {
        int Id PK
        int GrantCycleId FK
        int ReportTypeId FK
        string Name "e.g., Q1 2025"
        datetime StartOn
        datetime EndOn
        datetime DueOn
        boolean IsRequired
        datetime CreatedOn
    }

    GRANT_PROGRAM ||--o{ GRANT_CYCLE : "configures"
    GRANT_CYCLE ||--o{ REPORTING_PERIOD : "defines"
```

---

## 3. Organization & User Domain

```mermaid
erDiagram
    ORGANIZATION_TYPE_SVT {
        int Id PK
        string Name "IHE, LEA, COE, CTC"
        string Description
        int DisplayOrder
    }

    ORGANIZATION {
        int Id PK
        int TypeId FK
        string EcsCdsCode UK "CDS Code from ECS"
        int EcsOrganizationId "ECS sync reference"
        string Name
        string StreetAddress
        string City
        string StateProvince
        string PostalCode
        string PrimaryEmailAddress
        string EmailDomain
        boolean IsActive
        datetime LastEcsSyncOn
        datetime CreatedOn
    }

    USER_ROLE_SVT {
        int Id PK
        string Code UK "IHE_ADMIN, LEA_REVIEWER"
        string Name
        string Description
        int DisplayOrder
    }

    ACCOUNT {
        int Id PK
        int OrganizationId FK
        int RoleId FK
        string EmailAddress UK
        string FirstName
        string LastName
        string PhoneNumber
        string Title
        string EntraId "CTC staff SSO"
        string OAuthProvider "google, microsoft"
        boolean IsActive
        datetime LastLoginOn
        datetime CreatedOn
    }

    ORGANIZATION_TYPE_SVT ||--o{ ORGANIZATION : "categorizes"
    ORGANIZATION ||--o{ ACCOUNT : "employs"
    USER_ROLE_SVT ||--o{ ACCOUNT : "assigns"
```

---

## 4. Application & Candidate Domain

```mermaid
erDiagram
    APPLICATION {
        int Id PK
        int GrantCycleId FK
        int IHEOrgId FK "Initiator"
        int LEAOrgId FK "Completer"
        int InitiatorContactId FK
        int CompleterContactId FK
        string GAASignerName
        string GAASignerEmail
        json ExtendedContactsJson
        string Status "DRAFT, ACTIVE, CLOSED"
        datetime CreatedOn
    }

    CANDIDATE_STATUS_SVT {
        int Id PK
        string Code UK "DRAFT, LEA_REVIEWING, APPROVED"
        string Name
        string Category "SUBMISSION, REVIEW, DISBURSEMENT"
        string LEADisplayCategory "Simplified view"
        boolean IsTerminal
        json AllowedTransitionsJson
    }

    CANDIDATE {
        int Id PK
        int ApplicationId FK
        int StatusId FK
        string SEID UK "Educator ID from ECS"
        string FirstName
        string LastName
        string Last4SSN "Privacy: only last 4"
        string CredentialArea
        string SchoolCdsCode
        string SchoolName
        decimal RequestedAmount
        decimal AwardedAmount
        int WaitlistPosition "Gap: add to schema"
        datetime WaitlistAddedOn "Gap: add to schema"
        datetime SubmittedByInitiatorOn
        datetime SubmittedByCompleterOn
        datetime SubmittedToCTCOn
        datetime ApprovedOn
        int ApprovedByAccountId FK
        string RejectionReason
        datetime CreatedOn
    }

    APPLICATION ||--o{ CANDIDATE : "includes"
    CANDIDATE_STATUS_SVT ||--o{ CANDIDATE : "tracks status"
```

---

## 5. Submission & Disbursement Domain

> **Note:** Database uses `Batch` but UI displays as "Submission"

```mermaid
erDiagram
    SUBMISSION_STATUS_SVT {
        int Id PK
        string Code UK "DRAFT, PENDING_REVIEW, GAA_SENT"
        string Name
        string Category
        boolean IsTerminal
    }

    SUBMISSION {
        int Id PK "DB: Batch"
        int GrantCycleId FK
        int StatusId FK
        int SubmittingOrgId FK "LEA"
        string SubmissionNumber "DB: BatchNumber"
        string SubmissionPeriod "2025-11"
        int CandidateCount "Denormalized"
        decimal TotalAwardAmount "Denormalized"
        datetime SubmittedOn
        datetime ApprovedOn
        string RejectionReason
        datetime CreatedOn
    }

    SUBMISSION_CANDIDATE {
        int Id PK "DB: BatchCandidate"
        int SubmissionId FK "DB: BatchId"
        int CandidateId FK
        datetime AddedOn
        int AddedByAccountId FK
    }

    DISBURSEMENT_STATUS_SVT {
        int Id PK
        string Code UK
        string Name
    }

    DISBURSEMENT {
        int Id PK
        int SubmissionId FK "DB: BatchId"
        int StatusId FK
        string Phase "FULL, FIRST, SECOND"
        decimal Amount
        string GAAEnvelopeId "DocuSign"
        string GAAStatus
        datetime GAASentOn
        datetime GAASignedOn
        string PONumber
        decimal POAmount
        datetime POIssuedOn
        string InvoiceNumber
        datetime InvoiceSentOn
        datetime WarrantIssuedOn
        datetime CreatedOn
    }

    SUBMISSION ||--o{ SUBMISSION_CANDIDATE : "contains"
    SUBMISSION_CANDIDATE }o--|| CANDIDATE : "references"
    SUBMISSION ||--|| DISBURSEMENT : "triggers"
    SUBMISSION_STATUS_SVT ||--o{ SUBMISSION : "tracks"
    DISBURSEMENT_STATUS_SVT ||--o{ DISBURSEMENT : "tracks"
```

---

## 6. Reporting Domain

```mermaid
erDiagram
    REPORT_TYPE_SVT {
        int Id PK
        string Code UK "IHE_COMPLETION, LEA_PAYMENT"
        string Name
        int ReportingOrgTypeId FK "Who submits"
        json FormSchemaJson "Field definitions"
    }

    REPORTING_PERIOD {
        int Id PK
        int GrantCycleId FK
        int ReportTypeId FK
        string Name
        datetime StartOn
        datetime EndOn
        datetime DueOn
    }

    CANDIDATE_REPORT {
        int Id PK
        int CandidateId FK
        int ReportingPeriodId FK
        int ReportTypeId FK
        int SubmittedByOrgId FK
        int SubmittedByAccountId FK
        string Status "DRAFT, SUBMITTED, APPROVED"
        datetime SubmittedOn
        decimal HoursInProgram
        boolean IsEmployed
        string EmployerName
        boolean CredentialCompleted
        datetime CredentialCompletedOn
        json ExtendedDataJson
        datetime CreatedOn
    }

    FILE {
        int Id PK
        int FileTypeId FK
        string StorageKey UK "Azure Blob reference"
        string OriginalFilename
        string ContentType
        bigint FileSizeBytes
        string ContentHash "SHA-256 integrity"
        int UploadedByAccountId FK
        datetime UploadedOn
    }

    REPORTING_PERIOD ||--o{ CANDIDATE_REPORT : "collects"
    CANDIDATE ||--o{ CANDIDATE_REPORT : "submits"
    REPORT_TYPE_SVT ||--o{ CANDIDATE_REPORT : "defines"
    CANDIDATE_REPORT ||--o{ FILE : "attaches"
```

---

## 7. Proposed Integration Tables (Gap Analysis)

```mermaid
erDiagram
    ECS_SYNC_LOG {
        int Id PK
        string SyncType "ORGANIZATION_FULL, SEID_LOOKUP"
        string EntityType "Organization, Candidate"
        int EntityId FK
        string EcsRecordId
        string SyncStatus "SUCCESS, FAILED, CONFLICT"
        int RecordsProcessed
        string ErrorMessage
        json RequestPayloadJson
        json ResponsePayloadJson
        datetime SyncStartedOn
        datetime SyncCompletedOn
        datetime CreatedOn
    }

    DOCUSIGN_EVENT {
        int Id PK
        string EnvelopeId "DocuSign envelope ID"
        int DisbursementId FK
        string EventType "envelope-sent, recipient-signed"
        datetime EventTimestamp
        string RecipientEmail
        string RecipientName
        string RecipientStatus
        json RawPayloadJson
        datetime ReceivedOn
        datetime ProcessedOn
        string ProcessingStatus "PENDING, PROCESSED, FAILED"
        datetime CreatedOn
    }

    DISBURSEMENT ||--o{ DOCUSIGN_EVENT : "tracks webhooks"
    ORGANIZATION ||--o{ ECS_SYNC_LOG : "sync history"
    CANDIDATE ||--o{ ECS_SYNC_LOG : "SEID lookups"
```

---

## Quick View Instructions

1. **VS Code**: Install "Markdown Preview Mermaid Support" extension
2. **GitHub**: Paste into any `.md` file - renders automatically
3. **Online**: Copy diagram to https://mermaid.live
4. **Docusaurus/MkDocs**: Native Mermaid support

