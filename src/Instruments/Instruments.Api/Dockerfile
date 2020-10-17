FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Instruments/Instruments.Api/Instruments.Api.csproj", "Instruments/Instruments.Api/"]
COPY ["Common/Common.Data/Common.Data.csproj", "Common/Common.Data/"]
COPY ["Common/Common.ServiceConfiguration/Common.ServiceConfiguration.csproj", "Common/Common.ServiceConfiguration/"]
RUN dotnet restore "Instruments/Instruments.Api/Instruments.Api.csproj"
COPY . .
WORKDIR "/src/Instruments/Instruments.Api"
RUN dotnet build "Instruments.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Instruments.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForexMiner.Heimdallr.Instruments.Api.dll"]