using Fcg.Notificacao.Application.Common.Interfaces;
using Fcg.Notificacao.Application.Ports;
using Fcg.Notificacao.Domain.Entities;
using Fcg.Notificacao.Domain.Enum;
using Fcg.Notificacao.Domain.ValueObject;

namespace Fcg.Notificacao.Application.UseCase.ApprovedPaymentEmail
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

                var notification = new Notification(emailRecipient, NotificationType.OrderConfirmation);

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
