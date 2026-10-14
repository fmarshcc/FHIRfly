using FhirFly.Hl7;

namespace FhirFly.Tests;

public class MllpTests
{
    [Fact]
    public void FrameRoundTrips()
    {
        var framed = Mllp.Frame("MSH|^~\\&|A");

        Assert.Equal(Mllp.StartBlock, framed[0]);
        Assert.Equal("MSH|^~\\&|A", Mllp.Unframe(framed));
    }

    [Fact]
    public void IncompleteFrameReturnsNull()
    {
        var framed = Mllp.Frame("MSH|^~\\&|A");

        Assert.Null(Mllp.Unframe(framed.AsSpan(0, framed.Length - 1)));
    }
}
