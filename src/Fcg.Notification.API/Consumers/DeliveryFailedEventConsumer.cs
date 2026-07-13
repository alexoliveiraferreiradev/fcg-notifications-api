using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.UseCase.DeliveryFailedEmail;
using MassTransit;

namespace Fcg.Notification.API.Consumers
{
    public class DeliveryFailedEventConsumer : IConsumer<DeliveryFailedEvent>
    {
        private readonly ISendDeliveryFailedEmailUseCase _sendDeliveryFailedEmailUseCase;

        public DeliveryFailedEventConsumer(ISendDeliveryFailedEmailUseCase sendDeliveryFailedEmailUseCase)
        {
            _sendDeliveryFailedEmailUseCase = sendDeliveryFailedEmailUseCase;
        }

        public async Task Consume(ConsumeContext<DeliveryFailedEvent> context)
        {
            var mensagem = context.Message;
            
            var command = new SendDeliveryFailedEmailCommand(
                EventId: mensagem.EventId, 
                OrderId: mensagem.OrderId,
                UserId: mensagem.UserId,
                Reason: mensagem.Reason);

            await _sendDeliveryFailedEmailUseCase.ExecuteAsync(command, context.CancellationToken);
        }
    }
}
