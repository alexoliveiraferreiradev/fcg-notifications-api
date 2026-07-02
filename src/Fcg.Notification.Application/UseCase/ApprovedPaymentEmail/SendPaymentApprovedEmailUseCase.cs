using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Domain.Entities;
using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;

namespace Fcg.Notification.Application.UseCase.ApprovedPaymentEmail
{
    public class SendPaymentApprovedEmailUseCase
    {
        private readonly IEmailService _emailService;
        private readonly IIdempotencyService _idempotencyService;

        public SendPaymentApprovedEmailUseCase(IEmailService emailService, IIdempotencyService idempotencyService)
        {
            _emailService = emailService;
            _idempotencyService = idempotencyService;
        }

        public async Task ExecuteAsync(SendPaymentApprovedEmailCommand command, CancellationToken cancellationToken)
        {
            if (await _idempotencyService.HasBeenProcessedAsync(command.EventId))
                return;

                var emailRecipient = EmailAddress.Create(command.Email);

                var notification = new Domain.Entities.Notification(emailRecipient, NotificationType.OrderConfirmation);

            try
            {
                var subject = $"Pagamento aprovado com sucesso para o pedido {command.OrderId}!";
                var body = $"Olá {command.NomeUsuario}, o seu pagamento foi aprovado com sucesso." +
                           $"ID do Usuário: {command.UsuarioId}" +
                           $"Data Aquisição: {command.DataAquisicao}";

                await _emailService.SendEmailAsync(notification.Recipient, subject, body, cancellationToken);

                notification.MarkAsSent();

                await _idempotencyService.MarkAsProcessedAsync(command.EventId);
            }
            catch (Exception ex)
            {
                notification.MarkAsFailure(ex.Message);
                throw;
            }
        }
    }
}
