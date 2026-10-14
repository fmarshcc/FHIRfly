using FhirFly.Core;
using NHapi.Base;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;

namespace FhirFly.Hl7;

/// <summary>Parses an HL7 v2 message and checks the MSH fields needed to route and ACK it.</summary>
public sealed class Hl7MessageValidator
{
    private readonly PipeParser _parser = new();

    public ValidationResult Validate(string message)
    {
        IMessage parsed;
        try
        {
            parsed = _parser.Parse(Normalize(message));
        }
        catch (HL7Exception ex)
        {
            return ValidationResult.Failed("unknown", ex.Message);
        }

        var terser = new Terser(parsed);
        var type = $"{terser.Get("/MSH-9-1")}^{terser.Get("/MSH-9-2")}";
        var issues = new List<ValidationIssue>();
        foreach (var (path, name) in new[] { ("/MSH-10", "control ID"), ("/MSH-3", "sending application"), ("/MSH-12", "version") })
        {
            if (string.IsNullOrWhiteSpace(terser.Get(path)))
                issues.Add(new(IssueSeverity.Error, $"MSH {name} is required.", path.TrimStart('/')));
        }

        return new ValidationResult(type, issues);
    }

    /// <summary>HL7 v2 separates segments with CR; accept LF or CRLF from pasted text too.</summary>
    internal static string Normalize(string message) =>
        message.Replace("\r\n", "\r").Replace('\n', '\r').TrimEnd('\r');
}
