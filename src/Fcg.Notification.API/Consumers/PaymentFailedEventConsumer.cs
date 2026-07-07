using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Notification.Application.UseCase.RejectPaymentEmail;
using MassTransit;

namespace Fcg.Notification.API.Consumers.RejectPaymentEmail
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly SendPaymentRejectUseCase _useCase;

        public PaymentFailedEventConsumer(SendPaymentRejectUseCase useCase)
        {
            _useCase = useCase;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var mensagem = context.Message;

            var command = new SendPaymentRejectCommand(context.MessageId ?? Guid.NewGuid(), OrderId: mensagem.OrderId,
                UserId: mensagem.UserId,Reason: mensagem.Reason);

            await _useCase.ExecuteAsync(command, context.CancellationToken);
        }
    }
}
