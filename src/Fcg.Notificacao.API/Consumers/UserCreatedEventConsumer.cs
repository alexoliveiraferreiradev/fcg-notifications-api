using Fcg.Core.Abstractions.MessageContracts;
using Fcg.Notificacao.Application.UseCase.WelcomeEmail;
using MassTransit;

namespace Fcg.Notificacao.API.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly SendWelcomeEmailUseCase _useCase;

        public UserCreatedEventConsumer(SendWelcomeEmailUseCase useCase)
        {
            _useCase = useCase; 
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var mensagem = context.Message;

            var command = new SendWelcomeEmailCommand(
                EventId: context.MessageId ?? Guid.NewGuid(),
                mensagem.UserId, mensagem.Email, mensagem.Name);

            await _useCase.ExecuteAsync(command,context.CancellationToken);   
        }
    }
}
