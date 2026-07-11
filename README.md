# Fiap Cloud Games (FCG) - Notificação API

Este microsserviço de suporte é responsável pelo **envio simulado de e-mails** e comunicação com o cliente da plataforma **Fiap Cloud Games (FCG)**, utilizando uma arquitetura orientada a eventos. O serviço consome de forma assíncrona eventos de outros microsserviços (Identidade/Usuários e Pagamentos) para processar e logar no console mensagens transacionais, como boas-vindas e confirmações de compra.

---

## 🛠️ Tecnologias e Bibliotecas

A API faz uso das seguintes tecnologias e pacotes:

- **.NET 8.0**: Plataforma de desenvolvimento principal.
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

O microsserviço atua como um *subscriber* assíncrono no RabbitMQ e reage aos seguintes eventos:

1. **`UserCreatedEvent` (Cadastro de Usuário):** 
   - Publicado pelo `Users API` ao registrar uma nova conta.
   - O `Notifications API` consome o evento e simula o disparo de um e-mail de boas-vindas com as informações do novo usuário.
2. **`PaymentProcessedEvent` (Processamento de Pagamento):**
   - Publicado pelo `Payments API` ao finalizar o status da transação.
   - Caso o pagamento seja aprovado (`Approved`), o `Notifications API` consome o evento e simula o disparo de um e-mail de confirmação de compra do jogo.

---

## ⚙️ Configuração e Variáveis de Ambiente

Para o funcionamento correto da API de Notificações, certas variáveis de ambiente de banco de dados, mensageria e cache devem ser fornecidas dependendo do ambiente de execução.

### 1. Execução Local Standalone (Desenvolvimento)
Quando executada diretamente pela IDE ou linha de comando `dotnet run`, a API consome as configurações definidas no arquivo [appsettings.json](src/Fcg.Notification.API/appsettings.json) ou `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "RabbitMq": "amqp://guest:guest@localhost:5672",
    "Redis": "localhost:6379"
  }
}
```

---

### 2. Execução via Docker Compose
Ao rodar através do contêiner Docker configurado no repositório de orquestração (`fcg-infrastructure`), as seguintes variáveis de ambiente são injetadas no contêiner:

| Variável | Valor Padrão/Exemplo | Descrição |
| :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | `Development` | Define o ambiente de execução da aplicação. |
| `ConnectionStrings__RabbitMq` | `rabbitmq` | Host do RabbitMQ para consumo de eventos. |
| `ConnectionStrings__Redis` | `redis:6379` | Host do cache Redis (usado para idempotência). |

---

### 3. Execução no Kubernetes (ConfigMaps e Secrets)
No Kubernetes, as configurações são abstraídas em manifestos separados para dados não-sensíveis (ConfigMaps) e dados sensíveis (Secrets):

#### **ConfigMap: `notification-config`**
Armazena dados não sensíveis configurados no arquivo [configmap.yaml](k8s/configmap.yaml):
- `RABBITMQ_SERVER`: Nome do serviço DNS do RabbitMQ no cluster (Ex: `rabbitmq-service`).
- `RABBITMQ_PORT`: Porta TCP do RabbitMQ (Ex: `"5672"`).
- `NOTIFICATION_USER_CREATED_QUEUE`: Fila para evento de criação de usuário (Ex: `"notifications-user-created"`).
- `NOTIFICATION_PAYMENT_FAILED_QUEUE`: Fila para falha de pagamentos (Ex: `"notifications-payment-failed"`).
- `NOTIFICATION_PAYMENT_PROCESSED_QUEUE`: Fila para pagamento processado (Ex: `"notifications-payment-processed"`).
- `NOTIFICATION_DELIVERY_FAILED_QUEUE`: Fila de falha de entrega (Ex: `"notification-delivery-failed"`).
- `REDIS_SERVER`: Nome do serviço DNS do Redis (Ex: `redis-service`).
- `REDIS_PORT`: Porta do cache Redis (Ex: `"6379"`).
- `REDIS_NAME`: Prefixo/Name de identificador no Redis (Ex: `"FiapCloudGames:"`).
- `ENVIRONMENT`: Variável `ASPNETCORE_ENVIRONMENT` (Ex: `"Development"`).

#### **Secret: `notification-opaque`**
Armazena credenciais confidenciais codificadas em Base64 configuradas no arquivo [secret.yaml](k8s/secret.yaml):
- `RABBITMQ_USER`: Usuário do RabbitMQ (Ex: `guest` -> Base64 `Z3Vlc3Q=`).
- `RABBITMQ_PASS`: Senha do RabbitMQ (Ex: `guest` -> Base64 `Z3Vlc3Q=`).
- `REDIS_PASS`: Senha do banco de cache Redis (Ex: Base64 `VGVjaENoYWxsZW5nZUAyMDI2`).

---

## 🚀 Como Executar Localmente (Standalone)

### Pré-requisitos
- SDK do [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.
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
