using FhirFly.Core;
using FhirFly.Fhir;

namespace FhirFly.Tests;

public class FhirMessageValidatorTests
{
    private readonly FhirMessageValidator _validator = new();

    [Fact]
    public void ValidQuestionnaireResponsePasses()
    {
        var result = _validator.Validate("""
            {"resourceType":"QuestionnaireResponse","status":"completed","questionnaire":"Questionnaire/intake"}
            """);

        Assert.True(result.IsValid);
        Assert.Equal("QuestionnaireResponse", result.MessageType);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void MissingStatusIsAnError()
    {
        var result = _validator.Validate("""{"resourceType":"QuestionnaireResponse","questionnaire":"Questionnaire/intake"}""");

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, i => i.Message.Contains("status"));
    }

    [Fact]
    public void MissingQuestionnaireIsAWarning()
    {
        var result = _validator.Validate("""{"resourceType":"QuestionnaireResponse","status":"completed"}""");

        Assert.True(result.IsValid);
        Assert.Contains(result.Issues, i => i.Severity == IssueSeverity.Warning);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("""{"resourceType":"NotAResource"}""")]
    public void UnparseableInputFails(string json)
    {
        Assert.False(_validator.Validate(json).IsValid);
    }
}
