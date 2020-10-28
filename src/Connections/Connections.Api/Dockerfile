#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Connections/Connections.Api/Connections.Api.csproj", "Connections/Connections.Api/"]
RUN dotnet restore "Connections/Connections.Api/Connections.Api.csproj"
COPY . .
WORKDIR "/src/Connections/Connections.Api"
RUN dotnet build "Connections.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Connections.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Connections.Api.dll"]