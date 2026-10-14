# FHIR Fly — MVP Spec (draft)

Source brief: `FHIR Fly.docx`. This draft narrows that brief to a first shippable slice.

## Goal

Show the core loop end to end: **build a form → send it as FHIR or HL7 v2 → receive a message → validate it → display it as a filled form.**

## In scope

| # | Capability | Detail |
|---|---|---|
| 1 | Form builder | Create/edit forms stored as FHIR R4 `Questionnaire`. Field types: text, number, date, choice, boolean. |
| 2 | Form fill | Render a `Questionnaire`, capture a `QuestionnaireResponse`. |
| 3 | Outbound FHIR | POST the `QuestionnaireResponse` (or a transaction Bundle) to a configured FHIR REST endpoint. |
| 4 | Outbound HL7 v2 | Map the response to an HL7 v2.5.1 message (ORU^R01 with OBX per answer) and send over MLLP. |
| 5 | Inbound FHIR | REST endpoint accepting `QuestionnaireResponse` / Bundle. |
| 6 | Inbound HL7 v2 | MLLP listener; returns ACK/NAK. |
| 7 | Validation | FHIR: structural + profile validation (Firely validator). HL7 v2: parse + required-segment/field checks (NHapi). Errors shown to the user and returned in the ACK/OperationOutcome. |
| 8 | Message → form | Render an inbound message as a read-only filled form, using the linked `Questionnaire`. |
| 9 | Connections | CRUD for inbound/outbound connections (type, host/URL, port, auth). |
| 10 | Message log | Store every message (raw + parsed) with status. Optional per-connection "strip PHI before storing". |
| 11 | Auth | OAuth2 / OIDC login (e.g. Auth0, Entra ID, Cognito). Single tenant to start. |

## Out of scope for MVP

- Multi-tenant hosting and white labeling (designed for, not built)
- Billing and free/paid tiers
- De-identified data sharing program (needs legal review first)
- HL7 v2 message types other than ORU^R01 / ADT^A04 inbound
- SFTP, Direct, X12

## Proposed stack

- **Backend:** ASP.NET Core (.NET 8 LTS or later), C#
- **FHIR:** Firely .NET SDK (`Hl7.Fhir.R4`) + Firely validator
- **HL7 v2:** NHapi; custom MLLP listener on `TcpListener`
- **UI:** Blazor Server to start (keeps the team in C#); LHC-Forms can be embedded later for richer form rendering
- **DB:** PostgreSQL via EF Core (SQL Server also fine given team skills)
- **De-identification:** rule-based Safe Harbor stripping in C# first; Python service later if needed
- **Hosting (later):** AWS Fargate + RDS + NLB (for MLLP TCP) under an AWS BAA

## Repo layout (proposed)

```
src/
  FhirFly.Web/          Blazor UI + REST API
  FhirFly.Core/         domain: forms, connections, messages
  FhirFly.Fhir/         Firely wrappers, validation
  FhirFly.Hl7/          NHapi mapping, MLLP listener/sender
  FhirFly.Data/         EF Core
tests/
  FhirFly.Tests/
docker-compose.yml      app + Postgres + a test FHIR server (HAPI)
```

## Milestones

1. **Skeleton:** solution, Docker compose, CI build, health check.
2. **Round trip:** hard-coded Questionnaire → FHIR POST to local HAPI server; MLLP send/receive with ACK.
3. **Builder + validation:** form builder UI, inbound validation, message → filled form.
4. **Connections + log + PHI strip + login.**

## Open questions for Fred

- FHIR R4 only, or R5 too?
- Postgres or SQL Server?
- Which HL7 v2 message types do your first users actually send?
- Preferred identity provider for login?
