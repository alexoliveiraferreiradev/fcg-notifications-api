using Fcg.Core.Abstractions.Enum;
using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Notification.Application.UseCase.ApprovedPaymentEmail;
using MassTransit;

namespace Fcg.Notification.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly SendPaymentApprovedEmailUseCase _useCase;

        public PaymentProcessedEventConsumer(SendPaymentApprovedEmailUseCase useCase)
        {
            _useCase = useCase; 
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var mensagem = context.Message;

            if (mensagem.Status != PaymentStatus.Approved)
                return;

            var command = new SendPaymentApprovedEmailCommand(context.MessageId ?? Guid.NewGuid(),UsuarioId: mensagem.UserId,
                OrderId: mensagem.OrderId, NomeUsuario: mensagem.NomeUsuario, Email: mensagem.EmailUsuario,
                DataAquisicao: mensagem.CreatedAt);

            await _useCase.ExecuteAsync(command,context.CancellationToken);
        }
    }
}
