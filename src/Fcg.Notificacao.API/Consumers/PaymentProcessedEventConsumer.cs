using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Notificacao.Application.UseCase.ApprovedPaymentEmail;
using MassTransit;

namespace Fcg.Notificacao.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<OrderApprovedEvent>
    {
        private readonly SendPaymentApprovedEmailUseCase _useCase;

        public PaymentProcessedEventConsumer(SendPaymentApprovedEmailUseCase useCase)
        {
            _useCase = useCase; 
        }

        public async Task Consume(ConsumeContext<OrderApprovedEvent> context)
        {
            var mensagem = context.Message;

            var command = new SendPaymentApprovedEmailCommand(context.MessageId ?? Guid.NewGuid(),UsuarioId: mensagem.UserId,
                OrderId: mensagem.OrderId, NomeUsuario: mensagem.NomeUsuario, Email: mensagem.EmailUsuario,
                DataAquisicao: mensagem.DataCompra);

            await _useCase.ExecuteAsync(command,context.CancellationToken);
        }
    }
}
