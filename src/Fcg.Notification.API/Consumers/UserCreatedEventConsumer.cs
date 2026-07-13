using Fcg.Core.Abstractions.MessageContracts;
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
                EventId: context.MessageId ?? Guid.NewGuid(),
                mensagem.UserId, mensagem.Email, mensagem.Name);

            await _sendWelcomeEmailUseCase.ExecuteAsync(command,context.CancellationToken);   
        }
    }
}
