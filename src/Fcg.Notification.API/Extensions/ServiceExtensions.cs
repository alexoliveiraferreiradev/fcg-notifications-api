using Fcg.Core.Abstractions.Resources;
using Fcg.Notification.API.Consumers;
using Fcg.Notification.API.Consumers.RejectPaymentEmail;
using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Application.UseCase.ApprovedPaymentEmail;
using Fcg.Notification.Application.UseCase.DeliveryFailedEmail;
using Fcg.Notification.Application.UseCase.RejectPaymentEmail;
using Fcg.Notification.Application.UseCase.WelcomeEmail;
using Fcg.Notification.Infrastructure.Caching;
using Fcg.Notification.Infrastructure.Idempotency;
using Fcg.Notification.Infrastructure.MessageBroker;
using Fcg.Notification.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;
using StackExchange.Redis;

namespace Fcg.Notification.API.Extensions
{
    public static class ServiceExtensions
    {
        public static WebApplicationBuilder AddServicesExtensions(this WebApplicationBuilder builder)
        {
            builder
                .SerilogExtension()
                .HealthCheckExtension()
                .MassTransitExtension()
                .RedisExtension()
                .DependecyInjectionExtension();
            return builder;
        }
        private static WebApplicationBuilder HealthCheckExtension(this WebApplicationBuilder builder)
        {           
            var redisConfig = builder.Configuration.GetSection(RedisSettings.RedisSectionName).Get<RedisSettings>();
            var connectionString = redisConfig != null && !string.IsNullOrEmpty(redisConfig.Host)
                ? $"{redisConfig.Host}:{redisConfig.Port},password={redisConfig.Password}"
                : builder.Configuration.GetConnectionString("Redis")!;

            builder.Services.AddHealthChecks()
                .AddRedis(
                    connectionString,
                    name: "redis-healthcheck",
                    tags: new[] {"ready"});

            return builder;
        }

        private static WebApplicationBuilder SerilogExtension(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
                configuration.MinimumLevel.Information();
                configuration.Filter.ByExcluding(logEvent => logEvent.Exception is HttpRequestException &&
                logEvent.Exception.Message.Contains("Nenhuma conexão pôde ser feita"));
            });

            return builder;
        }
        private static WebApplicationBuilder DependecyInjectionExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ISendPaymentApprovedEmailUseCase,SendPaymentApprovedEmailUseCase>();
            builder.Services.AddScoped<ISendWelcomeEmailUseCase,SendWelcomeEmailUseCase>();
            builder.Services.AddScoped<ISendPaymentRejectUseCase,SendPaymentRejectUseCase>();
            builder.Services.AddScoped<ISendDeliveryFailedEmailUseCase,SendDeliveryFailedEmailUseCase>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IUserProfileIntegrationService, UserProfileIntegrationService>();
            builder.Services.AddScoped<IIdempotencyService, RedisIdempotencyService>();
            return builder;
        }
        private static WebApplicationBuilder MassTransitExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddOptions<RabbitMqSettings>().BindConfiguration(RabbitMqSettings.SectionName)
           .ValidateDataAnnotations().ValidateOnStart();
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumers(typeof(PaymentFailedEventConsumer).Assembly);
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfig = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                    cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.Port, "/", h =>
                    {
                        h.Username(rabbitMqConfig.Username);
                        h.Password(rabbitMqConfig.Password);
                    });


                    cfg.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });

                    cfg.ReceiveEndpoint(rabbitMqConfig.NotificationUserCreatedQueue, e =>
                    {
                        e.ConfigureConsumer<UserCreatedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(rabbitMqConfig.NotificationPaymentFailedQueue, e =>
                    {
                        e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(rabbitMqConfig.NotificationPaymentProcessedQueue, e =>
                    {
                        e.ConfigureConsumer<PaymentProcessedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(rabbitMqConfig.NotificationDeliveryFailedQueue, e =>
                    {
                        e.ConfigureConsumer<DeliveryFailedEventConsumer>(context);
                    });

                });
            });
            return builder;
        }
        private static WebApplicationBuilder RedisExtension(this WebApplicationBuilder builder)
        {
            var redisConfig = builder.Configuration.GetSection(RedisSettings.RedisSectionName).Get<RedisSettings>();
            ArgumentNullException.ThrowIfNull(redisConfig, nameof(RedisSettings));
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(RedisSettings.RedisSectionName));

            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { { redisConfig.Host, redisConfig.Port } },
                Password = redisConfig.Password,
                AbortOnConnectFail = false,
                ConnectRetry = 5,
                ReconnectRetryPolicy = new ExponentialRetry(5000, 30000)
            };

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = configurationOptions;
                options.InstanceName = redisConfig.InstanceName;
            });
            return builder;
        }
    }
}
