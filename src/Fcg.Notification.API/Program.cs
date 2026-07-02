using Fcg.Notification.API.Consumers;
using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Application.UseCase.ApprovedPaymentEmail;
using Fcg.Notification.Application.UseCase.WelcomeEmail;
using Fcg.Notification.Infrastructure.Caching;
using Fcg.Notification.Infrastructure.Idempotency;
using Fcg.Notification.Infrastructure.Services;
using MassTransit;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));

        cfg.ReceiveEndpoint("notification-payment-processed-queue", e =>
        {
            e.ConfigureConsumer<PaymentProcessedEventConsumer>(context);
        });

        cfg.ReceiveEndpoint("notification-user-created-queue", e =>
        {
            e.ConfigureConsumer<UserCreatedEventConsumer>(context);
        });

    });

    


});

builder.Services.AddScoped<SendPaymentApprovedEmailUseCase>();
builder.Services.AddScoped<SendWelcomeEmailUseCase>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IIdempotencyService, RedisIdempotencyService>();
var redisSection = builder.Configuration.GetSection("Redis");
builder.Services.Configure<RedisOptions>(redisSection);
var redisConfig = redisSection.Get<RedisOptions>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConfig.Configuration, true);
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConfig.Configuration;
    options.InstanceName = redisConfig.InstanceName;
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();