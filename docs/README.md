# Fiap Cloud Games (FCG) - Notificação API

Microsserviço de suporte focado na comunicação com o cliente através de uma arquitetura orientada a eventos. O serviço escuta eventos de diversos domínios da aplicação para processar o envio simulado de e-mails, como mensagens de boas-vindas e confirmações de aprovação de pagamento.

## 🚀 Tecnologias e Ferramentas

O projeto foi desenvolvido utilizando as seguintes tecnologias:

- **.NET 8 (C#)**
- **Arquitetura Limpa (Clean Architecture)**
- **MassTransit & RabbitMQ**: Para mensageria e consumo de eventos de forma assíncrona.
- **Redis**: Utilizado para controle de idempotência e cache.
- **Serilog**: Para registro de logs estruturados (console e arquivo).
- **Health Checks**: Endpoints de *Liveness* e *Readiness* para monitoramento contínuo do estado da aplicação e de suas dependências.

## 📦 Estrutura do Projeto

A solução foi estruturada seguindo os princípios de *Clean Architecture*, garantindo baixo acoplamento e alta coesão, dividida nos seguintes projetos:

- **API (`Fcg.Notification.API`)**: Camada de apresentação que hospeda os *Consumers* (consumidores de fila), configurações de *middlewares* e injeção de dependência.
- **Application (`Fcg.Notification.Application`)**: Contém a lógica de orquestração e os casos de uso, como envio de e-mails (`SendWelcomeEmailUseCase`, `SendPaymentApprovedEmailUseCase`).
- **Domain (`Fcg.Notification.Domain`)**: Núcleo do sistema, contendo as entidades, interfaces e regras de negócio.
- **Infrastructure (`Fcg.Notification.Infrastructure`)**: Implementação de detalhes técnicos, como persistência, serviços de cache (Redis) e manipulação de idempotência.

## ⚙️ Eventos Consumidos

O serviço atua como um *subscriber* no RabbitMQ e reage aos seguintes eventos:

1. **`UserCreatedEvent`**: Acionado sempre que um novo usuário é registrado no domínio de Identidade/Usuário. O microsserviço reage a este evento disparando um e-mail de boas-vindas.
2. **`PaymentProcessedEvent`**: Acionado sempre que um pagamento é processado. Caso o status do pagamento seja `Approved` (Aprovado), o serviço dispara um e-mail confirmando o sucesso da transação para o cliente.

## 🛠️ Configuração e Execução

### Pré-requisitos

Para rodar o projeto localmente, é necessário ter:
- .NET 8 SDK
- Instância do RabbitMQ em execução
- Instância do Redis em execução

### Variáveis de Ambiente / `appsettings.json`

Configure as dependências no arquivo `appsettings.json` do projeto API (ou através de variáveis de ambiente):

```json
{
  "ConnectionStrings": {
    "RabbitMq": "amqp://guest:guest@localhost:5672",
    "Redis": "localhost:6379"
  }
}
```

### Endpoints de Saúde (Health Checks)

O microsserviço expõe rotas nativas para checagem de saúde e integração com orquestradores (como Kubernetes):

- **Liveness**: `/health/liveness` (Informa se o pod/aplicação está rodando).
- **Readiness**: `/health/readiness` (Testa a conectividade real com os serviços dependentes, como Redis e RabbitMQ).

### Docker

O projeto já contempla um `Dockerfile` na raiz do repositório para conteinerização e deploy da API.
