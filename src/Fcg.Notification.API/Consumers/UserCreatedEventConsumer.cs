using Fcg.Core.SharedContracts.MessageContracts;
using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.UseCase.WelcomeEmail;
using MassTransit;

namespace Fcg.Notification.API.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly ISendWelcomeEmailUseCase _sendWelcomeEmailUseCase;

        public UserCreatedEventConsumer(ISendWelcomeEmailUseCase sendWelcomeEmailUseCase)
        {
            _sendWelcomeEmailUseCase = sendWelcomeEmailUseCase;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var mensagem = context.Message;

            var command = new SendWelcomeEmailCommand(
                EventId: mensagem.EventId,
                UserId: mensagem.UserId,
                Email: mensagem.Email,
                UserName: mensagem.Name);

            await _sendWelcomeEmailUseCase.ExecuteAsync(command,context.CancellationToken);   
        }
    }
}
