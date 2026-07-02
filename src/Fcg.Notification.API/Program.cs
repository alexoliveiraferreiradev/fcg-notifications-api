using Fcg.Notification.API.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServicesExtensions();    

var app = builder.Build();
app.MapHealthChecks("/health/liveness", new HealthCheckOptions
{
    Predicate = _ => false
});

// Readiness: Testa tudo (Redis + RabbitMQ).
app.MapHealthChecks("/health/readiness", new HealthCheckOptions
{
    Predicate = _ => true
});
app.Run();