namespace FhirFly.Core;

[System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter<IssueSeverity>))]
public enum IssueSeverity
{
    Information,
    Warning,
    Error,
}

public sealed record ValidationIssue(IssueSeverity Severity, string Message, string? Location = null);

/// <summary>Outcome of validating an inbound FHIR or HL7 v2 message.</summary>
public sealed record ValidationResult(string MessageType, IReadOnlyList<ValidationIssue> Issues)
{
    public bool IsValid => Issues.All(i => i.Severity != IssueSeverity.Error);

    public static ValidationResult Failed(string messageType, string message, string? location = null) =>
        new(messageType, [new ValidationIssue(IssueSeverity.Error, message, location)]);
}
