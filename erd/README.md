# GMS Entity Relationship Diagram

**California Commission on Teacher Credentialing**
**Grant Management System - Version 1.0**
**December 2025**

## How to View

Open `GMS-ERD.mermaid` in [mermaid.live](https://mermaid.live) to render the diagram.

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
Standard SVT fields:
- `Id` (PK)
- `Code` (UK)
- `Name`
- `Description`
- `DisplayOrder`

### Naming Conventions
- **DB-110**: DateTime fields end with "On" (e.g., `CreatedOn`, `ApprovedOn`)
- **DB-108**: Bit fields start with `Is`, `Has`, or `Can` (e.g., `IsActive`, `HasCredentialEarned`)

---

## Terminology Mapping

| Database Term | UI Term |
|---------------|---------|
| APPLICATION | Partnership |
| BATCH | Submission |
| CANDIDATE | Candidate |

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

---

## Table Count Summary

| Section | Tables |
|---------|--------|
| Core Configuration | 2 |
| Organizations & Accounts | 4 |
| Applications | 1 |
| Candidates | 3 |
| Batches (Submissions) | 5 |
| Disbursements | 4 |
| Reporting | 4 |
| Files | 2 |
| Notifications | 2 |
| Audit | 1 |
| **Total** | **28** |
