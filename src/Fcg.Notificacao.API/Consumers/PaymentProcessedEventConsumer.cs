using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;

namespace Fcg.Notificacao.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {
        public Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
