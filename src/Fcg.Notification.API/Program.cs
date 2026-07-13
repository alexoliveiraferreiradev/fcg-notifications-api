using Fcg.Notification.API.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServicesExtensions();    

var app = builder.Build();
app.UseSerilogRequestLogging(); 
app.MapHealthChecks("/health/liveness", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});


app.MapHealthChecks("/health/readiness", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.Run();