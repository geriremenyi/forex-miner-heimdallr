FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Configurations/Configurations.Api/Configurations.Api.csproj", "Configurations.Api/"]
RUN dotnet restore "Configurations/Configurations.Api/Configurations.Api.csproj"
COPY . .
WORKDIR "/src/Configurations/Configurations.Api"
RUN dotnet build "Configurations.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Configurations.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForexMiner.Heimdallr.Configurations.Api.dll"]