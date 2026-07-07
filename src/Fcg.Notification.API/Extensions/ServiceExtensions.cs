using Fcg.Notification.API.Consumers;
using Fcg.Notification.API.Consumers.RejectPaymentEmail;
using Fcg.Notification.Application.Client;
using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Application.UseCase.ApprovedPaymentEmail;
using Fcg.Notification.Application.UseCase.DeliveryFailedEmail;
using Fcg.Notification.Application.UseCase.RejectPaymentEmail;
using Fcg.Notification.Application.UseCase.WelcomeEmail;
using Fcg.Notification.Infrastructure.Caching;
using Fcg.Notification.Infrastructure.Idempotency;
using Fcg.Notification.Infrastructure.Services;
using MassTransit;
using Serilog;
using StackExchange.Redis;

namespace Fcg.Notification.API.Extensions
{
    public static class ServiceExtensions
    {
        public static WebApplicationBuilder AddServicesExtensions(this WebApplicationBuilder builder)
        {
            builder.InternalClientExtension()
                .SerilogExtension()
                .HealthCheckExtension()
                .MassTransitExtension()
                .RedisExtension()
                .DependecyInjectionExtension();
            return builder;
        }
        private static WebApplicationBuilder HealthCheckExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks().AddRedis(builder.Configuration.GetConnectionString("Redis"),
            name: "redis-healthcheck");
            return builder;
        }

        private static WebApplicationBuilder InternalClientExtension(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient<IUserApiClient, UserApiClient>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["UserApi:BaseUrl"]);
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("x-internal-api-key", builder.Configuration["InternalSecrets:ServiceApiKey"]);
            });

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
            builder.Services.AddScoped<SendPaymentApprovedEmailUseCase>();
            builder.Services.AddScoped<SendWelcomeEmailUseCase>();
            builder.Services.AddScoped<SendPaymentRejectUseCase>();
            builder.Services.AddScoped<SendDeliveryFailedEmailUseCase>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IUserProfileIntegrationService, UserProfileIntegrationService>();
            builder.Services.AddScoped<IIdempotencyService, RedisIdempotencyService>();
            return builder;
        }
        private static WebApplicationBuilder MassTransitExtension(this WebApplicationBuilder builder)
        {

            builder.Services.AddMassTransit(x =>
            {

                x.AddConsumers(typeof(PaymentFailedEventConsumer).Assembly);
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
                    cfg.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });
                    cfg.ReceiveEndpoint("notifications-user-created", e =>
                    {
                        e.ConfigureConsumer<UserCreatedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("notifications-payment-failed", e =>
                    {
                        e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("notifications-payment-processed", e =>
                    {
                        e.ConfigureConsumer<PaymentProcessedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("notification-delivery-failed", e =>
                    {
                        e.ConfigureConsumer<DeliveryFailedEventConsumer>(context);
                    });

                });
            });
            return builder;
        }
        private static WebApplicationBuilder RedisExtension(this WebApplicationBuilder builder)
        {
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
            return builder;
        }
    }
}
