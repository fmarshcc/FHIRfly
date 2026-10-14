using FhirFly.Hl7;

namespace FhirFly.Tests;

public class Hl7MessageValidatorTests
{
    private const string Oru = """
        MSH|^~\&|FHIRFLY|CLINIC|LAB|HOSP|20261014120000||ORU^R01|MSG0001|P|2.5.1
        PID|1||12345^^^CLINIC^MR||DOE^JANE||19800101|F
        OBR|1||ORD1|INTAKE^Intake form
        OBX|1|ST|Q1^Reason for visit||Headache||||||F
        """;

    private readonly Hl7MessageValidator _validator = new();

    [Fact]
    public void ValidOruPasses()
    {
        var result = _validator.Validate(Oru);

        Assert.True(result.IsValid, string.Join("; ", result.Issues.Select(i => i.Message)));
        Assert.Equal("ORU^R01", result.MessageType);
    }

    [Fact]
    public void MissingControlIdIsAnError()
    {
        var result = _validator.Validate(Oru.Replace("|MSG0001|", "||"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, i => i.Location == "MSH-10");
    }

    [Fact]
    public void GarbageFails()
    {
        Assert.False(_validator.Validate("hello").IsValid);
    }
}
