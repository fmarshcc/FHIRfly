using FhirFly.Core;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirFly.Fhir;

/// <summary>
/// Parses FHIR R4 JSON (which also enforces cardinality and types) and runs
/// form-specific checks. Profile validation with the
/// Firely validator comes in a later milestone.
/// </summary>
public sealed class FhirMessageValidator
{
    private readonly FhirJsonDeserializer _deserializer = new();

    public ValidationResult Validate(string json)
    {
        Resource resource;
        try
        {
            resource = _deserializer.Deserialize<Resource>(json);
        }
        catch (DeserializationFailedException ex)
        {
            return new ValidationResult("unknown", ex.Exceptions
                .Select(e => new ValidationIssue(IssueSeverity.Error, e.Message))
                .ToList());
        }
        catch (System.Text.Json.JsonException ex)
        {
            return ValidationResult.Failed("unknown", ex.Message);
        }

        var issues = new List<ValidationIssue>();
        if (resource is QuestionnaireResponse qr)
        {
            if (string.IsNullOrEmpty(qr.Questionnaire))
                issues.Add(new(IssueSeverity.Warning, "QuestionnaireResponse.questionnaire is missing, so the form cannot be rendered.", "QuestionnaireResponse.questionnaire"));
        }

        return new ValidationResult(resource.TypeName, issues);
    }
}
