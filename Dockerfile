FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release


WORKDIR /src

COPY ["nuget.config", "."]

COPY ["src/Fcg.Notification.API/Fcg.Notification.API.csproj", "Fcg.Notification.API/"]
COPY ["src/Fcg.Notification.Application/Fcg.Notification.Application.csproj", "Fcg.Notification.Application/"]
COPY ["src/Fcg.Notification.Domain/Fcg.Notification.Domain.csproj", "Fcg.Notification.Domain/"]
COPY ["src/Fcg.Notification.Infrastructure/Fcg.Notification.Infrastructure.csproj", "Fcg.Notification.Infrastructure/"]
RUN --mount=type=secret,id=github_token \
    export GITHUB_TOKEN=$(cat /run/secrets/github_token) && \
    dotnet restore "./Fcg.Notification.API/Fcg.Notification.API.csproj"


COPY src/ .
WORKDIR "/src/Fcg.Notification.API"
RUN dotnet build "./Fcg.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
RUN dotnet publish "./Fcg.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Fcg.Notification.API.dll"]