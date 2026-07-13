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
            builder.Services.AddHealthChecks()
                .AddRedis(
                    builder.Configuration.GetConnectionString("Redis")!,
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

            builder.Services.AddMassTransit(x =>
            {
                builder.Services.AddOptions<RabbitMqQueuesOptions>().BindConfiguration(RabbitMqQueuesOptions.SectionName)
                .ValidateDataAnnotations().ValidateOnStart();

                x.AddConsumers(typeof(PaymentFailedEventConsumer).Assembly);
                x.UsingRabbitMq((context, cfg) =>
                {
                    var options = builder.Configuration.GetSection(RabbitMqQueuesOptions.SectionName)
                    .Get<RabbitMqQueuesOptions>()!;
                    if (options == null || string.IsNullOrEmpty(options.NotificationUserCreatedQueue))
                    {
                        throw new Exception("Não foi configurado as queues para o rabbitmq");
                    }
                    cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
                    cfg.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });
                    cfg.ReceiveEndpoint(options.NotificationUserCreatedQueue, e =>
                    {
                        e.ConfigureConsumer<UserCreatedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(options.NotificationPaymentFailedQueue, e =>
                    {
                        e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(options.NotificationPaymentProcessedQueue, e =>
                    {
                        e.ConfigureConsumer<PaymentProcessedEventConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(options.NotificationDeliveryFailedQueue, e =>
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
