FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ConfigurationManager/ConfigurationManager.csproj", "ConfigurationManager/"]
COPY ["Cache/Cache.csproj", "Cache/"]
COPY ["Data/Data.csproj", "Data/"]
RUN dotnet restore "ConfigurationManager/ConfigurationManager.csproj"
COPY . .
WORKDIR "/src/ConfigurationManager"
RUN dotnet build "ConfigurationManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConfigurationManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForexMiner.Heimdallr.ConfigurationManager.dll"]