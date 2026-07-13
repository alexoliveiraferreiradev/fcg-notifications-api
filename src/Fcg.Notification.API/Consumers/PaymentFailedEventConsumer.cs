using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.UseCase.RejectPaymentEmail;
using MassTransit;

namespace Fcg.Notification.API.Consumers.RejectPaymentEmail
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly ISendPaymentRejectUseCase _sendPaymentRejectUseCase;

        public PaymentFailedEventConsumer(ISendPaymentRejectUseCase sendPaymentRejectUseCase)
        {
            _sendPaymentRejectUseCase = sendPaymentRejectUseCase;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var mensagem = context.Message;

            var command = new SendPaymentRejectCommand(
                EventId: mensagem.EventId, 
                OrderId: mensagem.OrderId,
                UserId: mensagem.UserId, 
                Reason: mensagem.Reason);

            await _sendPaymentRejectUseCase.ExecuteAsync(command, context.CancellationToken);
        }
    }
}
