FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["UserManager.Services/UserManager.Service.csproj", "UserManager.Services/"]
COPY ["UserManager.Model/UserManager.Model.csproj", "UserManager.Model/"]
RUN dotnet restore "UserManager.Services/UserManager.Service.csproj"
COPY . .
WORKDIR "/src/UserManager.Services"
RUN dotnet build "UserManager.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserManager.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManager.Service.dll"]