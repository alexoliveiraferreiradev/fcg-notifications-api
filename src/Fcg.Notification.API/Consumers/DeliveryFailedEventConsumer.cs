using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Notification.Application.UseCase.DeliveryFailedEmail;
using MassTransit;

namespace Fcg.Notification.API.Consumers
{
    public class DeliveryFailedEventConsumer : IConsumer<DeliveryFailedEvent>
    {
        private readonly SendDeliveryFailedEmailUseCase _useCase;

        public DeliveryFailedEventConsumer(SendDeliveryFailedEmailUseCase useCase)
        {
            _useCase = useCase;
        }

        public async Task Consume(ConsumeContext<DeliveryFailedEvent> context)
        {
            var mensagem = context.Message;

            var command = new SendDeliveryFailedEmailCommand(context.MessageId ?? Guid.NewGuid(), OrderId: mensagem.OrderId,
                 UserId: mensagem.UserId,Reason: mensagem.Reason);

            await _useCase.ExecuteAsync(command, context.CancellationToken);
        }
    }
}
