using FhirFly.Fhir;
using FhirFly.Hl7;
using FhirFly.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<FhirMessageValidator>();
builder.Services.AddSingleton<Hl7MessageValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHealthChecks("/health");

var api = app.MapGroup("/api/validate");
api.MapPost("/fhir", async (HttpRequest request, FhirMessageValidator validator) =>
    Results.Ok(validator.Validate(await ReadBodyAsync(request))));
api.MapPost("/hl7", async (HttpRequest request, Hl7MessageValidator validator) =>
    Results.Ok(validator.Validate(await ReadBodyAsync(request))));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task<string> ReadBodyAsync(HttpRequest request)
{
    using var reader = new StreamReader(request.Body);
    return await reader.ReadToEndAsync();
}

public partial class Program;
