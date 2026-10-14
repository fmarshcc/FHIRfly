# FHIR Fly

A web app for building forms that send FHIR or HL7 v2 messages, and for receiving, validating and displaying inbound messages as filled forms.

See [docs/MVP.md](docs/MVP.md) for scope, stack and milestones.

## Layout

| Project | Purpose |
|---|---|
| `src/FhirFly.Web` | Blazor UI and REST API |
| `src/FhirFly.Core` | Shared domain types |
| `src/FhirFly.Fhir` | FHIR R4 parsing and validation (Firely SDK) |
| `src/FhirFly.Hl7` | HL7 v2 parsing, validation and MLLP framing (NHapi) |
| `tests/FhirFly.Tests` | xUnit tests |

## Run

Requires the .NET 8 SDK.

```sh
dotnet test
dotnet run --project src/FhirFly.Web
```

Then try:

```sh
curl -X POST http://localhost:5037/api/validate/fhir \
  -H "Content-Type: application/fhir+json" \
  -d '{"resourceType":"QuestionnaireResponse","status":"completed"}'
```
