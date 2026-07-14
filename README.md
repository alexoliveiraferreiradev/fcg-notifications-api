# Fiap Cloud Games (FCG) - Notificação API

Este microsserviço de suporte é responsável pelo **envio simulado de e-mails** e comunicação com o cliente da plataforma **Fiap Cloud Games (FCG)**, utilizando uma arquitetura orientada a eventos. O serviço consome de forma assíncrona eventos de outros microsserviços (Identidade/Usuários e Pagamentos) para processar e logar no console mensagens transacionais, como boas-vindas e confirmações de compra.

---

## 🛠️ Tecnologias e Bibliotecas

A API faz uso das seguintes tecnologias e pacotes:

- **.NET 9.0**: Plataforma de desenvolvimento principal.
- **MassTransit & RabbitMQ**: Biblioteca/Framework de mensageria para consumo assíncrona de eventos.
- **Redis**: Armazenamento em cache e controle de idempotência para processamento de eventos.
- **Serilog**: Para registro de logs estruturados (console e arquivos).
- **xUnit**: Framework para testes automatizados.

---

## 🏗️ Arquitetura da Solução

O projeto está estruturado segundo os princípios de **Clean Architecture**, dividindo responsabilidades de forma desacoplada:

```
src/
├── Fcg.Notification.Domain         # Regras de Domínio, Interfaces e Entidades de Mensagens
├── Fcg.Notification.Application    # Casos de Uso (ex: SendWelcomeEmailUseCase, SendPaymentApprovedEmailUseCase)
├── Fcg.Notification.Infrastructure # Serviços de Persistência, Cache (Redis) e Idempotência
└── Fcg.Notification.API            # Host HTTP, Middlewares, Consumers do MassTransit e Program.cs
```

---

## 📐 Fluxo de Integração e Eventos Consumidos

O microsserviço atua como um *subscriber* assíncrono no RabbitMQ e reage aos seguintes eventos da arquitetura:

1. **`UserCreatedEvent` (Cadastro de Usuário):** 
   - Publicado pelo `Users API` ao registrar uma nova conta.
   - O `Notifications API` consome o evento e simula o disparo de um e-mail de boas-vindas com as informações do novo usuário.

2. **`PaymentProcessedEvent` (Pagamento Aprovado):**
   - Publicado pelo `Payments API` ao aprovar e finalizar a transação financeira com sucesso.
   - O `Notifications API` consome o evento e simula o disparo de um e-mail com o recibo e a confirmação de liberação do jogo.

3. **`PaymentFailedEvent` (Pagamento Rejeitado):**
   - Publicado pelo `Payments API` caso o pagamento seja recusado.
   - O `Notifications API` consome o evento e alerta o usuário através de um e-mail simulado informando a falha do pedido.

4. **`DeliveryFailedEvent` (Falha na Entrega do Jogo):**
   - Publicado pelo `Catalog API` em caso de erro sistêmico ao tentar adicionar o jogo à biblioteca após um pagamento aprovado.
   - O `Notifications API` consome o evento para disparar um e-mail de desculpas, notificando o estorno imediato do pagamento.

---

## ⚙️ Configuração e Variáveis de Ambiente

Para o funcionamento correto da API de Notificações, certas variáveis de ambiente de banco de dados, mensageria e cache devem ser fornecidas dependendo do ambiente de execução.

```json
{
  "RabbitMqSettings": {
    "Host": "localhost",
    "Port": "5672",
    "Username": "guest",
    "Password": "guest",
    "NotificationUserCreatedQueue": "notifications-user-created",
    "NotificationPaymentFailedQueue": "notifications-payment-failed",
    "NotificationPaymentProcessedQueue": "notifications-payment-processed",
    "NotificationDeliveryFailedQueue": "notification-delivery-failed"
  },
  "RedisSettings": {
    "Host": "localhost",
    "Port": "6379",
    "Password": "SuaSenhaSegura",
    "InstanceName": "FiapCloudGames:",
    "ExpirationInDays": 3
  }
}
```

---

## 🚀 Como Executar Localmente (Standalone)

### Pré-requisitos
- SDK do [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
- RabbitMQ e Redis acessíveis.

### Comandos de Terminal

1. **Restaurar Dependências e Compilar:**
   ```bash
   dotnet restore
   dotnet build
   ```

2. **Executar a API:**
   ```bash
   dotnet run --project src/Fcg.Notification.API/
   ```

---

## 🐳 Construção da Imagem Docker

Para validar e construir a imagem Docker do microsserviço de forma isolada, provando o funcionamento de sua conteinerização e empacotamento adequados, execute o seguinte comando a partir da raiz deste repositório:

```bash
docker build -t fcg-notifications-api .
```

---

## ☸️ Como Implantar no Kubernetes (k8s)

Os manifestos na pasta `/k8s` estão prontos para implantação local. Para aplicar todas as configurações, serviços de rede e deployments do microsserviço de notificações no cluster local configurado, execute:

```bash
kubectl apply -f k8s/
```

### Validação dos Recursos
Para validar se a implantação foi bem-sucedida, você pode executar:
```bash
# Verificar status dos pods (deve constar como Running)
kubectl get pods

# Verificar serviços expostos
kubectl get services
```

*Nota: Por padrão, o serviço de notificações é exposto internamente no cluster como **ClusterIP** sob o nome `svc-fcg-notification-api` na porta `8082`.*

### 🏥 Resiliência e Probes de Saúde (Health Checks)

A API expõe endpoints nativos de verificação de integridade operacional de suas dependências externas (como conectividade com Redis e RabbitMQ):
- **Liveness Probe (`/health/liveness`)**: Usada pelo Kubernetes para assegurar que a API está ativa.
- **Readiness Probe (`/health/readiness`)**: Testada pelo Kubernetes para validar se o microsserviço estabeleceu conexão com o broker RabbitMQ e o cache Redis antes de expor o pod ao recebimento de mensagens e tráfego.

Esses endpoints estão totalmente integrados nos manifestos de implantação (`k8s/deployment.yaml`) para garantir a resiliência e estabilidade da orquestração.

---

## 🧪 Testes Automatizados

Para rodar todos os testes unitários e de integração desenvolvidos para o microsserviço de notificações, execute a partir do diretório raiz:
```bash
dotnet test
```
