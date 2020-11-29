FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Deployer.Webhook.Api/Deployer.Webhook.Api.csproj", "Deployer.Webhook.Api/"]
RUN dotnet restore "Deployer.Webhook.Api/Deployer.Webhook.Api.csproj"
COPY . .
WORKDIR "/src/Deployer.Webhook.Api"
RUN dotnet build "Deployer.Webhook.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Deployer.Webhook.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Deployer.Webhook.Api.dll"]
