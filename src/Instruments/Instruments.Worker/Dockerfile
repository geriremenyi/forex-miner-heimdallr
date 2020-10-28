FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Instruments/Instruments.Worker/Instruments.Worker.csproj", "Instruments/Instruments.Worker/"]
COPY ["Common/Common.Data/Common.Data.csproj", "Common/Common.Data/"]
COPY ["Instruments/Instruments.Storage/Instruments.Storage.csproj", "Instruments/Instruments.Storage/"]
RUN dotnet restore "Instruments/Instruments.Worker/Instruments.Worker.csproj"
COPY . .
WORKDIR "/src/Instruments/Instruments.Worker"
RUN dotnet build "Instruments.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Instruments.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForexMiner.Heimdallr.Instruments.Worker.dll"]