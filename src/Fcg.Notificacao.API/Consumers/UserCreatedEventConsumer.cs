using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Notificacao.Domain.Common.Interfaces;
using MassTransit;

namespace Fcg.Notificacao.API.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IEmailService _emailService;

        public UserCreatedEventConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var mensagem = context.Message;
            await _emailService.SendWelcomeEmailAsync(mensagem.UserId, mensagem.Name, mensagem.Email);
        }
    }
}
