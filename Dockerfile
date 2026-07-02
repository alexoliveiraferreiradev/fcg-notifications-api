FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["src/Fcg.Notification.API/Fcg.Notification.API.csproj", "src/Fcg.Notification.API/"]
COPY ["src/Fcg.Notification.Application/Fcg.Notification.Application.csproj", "src/Fcg.Notification.Application/"]
COPY ["src/Fcg.Notification.Domain/Fcg.Notification.Domain.csproj", "src/Fcg.Notification.Domain/"]
COPY ["src/Fcg.Notification.Infrastructure/Fcg.Notification.Infrastructure.csproj", "src/Fcg.Notification.Infrastructure/"]


RUN dotnet restore "./src/Fcg.Notification.API/Fcg.Notification.API.csproj"


COPY . .
WORKDIR "/src/src/Fcg.Notification.API"


RUN dotnet build "./Fcg.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fcg.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app


COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "Fcg.Notification.API.dll"]
