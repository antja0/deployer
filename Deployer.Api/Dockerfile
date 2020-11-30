FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Deployer.Api/Deployer.Api.csproj", "Deployer.Api/"]
RUN dotnet restore "Deployer.Api/Deployer.Api.csproj"
COPY . .
WORKDIR "/src/Deployer.Api"
RUN dotnet build "Deployer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Deployer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Deployer.Api.dll"]
