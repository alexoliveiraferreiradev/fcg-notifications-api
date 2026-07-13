using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.UseCase.ApprovedPaymentEmail;
using MassTransit;

namespace Fcg.Notification.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly ISendPaymentApprovedEmailUseCase _sendPaymentApprovedEmailUseCase;

        public PaymentProcessedEventConsumer(ISendPaymentApprovedEmailUseCase sendPaymentApprovedEmailUseCase)
        {
            _sendPaymentApprovedEmailUseCase = sendPaymentApprovedEmailUseCase;
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var mensagem = context.Message;

            var command = new SendPaymentApprovedEmailCommand(context.MessageId ?? Guid.NewGuid(),UsuarioId: mensagem.UserId,
                OrderId: mensagem.OrderId,CreatedAt: mensagem.CreatedAt);

            await _sendPaymentApprovedEmailUseCase.ExecuteAsync(command,context.CancellationToken);
        }
    }
}
