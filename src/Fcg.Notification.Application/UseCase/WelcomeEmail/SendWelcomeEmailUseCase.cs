using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Fcg.Notification.Application.UseCase.WelcomeEmail
{
    public class SendWelcomeEmailUseCase : ISendWelcomeEmailUseCase
    {
        private readonly IEmailService _emailService;
        private readonly IIdempotencyService _idempotencyService;
        private readonly IDistributedCache _cache;
        private readonly ILogger<SendWelcomeEmailUseCase> _logger;
        public SendWelcomeEmailUseCase(IEmailService emailService, IIdempotencyService idempotencyService,
            IDistributedCache cache, ILogger<SendWelcomeEmailUseCase> logger)
        {
            _emailService = emailService;
            _idempotencyService = idempotencyService;
            _cache = cache;
            _logger = logger;
        }

        public async Task ExecuteAsync(SendWelcomeEmailCommand command, CancellationToken cancellationToken)
        {
            if (!await _idempotencyService.TryProcessAsync(command.EventId))
                return;

            try
            {
                var emailRecipient = EmailAddress.Create(command.Email);

                var userData = new { command.UserName, command.Email };
                var json = JsonSerializer.Serialize(userData);
                var cacheKey = $"user:{command.UserId}:profile";

                await _cache.SetStringAsync(cacheKey, json, cancellationToken);

                _logger.LogInformation("[NotificationsAPI] Perfil do usuário {UserId} cacheado com sucesso.", command.UserId);

                var notification = new Domain.Entities.Notification(emailRecipient, NotificationType.Welcome);
                if (emailRecipient.Address != "admin@fiapcloudgames.com.br")
                {

                var (subject, body) = notification.GenerateWelcomeContent(command.UserName);
                    await _emailService.SendEmailAsync(notification.Recipient, subject, body, cancellationToken);
                }

            }
            catch
            {
                await _idempotencyService.ReleaseAsync(command.EventId);

                throw;
            }
        }
    }
}
