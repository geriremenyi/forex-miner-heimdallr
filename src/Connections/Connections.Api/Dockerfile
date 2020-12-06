FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Connections/Connections.Api/Connections.Api.csproj", "Connections/Connections.Api/"]
COPY ["Connections/Connections.Secret/Connections.Secret.csproj", "Connections/Connections.Secret/"]
COPY ["Common/Common.Caching/Common.Caching.csproj", "Common/Common.Caching/"]
COPY ["Common/Common.Extensions/Common.Extensions.csproj", "Common/Common.Extensions/"]
COPY ["Common/Common.Data/Common.Data.csproj", "Common/Common.Data/"]
RUN dotnet restore "Connections/Connections.Api/Connections.Api.csproj"
COPY . .
WORKDIR "/src/Connections/Connections.Api"
RUN dotnet build "Connections.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Connections.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForexMiner.Heimdallr.Connections.Api.dll"]