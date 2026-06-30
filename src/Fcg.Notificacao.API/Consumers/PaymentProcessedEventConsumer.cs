using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Notificacao.Domain.Common.Interfaces;
using MassTransit;

namespace Fcg.Notificacao.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<OrderApprovedEvent>
    {
        private readonly IEmailService _fakeEmailService;

        public PaymentProcessedEventConsumer(IEmailService fakeEmailService)
        {
            _fakeEmailService = fakeEmailService;
        }

        public async Task Consume(ConsumeContext<OrderApprovedEvent> context)
        {
            var mensagem = context.Message;
            await _fakeEmailService.SendApprovedPaymentEmail(mensagem.UserId,mensagem.OrderId,mensagem.NomeUsuario,
                mensagem.EmailUsuario,mensagem.CreatedAt);
        }
    }
}
