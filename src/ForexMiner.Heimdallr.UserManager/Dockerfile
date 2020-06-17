FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ForexMiner.Heimdallr.UserManager/ForexMiner.Heimdallr.UserManager.csproj", "ForexMiner.Heimdallr.UserManager/"]
COPY ["ForexMiner.Heimdallr.Contracts/ForexMiner.Heimdallr.Contracts.csproj", "ForexMiner.Heimdallr.Contracts/"]
RUN dotnet restore "ForexMiner.Heimdallr.UserManager/ForexMiner.Heimdallr.UserManager.csproj"
COPY . .
WORKDIR "/src/ForexMiner.Heimdallr.UserManager"
RUN dotnet build "ForexMiner.Heimdallr.UserManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ForexMiner.Heimdallr.UserManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForexMiner.Heimdallr.UserManager.dll"]
