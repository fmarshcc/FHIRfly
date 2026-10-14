using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FhirFly.Tests;

public class ApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task HealthIsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ValidateFhirReturnsResult()
    {
        var body = new StringContent("""{"resourceType":"QuestionnaireResponse","status":"completed"}""", Encoding.UTF8, "application/fhir+json");
        var result = await (await _client.PostAsync("/api/validate/fhir", body)).Content.ReadFromJsonAsync<ResultDto>();

        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Equal("QuestionnaireResponse", result.MessageType);
    }

    private sealed record ResultDto(string MessageType, bool IsValid);
}
